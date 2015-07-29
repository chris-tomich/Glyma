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
using ServerObjects = TransactionalNodeService.Proxy.ServerObjects;

namespace TransactionalNodeService.Soap.TransactionFramework
{
    public class DeleteRelationshipTransactionLink : ISoapTransactionLink, ISoapTransactionLinkExecutor
    {
        private bool _onCompletedBound = false;
        private ISoapTransactionLink _firstLink = null;
        private SoapTransactionLinkParameter _domainParameter = null;
        private SoapTransactionLinkParameter _relationshipParameter = null;

        private event EventHandler InternalTransactionFailed;

        event EventHandler ISoapTransactionLinkExecutor.TransactionFailed
        {
            add { InternalTransactionFailed += value; }
            remove { InternalTransactionFailed -= value; }
        }

        private event EventHandler<Service.CompleteTransactionCompletedEventArgs> InternalTransactionCompleted;

        public DeleteRelationshipTransactionLink()
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

        public Proxy.IMapManager MapManager
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

        public Guid DomainId
        {
            get;
            set;
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

        public Proxy.IRelationship Relationship
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
            _relationshipParameter = null;

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
                ServiceProxy.RDCompleted -= OnDeleteRelationshipCompleted;
            }
        }

        public void AddNextLink(ISoapTransactionLink nextLink)
        {
            _firstLink = nextLink;
        }

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

            ServiceProxy.RDCompleted += OnDeleteRelationshipCompleted;
            ServiceProxy.RDAsync(System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString(), SessionId, DomainParameter.GetParameterValue(SessionId), RelationshipParameter.GetParameterValue(SessionId), this);
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

            Service.BRD relationshipBulkOperation = new Service.BRD();
            relationshipBulkOperation.I = TransactionLinkId;
            relationshipBulkOperation.D = DomainParameter.GetParameterValue(sessionId);
            relationshipBulkOperation.R = RelationshipParameter.GetParameterValue(SessionId);
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
                if (!_onCompletedBound)
                {
                    executor.TransactionFailed += OnTransactionFailed;
                    executor.TransactionCompleted += OnTransactionCompleted;

                    _onCompletedBound = true;
                }

                DebugLogger.Instance.LogMsg("Executing delete relationship transaction. ClientId - '{0}';", Relationship.ClientId);
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

            DebugLogger.Instance.LogMsg("Executing delete relationship transaction. ResponseParameter Id - '{0}'; ClientId - '{1}';", ResponseParameter.I, Relationship.ClientId);
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

        private void OnDeleteRelationshipCompleted(object sender, Service.RDCompletedEventArgs e)
        {
            if (e.UserState != this)
            {
                // This event is for a different object so don't execute.
                return;
            }

            // The following line must be done so that the transaction isn't executed again.
            ServiceProxy.RDCompleted -= OnDeleteRelationshipCompleted;

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
                throw new NotSupportedException("The next link in the chain is not a supported type of link or the next link is null. This link does not implement ISoapTransactionLinkExecutor.");
            }

            if (!_onCompletedBound)
            {
                executor.TransactionFailed += OnTransactionFailed;
                executor.TransactionCompleted += OnTransactionCompleted;

                _onCompletedBound = true;
            }

            DebugLogger.Instance.LogMsg("Executing delete relationship transaction. ResponseParameter Id - '{0}'; ClientId - '{1}';", ResponseParameter.I, Relationship.ClientId);
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
                        if (change.R != null)
                        {
                            DebugLogger.Instance.LogMsg("Delete relationship transaction completed. ResponseParameter Id - '{0}'; Node ClientId - '{1}'; Returned RelationshipUid - '{2}';", ResponseParameter.I, Relationship.ClientId, change.R.R);
                            ServerObjects.Relationship soRelationship = MapManager.ServerObjectConverter.ToRelationship(change.R);
                            MapManager.RelationshipFactory.DeleteRelationship(soRelationship);
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
