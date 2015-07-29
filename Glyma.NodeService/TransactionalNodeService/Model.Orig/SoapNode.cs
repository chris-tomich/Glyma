using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TransactionalNodeService
{
    [DataContract]
    public class SoapNode
    {
        public SoapNode()
        {
            Metadata = new Dictionary<MetadataContext, SoapMetadata>();
            Relationships = new Dictionary<Guid, SoapRelationship>();
        }

        [DataMember]
        public Guid Domain
        {
            get;
            set;
        }

        [DataMember]
        public Guid Id
        {
            get;
            set;
        }

        [DataMember]
        public SoapNodeType NodeType
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<Guid, SoapRelationship> Relationships
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<MetadataContext, SoapMetadata> Metadata
        {
            get;
            set;
        }
    }
}