/// <reference path="RelatedContentPanel.ts"/>
/// <reference path="VideoContentPanel.ts"/>
module Glyma.RelatedContentPanels {
    export class SilverlightVideoContentPanel extends VideoContentPanel {

        private static videoPlayerJSBridge = null;
        private static _isSilverlightInstalled: boolean = false;

        private static _panelConfig: RelatedContentPanelConfig = {
            PanelId: "SilverlightVideoPanel",
            PanelTitle: "VIDEO",
            Sortable: false,
            Disabled: true,
            Init: function () {
                //In IE Expanding the Video Player initialises it and gets it ready for receiving commands fast
                this.Panel.expandContentPanel();
                if (Utils.QueryString("VideoSource").length != 1) {
                    //if the VideoSource is not on the query string hide the video panel
                    this.Panel.close();
                    $("#IconPanel").hide();
                    this.Panel.disableContentPanel(false);
                    this.Controller.onRelatedContentPanelResize();
                }
                this.Panel.setHasContent(true);//the video panel can always be shown
            },
            OnShow: function () {
                var youTubePanel: RelatedContentPanel = this.Controller.contentPanels["YouTubePanel"];
                if (youTubePanel != null) {
                    youTubePanel.collapseContentPanel(true);
                    youTubePanel.disableContentPanel(false);
                }
                if (SilverlightVideoContentPanel._isSilverlightInstalled) {
                    VideoController.CurrentVideoPlayerType = VideoPlayerType.SILVERLIGHT;
                }
                else {
                    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
                        $("#silverlightVideoControlHost").css("height", "");
                        $("#silverlightVideoControlHost").html("<p class='no-silverlight-message'>The Glyma video player uses the Silverlight browser plug-in" +
                            ", unfortunately you are not using a browser that supports the installation of the Silverlight browser plug-in.<br/><br/>" +
                            "Glyma also supports YouTube video playback that will work in all browsers.</p>");
                    }
                    else {
                        //make the content the right height to show the install silverlight link
                        $("#silverlightVideoControlHost").css("height", 190);
                        $("#silverlightVideoControlHost").html("<p class='no-silverlight-message'>The Glyma video player uses the Silverlight browser plug-in" +
                            ", you can download and install it by clicking the link.<br/>" +
                            "<div class='no-silverlight-icon'><a href='http://www.microsoft.com/silverlight/' target='_blank' shape=''><span>Microsoft Silverlight</span></a></div></p>");
                    }
                }
                this.Controller.onRelatedContentPanelResize();
            },
            SizeChanged: function () {
                $("#silverlightVideoControlHost").css("width", this.Controller.getMaxContentWidth() + "px");
            },
            OnClose: function () {
                this.Panel.stopSilverlightVideoPlayer();
            },
            Reset: function () {
                this.Panel.close();
                this.Panel.disableContentPanel(false);
                this.Panel.setHasContent(true); //the video panel can always be shown
            },
            Content: "<div id='silverlightVideoControlHost' style='width: 400px; height: 300px; margin-top: 0px;'>" +
            "<object id='SLVideoPlayer' data='data:application/x-silverlight-2,' type='application/x-silverlight-2' width='400' height='300'>" +
            "<param name='source' value='{SERVER_RELATIVE_VERSIONED_LAYOUTS_FOLDER}/ClientBin/Glyma/VideoPlayer.xap' />" +
            "<param name='onLoad' value='onVideoPlayerLoaded' />" +
            "<param name='onError' value='onSilverlightError' />" +
            "<param name='background' value='white' />" +
            "<param name='minRuntimeVersion' value='5.0.61118.0' />" +
            "<param name='autoUpgrade' value='true' />" +
            "<param name='enableGPUAcceleration' value='true' />" +
            "<a href='http://go.microsoft.com/fwlink/?LinkID=149156&v=5.0.61118.0' style='text-decoration: none'>" +
            "<img src='http://go.microsoft.com/fwlink/?LinkId=161376' alt='Get Microsoft Silverlight' style='border-style: none' />" +
            "</a>" +
            "</object>" +
            "<iframe id='_sl_historyFrame' style='visibility: hidden; height: 0px; width: 0px; border: 0px'></iframe>" +
            "</div>",
            Icon: "{BASE_URL}/Style Library/Glyma/Icons/video.png",
            IconHover: "{BASE_URL}/Style Library/Glyma/Icons/video-hover.png",
            IconDisabled: "{BASE_URL}/Style Library/Glyma/Icons/video-unavailable.png"
        }

        constructor(serverRelativeVersionedLayoutsFolder: string, silverlightInstalled: boolean) {
            super(SilverlightVideoContentPanel._panelConfig, serverRelativeVersionedLayoutsFolder);
            SilverlightVideoContentPanel._isSilverlightInstalled = silverlightInstalled;
        }

        public applyConfigTransform(panelConfig: RelatedContentPanelConfig, serverRelativeVersionedLayoutsFolder: any): RelatedContentPanelConfig {
            if (serverRelativeVersionedLayoutsFolder.charAt(serverRelativeVersionedLayoutsFolder.length - 1) == '/') {
                serverRelativeVersionedLayoutsFolder = serverRelativeVersionedLayoutsFolder.substr(0, serverRelativeVersionedLayoutsFolder.length - 1); //trim the trailing slash
            }
            var PLACEHOLDER = "{SERVER_RELATIVE_VERSIONED_LAYOUTS_FOLDER}"
            panelConfig.Content = panelConfig.Content.replace(PLACEHOLDER, serverRelativeVersionedLayoutsFolder);
            return panelConfig;
        }

        public stopSilverlightVideoPlayer() {
            if (SilverlightVideoContentPanel.videoPlayerJSBridge != null) {
                try {
                    SilverlightVideoContentPanel.videoPlayerJSBridge.StopVideoPlayer();
                }
                catch (err) {
                    //This call will fail in Chrome because it destroys the object when it is hidden.
                    //Still need to call StopVideoPlayer when it's IE to kill the audio that will continue when not visible.
                }
            }
        }

        public videoPlayerDisposed() {
            SilverlightVideoContentPanel.videoPlayerJSBridge = null;
        }

        public static SetVideoPlayerJSBridge(videoPlayerJSBridge): void {
            SilverlightVideoContentPanel.videoPlayerJSBridge = videoPlayerJSBridge;
        }

        public sendSilverlightVideoPlayerMessage(message: string): void {
            if (SilverlightVideoContentPanel.videoPlayerJSBridge != null) {
                SilverlightVideoContentPanel.videoPlayerJSBridge.ReceiveVideoPlayerMessage(message);
            }
            else {
                this.retrySendSilverlightVideoPlayerMessage(message); //retry until the SL control has reloaded
            }
        }

        private retrySendSilverlightVideoPlayerMessage(message) {
            var silverlightVideoPanel = this;
            setTimeout(function () { silverlightVideoPanel.sendSilverlightVideoPlayerMessage(message); }, 100); //retry every 100ms
        }
    }
} 