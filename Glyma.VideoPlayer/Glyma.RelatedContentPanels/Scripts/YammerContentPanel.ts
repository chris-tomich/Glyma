/// <reference path="RelatedContentPanel.ts"/>
module Glyma.RelatedContentPanels {
    export class YammerContentPanel extends RelatedContentPanel {

        private static _panelConfig: RelatedContentPanelConfig = {
            Disabled: false,
            PanelId: "YammerPanel",
            PanelTitle: "YAMMER",
            Sortable: true,
            Init: function () {
                this.Panel.close(true);
                this.Panel.setHasContent(true);
            },
            Reset: function () {
                YammerContentPanel.ClearYammerFeed();
                this.Panel.close(true);
                this.Panel.setHasContent(true);
            },
            OnShow: function () {
                this.Controller.onRelatedContentPanelResize();
                YammerContentPanel.SetYammerFeed();
                $("#yammer-embedded-feed").css("width", this.Controller.getMaxContentWidth() + "px");
            },
            OnClose: function () {
                YammerContentPanel.ClearYammerFeed();
            },
            SizeChanged: function () {
                $("#yammer-embedded-feed").css("width", this.Controller.getMaxContentWidth() + "px");
            },
            Content: "<div id=\"yammer-embedded-feed\" style=\"height:600px;width:400px;\"></div>",
            Icon: "{BASE_URL}/Style Library/Glyma/Icons/yammer.png",
            IconHover: "{BASE_URL}/Style Library/Glyma/Icons/yammer-hover.png",
            IconDisabled: "{BASE_URL}/Style Library/Glyma/Icons/yammer-unavailable.png"
        };

        constructor() {
            super(YammerContentPanel._panelConfig, null);
        } 

        public static IsYammerEnabled() {
            if ($("#YammerEmbedAPI").length > 0) {
                //check if the map has any Yammer network properties setup.
                var yammerNetworkId = Glyma.MappingTool.MappingToolController.getInstance().GetRootMapMetadataValue("Yammer.Network");
                if (yammerNetworkId != "") {
                    return true;
                }
                else {
                    //using the user's home network if no network stated
                    var yammerFeedType = Glyma.MappingTool.MappingToolController.getInstance().GetRootMapMetadataValue("Yammer.FeedType");
                    if (yammerFeedType != "") {
                        if (yammerFeedType.toLowerCase() == "group" || yammerFeedType.toLowerCase() == "topic" || yammerFeedType.toLowerCase() == "user") {
                            return true; //group, topic or user is all that is required, if no FeedId specified it will default to all
                        }
                        else if (yammerFeedType.toLowerCase() == "open-graph") {
                            return true; //open-graph is all that is required
                        }
                    }
                    return false; //the feed type wasn't valid
                }
            }
            else {
                return false; //the Yammer API is not available on this page
            }
        }

        public static SetYammerFeed() {
            var feedContent:string = "";
            var yammerNetworkId: string = Glyma.MappingTool.MappingToolController.getInstance().GetRootMapMetadataValue("Yammer.Network");

            var yammerFeedType: string = Glyma.MappingTool.MappingToolController.getInstance().GetRootMapMetadataValue("Yammer.FeedType");
            if (yammerFeedType == "") {
                yammerFeedType = "open-graph"; //default to a open-graph feed
            }

            var promptText: string = Glyma.MappingTool.MappingToolController.getInstance().GetRootMapMetadataValue("Yammer.PromptText");
            if (promptText == "") {
                promptText = "Say something about this map";
            }

            yammerFeedType = yammerFeedType.toLowerCase();
            if (yammerFeedType == "open-graph") {
                var objectUrl: string = Glyma.MappingTool.MappingToolController.getInstance().GetRootMapMetadataValue("Yammer.ObjectUrl");
                if (objectUrl == "") {
                    objectUrl = Glyma.MappingTool.MappingToolController.getInstance().GetRootMapUrl();
                }

                if (yammerNetworkId != "") {
                    //Open-Graph with a specific object specified on a network
                    feedContent = "<script> yam.connect.embedFeed({" +
                                    "container: \"#yammer-embedded-feed\"," +
                                    "network: \"" + yammerNetworkId + "\", " +
                                    "feedType: \"open-graph\"," +
                                    "objectProperties: {" +
                                    " url: \"" + objectUrl + "\"" +
                                    "}," +
                                    "config: {" +
                                    "   promptText: \"" + promptText + "\"" +
                                    "}" +
                                    "});<\/script>";
                }
                else {
                    //Open-Graph with a specific object specified on any network.
                    feedContent = "<script> yam.connect.embedFeed({" +
                                    "container: \"#yammer-embedded-feed\"," +
                                    "feedType: \"open-graph\"," +
                                    "objectProperties: {" +
                                    " url: \"" + objectUrl + "\"" +
                                    "}," +
                                    "config: {" +
                                    "   promptText: \"" + promptText + "\"" +
                                    "}" +
                                    "});<\/script>";
                }
            }
            else if (yammerFeedType == "group" || yammerFeedType == "topic" || yammerFeedType == "user") {
                var yammerFeedId: string = Glyma.MappingTool.MappingToolController.getInstance().GetRootMapMetadataValue("Yammer.FeedId");
                if (yammerFeedId == "") {
                    yammerFeedId = "all"; //default to all in the feed
                }

                if (yammerNetworkId != "") {
                    feedContent = "<script> yam.connect.embedFeed({" +
                                    "container: \"#yammer-embedded-feed\"," +
                                    "network: \"" + yammerNetworkId + "\", " +
                                    "feedType: \"" + yammerFeedType + "\"," +
                                    "feedId: \"" + yammerFeedId + "\"," +
                                    "config: {" +
                                    "   promptText: \"" + promptText + "\"" +
                                    "}" +
                                    "});<\/script>";
                }
                else {
                    feedContent = "<script> yam.connect.embedFeed({" +
                                    "container: \"#yammer-embedded-feed\"," +
                                    "feedType: \"" + yammerFeedType + "\"," +
                                    "feedId: \"" + yammerFeedId + "\"," +
                                    "config: {" +
                                    "   promptText: \"" + promptText + "\"" +
                                    "}" +
                                    "});<\/script>";
                }
            }

            $("#yammer-embedded-feed").html(feedContent);
        }

        public static ClearYammerFeed() {
            $("#yammer-embedded-feed").html("");
        }
    }
} 