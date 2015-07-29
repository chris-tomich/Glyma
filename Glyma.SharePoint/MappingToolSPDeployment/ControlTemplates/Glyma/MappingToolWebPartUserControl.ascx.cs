using System;
using System.Web;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;

namespace SevenSigma.MappingTool.GlymaMappingToolWebPart
{
    public partial class MappingToolWebPartUserControl : UserControl
    {
        private string _initParameters;
#if SP2010
        private string _serverRelativeVersionedLayoutsFolder = "/_layouts";
#endif
#if SP2013
        private string _serverRelativeVersionedLayoutsFolder = SPUrlUtility.CombineUrl("/", SPUtility.ContextLayoutsFolder);
#endif

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public MappingToolWebPartUserControl()
        {
            CurrentSiteUrl = SPContext.Current.Web.Url;
            FileUploaderUrl = CurrentSiteUrl + GetServerRelativeVersionedLayoutsFolder() + "/Glyma/LargeFileUpload/LargeFileUpload.ashx";
            FileUploadLibrary = "Shared Documents";

            BaseUrl = SPContext.Current.Site.Url;
            MappingToolServiceUrl = BaseUrl + "/_vti_bin/SevenSigma/MappingToolService.svc";
            ThemeServiceUrl = BaseUrl + "/_vti_bin/SevenSigma/ThemeService.svc";
            LoggingServiceUrl = BaseUrl + "/_vti_bin/SevenSigma/LoggingService.svc";
            UserGroupServiceUrl = BaseUrl + "/_vti_bin/UserGroup.asmx";
            TransactionalMappingToolSvcUrl = BaseUrl + "/_vti_bin/sevensigma/transactionalmappingtoolservice.svc";
            GlymaIconLibraryUrl = BaseUrl + "/Style Library/Glyma/Icons";
            GlymaSecuritySvcUrl = BaseUrl + "/_vti_bin/SevenSigma/GlymaSecurityService.svc";
            GlymaUtilitySvcUrl = BaseUrl + "/_vti_bin/SevenSigma/GlymaUtilityService.svc";

            BuildInitParameters();

            try
            {
                string loginName = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                CurrentUserName = SPContext.Current.Web.EnsureUser(loginName).Name;
            }
            catch (Exception)
            {
                CurrentUserName = string.Empty;
            }
        }

        private void BuildInitParameters()
        {
            _initParameters = "currentSiteUrl=" + HttpUtility.UrlEncode(CurrentSiteUrl);
            _initParameters += ",fileUploaderUrl=" + HttpUtility.UrlEncode(FileUploaderUrl);
            _initParameters += ",fileUploadLibrary=" + HttpUtility.UrlEncode(FileUploadLibrary);
            _initParameters += ",userGroupServiceUrl=" + HttpUtility.UrlEncode(UserGroupServiceUrl);
            _initParameters += ",transactionalMappingToolSvcUrl=" + HttpUtility.UrlEncode(TransactionalMappingToolSvcUrl);
            _initParameters += ",mappingToolServiceUrl=" + HttpUtility.UrlEncode(MappingToolServiceUrl);
            _initParameters += ",glymaIconLibraryUrl=" + HttpUtility.UrlEncode(GlymaIconLibraryUrl);
            _initParameters += ",glymaSecuritySvcUrl=" + HttpUtility.UrlEncode(GlymaSecuritySvcUrl);
            _initParameters += ",glymaUtilitySvcUrl=" + HttpUtility.UrlEncode(GlymaUtilitySvcUrl);
        }

        protected string GetServerRelativeVersionedLayoutsFolder()
        {
           return _serverRelativeVersionedLayoutsFolder;
        }

        protected string MappingToolServiceUrl
        {
            get;
            private set;
        }

        protected string ThemeServiceUrl
        {
            get;
            private set;
        }


        protected string LoggingServiceUrl
        {
            get;
            private set;
        }

        protected string CurrentSiteUrl
        {
            get;
            private set;
        }

        protected string BaseUrl
        {
            get;
            private set;
        }

        protected string FileUploadLibrary
        {
            get;
            private set;
        }

        protected string FileUploaderUrl
        {
            get;
            private set;
        }

        protected string UserGroupServiceUrl
        {
            get;
            private set;
        }

        protected string TransactionalMappingToolSvcUrl
        {
            get;
            private set;
        }

        protected string GlymaSecuritySvcUrl
        {
            get;
            private set;
        }

        protected string GlymaUtilitySvcUrl
        {
            get;
            private set;
        }

        protected string GlymaIconLibraryUrl
        {
            get;
            private set;
        }

        protected string CurrentUserName
        {
            get;
            private set;
        }

        protected string InitParameters
        {
            get
            {
                return _initParameters;
            }
        }
    }
}
