module Glyma {
    export class NodeDetails {
        private _node: any;

        private _defaultLocations: Point[];

        private _detailButtons: NodeDetailButton[];

        private _container: any;

        constructor() {
            this._defaultLocations = new Point[6];
            this._defaultLocations.push(new Point(97, 70));
            this._defaultLocations.push(new Point(90, 44));
            this._defaultLocations.push(new Point(74, 22));
            this._defaultLocations.push(new Point(52, 7));
            this._defaultLocations.push(new Point(26, 0));
            this._defaultLocations.push(new Point(0, 7));
            this._container = $("#nodeDetailButtonContainer");

            this.initialEvents();
        }

        public showButtons(node: Node) {
            this._node = node;

            var count = 0;
            if (this._node._hasVideo) {
                this.addButton("video", count);
                count++;
            }

            if (this._node._hasContent) {
                this.addButton("content", count);
                count++;
            }

            if (this._node._hasFeed) {
                this.addButton("feed", count);
                count++;
            }

            if (this._node._hasMap) {
                this.addButton("map", count);
                count++;
            }

            if (this._node._hasLocation) {
                this.addButton("location", count);
                count++;
            }

            if (count > 1) {
                this.addButton("showall", count);
                count++;
            }
        }

        private addButton(type: string, num: number) {
            var button = $("#nodedetail" + type);
            this._container.clear();
            this._container.append(button);
            button.css("top", this._defaultLocations[num].y);
            button.css("top", this._defaultLocations[num].x);
        }


        private initialEvents() {

        }
    }
}  