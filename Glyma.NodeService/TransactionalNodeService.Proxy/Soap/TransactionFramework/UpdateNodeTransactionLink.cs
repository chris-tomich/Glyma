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
using Glyma.Debug;

namespace TransactionalNodeService.Soap.TransactionFramework
{
    public class UpdateNodeTransactionLink : ISoapTransactionLink, ISoapTransactionLinkExecutor
    {
        private bool _onCompletedBound = false;
        private ISoapTransactionLink _firstLink = null;
        private SoapTransactionLinkParameter _domainParameter = null;
        private SoapTransactionLinkParameter _nodeParameter = null;

        public UpdateNodeTransactionLink()
        {
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

        public int TransactionLinkId
        {
            get;
            set;
        }

        public Guid DomainId
        {
            get;
            set;
        }

        private SoapTransactionLinkParameter NodeParameter
        {
            get
            {
                if (_nodeParameter == null)
                {
                    _nodeParameter = new SoapTransactionLinkParameter(Service.MapParameterType.Node);
                }

                _nodeParameter.SetParameterValue(Node.Id);

                return _nodeParameter;
            }
        }

        public Proxy.INode Node
        {
            get;
            set;
        }

        public Proxy.NodeType NodeType
        {
            get;
            set;
        }

        public Proxy.IMapManager MapManager
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

        public ServerStatus TransactionStatus
        {
            get;
            private set;
        }

        public void Reset()
        {
            _onCompletedBound = false;
            _domainParameter = null;
            _nodeParameter = null;

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
                ServiceProxy.NUCompleted -= OnUpdateNodeCompleted;
            }
        }

        public void AddNextLink(ISoapTransactionLink nextLink)
        {
            _firstLink = nextLink;
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

            return executor.Dependencies;
        }

        void ISoapTransactionLinkExecutor.ExecuteTransaction(Guid sessionId, Service.TransactionalMappingToolServiceClient serviceProxy)
        {
            SessionId = sessionId;
            ServiceProxy = serviceProxy;

            /// TODO: This may need to be changed so that this update transaction link is called if the NodeType on the INode object is assigned. Need to consider what is the best method.
            Node.NodeType = NodeType;

            ServiceProxy.NUCompleted += OnUpdateNodeCompleted;
            ServiceProxy.NUAsync(System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString(), sessionId, DomainParameter.GetParameterValue(SessionId), NodeParameter.GetParameterValue(SessionId), MapManager.NodeTypes.ConvertProxyToService(NodeType), this);
        }

        void ISoapTransactionLinkExecutor.ExecuteBulkTransactions(Guid sessionId, Service.TransactionalMappingToolServiceClient serviceProxy, ref Service.OPS bulkOperations)
        {
            SessionId = sessionId;
            ServiceProxy = serviceProxy;

            /// TODO: This may need to be changed so that this update transaction link is called if the NodeType on the INode object is assigned. Need to consider what is the best method.
            Node.NodeType = NodeType;

            ResponseParameter = new Service.MP();
            ResponseParameter.I = Guid.NewGuid();
            ResponseParameter.S = sessionId;
            ResponseParameter.V = Guid.Empty;
            ResponseParameter.D = true;
            ResponseParameter.T = Service.MapParameterType.Node;

            Service.BNU nodeBulkOperation = new Service.BNU();
            nodeBulkOperation.I = TransactionLinkId;
            nodeBulkOperation.D = DomainParameter.GetParameterValue(sessionId);
            nodeBulkOperation.N = NodeParameter.GetParameterValue(SessionId);
            nodeBulkOperation.T = MapManager.NodeTypes.ConvertProxyToService(NodeType);
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
                if (!_onCompletedBound)
                {
                    executor.TransactionFailed += OnTransactionFailed;
                    executor.TransactionCompleted += OnTransactionCompleted;

                    _onCompletedBound = true;
                }

                DebugLogger.Instance.LogMsg("Executing update node transaction. NodeId - '{0}'; NodeTypeName - '{1}'", Node.Id, NodeType.Name);
                executor.ExecuteBulkTransactions(SessionId, ServiceProxy, ref bulkOperations);
            }
            else
            {
                ServiceProxy.BOCCompleted += OnSubmitBulkOperationCompleted;
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

            DebugLogger.Instance.LogMsg("Executing update node transaction. ResponseParameter Id - '{0}'; NodeId - '{1}'; NodeTypeName - '{2}'", ResponseParameter.I, Node.Id, NodeType.Name);
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

        private void OnUpdateNodeCompleted(object sender, Service.NUCompletedEventArgs e)
        {
            if (e.UserState != this)
            {
                // This event is for a different object so don't execute.
                return;
            }

            // The following lines must be done so that the transaction isn't executed again.
            ServiceProxy.NUCompleted -= OnUpdateNodeCompleted;

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

            DebugLogger.Instance.LogMsg("Executing update node transaction. ResponseParameter Id - '{0}'; NodeId - '{1}'; NodeTypeName - '{2}'", ResponseParameter.I, Node.Id, NodeType.Name);
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
            DebugLogger.Instance.LogMsg("Update node transaction completed. ResponseParameter Id - '{0}';", ResponseParameter.I);
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
