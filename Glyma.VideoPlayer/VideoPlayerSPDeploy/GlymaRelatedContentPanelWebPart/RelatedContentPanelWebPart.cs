using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;

namespace VideoPlayerSPDeploy.RelatedContentPanelWebPart
{
    [ToolboxItemAttribute(false)]
    public class RelatedContentPanelWebPart : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private string _ascxPath = string.Empty;

        protected override void CreateChildControls()
        {
            this.ChromeType = PartChromeType.None;
#if SP2013
            string versionedControlTemplatesFolder = SPUrlUtility.CombineUrl("/", SPUtility.ContextControlTemplatesFolder);
            _ascxPath = versionedControlTemplatesFolder + "/RelatedContentPanelWebPartUserControl.ascx";
#endif
#if SP2010
            _ascxPath = @"~/_CONTROLTEMPLATES/RelatedContentPanelWebPartUserControl.ascx";
#endif
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }
    }
}
