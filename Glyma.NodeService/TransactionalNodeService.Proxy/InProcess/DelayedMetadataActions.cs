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
using System.Collections.Generic;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace TransactionalNodeService.InProcess
{
    internal class DelayedMetadataActions
    {
        private Proxy.MetadataType _stringType = null;
        private Queue<DelayedMetadataAction> _queuedActions = null;

        public DelayedMetadataActions(InProcess.InProcessMetadata metadataContext)
        {
            MetadataContext = metadataContext;
        }

        #region Common Super Graph Types
        protected Proxy.MetadataType StringMetadataType
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

        private TransactionFramework.UpdateMetadataTransactionLink CreateMetadataUpdateTransaction(DelayedMetadataAction action)
        {
            TransactionFramework.UpdateMetadataTransactionLink updateMetadataTransaction = null;

            if (MetadataContext.Facade != null && MetadataContext.Facade.IsConcrete)
            {
                updateMetadataTransaction = new TransactionFramework.UpdateMetadataTransactionLink();
                updateMetadataTransaction.DomainId = action.DomainId;
                updateMetadataTransaction.MapManager = MetadataContext.MapManager;
                updateMetadataTransaction.Metadata = MetadataContext.Facade;
                updateMetadataTransaction.Name = action.Name;
                updateMetadataTransaction.Value = action.Value;
            }

            return updateMetadataTransaction;
        }

        public void CreateTransactions(ref TransactionFramework.TransactionChain chain)
        {
            foreach (DelayedMetadataAction action in QueuedActions)
            {
                switch (action.Action)
                {
                    case Proxy.TransactionActionType.Deleted:
                        // As this is metadata that hasn't yet been created, we don't need to delete it, we just do nothing with it.
                        return;
                    case Proxy.TransactionActionType.Updated:
                        {
                            TransactionFramework.UpdateMetadataTransactionLink updateMetadataTransaction = CreateMetadataUpdateTransaction(action);

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
