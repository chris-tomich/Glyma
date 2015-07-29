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

        private TransactionFramework.DeleteNodeTransactionLink CreateNodeDeletionTransaction(DelayedNodeAction action)
        {
            TransactionFramework.DeleteNodeTransactionLink deleteTransaction = null;

            if (NodeContext.Facade != null && NodeContext.Facade.IsConcrete)
            {
                deleteTransaction = new TransactionFramework.DeleteNodeTransactionLink();
                deleteTransaction.DomainId = NodeContext.DomainId;
                deleteTransaction.MapManager = NodeContext.MapManager;
                deleteTransaction.Node = NodeContext.Facade;
            }

            return deleteTransaction;
        }

        private TransactionFramework.UpdateNodeTransactionLink CreateNodeUpdatedTransaction(DelayedNodeAction action)
        {
            TransactionFramework.UpdateNodeTransactionLink updateTransaction = null;

            if (NodeContext.Facade != null && NodeContext.Facade.IsConcrete)
            {
                updateTransaction = new TransactionFramework.UpdateNodeTransactionLink();
                updateTransaction.DomainId = NodeContext.DomainId;
                updateTransaction.MapManager = NodeContext.MapManager;
                updateTransaction.Node = NodeContext.Facade;
                updateTransaction.NodeType = action.NodeType;
            }

            return updateTransaction;
        }

        public void CreateTransactions(ref TransactionFramework.TransactionChain chain)
        {
            TransactionFramework.DeleteNodeTransactionLink deleteTransaction = null;
            TransactionFramework.UpdateNodeTransactionLink updateTransaction = null;

            foreach (DelayedNodeAction action in QueuedActions)
            {
                switch (action.Action)
                {
                    case TransactionalNodeService.Proxy.TransactionActionType.Deleted:
                        deleteTransaction = CreateNodeDeletionTransaction(action);

                        if (deleteTransaction != null)
                        {
                            chain.AddTransaction(deleteTransaction);
                        }

                        return;
                    case TransactionalNodeService.Proxy.TransactionActionType.TypeUpdated:
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
