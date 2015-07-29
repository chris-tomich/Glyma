using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace TransactionalNodeService.Proxy
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
