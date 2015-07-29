using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TransactionalNodeService.Proxy.ServerObjects
{
    public class Node
    {
        public Node()
        {
            Metadata = new Dictionary<MetadataContext, Metadata>();
        }

        //[DataMember(Name = "N")]
        public Guid NodeUid
        {
            get;
            set;
        }

        //[DataMember(Name = "O")]
        public string NodeOriginalId
        {
            get;
            set;
        }

        //[DataMember(Name = "T")]
        public Guid NodeTypeUid
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

        //[DataMember(Name = "DH")]
        public int Depth
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