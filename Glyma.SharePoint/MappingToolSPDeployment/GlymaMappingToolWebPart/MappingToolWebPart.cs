using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;

using SevenSigma.MappingTool;
using System.Text;

namespace SevenSigma.MappingTool.MappingToolWebPart
{
    [ToolboxItemAttribute(false)]
    public class MappingToolWebPart : WebPart, INodeControlConnector
    {
        private List<string> _displayClientIDs = new List<string>();

        private GlymaMappingToolWebPart.MappingToolWebPartUserControl _mappingToolControl;
        private string domainUid;
        private string nodeUid;

        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private string _ascxPath = string.Empty;

        private string clientScriptBody =
            @"
                methodName = '[clientID]' + 'RunTransform';
                eval(methodName + '(' + nodeXML + ')');
            ";

        protected override void CreateChildControls()
        {
            Controls.Clear();

#if SP2010
            string versionedControlTemplatesFolder = @"~/_CONTROLTEMPLATES";
#endif
#if SP2013
            string versionedControlTemplatesFolder = SPUrlUtility.CombineUrl("/", SPUtility.ContextControlTemplatesFolder);
#endif

            _ascxPath = versionedControlTemplatesFolder + "/Glyma/MappingToolWebPartUserControl.ascx";

            _mappingToolControl = Page.LoadControl(_ascxPath) as GlymaMappingToolWebPart.MappingToolWebPartUserControl;
            if (_mappingToolControl != null)
            {
                Controls.Add(_mappingToolControl);
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            StringBuilder sb = new StringBuilder();

            if (_displayClientIDs.Count != 0)
            {
                sb.AppendLine("<script language=\"JavaScript\">");
                sb.AppendLine("    function sendMessage(nodeXML)");
                sb.AppendLine("    {");
                sb.AppendLine("        var methodName;");

                foreach (string clientID in _displayClientIDs)
                {
                    sb.Append(clientScriptBody.Replace("[clientID]", clientID));
                    sb.AppendLine();
                }

                sb.AppendLine("    }");
                sb.AppendLine("</script>");
            }
            writer.Write(sb.ToString());
            base.RenderContents(writer);
        }

        [WebBrowsable(true),
         WebDisplayName("Domain ID"),
         Category("Glyma Mapping Tool Properties"),
         DefaultValue(""),
         Personalizable(PersonalizationScope.Shared),
         Description("Specify the starting Domain ID")]
        public string DomainUid
        {
            get { return domainUid; }
            set { domainUid = value; }
        }

        [WebBrowsable(true),
         WebDisplayName("Map Node ID"),
         Category("Glyma Mapping Tool Properties"),
         DefaultValue(""),
         Personalizable(PersonalizationScope.Shared),
         Description("Specify the starting Map Node ID")]
        public string NodeUid
        {
            get { return nodeUid; }
            set { nodeUid = value; }
        }

        [ConnectionProvider("Map Node", "NodeControlConnectorProvider", AllowsMultipleConnections=true)]
        public INodeControlConnector NodeControlConnectorProvider()
        {
            return this;
        }

        public void AddListener(string clientID)
        {
            if (!_displayClientIDs.Contains(clientID))
            {
                _displayClientIDs.Add(clientID);
            }
        }
    }
}
