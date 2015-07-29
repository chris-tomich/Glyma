using System;
using TransactionalNodeService.Proxy.Universal.Service;

namespace TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework
{
    public class TransactionChainEnd : ISoapTransactionLink, ISoapTransactionLinkExecutor
    {
        public TransactionChainEnd()
        {
            TransactionStatus = ServerStatus.ProcessingClient;
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
            TransactionStatus = ServerStatus.ProcessingClient;

            if (ServiceProxy != null)
            {
                ServiceProxy.BOCCompleted -= OnSubmitBulkOperationCompleted;
                ServiceProxy.CompleteTransactionCompleted -= OnCompleteTransactionCompleted;
            }
        }

        public void AddNextLink(ISoapTransactionLink nextLink)
        {
            // Just ignore this call.
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
            TransactionStatus = ServerStatus.TransactionExecuting;

            SessionId = sessionId;
            ServiceProxy = serviceProxy;

            ServiceProxy.CompleteTransactionCompleted += OnCompleteTransactionCompleted;
            ServiceProxy.CompleteTransactionAsync(CallingUrl.Url, SessionId, this);
        }

        void ISoapTransactionLinkExecutor.ExecuteBulkTransactions(Guid sessionId, TransactionalMappingToolServiceClient serviceProxy, ref Service.OPS bulkOperations)
        {
            SessionId = sessionId;
            ServiceProxy = serviceProxy;

            if (bulkOperations.O.Count > 0)
            {
                TransactionStatus = ServerStatus.SendingTransaction;

                ServiceProxy.BOCCompleted += OnSubmitBulkOperationCompleted;
                ServiceProxy.SubmitBulkOperationsAsync(CallingUrl.Url, SessionId, bulkOperations, this);
            }
            else
            {
                ISoapTransactionLinkExecutor executor = this;
                executor.ExecuteTransaction(sessionId, serviceProxy);
            }
        }

        void OnSubmitBulkOperationCompleted(object sender, BOCCompletedEventArgs e)
        {
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

            ISoapTransactionLinkExecutor executor = this;
            executor.ExecuteTransaction(SessionId, ServiceProxy);
        }

        void ISoapTransactionLinkExecutor.ConsumeBulkOperationResponse(Service.BOR bulkOperationResponse)
        {
            /// This will never be listed inside the bulk operations response list as it is always executed inline.
        }

        private void OnCompleteTransactionCompleted(object sender, Service.CompleteTransactionCompletedEventArgs e)
        {
            TransactionStatus = ServerStatus.TransactionReceived;

            if (e.UserState != this)
            {
                // This event is for a different object so don't execute.
                return;
            }

            // The following lines must be done so that the transaction isn't executed again.
            ServiceProxy.CompleteTransactionCompleted -= OnCompleteTransactionCompleted;

            if (e.Error == null)
            {
                TransactionStatus = ServerStatus.TransactionCompleted;
            }
            else
            {
                TransactionStatus = ServerStatus.TransactionFailed;
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
