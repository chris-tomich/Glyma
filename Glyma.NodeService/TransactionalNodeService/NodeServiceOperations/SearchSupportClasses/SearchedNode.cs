using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Data;

namespace TransactionalNodeService
{
    [DataContract]
    public class SearchedNode
    {
        private Dictionary<string, string> _metadata = null;

        public SearchedNode()
        {
        }

        [DataMember]
        public Guid MapNodeUid
        {
            get;
            set;
        }

        [DataMember]
        public string MapNodeName
        {
            get;
            set;
        }

        [DataMember]
        public Guid NodeUid
        {
            get;
            set;
        }

        [DataMember]
        public string NodeOriginalId
        {
            get;
            set;
        }

        [DataMember]
        public Guid NodeTypeUid
        {
            get;
            set;
        }

        [DataMember]
        public Guid DomainUid
        {
            get;
            set;
        }

        [DataMember]
        public Guid? RootMapUid
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? Created
        {
            get;
            set;
        }

        [DataMember]
        public DateTime? Modified
        {
            get;
            set;
        }

        [DataMember]
        public string CreatedBy
        {
            get;
            set;
        }

        [DataMember]
        public string ModifiedBy
        {
            get;
            set;
        }

        [DataMember]
        public string CreatedByLogin
        {
            get;
            set;
        }

        [DataMember]
        public string ModifiedByLogin
        {
            get;
            set;
        }

        [DataMember]
        public Dictionary<string, string> Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    _metadata = new Dictionary<string, string>();
                }

                return _metadata;
            }
        }

        public void LoadNodeRecord(IMapObjects mapObjects, IDataReader record)
        {
            if (record["MapNodeUid"] != null && record["MapNodeUid"] != DBNull.Value)
            {
                MapNodeUid = (Guid)record["MapNodeUid"];
            }

            if (record["NodeUid"] != null && record["NodeUid"] != DBNull.Value)
            {
                NodeUid = (Guid)record["NodeUid"];
            }

            if (record["NodeOriginalId"] != null && record["NodeOriginalId"] != DBNull.Value)
            {
                NodeOriginalId = (string)record["NodeOriginalId"];
            }

            if (record["NodeTypeUid"] != null && record["NodeTypeUid"] != DBNull.Value)
            {
                NodeTypeUid = (Guid)record["NodeTypeUid"];
            }

            if (record["DomainUid"] != null && record["DomainUid"] != DBNull.Value)
            {
                DomainUid = (Guid)record["DomainUid"];
            }

            if (record["RootMapUid"] != null && record["RootMapUid"] != DBNull.Value)
            {
                RootMapUid = (Guid)record["RootMapUid"];
            }

            if (record["Created"] != null && record["Created"] != DBNull.Value)
            {
                Created = (DateTime)record["Created"];
            }

            if (record["Modified"] != null && record["Modified"] != DBNull.Value)
            {
                Modified = (DateTime)record["Modified"];
            }

            if (record["CreatedBy"] != null && record["CreatedBy"] != DBNull.Value)
            {
                CreatedByLogin = (string)record["CreatedBy"];

                CreatedBy = mapObjects.FindUsersName(CreatedByLogin);
            }

            if (record["ModifiedBy"] != null && record["ModifiedBy"] != DBNull.Value)
            {
                ModifiedByLogin = (string)record["ModifiedBy"];

                ModifiedBy = mapObjects.FindUsersName(ModifiedByLogin);
            }
        }

        public void AddMetadataRecord(IMapObjects mapObjects, IDataReader record)
        {
            if (record["NodeUid"] != null && record["NodeUid"] != DBNull.Value)
            {
                DateTime? metadataCreated = null;
                DateTime? metadataModified = null;
                string metadataCreatedBy = null;
                string metadataModifiedBy = null;
                string metadataCreatedByLogin = null;
                string metadataModifiedByLogin = null;

                if (record["Created"] != null && record["Created"] != DBNull.Value)
                {
                    metadataCreated = (DateTime)record["Created"];
                }

                if (record["Modified"] != null && record["Modified"] != DBNull.Value)
                {
                    metadataModified = (DateTime)record["Modified"];
                }

                if (Created > metadataCreated)
                {
                    if (record["CreatedBy"] != null && record["CreatedBy"] != DBNull.Value)
                    {
                        metadataCreatedByLogin = (string)record["CreatedBy"];

                        metadataCreatedBy = mapObjects.FindUsersName(metadataCreatedByLogin);
                    }

                    if (!string.IsNullOrEmpty(metadataCreatedByLogin))
                    {
                        Created = metadataCreated;
                        CreatedBy = metadataCreatedBy;
                        CreatedByLogin = metadataCreatedByLogin;
                    }
                }

                if (Modified < metadataModified)
                {
                    if (record["ModifiedBy"] != null && record["ModifiedBy"] != DBNull.Value)
                    {
                        metadataModifiedByLogin = (string)record["ModifiedBy"];

                        metadataModifiedBy = mapObjects.FindUsersName(metadataModifiedByLogin);
                    }

                    if (!string.IsNullOrEmpty(metadataModifiedByLogin))
                    {
                        Modified = metadataModified;
                        ModifiedBy = metadataModifiedBy;
                        ModifiedByLogin = metadataModifiedByLogin;
                    }
                }

                if (record["MetadataName"] != null && record["MetadataName"] != DBNull.Value)
                {
                    string metadataName = (string)record["MetadataName"];

                    if (record["MetadataValue"] != null & record["MetadataValue"] != DBNull.Value)
                    {
                        string metadataValue = (string)record["MetadataValue"];

                        Metadata[metadataName] = metadataValue;
                    }
                    else
                    {
                        Metadata[metadataName] = null;
                    }
                }
            }
        }
    }
}