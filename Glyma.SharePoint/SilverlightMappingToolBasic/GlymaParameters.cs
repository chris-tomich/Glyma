using System.Collections.Generic;
using System.Windows.Browser;

namespace SilverlightMappingToolBasic
{
    public class GlymaParameters
    {
        private const string FileUploaderUrlKey = "fileUploaderUrl";
        private const string FileUploadLibraryKey = "fileUploadLibrary";
        private const string CurrentSiteUrlKey = "currentSiteUrl";
        private const string UserGroupServiceUrlKey = "userGroupServiceUrl";
        private const string TransactionalMappingToolSvcUrlKey = "transactionalMappingToolSvcUrl";
        private const string MappingToolServiceUrlKey = "mappingToolServiceUrl";
        private const string GlymaIconLibraryUrlKey = "glymaIconLibraryUrl";
        private const string GlymaSecuritySvcUrlKey = "glymaSecuritySvcUrl";
        private const string GlymaUtilitySvcUrlKey = "glymaUtilitySvcUrl";

        public string SiteUrl { get; private set; }
        public string FileUploaderUrl { get; private set; }
        public string FileUploadLibrary { get; private set; }
        public string UserGroupServiceUrl { get; private set; }
        public string TransactionalMappingToolSvcUrl { get; private set; }
        public string MappingToolServiceUrl { get; private set; }
        public string GlymaIconLibraryUrl { get; private set; }

        public string GlymaSecuritySvcUrl { get; private set; }

        public string GlymaUtilitySvcUrl { get; private set; }

        public const int NodeImageHeight = 25;
        public const int NodeImageWidth = 25;
        public const int NodeTextWidth = 140;
        public const int Margin = 20;

        public bool HasVideoPlaying { get; set; }

        public GlymaParameters(IDictionary<string, string> initParms)
        {
            if (initParms.ContainsKey(FileUploaderUrlKey))
            {
                FileUploaderUrl = HttpUtility.UrlDecode(initParms[FileUploaderUrlKey]);
            }

            if (initParms.ContainsKey(FileUploadLibraryKey))
            {
                FileUploadLibrary = HttpUtility.UrlDecode(initParms[FileUploadLibraryKey]);
            }

            if (initParms.ContainsKey(CurrentSiteUrlKey))
            {
                SiteUrl = HttpUtility.UrlDecode(initParms[CurrentSiteUrlKey]);
            }

            if (initParms.ContainsKey(UserGroupServiceUrlKey))
            {
                UserGroupServiceUrl = HttpUtility.UrlDecode(initParms[UserGroupServiceUrlKey]);
            }

            if (initParms.ContainsKey(TransactionalMappingToolSvcUrlKey))
            {
                TransactionalMappingToolSvcUrl = HttpUtility.UrlDecode(initParms[TransactionalMappingToolSvcUrlKey]);
            }

            if (initParms.ContainsKey(MappingToolServiceUrlKey))
            {
                MappingToolServiceUrl = HttpUtility.UrlDecode(initParms[MappingToolServiceUrlKey]);
            }

            if (initParms.ContainsKey(GlymaIconLibraryUrlKey))
            {
                GlymaIconLibraryUrl = HttpUtility.UrlDecode(initParms[GlymaIconLibraryUrlKey]);
            }

            if (initParms.ContainsKey(GlymaSecuritySvcUrlKey))
            {
                GlymaSecuritySvcUrl = HttpUtility.UrlDecode(initParms[GlymaSecuritySvcUrlKey]);
            }

            if (initParms.ContainsKey(GlymaUtilitySvcUrlKey))
            {
                GlymaUtilitySvcUrl = HttpUtility.UrlDecode(initParms[GlymaUtilitySvcUrlKey]);
            }
        }

        
    }
}
