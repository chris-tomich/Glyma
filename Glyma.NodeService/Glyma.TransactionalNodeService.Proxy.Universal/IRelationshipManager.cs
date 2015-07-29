using TransactionalNodeService.Proxy.Universal.Collections;
using TransactionalNodeService.Proxy.Universal.Collections.RelationshipContainers;

namespace TransactionalNodeService.Proxy.Universal
{
    public interface IRelationshipManager
    {
        void Clear();
        void Load(ConnectionSet connectionSet);
        void Remove(IRelationship relationship);
        void UnionWith(RelationshipSet relationships);
    }
}
