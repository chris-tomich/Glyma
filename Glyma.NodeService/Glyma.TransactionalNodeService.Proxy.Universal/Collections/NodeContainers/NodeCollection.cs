using System;
using System.Collections.Generic;

namespace TransactionalNodeService.Proxy.Universal.Collections.NodeContainers
{
    public class NodeCollection : Dictionary<Guid, INode>
    {
        public NodeCollection()
        {
        }

        public NodeCollection(int capacity)
            : base(capacity)
        {
        }

        public NodeCollection(IDictionary<Guid, INode> dictionary)
            : base(dictionary)
        {
        }
    }
}
