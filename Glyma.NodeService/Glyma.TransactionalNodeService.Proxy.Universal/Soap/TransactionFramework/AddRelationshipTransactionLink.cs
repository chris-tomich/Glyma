using System;
using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal.Service;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework
{
    public class AddRelationshipTransactionLink : ISoapTransactionLink, ISoapTransactionLinkExecutor
    {
        private bool _onCompletedBound = false;
        private IRelationship _inProcessResponse = null;
        private ISoapTransactionLink _firstLink = null;
        private SoapTransactionLinkParameter _domainParameter = null;
        private SoapTransactionLinkParameter _rootMapParameter = null;
        private Dictionary<ConnectionType, INode> _proxyNodeConnections = null;
        private Dictionary<Service.DT, SoapTransactionLinkParameter> _transactionLinkConnections = null;

        public AddRelationshipTransactionLink()
        {
        }

        private event EventHandler InternalTransactionFailed;

        event EventHandler ISoapTransactionLinkExecutor.TransactionFailed
        {
            add { InternalTransactionFailed += value; }
            remove { InternalTransactionFailed -= value; }
        }

        private event EventHandler<Service.CompleteTransactionCompletedEventArgs> InternalTransactionCompleted;

        event EventHandler<Service.CompleteTransactionCompletedEventArgs> ISoapTransactionLinkExecutor.TransactionCompleted
        {
            add { InternalTransactionCompleted += value; }
            remove { InternalTransactionCompleted -= value; }
        }

        private Guid SessionId
        {
            get;
            set;
        }

        private TransactionalMappingToolServiceClient ServiceProxy
        {
            get;
            set;
        }

        private Dictionary<ConnectionType, INode> ProxyNodeConnections
        {
            get
            {
                if (_proxyNodeConnections == null)
                {
                    _proxyNodeConnections = new Dictionary<ConnectionType, INode>();
                }

                return _proxyNodeConnections;
            }
        }

        private Dictionary<Service.DT, SoapTransactionLinkParameter> TransactionLinkConnections
        {
            get
            {
                if (_transactionLinkConnections == null)
                {
                    _transactionLinkConnections = new Dictionary<Service.DT, SoapTransactionLinkParameter>();
                }

                return _transactionLinkConnections;
            }
        }

        public int TransactionLinkId
        {
            get;
            set;
        }

        public Service.MP ResponseParameter
        {
            get;
            private set;
        }

        public Service.MapParameterType ResponseParameterType
        {
            get
            {
                return Service.MapParameterType.Relationship;
            }
        }

        private SoapTransactionLinkParameter DomainParameter
        {
            get
            {
                if (_domainParameter == null)
                {
                    _domainParameter = new SoapTransactionLinkParameter(Service.MapParameterType.Domain);
                }

                _domainParameter.SetParameterValue(DomainId);

                return _domainParameter;
            }
        }

        private SoapTransactionLinkParameter RootMapParameter
        {
            get
            {
                if (_rootMapParameter == null)
                {
                    _rootMapParameter = new SoapTransactionLinkParameter(Service.MapParameterType.RootMap);
                }

                _rootMapParameter.SetParameterValue(RootMapId);

                return _rootMapParameter;
            }
        }

        public Guid DomainId
        {
            get;
            set;
        }

        public Guid RootMapId
        {
            get;
            set;
        }

        public string OriginalId
        {
            get;
            set;
        }

        public IMapManager MapManager
        {
            get;
            set;
        }

        public RelationshipType RelationshipType
        {
            get;
            set;
        }

        public ServerStatus TransactionStatus
        {
            get;
            private set;
        }

        public void Reset()
        {
            _onCompletedBound = false;
            _domainParameter = null;
            _rootMapParameter = null;

            if (_inProcessResponse != null)
            {
                FacadeRelationship facadeRelationship = _inProcessResponse as FacadeRelationship;

                if (facadeRelationship != null)
                {
                    facadeRelationship.ResetToFacade();
                }
            }

            ResponseParameter = null;

            TransactionStatus = ServerStatus.ProcessingClient;

            if (_firstLink != null)
            {
                ISoapTransactionLinkExecutor executor = _firstLink as ISoapTransactionLinkExecutor;
                executor.TransactionFailed -= OnTransactionFailed;
                executor.TransactionCompleted -= OnTransactionCompleted;
            }

            if (ServiceProxy != null)
            {
                ServiceProxy.BOCCompleted -= OnSubmitBulkOperationCompleted;
                ServiceProxy.RACompleted -= OnAddRelationshipCompleted;
            }
        }

        public void AddNode(ConnectionType connectionType, INode node)
        {
            ProxyNodeConnections[connectionType] = node;

            Service.DT descriptorType = MapManager.ConnectionTypes.ConvertProxyToService(connectionType);

            SoapTransactionLinkParameter linkParameter = new SoapTransactionLinkParameter(Service.MapParameterType.Node);

            FacadeNode facadeNode = node as FacadeNode;

            // Check if this is a facade and if it is and the base node isn't concrete, then use the base node's response parameter.
            if (facadeNode != null && !facadeNode.IsConcrete)
            {
                linkParameter.SetParameterValue(facadeNode.TransactionOrigin);
            }
            else
            {
                linkParameter.SetParameterValue(node.Id);
            }

            TransactionLinkConnections[descriptorType] = linkParameter;
        }

        public void AddNextLink(ISoapTransactionLink nextLink)
        {
            _firstLink = nextLink;
        }

        public IRelationship CreateInProcessObjects()
        {
            if (_inProcessResponse == null)
            {
                _inProcessResponse = MapManager.RelationshipFactory.CreateRelationship(this, DomainParameter.GetParameterValue(Guid.Empty).V, RootMapParameter.GetParameterValue(Guid.Empty).V, ProxyNodeConnections, RelationshipType, OriginalId);
                TransactionStatus = ServerStatus.ProcessingClient;
                FacadeRelationship facadeRelationship = _inProcessResponse as FacadeRelationship;

                if (facadeRelationship != null)
                {
                    facadeRelationship.TransactionOrigin = this;
                    _inProcessResponse = facadeRelationship;
                }
            }

            return _inProcessResponse;
        }

        private DependencyCollection _dependencies = null;

        DependencyCollection ISoapTransactionLinkExecutor.Dependencies
        {
            get
            {
                _dependencies = _dependencies ?? new DependencyCollection();

                return _dependencies;
            }
        }

        DependencyCollection ISoapTransactionLinkExecutor.UpdateDependencies()
        {
            ISoapTransactionLinkExecutor executor = (ISoapTransactionLinkExecutor)this;

            foreach (KeyValuePair<ConnectionType, INode> nodePairs in ProxyNodeConnections)
            {
                IFacade iFacadeNode = nodePairs.Value as IFacade;

                if (iFacadeNode != null)
                {
                    if (!iFacadeNode.IsConcrete)
                    {
                        FacadeNode facadeNode = iFacadeNode as FacadeNode;
                        InProcess.InProcessNode inProcessNode = facadeNode.BaseNode as InProcess.InProcessNode;

                        if (inProcessNode != null && inProcessNode.OriginLink != null && inProcessNode.OriginLink.OriginChain != OriginChain)
                        {
                            executor.Dependencies.AddFacade(iFacadeNode);
                        }
                    }
                }
            }

            return executor.Dependencies;
        }

        void ISoapTransactionLinkExecutor.ExecuteTransaction(Guid sessionId, TransactionalMappingToolServiceClient serviceProxy)
        {
            TransactionStatus = ServerStatus.SendingTransaction;
            SessionId = sessionId;
            ServiceProxy = serviceProxy;

            Dictionary<Service.DT, Service.MP> nodeConnections = new Dictionary<Service.DT, Service.MP>();

            foreach (KeyValuePair<Service.DT, SoapTransactionLinkParameter> keyValuePair in TransactionLinkConnections)
            {
                Service.DT descriptorType = keyValuePair.Key;
                Service.MP parameter = keyValuePair.Value.GetParameterValue(sessionId);

                nodeConnections[descriptorType] = parameter;
            }

            ServiceProxy.RACompleted += OnAddRelationshipCompleted;
            ServiceProxy.RAAsync(CallingUrl.Url, SessionId, DomainParameter.GetParameterValue(sessionId), RootMapParameter.GetParameterValue(sessionId), nodeConnections, MapManager.RelationshipTypes.ConvertProxyToService(RelationshipType), OriginalId, this);
        }

        void ISoapTransactionLinkExecutor.ExecuteBulkTransactions(Guid sessionId, TransactionalMappingToolServiceClient serviceProxy, ref Service.OPS bulkOperations)
        {
            TransactionStatus = ServerStatus.SendingTransaction;
            SessionId = sessionId;
            ServiceProxy = serviceProxy;

            Dictionary<Service.DT, Service.MP> nodeConnections = new Dictionary<Service.DT, Service.MP>();

            foreach (KeyValuePair<Service.DT, SoapTransactionLinkParameter> keyValuePair in TransactionLinkConnections)
            {
                Service.DT descriptorType = keyValuePair.Key;
                Service.MP parameter = keyValuePair.Value.GetParameterValue(sessionId);

                nodeConnections[descriptorType] = parameter;
            }

            ResponseParameter = new Service.MP();
            ResponseParameter.I = Guid.NewGuid();
            ResponseParameter.S = sessionId;
            ResponseParameter.V = Guid.Empty;
            ResponseParameter.D = true;
            ResponseParameter.T = Service.MapParameterType.Node;

            Service.BRA relationshipBulkOperation = new Service.BRA();
            relationshipBulkOperation.I = TransactionLinkId;
            relationshipBulkOperation.D = DomainParameter.GetParameterValue(sessionId);
            relationshipBulkOperation.RM = RootMapParameter.GetParameterValue(sessionId);
            relationshipBulkOperation.N = nodeConnections;
            relationshipBulkOperation.T = MapManager.RelationshipTypes.ConvertProxyToService(RelationshipType);
            relationshipBulkOperation.O = OriginalId;
            relationshipBulkOperation.RI = ResponseParameter.I;

            bulkOperations.O.Add(relationshipBulkOperation);

            ISoapTransactionLinkExecutor executor = _firstLink as ISoapTransactionLinkExecutor;

            if (executor == null)
            {
                DebugLogger.Instance.LogMsg("Exception: This is not a supported type of link. This link does not implement ISoapTransactionLinkExecutor.");
                throw new NotSupportedException("This is not a supported type of link. This link does not implement ISoapTransactionLinkExecutor.");
            }

            if (bulkOperations.O.Count < 200)
            {
                TransactionStatus = ServerStatus.TransactionReceived;

                if (!_onCompletedBound)
                {
                    executor.TransactionFailed += OnTransactionFailed;
                    executor.TransactionCompleted += OnTransactionCompleted;

                    _onCompletedBound = true;
                }

                TransactionStatus = ServerStatus.TransactionExecuting;

                DebugLogger.Instance.LogMsg("Executing add relationship transaction. ClientId - '{0}';", CreateInProcessObjects().ClientId);
                executor.ExecuteBulkTransactions(SessionId, ServiceProxy, ref bulkOperations);
            }
            else
            {
                ServiceProxy.BOCCompleted += new EventHandler<BOCCompletedEventArgs>(OnSubmitBulkOperationCompleted);
                ServiceProxy.SubmitBulkOperationsAsync(CallingUrl.Url, SessionId, bulkOperations, this);
            }
        }

        void OnSubmitBulkOperationCompleted(object sender, BOCCompletedEventArgs e)
        {
            if (e.UserState != this)
            {
                // This event is for a different object so don't execute.
                return;
            }

            TransactionStatus = ServerStatus.TransactionReceived;

            // The following lines must be done so that the transaction isn't executed again.
            ServiceProxy.BOCCompleted -= OnSubmitBulkOperationCompleted;

            if (e.Error != null)
            {
                TransactionStatus = ServerStatus.TransactionFailed;

                if (InternalTransactionFailed != null)
                {
                    InternalTransactionFailed(this, e);
                }

                return;
            }

            OriginChain.FillBulkOperations(e.DecompressedResult);

            Service.OPS newBulkOperationsSet = new Service.OPS();
            newBulkOperationsSet.O = new System.Collections.ObjectModel.ObservableCollection<object>();

            ISoapTransactionLinkExecutor executor = _firstLink as ISoapTransactionLinkExecutor;

            if (!_onCompletedBound)
            {
                executor.TransactionFailed += OnTransactionFailed;
                executor.TransactionCompleted += OnTransactionCompleted;

                _onCompletedBound = true;
            }

            TransactionStatus = ServerStatus.TransactionExecuting;

            DebugLogger.Instance.LogMsg("Executing add relationship transaction. ResponseParameter Id - '{0}'; ClientId - '{1}';", ResponseParameter.I, CreateInProcessObjects().ClientId);
            executor.ExecuteBulkTransactions(SessionId, ServiceProxy, ref newBulkOperationsSet);
        }

        void ISoapTransactionLinkExecutor.ConsumeBulkOperationResponse(Service.BOR bulkOperationResponse)
        {
            if (bulkOperationResponse.I == TransactionLinkId)
            {
                ResponseParameter = bulkOperationResponse.R;
            }
            else
            {
                DebugLogger.Instance.LogMsg("The provided bulk operation response has an ID that does not match this transaction link.");
            }
        }

        private void OnAddRelationshipCompleted(object sender, Service.RACompletedEventArgs e)
        {
            if (e.UserState != this)
            {
                // This event is for a different object so don't execute.
                return;
            }

            TransactionStatus = ServerStatus.TransactionReceived;

            // The following line must be done so that the transaction isn't executed again.
            ServiceProxy.RACompleted -= OnAddRelationshipCompleted;

            if (e.Error != null)
            {
                TransactionStatus = ServerStatus.TransactionCompleted;

                Service.CompleteTransactionCompletedEventArgs completedEventArgs = new Service.CompleteTransactionCompletedEventArgs(null, e.Error, false, null);

                if (InternalTransactionCompleted != null)
                {
                    InternalTransactionCompleted(this, completedEventArgs);
                }

                return;
            }

            ResponseParameter = e.Result;

            ISoapTransactionLinkExecutor executor = _firstLink as ISoapTransactionLinkExecutor;

            if (executor == null)
            {
                DebugLogger.Instance.LogMsg("Exception: This is not a supported type of link. This link does not implement ISoapTransactionLinkExecutor.");
                throw new NotSupportedException("The next link in the chain is not a supported type of link or the next link is null. This link does not implement ISoapTransactionLinkExecutor.");
            }

            TransactionStatus = ServerStatus.TransactionExecuting;

            if (!_onCompletedBound)
            {
                executor.TransactionFailed += OnTransactionFailed;
                executor.TransactionCompleted += OnTransactionCompleted;

                _onCompletedBound = true;
            }

            DebugLogger.Instance.LogMsg("Executing add relationship transaction. ResponseParameter Id - '{0}'; ClientId - '{1}';", ResponseParameter.I, CreateInProcessObjects().ClientId);
            executor.ExecuteTransaction(SessionId, ServiceProxy);
        }

        private void OnTransactionFailed(object sender, System.EventArgs e)
        {
            if (InternalTransactionFailed != null)
            {
                System.Diagnostics.Debug.WriteLine("Failed");
                InternalTransactionFailed(sender, e);
            }
        }

        private void OnTransactionCompleted(object sender, Service.CompleteTransactionCompletedEventArgs e)
        {
            TransactionStatus = ServerStatus.TransactionCompleted;

            if (e.Error == null)
            {
                foreach (Service.CH change in e.Result.C)
                {
                    if (change.P == ResponseParameter.I)
                    {
                        if (change.R != null)
                        {
                            DebugLogger.Instance.LogMsg("Load relationship transaction completed. ResponseParameter Id - '{0}'; ClientId - '{1}'; Returned NodeUid - '{2}';", ResponseParameter.I, CreateInProcessObjects().ClientId, change.R.R);
                            ServerObjects.Relationship soRelationship = MapManager.ServerObjectConverter.ToRelationship(change.R);
                            MapManager.RelationshipFactory.UpgradeFacade(this, soRelationship);
                        }
                    }
                }
            }

            if (InternalTransactionCompleted != null)
            {
                InternalTransactionCompleted(this, e);
            }
        }


        public TransactionChain OriginChain
        {
            get;
            set;
        }
    }
}
