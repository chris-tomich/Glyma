using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Collections.NodeContainers
{
    public struct NodeTriple
    {
        public INode Node;
        public ConnectionType ConnectionType;
        public IRelationship Relationship;
    }
}
