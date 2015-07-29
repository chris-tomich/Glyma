using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TransactionalNodeService.Proxy.Universal.Service;

namespace TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework
{
    public class TransactionChain : ISoapTransactionLinkExecutor
    {
        /// <summary>
        /// TODO: Remove this, it's for debugging purposes.
        /// </summary>
        private static int _currentChainId = 0;
        public int ChainId = -1;

        private bool _onCompletedBound = false;
        private TransactionChainStart _start = null;
        private TransactionChainEnd _end = null;
        private DependencyCollection _dependencies = null;
        private Dictionary<int, ISoapTransactionLink> _transactions = null;

        public event EventHandler TransactionExecuting;
        public event EventHandler TransactionFailed;
        public event EventHandler<Service.CompleteTransactionCompletedEventArgs> TransactionCompleted;

        public TransactionChain()
        {
            NumberOfTransactions = 0;
            LastLink = Start;
            IsBulkChain = true;

            /// TODO: Remove this, it's for debugging purposes.
            _currentChainId++;
            ChainId = _currentChainId;
        }

        protected int NumberOfTransactions
        {
            get;
            set;
        }

        public TransactionChainStart Start
        {
            get
            {
                _start = _start ?? new TransactionChainStart();
                _start.OriginChain = this;

                return _start;
            }
        }

        public TransactionChainEnd End
        {
            get
            {
                _end = _end ?? new TransactionChainEnd();
                _end.OriginChain = this;

                return _end;
            }
        }

        protected Dictionary<int, ISoapTransactionLink> Transactions
        {
            get
            {
                _transactions = _transactions ?? new Dictionary<int, ISoapTransactionLink>();

                return _transactions;
            }
        }

        protected ISoapTransactionLink LastLink
        {
            get;
            set;
        }

        protected DependencyCollection Dependencies
        {
            get
            {
                if (_dependencies == null)
                {
                    _dependencies = new DependencyCollection();
                    _dependencies.State = this;
                }

                return _dependencies;
            }
        }

        public bool IsBulkChain
        {
            get;
            set;
        }

        public int NumOfTransactions
        {
            get
            {
                return Transactions.Count;
            }
        }

        public void Reset()
        {
            foreach (ISoapTransactionLink link in Transactions.Values)
            {
                ISoapTransactionLinkExecutor executor = link as ISoapTransactionLinkExecutor;

                if (executor != null)
                {
                    executor.Reset();
                }
            }
        }

        public void AddTransaction(ISoapTransactionLink transaction)
        {
            NumberOfTransactions++;
            transaction.TransactionLinkId = NumberOfTransactions;
            transaction.OriginChain = this;
            Transactions.Add(transaction.TransactionLinkId, transaction);
            LastLink.AddNextLink(transaction);
            LastLink = transaction;
        }

        DependencyCollection ISoapTransactionLinkExecutor.Dependencies
        {
            get
            {
                return Dependencies;
            }
        }

        public void FillBulkOperations(ObservableCollection<Service.BOR> bulkOperationResponses)
        {
            foreach (Service.BOR bulkOperationResponse in bulkOperationResponses)
            {
                if (Transactions.ContainsKey(bulkOperationResponse.I))
                {
                    ISoapTransactionLink link = Transactions[bulkOperationResponse.I];

                    if (link is ISoapTransactionLinkExecutor)
                    {
                        ISoapTransactionLinkExecutor transactionLinkExecutor = link as ISoapTransactionLinkExecutor;

                        transactionLinkExecutor.ConsumeBulkOperationResponse(bulkOperationResponse);
                    }
                }
                else
                {
                    DebugLogger.Instance.LogMsg("A bulk operation response was returned that has no caller in this transaction chain.");
                }
            }
        }

        public DependencyCollection UpdateDependencies()
        {
            foreach (ISoapTransactionLink transaction in Transactions.Values)
            {
                ISoapTransactionLinkExecutor executor = transaction as ISoapTransactionLinkExecutor;

                if (executor != null)
                {
                    DependencyCollection dependencies = executor.UpdateDependencies();

                    Dependencies.UnionWith(dependencies);
                }
            }

            return Dependencies;
        }

        public void ExecuteTransaction(Guid sessionId, TransactionalMappingToolServiceClient serviceProxy)
        {
            if (TransactionExecuting != null)
            {
                System.EventArgs emptyEventArgs = new System.EventArgs();
                TransactionExecuting(this, emptyEventArgs);
            }

            ISoapTransactionLinkExecutor executor = Start;

            if (!_onCompletedBound)
            {
                LastLink.AddNextLink(End);

                executor.TransactionFailed += OnTransactionFailed;
                executor.TransactionCompleted += OnTransactionCompleted;

                _onCompletedBound = true;
            }

            if (IsBulkChain)
            {
                Service.OPS bulkOperations = new Service.OPS();

                executor.ExecuteBulkTransactions(sessionId, serviceProxy, ref bulkOperations);
            }
            else
            {
                executor.ExecuteTransaction(sessionId, serviceProxy);
            }
        }

        void ISoapTransactionLinkExecutor.ExecuteBulkTransactions(Guid sessionId, TransactionalMappingToolServiceClient serviceProxy, ref Service.OPS bulkOperations)
        {
            IsBulkChain = true;

            ExecuteTransaction(sessionId, serviceProxy);
        }

        void ISoapTransactionLinkExecutor.ConsumeBulkOperationResponse(Service.BOR bulkOperationResponse)
        {
            /// This will never be listed inside the bulk operations response list as it is always executed inline.
        }

        private void OnTransactionFailed(object sender, System.EventArgs e)
        {
            if (TransactionFailed != null)
            {
                System.Diagnostics.Debug.WriteLine(ChainId + " Failed");
                TransactionFailed(sender, e);
            }
        }

        private void OnTransactionCompleted(object sender, Service.CompleteTransactionCompletedEventArgs e)
        {
            if (TransactionCompleted != null)
            {
                System.Diagnostics.Debug.WriteLine(ChainId + " Completed");
                TransactionCompleted(sender, e);
            }
        }
    }
}
