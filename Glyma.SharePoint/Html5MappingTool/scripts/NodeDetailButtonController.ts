module Glyma {
    export class NodeDetailButtonController {
        private _node: any;

        private _defaultXs : number[];
        private _defaultYs : number[];

        private _container: any;

        private _canvas: any;

        constructor() {
            this._defaultXs = new Array();
            this._defaultYs = new Array();
            this._defaultXs.push(136);
            this._defaultXs.push(126);
            this._defaultXs.push(104);
            this._defaultXs.push(73);
            this._defaultXs.push(36);
            this._defaultXs.push(0);
            this._defaultYs.push(98);
            this._defaultYs.push(62);
            this._defaultYs.push(31);
            this._defaultYs.push(10);
            this._defaultYs.push(0);
            this._defaultYs.push(10);

            this._container = $("#nodeDetailButtonContainer");

            this.initialEvents();
        }

        public showButtons(node: Node, index: number, x: number, y: number) {
            this._node = node;
            var count = 0;
            if (this._node._hasVideo) {
                this.addButton("video", count, index, node.nodeId);
                count++;
            }

            if (this._node._hasContent) {
                this.addButton("content", count, index, node.nodeId);
                count++;
            }

            if (this._node._hasFeed) {
                this.addButton("feed", count, index, node.nodeId);
                count++;
            }

            if (this._node._hasMap) {
                this.addButton("map", count, index, node.nodeId);
                count++;
            }

            if (this._node._hasLocation) {
                this.addButton("location", count, index, node.nodeId);
                count++;
            }

            if (count > 1) {
                this.addButton("showall", count, index, node.nodeId);
                count++;
            }
            this._container.css("left", x);
            this._container.css("top", y);

            //if there is only 1 item don't show the node details button arc
            if (count != 1) {
                this._container.show();
            }
            else {
                this._container.hide();
            }
        }

        private addButton(type: string, num: number, index: number, nodeId:string):void {
            var button = $("#nodedetail" + type).show();
            button.css("margin-left", this._defaultXs[num] + "px");
            button.css("margin-top", this._defaultYs[num] + "px");
            $("#nodedetail" + type).attr("index", index);
            $("#nodedetail" + type).attr("nodeId", nodeId);
        }

        public hideButtons() {
            $("#nodeDetailButtonContainer img").hide();
            this._container.hide();
        }

        private initialEvents() {

        }
    }
}   