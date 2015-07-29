using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ServerObjects = TransactionalNodeService.Proxy.ServerObjects;
using Service = TransactionalNodeService.Service;
using Proxy = TransactionalNodeService.Proxy;
using Glyma.Debug;

namespace TransactionalNodeService.Soap.TransactionFramework
{
    public class AddMetadataTransactionLink : ISoapTransactionLink, ISoapTransactionLinkExecutor
    {
        private bool _onCompletedBound = false;
        private ISoapTransactionLink _firstLink = null;
        private SoapTransactionLinkParameter _domainParameter = null;
        private SoapTransactionLinkParameter _rootMapParameter = null;
        private SoapTransactionLinkParameter _nodeParameter = null;
        private SoapTransactionLinkParameter _relationshipParameter = null;

        public AddMetadataTransactionLink()
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

        private Service.TransactionalMappingToolServiceClient ServiceProxy
        {
            get;
            set;
        }

        public int TransactionLinkId
        {
            get;
            set;
        }

        public Proxy.IMetadataSet MetadataSet
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Value
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
                return Service.MapParameterType.Metadata;
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

                _rootMapParameter.SetParameterValue(RootMapId.Value);

                return _rootMapParameter;
            }
        }

        private SoapTransactionLinkParameter NodeParameter
        {
            get
            {
                if (_nodeParameter == null)
                {
                    _nodeParameter = new SoapTransactionLinkParameter(Service.MapParameterType.Node);
                }

                FacadeNode facadeNode = Node as FacadeNode;

                // Check if this is a facade and if it is and the base node isn't concrete, then use the base node's response parameter.
                if (facadeNode != null && !facadeNode.IsConcrete)
                {
                    _nodeParameter.SetParameterValue(facadeNode.TransactionOrigin);
                }
                else if (Node != null)
                {
                    _nodeParameter.SetParameterValue(Node.Id);
                }
                else
                {
                    _nodeParameter.SetParameterNull();
                }

                return _nodeParameter;
            }
        }

        private SoapTransactionLinkParameter RelationshipParameter
        {
            get
            {
                if (_relationshipParameter == null)
                {
                    _relationshipParameter = new SoapTransactionLinkParameter(Service.MapParameterType.Relationship);
                }

                FacadeRelationship facadeRelationship = Relationship as FacadeRelationship;

                // Check if this is a facade and if it is and the base relationship isn't concrete, then use the base relationship's response parameter.
                if (facadeRelationship != null && !facadeRelationship.IsConcrete)
                {
                    _relationshipParameter.SetParameterValue(facadeRelationship.TransactionOrigin);
                }
                else if (Relationship != null)
                {
                    _relationshipParameter.SetParameterValue(Relationship.Id);
                }
                else
                {
                    _relationshipParameter.SetParameterNull();
                }

                return _relationshipParameter;
            }
        }

        public Guid DomainId
        {
            get;
            set;
        }

        public Guid? RootMapId
        {
            get;
            set;
        }

        public Proxy.INode Node
        {
            get;
            set;
        }

        public Proxy.IRelationship Relationship
        {
            get;
            set;
        }

        public Proxy.ConnectionType ConnectionType
        {
            get;
            set;
        }

        public Proxy.IMapManager MapManager
        {
            get;
            set;
        }

        public Proxy.MetadataType MetadataType
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
            _nodeParameter = null;
            _relationshipParameter = null;

            if (MetadataSet != null)
            {
                Proxy.FacadeMetadataSet metadataSet = MetadataSet as Proxy.FacadeMetadataSet;

                if (metadataSet != null)
                {
                    metadataSet.ResetToFacade();
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
                ServiceProxy.MACompleted -= OnAddMetadataCompleted;
            }
        }

        public void AddNextLink(ISoapTransactionLink nextLink)
        {
            _firstLink = nextLink;
        }

        //public InProcessTransactionResponse CreateInProcessObjects()
        //{
        //    Proxy.IMetadataSet metadataSet = new Proxy.MetadataSet();
        //    metadataSet.Name = Name;
        //    metadataSet.Value = Value;
        //    metadataSet.Node = Node;
        //    metadataSet.Relationship = Relationship;
        //    metadataSet.ConnectionType = ConnectionType;

        //    InProcessTransactionResponse inProcessResponse = new InProcessTransactionResponse();
        //    inProcessResponse.Metadata.Load(metadataSet);

        //    return inProcessResponse;
        //}

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

            Proxy.IFacade iFacadeNode = Node as Proxy.IFacade;

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

            Proxy.IFacade iFacadeRelationship = Relationship as Proxy.IFacade;

            if (iFacadeRelationship != null)
            {
                if (!iFacadeRelationship.IsConcrete)
                {
                    FacadeRelationship facadeRelationship = iFacadeRelationship as FacadeRelationship;
                    InProcess.InProcessRelationship inProcessRelationship = facadeRelationship.BaseRelationship as InProcess.InProcessRelationship;

                    if (inProcessRelationship != null && inProcessRelationship.OriginLink != null && inProcessRelationship.OriginLink.OriginChain != OriginChain)
                    {
                        executor.Dependencies.AddFacade(iFacadeRelationship);
                    }
                }
            }

            return executor.Dependencies;
        }

        void ISoapTransactionLinkExecutor.ExecuteTransaction(Guid sessionId, Service.TransactionalMappingToolServiceClient serviceProxy)
        {
            SessionId = sessionId;
            ServiceProxy = serviceProxy;

            ServiceProxy.MACompleted += OnAddMetadataCompleted;
            ServiceProxy.MAAsync(System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString(), SessionId, DomainParameter.GetParameterValue(sessionId), RootMapParameter.GetParameterValue(sessionId), NodeParameter.GetParameterValue(sessionId), RelationshipParameter.GetParameterValue(sessionId), MapManager.ConnectionTypes.ConvertProxyToService(ConnectionType), MapManager.MetadataTypes.ConvertProxyToService(MetadataType), Name, Value, this);
        }

        void ISoapTransactionLinkExecutor.ExecuteBulkTransactions(Guid sessionId, Service.TransactionalMappingToolServiceClient serviceProxy, ref Service.OPS bulkOperations)
        {
            SessionId = sessionId;
            ServiceProxy = serviceProxy;

            ResponseParameter = new Service.MP();
            ResponseParameter.I = Guid.NewGuid();
            ResponseParameter.S = sessionId;
            ResponseParameter.V = Guid.Empty;
            ResponseParameter.D = true;
            ResponseParameter.T = Service.MapParameterType.Node;

            Service.BMA metadataBulkOperation = new Service.BMA();
            metadataBulkOperation.I = TransactionLinkId;
            metadataBulkOperation.D = DomainParameter.GetParameterValue(sessionId);
            metadataBulkOperation.RM = RootMapParameter.GetParameterValue(sessionId);
            metadataBulkOperation.N = NodeParameter.GetParameterValue(sessionId);
            metadataBulkOperation.R = RelationshipParameter.GetParameterValue(sessionId);
            metadataBulkOperation.DT = MapManager.ConnectionTypes.ConvertProxyToService(ConnectionType);
            metadataBulkOperation.T = MapManager.MetadataTypes.ConvertProxyToService(MetadataType);
            metadataBulkOperation.MN = Name;
            metadataBulkOperation.MV = Value;
            metadataBulkOperation.RI = ResponseParameter.I;

            bulkOperations.O.Add(metadataBulkOperation);

            ISoapTransactionLinkExecutor executor = _firstLink as ISoapTransactionLinkExecutor;

            if (executor == null)
            {
                DebugLogger.Instance.LogMsg("Exception: This is not a supported type of link. This link does not implement ISoapTransactionLinkExecutor.");
                throw new NotSupportedException("This is not a supported type of link. This link does not implement ISoapTransactionLinkExecutor.");
            }

            if (bulkOperations.O.Count < 200)
            {
                if (!_onCompletedBound)
                {
                    executor.TransactionFailed += OnTransactionFailed;
                    executor.TransactionCompleted += OnTransactionCompleted;

                    _onCompletedBound = true;
                }

                DebugLogger.Instance.LogMsg("Executing add metadata transaction. Name - '{0}'; Value - '{1}';", Name, Value);
                executor.ExecuteBulkTransactions(SessionId, ServiceProxy, ref bulkOperations);
            }
            else
            {
                ServiceProxy.BOCCompleted += new EventHandler<Service.BOCCompletedEventArgs>(OnSubmitBulkOperationCompleted);
                ServiceProxy.SubmitBulkOperationsAsync(System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString(), SessionId, bulkOperations, this);
            }
        }

        void OnSubmitBulkOperationCompleted(object sender, Service.BOCCompletedEventArgs e)
        {
            if (e.UserState != this)
            {
                // This event is for a different object so don't execute.
                return;
            }

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

            DebugLogger.Instance.LogMsg("Executing add metadata transaction. ResponseParameter Id - '{0}'; Name - '{1}'; Value - '{2}';", ResponseParameter.I, Name, Value);
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

        private void OnAddMetadataCompleted(object sender, Service.MACompletedEventArgs e)
        {
            if (e.UserState != this)
            {
                // This event is for a different object so don't execute.
                return;
            }

            // The following lines must be done so that the transaction isn't executed again.
            ServiceProxy.MACompleted -= OnAddMetadataCompleted;

            if (e.Error != null)
            {
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

            if (!_onCompletedBound)
            {
                executor.TransactionFailed += OnTransactionFailed;
                executor.TransactionCompleted += OnTransactionCompleted;

                _onCompletedBound = true;
            }

            DebugLogger.Instance.LogMsg("Executing add metadata transaction. ResponseParameter Id - '{0}'; Name - '{1}'; Value - '{2}';", ResponseParameter.I, Name, Value);
            executor.ExecuteTransaction(SessionId, ServiceProxy);
        }

        private void OnTransactionFailed(object sender, EventArgs e)
        {
            if (InternalTransactionFailed != null)
            {
                System.Diagnostics.Debug.WriteLine("Failed");
                InternalTransactionFailed(sender, e);
            }
        }

        private void OnTransactionCompleted(object sender, Service.CompleteTransactionCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                foreach (Service.CH change in e.Result.C)
                {
                    if (change.P == ResponseParameter.I)
                    {
                        if (change.M != null)
                        {
                            DebugLogger.Instance.LogMsg("Load metadata transaction completed. ResponseParameter Id - '{0}'; Name - '{1}'; Value - '{2}'; Returned MetadataId - '{3}'", ResponseParameter.I, Name, Value, change.M.M);
                            ServerObjects.Metadata soMetadata = MapManager.ServerObjectConverter.ToMetadata(change.M);
                            Proxy.MetadataSetFactory.GetInstance(MapManager).UpgradeFacade(this, soMetadata);
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
