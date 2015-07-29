using System;

namespace TransactionalNodeService.Proxy.ServerObjects
{
    public class Metadata
    {
        public Metadata()
        {
        }

        //[DataMember(Name = "M")]
        public Guid MetadataId
        {
            get;
            set;
        }

        //[DataMember(Name = "T")]
        public Guid MetadataTypeUid
        {
            get;
            set;
        }

        //[DataMember(Name = "DI")]
        public Guid DomainUid
        {
            get;
            set;
        }

        //[DataMember(Name = "RM")]
        public Guid? RootMapUid
        {
            get;
            set;
        }

        //[DataMember(Name = "N")]
        public Guid? NodeUid
        {
            get;
            set;
        }

        //[DataMember(Name = "R")]
        public Guid? RelationshipUid
        {
            get;
            set;
        }

        //[DataMember(Name = "D")]
        public Guid? DescriptorTypeUid
        {
            get;
            set;
        }

        //[DataMember(Name = "MN")]
        public string MetadataName
        {
            get;
            set;
        }

        //[DataMember(Name = "MV")]
        public string MetadataValue
        {
            get;
            set;
        }
    }
}