using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework;

namespace TransactionalNodeService.Proxy.Universal.InProcess
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

        private DeleteRelationshipTransactionLink CreateRelationshipDeletionTransaction(DelayedRelationshipAction action)
        {
            DeleteRelationshipTransactionLink deleteTransaction = null;

            if (RelationshipContext.Facade != null && RelationshipContext.Facade.IsConcrete)
            {
                deleteTransaction = new DeleteRelationshipTransactionLink();
                deleteTransaction.DomainId = RelationshipContext.DomainId;
                deleteTransaction.MapManager = RelationshipContext.MapManager;
                deleteTransaction.Relationship = RelationshipContext.Facade;
            }

            return deleteTransaction;
        }

        private UpdateRelationshipTransactionLink CreateRelationshipUpdatedTransaction(DelayedRelationshipAction action)
        {
            UpdateRelationshipTransactionLink updateTransaction = null;

            if (RelationshipContext.Facade != null && RelationshipContext.Facade.IsConcrete)
            {
                updateTransaction = new UpdateRelationshipTransactionLink();
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

        public void CreateTransactions(ref TransactionChain chain)
        {
            DeleteRelationshipTransactionLink deleteTransaction = null;
            UpdateRelationshipTransactionLink updateTransaction = null;

            foreach (DelayedRelationshipAction action in QueuedActions)
            {
                switch (action.Action)
                {
                    case TransactionActionType.Deleted:
                        deleteTransaction = CreateRelationshipDeletionTransaction(action);

                        if (deleteTransaction != null)
                        {
                            chain.AddTransaction(deleteTransaction);
                        }

                        return;
                    case TransactionActionType.Updated:
                        updateTransaction = updateTransaction ?? CreateRelationshipUpdatedTransaction(action);
                        updateTransaction.AddNode(action.ConnectionType, action.Node);

                        break;
                    case TransactionActionType.TypeUpdated:
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
