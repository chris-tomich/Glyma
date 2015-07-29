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
    internal class MetadataSetComparer : IEqualityComparer<IMetadataSet>
    {
        private static MetadataSetComparer _instance = new MetadataSetComparer();

        public static MetadataSetComparer Instance
        {
            get
            {
                return _instance;
            }
        }

        private MetadataSetComparer()
        {
        }

        public bool Equals(IMetadataSet x, IMetadataSet y)
        {
            return (x.Equals(y));
        }

        public int GetHashCode(IMetadataSet obj)
        {
            return obj.GetHashCode();
        }
    }
}
