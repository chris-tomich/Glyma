using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Collections.RelationshipContainers
{
    /// <summary>
    /// TODO: This need to be replaced with a proper and more efficient fix. There are other places this needs to be fix, search for 6218535.
    /// </summary>
    public class EqualityComparer1 : IEqualityComparer<KeyValuePair<ConnectionType, IRelationship>>
    {
        public bool Equals(KeyValuePair<ConnectionType, IRelationship> x, KeyValuePair<ConnectionType, IRelationship> y)
        {
            return (x.Key.Id == y.Key.Id) && (x.Value.Id == y.Value.Id);
        }

        public int GetHashCode(KeyValuePair<ConnectionType, IRelationship> obj)
        {
            unchecked
            {
                int hash = 19;
                int salt = 3;

                hash = obj.Key.Id.GetHashCode() * salt;
                hash = (obj.Value.Id.GetHashCode() * salt) + hash;

                return hash;
            }
        }
    }
}
