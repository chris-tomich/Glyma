<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages"
    Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MappingToolWebPartUserControl.ascx.cs"
    Inherits="SevenSigma.MappingTool.GlymaMappingToolWebPart.MappingToolWebPartUserControl" %>
<SharePoint:CssRegistration ID="GlymaMapCssRegistration" name="<% $SPUrl:~sitecollection/Style Library/Glyma/MappingTool/GlymaMappingWebPart.css %>" runat="server" After="corev4.css"/>
<SharePoint:CssRegistration ID="CssRegistration" name="<% $SPUrl:~sitecollection/Style Library/Glyma/Common/jquery-ui.css %>" runat="server" />
<SharePoint:ScriptLink language="javascript" name="~sitecollection/Style Library/Glyma/Common/modernizr-2.6.2.min.js" runat="server"/>
<style type="text/css">
    html, body
    {
        height: 100%;
        overflow: auto;
    }
    body
    {
        padding: 0;
        margin: 0;
    }
    #silverlightMappingControlHost
    {
        text-align: center;
    }
    #pinchzoom {
        touch-action: none;
        -ms-touch-action: none;
    }
    .nodedetailbutton {
        position: absolute;
    }
</style>
<script type="text/javascript">
    var glymaJSBridge = null;
    var isBusy = false;
    var urlParams;

    (window.onpopstate = function () {
        var match,
            pl = /\+/g,  // Regex for replacing addition symbol with a space
            search = /([^&=]+)=?([^&]*)/g,
            decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
            query = window.location.search.substring(1);

        urlParams = {};
        while (match = search.exec(query))
            urlParams[decode(match[1])] = decode(match[2]);
    })();


    window.onbeforeunload = confirmExit;

    function confirmExit() {
        if (isBusy) {
            return "All of your unsaved work will be lost if you close.";
        }
    }

    function getGlymaIconLibrary() {
        return "<%=GlymaIconLibraryUrl %>";
    }

    function getThemeSvcUrl() {
        return "<%=ThemeServiceUrl %>";
    }

    function getMappingToolSvcUrl() {
        return "<%=MappingToolServiceUrl %>";
    }

    function getLoggingSvcUrl() {
        return "<%=LoggingServiceUrl %>";
    }

    function getCurrentUserName() {
        return "<%=CurrentUserName %>";
    }

    function getCurrentSiteUrl() {
        return "<%=CurrentSiteUrl %>";
    }

    function getBaseUrl() {
        return "<%=BaseUrl %>";
    }

    function getServerRelativeVersionedLayoutsFolder() {
        return "<%=GetServerRelativeVersionedLayoutsFolder()%>"
    }

    function displayServiceErrorDialog() {
        alert("Unable to connect to web services at:\n<%=MappingToolServiceUrl %>");
    }

    function openDocument(documentUrl) {
        params = 'width=' + screen.width;
        params += ', height=' + screen.height;
        params += ', top=0, left=0';
        params += ', location=no';
        params += ', toolbar=no';
        params += ', status=yes';
        params += ', menubar=no';
        params += ', directories=no';
        params += ', scrollbars=yes';
        params += ', resizable=yes';

        newwin = window.open(documentUrl, 'mapDocWindow', params);
        if (window.focus) {
            newwin.focus();
        }
        return false;
    }

    function hasRelatedContentPanels() {
        // Check for the existence of functions that are called via HtmlPage.Window.Invoke
        if (typeof (Glyma) === "object" &&
            typeof (Glyma.RelatedContentPanels) === "object" &&
            typeof (Glyma.MappingTool) == "object" &&
            typeof (Glyma.MappingTool.SilverlightMappingTool) === "function" &&
            typeof (Glyma.MappingTool.Html5MappingTool) === "function" &&
            typeof (Glyma.RelatedContentPanels.RelatedContentController) === "function" &&
            typeof (Glyma.RelatedContentPanels.RelatedContentController.getInstance) === 'function') {
            return true;
        }
        else {
            return false; //the related content panel web part isn't added
        }
    }

    function IsYammerAvailable() {
        var usingScriptTagId = $("#YammerEmbedAPI").length > 0;
        if (usingScriptTagId) {
            return true; //this is the preferred/faster way to determine if the Yammer Embed API is available.
        }
        else {
            //if not using the ID YammerEmbedAPI on the script tag try detect the src of all script tags for the Yammer Embed API source.
            var result = false;
            if (window.jQuery) {
                $("script").each(function (index, scriptTag) {
                    var src = $(scriptTag).prop("src");
                    if (src.toLowerCase() == "https://assets.yammer.com/assets/platform_embed.js") {
                        result = true;
                        return false;
                    }
                });
            }
            return result;
        }
    }

    function onSilverlightLoaded(sender, args) {
        var slCtl = sender.getHost();
        glymaJSBridge = slCtl.Content.glymaMapCanvas;
        var silverlightMappingTool = Glyma.MappingTool.SilverlightMappingTool.getInstance();
        if (silverlightMappingTool != null) {
            silverlightMappingTool.setGlymaJSBridge(glymaJSBridge);
        }
    }

    //This sets isbusy from silverlight mapping tool.
    function SetBusy(isbusy) {
        isBusy = isbusy;
    }

    function IsJavascriptLibraryLoaded() {
        if (window.jQuery
            && (typeof window.jQuery.ui !== 'undefined')
            && (typeof (redirectToGlyma) === "function")
            && (typeof window.Silverlight !== 'undefined')
            && (typeof (onSilverlightError) === "function")) {
            return true;
        }
        else {
            return false;
        }
    }

    function LoadJS(src, callback) {
        var script = document.createElement('script'),
            loaded;
        script.setAttribute('src', src);
        if (callback) {
            script.onreadystatechange = script.onload = function () {
                if (!loaded) {
                    callback();
                }
                loaded = true;
            };
        }
        document.getElementsByTagName('head')[0].appendChild(script);
    }
</script>
<div id="loader" style="display:none;"></div>
<div id="mappingtool-wrapper">
<asp:Panel ID="SilverlightPanel" runat="server">
    <div id="silverlightMappingControlHost" class="unselectable">
        <script type="text/javascript">
            var isHtml5 = false;
            if (urlParams["html5"]) {
                if (urlParams["html5"] == "true") {
                    isHtml5 = true;
                }
            }

            if (!Silverlight.isInstalled("4.0")) {
                isHtml5 = true;
            }

            if (!isHtml5) {
                document.write('<object id="SLMappingTool" data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%">');
                document.write('<param name="source" value="<%=GetServerRelativeVersionedLayoutsFolder()%>/ClientBin/Glyma/SilverlightMappingToolBasic.xap" />');
                document.write('<param name="onLoad" value="onSilverlightLoaded" />');
                document.write('<param name="onError" value="onSilverlightError" />');
                document.write('<param name="background" value="white" />');
                document.write('<param name="minRuntimeVersion" value="5.0.61118.0" />');
                document.write('<param name="initParams" value="<%=InitParameters %>" />');
                document.write('<param name="autoUpgrade" value="true" />');
                document.write('<param name="enableGPUAcceleration" value="true" />');
                document.write('<param id="glymaWebPartCurrentSpSiteUrl" name="currentSpSiteUrl" value="<%=CurrentSiteUrl %>" />');
                document.write('</object>');
                document.write('<iframe id="_sl_historyFrame" style="visibility: hidden; height: 0px; width: 0px; border: 0px"></iframe>');
            }

        </script>
        <div class="unselectable" id="pinchzoom" style="-webkit-transform: translate3d(0, 0, 0); position: absolute; display: block; height: 100%; width: 100%;right: 15px;">
            <canvas id="mapCanvas" class="unselectable" ondragstart="return false;"></canvas>
    </div>
    <div id="left-sidebar-container" class="sidebar-container unselectable">
        <div id="left-sidebar" class="sidebar unselectable">
            <div id="sidebar-glyma-logo" class="unselectable"></div>
            <ul>
                <li id="sidebar-refresh" class="sidebar-button unselectable"><a href="#"></a></li>
                <li id="sidebar-realign" class="sidebar-button unselectable"><a href="#"></a></li>
                <li id="sidebar-fullscreen" class="sidebar-button unselectable"><a href="#"></a></li>
                <li id="sidebar-exitfullscreen" class="sidebar-button unselectable" style="display: none;"><a href="#"></a></li>
            </ul>
            
        </div>
        <div id="left-expender" class="expender unselectable"></div>
        <div id="left-menuPanel" class="menuPanel unselectable"></div>
        <div id="zoom-control" class="unselectable">
        <div id="zoom-view-reset" class="zoom-button unselectable">
        </div>
        <div id="zoom-in" class="zoom-button unselectable">
        </div>
        <div id="slider" class="unselectable"></div><div id="zoom-default" class="zoom-button-right unselectable">
                               </div>
        <div id="zoom-out" class="zoom-button unselectable">
        </div>
    </div>
    </div>
    <div id="breadcrumbs" class="unselectable">
        <div class="unselectable" id="breadcrumb-home" style="display: none"><a class="unselectable" href="javascript:void(0);">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</a></div>
        <ul id="breadcrumb-container" class="unselectable">
            
        </ul>  
        
    </div>
        <script type="text/javascript">
            $(function () {
                $("#slider").slider({
                    orientation: "vertical",
                    min: -22,
                    max: 22,
                    value: 0,
                    step: 1,
                    slide: function (event, ui) {
                        if (event.originalEvent) {
                            Glyma.SharedVariables.mapController.mapRenderer.scaleBySlider(ui.value);
                        }
                    }
                });
            });
        </script>
    </div>
    
</asp:Panel>
    <div id="nodeDetailButtonContainer" class="unselectable" style="position: absolute;z-index: 100;display: none;">
        <img id="nodedetailshowall" class="nodedetailbutton unselectable" height="35" width="35" src="<%= BaseUrl %>/Style Library/Glyma/Icons/nodemenu/showall-static.png" />
        <img id="nodedetailcontent" class="nodedetailbutton unselectable" height="35" width="35" src="<%= BaseUrl %>/Style Library/Glyma/Icons/nodemenu/content-static.png" />
        <img id="nodedetailfeed" class="nodedetailbutton unselectable" height="35" width="35" src="<%= BaseUrl %>/Style Library/Glyma/Icons/nodemenu/feed-static.png" />
        <img id="nodedetaillocate" class="nodedetailbutton unselectable" height="35" width="35" src="<%= BaseUrl %>/Style Library/Glyma/Icons/nodemenu/locate-static.png" />
        <img id="nodedetailmap" class="nodedetailbutton unselectable" height="35" width="35" src="<%= BaseUrl %>/Style Library/Glyma/Icons/nodemenu/map-static.png" />
        <img id="nodedetailvideo" class="nodedetailbutton unselectable" height="35" width="35" src="<%= BaseUrl %>/Style Library/Glyma/Icons/nodemenu/video-static.png" />
    </div>
</div>
<div id="hidePanels" style="display: none">
    
        <img id="proImage" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes1x/pro.png" style="display: none;" />
        <img id="conImage" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes1x/cons.png" style="display: none;" />
        <img id="ideaImage" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes1x/idea.png" style="display: none;" />
        <img id="questionImage" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes1x/question.png" style="display: none;" />
        <img id="noteImage" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes1x/note.png" style="display: none;" />
        <img id="mapImage" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes1x/map.png" style="display: none;" />
        <img id="decisionImage" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes1x/decision.png" style="display: none;" />

        <img id="proImage2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes2x/pro.png" style="display: none;" />
        <img id="conImage2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes2x/cons.png" style="display: none;" />
        <img id="ideaImage2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes2x/idea.png" style="display: none;" />
        <img id="questionImage2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes2x/question.png" style="display: none;" />
        <img id="noteImage2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes2x/note.png" style="display: none;" />
        <img id="mapImage2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes2x/map.png" style="display: none;" />
        <img id="decisionImage2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes2x/decision.png" style="display: none;" />

        <img id="proImage4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes4x/pro.png" style="display: none;" />
        <img id="conImage4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes4x/cons.png" style="display: none;" />
        <img id="ideaImage4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes4x/idea.png" style="display: none;" />
        <img id="questionImage4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes4x/question.png" style="display: none;" />
        <img id="noteImage4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes4x/note.png" style="display: none;" />
        <img id="mapImage4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes4x/map.png" style="display: none;" />
        <img id="decisionImage4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes4x/decision.png" style="display: none;" />

        <img id="proImage8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes8x/pro.png" style="display: none;" />
        <img id="conImage8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes8x/cons.png" style="display: none;" />
        <img id="ideaImage8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes8x/idea.png" style="display: none;" />
        <img id="questionImage8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes8x/question.png" style="display: none;" />
        <img id="noteImage8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes8x/note.png" style="display: none;" />
        <img id="mapImage8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes8x/map.png" style="display: none;" />
        <img id="decisionImage8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes8x/decision.png" style="display: none;" />

        <img id="proImage5x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes5x/pro.png" style="display: none;" />
        <img id="conImage5x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes5x/cons.png" style="display: none;" />
        <img id="ideaImage5x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes5x/idea.png" style="display: none;" />
        <img id="questionImage5x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes5x/question.png" style="display: none;" />
        <img id="noteImage5x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes5x/note.png" style="display: none;" />
        <img id="mapImage5x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes5x/map.png" style="display: none;" />
        <img id="decisionImage5x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes5x/decision.png" style="display: none;" />

        <img id="proImage10x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes10x/pro.png" style="display: none;" />
        <img id="conImage10x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes10x/cons.png" style="display: none;" />
        <img id="ideaImage10x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes10x/idea.png" style="display: none;" />
        <img id="questionImage10x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes10x/question.png" style="display: none;" />
        <img id="noteImage10x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes10x/note.png" style="display: none;" />
        <img id="mapImage10x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes10x/map.png" style="display: none;" />
        <img id="decisionImage10x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodes10x/decision.png" style="display: none;" />
        
        <img id="button-content1x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu1x/button-content-static.png" style="display: none;" />
        <img id="button-feed1x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu1x/button-feed-static.png" style="display: none;" />
        <img id="button-map1x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu1x/button-map-static.png" style="display: none;" />
        <img id="button-pause1x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu1x/button-pause-static.png" style="display: none;" />
        <img id="button-play1x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu1x/button-play-static.png" style="display: none;" />

        <img id="button-content2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu2x/button-content-static.png" style="display: none;" />
        <img id="button-feed2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu2x/button-feed-static.png" style="display: none;" />
        <img id="button-map2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu2x/button-map-static.png" style="display: none;" />
        <img id="button-pause2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu2x/button-pause-static.png" style="display: none;" />
        <img id="button-play2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu2x/button-play-static.png" style="display: none;" />

        <img id="button-content4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu4x/button-content-static.png" style="display: none;" />
        <img id="button-feed4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu4x/button-feed-static.png" style="display: none;" />
        <img id="button-map4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu4x/button-map-static.png" style="display: none;" />
        <img id="button-pause4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu4x/button-pause-static.png" style="display: none;" />
        <img id="button-play4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu4x/button-play-static.png" style="display: none;" />
    
        <img id="button-content8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu8x/button-content-static.png" style="display: none;" />
        <img id="button-feed8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu8x/button-feed-static.png" style="display: none;" />
        <img id="button-map8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu8x/button-map-static.png" style="display: none;" />
        <img id="button-pause8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu8x/button-pause-static.png" style="display: none;" />
        <img id="button-play8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu8x/button-play-static.png" style="display: none;" />
        
        <img id="button-hover-content1x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu1x/button-content-hover.png" style="display: none;" />
        <img id="button-hover-feed1x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu1x/button-feed-hover.png" style="display: none;" />
        <img id="button-hover-map1x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu1x/button-map-hover.png" style="display: none;" />
        <img id="button-hover-pause1x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu1x/button-pause-hover.png" style="display: none;" />
        <img id="button-hover-play1x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu1x/button-play-hover.png" style="display: none;" />

        <img id="button-hover-content2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu2x/button-content-hover.png" style="display: none;" />
        <img id="button-hover-feed2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu2x/button-feed-hover.png" style="display: none;" />
        <img id="button-hover-map2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu2x/button-map-hover.png" style="display: none;" />
        <img id="button-hover-pause2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu2x/button-pause-hover.png" style="display: none;" />
        <img id="button-hover-play2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu2x/button-play-hover.png" style="display: none;" />

        <img id="button-hover-content4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu4x/button-content-hover.png" style="display: none;" />
        <img id="button-hover-feed4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu4x/button-feed-hover.png" style="display: none;" />
        <img id="button-hover-map4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu4x/button-map-hover.png" style="display: none;" />
        <img id="button-hover-pause4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu4x/button-pause-hover.png" style="display: none;" />
        <img id="button-hover-play4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu4x/button-play-hover.png" style="display: none;" />
    
        <img id="button-hover-content8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu8x/button-content-hover.png" style="display: none;" />
        <img id="button-hover-feed8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu8x/button-feed-hover.png" style="display: none;" />
        <img id="button-hover-map8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu8x/button-map-hover.png" style="display: none;" />
        <img id="button-hover-pause8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu8x/button-pause-hover.png" style="display: none;" />
        <img id="button-hover-play8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu8x/button-play-hover.png" style="display: none;" />    
    

        <img id="extendbutton-hover" src="<%= BaseUrl %>/Style Library/Glyma/Icons/nodemenu/menubutton-hover.png" style="display: none;" />
        <img id="extendbutton-static" src="<%= BaseUrl %>/Style Library/Glyma/Icons/nodemenu/menubutton-static.png" style="display: none;" />
    
        <img id="semicollapsed1x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu1x/semicollapsed.png" style="display: none;" />
        <img id="collapsed1x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu1x/collapsed.png" style="display: none;" />
        <img id="expanded1x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu1x/expanded.png" style="display: none;" />
    
        <img id="semicollapsed2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu2x/semicollapsed.png" style="display: none;" />
        <img id="collapsed2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu2x/collapsed.png" style="display: none;" />
        <img id="expanded2x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu2x/expanded.png" style="display: none;" />
    
        <img id="semicollapsed4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu4x/semicollapsed.png" style="display: none;" />
        <img id="collapsed4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu4x/collapsed.png" style="display: none;" />
        <img id="expanded4x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu4x/expanded.png" style="display: none;" />
    
        <img id="semicollapsed8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu8x/semicollapsed.png" style="display: none;" />
        <img id="collapsed8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu8x/collapsed.png" style="display: none;" />
        <img id="expanded8x" src="<%= BaseUrl %>/Style Library/Glyma/Icons/scaled/nodemenu8x/expanded.png" style="display: none;" />

        <li class="breadcrumb-item unselectable" id="breadcrumb-template" map-id="" domain-id="" style="display: none;"><a href="#"></a></li>
<div id="errorPanel" class="noti">
    <h2 class="whatnow" id="errormessage">
    <b>We are unable to find your map, please try again later.</b>
    </h2>
    <div class="logos">
        <div class="b bg">
        <a class="l" href="http://www.glyma.co/" target="_blank"><span class="bro"></span>   
        <span class="vendor"></span></a>
    </div>
    </div>
    
    <h2 class="whatnow"> 
    For more information, please <a href="http://www.glyma.co/how-do-i-get-it" target="_blank">contact us</a>.
    </h2>
</div>

<div id="noti" class="noti">
<h2 class="whatnow">
    <b>Unfortunately, your browser does not support Glyma. You have following options available to get it working.</b>
</h2>
<div class="update-options">
    <div class="update-option" id="install-plugin"><div class="update-option-title">Install Browser Plugin</div>
   <div class="update-option-text">For authoring maps</div></div>
    <div class="update-option" id="upgrade-browser"><div class="update-option-title">Upgrade Browser</div>
  <div class="update-option-text">For all other users</div></div>
</div>
<div class="logos">
    <div class="b bs">
        <a class="l" href="http://www.microsoft.com/silverlight/" target="_blank"><span class="bro">Microsoft Silverlight</span>   
        <span class="vendor">Microsoft</span></a>
    </div>
  <div class="b bor"><span class="orspan">OR</span></div>

    <div class="b bf">
        <a class="l" href="http://www.mozilla.com/firefox/" target="_blank"><span class="bro">Firefox</span>   
        <span class="vendor">Mozilla Foundation</span></a>
    </div>
    <div class="b bc">
        <a class="l" href="http://www.google.com/chrome?hl=en" target="_blank"><span class="bro">Chrome</span>   
        <span class="vendor">Google</span>           </a>
    </div>
    <div class="b bi">
        <a class="l" href="http://windows.microsoft.com/en-US/internet-explorer/downloads/ie" target="_blank"><span class="bro">Internet Explorer</span>   
        <span class="vendor">Microsoft</span>        </a>
    </div>
</div>
<h2 class="whatnow"> 
    For more information, please <a href="http://www.glyma.co/how-do-i-get-it" target="_blank">contact us</a>.</h2>
</div>
<div id="mapselect" class="noti unselectable">
    <div class="div-row dialog-head unselectable">
        <div class="float-left dialog-text unselectable">
            <span>Map Selection</span>
        </div><div class="float-left  unselectable"><div class="close-icon unselectable" onclick="$('#loader').fadeOut('slow');"></div></div>
    </div>
    <div class="div-row dialog-body">
        <div class="dialog-content">
            <div class="dialog-content-head"><span>Select map to load</span></div>
            <div class="div-row dialog-content-row">
                <div class="float-left select-label">Project Name</div>
                <div class="float-left select-box"><select id="domains" name="projects">

        </select></div>
                
            </div>
            <div class="div-row dialog-content-row">
                <div class="float-left select-label">Map Name</div>
                <div class="float-left select-box"><select id="maps" name="maps">

        </select></div>
                
            </div>
                
        </div>
        <div class="dialog-actions">
            <a href="javascript:void(0)" onclick="alert($('#domains').val() + ' ' + $('#maps').val());" class="glyma-button ui-link">Load</a>
        </div>
    </div>
</div>
    </div>
<script type="text/javascript">
    var isMappingToolInitialised = false;
    var isHTML5MappingToolInitialised = false;

    function InitialiseMappingTool() {
        if (isMappingToolInitialised) {
            return; //ensure it cant run twice;
        }
        isMappingToolInitialised = true;

        if (isHtml5) {
            if (window.Modernizr.canvas) {
                InitialiseHtml5MappingTool();
            } else {
                showNoti();
            }
        } else {
            InitaliseSilverlightMappingTool();
        }
    }

    function InitialiseHtml5MappingTool() {
        if (isHTML5MappingToolInitialised) {
            return; //ensure it cant run twice;
        }
        isHTML5MappingToolInitialised = true;

        if (urlParams["DomainUid"] && urlParams["MapUid"]) {
            LoadJS("<%= BaseUrl %>/Style Library/Glyma/Common/RTree.js");
            LoadJS("<%= BaseUrl %>/Style Library/Glyma/Common/jquery.mobile-1.4.2.min.js");
            LoadJS("<%= BaseUrl %>/Style Library/Glyma/Common/plugins.js");
            LoadJS("<%= BaseUrl %>/Style Library/Glyma/Common/default.js");
            LoadJS("<%= BaseUrl %>/Style Library/Glyma/Common/jquery.mousewheel.js");
            LoadJS("<%= BaseUrl %>/Style Library/Glyma/Common/hammer.js");
            LoadJS("<%= BaseUrl %>/Style Library/Glyma/MappingTool/html5mappingtool.js", function () {
                Glyma.SharedVariables.mapController = new Glyma.MapController();
                Glyma.SharedVariables.mapController.getMapData(urlParams["DomainUid"], urlParams["MapUid"], urlParams["NodeUid"]);
            });
        } else {
            showError("We are unable to find your map, please try again later.");
        }

    }

    function showError(msg) {
        var loader = $("#loader");
        loader.removeClass("loading");
        loader.addClass("notification");
        loader.html('');
        $("#errormessage").html('<b>' + msg + '</b>');
        $("#errorPanel").clone().appendTo("#loader");
        loader.show();
    }

    function showNoti() {
        var loader = $("#loader");
        loader.removeClass("loading");
        loader.addClass("notification");
        loader.html('');
        $("#noti").clone().appendTo("#loader");
        loader.show();
    }

    function openInNewTab(url) {
        var win = window.open(url, '_blank');
        win.focus();
    }

    function mapSelection() {
        var loader = $("#loader");
        loader.removeClass("loading");
        loader.addClass("notification");
        loader.html('');
        $("#mapselect").clone().appendTo("#loader");
        loader.find('select#domains').change(function () {
            Glyma.SharedVariables.mapController.loadMaps($(this).val());
        });
        loader.show();
        Glyma.SharedVariables.mapController.loadDomains();
    }

    function showLoad() {
        var loader = $("#loader");
        loader.removeClass("notification");
        loader.addClass("loading");
        loader.html('');
        loader.show();
    }

    function InitaliseSilverlightMappingTool() {
        $("#loader").hide();
        $("#pinchzoom").hide();
        $("#left-sidebar-container").hide();
        $("#breadcrumbs").hide();
        $("#zoom-control").hide();
    }


    $(document).ready(function () {
        if (!isMappingToolInitialised) {
            ReinitMappingTool();
        }
    });

    function ReinitMappingTool() {
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
            InitMappingToolTimer();
        }
    }

    function InitMappingToolTimer() {
        setTimeout(ReinitMappingTool, 10);
    }

    if (_spBodyOnLoadFunctionNames) {
        _spBodyOnLoadFunctionNames.push('InitialiseMappingTool');
    }

</script>
