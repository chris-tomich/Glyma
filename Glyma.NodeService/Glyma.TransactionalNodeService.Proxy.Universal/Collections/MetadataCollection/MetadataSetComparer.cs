using System.Collections.Generic;

namespace TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection
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
