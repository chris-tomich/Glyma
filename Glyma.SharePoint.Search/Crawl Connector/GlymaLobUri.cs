using System;
using Microsoft.BusinessData.MetadataModel.Collections;
using Microsoft.Office.Server.Search.Connector.BDC;

namespace Glyma.SharePoint.Search
{
    /// <summary>
    /// Defines how to map the content item URL passed from the search system to the external system object for Glyma.
    /// </summary>
    public class GlymaLobUri : LobUri
    {
        private Microsoft.BusinessData.MetadataModel.IEntity _entity;
        private Microsoft.BusinessData.Runtime.Identity _identity;
        private Microsoft.BusinessData.MetadataModel.ILobSystem _lobSystem;
        private Microsoft.BusinessData.MetadataModel.ILobSystemInstance _lobSystemInstance;
        private Uri _sourceUri;


        public GlymaLobUri()
            : base(GlymaModelConstants.Protocol)
        {
        }


        /// <summary>
        /// Initialises the class to map the content item URL.
        /// </summary>
        /// <param name="context">An IConnectionContext object that contains the content item URL to map.</param>
        /// <remarks>
        /// The following URL's are supported:
        /// glyma://[Repository Name] - Defines a crawl start address for a Glyma repository defined in the BCS model file. 
        /// glyma://[Repository Name]/[Domain Guid] - Defines a crawl start adddress for the specified domain in a Glyma repository.  
        /// glyma://[Repository Name]/[Domain GUID]/[Root Map GUID]/[Parent Map GUID]/[Node GUID]/ - Defines the URL for a Glyma Map.
        /// glyma://[Repository Name]/[Domain GUID]/[Root Map GUID]/[Parent Map GUID]/[Node GUID] - Defines the URL for a Glyma Node.
        /// </remarks>
        public override void Initialize(Microsoft.Office.Server.Search.Connector.IConnectionContext context)
        {
            _sourceUri = context.Path;
            if (!_sourceUri.Scheme.Equals(GlymaModelConstants.Protocol, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Invalid Glyma URL provided.  The URL must start with: glyma://.");
            }

            _lobSystem = this.Catalog.GetLobSystem(GlymaModelConstants.LobSystemName);
            // Get the LOB system instance based on the host specified in the context path.  
            // The LOB system instance is passed as a parameter to the ExecuteStatic method in the GlymaRepositoryUtility class.            
            if (string.IsNullOrEmpty(_sourceUri.Host))
            {
                throw new ArgumentException("Invalid Glyma URL provided.  The URL must specify a repository name in the form: glyma://[Repository Name] where [Repository Name] is the name of a LOB system instance in the BCS model file.");
            }
            INamedLobSystemInstanceDictionary lobInstances = _lobSystem.GetLobSystemInstances();
            _lobSystemInstance = _lobSystem.GetLobSystemInstances().GetByKey(_sourceUri.Host, StringComparison.OrdinalIgnoreCase);
            if (_lobSystemInstance == null)
            {
                throw new ArgumentException("Invalid Glyma URL provided.  The repository name specified does not exist in the BCS model file.");
            }
 
            // Set the entity to be a Glyma Map by default.  When the URL is a crawl start address, it won't contain the identity of an entity instance.
            // By setting the entity but not the entity instance, the Finder method of the entity is executed to return entity instances to crawl. 
            _entity = this.Catalog.GetEntity(GlymaModelConstants.Namespace, GlymaModelConstants.MapEntityName);


            // Extract the entity instance identity from the context path.  The entity instance could either be a Glyma Map (which contains a trailing '/' in its URL) or a Glyma Node.
            // If an entity instance identity can be extracted, the SpecificFinder method of the entity is executed to return details for that entity instance.
            if (_sourceUri.Segments.Length == 5)
            {
                if (!_sourceUri.Segments[4].EndsWith("/"))
                {
                    _entity = this.Catalog.GetEntity(GlymaModelConstants.Namespace, GlymaModelConstants.NodeEntityName);
                }
                string domainIdString = _sourceUri.Segments[1].TrimEnd('/');
                string rootMapIdString = _sourceUri.Segments[2].TrimEnd('/');
                string mapIdString = _sourceUri.Segments[3].TrimEnd('/');
                string nodeIdString = _sourceUri.Segments[4].TrimEnd('/');

                Guid domainId = new Guid(domainIdString);
                Guid rootMapId = new Guid(rootMapIdString);
                Guid mapId = new Guid(mapIdString);
                Guid nodeId = new Guid(nodeIdString);

                Object[] identifier = { _lobSystemInstance.Name, domainId, rootMapId, mapId, nodeId };
                _identity = new Microsoft.BusinessData.Runtime.Identity(identifier);
            }
        }


        public override Microsoft.BusinessData.MetadataModel.IEntity Entity
        {
            get
            {
                return _entity;
            }
        }


        public override Microsoft.BusinessData.Runtime.Identity Identity
        {
            get
            {
                return _identity;
            }
        }


        public override Microsoft.BusinessData.MetadataModel.ILobSystem LobSystem
        {
            get
            {
                return _lobSystem;
            }
        }


        public override Microsoft.BusinessData.MetadataModel.ILobSystemInstance LobSystemInstance
        {
            get
            {
                return _lobSystemInstance;
            }
        }


        public override Guid PartitionId
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public override Uri SourceUri
        {
            get
            {
                return _sourceUri;
            }

            set
            {
                _sourceUri = value;
            }
        }
    }
}
