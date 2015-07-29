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
using TransactionalNodeService.SharePoint;
using TransactionalNodeService.SharePoint.SecurityModel;
using System.IO;
using Ionic.Zlib;

namespace TransactionalNodeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TransactionalMappingToolService" in code, svc and config file together.
    [BasicHttpBindingServiceMetadataExchangeEndpointAttribute]
    [AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Required)]
#if !DEBUG
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, Namespace = "http://sevensigma.com.au/TransactionalNodeService")]
#endif
#if DEBUG
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, Namespace = "http://sevensigma.com.au/TransactionalNodeService", IncludeExceptionDetailInFaults = true)]
#endif
    public class TransactionalMappingToolService : ITransactionalMappingToolService
    {
        public GlymaVersion GetApplicationVersion()
        {
            return TransactionalNodeService.Common.GlymaServerVersion.Current;
        }

        public MapResponse CreateDomain(string callingUrl, string name)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, null, null, SPGlymaRightFactory.Instance.ProjectCreateRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.CreateDomain(configuration, name);
                }
            }
        }

        public QueryResponse CreateRootMap(string callingUrl, Guid domainId, string name, NodeType nodeType, string originalId)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, domainId, null, SPGlymaRightFactory.Instance.RootMapCreateRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.CreateRootMap(configuration, domainId, name, nodeType, originalId);
                }
            }
        }

        public int DeleteDomain(string callingUrl, Guid domainId)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, domainId, null, SPGlymaRightFactory.Instance.RootMapCreateRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.DeleteDomain(configuration, domainId);
                }
            }
        }

        public int DeleteRootMap(string callingUrl, Guid domainId, Guid rootMapId)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, domainId, null, SPGlymaRightFactory.Instance.RootMapCreateRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.DeleteRootMap(configuration, domainId, rootMapId);
                }
            }
        }

        public Guid BeginTransaction(string callingUrl)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, null, null, SPGlymaRightFactory.Instance.TransactionRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.BeginTransaction(configuration);
                }
            }
        }

        public MapResponse CompleteTransaction(string callingUrl, Guid sessionId)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, null, null, SPGlymaRightFactory.Instance.TransactionRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.CompleteTransaction(configuration, sessionId);
                }
            }
        }

        public TypeResponse GetAllMapTypes(string callingUrl)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, null, null, SPGlymaRightFactory.Instance.BenignAccessRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.GetAllMapTypes(configuration);
                }
            }
        }

        public Dictionary<Guid, QueryResponse> QueryDomains(string callingUrl)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                /// This method requires security trimming so there is no need to check authentication here.
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    Dictionary<Guid, QueryResponse> allDomains = nodeServiceClient.QueryDomains(configuration);

                    GlymaSecurityTrimmer securityTrimmer = new GlymaSecurityTrimmer(glymaSession.Web, glymaSession);

                    Dictionary<Guid, QueryResponse> authorisedDomains = securityTrimmer.TrimResponse(allDomains, SPGlymaRightFactory.Instance.ProjectReadRight);

                    return authorisedDomains;
                }
            }
        }

        public QueryResponse QueryMap(string callingUrl, Guid domainId, Guid nodeId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                /// This method requires security trimming so there is no need to check authentication here.
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    /// This method doesn't traverse rootmap boundaries.
                    GlymaSecurityTrimmer securityTrimmer = new GlymaSecurityTrimmer(glymaSession.Web, glymaSession);

                    securityTrimmer.CheckRootMapAuthorisationBaseOnNode(domainId, nodeId, SPGlymaRightFactory.Instance.MapReadRight);

                    QueryResponse response = nodeServiceClient.QueryMap(configuration, domainId, nodeId, maxDepth, edgeConditions, filterConditions);
                    response.CompressResponse();

                    return response;
                }
            }
        }

        public QueryResponse QueryMapPaged(string callingUrl, Guid domainId, Guid nodeId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions, int objectIndex)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                /// This method requires security trimming so there is no need to check authentication here.
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    /// This method doesn't traverse rootmap boundaries.
                    GlymaSecurityTrimmer securityTrimmer = new GlymaSecurityTrimmer(glymaSession.Web, glymaSession);

                    securityTrimmer.CheckRootMapAuthorisationBaseOnNode(domainId, nodeId, SPGlymaRightFactory.Instance.MapReadRight);

                    QueryResponse response = nodeServiceClient.QueryMapPaged(configuration, domainId, nodeId, maxDepth, edgeConditions, filterConditions, objectIndex);
                    response.CompressResponse();

                    return response;
                }
            }
        }

        public QueryResponse QueryMapByDomain(string callingUrl, Guid domainId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                /// This method requires security trimming so there is no need to check authentication here.
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    QueryResponse allRootMaps = nodeServiceClient.QueryMapByDomain(configuration, domainId, maxDepth, edgeConditions, filterConditions);

                    /// This method DOES traverse rootmap boundaries.
                    GlymaSecurityTrimmer securityTrimmer = new GlymaSecurityTrimmer(glymaSession.Web, glymaSession);

                    QueryResponse trimmedRootMaps = securityTrimmer.TrimResponse(allRootMaps, SPGlymaRightFactory.Instance.RootMapReadRight);
                    trimmedRootMaps.CompressResponse();

                    return trimmedRootMaps;
                }
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
        public SearchMapResult SearchMap(string callingUrl, Guid domainId, Guid rootMapUid, SearchConditions conditions, int pageNumber, int pageSize)
        {
            if (pageNumber == 0)
            {
                pageNumber = 1;
            }

            if (pageSize == 0)
            {
                pageSize = 5;
            }

            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, domainId, rootMapUid, SPGlymaRightFactory.Instance.MapReadRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.SearchMap(configuration, domainId, rootMapUid, conditions, pageNumber, pageSize);
                }
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
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, true, domainId, rootMapId, SPGlymaRightFactory.Instance.MapCreateRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.AddBulkNode(configuration, sessionId, responseParameter, domainId, rootMapId, nodeType, originalId);
                }
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
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, true, domainId, rootMapId, SPGlymaRightFactory.Instance.MapCreateRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.AddBulkRelationship(configuration, sessionId, responseParameter, domainId, rootMapId, nodes, relationshipType, originalId);
                }
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
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, true, domainId, rootMapId, SPGlymaRightFactory.Instance.MapCreateRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.AddBulkMetadata(configuration, sessionId, responseParameter, domainId, rootMapId, node, relationship, descriptorType, metadataType, name, value);
                }
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
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, true, domainId, null, SPGlymaRightFactory.Instance.MapDeleteRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.DeleteBulkNode(configuration, sessionId, responseParameter, domainId, nodeId);
                }
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
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, true, domainId, null, SPGlymaRightFactory.Instance.MapDeleteRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.DeleteBulkRelationship(configuration, sessionId, responseParameter, domainId, relationshipId);
                }
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
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, true, domainId, null, SPGlymaRightFactory.Instance.MapDeleteRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.DeleteBulkMetadata(configuration, sessionId, responseParameter, domainId, metadata);
                }
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
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, true, domainId, null, SPGlymaRightFactory.Instance.MapUpdateRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.UpdateBulkNode(configuration, sessionId, responseParameter, domainId, nodeId, nodeType);
                }
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
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, true, domainId, null, SPGlymaRightFactory.Instance.MapUpdateRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.UpdateBulkRelationship(configuration, sessionId, responseParameter, domainId, relationshipId, nodes, relationshipType);
                }
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
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, true, domainId, null, SPGlymaRightFactory.Instance.MapUpdateRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.UpdateBulkMetadata(configuration, sessionId, responseParameter, domainId, metadata, name, value);
                }
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

        public string SubmitCompressedBulkOperations(string callingUrl, Guid sessionId, string compressedOperations)
        {
            if (string.IsNullOrEmpty(compressedOperations))
            {
                return CompressBulkOperationsResponse(new List<BulkOperationResponse>());
            }

            BulkOperations operations = DecompressBulkOperations(compressedOperations);

            return CompressBulkOperationsResponse(SubmitBulkOperations(callingUrl, sessionId, operations));
        }

        private BulkOperations DecompressBulkOperations(string compressedOperations)
        {
            MemoryStream compressedOperationsStream = null;
            MemoryStream decompressedOperationsStream = null;
            ZlibStream decompressionStream = null;

            try
            {
                byte[] compressedOperationsBytes = Convert.FromBase64String(compressedOperations);
                compressedOperationsStream = new MemoryStream(compressedOperationsBytes);
                byte[] buffer = new byte[1024];
                int numBytesRead = 0;
                bool start = true;

                decompressedOperationsStream = new MemoryStream();

                using (decompressionStream = new ZlibStream(compressedOperationsStream, CompressionMode.Decompress))
                {
                    while (start || numBytesRead > 0)
                    {
                        numBytesRead = decompressionStream.Read(buffer, 0, buffer.Length);

                        if (numBytesRead > 0)
                        {
                            decompressedOperationsStream.Write(buffer, 0, numBytesRead);
                        }

                        start = false;
                    }
                }

                decompressedOperationsStream.Position = 0;

                DataContractSerializer deserializer = new DataContractSerializer(typeof(BulkOperations));

                BulkOperations operations = deserializer.ReadObject(decompressedOperationsStream) as BulkOperations;

                return operations;
            }
            finally
            {
                if (decompressedOperationsStream != null)
                {
                    decompressedOperationsStream.Dispose();
                    decompressedOperationsStream = null;
                }
            }
        }

        private string CompressBulkOperationsResponse(List<BulkOperationResponse> bulkOperationsResponses)
        {
            MemoryStream bulkOperationsResponsesXml = null;
            MemoryStream compressedBulkOperationsResponsesStream = null;
            ZlibStream zipStream = null;

            try
            {
                bulkOperationsResponsesXml = new MemoryStream();

                DataContractSerializer serializer = new DataContractSerializer(typeof(List<BulkOperationResponse>));

                serializer.WriteObject(bulkOperationsResponsesXml, bulkOperationsResponses);

                byte[] metadataCollectionBytes = bulkOperationsResponsesXml.ToArray();
                compressedBulkOperationsResponsesStream = new MemoryStream();

                using (zipStream = new ZlibStream(compressedBulkOperationsResponsesStream, CompressionMode.Compress))
                {
                    zipStream.Write(metadataCollectionBytes, 0, metadataCollectionBytes.Length);
                }

                byte[] compressedMetadataCollectionBytes = compressedBulkOperationsResponsesStream.ToArray();

                string compressedResponses = Convert.ToBase64String(compressedMetadataCollectionBytes);

                return compressedResponses;
            }
            finally
            {
                if (bulkOperationsResponsesXml != null)
                {
                    bulkOperationsResponsesXml.Dispose();
                    bulkOperationsResponsesXml = null;
                }
            }
        }

        public bool IsSessionCompleted(string callingUrl, Guid sessionId)
        {
            using (GlymaNSApp.NodeServiceClient nodeServiceClient = new GlymaNSApp.NodeServiceClient(callingUrl))
            {
                using (WebAppSPGlymaSession glymaSession = new WebAppSPGlymaSession(callingUrl, null, null, SPGlymaRightFactory.Instance.TransactionRight))
                {
                    GlymaSessionConfiguration configuration = glymaSession.ExportGlymaSession();

                    return nodeServiceClient.IsSessionCompleted(configuration, sessionId);
                }
            }
        }
    }
}
