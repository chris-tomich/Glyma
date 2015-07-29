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
    public class NavigatorViewCollection : Dictionary<Guid, NavigatorView>
    {
        public NavigatorViewCollection()
            : base()
        {
        }

        public NavigatorViewCollection(int capacity)
            : base(capacity)
        {
        }

        public NavigatorViewCollection(IEqualityComparer<Guid> comparer)
            : base(comparer)
        {
        }

        public NavigatorViewCollection(IDictionary<Guid, NavigatorView> dictionary)
            : base(dictionary)
        {
        }

        public NavigatorViewCollection(int capacity, IEqualityComparer<Guid> comparer)
            : base(capacity, comparer)
        {
        }

        public NavigatorViewCollection(IDictionary<Guid, NavigatorView> dictionary, IEqualityComparer<Guid> comparer)
            : base(dictionary, comparer)
        {
        }
    }
}
