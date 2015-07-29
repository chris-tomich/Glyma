'use strict';
module Glyma {
    export class MapRenderer {
        private _mapData = null;
        private _nodes: Array<Node> = [];
        private _panX = 0;
        private _panY = 0;
        private _isDrag = false;
        private _arrowRenderer: ArrowRenderer = null;
        private _nodeRenderer: NodeRenderer = null;
        private _breadcrumbControl: BreadcrumbControl = null;
        private _rTreeManager: RTreeManager = null;
        private _sliderValue: number = 0

        private _topMost = 0;
        private _bottomMost = 0;
        private _leftMost = 0;
        private _rightMost = 0;

        private _rootMap: any = null;
        private _domainId: string = null;

        constructor() {

        }

        public get domainId(): string {
            return this._domainId;
        }

        public set domainId(value: string) {
            this._domainId = value;
        }

        get rTreeManager(): RTreeManager {
            if (this._rTreeManager == null) {
                this._rTreeManager = new RTreeManager();
            }
            return this._rTreeManager;
        }

        get arrowRendererInstance(): ArrowRenderer {
            if (this._arrowRenderer == null) {
                this._arrowRenderer = new ArrowRenderer(SharedVariables.canvas, SharedVariables.context);
            }

            return this._arrowRenderer;
        }

        get breadcrumbControl(): BreadcrumbControl {
            if (this._breadcrumbControl == null) {
                this._breadcrumbControl = new BreadcrumbControl(this._domainId);
            }

            return this._breadcrumbControl;
        }

        get nodeRendererInstance(): NodeRenderer {
            if (this._nodeRenderer == null) {
                this._nodeRenderer = new NodeRenderer(SharedVariables.context);
            }

            return this._nodeRenderer;
        }

        get nodes(): Array<Node> {
            return this._nodes;
        }

        get arrows(): Array<any> {
            return this._mapData.arrows;
        }

        get rootMap(): any {
            return this._rootMap;
        } 

        set nodes(value: Array<Node>) {
            this._nodes = value;
        }

        get rightMost(): number {
            return this._rightMost;
        }

        get leftMost(): number {
            return this._leftMost;
        }

        get bottomMost(): number {
            return this._bottomMost;
        }

        get topMost(): number {
            return this._topMost;
        }

        private _getSliderValue(scale: number): number {
            var value = scale;
            this._sliderValue = 0;
            if (scale > 1) {
                while (value > 1) {
                    value *= 0.9;
                    this._sliderValue++;
                }
            } else {
                while (value < 1) {
                    value /= 0.9;
                    this._sliderValue--;
                }
            }
            return this._sliderValue;
        }

        private _getScaleValue(value: number): number {
            if (value == 0) {
                return 1;
            } else if (value > 0) {
                return Math.min(Math.pow(0.9, -value), 10);
            } else {
                return Math.max(Math.pow(0.9, -value), 0.1);
            }
        }

        private _getPercentage(value: number): number {
            
            if (value == 1) {
                return 50;
            } else {
                var slider = this._getSliderValue(value);
                return 50 + 50 / 22 * slider;
            }
        }

        public scaleBySlider(value: number, changeSlider = false) {
            SharedVariables.mouseX = $('#pinchzoom').width() / 2;
            SharedVariables.mouseY = $('#pinchzoom').height() / 2;

            SharedVariables.lastMouseX = SharedVariables.mouseX;
            SharedVariables.mouseY = SharedVariables.mouseY;

            this.scaleMap(this._getScaleValue(value), changeSlider);
            this.refresh();
            this._sliderValue = value;
            //$("#slider .ui-slider-range").css("height", value + "%");
            //$("#slider .ui-slider-handle").css("bottom", value + "%");
        }


        public zoomIn() {
            this._sliderValue = Math.min(22, this._sliderValue + 1);
            this.scaleBySlider(this._sliderValue, true);
        }

        public zoomOut() {
            this._sliderValue = Math.max(-22, this._sliderValue - 1);
            this.scaleBySlider(this._sliderValue, true);
        }

        public defaultZoom() {
            this._sliderValue = 0;
            this.scaleBySlider(this._sliderValue, true);
        }

        public scaleMap(scale: number, changeSlider:boolean = true) {
            var oldScale = SharedVariables.scale;
            var ratio = (scale / oldScale - 1);
            var offsetX = SharedVariables.mouseX * ratio / scale;
            var offsetY = SharedVariables.mouseY * ratio / scale;

            SharedVariables.posX -= offsetX;
            SharedVariables.posY -= offsetY;

            SharedVariables.lastPosX = SharedVariables.posX;
            SharedVariables.lastPosY = SharedVariables.posY;

            SharedVariables.scale = scale;

            if (changeSlider) {
                $("#slider .ui-slider-range").css("height", this._getPercentage(scale) + "%");
                $("#slider .ui-slider-handle").css("bottom", this._getPercentage(scale) + "%");
            }
        }

        public reRenderNode(i: number) {
            if (this._mapData) {
                SharedVariables.context.save();
                if (this._nodes[i].isVisible) {
                    this.nodeRendererInstance.redrawNodeImage(this._nodes[i], SharedVariables.posX, SharedVariables.posY);
                }
                SharedVariables.context.restore();
            }
        }

        public resetMap() {
            SharedVariables.scale = 1;
            SharedVariables.lastScale = 1;
            SharedVariables.posX = 0;
            SharedVariables.posY = 0;
            SharedVariables.lastPosX = 0;
            SharedVariables.lastPosY = 0;
        }

        public createBreadcrumb() {
            if (this._mapData.breadcrumbs.length > 0) {
                var originLength = this.breadcrumbControl.breadcrumbs.length;

                if (originLength > 0) {
                    var currentBreadcrumb = this._mapData.breadcrumbs[this._mapData.breadcrumbs.length - 1];
                    var index = this.breadcrumbControl.getIndexByNodeId(currentBreadcrumb.uniqueId);
                    if (index == -1) {
                        this.breadcrumbControl.addBreadcrumb(currentBreadcrumb);
                    } else {
                        this.breadcrumbControl.removeToIndex(index);
                    }
                } else {
                    for (var j = 0; j < this._mapData.breadcrumbs.length; j++) {
                        this.breadcrumbControl.addBreadcrumb(this._mapData.breadcrumbs[j]);
                    }
                }
                this.breadcrumbControl.refreshBreadcrumb();
            }
        }

        public refresh() {
            if (this._mapData) {
                SharedVariables.context.clearRect(0, 0, SharedVariables.canvas.width, SharedVariables.canvas.height);

                SharedVariables.context.save();
                SharedVariables.context.scale(SharedVariables.scale, SharedVariables.scale);
                SharedVariables.context.translate(SharedVariables.posX, SharedVariables.posY);
                for (var i = 0; i < this._nodes.length; i++) {
                    if (this._nodes[i].isVisible) {
                        var child = this._nodes[i].childIndexes;
                        var hasVisibleChild = false;
                        var hasHiddenChild = false;
                        var hasChild = child.length > 0;
                        for (var m = 0; m < child.length; m++) {
                            if (this._nodes[child[m]].isVisible) {
                                hasVisibleChild = true;
                            } else {
                                hasHiddenChild = true;
                            }
                        }

                        if (!hasChild) {
                            this._nodes[i].collpaseState = "none";
                        } else {
                            if (hasVisibleChild && !hasHiddenChild) {
                                this._nodes[i].collpaseState = "expanded";
                            } else if (hasVisibleChild && hasHiddenChild) {
                                this._nodes[i].collpaseState = "semicollapsed";
                            } else {
                                this._nodes[i].collpaseState = "collapsed";
                            }
                        }
                    } 
                }

                var visibleArrows = this.rTreeManager.searchArrows(-SharedVariables.posX, -SharedVariables.posY, SharedVariables.canvas.width / SharedVariables.scale, SharedVariables.canvas.height / SharedVariables.scale);

                for (var n = 0; n < visibleArrows.length; n++) {
                    var arrow = this._mapData.arrows[visibleArrows[n]];
                    if (arrow != null && arrow !== "undefined") {
                        if (this._nodes[arrow.from].isVisible && this._nodes[arrow.to].isVisible) {
                            this.arrowRendererInstance.drawArrow(this._nodes[arrow.from], this._nodes[arrow.to]);
                        }
                    }
                }

                var visibleNodes = this.rTreeManager.searchNodes(-SharedVariables.posX, -SharedVariables.posY, SharedVariables.canvas.width / SharedVariables.scale, SharedVariables.canvas.height / SharedVariables.scale);

                for (var k = 0, l = visibleNodes.length; k < l; k++) {
                    if (this.nodes[visibleNodes[k]].isVisible) {
                        this.nodeRendererInstance.drawNode(this.nodes[visibleNodes[k]]);
                    }
                }
                SharedVariables.context.restore();
            }
        }

        public  calculateBorder() {
            if (this._nodes.length > 0) {
                this._leftMost = this._nodes[0].xPosition;
                this._rightMost = this._nodes[0].xPosition;
                this._topMost = this._nodes[0].yPosition;
                this._bottomMost = this._nodes[0].yPosition;

                if (this._nodes.length > 1) {
                    for (var i = 1, l = this._nodes.length; i < l; i++) {
                        if (this._nodes[i].xPosition < this._leftMost) {
                            this._leftMost = this._nodes[i].xPosition;
                        }

                        if (this._nodes[i].yPosition < this._topMost) {
                            this._topMost = this._nodes[i].yPosition;
                        }

                        if (this._nodes[i].xPosition > this._rightMost) {
                            this._rightMost = this._nodes[i].xPosition;
                        }

                        if (this._nodes[i].yPosition > this._bottomMost) {
                            this._bottomMost = this._nodes[i].yPosition;
                        }
                    }
                }

            }
        }

        public drawMap(data: any) {
            this._mapData = data;
            if (this._rootMap == null) {
                this._rootMap = data.rootMap;
            }
            this._nodes = [];
            this.createBreadcrumb();
            this.rTreeManager.reset(this._mapData.nodes.length, this._mapData.arrows.length);
            for (var i = 0; i < this._mapData.nodes.length; i++) {
                var item = this._mapData.nodes[i];
                var node = new Node(item,i);
                this._nodes.push(node);
            }

            for (var j = 0; j < this._mapData.arrows.length; j++) {
                var arrow = this._mapData.arrows[j];
                this._nodes[arrow.to].childIndexes.push(arrow.from);
                if (this._nodes[arrow.from].parentIndexes.length == 0) {
                    this._nodes[arrow.from].parentIndexes.push(arrow.to);
                }
            }


            this.checkIncorrectVisibility();
        }

        public checkIncorrectVisibility(): void {
            var needToCheck = true;
            while (needToCheck) {
                needToCheck = false;
                for (var l = 0; l < this.nodes.length; l++) {
                    var needBreak = false;
                    if (!this.nodes[l].isVisible) {
                        for (var m = 0; m < this.nodes[l].childIndexes.length; m++) {
                            if (this.nodes[this.nodes[l].childIndexes[m]].isVisible) {
                                this.nodes[l].isVisible = true;
                                needToCheck = true;
                                needBreak = true;
                                break;
                            }
                        }
                    }

                    if (needBreak) {
                        break;
                    }
                }
            }
        }
    }
}
