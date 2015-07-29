using System;
using System.Collections.Generic;

namespace TransactionalNodeService.Proxy.Universal.ServerObjects
{
    public class Relationship
    {
        public Relationship()
        {
            Nodes = new Dictionary<Guid, Guid>();
            Metadata = new Dictionary<MetadataContext, Metadata>();
        }

        //[DataMember(Name = "R")]
        public Guid RelationshipUid
        {
            get;
            set;
        }

        //[DataMember(Name = "O")]
        public string RelationshipOriginalId
        {
            get;
            set;
        }

        //[DataMember(Name = "T")]
        public Guid RelationshipTypeUid
        {
            get;
            set;
        }

        //[DataMember(Name = "D")]
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
        public Dictionary<Guid, Guid> Nodes
        {
            get;
            set;
        }

        //[DataMember(Name = "M")]
        public Dictionary<MetadataContext, Metadata> Metadata
        {
            get;
            set;
        }
    }
}