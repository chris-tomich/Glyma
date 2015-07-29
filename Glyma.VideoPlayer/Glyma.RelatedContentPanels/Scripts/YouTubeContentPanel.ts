/// <reference path="RelatedContentPanel.ts"/>
/// <reference path="VideoContentPanel.ts"/>
module Glyma.RelatedContentPanels {
    export class YouTubeContentPanel extends RelatedContentPanel {

        private static _youTubePlayer = null;
        private static _youTubePlayerReady: boolean = false;
        private static _youTubePlayerState: string = "Idle";
        private static _cachedYouTubeCommand = null;

        private _videoSize: number = -1;

        private _lastSelectedSize: number = 3;
        private _muteSizeChangedSelectionEvents: boolean = false;

        public CurrentNode = {
            Source: null,
            NodeId: "00000000-0000-0000-0000-000000000000",
            EndTimeCodeProvided: false,
            StartTimeCodeProvided: false,
            EndTime: 0,
            StartTime: 0
        };

        private static _panelConfig =
        {
            Disabled: true,
            PanelId: "YouTubePanel",
            PanelTitle: "YOUTUBE VIDEO",
            Sortable: false,
            Init: function () {
                YouTubeContentPanel.InjectYouTubeAPI();
                this.Panel.close(false);
                this.Panel.disableContentPanel(false);
                this.Panel.setHasContent(true);

                var panel: YouTubeContentPanel = <YouTubeContentPanel>this.Panel;

                //setup the event handler for the youtube address bar button
                $("#SetYouTubeAddressButton").click(function () {
                    var address = $("#YouTubeAddress").val();
                    if (YouTubeContentPanel._youTubePlayer != null) {
                        var url = Utils.ConvertToYouTubeEmbedUrl(address);
                        if (url != null) {
                            panel.loadOrCueYouTubeUrl({
                                AddressBarUsed: true,
                                url: url
                            });
                        }
                    }
                });

                $("#YouTubeAddress").keydown(function (event:JQueryKeyEventObject) {
                    if (event.which == 13) {
                        var address = $("#YouTubeAddress").val();
                        if (YouTubeContentPanel._youTubePlayer != null) {
                            var url = Utils.ConvertToYouTubeEmbedUrl(address);
                            if (url != null) {
                                panel.loadOrCueYouTubeUrl({
                                    AddressBarUsed: true,
                                    url: url
                                });
                            }
                        }
                    }
                });

                $('#YouTubeSizeSelect').change(function (eventObject: JQueryEventObject) {
                    if (panel != null) {
                        if (!panel._muteSizeChangedSelectionEvents) {
                            var controller = RelatedContentController.getInstance();
                            var currentMaxPanelWidth = controller.getMaxContentWidth();
                            if (YouTubeContentPanel._youTubePlayer != null) {
                                var selectedValue = $(this).val();
                                var newWidth = 400;
                                switch (selectedValue) {
                                    case "1":
                                        newWidth = 800;
                                        panel._lastSelectedSize = 1;
                                        break;
                                    case "2":
                                        newWidth = 600;
                                        panel._lastSelectedSize = 2;
                                        break;
                                    case "3":
                                        newWidth = 400;
                                        panel._lastSelectedSize = 3;
                                        break;
                                }
                                controller.setMaxWidth(newWidth, panel.getPanelId());
                                controller.onRelatedContentPanelResize();
                            }
                        }
                    }
                });
            },
            Reset: function () {
                this.Panel.close(true);
                this.Panel.disableContentPanel(false);
                this.Panel.setHasContent(true);
            },
            OnShow: function () {
                var panel: YouTubeContentPanel = <YouTubeContentPanel>this.Panel;
                if (this.Controller.getAuthorMode()) {
                    $("#YouTubeAddressBar").show();
                }
                else {
                    $("#YouTubeAddressBar").hide();
                }

                VideoController.CurrentVideoPlayerType = VideoPlayerType.YOUTUBE;
                var videoPanel: RelatedContentPanel = this.Controller.contentPanels["SilverlightVideoPanel"];
                if (videoPanel != null) {
                    videoPanel.collapseContentPanel(true);
                    videoPanel.disableContentPanel(false);
                }

                panel.addPopoutLink(panel.popOutIframeWindow, null);
                panel.sendYouTubePlayerInitialised();

                var newWidth = 400;
                switch (panel._lastSelectedSize) {
                    case 1:
                        newWidth = 800;
                        break;
                    case 2:
                        newWidth = 600;
                        break;
                    case 3:
                        newWidth = 400;
                        break;
                }
                this.Controller.setMaxWidth(newWidth, panel.getPanelId());

                this.Controller.onRelatedContentPanelResize();
            },
            OnClose: function () {
                var panel: YouTubeContentPanel = <YouTubeContentPanel>this.Panel;
                if (panel != null && panel.isWidestPanel()) {
                    this.Controller.setMaxWidth(this.Controller.DEFAULT_CONTENT_WIDTH, panel.getPanelId()); //reset the max panel width if the large content is closed
                }
                if (YouTubeContentPanel._youTubePlayer != null) {
                    YouTubeContentPanel._youTubePlayer.pauseVideo(); //stop the video playing when it's hidden
                }
            },
            SizeChanged: function () {
                var maxWidth = this.Controller.getMaxContentWidth();
                $("#YouTubePanel").css("width", maxWidth + "px");
                if (YouTubeContentPanel._youTubePlayer != null) {
                    YouTubeContentPanel._youTubePlayer.setSize(maxWidth, Math.round(0.5625 * maxWidth));
                }
                this.Panel._muteSizeChangedSelectionEvents = true;
                if (maxWidth >= 400 && maxWidth < 600) {
                    $('#YouTubeSizeSelect').val("3");
                }
                else if (maxWidth >= 600 && maxWidth < 800) {
                    $('#YouTubeSizeSelect').val("2");
                }
                else if (maxWidth >= 800) {
                    $('#YouTubeSizeSelect').val("1");
                }
                else {
                    $('#YouTubeSizeSelect').val("3");
                }
                this.Panel._muteSizeChangedSelectionEvents = false;
            },
            Content: "<div id='YouTubeAddressBar'>" +
            "<input type='button' value='GO' id='SetYouTubeAddressButton'/>" +
            "<label id='YouTubeAddressLabel'>YouTube URL:</label>" +
            "<span><input type='text' id='YouTubeAddress'/></span>" +
            "</div>" +
            "<div id='YouTubePlayer'></div>" +
            "<div id='YouTubeSizeChoices'><span class='you-tube-videosize-label'>Video Size:</span> " +
            "<select id='YouTubeSizeSelect'><option value='1'>Large</option><option value='2'>Medium</option><option value='3' selected>Small</option></select></div>",
            Icon: "{BASE_URL}/Style Library/Glyma/Icons/youtube.png",
            IconHover: "{BASE_URL}/Style Library/Glyma/Icons/youtube-hover.png",
            IconDisabled: "{BASE_URL}/Style Library/Glyma/Icons/youtube-unavailable.png"
        };

        constructor() {
            super(YouTubeContentPanel._panelConfig, null);
        }

        private static InjectYouTubeAPI() {
            var tag = document.createElement('script');
            tag.src = "https://www.youtube.com/iframe_api";
            var firstScriptTag = document.getElementsByTagName('script')[0];
            firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
        }

        public setSize(videoSize: string): void {
            var newWidth: number = 400;
            switch (videoSize) {
                case "large":
                    newWidth = 800;
                    this._lastSelectedSize = 1;
                    break;
                case "medium":
                    newWidth = 600;
                    this._lastSelectedSize = 2;
                    break;
                case "small":
                    newWidth = 400;
                    this._lastSelectedSize = 3;
                    break;
                case "notspecified":
                    return; //don't change the size at all
                    break;
            }
            var controller: RelatedContentController = RelatedContentController.getInstance();
            if (controller != null) {
                controller.setMaxWidth(newWidth, this.getPanelId());
                if (YouTubeContentPanel._youTubePlayer == null) {
                    this._videoSize = newWidth;
                }
                controller.onRelatedContentPanelResize();
            }
        }

        public popOutIframeWindow(panel: RelatedContentPanel, params) {
            var panelEl = $("#" + panel.getPanelId());
            var icons = $(panelEl).find("span.panel-icons");
            var closeIcon = $(icons).find("div.close-icon");
            var currentVideo = this.getCurrentVideoUrl();
            if (currentVideo != null) {
                window.open(currentVideo, "_blank");
                this.handleYouTubePause(); //pause the player since it's opening in a new window
            }
        }

        public onYouTubePlayerReady(event): void {
            if (event) {
                YouTubeContentPanel._youTubePlayer = event.target;
                YouTubeContentPanel._youTubePlayerReady = true;
                var youTubePanel: YouTubeContentPanel = <YouTubeContentPanel>VideoController.GetPlayerPanel(VideoPlayerType.YOUTUBE);
                if (youTubePanel != null) {
                    youTubePanel.sendYouTubePlayerInitialised();
                    if (YouTubeContentPanel._cachedYouTubeCommand != null) {
                        youTubePanel.sendYouTubePlayerMessage(YouTubeContentPanel._cachedYouTubeCommand);
                        YouTubeContentPanel._cachedYouTubeCommand = null;
                    }
                    if (youTubePanel._videoSize != -1) {
                        YouTubeContentPanel._youTubePlayer.setSize(youTubePanel._videoSize, Math.round(0.5625 * youTubePanel._videoSize));
                        youTubePanel._videoSize = -1;
                    }
                }
            }
        }

        public onYouTubePlayerStateChange(event) {
            var youTubePanel: YouTubeContentPanel = <YouTubeContentPanel>VideoController.GetPlayerPanel(VideoPlayerType.YOUTUBE);
            if (event.data == YT.PlayerState.PLAYING) {
                YouTubeContentPanel._youTubePlayerState = "Playing";
                var eventXml = VideoController.CreateVideoEventXml("CurrentStateChanged", [{ Name: "NodeId", Value: youTubePanel.CurrentNode.NodeId }, { Name: "State", Value: "Playing" }]);
                Glyma.MappingTool.MappingToolController.getInstance().SendGlymaMessage(eventXml);
            }
            else if (event.data == YT.PlayerState.PAUSED || event.data == YT.PlayerState.ENDED || event.data == YT.PlayerState.CUED || event.data == -1) {
                YouTubeContentPanel._youTubePlayerState = "Idle";
                var eventXml = VideoController.CreateVideoEventXml("CurrentStateChanged", [{ Name: "NodeId", Value: youTubePanel.CurrentNode.NodeId }, { Name: "State", Value: "Idle" }]);
                Glyma.MappingTool.MappingToolController.getInstance().SendGlymaMessage(eventXml);
            }
            else if (event.data == YT.PlayerState.BUFFERING) {
                YouTubeContentPanel._youTubePlayerState = "Buffering";
                var eventXml = VideoController.CreateVideoEventXml("CurrentStateChanged", [{ Name: "NodeId", Value: youTubePanel.CurrentNode.NodeId }, { Name: "State", Value: "Buffering" }]);
                Glyma.MappingTool.MappingToolController.getInstance().SendGlymaMessage(eventXml);
            }
        }

        public onYouTubePlayerError(event): void {
            //TODO: implement any error handling
            var youTubePanel: YouTubeContentPanel = <YouTubeContentPanel>VideoController.GetPlayerPanel(VideoPlayerType.YOUTUBE);
            YouTubeContentPanel._youTubePlayerState = "Idle";
            var eventXml = VideoController.CreateVideoEventXml("CurrentStateChanged", [{ Name: "NodeId", Value: youTubePanel.CurrentNode.NodeId }, { Name: "State", Value: "Idle" }]);
            Glyma.MappingTool.MappingToolController.getInstance().SendGlymaMessage(eventXml);
        }

        private sendYouTubePlayerInitialised(): void {
            var eventXml = VideoController.CreateVideoEventXml("Initialised", null);
            Glyma.MappingTool.MappingToolController.getInstance().SendGlymaMessage(eventXml);
        }

        public sendYouTubePlayerMessage(xmlMessage: string): void {
            if (YouTubeContentPanel._youTubePlayerReady) {
                var commandName = VideoController.GetVideoCommandName(xmlMessage);

                switch (commandName.toLowerCase()) {
                    case "play":
                        this.handleYouTubePlay(xmlMessage);
                        break;
                    case "pause":
                        this.handleYouTubePause();
                        break;
                    case "stop":
                        this.handleYouTubeStop();
                        break;
                    case "getsourceandposition":
                        this.handleYouTubeGetSourceAndPosition(xmlMessage);
                        break;
                }
            }
            else {
                YouTubeContentPanel._cachedYouTubeCommand = xmlMessage;
            }
        }

        private handleYouTubePause(): void {
            if (YouTubeContentPanel._youTubePlayer != null) {
                YouTubeContentPanel._youTubePlayer.pauseVideo();
            }
        }

        private handleYouTubePlay(command: string): void {
            var sourceValue = VideoController.GetVideoCommandParam(command, "Source");
            var nodeId = VideoController.GetVideoCommandParam(command, "NodeId");
            var startTimeCode = VideoController.GetVideoCommandParam(command, "StartTimeCode");

            if (startTimeCode != null) {
                this.CurrentNode.StartTimeCodeProvided = true;
                this.CurrentNode.StartTime = Utils.ConvertTimeSpanToSeconds(startTimeCode);
            }
            var endTimeCode = VideoController.GetVideoCommandParam(command, "EndTimeCode");

            if (endTimeCode != null) {
                this.CurrentNode.EndTimeCodeProvided = true;
                this.CurrentNode.EndTime = Utils.ConvertTimeSpanToSeconds(endTimeCode);
            }
            if (YouTubeContentPanel._youTubePlayer != null) {
                $("#YouTubeAddress").val(sourceValue);
                var url = Utils.ConvertToYouTubeEmbedUrl(sourceValue);
                if (this.CurrentNode.NodeId == nodeId) {
                    if (this.CurrentNode.Source == sourceValue &&
                        YouTubeContentPanel._youTubePlayerState == "Idle") {
                        if (!this.CurrentNode.EndTimeCodeProvided) {
                            YouTubeContentPanel._youTubePlayer.playVideo();
                        }
                        else {
                            if (this.CurrentNode.StartTimeCodeProvided) {
                                if (YouTubeContentPanel._youTubePlayer.getCurrentTime() < this.CurrentNode.EndTime &&
                                    YouTubeContentPanel._youTubePlayer.getCurrentTime() > this.CurrentNode.StartTime) {
                                    YouTubeContentPanel._youTubePlayer.playVideo();
                                }
                                else {
                                    this.loadOrCueYouTubeUrl({
                                        url: url,
                                        StartTime: this.CurrentNode.StartTime,
                                        EndTime: this.CurrentNode.EndTime
                                    });
                                }
                            }
                            else {
                                if (YouTubeContentPanel._youTubePlayer.getCurrentTime() < this.CurrentNode.EndTime) {
                                    YouTubeContentPanel._youTubePlayer.playVideo();
                                }
                                else {
                                    this.loadOrCueYouTubeUrl({
                                        url: url,
                                        StartTime: 0,
                                        EndTime: this.CurrentNode.EndTime
                                    });
                                }
                            }
                        }
                    }

                    else {
                        if (this.CurrentNode.StartTimeCodeProvided && this.CurrentNode.EndTimeCodeProvided) {
                            this.loadOrCueYouTubeUrl({
                                url: url,
                                StartTime: this.CurrentNode.StartTime,
                                EndTime: this.CurrentNode.EndTime
                            });
                        }
                        else if (this.CurrentNode.StartTimeCodeProvided && !this.CurrentNode.EndTimeCodeProvided) {
                            this.loadOrCueYouTubeUrl({
                                url: url,
                                StartTime: this.CurrentNode.StartTime
                            });
                        }
                        else if (!this.CurrentNode.StartTimeCodeProvided && this.CurrentNode.EndTimeCodeProvided) {
                            this.loadOrCueYouTubeUrl({
                                url: url,
                                StartTime: 0,
                                EndTime: this.CurrentNode.EndTime
                            });
                        }
                    }
                }
                else {
                    this.notifyYouTubeNodeStopped();
                    this.CurrentNode.NodeId = nodeId;
                    this.CurrentNode.Source = sourceValue;

                    if (this.CurrentNode.StartTimeCodeProvided && this.CurrentNode.EndTimeCodeProvided) {
                        this.loadOrCueYouTubeUrl({
                            url: url,
                            StartTime: this.CurrentNode.StartTime,
                            EndTime: this.CurrentNode.EndTime
                        });
                    }
                    else if (this.CurrentNode.StartTimeCodeProvided && !this.CurrentNode.EndTimeCodeProvided) {
                        this.loadOrCueYouTubeUrl({
                            url: url,
                            StartTime: this.CurrentNode.StartTime,
                        });
                    }
                    else if (!this.CurrentNode.StartTimeCodeProvided && this.CurrentNode.EndTimeCodeProvided) {
                        this.loadOrCueYouTubeUrl({
                            url: url,
                            StartTime: 0,
                            EndTime: this.CurrentNode.EndTime
                        });
                    }
                }
            }
        }

        private loadOrCueYouTubeUrl(params: any): void {
            if (YouTubeContentPanel.isApplePortableDevice()) {
                if (params.AddressBarUsed) {
                    YouTubeContentPanel._youTubePlayer.cueVideoByUrl(params.url, 0);
                }
                else {
                    if (params.EndTime != undefined && params.StartTime != undefined) {
                        YouTubeContentPanel._youTubePlayer.cueVideoByUrl(
                            {
                                mediaContentUrl: params.url,
                                startSeconds: params.StartTime,
                                endSeconds: params.EndTime
                            });
                    }
                    else if (params.EndTime == undefined && params.StartTime != undefined) {
                        YouTubeContentPanel._youTubePlayer.cueVideoByUrl(
                            {
                                mediaContentUrl: params.url,
                                startSeconds: params.StartTime
                            });
                    }
                }
            }
            else {
                if (params.AddressBarUsed) {
                    YouTubeContentPanel._youTubePlayer.loadVideoByUrl(params.url, 0);
                }
                else {
                    if (params.EndTime != undefined && params.StartTime != undefined) {
                        YouTubeContentPanel._youTubePlayer.loadVideoByUrl(
                            {
                                mediaContentUrl: params.url,
                                startSeconds: params.StartTime,
                                endSeconds: params.EndTime
                            });
                    }
                    else if (params.EndTime == undefined && params.StartTime != undefined) {
                        YouTubeContentPanel._youTubePlayer.loadVideoByUrl(
                            {
                                mediaContentUrl: params.url,
                                startSeconds: params.StartTime
                            });
                    }
                }
            }
        }

        private notifyYouTubeNodeStopped(): void {
            if (this.CurrentNode.NodeId != "00000000-0000-0000-0000-000000000000") {
                var eventXml = VideoController.CreateVideoEventXml("CurrentStateChanged", [{ Name: "NodeId", Value: this.CurrentNode.NodeId }, { Name: "State", Value: "Idle" }]);
                Glyma.MappingTool.MappingToolController.getInstance().SendGlymaMessage(eventXml);
            }
        }

        private handleYouTubeStop(): void {
            if (YouTubeContentPanel._youTubePlayer != null) {
                YouTubeContentPanel._youTubePlayer.stopVideo();
            }
        }

        private handleYouTubeGetSourceAndPosition(xmlMessage: string): void {
            var nodeId = VideoController.GetVideoCommandParam(xmlMessage, "NodeId");
            var callbackId = VideoController.GetVideoCommandParam(xmlMessage, "CallbackId");

            if (YouTubeContentPanel._youTubePlayer != null) {
                var currentPosition = YouTubeContentPanel._youTubePlayer.getCurrentTime();
                currentPosition = Utils.ConvertSecondsToTimeSpanString(currentPosition);
                var videoSource = YouTubeContentPanel._youTubePlayer.getVideoUrl();
                videoSource = encodeURIComponent(videoSource);
            }
            var eventXml = VideoController.CreateVideoEventXml("GetSourceAndPositionCallback", [{ Name: "NodeId", Value: nodeId }, { Name: "CallbackId", Value: callbackId },
                { Name: "Source", Value: videoSource }, { Name: "Position", Value: currentPosition }]);
            Glyma.MappingTool.MappingToolController.getInstance().SendGlymaMessage(eventXml);
        }

        public getCurrentVideoUrl(): string {
            var videoSource: string = null;
            if (YouTubeContentPanel._youTubePlayer != null) {
               videoSource = YouTubeContentPanel._youTubePlayer.getVideoUrl();
            }
            return videoSource;
        }

        private static isApplePortableDevice():boolean {
            return (
                //Detect iPhone
                (navigator.platform.indexOf("iPhone") != -1) ||
                //Detect iPod
                (navigator.platform.indexOf("iPod") != -1) ||
                //Detect iPad
                (navigator.platform.indexOf("iPad") != -1)
                );
        }
    }
} 