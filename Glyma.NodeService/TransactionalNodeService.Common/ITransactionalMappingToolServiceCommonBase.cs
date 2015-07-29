using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransactionalNodeService.Common.Model;

namespace TransactionalNodeService.Common
{
    public interface ITransactionalMappingToolServiceCommonBase
    {
        MapResponse CreateDomain(IGlymaSession glymaSession, string name);

        QueryResponse CreateRootMap(IGlymaSession glymaSession, Guid domainId, string name, NodeType nodeType, string originalId);

        Guid BeginTransaction(IGlymaSession glymaSession);

        MapResponse CompleteTransaction(IGlymaSession glymaSession, Guid sessionId);

        TypeResponse GetAllMapTypes(IGlymaSession glymaSession);

        Dictionary<Guid, QueryResponse> QueryDomains(IGlymaSession glymaSession);

        QueryResponse QueryMap(IGlymaSession glymaSession, Guid domainId, Guid nodeId, int maxDepth, bool isFullDomainSearch, EdgeConditions edgeConditions, FilterConditions filterConditions, int objectIndex);

        QueryResponse QueryMapByDomain(IGlymaSession glymaSession, Guid domainId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions);

        SearchMapResult SearchMap(IGlymaSession glymaSession, Guid domainId, Guid rootMapUid, SearchConditions conditions, int pageNumber = 1, int pageSize = 5);

        MapParameter AddBulkNode(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, NodeType nodeType, string originalId);

        MapParameter AddBulkRelationship(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType, string originalId);

        MapParameter AddBulkMetadata(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, MapParameter node, MapParameter relationship, DescriptorType descriptorType, MetadataType metadataType, string name, string value);

        MapParameter DeleteBulkNode(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter nodeId);

        MapParameter DeleteBulkRelationship(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter relationshipId);

        MapParameter DeleteBulkMetadata(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter metadata);

        MapParameter UpdateBulkNode(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter nodeId, NodeType nodeType);

        MapParameter UpdateBulkRelationship(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter relationshipId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType);

        MapParameter UpdateBulkMetadata(IGlymaSession glymaSession, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter metadata, string name, string value);

        bool IsSessionCompleted(IGlymaSession glymaSession, Guid sessionId);
    }
}
