using System;

namespace TransactionalNodeService.Proxy.ServerObjects
{
    public class MetadataContext
    {
        public MetadataContext()
        {
        }

        public Guid MetadataId
        {
            get;
            set;
        }

        public string MetadataName
        {
            get;
            set;
        }

        public Guid? NodeUid
        {
            get;
            set;
        }

        public Guid? RelationshipUid
        {
            get;
            set;
        }

        public Guid? DescriptorTypeUid
        {
            get;
            set;
        }

        public MetadataContextType GetContextType()
        {
            if (NodeUid != null && NodeUid.HasValue && NodeUid.Value != Guid.Empty)
            {
                return MetadataContextType.Node;
            }
            else if (RelationshipUid != null && RelationshipUid.HasValue && RelationshipUid.Value != Guid.Empty)
            {
                return MetadataContextType.Relationship;
            }
            else
            {
                return MetadataContextType.Unknown;
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = (hash * 3) + MetadataId.GetHashCode();
                hash = (hash * 5) + NodeUid.GetHashCode();
                hash = (hash * 7) + RelationshipUid.GetHashCode();
                hash = (hash * 13) + DescriptorTypeUid.GetHashCode();

                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            MetadataContext comparingObject = obj as MetadataContext;

            if (comparingObject != null)
            {
                return (MetadataId == comparingObject.MetadataId) && (NodeUid == comparingObject.NodeUid) && (RelationshipUid == comparingObject.RelationshipUid) && (DescriptorTypeUid == comparingObject.DescriptorTypeUid);
            }
            else
            {
                return false;
            }
        }
    }
}