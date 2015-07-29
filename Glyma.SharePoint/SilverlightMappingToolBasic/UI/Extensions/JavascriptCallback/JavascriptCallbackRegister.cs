using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu.Base;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.Extensions.JavascriptCallback
{
    public class JavascriptCallbackRegister
    {
        private List<string> _authorItems;
        private List<string> _viewerItems;
        private static JavascriptCallbackRegister _instance;
        private static readonly object Padlock = new object();

        public List<string> ViewerItems
        {
            get
            {
                if (_viewerItems == null)
                {
                    _viewerItems = new List<string>();
                }
                return _viewerItems;
            }
            set { _viewerItems = value; }
        }

        public List<string> AuthorItems
        {
            get
            {
                if (_authorItems == null)
                {
                    _authorItems = new List<string>();
                }
                return _authorItems;
            }
            set { _authorItems = value; }
        }

        public bool HasAuthorCustomContextMenuItem
        {
            get { return AuthorItems.Count > 0; }
        }

        public bool HasViewerCustomContextMenuItem
        {
            get { return ViewerItems.Count > 0; }
        }

        public static JavascriptCallbackRegister Instance
        {
            get
            {
                lock (Padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new JavascriptCallbackRegister();
                    }

                    return _instance;
                }
            }
        }

        private JavascriptCallbackRegister()
        {
        }

        public void CallBack(ContextMenuType type, string name, string rootMapJson, string mapJson, string nodeJson)
        {
            switch (type)
            {
                 case ContextMenuType.Author:
                    RelatedContentPanelUtil.Instance.InvokeAuthorContextMenuCallback(name, rootMapJson, mapJson, nodeJson);
                    break;
                case ContextMenuType.Viewer:
                    RelatedContentPanelUtil.Instance.InvokeReaderContextMenuCallback(name, rootMapJson, mapJson, nodeJson);
                    break;
            }
            
        }
    }
}
