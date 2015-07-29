using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Data;
using Microsoft.SharePoint;

namespace Glyma.HttpHandlers.ViewMapClasses
{
    public class SearchedNode
    {
        private Dictionary<string, string> _metadata = null;

        public SearchedNode()
        {
        }

        public Guid MapNodeUid
        {
            get;
            set;
        }

        public string MapNodeName
        {
            get;
            set;
        }

        public Guid NodeUid
        {
            get;
            set;
        }

        public string NodeOriginalId
        {
            get;
            set;
        }

        public Guid NodeTypeUid
        {
            get;
            set;
        }

        public Guid DomainUid
        {
            get;
            set;
        }

        public Guid? RootMapUid
        {
            get;
            set;
        }

        public DateTime? Created
        {
            get;
            set;
        }

        public DateTime? Modified
        {
            get;
            set;
        }

        public string CreatedBy
        {
            get;
            set;
        }

        public string ModifiedBy
        {
            get;
            set;
        }

        public string CreatedByLogin
        {
            get;
            set;
        }

        public string ModifiedByLogin
        {
            get;
            set;
        }

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

        public void LoadNodeRecord(SPWeb web, IDataReader record)
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
                CreatedBy = CreatedByLogin;

                try
                {
                    if (web != null)
                    {
                        CreatedBy = web.EnsureUser(CreatedByLogin).Name;
                    }
                }
                catch (SPException)
                {
                    /// Means the user is in the system so just use the login name. This is a highly unusual circumstance as if they aren't in the system, how did they add the node.
                }
            }

            if (record["ModifiedBy"] != null && record["ModifiedBy"] != DBNull.Value)
            {
                ModifiedByLogin = (string)record["ModifiedBy"];
                ModifiedBy = ModifiedByLogin;

                try
                {
                    if (web != null)
                    {
                        ModifiedBy = web.EnsureUser(ModifiedByLogin).Name;
                    }
                }
                catch (SPException)
                {
                    /// Means the user is in the system so just use the login name. This is a highly unusual circumstance as if they aren't in the system, how did they add the node.
                }
            }
        }

        public void AddMetadataRecord(SPWeb web, IDataReader record)
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
                        metadataCreatedBy = metadataCreatedByLogin;

                        try
                        {
                            if (web != null)
                            {
                                metadataCreatedBy = web.EnsureUser(metadataCreatedByLogin).Name;
                            }
                        }
                        catch (SPException)
                        {
                            /// Means the user is in the system so just use the login name. This is a highly unusual circumstance as if they aren't in the system, how did they add the node.
                        }
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
                        metadataModifiedBy = metadataModifiedByLogin;

                        try
                        {
                            if (web != null)
                            {
                                metadataModifiedBy = web.EnsureUser(metadataModifiedByLogin).Name;
                            }
                        }
                        catch (SPException)
                        {
                            /// Means the user is in the system so just use the login name. This is a highly unusual circumstance as if they aren't in the system, how did they add the node.
                        }
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