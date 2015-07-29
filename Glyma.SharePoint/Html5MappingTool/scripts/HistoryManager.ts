module Glyma {
    export class HistoryManager {
        private _historyMaps: Array<string> = null;

        private _currentUrl: string = null;

        get historyMaps(): Array<string> {
            if (this._historyMaps == null) {
                this._historyMaps = new Array<string>();
            }
            return this._historyMaps;
        }

        constructor() {
        }

        public pushHistory(mapName: string, mapId: string, domainId: string, nodeId: string = null) {
            if (nodeId === "undefined") {
                nodeId = null;
            }
            var path = window.location.pathname;
            path = path + "?";
            if (nodeId != null && nodeId !== "undefined") {
                path = path + "NodeUid=" + nodeId + "&";
            }
            path = path + "MapUid=" + mapId + "&DomainUid=" + domainId;
            if (this.historyMaps.indexOf(path) < 0 && path != this._currentUrl) {
                if (this.historyMaps.length > 0) {
                    window.history.pushState({ domainId: domainId, mapId: mapId, nodeId: nodeId }, name, path);
                } else {
                    window.history.replaceState({domainId: domainId, mapId: mapId, nodeId: nodeId}, name);
                }
                this.historyMaps.push(path);
                this._currentUrl = path;
            }
        }
    }
} 