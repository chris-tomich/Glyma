using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TransactionalNodeService
{
    [DataContract]
    public class SoapRelationship
    {
        public SoapRelationship()
        {
            Metadata = new Dictionary<string, SoapMetadata>();
            Nodes = new Dictionary<SoapDescriptorType, Guid>();
        }

        [DataMember]
        public Guid Id
        {
            get;
            set;
        }

        /// <summary>
        /// Guid - The Node Uid.
        /// </summary>
        [DataMember]
        public Dictionary<SoapDescriptorType, Guid> Nodes
        {
            get;
            set;
        }

        [DataMember]
        public SoapRelationshipType RelationshipType
        {
            get;
            set;
        }

        /// <summary>
        /// string - The name of the metadata.
        /// </summary>
        [DataMember]
        public Dictionary<string, SoapMetadata> Metadata
        {
            get;
            set;
        }
    }
}