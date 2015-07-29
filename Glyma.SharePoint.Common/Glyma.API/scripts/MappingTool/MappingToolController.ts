/// <reference path="silverlightmappingtool.ts" />
module Glyma.MappingTool {
    export enum MappingToolType {
        Silverlight,
        HTML5,
        Unknown
    }

    export class MappingToolController {
        private static _instance: MappingToolController = null;
        private static _mappingToolType: MappingToolType = MappingToolType.Unknown;

        private _authorContextMenuCallbacks: { [key: string]: (rootMap: string, map: string, node: string) => void } = {};
        private _readerContextMenuCallbacks: { [key: string]: (rootMap: string, map: string, node: string) => void } = {};
        private static _mappingToolTypeIsSet: boolean = false;

        constructor() {
            if (MappingToolController._instance != null) {
                throw new Error("Error: Instantiation failed: Use MappingToolController.getInstance() instead of new");
            }
            MappingToolController._instance = this;
            this.determineType();
        }

        public static getInstance(): MappingToolController {
            if (MappingToolController._instance === null) {
                MappingToolController._instance = new MappingToolController();
            }
            return MappingToolController._instance;
        }

        /*
         * This function determines if it's the Silverlight Glyma Mapping Tool or HTML5 Glyma Mapping Tool that it should be communicating with.
         * The IsSet function is set to true when Silverlight loads successfully.
         */
        private determineType() {
            if (MappingTool.SilverlightMappingTool.IsSet()) {
                MappingToolController._mappingToolType = MappingToolType.Silverlight;
                MappingToolController._mappingToolTypeIsSet = true;
            }
            else if (MappingTool.Html5MappingTool.IsLoaded()) {
                MappingToolController._mappingToolType = MappingToolType.HTML5;
                MappingToolController._mappingToolTypeIsSet = true;
            }
            else {
                MappingToolController._mappingToolType = MappingToolType.Unknown;
                MappingToolController._mappingToolTypeIsSet = false;
            }
        }

        public static getType(): MappingToolType {
            return MappingToolController._mappingToolType;
        }

        public static setType(value: MappingToolType) {
            MappingToolController._mappingToolType = value;
            MappingToolController._mappingToolTypeIsSet = true;
        }

        /*
         * This function loads the specified map and centers on the current node passed in.
         */
        public LoadMapAndSelectNode(domainId: string, mapNodeId: string, currentNode: string): void {
            switch (MappingToolController.getType()) {
                case MappingToolType.Silverlight:
                    SilverlightMappingTool.getInstance().LoadMapAndSelectNode(domainId, mapNodeId, currentNode);
                    break;
                case MappingToolType.HTML5:
                    Html5MappingTool.getInstance().LoadMapAndSelectNode(domainId, mapNodeId, currentNode);
                    break;
            }
        }

        /*
         * This function returns the current domain unique identifier.
         */
        public GetCurrentDomainUid(): string {
            switch (MappingToolController.getType()) {
                case MappingToolType.Silverlight:
                    return SilverlightMappingTool.getInstance().GetCurrentDomainUid();
                    break;
                case MappingToolType.HTML5:
                    return Html5MappingTool.getInstance().GetCurrentDomainUid();
                    break;
            }
        }

        /*
         * This function returns the current root map unique identifier.
         */
        public GetCurrentRootMapUid(): string {
            switch (MappingToolController.getType()) {
                case MappingToolType.Silverlight:
                    return SilverlightMappingTool.getInstance().GetCurrentRootMapUid();
                    break;
                case MappingToolType.HTML5:
                    return Html5MappingTool.getInstance().GetCurrentRootMapUid();
                    break;
            }
        }

        /*
         * The function looks up a root map node's properties for the value by name.
         * 
         * Params:
         *  metadataKey: The key to the property eg Video.Source
         */
        public GetRootMapMetadataValue(metadataKey): string {
            switch (MappingToolController.getType()) {
                case MappingToolType.Silverlight:
                    return SilverlightMappingTool.getInstance().GetRootMapMetadataValue(metadataKey);
                    break;
                case MappingToolType.HTML5:
                    return Html5MappingTool.getInstance().GetRootMapMetadataValue(metadataKey);
                    break;
            }
        }

        /*
         * This function returns a URL to the current root map.
         * This URL is the unique identifier for the root map, used in open-graph it must be the same format exactly in both Silverlight and HTML5
         */
        public GetRootMapUrl(): string {
            switch (MappingToolController.getType()) {
                case MappingToolType.Silverlight:
                    return SilverlightMappingTool.getInstance().GetRootMapUrl();
                    break;
                case MappingToolType.HTML5:
                    return Html5MappingTool.getInstance().GetRootMapUrl();
                    break;
            }
        }

        /*
         * This function communicates with Glyma - it sends the video playback events to Glyma
         * 
         * Params:
         *  * message: The XML message.
         */
        public SendGlymaMessage(message: string): void {
            switch (MappingToolController.getType()) {
                case MappingToolType.Silverlight:
                    SilverlightMappingTool.getInstance().ReceiveGlymaMessage(message);
                    break;
                case MappingToolType.HTML5:
                    Html5MappingTool.getInstance().ReceiveGlymaMessage(message);
                    break;
            }
        }

        /*
         * This function adds a context menu item to the author context menu and associates a JavaScript callback that will be called when it's clicked.
         * 
         * Params:
         *  * contextMenuItemText: The text to display in the context menu
         *  * callbackMethod: The method to call when the context menu item is clicked.
         */
        public AddAuthorContextMenuItem(contextMenuItemText: string, callbackMethod: (rootMap, map, node) => void) {
            this._authorContextMenuCallbacks[contextMenuItemText] = callbackMethod;
            if (MappingToolController._mappingToolTypeIsSet) {
                switch (MappingToolController.getType()) {
                    case MappingToolType.Silverlight:
                        var silverlightMappingTool: SilverlightMappingTool = SilverlightMappingTool.getInstance();
                        if (silverlightMappingTool != null) {
                            silverlightMappingTool.AddAuthorContextMenuItem(contextMenuItemText);
                        }
                        break;
                    case MappingToolType.HTML5:
                        var html5MappingTool: Html5MappingTool = Html5MappingTool.getInstance();
                        if (html5MappingTool != null) {
                            html5MappingTool.AddAuthorContextMenuItem(contextMenuItemText);
                        }
                        break;
                }
            }
        }

        /*
         * This function adds a context menu item to the reader context menu and associates a JavaScript callback that will be called when it's clicked.
         * 
         * Params:
         *  * contextMenuItemText: The text to display in the context menu
         *  * callbackMethod: The method to call when the context menu item is clicked.
         */
        public AddReaderContextMenuItem(contextMenuItemText: string, callbackMethod: (rootMap, map, node) => void) {
            this._readerContextMenuCallbacks[contextMenuItemText] = callbackMethod;
            if (MappingToolController._mappingToolTypeIsSet) {
                switch (MappingToolController.getType()) {
                    case MappingToolType.Silverlight:
                        var silverlightMappingTool: SilverlightMappingTool = SilverlightMappingTool.getInstance();
                        if (silverlightMappingTool != null) {
                            silverlightMappingTool.AddReaderContextMenuItem(contextMenuItemText);
                        }
                        break;
                    case MappingToolType.HTML5:
                        var html5MappingTool: Html5MappingTool = Html5MappingTool.getInstance();
                        if (html5MappingTool != null) {
                            html5MappingTool.AddReaderContextMenuItem(contextMenuItemText);
                        }
                        break;
                }
            }
        }

        /*
         * This function invokes the custom author context menu item's callback that it was associated with.
         */
        public InvokeAuthorContextMenuCallback(contextMenuItemName: string, rootMap: string, map: string, node: string): void {
            try {
                this._authorContextMenuCallbacks[contextMenuItemName](rootMap, map, node);
            }
            catch (err) {
                //any errors that happen in their callbacks need to stop here
            }
        }

        /*
         * This function invokes the custom reader context menu item's callback that it was associated with.
         */
        public InvokeReaderContextMenuCallback(contextMenuItemName: string, rootMap: string, map: string, node: string): void {
            try {
                this._readerContextMenuCallbacks[contextMenuItemName](rootMap, map, node);
            }
            catch (err) {
                //any errors that happen in their callbacks need to stop here
            }
        } 

        /*
         * This function is invoked when the Silverlight Mapping Tool is loaded, it should also be called when the HTML5 Mapping Tool is loaded.
         * Currently only the Silverlight tool has a right click context menu so it hasn't been fully implemented for the HTML5 version.
         */
        public ProcessAddingContextMenuItems() {
            switch (MappingToolController.getType()) {
                case MappingToolType.Silverlight:
                    var silverlightMappingTool: SilverlightMappingTool = SilverlightMappingTool.getInstance();
                    if (silverlightMappingTool != null) {
                        $.each(this._authorContextMenuCallbacks, function (key, value) {
                            silverlightMappingTool.AddAuthorContextMenuItem(key);
                        });
                        $.each(this._readerContextMenuCallbacks, function (key, value) {
                            silverlightMappingTool.AddReaderContextMenuItem(key);
                        });
                    }
                    break;
                case MappingToolType.HTML5:
                    var html5MappingTool: Html5MappingTool = Html5MappingTool.getInstance();
                    if (html5MappingTool != null) {
                        $.each(this._authorContextMenuCallbacks, function (key, value) {
                            html5MappingTool.AddAuthorContextMenuItem(key);
                        });
                        $.each(this._readerContextMenuCallbacks, function (key, value) {
                            html5MappingTool.AddReaderContextMenuItem(key);
                        });
                    }
                    break;
            }
        }

    }
} 