using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data.SqlClient;
using System.ServiceModel.Activation;
using System.Data;
using Microsoft.SharePoint.Client.Services;
using Microsoft.SharePoint;
using TransactionalNodeService.Common;
using TransactionalNodeService.Common.Model;
using TransactionalNodeService.Common.NodeServiceOperations.Bulk;
using GlymaNSApp = Glyma.NodeServiceApp;

namespace TransactionalNodeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TransactionalMappingToolService" in code, svc and config file together.
    [BasicHttpBindingServiceMetadataExchangeEndpointAttribute]
    [AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, Namespace = "http://sevensigma.com.au/TransactionalNodeService")]
    public class TransactionalMappingToolService : ITransactionalMappingToolService
    {
        private TransactionalMappingToolServiceCommonBase _base = null;

        private TransactionalMappingToolServiceCommonBase Base
        {
            get
            {
                if (_base == null)
                {
                    _base = new TransactionalMappingToolServiceCommonBase();
                }

                return _base;
            }
        }

        public GlymaVersion GetApplicationVersion()
        {
            return GlymaNSApp.GlymaServerVersion.Current;
        }

        public MapResponse CreateDomain(string callingUrl, string name)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.CreateDomain(callingUrl, name);
            }
        }

        public QueryResponse CreateRootMap(string callingUrl, Guid domainId, string name, NodeType nodeType, string originalId)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.CreateRootMap(callingUrl, domainId, name, nodeType, originalId);
            }
        }

        public Guid BeginTransaction(string callingUrl)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.BeginTransaction(callingUrl);
            }
        }

        public MapResponse CompleteTransaction(string callingUrl, Guid sessionId)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.CompleteTransaction(callingUrl, sessionId);
            }
        }

        public TypeResponse GetAllMapTypes(string callingUrl)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.GetAllMapTypes(callingUrl);
            }
        }

        public Dictionary<Guid, QueryResponse> QueryDomains(string callingUrl)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.QueryDomains(callingUrl);
            }
        }

        public QueryResponse QueryMap(string callingUrl, Guid domainId, Guid nodeId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.QueryMap(callingUrl, domainId, nodeId, maxDepth, edgeConditions, filterConditions);
            }
        }

        public QueryResponse QueryMapPaged(string callingUrl, Guid domainId, Guid nodeId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions, int objectIndex)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.QueryMapPaged(callingUrl, domainId, nodeId, maxDepth, edgeConditions, filterConditions, objectIndex);
            }
        }

        public QueryResponse QueryMapByDomain(string callingUrl, Guid domainId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.QueryMapByDomain(callingUrl, domainId, maxDepth, edgeConditions, filterConditions);
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
        public SearchMapResult SearchMap(string callingUrl, Guid domainId, Guid rootMapUid, SearchConditions conditions, int pageNumber = 1, int pageSize = 5)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.SearchMap(callingUrl, domainId, rootMapUid, conditions, pageNumber, pageSize);
            }
        }

        public MapParameter AddNode(string callingUrl, Guid sessionId, MapParameter domainId, MapParameter rootMapId, NodeType nodeType, string originalId)
        {
            return AddBulkNode(callingUrl, sessionId, Guid.Empty, domainId, rootMapId, nodeType, originalId);
        }

        public MapParameter AddBulkNode(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, NodeType nodeType, string originalId)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.AddBulkNode(callingUrl, sessionId, responseParameter, domainId, rootMapId, nodeType, originalId);
            }
        }

        public MapParameter AddRelationship(string callingUrl, Guid sessionId, MapParameter domainId, MapParameter rootMapId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType, string originalId)
        {
            return AddBulkRelationship(callingUrl, sessionId, Guid.Empty, domainId, rootMapId, nodes, relationshipType, originalId);
        }

        public MapParameter AddBulkRelationship(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType, string originalId)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.AddBulkRelationship(callingUrl, sessionId, responseParameter, domainId, rootMapId, nodes, relationshipType, originalId);
            }
        }

        public MapParameter AddMetadata(string callingUrl, Guid sessionId, MapParameter domainId, MapParameter rootMapId, MapParameter node, MapParameter relationship, DescriptorType descriptorType, MetadataType metadataType, string name, string value)
        {
            return AddBulkMetadata(callingUrl, sessionId, Guid.Empty, domainId, rootMapId, node, relationship, descriptorType, metadataType, name, value);
        }

        public MapParameter AddBulkMetadata(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, MapParameter node, MapParameter relationship, DescriptorType descriptorType, MetadataType metadataType, string name, string value)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.AddBulkMetadata(callingUrl, sessionId, responseParameter, domainId, rootMapId, node, relationship, descriptorType, metadataType, name, value);
            }
        }

        public MapParameter DeleteNode(string callingUrl, Guid sessionId, MapParameter domainId, MapParameter nodeId)
        {
            return DeleteBulkNode(callingUrl, sessionId, Guid.Empty, domainId, nodeId);
        }

        public MapParameter DeleteBulkNode(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter nodeId)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.DeleteBulkNode(callingUrl, sessionId, responseParameter, domainId, nodeId);
            }
        }

        public MapParameter DeleteRelationship(string callingUrl, Guid sessionId, MapParameter domainId, MapParameter relationshipId)
        {
            return DeleteBulkRelationship(callingUrl, sessionId, Guid.Empty, domainId, relationshipId);
        }

        public MapParameter DeleteBulkRelationship(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter relationshipId)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.DeleteBulkRelationship(callingUrl, sessionId, responseParameter, domainId, relationshipId);
            }
        }

        public MapParameter DeleteMetadata(string callingUrl, Guid sessionId, MapParameter domainId, MapParameter metadata)
        {
            return DeleteBulkMetadata(callingUrl, sessionId, Guid.Empty, domainId, metadata);
        }

        public MapParameter DeleteBulkMetadata(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter metadata)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.DeleteBulkMetadata(callingUrl, sessionId, responseParameter, domainId, metadata);
            }
        }

        public MapParameter UpdateNode(string callingUrl, Guid sessionId, MapParameter domainId, MapParameter nodeId, NodeType nodeType)
        {
            return UpdateBulkNode(callingUrl, sessionId, Guid.Empty, domainId, nodeId, nodeType);
        }

        public MapParameter UpdateBulkNode(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter nodeId, NodeType nodeType)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.UpdateBulkNode(callingUrl, sessionId, responseParameter, domainId, nodeId, nodeType);
            }
        }

        public MapParameter UpdateRelationship(string callingUrl, Guid sessionId, MapParameter domainId, MapParameter relationshipId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType)
        {
            return UpdateBulkRelationship(callingUrl, sessionId, Guid.Empty, domainId, relationshipId, nodes, relationshipType);
        }

        public MapParameter UpdateBulkRelationship(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter relationshipId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.UpdateBulkRelationship(callingUrl, sessionId, responseParameter, domainId, relationshipId, nodes, relationshipType);
            }
        }

        public MapParameter UpdateMetadata(string callingUrl, Guid sessionId,  MapParameter domainId, MapParameter metadata, string name, string value)
        {
            return UpdateBulkMetadata(callingUrl, sessionId, Guid.Empty, domainId, metadata, name, value);
        }

        public MapParameter UpdateBulkMetadata(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter metadata, string name, string value)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.UpdateBulkMetadata(callingUrl, sessionId, responseParameter, domainId, metadata, name, value);
            }
        }

        public List<BulkOperationResponse> SubmitBulkOperations(string callingUrl, Guid sessionId, BulkOperations operations)
        {
            List<BulkOperationResponse> operationResponses = new List<BulkOperationResponse>();

            foreach (IBulkOperation operation in operations.Operations)
            {
                BulkOperationResponse operationResponse = operation.SubmitOperation(this, callingUrl, sessionId);

                operationResponses.Add(operationResponse);
            }

            return operationResponses;
        }

        public bool IsSessionCompleted(string callingUrl, Guid sessionId)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                return nodeServiceClient.IsSessionCompleted(callingUrl, sessionId);
            }
        }
    }
}
