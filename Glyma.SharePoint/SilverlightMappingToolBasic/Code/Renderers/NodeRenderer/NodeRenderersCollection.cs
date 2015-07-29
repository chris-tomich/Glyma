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

namespace SilverlightMappingToolBasic
{
    public class NodeRendererCollection : Dictionary<Guid, NodeRenderer>
    {
        public NodeRendererCollection()
            : base()
        {
        }

        public NodeRendererCollection(int capacity)
            : base(capacity)
        {
        }

        public NodeRendererCollection(IEqualityComparer<Guid> comparer)
            : base(comparer)
        {
        }

        public NodeRendererCollection(IDictionary<Guid, NodeRenderer> dictionary)
            : base(dictionary)
        {
        }

        public NodeRendererCollection(int capacity, IEqualityComparer<Guid> comparer)
            : base(capacity, comparer)
        {
        }

        public NodeRendererCollection(IDictionary<Guid, NodeRenderer> dictionary, IEqualityComparer<Guid> comparer)
            : base(dictionary, comparer)
        {
        }
    }
}
