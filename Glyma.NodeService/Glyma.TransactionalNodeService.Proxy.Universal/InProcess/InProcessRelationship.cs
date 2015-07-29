using TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection;
using TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.InProcess
{
    internal class InProcessRelationship : Relationship, IInProcess
    {
        private DelayedRelationshipActions _delayedActions = null;

        protected InProcessRelationship()
        {
        }

        public InProcessRelationship(IMapManager mapManager)
            : base(mapManager)
        {
        }

        public Soap.FacadeRelationship Facade
        {
            get;
            set;
        }

        public override RelationshipMetadataCollection Metadata
        {
            get
            {
                if (Status == LoadState.None)
                {
                    return null;
                }

                if (_metadataCollection == null)
                {
                    _metadataCollection = new RelationshipMetadataCollection(Facade);
                }

                return _metadataCollection;
            }
            protected set
            {
                _metadataCollection = value;
            }
        }

        public DelayedRelationshipActions DelayedActions
        {
            get
            {
                _delayedActions = _delayedActions ?? new DelayedRelationshipActions(this);

                return _delayedActions;
            }
        }

        public override void ConnectNode(ConnectionType connectionType, INode node, ref Soap.TransactionFramework.TransactionChain chain)
        {
            base.ConnectNode(connectionType, node, ref chain);

            if (Facade != null)
            {
                AddRelationshipTransactionLink addRelationshipTransaction = Facade.TransactionOrigin as AddRelationshipTransactionLink;

                if (addRelationshipTransaction != null)
                {
                    if (addRelationshipTransaction.TransactionStatus == ServerStatus.ProcessingClient)
                    {
                        addRelationshipTransaction.AddNode(connectionType, node);

                        return;
                    }
                }
            }
            
            DelayedActions.Enqueue(new DelayedRelationshipAction() { Action = TransactionActionType.Updated, ConnectionType = connectionType, Node = node });
        }

        public override void Update(RelationshipType relationshipType, ref Soap.TransactionFramework.TransactionChain chain)
        {
            if (Facade != null)
            {
                AddRelationshipTransactionLink addRelationshipTransaction = Facade.TransactionOrigin as AddRelationshipTransactionLink;

                if (addRelationshipTransaction != null)
                {
                    if (addRelationshipTransaction.TransactionStatus == ServerStatus.ProcessingClient)
                    {
                        addRelationshipTransaction.RelationshipType = relationshipType;

                        return;
                    }
                }
            }

            DelayedActions.Enqueue(new DelayedRelationshipAction() { Action = TransactionActionType.TypeUpdated, RelationshipType = relationshipType });
        }

        public override void Delete(ref Soap.TransactionFramework.TransactionChain chain)
        {
            if (Facade != null)
            {
                AddRelationshipTransactionLink addRelationshipTransaction = Facade.TransactionOrigin as AddRelationshipTransactionLink;

                if (addRelationshipTransaction != null)
                {
                    if (addRelationshipTransaction.TransactionStatus == ServerStatus.ProcessingClient)
                    {
                        DeleteRelationshipTransactionLink deleteTransaction = new DeleteRelationshipTransactionLink();

                        deleteTransaction.DomainId = DomainId;
                        deleteTransaction.MapManager = MapManager;
                        deleteTransaction.Relationship = Facade;

                        chain.AddTransaction(deleteTransaction);

                        return;
                    }
                }
            }

            DelayedActions.Enqueue(new DelayedRelationshipAction() { Action = TransactionActionType.Deleted });
        }

        public ISoapTransactionLink OriginLink
        {
            get;
            set;
        }
    }
}
