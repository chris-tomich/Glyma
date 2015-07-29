using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Collections
{
    public interface IRelationshipTuple
    {
        IRelationship Relationship { get; }
        ConnectionType ConnectionType { get; }
    }
}
