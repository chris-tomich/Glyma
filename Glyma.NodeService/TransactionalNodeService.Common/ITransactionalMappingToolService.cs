using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using TransactionalNodeService.Common.Model;
using TransactionalNodeService.Common.NodeServiceOperations.Bulk;

namespace TransactionalNodeService.Common
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ITransactionalMappingToolService" in both code and config file together.
    [ServiceContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService")]
    public interface ITransactionalMappingToolService
    {
        [OperationContract]
        GlymaVersion GetApplicationVersion();

        [OperationContract]
        MapResponse CreateDomain(string callingUrl, string name);

        [OperationContract]
        QueryResponse CreateRootMap(string callingUrl, Guid domainId, string name, NodeType nodeType, string originalId);

        [OperationContract]
        int DeleteDomain(string callingUrl, Guid domainId);

        [OperationContract]
        int DeleteRootMap(string callingUrl, Guid domainId, Guid rootMapId);

        [OperationContract]
        Guid BeginTransaction(string callingUrl);

        [OperationContract]
        MapResponse CompleteTransaction(string callingUrl, Guid sessionId);

        [OperationContract]
        TypeResponse GetAllMapTypes(string callingUrl);

        [OperationContract]
        Dictionary<Guid, QueryResponse> QueryDomains(string callingUrl);

        [OperationContract]
        QueryResponse QueryMap(string callingUrl, Guid domainId, Guid nodeId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions);

        [OperationContract]
        QueryResponse QueryMapPaged(string callingUrl, Guid domainId, Guid nodeId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions, int objectIndex);

        [OperationContract]
        QueryResponse QueryMapByDomain(string callingUrl, Guid domainId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions);

        [OperationContract]
        SearchMapResult SearchMap(string callingUrl, Guid domainId, Guid rootMapUid, SearchConditions conditions, int pageNumber = 1, int pageSize = 5);

        [OperationContract(Name = "NA")]
        MapParameter AddNode(string callingUrl, Guid sessionId, MapParameter domainId, MapParameter rootMapId, NodeType nodeType, string originalId);

        [OperationContract(Name = "BNA")]
        MapParameter AddBulkNode(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, NodeType nodeType, string originalId);

        [OperationContract(Name = "ND")]
        MapParameter DeleteNode(string callingUrl, Guid sessionId, MapParameter domainId, MapParameter nodeId);

        [OperationContract(Name = "BND")]
        MapParameter DeleteBulkNode(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter nodeId);

        [OperationContract(Name = "NU")]
        MapParameter UpdateNode(string callingUrl, Guid sessionId, MapParameter domainId, MapParameter nodeId, NodeType nodeType);

        [OperationContract(Name = "BNU")]
        MapParameter UpdateBulkNode(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter nodeId, NodeType nodeType);

        [OperationContract(Name = "RA")]
        MapParameter AddRelationship(string callingUrl, Guid sessionId, MapParameter domainId, MapParameter rootMapId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType, string originalId);

        [OperationContract(Name = "BRA")]
        MapParameter AddBulkRelationship(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType, string originalId);

        [OperationContract(Name = "RD")]
        MapParameter DeleteRelationship(string callingUrl, Guid sessionId, MapParameter domainId, MapParameter relationshipId);

        [OperationContract(Name = "BRD")]
        MapParameter DeleteBulkRelationship(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter relationshipId);

        [OperationContract(Name = "RU")]
        MapParameter UpdateRelationship(string callingUrl, Guid sessionId, MapParameter domainId, MapParameter relationshipId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType);

        [OperationContract(Name = "BRU")]
        MapParameter UpdateBulkRelationship(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter relationshipId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType);

        [OperationContract(Name = "MA")]
        MapParameter AddMetadata(string callingUrl, Guid sessionId, MapParameter domainId, MapParameter rootMapId, MapParameter node, MapParameter relationship, DescriptorType descriptorType, MetadataType metadataType, string name, string value);

        [OperationContract(Name = "BMA")]
        MapParameter AddBulkMetadata(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, MapParameter node, MapParameter relationship, DescriptorType descriptorType, MetadataType metadataType, string name, string value);

        [OperationContract(Name = "MD")]
        MapParameter DeleteMetadata(string callingUrl, Guid sessionId, MapParameter domainId, MapParameter metadata);

        [OperationContract(Name = "BMD")]
        MapParameter DeleteBulkMetadata(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter metadata);

        [OperationContract(Name = "MU")]
        MapParameter UpdateMetadata(string callingUrl, Guid sessionId, MapParameter domainId, MapParameter metadata, string name, string value);

        [OperationContract(Name = "BMU")]
        MapParameter UpdateBulkMetadata(string callingUrl, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter metadata, string name, string value);

        [OperationContract(Name = "BO")]
        List<BulkOperationResponse> SubmitBulkOperations(string callingUrl, Guid sessionId, BulkOperations operations);

        [OperationContract(Name = "BOC")]
        string SubmitCompressedBulkOperations(string callingUrl, Guid sessionId, string compressedOperations);

        [OperationContract]
        bool IsSessionCompleted(string callingUrl, Guid sessionId);
    }
}
