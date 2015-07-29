using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.InProcess
{
    internal class DelayedMetadataActions
    {
        private MetadataType _stringType = null;
        private Queue<DelayedMetadataAction> _queuedActions = null;

        public DelayedMetadataActions(InProcess.InProcessMetadata metadataContext)
        {
            MetadataContext = metadataContext;
        }

        #region Common Super Graph Types
        protected MetadataType StringMetadataType
        {
            get
            {
                if (_stringType == null)
                {
                    _stringType = MetadataContext.MapManager.MetadataTypes["string"];
                }

                return _stringType;
            }
        }
        #endregion

        public InProcess.InProcessMetadata MetadataContext
        {
            get;
            private set;
        }

        private Queue<DelayedMetadataAction> QueuedActions
        {
            get
            {
                _queuedActions = _queuedActions ?? new Queue<DelayedMetadataAction>();

                return _queuedActions;
            }
        }

        public void Enqueue(DelayedMetadataAction action)
        {
            QueuedActions.Enqueue(action);
        }

        public DelayedMetadataAction Dequeue()
        {
            return QueuedActions.Dequeue();
        }

        private UpdateMetadataTransactionLink CreateMetadataUpdateTransaction(DelayedMetadataAction action)
        {
            UpdateMetadataTransactionLink updateMetadataTransaction = null;

            if (MetadataContext.Facade != null && MetadataContext.Facade.IsConcrete)
            {
                updateMetadataTransaction = new UpdateMetadataTransactionLink();
                updateMetadataTransaction.DomainId = action.DomainId;
                updateMetadataTransaction.MapManager = MetadataContext.MapManager;
                updateMetadataTransaction.Metadata = MetadataContext.Facade;
                updateMetadataTransaction.Name = action.Name;
                updateMetadataTransaction.Value = action.Value;
            }

            return updateMetadataTransaction;
        }

        public void CreateTransactions(ref TransactionChain chain)
        {
            foreach (DelayedMetadataAction action in QueuedActions)
            {
                switch (action.Action)
                {
                    case TransactionActionType.Deleted:
                        // As this is metadata that hasn't yet been created, we don't need to delete it, we just do nothing with it.
                        return;
                    case TransactionActionType.Updated:
                        {
                            UpdateMetadataTransactionLink updateMetadataTransaction = CreateMetadataUpdateTransaction(action);

                            if (updateMetadataTransaction != null)
                            {
                                chain.AddTransaction(updateMetadataTransaction);
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
        }
    }
}
