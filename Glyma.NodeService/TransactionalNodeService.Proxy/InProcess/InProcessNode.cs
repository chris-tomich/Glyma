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
using Proxy = TransactionalNodeService.Proxy;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace TransactionalNodeService.InProcess
{
    internal class InProcessNode : Proxy.Node, IInProcess
    {
        private DelayedNodeActions _delayedActions = null;

        protected InProcessNode()
        {
        }

        public InProcessNode(Proxy.IMapManager mapManager)
            : base(mapManager)
        {
        }

        public Soap.FacadeNode Facade
        {
            get;
            set;
        }

        public override Proxy.NodeMetadataCollection Metadata
        {
            get
            {
                if (Status == Proxy.LoadState.None)
                {
                    return null;
                }

                if (_metadataCollection == null)
                {
                    _metadataCollection = new Proxy.NodeMetadataCollection(Facade, MapManager);
                }

                return _metadataCollection;
            }
            protected set
            {
                _metadataCollection = value;
            }
        }

        public DelayedNodeActions DelayedActions
        {
            get
            {
                _delayedActions = _delayedActions ?? new DelayedNodeActions(this);

                return _delayedActions;
            }
        }

        public override void Update(Proxy.NodeType nodeType, ref Soap.TransactionFramework.TransactionChain chain)
        {
            if (Facade != null)
            {
                TransactionFramework.AddNodeTransactionLink addNodeTransaction = Facade.TransactionOrigin as TransactionFramework.AddNodeTransactionLink;

                if (addNodeTransaction != null)
                {
                    if (addNodeTransaction.TransactionStatus == TransactionFramework.ServerStatus.ProcessingClient)
                    {
                        addNodeTransaction.NodeType = nodeType;

                        return;
                    }
                }
            }

            DelayedActions.Enqueue(new DelayedNodeAction() { Action = Proxy.TransactionActionType.TypeUpdated, NodeType = nodeType });
        }

        public override void Delete(ref Soap.TransactionFramework.TransactionChain chain)
        {
            if (Facade != null)
            {
                TransactionFramework.AddNodeTransactionLink addNodeTransaction = Facade.TransactionOrigin as TransactionFramework.AddNodeTransactionLink;

                if (addNodeTransaction != null)
                {
                    if (addNodeTransaction.TransactionStatus == TransactionFramework.ServerStatus.ProcessingClient)
                    {
                        TransactionFramework.DeleteNodeTransactionLink deleteTransaction = new TransactionFramework.DeleteNodeTransactionLink();

                        deleteTransaction.DomainId = DomainId;
                        deleteTransaction.MapManager = MapManager;
                        deleteTransaction.Node = Facade;

                        chain.AddTransaction(deleteTransaction);

                        return;
                    }
                }
            }

            DelayedActions.Enqueue(new DelayedNodeAction() { Action = Proxy.TransactionActionType.Deleted });
        }

        public TransactionFramework.ISoapTransactionLink OriginLink
        {
            get;
            set;
        }
    }
}
