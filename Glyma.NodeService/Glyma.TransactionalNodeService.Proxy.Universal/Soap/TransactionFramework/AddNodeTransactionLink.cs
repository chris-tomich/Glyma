using System;
using TransactionalNodeService.Proxy.Universal.Service;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework
{
    public class AddNodeTransactionLink : ISoapTransactionLink, ISoapTransactionLinkExecutor
    {
        private bool _onCompletedBound = false;
        private INode _inProcessResponse = null;
        private ISoapTransactionLink _firstLink = null;
        private SoapTransactionLinkParameter _domainParameter = null;
        private SoapTransactionLinkParameter _rootMapParameter = null;

        public AddNodeTransactionLink()
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
                return Service.MapParameterType.Node;
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

        public NodeType NodeType
        {
            get;
            set;
        }

        public IMapManager MapManager
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
                FacadeNode facadeNode = _inProcessResponse as FacadeNode;

                if (facadeNode != null)
                {
                    facadeNode.ResetToFacade();
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
                ServiceProxy.NACompleted -= OnAddNodeCompleted;
            }
        }

        public void AddNextLink(ISoapTransactionLink nextLink)
        {
            _firstLink = nextLink;
        }

        public INode CreateInProcessObjects()
        {
            if (_inProcessResponse == null)
            {
                _inProcessResponse = MapManager.NodeFactory.CreateNode(this, DomainParameter.GetParameterValue(Guid.Empty).V, RootMapParameter.GetParameterValue(Guid.Empty).V, NodeType, OriginalId);
                TransactionStatus = ServerStatus.ProcessingClient;
                FacadeNode facadeNode = _inProcessResponse as FacadeNode;

                if (facadeNode != null)
                {
                    facadeNode.TransactionOrigin = this;
                    _inProcessResponse = facadeNode;
                }
            }

            return _inProcessResponse;
        }

        DependencyCollection ISoapTransactionLinkExecutor.Dependencies
        {
            get
            {
                return null;
            }
        }

        DependencyCollection ISoapTransactionLinkExecutor.UpdateDependencies()
        {
            return null;
        }

        void ISoapTransactionLinkExecutor.ExecuteTransaction(Guid sessionId, TransactionalMappingToolServiceClient serviceProxy)
        {
            TransactionStatus = ServerStatus.SendingTransaction;
            SessionId = sessionId;
            ServiceProxy = serviceProxy;

            ServiceProxy.NACompleted += OnAddNodeCompleted;
            ServiceProxy.NAAsync(CallingUrl.Url, SessionId, DomainParameter.GetParameterValue(sessionId), RootMapParameter.GetParameterValue(sessionId), MapManager.NodeTypes.ConvertProxyToService(NodeType), OriginalId, this);
        }

        void ISoapTransactionLinkExecutor.ExecuteBulkTransactions(Guid sessionId, TransactionalMappingToolServiceClient serviceProxy, ref Service.OPS bulkOperations)
        {
            TransactionStatus = ServerStatus.SendingTransaction;
            SessionId = sessionId;
            ServiceProxy = serviceProxy;

            ResponseParameter = new Service.MP();
            ResponseParameter.I = Guid.NewGuid();
            ResponseParameter.S = sessionId;
            ResponseParameter.V = Guid.Empty;
            ResponseParameter.D = true;
            ResponseParameter.T = Service.MapParameterType.Node;

            Service.BNA nodeBulkOperation = new Service.BNA();
            nodeBulkOperation.I = TransactionLinkId;
            nodeBulkOperation.D = DomainParameter.GetParameterValue(sessionId);
            nodeBulkOperation.RM = RootMapParameter.GetParameterValue(sessionId);
            nodeBulkOperation.T = MapManager.NodeTypes.ConvertProxyToService(NodeType);
            nodeBulkOperation.O = OriginalId;
            nodeBulkOperation.RI = ResponseParameter.I;

            bulkOperations.O.Add(nodeBulkOperation);

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

                DebugLogger.Instance.LogMsg("Executing add node transaction. ClientId - '{0}';", _inProcessResponse.ClientId);
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

            DebugLogger.Instance.LogMsg("Executing add node transaction. ResponseParameter Id - '{0}'; ClientId - '{1}';", ResponseParameter.I, _inProcessResponse.ClientId);
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

        private void OnAddNodeCompleted(object sender, Service.NACompletedEventArgs e)
        {
            if (e.UserState != this)
            {
                // This event is for a different object so don't execute.
                return;
            }

            TransactionStatus = ServerStatus.TransactionReceived;

            // The following lines must be done so that the transaction isn't executed again.
            ServiceProxy.NACompleted -= OnAddNodeCompleted;

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
                throw new NotSupportedException("This is not a supported type of link. This link does not implement ISoapTransactionLinkExecutor.");
            }

            TransactionStatus = ServerStatus.TransactionExecuting;

            if (!_onCompletedBound)
            {
                executor.TransactionFailed += OnTransactionFailed;
                executor.TransactionCompleted += OnTransactionCompleted;

                _onCompletedBound = true;
            }

            DebugLogger.Instance.LogMsg("Executing add node transaction. ResponseParameter Id - '{0}'; ClientId - '{1}';", ResponseParameter.I, _inProcessResponse.ClientId);
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
                        if (change.N != null)
                        {
                            DebugLogger.Instance.LogMsg("Load node transaction completed. ResponseParameter Id - '{0}'; ClientId - '{1}'; Returned NodeUid - '{2}';", ResponseParameter.I, _inProcessResponse.ClientId, change.N.N);
                            ServerObjects.Node soNode = MapManager.ServerObjectConverter.ToNode(change.N);
                            MapManager.NodeFactory.UpgradeFacade(this, soNode);
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
