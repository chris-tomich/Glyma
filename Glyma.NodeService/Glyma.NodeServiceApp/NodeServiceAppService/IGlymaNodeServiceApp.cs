using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using TransactionalNodeService.Common;
using TransactionalNodeService.Common.Model;
using TransactionalNodeService.Common.NodeServiceOperations.Bulk;

namespace Glyma.NodeServiceApp
{
    [ServiceContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService")]
    public interface IGlymaNodeServiceApp
    {
        [OperationContract]
        GlymaVersion GetApplicationVersion();

        [OperationContract]
        MapResponse CreateDomain(GlymaSessionConfiguration configuration, string name);

        [OperationContract]
        QueryResponse CreateRootMap(GlymaSessionConfiguration configuration, Guid domainId, string name, NodeType nodeType, string originalId);

        [OperationContract]
        int DeleteDomain(GlymaSessionConfiguration configuration, Guid domainId);

        [OperationContract]
        int DeleteRootMap(GlymaSessionConfiguration configuration, Guid domainId, Guid rootMapId);

        [OperationContract]
        Guid BeginTransaction(GlymaSessionConfiguration configuration);

        [OperationContract]
        MapResponse CompleteTransaction(GlymaSessionConfiguration configuration, Guid sessionId);

        [OperationContract]
        TypeResponse GetAllMapTypes(GlymaSessionConfiguration configuration);

        [OperationContract]
        Dictionary<Guid, QueryResponse> QueryDomains(GlymaSessionConfiguration configuration);

        [OperationContract]
        QueryResponse QueryMap(GlymaSessionConfiguration configuration, Guid domainId, Guid nodeId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions);

        [OperationContract]
        QueryResponse QueryMapPaged(GlymaSessionConfiguration configuration, Guid domainId, Guid nodeId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions, int objectIndex);

        [OperationContract]
        QueryResponse QueryMapByDomain(GlymaSessionConfiguration configuration, Guid domainId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions);

        [OperationContract]
        SearchMapResult SearchMap(GlymaSessionConfiguration configuration, Guid domainId, Guid rootMapUid, SearchConditions conditions, int pageNumber, int pageSize);

        [OperationContract(Name = "NA")]
        MapParameter AddNode(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter rootMapId, NodeType nodeType, string originalId);

        [OperationContract(Name = "BNA")]
        MapParameter AddBulkNode(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, NodeType nodeType, string originalId);

        [OperationContract(Name = "ND")]
        MapParameter DeleteNode(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter nodeId);

        [OperationContract(Name = "BND")]
        MapParameter DeleteBulkNode(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter nodeId);

        [OperationContract(Name = "NU")]
        MapParameter UpdateNode(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter nodeId, NodeType nodeType);

        [OperationContract(Name = "BNU")]
        MapParameter UpdateBulkNode(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter nodeId, NodeType nodeType);

        [OperationContract(Name = "RA")]
        MapParameter AddRelationship(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter rootMapId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType, string originalId);

        [OperationContract(Name = "BRA")]
        MapParameter AddBulkRelationship(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType, string originalId);

        [OperationContract(Name = "RD")]
        MapParameter DeleteRelationship(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter relationshipId);

        [OperationContract(Name = "BRD")]
        MapParameter DeleteBulkRelationship(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter relationshipId);

        [OperationContract(Name = "RU")]
        MapParameter UpdateRelationship(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter relationshipId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType);

        [OperationContract(Name = "BRU")]
        MapParameter UpdateBulkRelationship(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter relationshipId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType);

        [OperationContract(Name = "MA")]
        MapParameter AddMetadata(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter rootMapId, MapParameter node, MapParameter relationship, DescriptorType descriptorType, MetadataType metadataType, string name, string value);

        [OperationContract(Name = "BMA")]
        MapParameter AddBulkMetadata(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, MapParameter node, MapParameter relationship, DescriptorType descriptorType, MetadataType metadataType, string name, string value);

        [OperationContract(Name = "MD")]
        MapParameter DeleteMetadata(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter metadata);

        [OperationContract(Name = "BMD")]
        MapParameter DeleteBulkMetadata(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter metadata);

        [OperationContract(Name = "MU")]
        MapParameter UpdateMetadata(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter metadata, string name, string value);

        [OperationContract(Name = "BMU")]
        MapParameter UpdateBulkMetadata(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter metadata, string name, string value);

        [OperationContract]
        bool IsSessionCompleted(GlymaSessionConfiguration configuration, Guid sessionId);
    }
}
