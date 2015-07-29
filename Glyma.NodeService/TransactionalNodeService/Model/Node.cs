using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Data;

namespace TransactionalNodeService.Model
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "NO")]
    public class Node : IMapElement, IMapElementLoader
    {
        public Node()
        {
            Metadata = new Dictionary<MetadataContext, Metadata>();
        }

        [DataMember(Name = "N")]
        public Guid NodeUid
        {
            get;
            set;
        }

        [DataMember(Name = "O")]
        public string NodeOriginalId
        {
            get;
            set;
        }

        [DataMember(Name = "T")]
        public Guid NodeTypeUid
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

        [DataMember(Name = "DH")]
        public int Depth
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

        [IgnoreDataMember]
        public Guid Origin
        {
            get;
            set;
        }

        [IgnoreDataMember]
        public Guid ConnectingRelationship
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

        public void LoadElement(IDataRecord record)
        {
            NodeUid = (Guid)record["NodeUid"];

            if (record["NodeOriginalId"] != DBNull.Value)
            {
                NodeOriginalId = (string)record["NodeOriginalId"];
            }
            else
            {
                NodeOriginalId = null;
            }

            NodeTypeUid = (Guid)record["NodeTypeUid"];
            DomainUid = (Guid)record["DomainUid"];

            if (ContainsField(record, "Level"))
            {
                Depth = (int)record["Level"];
            }

            if (ContainsField(record, "Origin"))
            {
                Origin = (Guid)record["Origin"];
            }

            if (ContainsField(record, "RelationshipUid"))
            {
                ConnectingRelationship = (Guid)record["RelationshipUid"];
            }

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

        public bool ContainsField(IDataRecord record, string fieldName)
        {
            for (int i = 0; i < record.FieldCount; i++)
            {
                if (record.GetName(i) == fieldName)
                {
                    return true;
                }
            }

            return false;
        }

        Guid IMapElement.Id
        {
            get
            {
                return NodeUid;
            }
            set
            {
                NodeUid = value;
            }
        }

        Guid IMapElement.TypeId
        {
            get
            {
                return NodeTypeUid;
            }
            set
            {
                NodeTypeUid = value;
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
                return NodeOriginalId;
            }
            set
            {
                NodeOriginalId = value;
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