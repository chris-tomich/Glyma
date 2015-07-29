/// <reference path="videocontentpanel.ts" />
/// <reference path="youtubecontentpanel.ts" />
/// <reference path="silverlightvideocontentpanel.ts" />
module Glyma.RelatedContentPanels {
    export class VideoController {
        private static _instance: VideoController = null;

        public static CurrentVideoPlayerType: VideoPlayerType = null;

        constructor() {
            if (VideoController._instance != null) {
                throw new Error("Error: Instantiation failed: Use VideoController.getInstance() instead of new ");
            }
            VideoController._instance = this;
        }

        public static GetPlayerPanel(videoPlayerType: VideoPlayerType): VideoContentPanel {
            var controller = RelatedContentController.getInstance();
            var playerPanel: VideoContentPanel = null;
            if (controller != null) {
                switch (videoPlayerType) {
                    case VideoPlayerType.YOUTUBE:
                        playerPanel = <VideoContentPanel>controller.getContentPanelByName("YouTubePanel");
                        break;
                    case VideoPlayerType.SILVERLIGHT:
                        playerPanel = <VideoContentPanel>controller.getContentPanelByName("SilverlightVideoPanel");
                        break;
                    case VideoPlayerType.HTML5:
                        playerPanel = null; //not implemented yet
                        break;
                    case null:
                        playerPanel = <VideoContentPanel>controller.getContentPanelByName("SilverlightVideoPanel");
                        break;
                }
            }
            return playerPanel;
        }

        static getInstance(): VideoController {
            if (VideoController._instance === null) {
                VideoController._instance = new VideoController();
            }
            return VideoController._instance;
        }

        static CreateVideoEventXml(eventName: string, eventArgs): string {
            var eventXml = "<Event xmlns:i='http://www.w3.org/2001/XMLSchema-instance' xmlns='http://schemas.datacontract.org/2004/07/VideoPlayerSharedLib'>";
            var eventArgXml = "";
            if (eventArgs == undefined || eventArgs === null || eventArgs.length == 0) {
                eventArgXml = "<EventArgs/>";
            }
            else {
                eventArgXml += "<EventArgs>";
                $.each(eventArgs, function (index, eventArg) {
                    eventArgXml += "<EventArg><Name>" + this.Name + "</Name><Value>" + this.Value + "</Value></EventArg>";
                });
                eventArgXml += "</EventArgs>";
            }
            eventXml += eventArgXml;
            eventXml += "<Name>" + eventName + "</Name>"
            eventXml += "</Event>";
            return eventXml;
        }

        static CreateVideoCommandXml(commandName: string, paramArgs): string {
            var commandXml = "<Command xmlns:i='http://www.w3.org/2001/XMLSchema-instance' xmlns='http://schemas.datacontract.org/2004/07/VideoPlayerSharedLib'>";
            commandXml += "<Name>" + commandName + "</Name>"
            var paramsXml = "";
            if (paramArgs == undefined || paramArgs === null || paramArgs.length == 0) {
                paramsXml = "<Params/>";
            }
            else {
                paramsXml += "<Params>";
                $.each(paramArgs, function (index, paramArg) {
                    var value = this.Value;
                    if (this.Name == "Source") {
                        value = decodeURIComponent(this.Value);
                    }
                    paramsXml += "<Param><Name>" + this.Name + "</Name><Value>" + value + "</Value></Param>";
                });
                paramsXml += "</Params>";
            }
            commandXml += paramsXml;
            commandXml += "</Command>";
            return commandXml;
        }

        public static SendVideoPlayerMessage(message:string):void {
            var commandName = VideoController.GetVideoCommandName(message);
            var playerPanel: VideoContentPanel = VideoController.GetPlayerPanel(VideoController.CurrentVideoPlayerType);
            if (playerPanel instanceof SilverlightVideoContentPanel || commandName.toLowerCase() == "initialised") {
                var silverlightVideoPanel: SilverlightVideoContentPanel = <SilverlightVideoContentPanel>playerPanel;
                silverlightVideoPanel.sendSilverlightVideoPlayerMessage(message);
            }
            else if (playerPanel instanceof YouTubeContentPanel) {
                var youTubePanel: YouTubeContentPanel = <YouTubeContentPanel>playerPanel;
                youTubePanel.sendYouTubePlayerMessage(message);
            }
            else {
                //UNKNOWN VIDEO PLAYER TYPE
            }
        }

        public static GetVideoCommandName(commandXML: string): string {
            var xmlDoc = $.parseXML(commandXML);
            var name = $(xmlDoc).children("Command").children("Name");
            return $(name).text();
        }

        public static GetVideoCommandParam(commandXML: string, paramName: string): string {
            var xmlDoc = $.parseXML(commandXML);
            var params = $(xmlDoc).find("Params").children();
            var paramValue = null;
            $.each(params, function (index, param) {
                if ($(this).find("Name").text() == paramName) {
                    paramValue = $(this).find("Value").text();
                    return false;
                }
            });
            return paramValue;
        }

        public loadVideoContent(params: any, videoSize: string): void {
            var url = decodeURIComponent(params);
            var playerType = Glyma.RelatedContentPanels.Utils.GetPlayerType(url);
            var playerPanel: VideoContentPanel = <VideoContentPanel>VideoController.GetPlayerPanel(playerType);
            if (playerPanel instanceof YouTubeContentPanel) {
                var youTubePanel: YouTubeContentPanel = <YouTubeContentPanel>playerPanel;
                if (youTubePanel != null) {
                    youTubePanel.setHasContent(true);
                    youTubePanel.expandContentPanel(true);
                    youTubePanel.setSize(videoSize);
                }
            }
            else if (playerPanel instanceof SilverlightVideoContentPanel) {
                var videoPanel: SilverlightVideoContentPanel = <SilverlightVideoContentPanel>playerPanel;
                if (videoPanel != null) {
                    videoPanel.setHasContent(true);
                    videoPanel.expandContentPanel(true);
                }
            }
        }
    }
} 