<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, $MicrosoftSharePointAssemblyVersion$" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, $MicrosoftSharePointAssemblyVersion$" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, $MicrosoftSharePointAssemblyVersion$" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, $MicrosoftSharePointAssemblyVersion$" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RelatedContentPanelWebPartUserControl.ascx.cs" Inherits="VideoPlayerSPDeploy.CONTROLTEMPLATES.RelatedContentPanelWebPartUserControl" %>
<SharePoint:CssRegistration ID="RelatedContentPanelCssRegistration" name="<% $SPUrl:~sitecollection/Style Library/Glyma/RelatedContentPanels/RelatedContentPanel.css %>" runat="server" after="Themable/corev15.css" />
<SharePoint:ScriptLink Language="javascript" name="~sitecollection/Style Library/Glyma/RelatedContentPanels/GlymaRelatedContentPanels.js" runat="server" />
<script type="text/javascript">
    function AddResizeCallback(callback) {
        //the function hasRelatedContentPanels is defined in the Glyma Control Template
        //if the function exists and it returns true then it is possible to add the resize callback
        if (typeof(hasRelatedContentPanels) === "function" && hasRelatedContentPanels()) {
            var controller = Glyma.RelatedContentPanels.RelatedContentController.getInstance();
            if (controller != null) {
                controller.resizeCallbacks.push(callback);
            }
        }
    }

    function onYouTubeIframeAPIReady() {
        var controller = Glyma.RelatedContentPanels.RelatedContentController.getInstance();
        if (controller != null) {
            var youTubePanel = controller.getContentPanelByName("YouTubePanel");
            if (youTubePanel != null) {
                var newPlayer = new YT.Player('YouTubePlayer',
                    {
                        height: '225',
                        width: '400',
                        playerVars: {
                            'autoplay': 1,
                            'enabledjsapi': 1,
                            'modestbranding': 1,
                            "fs": 0,
                            'origin': encodeURIComponent(window.location.protocol + "//" + document.domain)
                        },
                        events: {
                            'onReady': youTubePanel.onYouTubePlayerReady,
                            'onStateChange': youTubePanel.onYouTubePlayerStateChange,
                            'onError': youTubePanel.onYouTubePlayerError
                        }
                    });
            }
        }
    }

    function onVideoPlayerLoaded(sender, args) {
        var slCtl = sender.getHost();
        videoPlayerJSBridge = slCtl.Content.glymaVideoPlayerBridge;
        Glyma.RelatedContentPanels.SilverlightVideoContentPanel.SetVideoPlayerJSBridge(videoPlayerJSBridge);
    }

    $(window).resize(function() {
        Glyma.RelatedContentPanels.RelatedContentController.onWindowResized();
    });

    var _initialiseRelatedContentPanelsCalled = false;
    function InitialiseRelatedContentPanels() {
        if (_initialiseRelatedContentPanelsCalled) {
            return; //ensure it can't run twice
        }
        _initialiseRelatedContentPanelsCalled = true; //set it to true immediately

        if (typeof (hasRelatedContentPanels) === "function" && hasRelatedContentPanels()) {
            var controller = Glyma.RelatedContentPanels.RelatedContentController.getInstance();
            if (controller != null) {

                var isSilverlightInstalled = Silverlight.isInstalled("4.0");

                Glyma.RelatedContentPanels.RelatedContentController.setBaseUrl('<%= BaseUrl %>');
                var relatedMapsPanel = new Glyma.RelatedContentPanels.RelatedMapsContentPanel();
                var videoPanel = new Glyma.RelatedContentPanels.SilverlightVideoContentPanel('<%= GetServerRelativeVersionedLayoutsFolder() %>', isSilverlightInstalled);
                var youTubePanel = new Glyma.RelatedContentPanels.YouTubeContentPanel();
                var pagePanel = new Glyma.RelatedContentPanels.PageContentPanel();
                var filteredFeedPanel = new Glyma.RelatedContentPanels.FilteredFeedContentPanel();
                var activityFeedPanel = new Glyma.RelatedContentPanels.ActivityFeedContentPanel();

                controller.addRelatedContentPanel(relatedMapsPanel);
                controller.addRelatedContentPanel(videoPanel);
                controller.addRelatedContentPanel(youTubePanel);
                controller.addRelatedContentPanel(pagePanel);
                controller.addRelatedContentPanel(filteredFeedPanel);
                controller.addRelatedContentPanel(activityFeedPanel);

                Glyma.RelatedContentPanels.RelatedContentController.onWindowResized();
            }
        }
    }

    //Push the inialise function for when SharePoint has initialised the page fully
    if (_spBodyOnLoadFunctionNames) {
        _spBodyOnLoadFunctionNames.push('InitialiseRelatedContentPanels');
    }

    $(document).ready(function () {
        if (!_initialiseRelatedContentPanelsCalled) {
            Reinit();
        }
    });

    function Reinit() {
        //verify we have finished loading init.js
        if (typeof (_spBodyOnLoadWrapper) != 'undefined') {
            //verify we have not already initialized the onload wrapper
            if (_spBodyOnLoadCalled == false) {
                //initialize onload functions
                _spBodyOnLoadWrapper();
            }
        }
        //wait for 10ms and try again if init.js has not been loaded
        else {
            InitTimer();
        }
    }

    function InitTimer() {
        setTimeout(Reinit, 10);
    }
</script>

<div id="RelatedContentPanelWrapper"><!--
    --><div id="RelatedContentPanel"><!--
        --><div id="ExpanderPanel"><div id="Ellipsis"></div></div><!--
        --><div id="IconPanelBackground"><!--
            --><div id="IconPanelBackgroundTop"></div><!--
            --><div id="IconPanel"><!--
    	        --><div id="ShowAllIcon"><!--
                    --><div class="icon"></div><!--
                --></div><!--
            --></div><!--
            --><div id="IconPanelBackgroundBottom"></div><!--
        --></div><!--
    --></div><!--
--></div><!--
--><div id="ContentPanel"><!--
    --><div id="ContentPanels"></div><!--
--></div><!--
--><div class="clear"></div>
<!-- DIV.related-content-header used for reading CSS style -->
<div class="related-content-header" style="display:none;"></div>