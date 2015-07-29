using TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection;
using TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.InProcess
{
    internal class InProcessNode : Node, IInProcess
    {
        private DelayedNodeActions _delayedActions = null;

        protected InProcessNode()
        {
        }

        public InProcessNode(IMapManager mapManager)
            : base(mapManager)
        {
        }

        public Soap.FacadeNode Facade
        {
            get;
            set;
        }

        public override NodeMetadataCollection Metadata
        {
            get
            {
                if (Status == LoadState.None)
                {
                    return null;
                }

                if (_metadataCollection == null)
                {
                    _metadataCollection = new NodeMetadataCollection(Facade, MapManager);
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

        public override void Update(NodeType nodeType, ref Soap.TransactionFramework.TransactionChain chain)
        {
            if (Facade != null)
            {
                AddNodeTransactionLink addNodeTransaction = Facade.TransactionOrigin as AddNodeTransactionLink;

                if (addNodeTransaction != null)
                {
                    if (addNodeTransaction.TransactionStatus == ServerStatus.ProcessingClient)
                    {
                        addNodeTransaction.NodeType = nodeType;

                        return;
                    }
                }
            }

            DelayedActions.Enqueue(new DelayedNodeAction() { Action = TransactionActionType.TypeUpdated, NodeType = nodeType });
        }

        public override void Delete(ref Soap.TransactionFramework.TransactionChain chain)
        {
            if (Facade != null)
            {
                AddNodeTransactionLink addNodeTransaction = Facade.TransactionOrigin as AddNodeTransactionLink;

                if (addNodeTransaction != null)
                {
                    if (addNodeTransaction.TransactionStatus == ServerStatus.ProcessingClient)
                    {
                        DeleteNodeTransactionLink deleteTransaction = new DeleteNodeTransactionLink();

                        deleteTransaction.DomainId = DomainId;
                        deleteTransaction.MapManager = MapManager;
                        deleteTransaction.Node = Facade;

                        chain.AddTransaction(deleteTransaction);

                        return;
                    }
                }
            }

            DelayedActions.Enqueue(new DelayedNodeAction() { Action = TransactionActionType.Deleted });
        }

        public ISoapTransactionLink OriginLink
        {
            get;
            set;
        }
    }
}
