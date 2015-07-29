using System;
using System.Collections.Generic;

namespace TransactionalNodeService.Proxy.Universal.Collections.RelationshipContainers
{
    public class RelationshipCollection : Dictionary<Guid, INode>
    {
        public RelationshipCollection()
        {
        }

        public RelationshipCollection(int capacity)
            : base(capacity)
        {
        }

        public RelationshipCollection(IDictionary<Guid, INode> dictionary)
            : base(dictionary)
        {
        }
    }
}
