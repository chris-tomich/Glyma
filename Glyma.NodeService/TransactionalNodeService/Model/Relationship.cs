using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Data;

namespace TransactionalNodeService.Model
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "RE")]
    public class Relationship : IMapElement, IMapElementLoader
    {
        protected Dictionary<Guid, Metadata> _metadata = new Dictionary<Guid, Metadata>();
        protected Dictionary<Guid, Metadata> _nodes = new Dictionary<Guid, Metadata>();

        public Relationship()
        {
            Nodes = new Dictionary<Guid, Guid>();
            Metadata = new Dictionary<MetadataContext, Metadata>();
        }

        [DataMember(Name = "R")]
        public Guid RelationshipUid
        {
            get;
            set;
        }

        [DataMember(Name = "O")]
        public string RelationshipOriginalId
        {
            get;
            set;
        }

        [DataMember(Name = "T")]
        public Guid RelationshipTypeUid
        {
            get;
            set;
        }

        [DataMember(Name = "D")]
        public Guid DomainUid
        {
            get;
            set;
        }

        [DataMember(Name = "RM")]
        public Guid? RootMapUid
        {
            get;
            set;
        }

        [DataMember(Name = "CR")]
        public DateTime? Created
        {
            get;
            set;
        }

        [DataMember(Name = "MO")]
        public DateTime? Modified
        {
            get;
            set;
        }

        [DataMember(Name = "CB")]
        public string CreatedBy
        {
            get;
            set;
        }

        [DataMember(Name = "MB")]
        public string ModifiedBy
        {
            get;
            set;
        }

        [DataMember(Name = "N")]
        public Dictionary<Guid, Guid> Nodes
        {
            get;
            set;
        }

        [DataMember(Name = "M")]
        public Dictionary<MetadataContext, Metadata> Metadata
        {
            get;
            set;
        }

        public void AddNode(Descriptor descriptor)
        {
            Nodes.Add(descriptor.DescriptorTypeUid, descriptor.NodeUid);
        }

        public void LoadElement(IDataRecord record)
        {
            RelationshipUid = (Guid)record["RelationshipUid"];

            if (record["RelationshipOriginalId"] != DBNull.Value)
            {
                RelationshipOriginalId = (string)record["RelationshipOriginalId"];
            }
            else
            {
                RelationshipOriginalId = null;
            }

            RelationshipTypeUid = (Guid)record["RelationshipTypeUid"];
            DomainUid = (Guid)record["DomainUid"];

            if (record["RootMapUid"] != DBNull.Value)
            {
                RootMapUid = (Guid)record["RootMapUid"];
            }
            else
            {
                RootMapUid = Guid.Empty;
            }

            if (record["Created"] != DBNull.Value)
            {
                Created = (DateTime?)record["Created"];
            }
            else
            {
                Created = null;
            }

            if (record["Modified"] != DBNull.Value)
            {
                Modified = (DateTime?)record["Modified"];
            }
            else
            {
                Modified = null;
            }

            if (record["CreatedBy"] != DBNull.Value)
            {
                CreatedBy = (string)record["CreatedBy"];
            }
            else
            {
                CreatedBy = null;
            }

            if (record["ModifiedBy"] != DBNull.Value)
            {
                ModifiedBy = (string)record["ModifiedBy"];
            }
            else
            {
                ModifiedBy = null;
            }
        }

        Guid IMapElement.Id
        {
            get
            {
                return RelationshipUid;
            }
            set
            {
                RelationshipUid = value;
            }
        }

        Guid IMapElement.TypeId
        {
            get
            {
                return RelationshipTypeUid;
            }
            set
            {
                RelationshipTypeUid = value;
            }
        }

        Guid IMapElement.DomainId
        {
            get
            {
                return DomainUid;
            }
            set
            {
                DomainUid = value;
            }
        }

        string IMapElement.OriginalId
        {
            get
            {
                return RelationshipOriginalId;
            }
            set
            {
                RelationshipOriginalId = value;
            }
        }

        Dictionary<MetadataContext, Metadata> IMapElement.Metadata
        {
            get
            {
                return Metadata;
            }
            set
            {
                Metadata = value;
            }
        }
    }
}