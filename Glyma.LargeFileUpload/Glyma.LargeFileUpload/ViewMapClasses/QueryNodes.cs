using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SharePoint;

namespace Glyma.HttpHandlers.ViewMapClasses
{
    public class QueryNodes
    {
        public QueryNodes(SPWeb web, SqlConnection connection)
        {
            Web = web;
            Connection = connection;
        }

        private SPWeb Web
        {
            get;
            set;
        }

        private SqlConnection Connection
        {
            get;
            set;
        }

        public Dictionary<Guid, SearchedNode> GetNodes(DataTable nodesList)
        {
            SqlParameter nodesListParameter = new SqlParameter();
            nodesListParameter.ParameterName = "@Nodes";
            nodesListParameter.SqlDbType = SqlDbType.Structured;
            nodesListParameter.Value = nodesList;

            SqlCommand queryNodesCommand = new SqlCommand("QueryNodes", Connection);
            queryNodesCommand.CommandType = CommandType.StoredProcedure;
            queryNodesCommand.Parameters.Add(nodesListParameter);

            SqlDataReader queryNodesResults = queryNodesCommand.ExecuteReader();

            Dictionary<Guid, string> mapNames = new Dictionary<Guid, string>();
            Dictionary<Guid, SearchedNode> searchedNodes = new Dictionary<Guid, SearchedNode>();

            do
            {
                while (queryNodesResults.Read())
                {
                    if (queryNodesResults.GetSchemaTable().Select("ColumnName = 'NodeUid'").Length > 0)
                    {
                        if (queryNodesResults["NodeUid"] != null && queryNodesResults["NodeUid"] != DBNull.Value)
                        {
                            SearchedNode node;
                            Guid nodeId = (Guid)queryNodesResults["NodeUid"];

                            if (queryNodesResults.GetSchemaTable().Select("ColumnName = 'MetadataId'").Length > 0)
                            {
                                if (searchedNodes.ContainsKey(nodeId))
                                {
                                    node = searchedNodes[nodeId];
                                    node.AddMetadataRecord(Web, queryNodesResults);
                                }
                                else if (mapNames.ContainsKey(nodeId))
                                {
                                    if (queryNodesResults["MetadataName"] != null && queryNodesResults["MetadataName"] != DBNull.Value)
                                    {
                                        string metadataName = (string)queryNodesResults["MetadataName"];

                                        if (metadataName == "Name")
                                        {
                                            if (queryNodesResults["MetadataValue"] != null & queryNodesResults["MetadataValue"] != DBNull.Value)
                                            {
                                                string metadataValue = (string)queryNodesResults["MetadataValue"];
                                                mapNames[nodeId] = metadataValue;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (queryNodesResults.GetSchemaTable().Select("ColumnName = 'MapNodeUid'").Length > 0)
                            {
                                if (!searchedNodes.ContainsKey(nodeId))
                                {
                                    node = new SearchedNode();
                                    node.LoadNodeRecord(Web, queryNodesResults);

                                    searchedNodes[nodeId] = node;
                                    mapNames[node.MapNodeUid] = null;
                                }
                            }
                        }
                    }
                }
            }
            while (queryNodesResults.NextResult());

            foreach (SearchedNode searchedNode in searchedNodes.Values)
            {
                if (mapNames.ContainsKey(searchedNode.MapNodeUid))
                {
                    searchedNode.MapNodeName = mapNames[searchedNode.MapNodeUid];
                }
            }

            return searchedNodes;
        }
    }
}