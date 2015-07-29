using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.InProcess
{
    internal class DelayedNodeAction
    {
        public TransactionActionType Action
        {
            get;
            set;
        }

        public NodeType NodeType
        {
            get;
            set;
        }
    }
}
