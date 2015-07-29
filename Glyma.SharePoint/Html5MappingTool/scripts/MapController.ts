'use strict';
module Glyma {
    export class MapController {
        private _nodeDetails: NodeDetailButtonController;
        private _mapRenderer: MapRenderer = null;
        private _breadcrumbControl: BreadcrumbControl = null;
        private _rTreeManager: RTreeManager = null;
        private _realignController: RealignController = null;
        private _historyManager: HistoryManager = null;

        private _mapId: string = null;
        private _isRootMapLoaded: boolean = false;

        private _isLeftbarExpended: boolean = true;

        public get mapRenderer(): MapRenderer {
            if (this._mapRenderer == null) {
                this._mapRenderer = new MapRenderer();
            }
            return this._mapRenderer;
        }

        public get nodes(): Array<Node> {
            return this.mapRenderer.nodes;
        }

        private get realignController(): RealignController {
            if (this._realignController == null) {
                this._realignController = new RealignController(this);
            }
            return this._realignController;
        }

        get arrows(): Array<any> {
            return this.mapRenderer.arrows;
        }

        public get domainId(): string {
            return this.mapRenderer.domainId;
        }

        get rTreeManager(): RTreeManager {
            return this.mapRenderer.rTreeManager;
        }

        get breadcrumbControl(): BreadcrumbControl {
            if (this._breadcrumbControl == null) {
                this._breadcrumbControl = new BreadcrumbControl(this.domainId);
            }

            return this._breadcrumbControl;
        }

        get visibleNodeIndexes(): Array<number> {
            var output = new Array<number>();
            for (var i = 0; i < this.nodes.length; i++) {
                if (this.nodes[i].isVisible) {
                    output.push(i);
                }
            }
            return output;
        }

        get historyManager(): HistoryManager {
            if (this._historyManager == null) {
                this._historyManager = new HistoryManager();
            }
            return this._historyManager;
        }

        get nodeDetails(): NodeDetailButtonController {
            if (this._nodeDetails == null) {
                this._nodeDetails = new NodeDetailButtonController();
            }
            return this._nodeDetails;
        }

        get isLeftbarExpended(): boolean {
            return this._isLeftbarExpended;
        }

        set isLeftbarExpended(value: boolean) {
            this._isLeftbarExpended = value;
        }

        constructor() {
            SharedVariables.canvas = <HTMLCanvasElement>document.getElementById('mapCanvas');
            SharedVariables.pinchzoom = document.getElementById('pinchzoom');
            SharedVariables.context = SharedVariables.canvas.getContext("2d");
            this.initialise();
            SharedVariables.touchManager = new TouchManager();
        }

        public clearRTree() {
            this.rTreeManager.clear();
        }

        public hideNodeDetailButtons() {
            this.nodeDetails.hideButtons();
        }

        public refreshCurrentMap() {
            if (this.domainId != null && this._mapId != null) {
                this.getMapData(this.domainId, this._mapId);
            }
        }

        public mouseMoved(x: number, y: number) {
            var relativeX = x / SharedVariables.scale - SharedVariables.posX;
            var relativeY = y / SharedVariables.scale - SharedVariables.posY;
            var visibleNodeIndexes = this.rTreeManager.searchNodes(relativeX - 20, relativeY - 20, 40, 40 );

            var hovered = false;
            for (var i = 0; i < visibleNodeIndexes.length; i++) {
                var node = this.nodes[visibleNodeIndexes[i]];
                node.isHoverCornerButton = false;
                node.isHoverCornerExtendButton = false;
                node.isHoverCollapseButton = false;
                //this.NodeRendererInstance.cornerButton(node, Common.Constants.posX, Common.Constants.posY, false);
                //this.NodeRendererInstance.cornerExtendButton(node, Common.Constants.posX, Common.Constants.posY, false);
                if (SharedVariables.canvas.style) {
                    if (node.hasLink
                        && relativeX >= node.left
                        && relativeY >= node.top + Common.Constants.imageSize
                        && relativeX <= node.left + node.TextCanvas.maxWidth / node.nodeTextBox.scale
                        && relativeY <= node.top + Common.Constants.imageSize + node.TextCanvas.height / node.nodeTextBox.scale) {
                        SharedVariables.canvas.style.cursor = "pointer";
                        hovered = true;
                    } else if (node.nodeCornerButton.hasExtendButton
                        && relativeX >= node.cornerButtonLeft + Common.Constants.cornerButtonWidth
                        && relativeY >= node.cornerButtonTop
                        && relativeX <= node.cornerButtonLeft + Common.Constants.cornerButtonWidth + Common.Constants.extendCornerButtonWidth
                        && relativeY <= node.cornerButtonTop + Common.Constants.cornerButtonHeight) {
                        SharedVariables.canvas.style.cursor = "pointer";
                        node.isHoverCornerExtendButton = true;
                        hovered = true;
                    } else if (node.nodeCornerButton.hasCornerButton
                        && relativeX >= node.cornerButtonLeft
                        && relativeY >= node.cornerButtonTop
                        && relativeX <= node.cornerButtonLeft + Common.Constants.cornerButtonWidth
                        && relativeY <= node.cornerButtonTop + Common.Constants.cornerButtonHeight) {
                        SharedVariables.canvas.style.cursor = "pointer";
                        node.isHoverCornerButton = true;
                        hovered = true;
                    } else if (node.collpaseState != "none"
                        && relativeX >= node.collapseButtonLeft
                        && relativeY >= node.collapseButtonTop
                        && relativeX <= node.collapseButtonLeft + Common.Constants.collapseImageSize
                        && relativeY <= node.collapseButtonTop + Common.Constants.collapseImageSize) {
                        node.isHoverCollapseButton = true;
                        SharedVariables.canvas.style.cursor = "pointer";
                        hovered = true;
                    } else if (node.nodeType == "Map"
                        && relativeX <= node.imageLeft + Common.Constants.imageSize
                        && relativeX >= node.imageLeft
                        && relativeY >= node.imageTop
                        && relativeY <= node.imageTop + Common.Constants.imageSize) {
                        SharedVariables.canvas.style.cursor = "pointer";
                        hovered = true;
                    }
                }
            }

            if (!hovered) {
                SharedVariables.canvas.style.cursor = "auto";
            }

            if (visibleNodeIndexes.length > 0) {
                this.refresh();
            }
        }

        public clicked(x: number, y: number, type: string = "single") {
            this.nodeDetails.hideButtons();
            var relativeX = x / SharedVariables.scale - SharedVariables.posX;
            var relativeY = y / SharedVariables.scale - SharedVariables.posY;

            var visibleNodeIndexes = this.rTreeManager.searchNodes(relativeX - 10, relativeY - 10, 20, 20);
            var allVisibleNodes = this.visibleNodeIndexes;
            for (var k = 0; k < allVisibleNodes.length; k++) {
                if (this.nodes[allVisibleNodes[k]].isSelected) {
                    this.nodes[allVisibleNodes[k]].deselect();
                }
            }


            for (var i = 0; i < visibleNodeIndexes.length; i++) {
                if (relativeX >= this.nodes[visibleNodeIndexes[i]].collapseButtonLeft
                    && relativeX <= this.nodes[visibleNodeIndexes[i]].collapseButtonLeft + Common.Constants.collapseImageSize
                    && relativeY >= this.nodes[visibleNodeIndexes[i]].collapseButtonTop
                    && relativeY <= this.nodes[visibleNodeIndexes[i]].collapseButtonTop + Common.Constants.collapseImageSize) {

                    var xPos = this.nodes[visibleNodeIndexes[i]].xPosition;
                    var yPos = this.nodes[visibleNodeIndexes[i]].yPosition;

                    if (this.nodes[visibleNodeIndexes[i]].collpaseState == "collapsed" || this.nodes[visibleNodeIndexes[i]].collpaseState == "semicollapsed") {
                        if (type == "single") {
                            this.expandNodes(visibleNodeIndexes[i]);
                        } else {
                            this.expandAllNodes(visibleNodeIndexes[i]);
                        }
                    } else if (this.nodes[visibleNodeIndexes[i]].collpaseState != "none") {
                        if (type == "single") {
                            this.collapseNode(visibleNodeIndexes[i]);
                        } else {
                            this.expandAllNodes(visibleNodeIndexes[i]);
                        }
                    }
                    this.realignController.horizontalRealign();

                    var newXPos = this.nodes[visibleNodeIndexes[i]].xPosition;
                    var newYPos = this.nodes[visibleNodeIndexes[i]].yPosition;
                    SharedVariables.posX += xPos - newXPos;
                    SharedVariables.posY += yPos - newYPos;
                    SharedVariables.lastPosX = SharedVariables.posX;
                    SharedVariables.lastPosY = SharedVariables.posY;
                    this.nodes[visibleNodeIndexes[i]].select();
                    this.refresh();
                    return;
                } else if (relativeX >= this.nodes[visibleNodeIndexes[i]].cornerButtonLeft
                    && relativeX <= this.nodes[visibleNodeIndexes[i]].cornerButtonLeft + Common.Constants.cornerButtonWidth + Common.Constants.extendCornerButtonWidth
                    && relativeY >= this.nodes[visibleNodeIndexes[i]].cornerButtonTop
                    && relativeY <= this.nodes[visibleNodeIndexes[i]].cornerButtonTop + Common.Constants.cornerButtonHeight) {
                    this.nodes[visibleNodeIndexes[i]].select();
                    if (relativeX <= this.nodes[visibleNodeIndexes[i]].cornerButtonLeft + Common.Constants.cornerButtonWidth) {
                        this.nodes[visibleNodeIndexes[i]].cornerButtonClicked();
                    } else {
                        //TODO: Remove the #s4-workspace hardcoding - it's measuring the amount scrolled but only works with this masterpage
                        var scrollTop: number = $("#s4-workspace")[0].scrollTop;
                        this.nodeDetails.showButtons(this.nodes[visibleNodeIndexes[i]], visibleNodeIndexes[i], x - 98, y - 98 + scrollTop);
                    }
                    this.refresh();
                    return;
                } else if (relativeX <= this.nodes[visibleNodeIndexes[i]].imageLeft + Common.Constants.imageSize
                    && relativeX >= this.nodes[visibleNodeIndexes[i]].imageLeft
                    && relativeY >= this.nodes[visibleNodeIndexes[i]].imageTop
                    && relativeY <= this.nodes[visibleNodeIndexes[i]].imageTop + Common.Constants.imageSize) {
                    if (type == "single") {
                        this.nodes[visibleNodeIndexes[i]].select();
                        //TODO: Remove the #s4-workspace hardcoding - it's measuring the amount scrolled but only works with this masterpage
                        var scrollTop2: number = $("#s4-workspace")[0].scrollTop;
                        this.nodeDetails.showButtons(this.nodes[visibleNodeIndexes[i]], visibleNodeIndexes[i], x - 98, y - 98 + scrollTop2);
                        this.refresh();
                    } else {
                        if (this.nodes[visibleNodeIndexes[i]].nodeType == "Map") {
                            this.getMapData(this.domainId, this.nodes[visibleNodeIndexes[i]].nodeId);
                        }
                    }
                    return;
                } else if (this.nodes[visibleNodeIndexes[i]].hasLink
                    && relativeX >= this.nodes[visibleNodeIndexes[i]].left
                    && relativeY >= this.nodes[visibleNodeIndexes[i]].top + Common.Constants.imageSize
                    && relativeX <= this.nodes[visibleNodeIndexes[i]].left + this.nodes[visibleNodeIndexes[i]].TextCanvas.maxWidth / this.nodes[visibleNodeIndexes[i]].nodeTextBox.scale
                    && relativeY <= this.nodes[visibleNodeIndexes[i]].top + Common.Constants.imageSize + this.nodes[visibleNodeIndexes[i]].TextCanvas.height / this.nodes[visibleNodeIndexes[i]].nodeTextBox.scale) {
                    if (type == "single") {
                        openInNewTab(this.nodes[visibleNodeIndexes[i]].link);
                    }
                    return;
                }
            }
        }

        public getMapData(domainId: string, mapId: string, nodeId: string = null, isPushState: boolean = true): any {
            if (!$("#loader").is(":visible")) {
                showLoad();
            }
            this.nodeDetails.hideButtons();
            this.mapRenderer.resetMap();
            var url = getServerRelativeVersionedLayoutsFolder() + "/Glyma/HttpHandlers/ViewMap.ashx";
            var site = encodeURI(getCurrentSiteUrl());
            this.mapRenderer.domainId = domainId;
            this._mapId = mapId;
            $.ajax({
                type: "GET",
                url: url,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: {
                    'DomainId': domainId,
                    'MapId': mapId,
                    'site': site
                },
                responseType: "json",
                success: result => {
                    if (isPushState) {
                        //this.historyManager.pushHistory("", mapId, domainId, nodeId);
                    }
                    this.mapRenderer.drawMap(result);
                    this.realignController.horizontalRealign();
                    this.mapRenderer.calculateBorder();
                    if (nodeId != null) {
                        this.centraliseNode(nodeId);
                    } else {
                        this.centraliseMostImportantNode();
                    }
                    this.refresh();
                    $("#loader").fadeOut("slow");

                    if (typeof (Glyma.RelatedContentPanels.RelatedContentController) === "function") {
                        var controller = Glyma.RelatedContentPanels.RelatedContentController.getInstance();
                        if (controller != null) {
                            controller.mapLoadCompleted(this._isRootMapLoaded);
                        }
                    }
                    if (!this._isRootMapLoaded) {
                        this._isRootMapLoaded = true;
                    }
                },
                error: function (result) {
                    showError("We are unable to find your map, please try again later.");
                }
            });
        }

        public getNodeIndexById(nodeId: string): number {
            for (var i = 0; i < this.nodes.length; i++) {
                if (this.nodes[i].nodeId == nodeId) {
                    return i;
                }
            }
            return -1;
        }

        public loadMaps(domainId: string): void {
            $("#maps").html('');
            if (domainId !== "undefined" && domainId != "" && domainId.length > 0) {
                var maps = [{ mapName: "Map 1", mapId: "asdfasfj-asdqwe-asdqweas-123123", domainId: domainId }, { mapName: "Map 2", mapId: "asdfasfj-asdqwe-asdqweas-1231232", domainId: domainId }];
                for (var i = 0; i < maps.length; i++) {
                    $("#maps").append('<option domainId="' + maps[i].domainId + '" value="' + maps[i].mapId + '">' + maps[i].mapName + '</option>');
                }
            }
        }

        public loadDomains(): void {
            $("#domains").html('');
            $("#maps").html('');
            var domains = [{ domainName: "Domain 1", domainId: "asdfasfj-asdqwe-asdqweas-dddddd1" }, { domainName: "Domain 2", domainId: "asdfasfj-asdqwe-asdqweas-dddddd2" }];
            for (var i = 0; i < domains.length; i++) {
                $("#domains").append('<option value="' + domains[i].domainId + '">' + domains[i].domainName+'</option>');
            }
        }

        public centraliseNode(nodeId: string = null) {
            if (nodeId != null) {
                var index = this.getNodeIndexById(nodeId);
                if (index >= 0) {
                    this._centraliseNodeByIndex(index, true);
                    return;
                }
            }
            this.centraliseMostImportantNode();
        }

        public centraliseMostImportantNode() {
            var parent = new Array<number>(), mostImportantNodeIndex = -1;
            for (var i = 0; i < this.nodes.length; i++) {
                if (this.nodes[i].rootDepth == 0 && this.nodes[i].isVisible) {
                    parent.push(i);
                }
            }

            if (parent.length > 0) {
                for (var j = 0; j < parent.length; j++) {
                    if (j == 0) {
                        mostImportantNodeIndex = parent[j];
                    } else if (this.nodes[mostImportantNodeIndex].childIndexes.length < this.nodes[parent[j]].childIndexes.length) {
                        mostImportantNodeIndex = parent[j];
                    }
                }
            }
            this._centraliseNodeByIndex(mostImportantNodeIndex, false);
        }

        private _centraliseNodeByIndex(index: number, selectNode: boolean = false) {
            if (index >= 0) {
                if (!this.nodes[index].isVisible) {
                    this._bringNodeVisible(index);
                }

                if (this.mapRenderer.rightMost - this.mapRenderer.leftMost + Common.Constants.realignNodeWidth > SharedVariables.canvas.width) {
                    if (this.nodes[index].xPosition > SharedVariables.canvas.width / 2) {
                        if (this.nodes[index].xPosition > this.mapRenderer.rightMost + Common.Constants.realignNodeWidth / 2 - SharedVariables.canvas.width / 2) {
                            SharedVariables.posX = SharedVariables.canvas.width - this.mapRenderer.rightMost - Common.Constants.realignNodeWidth / 2;
                        } else {
                            SharedVariables.posX = SharedVariables.canvas.width / 2 - this.nodes[index].xPosition;
                        }
                    }
                }

                if (this.mapRenderer.bottomMost - this.mapRenderer.topMost + 200 > SharedVariables.canvas.height) {
                    if (this.nodes[index].yPosition > SharedVariables.canvas.height / 2) {
                        if (this.nodes[index].yPosition > this.mapRenderer.bottomMost + 100 - SharedVariables.canvas.height / 2) {
                            SharedVariables.posY = SharedVariables.canvas.height - this.mapRenderer.bottomMost - 100;
                        } else {
                            SharedVariables.posY = SharedVariables.canvas.height / 2 - this.nodes[index].yPosition;
                        }
                    }
                } else {
                    SharedVariables.posY = (this.mapRenderer.topMost + SharedVariables.canvas.height - this.mapRenderer.bottomMost) / 2;
                }
                if (selectNode) {
                    this.nodes[index].select();
                }
                SharedVariables.lastPosX = SharedVariables.posX;
                SharedVariables.lastPosY = SharedVariables.posY;
            }
        }

        private _bringNodeVisible(index: number): void {
            this.nodes[index].isVisible = true;
            this.mapRenderer.checkIncorrectVisibility();
            this.realignController.horizontalRealign();
        }

        public getAllChildIndexes(index: number): Array<number> {
            var nodeToLoop = this.nodes[index].childIndexes.slice(0);
            var output = nodeToLoop.slice(0);
            //output.push();




            while (nodeToLoop.length > 0) {
                var childrenFound = [];
                for (var i = 0; i < nodeToLoop.length; i++) {
                    var childIndex = nodeToLoop[i];


                    var found = this.nodes[childIndex].childIndexes;
                    for (var j = 0; j < found.length; j++) {
                        if (output.indexOf(found[j]) < 0 && found[j] != index) {
                            output.push(found[j]);
                            childrenFound.push(found[j]);
                        }
                    }
                }
                nodeToLoop = childrenFound;
            }

            return output;
        }

        public collapseNode(index: number) {
            var children = this.getAllChildIndexes(index);

            for (var j = 0; j < children.length; j++) {
                this.nodes[children[j]].isVisible = false;
            }
            this.nodes[index].collpaseState = "collapsed";


        }

        public expandNodes(index: number) {
            var children = this.nodes[index].childIndexes;
            for (var i = 0; i < children.length; i++) {
                this.nodes[children[i]].isVisible = true;
            }
            this.nodes[index].collpaseState = "expanded";
        }

        public expandAllNodes(index: number) {
            var children = this.getAllChildIndexes(index);
            for (var i = 0; i < children.length; i++) {
                this.nodes[children[i]].isVisible = true;
            }
            this.nodes[index].collpaseState = "expanded";
        }

        public refresh(): void {
            this.mapRenderer.refresh();
        }

        public initialise(): void {
            $(window).bind('resize', function () {
                SharedVariables.canvas.height = Glyma.SharedVariables.getMapHeight();
                SharedVariables.canvas.width = Glyma.SharedVariables.getMapWidth();
                SharedVariables.mapController.refresh();
            });

            $(document).on("mobileinit", function () {
                $.mobile.loader.prototype.options.disabled = true;
            });

            //TODO: Remove the #s4-workspace hardcoding if possible as this only works with this master page layout.
            $("#s4-workspace").scroll(function () {
                SharedVariables.mapController.hideNodeDetailButtons();
            });

            $('#pinchzoom').bind('mousewheel', function (event) {
                event.preventDefault();
                SharedVariables.mapController.nodeDetails.hideButtons();
                if (event.ctrlKey) {
                    SharedVariables.lastScale = SharedVariables.scale;
                    SharedVariables.mapController.mapRenderer.scaleMap(Math.max(0.1, Math.min(SharedVariables.lastScale / Math.pow(0.9, event.deltaY), 10)));
                    SharedVariables.lastScale = SharedVariables.scale;
                    SharedVariables.mapController.refresh();
                } else {
                    SharedVariables.lastPosY = SharedVariables.lastPosY + (event.deltaY * event.deltaFactor / SharedVariables.scale);
                    SharedVariables.posY = SharedVariables.lastPosY;
                    SharedVariables.mapController.refresh();
                }

            });

            $("#zoom-in").click(function () {
                SharedVariables.mapController.mapRenderer.zoomIn();
            });

            $("#breadcrumb-home").click(function () {
                mapSelection();
            });

            $("#zoom-out").click(function () {
                SharedVariables.mapController.mapRenderer.zoomOut();
            });

            $("#zoom-default").click(function () {
                SharedVariables.mapController.mapRenderer.defaultZoom();
            });

            $("#zoom-view-reset").click(function () {
                SharedVariables.posX = 0;
                SharedVariables.posY = 0;
                SharedVariables.lastPosX = 0;
                SharedVariables.lastPosY = 0;
                SharedVariables.mapController.mapRenderer.defaultZoom();
                $("#slider .ui-slider-range").css("height", "50%");
                $("#slider .ui-slider-handle").css("bottom", "50%");
                SharedVariables.mapController.centraliseNode();
                SharedVariables.mapController.refresh();
            });

            $("#left-expender").click(function () {
                if (SharedVariables.mapController.isLeftbarExpended) {
                    $("#left-sidebar").hide();
                    SharedVariables.mapController.isLeftbarExpended = false;
                } else {
                    $("#left-sidebar").show();
                    SharedVariables.mapController.isLeftbarExpended = true;
                }
            });

            $("#sidebar-refresh").click(function () {
                SharedVariables.mapController.refreshCurrentMap();
                if (typeof (Glyma.RelatedContentPanels.RelatedContentController) === "function") {
                    var controller = Glyma.RelatedContentPanels.RelatedContentController.getInstance();
                    if (controller != null) {
                        controller.resetAndHidePanels();
                    }
                }
            });

            $("#sidebar-fullscreen").click(function () {
                var changed = false;
                if (document.body.requestFullscreen) {
                    changed = true;
                    document.body.requestFullscreen();
                } else if (document.body.msRequestFullscreen) {
                    changed = true;
                    document.body.msRequestFullscreen();
                } else if (document.body.mozRequestFullScreen) {
                    changed = true;
                    document.body.mozRequestFullScreen();
                } else if (document.body.webkitRequestFullscreen) {
                    changed = true;
                    document.body.webkitRequestFullscreen();
                } else {
                    alert("Your browser does not support full screen mode.");
                }

                if (changed) {
                    Common.Constants.isFullScreen = true;
                    $("#sidebar-fullscreen").hide();
                    $("#sidebar-exitfullscreen").show();
                }
            });

            $("#sidebar-exitfullscreen").click(function () {
                if (Common.Constants.isFullScreen) {
                    var changed = false;
                    if (document.exitFullscreen) {
                        document.exitFullscreen();
                        changed = true;
                    } else if (document.mozCancelFullScreen) {
                        document.mozCancelFullScreen();
                        changed = true;
                    } else if (document.msCancelFullScreen) {
                        document.msCancelFullScreen();
                        changed = true;
                    } else if (document.webkitExitFullscreen) {
                        document.webkitExitFullscreen();
                        changed = true;
                    }

                    if (changed) {
                        Common.Constants.isFullScreen = false;
                        $("#sidebar-fullscreen").show();
                        $("#sidebar-exitfullscreen").hide();
                    }
                }
            });

            $("#sidebar-realign").click(function () {
                SharedVariables.mapController.realignController.horizontalRealign();
            });

            $("#nodedetailvideo").click(function () {
                var index = parseInt($(this).attr("index"));
                NodeController.playVideo(index);
            });

            $("#nodedetailmap").click(function () {
                var index = parseInt($(this).attr("index"));
                NodeController.showRelatedMaps(SharedVariables.mapController.nodes[index].relatedMaps);
            });

            $("#nodedetaillocate").click(function () {

            });

            $("#nodedetailfeed").click(function () {

            });

            $("#nodedetailcontent").click(function () {
                var index = parseInt($(this).attr("index"));
                NodeController.showContent(index);
            });

            $("#nodedetailshowall").click(function () {
                var index = parseInt($(this).attr("index"));
                NodeController.showContent(index);
                NodeController.showRelatedMaps(SharedVariables.mapController.nodes[index].relatedMaps);
                NodeController.playVideo(index);
            });

            Glyma.SharedVariables.canvas.height = Glyma.SharedVariables.getMapHeight();
            Glyma.SharedVariables.canvas.width = Glyma.SharedVariables.getMapWidth();
        }
    }
} 

declare function getCurrentSiteUrl(): string;
declare function getBaseUrl(): string;
declare function getServerRelativeVersionedLayoutsFolder(): string;
declare function showError(msg: string): void;
declare function openInNewTab(url: string): void;
declare function showLoad(): void;
declare function mapSelection(): void;
