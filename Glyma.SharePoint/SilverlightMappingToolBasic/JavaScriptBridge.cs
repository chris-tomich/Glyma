using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;
using SilverlightMappingToolBasic.UI.Extensions.JavascriptCallback;
using SilverlightMappingToolBasic.UI.SuperGraph.View;
using SilverlightMappingToolBasic.UI.ViewModel;
using VideoPlayerSharedLib;

namespace SilverlightMappingToolBasic
{
    public class JavaScriptBridge
    {
        private SuperGraphControl _superGraphControl = null;

        public JavaScriptBridge(SuperGraphControl superGraphControl)
        {
            _superGraphControl = superGraphControl;
        }

        [ScriptableMember]
        public void LoadMapAndSelectNode(string domainId, string mapNodeId, string nodeId)
        {
            Guid domain, mapNode, id;
            if (Guid.TryParse(domainId, out domain) && Guid.TryParse(mapNodeId, out mapNode) && Guid.TryParse(nodeId, out id))
            {
                _superGraphControl.Ref.SuperGraph.ClearMapObjects();
                _superGraphControl.Ref.Breadcrumbs.BreadcrumbTrail.Clear();
                _superGraphControl.Ref.IsLoadMapByGuid = true; //loading a map, set the value
                _superGraphControl.Ref.NodeId = id;      //this is the node to select once the map loads
                _superGraphControl.Ref.LoadMapById(domain, mapNode, id); //load the map
            }
        }

        [ScriptableMember]
        public void ReceiveGlymaMessage(string message)
        {
            if (_superGraphControl.Ref.SuperGraphController != null)
            {
                _superGraphControl.Ref.SuperGraphController.VideoController.ReceiveMessage(message);
            }
        }

        [ScriptableMember]
        public Guid GetCurrentRootMapUid()
        {
            Guid rootMapId = Guid.Empty;
            if (_superGraphControl.Ref.SuperGraphController != null)
            {
                if (_superGraphControl.Ref.SuperGraphController.Context.Proxy.RootMapId.HasValue)
                {
                    rootMapId = _superGraphControl.Ref.SuperGraphController.Context.Proxy.RootMapId.Value;
                }
            }
            return rootMapId;
        }

        [ScriptableMember]
        public Guid GetCurrentDomainUid()
        {
            Guid domainId = Guid.Empty;
            if (_superGraphControl.Ref.SuperGraphController != null)
            {
                domainId = _superGraphControl.Ref.SuperGraphController.DomainId;
            }
            return domainId;
        }

        [ScriptableMember]
        public string GetRootMapMetadataValue(string metadataName)
        {
            string result = string.Empty;
            if (_superGraphControl.Ref.SuperGraphController != null)
            {
                if (RootMapProperties.Instance.Metadata.ContainsKey(metadataName))
                {
                    return RootMapProperties.Instance.Metadata[metadataName];
                }
            }
            return result;
        }

        [ScriptableMember]
        public string GetRootMapUrl()
        {
            Guid domainUid = GetCurrentDomainUid();
            Guid rootMapUid = GetCurrentRootMapUid();
            string query = HtmlPage.Document.DocumentUri.Query;
            string absoluteUri = HtmlPage.Document.DocumentUri.AbsoluteUri;
            string baseUrl = string.IsNullOrWhiteSpace(query)
                ? absoluteUri
                : absoluteUri.Replace(query, "");
            string url = string.Format("{0}?DomainUid={1}&MapUid={2}", baseUrl, domainUid, rootMapUid);
            return url;
        }

        [ScriptableMember]
        public void AddAuthorContextMenuItem(string contextMenuItemName)
        {
            JavascriptCallbackRegister.Instance.AuthorItems.Add(contextMenuItemName);
        }

        [ScriptableMember]
        public void AddReaderContextMenuItem(string contextMenuItemName)
        {
            JavascriptCallbackRegister.Instance.ViewerItems.Add(contextMenuItemName);
        }
    }
}
