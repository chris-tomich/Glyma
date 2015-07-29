using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TransactionalNodeService.Common.Model;
using TransactionalNodeService.Common.NodeServiceOperations.Bulk;
using TransactionalNodeService.Common.TransactionOperations;

namespace TransactionalNodeService.Common
{
    public class TransactionalMappingToolServiceCommonBase : ITransactionalMappingToolServiceCommonBase
    {
        public static int SoftObjectLimit = 5500;
        public static int HardObjectLimit = 6500;

        public MapResponse CreateDomain(IGlymaSession glymaSession, string name)
        {
            using (IDbConnectionAbstraction mapDbConnection = glymaSession.ConnectionFactory.CreateMapDbConnection())
            {
                Domain newDomain = new Domain(true);
                newDomain.DomainUid = Guid.NewGuid();
                newDomain.DomainOriginalId = null;

                TypeResponse types = GetAllMapTypes(glymaSession);

                NodeType domainNodeType = types.NodeTypes["DomainNode"];
                MetadataType stringMetadataType = types.MetadataTypes["string"];

                newDomain.PersistSessionObject(mapDbConnection);

                Guid sessionId = BeginTransaction(glymaSession);

                MapParameter domainIdParameter = new MapParameter();
                domainIdParameter.Id = Guid.NewGuid();
                domainIdParameter.IsDelayed = false;
                domainIdParameter.ParameterType = MapParameterType.Domain;
                domainIdParameter.SessionId = sessionId;
                domainIdParameter.Value = newDomain.DomainUid;

                MapParameter newNode = AddNode(glymaSession, sessionId, domainIdParameter, null, domainNodeType, string.Empty);
                AddMetadata(glymaSession, sessionId, domainIdParameter, null, newNode, null, null, stringMetadataType, "Name", name);

                MapResponse newDomainNodeResponse = CompleteTransaction(glymaSession, sessionId);

                return newDomainNodeResponse;
            }
        }

        public QueryResponse CreateRootMap(IGlymaSession glymaSession, Guid domainId, string name, NodeType nodeType, string originalId)
        {
            IDbConnectionAbstraction mapDbConnection = null;
            IDbConnectionAbstraction parametersDbConnection = null;
            IDbConnectionAbstraction sessionDbConnection = null;

            try
            {
                mapDbConnection = glymaSession.ConnectionFactory.CreateMapDbConnection();
                parametersDbConnection = glymaSession.ConnectionFactory.CreateParametersDbConnection();
                sessionDbConnection = glymaSession.ConnectionFactory.CreateSessionDbConnection();

                CreateRootMap createRootMapOperation = new CreateRootMap(mapDbConnection, parametersDbConnection, sessionDbConnection);

                mapDbConnection.Open();
                parametersDbConnection.Open();
                sessionDbConnection.Open();

                Guid nodeId = createRootMapOperation.Create(domainId, name, nodeType, originalId);

                sessionDbConnection.Close();
                parametersDbConnection.Close();
                mapDbConnection.Close();

                QueryResponse response = QueryMap(glymaSession, domainId, nodeId, 0, false, null, null, 0, false);

                return response;
            }
            finally
            {
                if (mapDbConnection != null)
                {
                    mapDbConnection.Dispose();
                }

                if (parametersDbConnection != null)
                {
                    parametersDbConnection.Dispose();
                }

                if (sessionDbConnection != null)
                {
                    sessionDbConnection.Dispose();
                }
            }
        }

        public int DeleteDomain(IGlymaSession glymaSession, Guid domainId)
        {
            IDbConnectionAbstraction mapDbConnection = null;
            IDbConnectionAbstraction parametersDbConnection = null;
            IDbConnectionAbstraction sessionDbConnection = null;

            int deletions = -1;

            try
            {
                mapDbConnection = glymaSession.ConnectionFactory.CreateMapDbConnection();
                parametersDbConnection = glymaSession.ConnectionFactory.CreateParametersDbConnection();
                sessionDbConnection = glymaSession.ConnectionFactory.CreateSessionDbConnection();

                DeleteDomain deleteDomainOperation = new DeleteDomain(mapDbConnection, parametersDbConnection, sessionDbConnection);

                mapDbConnection.Open();
                parametersDbConnection.Open();
                sessionDbConnection.Open();

                deletions = deleteDomainOperation.ExecuteDeletion(domainId);

                sessionDbConnection.Close();
                parametersDbConnection.Close();
                mapDbConnection.Close();
            }
            finally
            {
                if (mapDbConnection != null)
                {
                    mapDbConnection.Dispose();
                }

                if (parametersDbConnection != null)
                {
                    parametersDbConnection.Dispose();
                }

                if (sessionDbConnection != null)
                {
                    sessionDbConnection.Dispose();
                }
            }

            return deletions;
        }

        public int DeleteRootMap(IGlymaSession glymaSession, Guid domainId, Guid rootMapId)
        {
            IDbConnectionAbstraction mapDbConnection = null;
            IDbConnectionAbstraction parametersDbConnection = null;
            IDbConnectionAbstraction sessionDbConnection = null;

            int deletions = -1;

            try
            {
                mapDbConnection = glymaSession.ConnectionFactory.CreateMapDbConnection();
                parametersDbConnection = glymaSession.ConnectionFactory.CreateParametersDbConnection();
                sessionDbConnection = glymaSession.ConnectionFactory.CreateSessionDbConnection();

                DeleteRootMap deleteRootMapOperation = new DeleteRootMap(mapDbConnection, parametersDbConnection, sessionDbConnection);

                mapDbConnection.Open();
                parametersDbConnection.Open();
                sessionDbConnection.Open();

                deletions = deleteRootMapOperation.ExecuteDeletion(domainId, rootMapId);

                sessionDbConnection.Close();
                parametersDbConnection.Close();
                mapDbConnection.Close();
            }
            finally
            {
                if (mapDbConnection != null)
                {
                    mapDbConnection.Dispose();
                }

                if (parametersDbConnection != null)
                {
                    parametersDbConnection.Dispose();
                }

                if (sessionDbConnection != null)
                {
                    sessionDbConnection.Dispose();
                }
            }

            return deletions;
        }

        public Guid BeginTransaction(IGlymaSession glymaSession)
        {
            MapSession session = glymaSession.Session;

            glymaSession.PersistSessionObject();

            return glymaSession.Session.Id;
        }

        public MapResponse CompleteTransaction(IGlymaSession glymaSession, Guid sessionId)
        {
            MapSession session = glymaSession.Session;

            MapResponse response = new MapResponse();

            using (IDbConnectionAbstraction parametersDbConnection = glymaSession.ConnectionFactory.CreateParametersDbConnection())
            {
                while (session.Count > 0)
                {
                    MapTransactionWrapper transaction = session.Dequeue();

                    transaction.ExecuteOperation(ref response);

                    transaction.ResponseParameter.PersistSessionObject(parametersDbConnection);
                }

                SessionFinalisation sessionFinalisation = new SessionFinalisation(glymaSession, sessionId);
                sessionFinalisation.ExecuteCompleteSession();

                return response;
            }
        }

        public TypeResponse GetAllMapTypes(IGlymaSession glymaSession)
        {
            using (IDbConnectionAbstraction mapDbConnection = glymaSession.ConnectionFactory.CreateMapDbConnection())
            {
                MapSchema mapSchema = new MapSchema(mapDbConnection);
                TypeResponse response = mapSchema.LoadTypesFromDb();

                return response;
            }
        }

        public Dictionary<Guid, QueryResponse> QueryDomains(IGlymaSession glymaSession)
        {
            using (IDbConnectionAbstraction mapDbConnection = glymaSession.ConnectionFactory.CreateMapDbConnection())
            {
                SqlCommand queryDomainMapCommand = new SqlCommand("SELECT [DomainUid], [NodeUid] FROM [Nodes] WHERE NodeTypeUid = '263754C2-2F31-4D21-B9C4-6509E00A5E94'", mapDbConnection.Connection);
                queryDomainMapCommand.CommandType = CommandType.Text;

                mapDbConnection.Open();

                SqlDataReader queryMapResults = queryDomainMapCommand.ExecuteReader();

                Dictionary<Guid, QueryResponse> domainResponses = new Dictionary<Guid, QueryResponse>();

                if (queryMapResults.HasRows)
                {
                    while (queryMapResults.Read())
                    {
                        Guid domainId = queryMapResults.GetGuid(0);
                        Guid nodeId = queryMapResults.GetGuid(1);

                        QueryResponse response = QueryMap(glymaSession, domainId, nodeId, 0, false, null, null, 0, false);
                        domainResponses[domainId] = response;
                    }
                }
                mapDbConnection.Close();

                try
                {
                    mapDbConnection.Open();
                    AuditLogItem logItem = new AuditLogItem(mapDbConnection.Connection);
                    logItem.OperationName = "QueryDomains";
                    //logItem.CallingUrl = callingUrl;
                    logItem.DomainUid = null;
                    logItem.NodeUid = null;
                    logItem.RootMapUid = null;
                    logItem.MaxDepth = null;
                    logItem.ObjectIndex = null;
                    logItem.EdgeConditions = null;
                    logItem.FilterConditions = null;
                    logItem.SearchConditions = null;
                    logItem.PageNumber = null;
                    logItem.PageSize = null;
                    logItem.Commit();
                    mapDbConnection.Close();
                }
                catch
                {
                    /// Don't do anything. This is here because audit logging is a very low importance task and we don't want it potentially killing the more important tasks at hand.
                }

                return domainResponses;
            }
        }

        public QueryResponse QueryMap(IGlymaSession glymaSession, Guid domainId, Guid nodeId, int maxDepth, bool isFullDomainSearch, EdgeConditions edgeConditions, FilterConditions filterConditions, int objectIndex)
        {
            return QueryMap(glymaSession, domainId, nodeId, maxDepth, isFullDomainSearch, edgeConditions, filterConditions, objectIndex, true);
        }

        public QueryResponse QueryMap(IGlymaSession glymaSession, Guid domainId, Guid nodeId, int maxDepth, bool isFullDomainSearch, EdgeConditions edgeConditions, FilterConditions filterConditions, int objectIndex, bool isCompressed)
        {
            if (!isCompressed)
            {
                TransactionalMappingToolServiceCommonBase.SoftObjectLimit = 3000;
                TransactionalMappingToolServiceCommonBase.HardObjectLimit = 4000;
            }
            else
            {
                TransactionalMappingToolServiceCommonBase.SoftObjectLimit = 5500;
                TransactionalMappingToolServiceCommonBase.HardObjectLimit = 6500;
            }

            using (IDbConnectionAbstraction mapDbConnection = glymaSession.ConnectionFactory.CreateMapDbConnection())
            {
                SqlCommand queryMapCommand = new SqlCommand("QueryMap", mapDbConnection.Connection);
                queryMapCommand.CommandType = CommandType.StoredProcedure;
                queryMapCommand.Parameters.Add(new SqlParameter("@DomainId", domainId));
                queryMapCommand.Parameters.Add(new SqlParameter("@NodeId", nodeId));
                queryMapCommand.Parameters.Add(new SqlParameter("@Depth", maxDepth));
                queryMapCommand.Parameters.Add(new SqlParameter("@FullDomain", isFullDomainSearch));

                mapDbConnection.Open();

                SqlDataReader queryMapResults = queryMapCommand.ExecuteReader();

                QueryResponse queryResponse = new QueryResponse();

                List<Node>[] orderedNodes = new List<Node>[maxDepth + 1];

                do
                {
                    while (queryMapResults.Read())
                    {
                        if (queryMapResults.GetSchemaTable().Select("ColumnName = 'Level'").Length > 0)
                        {
                            Node node = new Node();
                            node.LoadElement(queryMapResults);

                            List<Node> nodes;

                            if (orderedNodes[node.Depth] != null)
                            {
                                nodes = orderedNodes[node.Depth];
                            }
                            else
                            {
                                nodes = new List<Node>();
                                orderedNodes[node.Depth] = nodes;
                            }

                            nodes.Add(node);

                            queryResponse.AddNode(node);
                        }
                        else if (queryMapResults.GetSchemaTable().Select("ColumnName = 'MetadataId'").Length > 0)
                        {
                            Metadata metadata = new Metadata();
                            metadata.LoadElement(queryMapResults);

                            queryResponse.AddMetadata(metadata);
                        }
                        else if (queryMapResults.GetSchemaTable().Select("ColumnName = 'DescriptorUid'").Length > 0)
                        {
                            Descriptor descriptor = new Descriptor();
                            descriptor.LoadElement(queryMapResults);

                            queryResponse.AddDescriptor(descriptor);
                        }
                        else if (queryMapResults.GetSchemaTable().Select("ColumnName = 'RelationshipUid'").Length > 0)
                        {
                            Relationship relationship = new Relationship();
                            relationship.LoadElement(queryMapResults);

                            queryResponse.AddRelationship(relationship);
                        }
                    }
                }
                while (queryMapResults.NextResult());

                mapDbConnection.Close();

                try
                {
                    mapDbConnection.Open();
                    AuditLogItem logItem = new AuditLogItem(mapDbConnection.Connection);
                    logItem.OperationName = "QueryMap";
                    //logItem.CallingUrl = callingUrl;
                    logItem.DomainUid = domainId;
                    logItem.NodeUid = nodeId;
                    logItem.RootMapUid = null;
                    logItem.MaxDepth = maxDepth;
                    logItem.ObjectIndex = objectIndex;
                    logItem.EdgeConditions = null;
                    logItem.FilterConditions = null;
                    logItem.SearchConditions = null;
                    logItem.PageNumber = null;
                    logItem.PageSize = null;
                    logItem.Commit();
                    mapDbConnection.Close();
                }
                catch
                {
                    /// Don't do anything. This is here because audit logging is a very low importance task and we don't want it potentially killing the more important tasks at hand.
                }

                if (!queryResponse.Nodes.ContainsKey(nodeId))
                {
                    queryResponse.ErrorId = 1;
                    queryResponse.ErrorMessage = "Provided node ID context doesn't exist";

                    return queryResponse;
                }

                queryResponse.AttachElements();
                queryResponse.NodeContext = queryResponse.Nodes[nodeId];

                if (maxDepth > 0 && edgeConditions != null && edgeConditions.EdgeCondition != null)
                {
                    List<Guid> nodesToRemove = new List<Guid>();
                    List<Guid> relationshipsToRemove = new List<Guid>();

                    List<Guid> boundaryNodes = new List<Guid>();
                    List<Guid> boundaryRelationships = new List<Guid>();

                    for (int i = 1; i <= maxDepth; i++)
                    {
                        List<Node> nodes = orderedNodes[i];

                        foreach (Node node in nodes)
                        {
                            bool isBoundaryNode = false;
                            bool isBoundaryRelationship = false;
                            bool isNodeIncluded = true;
                            bool isRelationshipIncluded = true;
                            Relationship connectingRelationship = queryResponse.Relationships[node.ConnectingRelationship];

                            if (boundaryNodes.Contains(node.Origin))
                            {
                                isBoundaryNode = true;
                                isBoundaryRelationship = true;
                                isNodeIncluded = false;
                                isRelationshipIncluded = false;
                            }
                            else
                            {
                                EdgeResult relationshipEvalResult = edgeConditions.EdgeCondition.EvaluateCondition(connectingRelationship);

                                if (relationshipEvalResult.IsEdge.HasValue && relationshipEvalResult.IsEdge.Value)
                                {
                                    // THis means the relationship was evaluated to be a boundary edge.
                                    isBoundaryRelationship = true;
                                    isRelationshipIncluded = relationshipEvalResult.IsIncluded;
                                }

                                EdgeResult nodeEvalResult = edgeConditions.EdgeCondition.EvaluateCondition(node);

                                if (nodeEvalResult.IsEdge.HasValue && nodeEvalResult.IsEdge.Value)
                                {
                                    // This means the node was evaluated to be a boundary edge.
                                    isBoundaryNode = true;
                                    isNodeIncluded = nodeEvalResult.IsIncluded;
                                    // The inclusion value for the node trumps the relationship value as the relationship is dependent on the node existing anyway.
                                    isRelationshipIncluded = isNodeIncluded;
                                }
                                else if (isBoundaryRelationship)
                                {
                                    // If the relationship was discovered to be a boundary then this node will be a boundary edge too.
                                    isBoundaryNode = true;
                                    isNodeIncluded = isRelationshipIncluded;
                                }
                            }

                            if (isBoundaryNode)
                            {
                                boundaryNodes.Add(node.NodeUid);
                            }

                            if (isBoundaryRelationship)
                            {
                                boundaryRelationships.Add(connectingRelationship.RelationshipUid);
                            }

                            if (!isNodeIncluded)
                            {
                                nodesToRemove.Add(node.NodeUid);
                            }

                            if (!isRelationshipIncluded)
                            {
                                relationshipsToRemove.Add(connectingRelationship.RelationshipUid);
                            }
                        }
                    }

                    foreach (Guid nodeIdToRemove in nodesToRemove)
                    {
                        queryResponse.Nodes.Remove(nodeIdToRemove);
                    }

                    foreach (Guid relationshipIdToRemove in relationshipsToRemove)
                    {
                        queryResponse.Relationships.Remove(relationshipIdToRemove);
                    }
                }

                int totalObjects = queryResponse.CountObjects();

                queryResponse.Domain = new Domain();
                queryResponse.Domain.DomainUid = domainId;

                if (totalObjects > TransactionalMappingToolServiceCommonBase.HardObjectLimit || objectIndex > 0)
                {
                    return queryResponse.GetPage(objectIndex);
                }

                return queryResponse;
            }
        }

        public QueryResponse QueryMapByDomain(IGlymaSession glymaSession, Guid domainId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions)
        {
            using (IDbConnectionAbstraction mapDbConnection = glymaSession.ConnectionFactory.CreateMapDbConnection())
            {
                SqlCommand queryDomainMapCommand = new SqlCommand("SELECT [NodeUid] FROM [Nodes] WHERE DomainUid = @DomainId AND NodeTypeUid = '263754C2-2F31-4D21-B9C4-6509E00A5E94'", mapDbConnection.Connection);
                queryDomainMapCommand.CommandType = CommandType.Text;
                queryDomainMapCommand.Parameters.Add(new SqlParameter("@DomainId", domainId));

                Guid domainNodeId = Guid.Empty;

                mapDbConnection.Open();

                SqlDataReader queryMapResults = queryDomainMapCommand.ExecuteReader();

                if (queryMapResults.HasRows)
                {
                    queryMapResults.Read();

                    domainNodeId = queryMapResults.GetGuid(0);
                }
                mapDbConnection.Close();

                try
                {
                    mapDbConnection.Open();
                    AuditLogItem logItem = new AuditLogItem(mapDbConnection.Connection);
                    logItem.OperationName = "QueryMapByDomain";
                    //logItem.CallingUrl = callingUrl;
                    logItem.DomainUid = domainId;
                    logItem.NodeUid = null;
                    logItem.RootMapUid = null;
                    logItem.MaxDepth = maxDepth;
                    logItem.ObjectIndex = null;
                    logItem.EdgeConditions = null;
                    logItem.FilterConditions = null;
                    logItem.SearchConditions = null;
                    logItem.PageNumber = null;
                    logItem.PageSize = null;
                    logItem.Commit();
                    mapDbConnection.Close();
                }
                catch
                {
                    /// Don't do anything. This is here because audit logging is a very low importance task and we don't want it potentially killing the more important tasks at hand.
                }

                return QueryMap(glymaSession, domainId, domainNodeId, maxDepth, true, edgeConditions, filterConditions, 0);
            }
        }

        /// <summary>
        /// TODO: This method needs to be moved from here as it makes calls to schema specific methods.
        /// </summary>
        /// <param name="callingUrl"></param>
        /// <param name="domainId"></param>
        /// <param name="conditions"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public SearchMapResult SearchMap(IGlymaSession glymaSession, Guid domainId, Guid rootMapUid, SearchConditions conditions, int pageNumber = 1, int pageSize = 5)
        {
            SearchMapResult result = new SearchMapResult();

            using (IDbConnectionAbstraction mapDbConnection = glymaSession.ConnectionFactory.CreateMapDbConnection())
            {
                mapDbConnection.Open();

                SqlCommand findMetadata = conditions.BuildSearchQuery(mapDbConnection.Connection, rootMapUid, pageNumber, pageSize);

                SqlDataReader matchingMetadata = findMetadata.ExecuteReader();

                matchingMetadata.Read();
                int totalMetadataCount = (int)matchingMetadata[0];
                matchingMetadata.NextResult();

                int currentMetadataPagePosition = pageNumber * pageSize;

                DataTable nodesList;

                result.Count = totalMetadataCount;

                if (currentMetadataPagePosition > totalMetadataCount && pageNumber > 1)
                {
                    int itemsToRemove = currentMetadataPagePosition - totalMetadataCount;

                    if (itemsToRemove >= pageSize)
                    {
                        result = new SearchMapResult();

                        return result;
                    }
                    else
                    {
                        nodesList = new DataTable("NodesList");
                        nodesList.Load(matchingMetadata);

                        for (int i = 0; i < itemsToRemove; i++)
                        {
                            nodesList.Rows.RemoveAt(0);
                        }

                        nodesList.Columns.Remove("Modified");
                    }
                }
                else
                {
                    nodesList = new DataTable("NodesList");
                    nodesList.Load(matchingMetadata);
                    nodesList.Columns.Remove("Modified");
                }

                QueryNodes queryNodes = new QueryNodes(mapDbConnection);
                Dictionary<Guid, SearchedNode> nodes = queryNodes.GetNodes(nodesList);

                result.SearchedNodes = nodes;

                mapDbConnection.Close();

                try
                {
                    mapDbConnection.Open();
                    AuditLogItem logItem = new AuditLogItem(mapDbConnection.Connection);
                    logItem.OperationName = "SearchMap";
                    //logItem.CallingUrl = callingUrl;
                    logItem.DomainUid = domainId;
                    logItem.NodeUid = null;
                    logItem.RootMapUid = rootMapUid;
                    logItem.MaxDepth = null;
                    logItem.ObjectIndex = null;
                    logItem.EdgeConditions = null;
                    logItem.FilterConditions = null;
                    logItem.SearchConditions = null;
                    logItem.PageNumber = pageNumber;
                    logItem.PageSize = pageSize;
                    logItem.Commit();
                    mapDbConnection.Close();
                }
                catch
                {
                    /// Don't do anything. This is here because audit logging is a very low importance task and we don't want it potentially killing the more important tasks at hand.
                }

                return result;
            }
        }

        private MapParameter AddNode(IGlymaSession glymaSession, Guid sessionId, MapParameter domainId, MapParameter rootMapId, NodeType nodeType, string originalId)
        {
            return AddBulkNode(glymaSession, sessionId, Guid.Empty, domainId, rootMapId, nodeType, originalId);
        }

        public MapParameter AddBulkNode(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, NodeType nodeType, string originalId)
        {
            MapSession session = glymaSession.Session;
            MapParameters parameters = glymaSession.Parameters;
            parameters.AddParameter(domainId);
            parameters.AddParameter(rootMapId);

            MapTransactionWrapper createNodeOpWrapper = new MapTransactionWrapper(glymaSession);
            IMapTransaction createNodeOpCore = (IMapTransaction)createNodeOpWrapper;
            createNodeOpCore.OperationId = TransactionType.CreateNode;
            createNodeOpWrapper.DomainParameter = domainId;
            createNodeOpWrapper.RootMapParameter = rootMapId;
            createNodeOpCore.NodeTypeUid = nodeType.Id;

            session.Enqueue(createNodeOpWrapper);

            MapParameter response;

            if (responseParameter == Guid.Empty)
            {
                response = createNodeOpWrapper.ResponseParameter;
            }
            else
            {
                response = createNodeOpWrapper.BuildResponseParameterFromGuid(responseParameter);
            }

            glymaSession.PersistSessionObject();

            return response;
        }

        public MapParameter AddBulkRelationship(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType, string originalId)
        {
            MapSession session = glymaSession.Session;
            MapParameters parameters = glymaSession.Parameters;

            parameters.AddParameter(domainId);
            parameters.AddParameter(rootMapId);

            MapTransactionWrapper createRelationshipOpWrapper = new MapTransactionWrapper(glymaSession);
            IMapTransaction createRelationshipOpCore = (IMapTransaction)createRelationshipOpWrapper;
            createRelationshipOpCore.OperationId = TransactionType.CreateRelationship;
            createRelationshipOpWrapper.DomainParameter = domainId;
            createRelationshipOpWrapper.RootMapParameter = rootMapId;
            createRelationshipOpCore.RelationshipTypeUid = relationshipType.Id;

            session.Enqueue(createRelationshipOpWrapper);

            MapParameter response;

            if (responseParameter == Guid.Empty)
            {
                response = createRelationshipOpWrapper.ResponseParameter;
            }
            else
            {
                response = createRelationshipOpWrapper.BuildResponseParameterFromGuid(responseParameter);
            }

            if (nodes != null)
            {
                /// The nodes collection could be null if this is a relationship that is being created without the descriptors having yet been declaread.
                foreach (KeyValuePair<DescriptorType, MapParameter> descriptorToCreate in nodes)
                {
                    parameters.AddParameter(descriptorToCreate.Value);

                    MapTransactionWrapper createDescriptorOpWrapper = new MapTransactionWrapper(glymaSession);
                    IMapTransaction createDescriptorOpCore = (IMapTransaction)createDescriptorOpWrapper;
                    createDescriptorOpCore.OperationId = TransactionType.CreateDescriptor;
                    createDescriptorOpWrapper.DomainParameter = domainId;
                    createDescriptorOpWrapper.RootMapParameter = rootMapId;
                    createDescriptorOpCore.DescriptorTypeUid = descriptorToCreate.Key.Id;
                    createDescriptorOpWrapper.NodeParameter = descriptorToCreate.Value;
                    createDescriptorOpWrapper.RelationshipParameter = createRelationshipOpWrapper.ResponseParameter;

                    session.Enqueue(createDescriptorOpWrapper);
                }
            }

            glymaSession.PersistSessionObject();

            return response;
        }

        private MapParameter AddMetadata(IGlymaSession glymaSession, Guid sessionId, MapParameter domainId, MapParameter rootMapId, MapParameter node, MapParameter relationship, DescriptorType descriptorType, MetadataType metadataType, string name, string value)
        {
            return AddBulkMetadata(glymaSession, sessionId, Guid.Empty, domainId, rootMapId, node, relationship, descriptorType, metadataType, name, value);
        }

        public MapParameter AddBulkMetadata(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, MapParameter node, MapParameter relationship, DescriptorType descriptorType, MetadataType metadataType, string name, string value)
        {
            MapSession session = glymaSession.Session;
            MapParameters parameters = glymaSession.Parameters;

            parameters.AddParameter(domainId);
            parameters.AddParameter(rootMapId);
            parameters.AddParameter(node);
            parameters.AddParameter(relationship);

            MapTransactionWrapper createMetadataOpWrapper = new MapTransactionWrapper(glymaSession);
            IMapTransaction createMetadataOpCore = (IMapTransaction)createMetadataOpWrapper;
            createMetadataOpCore.OperationId = TransactionType.CreateMetadata;
            createMetadataOpWrapper.DomainParameter = domainId;
            createMetadataOpWrapper.RootMapParameter = rootMapId;
            createMetadataOpWrapper.NodeParameter = node;
            createMetadataOpWrapper.RelationshipParameter = relationship;

            if (descriptorType == null)
            {
                createMetadataOpCore.DescriptorTypeUid = null;
            }
            else
            {
                createMetadataOpCore.DescriptorTypeUid = descriptorType.Id;
            }

            if (metadataType == null)
            {
                createMetadataOpCore.MetadataTypeUid = null;
            }
            else
            {
                createMetadataOpCore.MetadataTypeUid = metadataType.Id;
            }

            createMetadataOpCore.MetadataName = name;
            createMetadataOpCore.MetadataValue = value;

            session.Enqueue(createMetadataOpWrapper);

            MapParameter response;

            if (responseParameter == Guid.Empty)
            {
                response = createMetadataOpWrapper.ResponseParameter;
            }
            else
            {
                response = createMetadataOpWrapper.BuildResponseParameterFromGuid(responseParameter);
            }

            glymaSession.PersistSessionObject();

            return response;
        }

        public MapParameter DeleteBulkNode(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter nodeId)
        {
            MapSession session = glymaSession.Session;
            MapParameters parameters = glymaSession.Parameters;

            parameters.AddParameter(domainId);
            parameters.AddParameter(nodeId);

            MapTransactionWrapper deleteNodeOpWrapper = new MapTransactionWrapper(glymaSession);
            IMapTransaction deleteNodeOpCore = (IMapTransaction)deleteNodeOpWrapper;
            deleteNodeOpCore.OperationId = TransactionType.DeleteNode;
            deleteNodeOpWrapper.DomainParameter = domainId;
            deleteNodeOpWrapper.NodeParameter = nodeId;

            session.Enqueue(deleteNodeOpWrapper);

            MapParameter response;

            if (responseParameter == Guid.Empty)
            {
                response = deleteNodeOpWrapper.ResponseParameter;
            }
            else
            {
                response = deleteNodeOpWrapper.BuildResponseParameterFromGuid(responseParameter);
            }

            glymaSession.PersistSessionObject();

            return response;
        }

        public MapParameter DeleteBulkRelationship(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter relationshipId)
        {
            MapSession session = glymaSession.Session;
            MapParameters parameters = glymaSession.Parameters;

            parameters.AddParameter(domainId);
            parameters.AddParameter(relationshipId);

            MapTransactionWrapper deleteRelationshipOpWrapper = new MapTransactionWrapper(glymaSession);
            IMapTransaction deleteRelationshipOpCore = (IMapTransaction)deleteRelationshipOpWrapper;
            deleteRelationshipOpCore.OperationId = TransactionType.DeleteRelationship;
            deleteRelationshipOpWrapper.DomainParameter = domainId;
            deleteRelationshipOpWrapper.RelationshipParameter = relationshipId;

            session.Enqueue(deleteRelationshipOpWrapper);

            MapParameter response;

            if (responseParameter == Guid.Empty)
            {
                response = deleteRelationshipOpWrapper.ResponseParameter;
            }
            else
            {
                response = deleteRelationshipOpWrapper.BuildResponseParameterFromGuid(responseParameter);
            }

            glymaSession.PersistSessionObject();

            return response;
        }

        public MapParameter DeleteBulkMetadata(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter metadata)
        {
            MapSession session = glymaSession.Session;
            MapParameters parameters = glymaSession.Parameters;

            parameters.AddParameter(domainId);
            parameters.AddParameter(metadata);

            MapTransactionWrapper deleteMetadataOpWrapper = new MapTransactionWrapper(glymaSession);
            IMapTransaction deleteMetadataOpCore = (IMapTransaction)deleteMetadataOpWrapper;
            deleteMetadataOpCore.OperationId = TransactionType.DeleteMetadata;
            deleteMetadataOpWrapper.DomainParameter = domainId;
            deleteMetadataOpWrapper.MetadataParameter = metadata;

            session.Enqueue(deleteMetadataOpWrapper);

            MapParameter response;

            if (responseParameter == Guid.Empty)
            {
                response = deleteMetadataOpWrapper.ResponseParameter;
            }
            else
            {
                response = deleteMetadataOpWrapper.BuildResponseParameterFromGuid(responseParameter);
            }

            glymaSession.PersistSessionObject();

            return response;
        }

        public MapParameter UpdateBulkNode(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter nodeId, NodeType nodeType)
        {
            MapSession session = glymaSession.Session;
            MapParameters parameters = glymaSession.Parameters;

            parameters.AddParameter(domainId);
            parameters.AddParameter(nodeId);

            MapTransactionWrapper updateNodeOpWrapper = new MapTransactionWrapper(glymaSession);
            IMapTransaction updateNodeOpCore = (IMapTransaction)updateNodeOpWrapper;
            updateNodeOpCore.OperationId = TransactionType.UpdateNode;
            updateNodeOpWrapper.DomainParameter = domainId;
            updateNodeOpWrapper.NodeParameter = nodeId;
            updateNodeOpCore.NodeTypeUid = nodeType.Id;

            session.Enqueue(updateNodeOpWrapper);

            MapParameter response;

            if (responseParameter == Guid.Empty)
            {
                response = updateNodeOpWrapper.ResponseParameter;
            }
            else
            {
                response = updateNodeOpWrapper.BuildResponseParameterFromGuid(responseParameter);
            }

            glymaSession.PersistSessionObject();

            return response;
        }

        public MapParameter UpdateBulkRelationship(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter relationshipId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType)
        {
            MapSession session = glymaSession.Session;
            MapParameters parameters = glymaSession.Parameters;

            parameters.AddParameter(domainId);
            parameters.AddParameter(relationshipId);

            MapParameter response = null;

            if (relationshipType != null)
            {
                MapTransactionWrapper updateRelationshipOpWrapper = new MapTransactionWrapper(glymaSession);
                IMapTransaction updateRelationshipOpCore = (IMapTransaction)updateRelationshipOpWrapper;
                updateRelationshipOpCore.OperationId = TransactionType.UpdateRelationship;
                updateRelationshipOpWrapper.DomainParameter = domainId;
                updateRelationshipOpWrapper.RelationshipParameter = relationshipId;
                updateRelationshipOpCore.RelationshipTypeUid = relationshipType.Id;

                session.Enqueue(updateRelationshipOpWrapper);

                if (responseParameter == Guid.Empty)
                {
                    response = updateRelationshipOpWrapper.ResponseParameter;
                }
                else
                {
                    response = updateRelationshipOpWrapper.BuildResponseParameterFromGuid(responseParameter);
                }
            }

            if (nodes != null)
            {
                foreach (KeyValuePair<DescriptorType, MapParameter> descriptorToUpdate in nodes)
                {
                    parameters.AddParameter(descriptorToUpdate.Value);

                    MapTransactionWrapper updateDescriptorOpWrapper = new MapTransactionWrapper(glymaSession);
                    IMapTransaction updateDescriptorOpCore = (IMapTransaction)updateDescriptorOpWrapper;
                    updateDescriptorOpCore.OperationId = TransactionType.UpdateDescriptor;
                    updateDescriptorOpWrapper.DomainParameter = domainId;
                    updateDescriptorOpWrapper.RelationshipParameter = relationshipId;
                    updateDescriptorOpWrapper.NodeParameter = descriptorToUpdate.Value;
                    updateDescriptorOpCore.DescriptorTypeUid = descriptorToUpdate.Key.Id;

                    session.Enqueue(updateDescriptorOpWrapper);
                }
            }

            glymaSession.PersistSessionObject();

            return response;
        }

        public MapParameter UpdateBulkMetadata(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter metadata, string name, string value)
        {
            MapSession session = glymaSession.Session;
            MapParameters parameters = glymaSession.Parameters;

            parameters.AddParameter(domainId);
            parameters.AddParameter(metadata);

            MapTransactionWrapper updateMetadataOpWrapper = new MapTransactionWrapper(glymaSession);
            IMapTransaction updateMetadataOpCore = (IMapTransaction)updateMetadataOpWrapper;
            updateMetadataOpCore.OperationId = TransactionType.UpdateMetadata;
            updateMetadataOpWrapper.DomainParameter = domainId;
            updateMetadataOpWrapper.MetadataParameter = metadata;
            updateMetadataOpCore.MetadataName = name;
            updateMetadataOpCore.MetadataValue = value;

            session.Enqueue(updateMetadataOpWrapper);

            MapParameter response;

            if (responseParameter == Guid.Empty)
            {
                response = updateMetadataOpWrapper.ResponseParameter;
            }
            else
            {
                response = updateMetadataOpWrapper.BuildResponseParameterFromGuid(responseParameter);
            }

            glymaSession.PersistSessionObject();

            return response;
        }

        public bool IsSessionCompleted(IGlymaSession glymaSession, Guid sessionId)
        {
            SessionFinalisation sessionFinalisation = new SessionFinalisation(glymaSession, sessionId);
            bool isSessionCompleted = sessionFinalisation.ExecuteIsCompletedQuery();

            return isSessionCompleted;
        }
    }
}
