using System;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace VideoPlayerSPDeploy.CONTROLTEMPLATES
{
    public partial class RelatedContentPanelWebPartUserControl : UserControl
    {

#if SP2010
        private readonly string _serverRelativeVersionedLayoutsFolder = "/_layouts";
#endif
#if SP2013
        private readonly string _serverRelativeVersionedLayoutsFolder = SPUrlUtility.CombineUrl("/", SPUtility.ContextLayoutsFolder);
#endif
        

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public RelatedContentPanelWebPartUserControl()
        {
            CurrentSiteUrl = SPContext.Current.Web.Url;
            BaseUrl = SPContext.Current.Site.Url;
        }

        protected string GetServerRelativeVersionedLayoutsFolder()
        {
           return _serverRelativeVersionedLayoutsFolder;
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
    }
}
