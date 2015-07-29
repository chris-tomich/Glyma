module Glyma.MappingTool {
    export class SilverlightMappingTool {
        private static _instance: SilverlightMappingTool = null;
        private _glymaJSBridgeObject = null;
        private _isSet: boolean = false;

        constructor() {
            if (SilverlightMappingTool._instance != null) {
                throw new Error("Error: Instantiation failed: Use SilverlightMappingTool.getInstance() instead of new");
            }
            SilverlightMappingTool._instance = this;
        }

        public static getInstance(): SilverlightMappingTool {
            if (SilverlightMappingTool._instance === null) {
                SilverlightMappingTool._instance = new SilverlightMappingTool();
            }
            return SilverlightMappingTool._instance;
        }

        public setGlymaJSBridge(glymaJSBridge: any): void {
            this._glymaJSBridgeObject = glymaJSBridge;
            MappingToolController.setType(MappingToolType.Silverlight);

            this._isSet = true;

            //Now that Silverlight has loaded add in the context menues to the application
            var mappingToolController: MappingToolController = MappingToolController.getInstance();
            if (mappingToolController != null) {
                mappingToolController.ProcessAddingContextMenuItems();
            }
        }

        public static IsSet(): boolean {
            if (SilverlightMappingTool.getInstance()._glymaJSBridgeObject === null) {
                return false;
            }
            else {
                if (SilverlightMappingTool.getInstance()._isSet) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }

        public LoadMapAndSelectNode(domainId: string,
                                            mapNodeId: string,
                                            currentNode: string): void {
            if (SilverlightMappingTool.IsSet()) {
                this._glymaJSBridgeObject.LoadMapAndSelectNode(domainId, mapNodeId, currentNode);
            }
            else {
                //throw new Error("Glyma's Silverlight object not set");
            }
        }

        public GetCurrentDomainUid(): string {
            if (SilverlightMappingTool.IsSet()) {
                return this._glymaJSBridgeObject.GetCurrentDomainUid();
            }
            else {
                //throw new Error("Glyma's Silverlight object not set");
            }
        }

        public GetCurrentRootMapUid(): string {
            if (SilverlightMappingTool.IsSet()) {
                return this._glymaJSBridgeObject.GetCurrentRootMapUid();
            }
            else {
                //throw new Error("Glyma's Silverlight object not set");
            }
        }

        public GetRootMapMetadataValue(metadataKey:string): string {
            if (SilverlightMappingTool.IsSet()) {
                return this._glymaJSBridgeObject.GetRootMapMetadataValue(metadataKey);
            }
            else {
                //throw new Error("Glyma's Silverlight object not set");
            }
        }

        public GetRootMapUrl(): string {
            if (SilverlightMappingTool.IsSet()) {
                return this._glymaJSBridgeObject.GetRootMapUrl();
            }
            else {
                //throw new Error("Glyma's Silverlight object not set");
            }
        }

        public ReceiveGlymaMessage(message: string): void {
            if (SilverlightMappingTool.IsSet()) {
                this._glymaJSBridgeObject.ReceiveGlymaMessage(message);
            }
            else {
                //throw new Error("Glyma's Silverlight object not set");
            }
        }

        public AddAuthorContextMenuItem(contextMenuItemText: string) {
            if (SilverlightMappingTool.IsSet()) {
                this._glymaJSBridgeObject.AddAuthorContextMenuItem(contextMenuItemText);
            }
            else {
                //throw new Error("Glyma's Silverlight object not set");
            }
        }

        public AddReaderContextMenuItem(contextMenuItemText: string) {
            if (SilverlightMappingTool.IsSet()) {
                this._glymaJSBridgeObject.AddReaderContextMenuItem(contextMenuItemText);
            }
            else {
                //throw new Error("Glyma's Silverlight object not set");
            }
        }
    }
} 