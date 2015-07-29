module Glyma {
    export class RTreeManager
    {
        private _rTree: RTree = null;
        private _arrowTree: RTree = null;
        private _nodeLength: number = 0;
        private _arrowLength: number = 0;

        private get rTree(): RTree {
            return this._rTree;
        }

        private get arrowTree(): RTree {
            return this._arrowTree;
        }

        private set rTree(value: RTree) {
            this._rTree = value;
        }

        private set arrowTree(value: RTree) {
            this._arrowTree = value;
        }

        constructor()
        {
            
        }

        public clear(): void {
            if (this._nodeLength != 0) {
                this.rTree = new RTree(this._nodeLength);
            } else {
                this.rTree = null;
            }
            
            if (this._arrowLength != 0) {
                this.arrowTree = new RTree(this._arrowLength);
            } else {
                this.arrowTree = null;
            }
        }

        public insertNode(x: number, y: number, w: number, h: number, object: any): void {
            if (this.rTree != null) {
                this.rTree.insert({ x: x, y: y, w: w, h: h }, object);
            }
        }

        public insertArrow(x: number, y: number, w: number, h: number, object: any): void {
            if (this.arrowTree != null) {
                if (w == 0) {
                    w = 1;
                }

                if (h == 0) {
                    h = 1;
                }
                this.arrowTree.insert({ x: x, y: y, w: w, h: h }, object);
            }
        }

        public searchNodes(x: number, y: number, w: number, h: number): any {
            if (this.rTree == null) {
                return [];
            }
            return this.rTree.search({ x: x, y: y, w: w, h: h });
        }

        public searchArrows(x: number, y: number, w: number, h: number): any {
            if (this.arrowTree == null) {
                return [];
            }
            return this.arrowTree.search({ x: x, y: y, w: w, h: h });
        }

        public reset(nodeNumber: number, arrowNumber: number) {
            this._nodeLength = nodeNumber;
            this._arrowLength = arrowNumber;
            this.clear();
        }
    }
}

declare class RTree {
    constructor(length: number);
    clear(): void;
    insert(area: any, object: any): void;
    search(area: any): any;
    remove(area: any);
    delete(area: any);
}