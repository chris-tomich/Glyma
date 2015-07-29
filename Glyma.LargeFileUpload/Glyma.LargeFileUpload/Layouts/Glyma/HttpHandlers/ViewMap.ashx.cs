using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.SharePoint;
using System.Data.Linq;
using System.Text;
using Glyma.HttpHandlers.ViewMapClasses;
using System.Web.Script.Serialization;
using System.Data.SqlClient;
using System.Data;

namespace Glyma.HttpHandlers
{
    /// <summary>
    /// Summary description for ViewMap
    /// </summary>
    public class ViewMap : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                context.Response.Expires = -1;
                context.Response.ExpiresAbsolute = DateTime.Now.AddSeconds(-1);
                context.Response.AddHeader("cache-control", "private");
                context.Response.CacheControl = "no-cache";
                context.Response.ContentType = "text/javascript";

                string serverUrl = HttpUtility.UrlDecode(context.Request.QueryString["site"]);
                string mapIdString = context.Request.QueryString["MapId"];
                string domainIdString = context.Request.QueryString["DomainId"];

                if (string.IsNullOrEmpty(mapIdString) || string.IsNullOrEmpty(domainIdString))
                {
                    /// We haven't been given enough information to be able to bring back a map.
                    context.Response.Write("[]");

                    return;
                }

                Guid mapId;
                Guid domainId;

                try
                {
                    mapId = new Guid(mapIdString);
                    domainId = new Guid(domainIdString);
                }
                catch
                {
                    /// Means we got an illegally formed ID.
                    context.Response.Write("[]");

                    return;
                }

                ConnectionStringBuilder connectionStringBuilder = new ConnectionStringBuilder(serverUrl);
                string connectionString = connectionStringBuilder.BuildMapConnectionString();

                using (MappingToolDatabaseDataContext dataContext = new MappingToolDatabaseDataContext(connectionString))
                {
                    IMultipleResults results = dataContext.QueryMapMultiDepth(domainId, mapId, 1, false);

                    QueryMapResultSet resultSet = new QueryMapResultSet(results);

                    List<QueryMapNode> nodesInMap = new List<QueryMapNode>();
                    Dictionary<Guid, Dictionary<Guid, QueryMapDescriptor>> arrowsInMap = new Dictionary<Guid, Dictionary<Guid, QueryMapDescriptor>>();

                    HashSet<Guid> relatedMapsNodeSet = new HashSet<Guid>();

                    #region Find all the nodes in the current map and get their relationships to other nodes.
                    var mapRelationships = resultSet.GetRelationshipsByDescriptorType(mapId, QueryMapResultSet.ToDescriptorType);

                    foreach (QueryMapRelationship mapRelationship in mapRelationships)
                    {
                        QueryMapNode node = resultSet.GetNode(mapRelationship.RelationshipUid, QueryMapResultSet.FromDescriptorType);
                        node.SetMetadataView(mapRelationship.RelationshipUid);

                        if (node != null)
                        {
                            nodesInMap.Add(node);
                        }

                        IEnumerable<QueryMapRelationship> queryMapRelationships = resultSet.GetRelationshipsByRelationshipType(node.NodeUid, QueryMapResultSet.FromToRelationshipType, QueryMapResultSet.TransclusionFromToRelationshipType);

                        foreach (QueryMapRelationship relationship in queryMapRelationships)
                        {
                            Dictionary<Guid, QueryMapDescriptor> descriptors = resultSet.GetNodes(relationship.RelationshipUid);

                            if (relationship.RelationshipTypeUid.Value == QueryMapResultSet.TransclusionFromToRelationshipType)
                            {
                                if (!descriptors.ContainsKey(QueryMapResultSet.TransclusionMapDescriptorType))
                                {
                                    continue;
                                }

                                if (descriptors[QueryMapResultSet.TransclusionMapDescriptorType].NodeUid.Value != mapId)
                                {
                                    continue;
                                }
                            }

                            arrowsInMap[relationship.RelationshipUid] = descriptors;
                        }
                    }
                    #endregion

                    JsonMapObjects mapObjects = new JsonMapObjects();
                    mapObjects.domain = domainId;

                    QueryMapNode rootMapNode = null;
                    QueryMapNode mapNode = null;

                    #region Build out the nodes
                    List<JsonNode> nodes = new List<JsonNode>();

                    int nodeCount = 0;
                    Dictionary<Guid, int> nodeIndexes = new Dictionary<Guid, int>();

                    foreach (QueryMapNode node in nodesInMap)
                    {
                        if (node.NodeUid == mapId)
                        {
                            /// If we encounter the map node, don't include this in the returned nodes as we don't need it present on the map. We will put aside though as we will need it for the breadcrumb.
                            mapNode = node;
                            continue;
                        }

                        JsonNode jsonNode = new JsonNode(node);

                        if (jsonNode.IsProperlyStructured)
                        {
                            nodeIndexes[node.NodeUid] = nodeCount;
                            nodes.Add(jsonNode);
                            nodeCount++;

                            /// The following procedure is to get the related maps.
                            resultSet.FindNodeIdsByRelationshipType(node.NodeUid, ref relatedMapsNodeSet, mapId, QueryMapResultSet.ToDescriptorType, QueryMapResultSet.MapContainerRelationshipType);
                        }
                    }

                    #region Find transcluded maps
                    Dictionary<Guid, SearchedNode> relatedMapsNodes = null;

                    if (relatedMapsNodeSet.Count > 0)
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();

                            DataTable relatedMapsNodeList = new DataTable();
                            relatedMapsNodeList.Columns.Add("NodeUid");

                            foreach (Guid relatedMapsNode in relatedMapsNodeSet)
                            {
                                relatedMapsNodeList.Rows.Add(relatedMapsNode);
                            }

                            using (SPSite site = new SPSite(serverUrl))
                            {
                                SPWeb web = site.OpenWeb();

                                QueryNodes queryNodes = new QueryNodes(web, connection);
                                relatedMapsNodes = queryNodes.GetNodes(relatedMapsNodeList);
                            }

                            connection.Close();
                        }
                    }

                    if (relatedMapsNodes != null)
                    {
                        foreach (JsonNode node in nodes)
                        {
                            HashSet<Guid> relatedMapIds = new HashSet<Guid>();

                            if (node.IsProperlyStructured)
                            {
                                resultSet.FindNodeIdsByRelationshipType(node.UniqueId, ref relatedMapIds, mapId, QueryMapResultSet.ToDescriptorType, QueryMapResultSet.MapContainerRelationshipType);

                                List<JsonRelatedMap> jsonRelatedMaps = new List<JsonRelatedMap>();

                                foreach (Guid relatedMapId in relatedMapIds)
                                {
                                    if (relatedMapsNodes.ContainsKey(relatedMapId))
                                    {
                                        JsonRelatedMap jsonRelatedMap = new JsonRelatedMap();
                                        jsonRelatedMap.DomainId = domainId;
                                        jsonRelatedMap.NodeId = node.UniqueId;
                                        jsonRelatedMap.MapNodeId = relatedMapId;
                                        jsonRelatedMap.Name = Uri.EscapeDataString(relatedMapsNodes[relatedMapId].Metadata["Name"]);

                                        jsonRelatedMaps.Add(jsonRelatedMap);
                                    }
                                }

                                if (jsonRelatedMaps.Count > 0)
                                {
                                    node["RelatedMaps"] = jsonRelatedMaps.ToArray();
                                }
                            }
                        }
                    }
                    #endregion
                    #endregion

                    mapObjects.nodes = nodes.ToArray();

                    #region Build out the arrows
                    List<JsonArrow> arrows = new List<JsonArrow>();

                    int arrowCount = 0;

                    foreach (Dictionary<Guid, QueryMapDescriptor> arrowDescriptors in arrowsInMap.Values)
                    {
                        int fromIndex = -1;
                        int toIndex = -1;

                        foreach (KeyValuePair<Guid, QueryMapDescriptor> arrowDescriptorPair in arrowDescriptors)
                        {
                            if (arrowDescriptorPair.Key == QueryMapResultSet.FromDescriptorType)
                            {
                                if (nodeIndexes.ContainsKey(arrowDescriptorPair.Value.NodeUid.Value))
                                {
                                    fromIndex = nodeIndexes[arrowDescriptorPair.Value.NodeUid.Value];
                                }
                            }
                            else if (arrowDescriptorPair.Key == QueryMapResultSet.ToDescriptorType)
                            {
                                if (nodeIndexes.ContainsKey(arrowDescriptorPair.Value.NodeUid.Value))
                                {
                                    toIndex = nodeIndexes[arrowDescriptorPair.Value.NodeUid.Value];
                                }
                            }
                        }

                        arrowCount++;

                        if (fromIndex >= 0 && toIndex >= 0)
                        {
                            JsonArrow arrow = new JsonArrow();
                            arrow.from = fromIndex;
                            arrow.to = toIndex;

                            arrows.Add(arrow);
                        }
                    }
                    #endregion

                    mapObjects.arrows = arrows.ToArray();

                    #region Build the root map node details
                    if (mapNode == null && resultSet.Nodes.ContainsKey(mapId))
                    {
                        mapNode = resultSet.Nodes[mapId];
                    }

                    Guid rootMapId = mapNode.RootMapUid.Value;

                    IMultipleResults rootMapResults = dataContext.QueryMapMultiDepth(domainId, rootMapId, 0, false);
                    QueryMapResultSet rootMapResultSet = new QueryMapResultSet(rootMapResults);

                    if (rootMapResultSet.Nodes.ContainsKey(rootMapId))
                    {
                        rootMapNode = rootMapResultSet.Nodes[rootMapId];
                        JsonNode rootMapJsonNode = new JsonNode(rootMapNode);

                        mapObjects.rootMap = rootMapJsonNode;
                    }
                    #endregion

                    #region Build out the breadcrumbs
                    /// TODO: This is just a basic breadcrumb for now.
                    List<JsonBreadcrumb> breadcrumbs = new List<JsonBreadcrumb>();

                    if (rootMapNode != null)
                    {
                        breadcrumbs.Add(new JsonBreadcrumb(rootMapNode));
                    }

                    if (mapId != rootMapNode.NodeUid)
                    {
                        if (mapNode != null)
                        {
                            breadcrumbs.Add(new JsonBreadcrumb(mapNode));
                        }
                    }
                    #endregion

                    mapObjects.breadcrumbs = breadcrumbs.ToArray();

                    JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
                    string mapObjectsAsJson = jsonSerializer.Serialize(mapObjects);

                    context.Response.Write(mapObjectsAsJson);
                }
            });
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}