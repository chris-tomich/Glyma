using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransactionalNodeService.Common;
using TransactionalNodeService.Common.Model;
using TransactionalNodeService.Common.NodeServiceOperations.Bulk;

namespace Glyma.NodeServiceApp
{
    public sealed class NodeServiceClient : IDisposable
    {
        private SPSite _site = null;
        private SPServiceContext _serviceContext;

        public NodeServiceClient(string callingUrl)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                _site = new SPSite(callingUrl);

                _serviceContext = SPServiceContext.GetContext(_site);
            });
        }

        public GlymaVersion GetApplicationVersion()
        {
            GlymaVersion result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.GetApplicationVersion());

            return result;
        }

        public MapResponse CreateDomain(GlymaSessionConfiguration configuration, string name)
        {
            MapResponse result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.CreateDomain(configuration, name));

            return result;
        }

        public QueryResponse CreateRootMap(GlymaSessionConfiguration configuration, Guid domainId, string name, NodeType nodeType, string originalId)
        {
            QueryResponse result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.CreateRootMap(configuration, domainId, name, nodeType, originalId));

            return result;
        }

        public int DeleteDomain(GlymaSessionConfiguration configuration, Guid domainId)
        {
            int result = 0;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.DeleteDomain(configuration, domainId));

            return result;
        }

        public int DeleteRootMap(GlymaSessionConfiguration configuration, Guid domainId, Guid rootMapId)
        {
            int result = 0;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.DeleteRootMap(configuration, domainId, rootMapId));

            return result;
        }

        public Guid BeginTransaction(GlymaSessionConfiguration configuration)
        {
            Guid result = Guid.Empty;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.BeginTransaction(configuration));

            return result;
        }

        public MapResponse CompleteTransaction(GlymaSessionConfiguration configuration, Guid sessionId)
        {
            MapResponse result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.CompleteTransaction(configuration, sessionId));

            return result;
        }

        public TypeResponse GetAllMapTypes(GlymaSessionConfiguration configuration)
        {
            TypeResponse result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.GetAllMapTypes(configuration));

            return result;
        }

        public Dictionary<Guid, QueryResponse> QueryDomains(GlymaSessionConfiguration configuration)
        {
            Dictionary<Guid, QueryResponse> result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.QueryDomains(configuration));

            return result;
        }

        public QueryResponse QueryMap(GlymaSessionConfiguration configuration, Guid domainId, Guid nodeId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions)
        {
            QueryResponse result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.QueryMap(configuration, domainId, nodeId, maxDepth, edgeConditions, filterConditions));

            return result;
        }

        public QueryResponse QueryMapPaged(GlymaSessionConfiguration configuration, Guid domainId, Guid nodeId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions, int objectIndex)
        {
            QueryResponse result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.QueryMapPaged(configuration, domainId, nodeId, maxDepth, edgeConditions, filterConditions, objectIndex));

            return result;
        }

        public QueryResponse QueryMapByDomain(GlymaSessionConfiguration configuration, Guid domainId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions)
        {
            QueryResponse result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.QueryMapByDomain(configuration, domainId, maxDepth, edgeConditions, filterConditions));

            return result;
        }

        public SearchMapResult SearchMap(GlymaSessionConfiguration configuration, Guid domainId, Guid rootMapUid, SearchConditions conditions, int pageNumber, int pageSize)
        {
            if (pageNumber == 0)
            {
                pageNumber = 1;
            }

            if (pageSize == 0)
            {
                pageSize = 5;
            }

            SearchMapResult result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.SearchMap(configuration, domainId, rootMapUid, conditions, pageNumber, pageSize));

            return result;
        }

        public MapParameter AddNode(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter rootMapId, NodeType nodeType, string originalId)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.AddNode(configuration, sessionId, domainId, rootMapId, nodeType, originalId));

            return result;
        }

        public MapParameter AddBulkNode(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, NodeType nodeType, string originalId)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.AddBulkNode(configuration, sessionId, responseParameter, domainId, rootMapId, nodeType, originalId));

            return result;
        }

        public MapParameter DeleteNode(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter nodeId)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.DeleteNode(configuration, sessionId, domainId, nodeId));

            return result;
        }

        public MapParameter DeleteBulkNode(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter nodeId)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.DeleteBulkNode(configuration, sessionId, responseParameter, domainId, nodeId));

            return result;
        }

        public MapParameter UpdateNode(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter nodeId, NodeType nodeType)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.UpdateNode(configuration, sessionId, domainId, nodeId, nodeType));

            return result;
        }

        public MapParameter UpdateBulkNode(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter nodeId, NodeType nodeType)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.UpdateBulkNode(configuration, sessionId, responseParameter, domainId, nodeId, nodeType));

            return result;
        }

        public MapParameter AddRelationship(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter rootMapId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType, string originalId)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.AddRelationship(configuration, sessionId, domainId, rootMapId, nodes, relationshipType, originalId));

            return result;
        }

        public MapParameter AddBulkRelationship(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType, string originalId)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.AddBulkRelationship(configuration, sessionId, responseParameter, domainId, rootMapId, nodes, relationshipType, originalId));

            return result;
        }

        public MapParameter DeleteRelationship(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter relationshipId)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.DeleteRelationship(configuration, sessionId, domainId, relationshipId));

            return result;
        }

        public MapParameter DeleteBulkRelationship(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter relationshipId)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.DeleteBulkRelationship(configuration, sessionId, responseParameter, domainId, relationshipId));

            return result;
        }

        public MapParameter UpdateRelationship(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter relationshipId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.UpdateRelationship(configuration, sessionId, domainId, relationshipId, nodes, relationshipType));

            return result;
        }

        public MapParameter UpdateBulkRelationship(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter relationshipId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.UpdateBulkRelationship(configuration, sessionId, responseParameter, domainId, relationshipId, nodes, relationshipType));

            return result;
        }

        public MapParameter AddMetadata(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter rootMapId, MapParameter node, MapParameter relationship, DescriptorType descriptorType, MetadataType metadataType, string name, string value)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.AddMetadata(configuration, sessionId, domainId, rootMapId, node, relationship, descriptorType, metadataType, name, value));

            return result;
        }

        public MapParameter AddBulkMetadata(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, MapParameter node, MapParameter relationship, DescriptorType descriptorType, MetadataType metadataType, string name, string value)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.AddBulkMetadata(configuration, sessionId, responseParameter, domainId, rootMapId, node, relationship, descriptorType, metadataType, name, value));

            return result;
        }

        public MapParameter DeleteMetadata(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter metadata)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.DeleteMetadata(configuration, sessionId, domainId, metadata));

            return result;
        }

        public MapParameter DeleteBulkMetadata(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter metadata)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.DeleteBulkMetadata(configuration, sessionId, responseParameter, domainId, metadata));

            return result;
        }

        public MapParameter UpdateMetadata(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter metadata, string name, string value)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.UpdateMetadata(configuration, sessionId, domainId, metadata, name, value));

            return result;
        }

        public MapParameter UpdateBulkMetadata(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter metadata, string name, string value)
        {
            MapParameter result = null;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.UpdateBulkMetadata(configuration, sessionId, responseParameter, domainId, metadata, name, value));

            return result;
        }

        public bool IsSessionCompleted(GlymaSessionConfiguration configuration, Guid sessionId)
        {
            bool result = false;

            // run the call against the application proxy
            NodeServiceApplicationProxy.Invoke(_serviceContext,
                proxy => result = proxy.IsSessionCompleted(configuration, sessionId));

            return result;
        }

        public void Dispose()
        {
            if (_site != null)
            {
                _site.Dispose();
                _site = null;
            }
        }
    }
}
