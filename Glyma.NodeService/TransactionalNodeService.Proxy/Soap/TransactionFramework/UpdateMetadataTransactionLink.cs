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
    public class UpdateMetadataTransactionLink : ISoapTransactionLink, ISoapTransactionLinkExecutor
    {
        private bool _onCompletedBound = false;
        private ISoapTransactionLink _firstLink = null;
        private SoapTransactionLinkParameter _domainParameter = null;
        private SoapTransactionLinkParameter _metadataParameter = null;

        public UpdateMetadataTransactionLink()
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

        public Guid DomainId
        {
            get;
            set;
        }

        private SoapTransactionLinkParameter MetadataParameter
        {
            get
            {
                if (_metadataParameter == null)
                {
                    _metadataParameter = new SoapTransactionLinkParameter(Service.MapParameterType.Metadata);
                }

                _metadataParameter.SetParameterValue(Metadata.Id);

                return _metadataParameter;
            }
        }

        public Proxy.IMetadataSet Metadata
        {
            get;
            set;
        }

        public Proxy.IMapManager MapManager
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
            _metadataParameter = null;

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
                ServiceProxy.MUCompleted -= OnUpdateMetadataCompleted;
            }
        }

        public void AddNextLink(ISoapTransactionLink nextLink)
        {
            _firstLink = nextLink;
        }

        //public InProcessTransactionResponse CreateInProcessObjects()
        //{
        //    Metadata.Name = Name;
        //    Metadata.Value = Value;

        //    InProcessTransactionResponse inProcessResponse = new InProcessTransactionResponse();
        //    inProcessResponse.Metadata.Load(Metadata);

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

            Proxy.IFacade iFacadeMetadata = Metadata as Proxy.IFacade;

            if (iFacadeMetadata != null)
            {
                if (!iFacadeMetadata.IsConcrete)
                {
                    Proxy.FacadeMetadataSet facadeMetadata = iFacadeMetadata as Proxy.FacadeMetadataSet;
                    InProcess.InProcessMetadata inProcessMetadata = facadeMetadata.BaseMetadata as InProcess.InProcessMetadata;

                    if (inProcessMetadata != null && inProcessMetadata.OriginLink != null && inProcessMetadata.OriginLink.OriginChain != OriginChain)
                    {
                        executor.Dependencies.AddFacade(iFacadeMetadata);
                    }
                }
            }

            return executor.Dependencies;
        }

        void ISoapTransactionLinkExecutor.ExecuteTransaction(Guid sessionId, Service.TransactionalMappingToolServiceClient serviceProxy)
        {
            SessionId = sessionId;
            ServiceProxy = serviceProxy;

            ServiceProxy.MUCompleted += OnUpdateMetadataCompleted;
            ServiceProxy.MUAsync(System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString(), SessionId, DomainParameter.GetParameterValue(sessionId), MetadataParameter.GetParameterValue(sessionId), Name, Value, this);
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

            Service.BMU metadataBulkOperation = new Service.BMU();
            metadataBulkOperation.I = TransactionLinkId;
            metadataBulkOperation.D = DomainParameter.GetParameterValue(sessionId);
            metadataBulkOperation.M = MetadataParameter.GetParameterValue(sessionId);
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

                DebugLogger.Instance.LogMsg("Executing update metadata transaction. Name - '{0}'; Value - '{1}'; MetadataId - '{2}'", Name, Value, Metadata.Id);
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

            DebugLogger.Instance.LogMsg("Executing update metadata transaction. ResponseParameter Id - '{0}'; Name - '{1}'; Value - '{2}'; MetadataId - '{3}'", ResponseParameter.I, Name, Value, Metadata.Id);
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

        private void OnUpdateMetadataCompleted(object sender, Service.MUCompletedEventArgs e)
        {
            if (e.UserState != this)
            {
                // This event is for a different object so don't execute.
                return;
            }

            // The following lines must be done so that the transaction isn't executed again.
            ServiceProxy.MUCompleted -= OnUpdateMetadataCompleted;

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

            DebugLogger.Instance.LogMsg("Executing update metadata transaction. ResponseParameter Id - '{0}'; Name - '{1}'; Value - '{2}'; MetadataId - '{3}'", ResponseParameter.I, Name, Value, Metadata.Id);
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
            DebugLogger.Instance.LogMsg("Update metadata transaction completed. ResponseParameter Id - '{0}';", ResponseParameter.I);
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
