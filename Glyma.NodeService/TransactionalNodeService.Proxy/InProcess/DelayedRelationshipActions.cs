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
    internal class DelayedRelationshipActions
    {
        private Queue<DelayedRelationshipAction> _queuedActions = null;

        public DelayedRelationshipActions(InProcess.InProcessRelationship relationshipContext)
        {
            RelationshipContext = relationshipContext;
        }

        public InProcess.InProcessRelationship RelationshipContext
        {
            get;
            private set;
        }

        private Queue<DelayedRelationshipAction> QueuedActions
        {
            get
            {
                _queuedActions = _queuedActions ?? new Queue<DelayedRelationshipAction>();

                return _queuedActions;
            }
        }

        private TransactionFramework.DeleteRelationshipTransactionLink CreateRelationshipDeletionTransaction(DelayedRelationshipAction action)
        {
            TransactionFramework.DeleteRelationshipTransactionLink deleteTransaction = null;

            if (RelationshipContext.Facade != null && RelationshipContext.Facade.IsConcrete)
            {
                deleteTransaction = new TransactionFramework.DeleteRelationshipTransactionLink();
                deleteTransaction.DomainId = RelationshipContext.DomainId;
                deleteTransaction.MapManager = RelationshipContext.MapManager;
                deleteTransaction.Relationship = RelationshipContext.Facade;
            }

            return deleteTransaction;
        }

        private TransactionFramework.UpdateRelationshipTransactionLink CreateRelationshipUpdatedTransaction(DelayedRelationshipAction action)
        {
            TransactionFramework.UpdateRelationshipTransactionLink updateTransaction = null;

            if (RelationshipContext.Facade != null && RelationshipContext.Facade.IsConcrete)
            {
                updateTransaction = new TransactionFramework.UpdateRelationshipTransactionLink();
                updateTransaction.DomainId = RelationshipContext.DomainId;
                updateTransaction.MapManager = RelationshipContext.MapManager;
                updateTransaction.Relationship = RelationshipContext.Facade;
                updateTransaction.RelationshipType = null;
            }

            return updateTransaction;
        }

        public void Enqueue(DelayedRelationshipAction action)
        {
            QueuedActions.Enqueue(action);
        }

        public DelayedRelationshipAction Dequeue()
        {
            return QueuedActions.Dequeue();
        }

        public void CreateTransactions(ref TransactionFramework.TransactionChain chain)
        {
            TransactionFramework.DeleteRelationshipTransactionLink deleteTransaction = null;
            TransactionFramework.UpdateRelationshipTransactionLink updateTransaction = null;

            foreach (DelayedRelationshipAction action in QueuedActions)
            {
                switch (action.Action)
                {
                    case TransactionalNodeService.Proxy.TransactionActionType.Deleted:
                        deleteTransaction = CreateRelationshipDeletionTransaction(action);

                        if (deleteTransaction != null)
                        {
                            chain.AddTransaction(deleteTransaction);
                        }

                        return;
                    case TransactionalNodeService.Proxy.TransactionActionType.Updated:
                        updateTransaction = updateTransaction ?? CreateRelationshipUpdatedTransaction(action);
                        updateTransaction.AddNode(action.ConnectionType, action.Node);

                        break;
                    case TransactionalNodeService.Proxy.TransactionActionType.TypeUpdated:
                        updateTransaction = updateTransaction ?? CreateRelationshipUpdatedTransaction(action);
                        updateTransaction.RelationshipType = action.RelationshipType;

                        break;
                    default:
                        break;
                }
            }

            if (updateTransaction != null)
            {
                chain.AddTransaction(updateTransaction);
            }
        }
    }
}
