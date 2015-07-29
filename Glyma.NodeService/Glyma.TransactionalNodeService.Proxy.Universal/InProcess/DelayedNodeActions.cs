using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework;

namespace TransactionalNodeService.Proxy.Universal.InProcess
{
    internal class DelayedNodeActions
    {
        private Queue<DelayedNodeAction> _queuedActions = null;

        public DelayedNodeActions(InProcess.InProcessNode nodeContext)
        {
            NodeContext = nodeContext;
        }

        public InProcess.InProcessNode NodeContext
        {
            get;
            private set;
        }

        private Queue<DelayedNodeAction> QueuedActions
        {
            get
            {
                _queuedActions = _queuedActions ?? new Queue<DelayedNodeAction>();

                return _queuedActions;
            }
        }

        public void Enqueue(DelayedNodeAction action)
        {
            QueuedActions.Enqueue(action);
        }

        public DelayedNodeAction Dequeue()
        {
            return QueuedActions.Dequeue();
        }

        private DeleteNodeTransactionLink CreateNodeDeletionTransaction(DelayedNodeAction action)
        {
            DeleteNodeTransactionLink deleteTransaction = null;

            if (NodeContext.Facade != null && NodeContext.Facade.IsConcrete)
            {
                deleteTransaction = new DeleteNodeTransactionLink();
                deleteTransaction.DomainId = NodeContext.DomainId;
                deleteTransaction.MapManager = NodeContext.MapManager;
                deleteTransaction.Node = NodeContext.Facade;
            }

            return deleteTransaction;
        }

        private UpdateNodeTransactionLink CreateNodeUpdatedTransaction(DelayedNodeAction action)
        {
            UpdateNodeTransactionLink updateTransaction = null;

            if (NodeContext.Facade != null && NodeContext.Facade.IsConcrete)
            {
                updateTransaction = new UpdateNodeTransactionLink();
                updateTransaction.DomainId = NodeContext.DomainId;
                updateTransaction.MapManager = NodeContext.MapManager;
                updateTransaction.Node = NodeContext.Facade;
                updateTransaction.NodeType = action.NodeType;
            }

            return updateTransaction;
        }

        public void CreateTransactions(ref TransactionChain chain)
        {
            DeleteNodeTransactionLink deleteTransaction = null;
            UpdateNodeTransactionLink updateTransaction = null;

            foreach (DelayedNodeAction action in QueuedActions)
            {
                switch (action.Action)
                {
                    case TransactionActionType.Deleted:
                        deleteTransaction = CreateNodeDeletionTransaction(action);

                        if (deleteTransaction != null)
                        {
                            chain.AddTransaction(deleteTransaction);
                        }

                        return;
                    case TransactionActionType.TypeUpdated:
                        updateTransaction = CreateNodeUpdatedTransaction(action);
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
