module Glyma.MappingTool {
    export class Html5MappingTool {
        private static _instance: Html5MappingTool = null;

        constructor() {
            if (Html5MappingTool._instance != null) {
                throw new Error("Error: Instantiation failed: Use Html5MappingTool.getInstance() instead of new");
            }
            Html5MappingTool._instance = this;
        }

        public static getInstance(): Html5MappingTool {
            if (Html5MappingTool._instance === null) {
                Html5MappingTool._instance = new Html5MappingTool();
            }
            return Html5MappingTool._instance;
        }

        public static IsLoaded(): boolean {
            if (typeof (Glyma) === "object" &&
                typeof (Glyma.Html5) === "object" &&
                typeof (Glyma.Html5.MappingTool) === "object" &&
                typeof (Glyma.Html5.MappingTool.RelatedContentPanelBridge) === "function") {
                return true;
            }
            else {
                return false;
            }
        }

        public LoadMapAndSelectNode(domainId: string,
                                            mapNodeId: string,
                                            currentNode: string): void {
            if (Html5MappingTool.IsLoaded()) {
                Glyma.Html5.MappingTool.RelatedContentPanelBridge.loadMapAndSelectedNode(domainId, mapNodeId, currentNode);
            }
            else {
                //throw new Error("Glyma's HTML5 Mapping Tool is not loaded");
            }
        }

        public GetCurrentDomainUid(): string {
            if (Html5MappingTool.IsLoaded()) {
                return Glyma.Html5.MappingTool.RelatedContentPanelBridge.getCurrentDomainUid();
            }
            else {
                //throw new Error("Glyma's HTML5 Mapping Tool is not loaded");
            }
        }

        public GetCurrentRootMapUid(): string {
            if (Html5MappingTool.IsLoaded()) {
                return Glyma.Html5.MappingTool.RelatedContentPanelBridge.getCurrentRootMapUid();
            }
            else {
                //throw new Error("Glyma's HTML5 Mapping Tool is not loaded");
            }
        }

        public GetRootMapMetadataValue(metadataKey:string): string {
            if (Html5MappingTool.IsLoaded()) {
                return Glyma.Html5.MappingTool.RelatedContentPanelBridge.getRootMapMetadataValue(metadataKey);
            }
            else {
                //throw new Error("Glyma's HTML5 Mapping Tool is not loaded");
            }
        }

        public GetRootMapUrl(): string {
            if (Html5MappingTool.IsLoaded()) {
                return Glyma.Html5.MappingTool.RelatedContentPanelBridge.getRootMapUrl();
            }
            else {
                //throw new Error("Glyma's HTML5 Mapping Tool is not loaded");
            }
        }

        public ReceiveGlymaMessage(message: string): void {
            if (Html5MappingTool.IsLoaded()) {
                Glyma.Html5.MappingTool.RelatedContentPanelBridge.receiveGlymaMessage(message);
            }
            else {
                //throw new Error("Glyma's HTML5 Mapping Tool is not loaded");
            }
        }

        public AddAuthorContextMenuItem(contextMenuItemText: string) {
            if (Html5MappingTool.IsLoaded()) {
                //TODO: Add implementation when a context menu is added to the HTML5 version of Glyma
            }
            else {
                //throw new Error("Glyma's HTML5 Mapping Tool is not loaded");
            }
        }

        public AddReaderContextMenuItem(contextMenuItemText: string) {
            if (Html5MappingTool.IsLoaded()) {
                //TODO: Add implementation when a context menu is added to the HTML5 version of Glyma
            }
            else {
                //throw new Error("Glyma's HTML5 Mapping Tool is not loaded");
            }
        }
    }
} 