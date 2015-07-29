using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal
{
    public interface IRelationshipQueryable
    {
        IEnumerable<KeyValuePair<ConnectionType, IRelationship>> FindRelationships();
        IEnumerable<IRelationship> FindRelationships(ConnectionType connectionType);
        IEnumerable<KeyValuePair<ConnectionType, IRelationship>> FindRelationships(RelationshipType relationshipType);
        IEnumerable<IRelationship> FindRelationships(ConnectionType connectionType, RelationshipType relationshipType);
    }
}
