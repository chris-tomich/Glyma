using System.Collections.Generic;

namespace TransactionalNodeService.Proxy.Universal.Collections.RelationshipContainers
{
    /// <summary>
    ///  TODO: This need to be replaced with a proper and more efficient fix. There are other places this needs to be fix, search for 6218535.
    /// </summary>
    public class EqualityComparer2 : IEqualityComparer<IRelationship>
    {
        public bool Equals(IRelationship x, IRelationship y)
        {
            return (x.Id == y.Id);
        }

        public int GetHashCode(IRelationship obj)
        {
            unchecked
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}
