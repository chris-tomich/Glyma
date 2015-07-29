using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using TransactionalNodeService;
using TransactionalNodeService.Common;
using TransactionalNodeService.Common.Model;
using TransactionalNodeService.Common.NodeServiceOperations.Bulk;
using TransactionalNodeService.SharePoint;

namespace Glyma.NodeServiceApp
{
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        ConcurrencyMode = ConcurrencyMode.Multiple,
        IncludeExceptionDetailInFaults = true)]
    [System.Runtime.InteropServices.Guid("FBAAA46D-27C0-45D2-8ACF-0466A1D08A13")]
    public class NodeServiceApplication : SPIisWebServiceApplication, IGlymaNodeServiceApp
    {
        private TransactionalMappingToolServiceCommonBase _base = null;

        public NodeServiceApplication()
            : base()
        {
        }

        private NodeServiceApplication(string name, NodeService service, SPIisWebServiceApplicationPool appPool)
            : base(name, service, appPool)
        {
        }

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

        public static NodeServiceApplication Create(string name, NodeService service, SPIisWebServiceApplicationPool appPool)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            if (appPool == null)
            {
                throw new ArgumentNullException("appPool");
            }

            // Create the service application.
            NodeServiceApplication serviceApplication = new NodeServiceApplication(name, service, appPool);
            serviceApplication.Update();

            // Register the supported endpoints.
            serviceApplication.AddServiceEndpoint("http", SPIisWebServiceBindingType.Http);
            serviceApplication.AddServiceEndpoint("https", SPIisWebServiceBindingType.Https, "secure");

            return serviceApplication;
        }

        protected override string DefaultEndpointName
        {
            get
            {
                return "http";
            }
        }

        public override string TypeName
        {
            get
            {
                return "Glyma Node Service Application";
            }
        }

        protected override string InstallPath
        {
            get
            {
#if SP2010
                return SPUtility.GetGenericSetupPath(@"WebServices\GlymaNodeServiceApp");
#endif
#if SP2013
                return SPUtility.GetVersionedGenericSetupPath(@"WebServices\GlymaNodeServiceApp", SPUtility.ContextCompatibilityLevel);
#endif
            }
        }

        protected override string VirtualPath
        {
            get
            {
                return "NodeService.svc";
            }
        }

        public override Guid ApplicationClassId
        {
            get { return new Guid("FBAAA46D-27C0-45D2-8ACF-0466A1D08A13"); }
        }

        public override Version ApplicationVersion
        {
            get { return new Version("1.0.0.0"); }
        }

        public override SPAdministrationLink ManageLink
        {
            get
            {
                return new SPAdministrationLink("/_admin/GlymaNodeServiceApp/Manage.aspx");
            }
        }

        public override SPAdministrationLink PropertiesLink
        {
            get
            {
                return new SPAdministrationLink("/_admin/GlymaNodeServiceApp/Manage.aspx");
            }
        }

        #region IGlymaNodeServiceApp Implementation
        public GlymaVersion GetApplicationVersion()
        {
            return GlymaServerVersion.Current;
        }

        public MapResponse CreateDomain(GlymaSessionConfiguration configuration, string name)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration))
            {
                return Base.CreateDomain(glymaSession, name);
            }
        }

        public QueryResponse CreateRootMap(GlymaSessionConfiguration configuration, Guid domainId, string name, NodeType nodeType, string originalId)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration))
            {
                return Base.CreateRootMap(glymaSession, domainId, name, nodeType, originalId);
            }
        }

        public int DeleteDomain(GlymaSessionConfiguration configuration, Guid domainId)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration))
            {
                return Base.DeleteDomain(glymaSession, domainId);
            }
        }

        public int DeleteRootMap(GlymaSessionConfiguration configuration, Guid domainId, Guid rootMapId)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration))
            {
                return Base.DeleteRootMap(glymaSession, domainId,rootMapId);
            }
        }

        public Guid BeginTransaction(GlymaSessionConfiguration configuration)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration))
            {
                return Base.BeginTransaction(glymaSession);
            }
        }

        public MapResponse CompleteTransaction(GlymaSessionConfiguration configuration, Guid sessionId)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration, sessionId, true))
            {
                return Base.CompleteTransaction(glymaSession, sessionId);
            }
        }

        public TypeResponse GetAllMapTypes(GlymaSessionConfiguration configuration)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration))
            {
                return Base.GetAllMapTypes(glymaSession);
            }
        }

        public Dictionary<Guid, QueryResponse> QueryDomains(GlymaSessionConfiguration configuration)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration))
            {
                return Base.QueryDomains(glymaSession);
            }
        }

        public QueryResponse QueryMap(GlymaSessionConfiguration configuration, Guid domainId, Guid nodeId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions)
        {
            return QueryMap(configuration, domainId, nodeId, maxDepth, false, edgeConditions, filterConditions, 0);
        }

        public QueryResponse QueryMapPaged(GlymaSessionConfiguration configuration, Guid domainId, Guid nodeId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions, int objectIndex)
        {
            return QueryMap(configuration, domainId, nodeId, maxDepth, false, edgeConditions, filterConditions, objectIndex);
        }

        public QueryResponse QueryMap(GlymaSessionConfiguration configuration, Guid domainId, Guid nodeId, int maxDepth, bool isFullDomainSearch, EdgeConditions edgeConditions, FilterConditions filterConditions, int objectIndex)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration))
            {
                return Base.QueryMap(glymaSession, domainId, nodeId, maxDepth, isFullDomainSearch, edgeConditions, filterConditions, objectIndex);
            }
        }

        public QueryResponse QueryMapByDomain(GlymaSessionConfiguration configuration, Guid domainId, int maxDepth, EdgeConditions edgeConditions, FilterConditions filterConditions)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration))
            {
                return Base.QueryMapByDomain(glymaSession, domainId, maxDepth, edgeConditions, filterConditions);
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

            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration))
            {
                return Base.SearchMap(glymaSession, domainId, rootMapUid, conditions, pageNumber, pageSize);
            }
        }

        public MapParameter AddNode(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter rootMapId, NodeType nodeType, string originalId)
        {
            return AddBulkNode(configuration, sessionId, Guid.Empty, domainId, rootMapId, nodeType, originalId);
        }

        public MapParameter AddBulkNode(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, NodeType nodeType, string originalId)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration, sessionId))
            {
                return Base.AddBulkNode(glymaSession, sessionId, responseParameter, domainId, rootMapId, nodeType, originalId);
            }
        }

        public MapParameter AddRelationship(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter rootMapId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType, string originalId)
        {
            return AddBulkRelationship(configuration, sessionId, Guid.Empty, domainId, rootMapId, nodes, relationshipType, originalId);
        }

        public MapParameter AddBulkRelationship(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType, string originalId)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration, sessionId))
            {
                return Base.AddBulkRelationship(glymaSession, sessionId, responseParameter, domainId, rootMapId, nodes, relationshipType, originalId);
            }
        }

        public MapParameter AddMetadata(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter rootMapId, MapParameter node, MapParameter relationship, DescriptorType descriptorType, MetadataType metadataType, string name, string value)
        {
            return AddBulkMetadata(configuration, sessionId, Guid.Empty, domainId, rootMapId, node, relationship, descriptorType, metadataType, name, value);
        }

        public MapParameter AddBulkMetadata(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter rootMapId, MapParameter node, MapParameter relationship, DescriptorType descriptorType, MetadataType metadataType, string name, string value)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration, sessionId))
            {
                return Base.AddBulkMetadata(glymaSession, sessionId, responseParameter, domainId, rootMapId, node, relationship, descriptorType, metadataType, name, value);
            }
        }

        public MapParameter DeleteNode(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter nodeId)
        {
            return DeleteBulkNode(configuration, sessionId, Guid.Empty, domainId, nodeId);
        }

        public MapParameter DeleteBulkNode(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter nodeId)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration, sessionId))
            {
                return Base.DeleteBulkNode(glymaSession, sessionId, responseParameter, domainId, nodeId);
            }
        }

        public MapParameter DeleteRelationship(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter relationshipId)
        {
            return DeleteBulkRelationship(configuration, sessionId, Guid.Empty, domainId, relationshipId);
        }

        public MapParameter DeleteBulkRelationship(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter relationshipId)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration, sessionId))
            {
                return Base.DeleteBulkRelationship(glymaSession, sessionId, responseParameter, domainId, relationshipId);
            }
        }

        public MapParameter DeleteMetadata(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter metadata)
        {
            return DeleteBulkMetadata(configuration, sessionId, Guid.Empty, domainId, metadata);
        }

        public MapParameter DeleteBulkMetadata(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter metadata)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration, sessionId))
            {
                return Base.DeleteBulkMetadata(glymaSession, sessionId, responseParameter, domainId, metadata);
            }
        }

        public MapParameter UpdateNode(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter nodeId, NodeType nodeType)
        {
            return UpdateBulkNode(configuration, sessionId, Guid.Empty, domainId, nodeId, nodeType);
        }

        public MapParameter UpdateBulkNode(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter nodeId, NodeType nodeType)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration, sessionId))
            {
                return Base.UpdateBulkNode(glymaSession, sessionId, responseParameter, domainId, nodeId, nodeType);
            }
        }

        public MapParameter UpdateRelationship(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter relationshipId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType)
        {
            return UpdateBulkRelationship(configuration, sessionId, Guid.Empty, domainId, relationshipId, nodes, relationshipType);
        }

        public MapParameter UpdateBulkRelationship(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter relationshipId, Dictionary<DescriptorType, MapParameter> nodes, RelationshipType relationshipType)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration, sessionId))
            {
                return Base.UpdateBulkRelationship(glymaSession, sessionId, responseParameter, domainId, relationshipId, nodes, relationshipType);
            }
        }

        public MapParameter UpdateMetadata(GlymaSessionConfiguration configuration, Guid sessionId, MapParameter domainId, MapParameter metadata, string name, string value)
        {
            return UpdateBulkMetadata(configuration, sessionId, Guid.Empty, domainId, metadata, name, value);
        }

        public MapParameter UpdateBulkMetadata(GlymaSessionConfiguration configuration, Guid sessionId, Guid responseParameter, MapParameter domainId, MapParameter metadata, string name, string value)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration, sessionId))
            {
                return Base.UpdateBulkMetadata(glymaSession, sessionId, responseParameter, domainId, metadata, name, value);
            }
        }

        public bool IsSessionCompleted(GlymaSessionConfiguration configuration, Guid sessionId)
        {
            using (IGlymaSession glymaSession = new SvcAppSPGlymaSession(configuration, sessionId))
            {
                return Base.IsSessionCompleted(glymaSession, sessionId);
            }
        }
        #endregion
    }
}
