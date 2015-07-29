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
using VideoPlayerSharedLib;
using SilverlightMappingToolBasic.UI.Extensions.VideoWebPart;

namespace SilverlightMappingToolBasic
{
    public class RelatedContentPanelUtil
    {
        private static RelatedContentPanelUtil instance = null;
        private bool _hasRelatedContentPanel = false;

        //cached controller objects
        private static ScriptObject _relatedContentController = null;
        private static ScriptObject _mappingToolController = null;

        //cached panel objects
        private static ScriptObject _relatedMapsPanel = null;
        private static ScriptObject _activityFeedPanel = null;
        private static ScriptObject _filteredFeedPanel = null;

        private bool _domainMapSelectionDialogShown = false;

        private RelatedContentPanelUtil()
        {
            object result = HtmlPage.Window.Invoke("hasRelatedContentPanels");
            HasRelatedContentPanel = (bool)result;

            if (HasRelatedContentPanel)
            {
                var relatedContentControllerObject = HtmlPage.Window.Eval("Glyma.RelatedContentPanels.RelatedContentController") as ScriptObject;
                if (relatedContentControllerObject != null)
                {
                    RelatedContentController = relatedContentControllerObject.Invoke("getInstance") as ScriptObject;

                    if (RelatedContentController != null)
                    {
                        var contentPanelsArray = RelatedContentController.GetProperty("contentPanels") as ScriptObject;
                        if (contentPanelsArray != null)
                        {
                            RelatedMapsPanel = contentPanelsArray.GetProperty("RelatedNodesPanel") as ScriptObject;
                            ActivityFeedPanel = contentPanelsArray.GetProperty("ActivityFeedPanel") as ScriptObject;
                            FilteredFeedPanel = contentPanelsArray.GetProperty("FilteredFeedPanel") as ScriptObject;
                        }
                    }
                }

                var mappingToolContentControllerObject = HtmlPage.Window.Eval("Glyma.MappingTool.MappingToolController") as ScriptObject;
                if (mappingToolContentControllerObject != null)
                {
                    MappingToolController = mappingToolContentControllerObject.Invoke("getInstance") as ScriptObject;
                }
            }
        }

        public static RelatedContentPanelUtil Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RelatedContentPanelUtil();
                }
                return instance;
            }
        }

        public bool HasRelatedContentPanel
        {
            get
            {
                return _hasRelatedContentPanel;
            }
            private set
            {
                _hasRelatedContentPanel = value;
            }

        }

        #region Controllers
        private ScriptObject RelatedContentController 
        {
            get
            {
                return _relatedContentController;
            }
            set
            {
                _relatedContentController = value;
            }
        }

        private ScriptObject MappingToolController
        {
            get
            {
                return _mappingToolController;
            }
            set
            {
                _mappingToolController = value;
            }
        }
        #endregion

        #region Related Content Panels
        private ScriptObject RelatedMapsPanel
        {
            get
            {
                return _relatedMapsPanel;
            }
            set
            {
                _relatedMapsPanel = value;
            }
        }

        private ScriptObject ActivityFeedPanel
        {
            get
            {
                return _activityFeedPanel;
            }
            set
            {
                _activityFeedPanel = value;
            }
        }

        private ScriptObject FilteredFeedPanel
        {
            get
            {
                return _filteredFeedPanel;
            }
            set
            {
                _filteredFeedPanel = value;
            }
        }
        #endregion

        public bool IsDomainAndMapSelectionDialogShown
        {
            get
            {
                return _domainMapSelectionDialogShown;
            }
            set
            {
                _domainMapSelectionDialogShown = value;
            }
        }

        public void ResetAndHidePanels()
        {
            if (HasRelatedContentPanel)
            {
                if (RelatedContentController != null)
                {
                    RelatedContentController.Invoke("resetAndHidePanels");
                }
            }
        }

        public void LoadRelatedVideoContent(string videoSourceUrl, VideoSize videoSize)
        {
            if (HasRelatedContentPanel)
            {
                if (RelatedContentController != null)
                {
                    string sizeValue = videoSize.ToString().ToLower();
                    RelatedContentController.Invoke("loadRelatedContent", "video", HttpUtility.UrlEncode(videoSourceUrl), sizeValue);
                }
            }
        }

        public void LoadRelatedIframeContent(string content, int width = 0, int height = 0)
        {
            if (HasRelatedContentPanel)
            {
                if (RelatedContentController != null)
                {
                    RelatedContentController.Invoke("loadRelatedContent", "iframeUrl", content, width, height);
                }
            }
        }

        public void LoadRelatedNodeHtmlContent(string content, int width = 0, int height = 0)
        {
            if (HasRelatedContentPanel)
            {
                if (RelatedContentController != null)
                {
                    RelatedContentController.Invoke("loadRelatedContent", "nodeHtml", content, width, height);
                }
            }
        }

        public void ShowRelatedMaps(string relatedMapsJson)
        {
            if (HasRelatedContentPanel)
            {
                if (RelatedMapsPanel != null)
                {
                    RelatedMapsPanel.Invoke("showRelatedMaps", relatedMapsJson);
                }
            }
        }

        public void ClearRelatedMaps()
        {
            if (HasRelatedContentPanel)
            {
                if (RelatedMapsPanel != null)
                {
                    RelatedMapsPanel.Invoke("clearRelatedMaps");
                }
            }
        }

        public void SetAuthorMode()
        {
            if (HasRelatedContentPanel)
            {
                if (RelatedContentController != null)
                {
                    RelatedContentController.Invoke("setAuthorMode");
                }
            }
        }

        public void SetReaderMode()
        {
            if (HasRelatedContentPanel)
            {
                if (RelatedContentController != null)
                {
                    RelatedContentController.Invoke("setReaderMode");
                }
            }
        }

        public void ShowActivityFeeds()
        {
            if (HasRelatedContentPanel)
            {
                if (ActivityFeedPanel != null)
                {
                    ActivityFeedPanel.Invoke("showFeedPanel");
                }
                if (FilteredFeedPanel != null)
                {
                    FilteredFeedPanel.Invoke("showFeedPanel");
                }
            }
        }

        public void MapLoadedCallback(bool rootMapLoaded)
        {
            if (!this.IsDomainAndMapSelectionDialogShown)
            {
                if (HasRelatedContentPanel)
                {
                    if (RelatedContentController != null)
                    {
                        RelatedContentController.Invoke("mapLoadCompleted", rootMapLoaded);
                    }
                }
            }
        }

        public void InvokeAuthorContextMenuCallback(string contextMenuItemName, string rootMap, string map, string node)
        {
            if (HasRelatedContentPanel)
            {
                if (MappingToolController != null)
                {
                    MappingToolController.Invoke("InvokeAuthorContextMenuCallback", contextMenuItemName, rootMap, map, node);
                }
            }
        }

        public void InvokeReaderContextMenuCallback(string contextMenuItemName, string rootMap, string map, string node)
        {
            if (HasRelatedContentPanel)
            {
                if (MappingToolController != null)
                {
                    MappingToolController.Invoke("InvokeReaderContextMenuCallback", contextMenuItemName, rootMap, map, node);
                }
            }
        }

        /// <summary>
        /// This returns whether or not the Yammer Embed API is available in the currently used page-layout/master-page combination.
        /// 
        /// Note: It is meant to be called where no map is loaded yet in the Management Console and allows setting up of Yammer properties.
        /// There is a more thorough test of if a particular root map has Yammer available that is done when a map is loaded, that exists in the RelatedContentPanels logic.
        /// </summary>
        /// <returns>True if the Yammer Embed JavaScript API is included in the page somewhere</returns>
        public bool IsYammerAvailable()
        {
            bool isAvailable = false;
            if (HasRelatedContentPanel)
            {
                object result = HtmlPage.Window.Invoke("IsYammerAvailable");
                isAvailable = (bool)result;
            }
            return isAvailable;
        }

        public bool IsJavascriptLibraryLoaded()
        {
            object result = HtmlPage.Window.Invoke("IsJavascriptLibraryLoaded");
            return (bool)result; 
        }
    }
}
