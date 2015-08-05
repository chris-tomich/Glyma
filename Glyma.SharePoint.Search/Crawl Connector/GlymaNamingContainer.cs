using System;
using Microsoft.Office.Server.Search.Connector.BDC;

namespace Glyma.SharePoint.Search
{
    /// <summary>
    /// Defines how to map the external item to the URL used by the search system.
    /// </summary>
    public class GlymaNamingContainer : INamingContainer
    {
        private Uri _searchUri;


        public void Initialize(Uri uri)
        {
            _searchUri = uri;
        }


        public Guid PartitionId
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public Guid PropertySet
        {
            get
            {
                return GlymaModelConstants.PropertySetGuid;
            }
        }


        /// <summary>
        /// Maps the specified external item to the URL used by the search system.
        /// </summary>
        /// <param name="entityInstance">An IEntityInstance object containing details of the external item.</param>
        /// <returns>A Uri object containing the mapped URL.</returns>
        /// <remarks>
        /// The URL's are in the following formats:
        /// glyma://[Repository Name]/[Domain GUID]/[Root Map GUID]/[Parent Map GUID]/[Node GUID]/ - Defines the URL for a Glyma Map.
        /// glyma://[Repository Name]/[Domain GUID]/[Root Map GUID]/[Parent Map GUID]/[Node GUID] - Defines the URL for a Glyma Node.
        /// </remarks>
        public Uri GetAccessUri(Microsoft.BusinessData.Runtime.IEntityInstance entityInstance)
        {
            Uri entityUrl = null;
            if (entityInstance.Entity.Name.Equals(GlymaModelConstants.NodeEntityName, StringComparison.OrdinalIgnoreCase) || entityInstance.Entity.Name.Equals(GlymaModelConstants.MapEntityName, StringComparison.OrdinalIgnoreCase))
            {                
                object[] ids = entityInstance.GetIdentity().GetIdentifierValues();
                string repositoryName = ids[0].ToString();
                string domainIdString = ids[1].ToString();
                string rootMapIdString = ids[2].ToString();
                string mapIdString = ids[3].ToString();
                string nodeIdString = ids[4].ToString();
                string entityUrlString = GlymaModelConstants.Protocol + "://" + repositoryName + "/" + domainIdString + "/" + rootMapIdString + "/" + mapIdString + "/" + nodeIdString;
                if (entityInstance.Entity.Name.Equals(GlymaModelConstants.MapEntityName, StringComparison.OrdinalIgnoreCase))
                {
                    entityUrlString += "/";
                }
                entityUrl = new Uri(entityUrlString);
            }
            return entityUrl;
        }


        /// <summary>
        /// Maps the specified external item to the URL used by the search system.
        /// </summary>
        /// <param name="entityInstance">An IEntityInstance object containing details of the external item.</param>
        /// <param name="parentEntityInstance">An IEntityInstance object containing details of the parent external item.</param>
        /// <returns>A Uri object containing the mapped URL.</returns>
        public Uri GetAccessUri(Microsoft.BusinessData.Runtime.IEntityInstance entityInstance, Microsoft.BusinessData.Runtime.IEntityInstance parentEntityInstance)
        {
            return GetAccessUri(entityInstance);
        }


        public Uri GetAccessUri(Microsoft.BusinessData.MetadataModel.IEntity entity, Microsoft.BusinessData.MetadataModel.ILobSystemInstance lobSystemInstance)
        {
            return _searchUri;
        }


        public Uri GetAccessUri(Microsoft.BusinessData.MetadataModel.ILobSystemInstance lobSystemInstance)
        {
            throw new NotImplementedException();
        }


        public Uri GetAccessUri(Microsoft.BusinessData.MetadataModel.ILobSystem lobSystem)
        {
            throw new NotImplementedException();
        }


        public Uri GetDisplayUri(Microsoft.BusinessData.Runtime.IEntityInstance entityInstance, Microsoft.BusinessData.Runtime.IEntityInstance parentEntityInstance)
        {
            return GetDisplayUri(entityInstance, string.Empty);
        }


        /// <summary>
        /// Maps the specified external item to the display URL used by the search system. 
        /// </summary>
        /// <param name="entityInstance">An IEntityInstance object containing details of the external item.</param>
        /// <returns>A Uri object containing the mapped URL.</returns>
        /// <remarks>
        /// The display URL is what is displayed in the search results.  For glyma, the display URL is the same as the access URL.
        /// </remarks>
        public Uri GetDisplayUri(Microsoft.BusinessData.Runtime.IEntityInstance entityInstance, string computedDisplayUri)
        {
            Uri displayUri = null;

            if (string.IsNullOrEmpty(computedDisplayUri))
            {
                displayUri = GetAccessUri(entityInstance);
            }
            else
            {
                displayUri = new Uri(computedDisplayUri);
            }
            return displayUri;
        }


        public Uri GetDisplayUri(Microsoft.BusinessData.MetadataModel.IEntity entity, Microsoft.BusinessData.MetadataModel.ILobSystemInstance lobSystemInstance)
        {
            return _searchUri;
        }


        public Uri GetDisplayUri(Microsoft.BusinessData.MetadataModel.ILobSystemInstance lobSystemInstance)
        {
            throw new NotImplementedException(); 
        }


        public Uri GetDisplayUri(Microsoft.BusinessData.MetadataModel.ILobSystem lobSystem)
        {
            throw new NotImplementedException();
        }

    }
}
