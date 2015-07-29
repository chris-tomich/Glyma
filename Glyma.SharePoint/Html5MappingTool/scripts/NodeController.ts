module Glyma.NodeController {
    export function playVideo(index: number) {
        if (typeof(Glyma.RelatedContentPanels.RelatedContentController) === "function" && typeof(Glyma.RelatedContentPanels.VideoController) === "function") {
            var controller: Glyma.RelatedContentPanels.RelatedContentController = Glyma.RelatedContentPanels.RelatedContentController.getInstance();
            if (controller != null) {
                var node: Node = Glyma.SharedVariables.mapController.nodes[index];

                if (node.hasVideo) {
                    controller.loadRelatedContent("video", node.videoSource, 0, 0);
                    var command = Glyma.RelatedContentPanels.VideoController.CreateVideoCommandXml("Play", node.videoParams);
                    Glyma.RelatedContentPanels.VideoController.SendVideoPlayerMessage(command);

                    if (node.nodeActionOptions.showRelatedContentWithVideo) {
                        showContent(index);
                    }
                }
            }
        }
    }

    export function stopVideo() {
        if (typeof (Glyma.RelatedContentPanels.VideoController) === "function") {
            var command = Glyma.RelatedContentPanels.VideoController.CreateVideoCommandXml("Pause", null);
            Glyma.RelatedContentPanels.VideoController.SendVideoPlayerMessage(command);
        }
    }

    export function showRelatedMaps(maps: any) {
        if (typeof(Glyma.RelatedContentPanels.RelatedContentController) === "function") {
            var controller: Glyma.RelatedContentPanels.RelatedContentController = Glyma.RelatedContentPanels.RelatedContentController.getInstance();
            if (controller != null) {
                var mapsPanel: Glyma.RelatedContentPanels.RelatedMapsContentPanel = <Glyma.RelatedContentPanels.RelatedMapsContentPanel>controller.getContentPanelByName("RelatedNodesPanel");
                if (mapsPanel != null) {
                    if (maps == null || maps === "undefined") {
                        mapsPanel.clearRelatedMaps();
                    }
                    else {
                        mapsPanel.showRelatedMaps(maps);
                    }
                }
            }
        }
    }

    export function showContent(index: number) {
        if (Glyma.SharedVariables.mapController.nodes[index].hasContent) {
            if (typeof(Glyma.RelatedContentPanels.RelatedContentController) === "function") {
                var controller: Glyma.RelatedContentPanels.RelatedContentController = Glyma.RelatedContentPanels.RelatedContentController.getInstance();
                if (controller != null) {
                    var descriptionType = Glyma.SharedVariables.mapController.nodes[index].descriptionType;
                    var width = parseInt(Glyma.SharedVariables.mapController.nodes[index].descriptionWidth);
                    var height = parseInt(Glyma.SharedVariables.mapController.nodes[index].descriptionHeight);
                    if (descriptionType == "iframeUrl") {
                        var url = Glyma.SharedVariables.mapController.nodes[index].descriptionUrl;
                        controller.loadRelatedContent(descriptionType, url, width, height);
                    } else {
                        var content = Glyma.SharedVariables.mapController.nodes[index].descriptionContent;
                        controller.loadRelatedContent(descriptionType, content, width, height);
                    }
                }
            }
        }
    }
}