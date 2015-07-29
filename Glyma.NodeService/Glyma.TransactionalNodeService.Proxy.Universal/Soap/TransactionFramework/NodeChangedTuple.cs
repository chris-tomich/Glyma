namespace TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework
{
    public struct NodeChangedTuple
    {
        public TransactionActionType Action;
        public INode Node;
    }
}
