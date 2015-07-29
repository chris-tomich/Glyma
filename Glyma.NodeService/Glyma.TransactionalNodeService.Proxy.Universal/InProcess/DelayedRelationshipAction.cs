using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.InProcess
{
    internal class DelayedRelationshipAction
    {
        public TransactionActionType Action
        {
            get;
            set;
        }

        public RelationshipType RelationshipType
        {
            get;
            set;
        }

        public ConnectionType ConnectionType
        {
            get;
            set;
        }

        public INode Node
        {
            get;
            set;
        }
    }
}
