using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Data;

namespace TransactionalNodeService.Model
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "DA")]
    public class Metadata : IMapElementLoader
    {
        public Metadata()
        {
        }

        [DataMember(Name = "M")]
        public Guid MetadataId
        {
            get;
            set;
        }

        [DataMember(Name = "T")]
        public Guid MetadataTypeUid
        {
            get;
            set;
        }

        [DataMember(Name = "N")]
        public Guid? NodeUid
        {
            get;
            set;
        }

        [DataMember(Name = "R")]
        public Guid? RelationshipUid
        {
            get;
            set;
        }

        [DataMember(Name = "D")]
        public Guid? DescriptorTypeUid
        {
            get;
            set;
        }

        [DataMember(Name = "MN")]
        public string MetadataName
        {
            get;
            set;
        }

        [DataMember(Name = "MV")]
        public string MetadataValue
        {
            get;
            set;
        }

        [DataMember(Name = "DI")]
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

        public void LoadElement(IDataRecord record)
        {
            MetadataId = (Guid)record["MetadataId"];
            MetadataTypeUid = (Guid)record["MetadataTypeUid"];

            if (record["NodeUid"] != DBNull.Value)
            {
                NodeUid = (Guid?)record["NodeUid"];
            }
            else
            {
                NodeUid = null;
            }

            if (record["RelationshipUid"] != DBNull.Value)
            {
                RelationshipUid = (Guid?)record["RelationshipUid"];
            }
            else
            {
                RelationshipUid = null;
            }

            if (record["DescriptorTypeUid"] != DBNull.Value)
            {
                DescriptorTypeUid = (Guid?)record["DescriptorTypeUid"];
            }
            else
            {
                DescriptorTypeUid = null;
            }

            MetadataName = (string)record["MetadataName"];

            if (record["MetadataValue"] != DBNull.Value)
            {
                MetadataValue = (string)record["MetadataValue"];
            }
            else
            {
                MetadataValue = null;
            }

            if (record["DomainUid"] != DBNull.Value)
            {
                DomainUid = (Guid)record["DomainUid"];
            }
            else
            {
                DomainUid = Guid.Empty;
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
    }
}