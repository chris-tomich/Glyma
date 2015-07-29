using System;
using TransactionalNodeService.Proxy.Universal.Service;

namespace TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework
{
    public class TransactionChainStart : ISoapTransactionLink, ISoapTransactionLinkExecutor
    {
        private bool _onCompletedBound = false;
        private ISoapTransactionLink _firstLink = null;

        public TransactionChainStart()
        {
            IsBulkExecution = false;
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

        private bool IsBulkExecution
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
            get
            {
                return new Service.MP();
            }
        }

        public Service.MapParameterType ResponseParameterType
        {
            get
            {
                return Service.MapParameterType.Empty;
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

            TransactionStatus = ServerStatus.ProcessingClient;

            if (_firstLink != null)
            {
                ISoapTransactionLinkExecutor executor = _firstLink as ISoapTransactionLinkExecutor;
                executor.TransactionFailed -= OnTransactionFailed;
                executor.TransactionCompleted -= OnTransactionCompleted;
            }

            if (ServiceProxy != null)
            {
                ServiceProxy.BeginTransactionCompleted -= OnBeginTransactionCompleted;
            }
        }

        public void AddNextLink(ISoapTransactionLink nextLink)
        {
            _firstLink = nextLink;
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
            ServiceProxy = serviceProxy;

            serviceProxy.BeginTransactionCompleted += OnBeginTransactionCompleted;
            serviceProxy.BeginTransactionAsync(CallingUrl.Url, this);
        }

        void ISoapTransactionLinkExecutor.ExecuteBulkTransactions(Guid sessionId, TransactionalMappingToolServiceClient serviceProxy, ref Service.OPS bulkOperations)
        {
            IsBulkExecution = true;

            ISoapTransactionLinkExecutor executor = this;
            executor.ExecuteTransaction(sessionId, serviceProxy);
        }

        void ISoapTransactionLinkExecutor.ConsumeBulkOperationResponse(Service.BOR bulkOperationResponse)
        {
            /// This will never be listed inside the bulk operations response list as it is always executed inline.
        }

        private void OnBeginTransactionCompleted(object sender, Service.BeginTransactionCompletedEventArgs e)
        {
            if (e.UserState != this)
            {
                // This event is for a different object so don't execute.
                return;
            }

            // The following lines must be done so that the transaction isn't executed again.
            ServiceProxy.BeginTransactionCompleted -= OnBeginTransactionCompleted;

            if (e.Error != null)
            {
                Service.CompleteTransactionCompletedEventArgs completedEventArgs = new Service.CompleteTransactionCompletedEventArgs(null, e.Error, false, null);

                if (InternalTransactionCompleted != null)
                {
                    InternalTransactionCompleted(this, completedEventArgs);
                }

                return;
            }

            ISoapTransactionLinkExecutor executor = _firstLink as ISoapTransactionLinkExecutor;

            if (executor == null)
            {
                throw new NotSupportedException("This is not a supported type of link. This link does not implement ISoapTransactionLinkExecutor.");
            }

            Guid sessionId = e.Result;

            if (!_onCompletedBound)
            {
                executor.TransactionFailed += OnTransactionFailed;
                executor.TransactionCompleted += OnTransactionCompleted;

                _onCompletedBound = true;
            }

            if (IsBulkExecution)
            {
                Service.OPS bulkOperations = new Service.OPS();
                bulkOperations.O = new System.Collections.ObjectModel.ObservableCollection<object>();

                executor.ExecuteBulkTransactions(sessionId, ServiceProxy, ref bulkOperations);
            }
            else
            {
                executor.ExecuteTransaction(sessionId, ServiceProxy);
            }
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
