module Glyma.Html5.MappingTool {
    export class RelatedContentPanelBridge {
        private static _instance: RelatedContentPanelBridge = null;

        constructor() {
            if (RelatedContentPanelBridge._instance != null) {
                throw new Error("Instantiation Error: Use RelatedContentPanelBridge.getInstance() instead of new.");
            }
            RelatedContentPanelBridge._instance = new RelatedContentPanelBridge();
        }

        public static getInstance(): RelatedContentPanelBridge {
            if (RelatedContentPanelBridge._instance === null) {
                RelatedContentPanelBridge._instance = new RelatedContentPanelBridge();
            }
            return RelatedContentPanelBridge._instance;
        }

        public static getRootMapUrl(): string {
            ///todo: 
            return "";
        }

        public static getCurrentRootMapUid(): string {
            return Glyma.SharedVariables.mapController.mapRenderer.rootMap.uniqueId;
        }

        public static getCurrentDomainUid(): string {
            return Glyma.SharedVariables.mapController.mapRenderer.domainId;
        }

        public static getRootMapMetadataValue(metadataKey: string): string {
            var parts = metadataKey.split("."),
                length = parts.length,
                i,
                property = Glyma.SharedVariables.mapController.mapRenderer.rootMap || this;

            for (i = 0; i < length; i++) {
                property = property[parts[i]];
                if (property == null || property === "undefined") {
                    return "";
                }
            }

            return property;
        }

        public static loadMapAndSelectedNode(domainId: string, mapNodeId: string, currentNode: string):void {
            Glyma.SharedVariables.mapController.getMapData(domainId, mapNodeId, currentNode);
        }
        
        public static receiveGlymaMessage(message: string):void {
            var eventName = RelatedContentPanelBridge.getVideoEventName(message);
            if (eventName == "CurrentStateChanged") {
                var nodeId = RelatedContentPanelBridge.getVideoEventArg(message,"NodeId");
                var state = RelatedContentPanelBridge.getVideoEventArg(message,"State");
                var index = Glyma.SharedVariables.mapController.getNodeIndexById(nodeId);
                if (index >= 0) {
                    var changeTo = "";
                    switch (state.toLowerCase()) {
                        case "playing":
                            changeTo = "pause";
                            break;
                        case "idle":
                            changeTo = "play";
                            break;
                        case "buffering":
                            changeTo = "pause";
                            break;
                        default:
                            changeTo = "play";
                            break;
                    }

                    if (Glyma.SharedVariables.mapController.nodes[index].nodeCornerButton.showingButton != changeTo) {
                        Glyma.SharedVariables.mapController.nodes[index].nodeCornerButton.showingButton = changeTo;
                        Glyma.SharedVariables.mapController.refresh();
                    }
                }
            }
        }

        public static getVideoEventArg(commandXml: string, eventArgName: string): string {
            var xmlDoc = $.parseXML(commandXml);
            var eventArgs = $(xmlDoc).find("EventArgs").children();
            var eventArgValue = null;
            $.each(eventArgs, function (index, eventArg) {
                if ($(this).find("Name").text() == eventArgName) {
                    eventArgValue = $(this).find("Value").text();
                    return false;
                }
            });
            return eventArgValue;
        }

        public static getVideoEventName(commandXml: string): string {
            var xmlDoc = $.parseXML(commandXml);
            var name = $(xmlDoc).children("Event").children("Name");
            return $(name).text();
        }

    }
}