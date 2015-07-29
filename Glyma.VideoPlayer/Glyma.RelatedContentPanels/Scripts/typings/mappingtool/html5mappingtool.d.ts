declare module Glyma {
    class Breadcrumb {
        private _uniqueId;
        private _name;
        constructor(breadcrumbData: any);
        public name : string;
        public fullName : string;
        public uniqueId : string;
    }
}
declare module Glyma {
    class BreadcrumbControl {
        private _breadcrumbs;
        private _domainId;
        constructor(domainId: any);
        public breadcrumbs : Breadcrumb[];
        public currentBreadcrumb : Breadcrumb;
        public refreshBreadcrumb(): void;
        public removeToIndex(index: number): void;
        public getIndexByNodeId(nodeId: string): number;
        public addBreadcrumb(breadcrumbData: any): void;
        public insertBreadcrumb(breadcrumbData: any, index?: number): void;
        private _createBreadcrumbElement(breadcrumb);
    }
}
declare module Glyma {
    class Point {
        public x: number;
        public y: number;
        constructor(x: number, y: number);
        private initialEvents();
    }
}
declare module Glyma {
    class ElementLoader {
        private _hiddenContainer;
        private _baseUrl;
        constructor(baseUrl: string);
        public load(): void;
        private _loadImages();
        private _loadOthers();
    }
}
declare module Glyma {
    interface IEvent {
        add(listener: () => void): void;
        remove(listener: () => void): void;
        trigger(...a: any[]): void;
    }
    class TypedEvent implements IEvent {
        private _listeners;
        public add(listener: () => void): void;
        public remove(listener?: () => void): void;
        public trigger(...a: any[]): void;
    }
    interface IStateChangedEvent extends IEvent {
        add(listener: (mapId: string, domainId: string, nodeId: string) => void): void;
        remove(listener: (mapId: string, domainId: string, nodeId: string) => void): void;
        trigger(mapId: string, domainId: string, nodeId: string): void;
    }
}
declare module Glyma {
    class HistoryManager {
        private _historyMaps;
        private _currentUrl;
        public historyMaps : string[];
        constructor();
        public pushHistory(mapName: string, mapId: string, domainId: string, nodeId?: string): void;
    }
}
declare module Glyma {
    class MapController {
        private _nodeDetails;
        private _mapRenderer;
        private _breadcrumbControl;
        private _rTreeManager;
        private _realignController;
        private _historyManager;
        private _mapId;
        private _isRootMapLoaded;
        private _isLeftbarExpended;
        public mapRenderer : MapRenderer;
        public nodes : Node[];
        private realignController;
        public arrows : any[];
        public domainId : string;
        public rTreeManager : RTreeManager;
        public breadcrumbControl : BreadcrumbControl;
        public visibleNodeIndexes : number[];
        public historyManager : HistoryManager;
        public nodeDetails : NodeDetailButtonController;
        public isLeftbarExpended : boolean;
        constructor();
        public clearRTree(): void;
        public hideNodeDetailButtons(): void;
        public refreshCurrentMap(): void;
        public mouseMoved(x: number, y: number): void;
        public clicked(x: number, y: number, type?: string): void;
        public getMapData(domainId: string, mapId: string, nodeId?: string, isPushState?: boolean): any;
        public getNodeIndexById(nodeId: string): number;
        public loadMaps(domainId: string): void;
        public loadDomains(): void;
        public centraliseNode(nodeId?: string): void;
        public centraliseMostImportantNode(): void;
        private _centraliseNodeByIndex(index, selectNode?);
        private _bringNodeVisible(index);
        public getAllChildIndexes(index: number): number[];
        public collapseNode(index: number): void;
        public expandNodes(index: number): void;
        public expandAllNodes(index: number): void;
        public refresh(): void;
        public initialise(): void;
    }
}
declare function getCurrentSiteUrl(): string;
declare function getBaseUrl(): string;
declare function getServerRelativeVersionedLayoutsFolder(): string;
declare function showError(msg: string): void;
declare function openInNewTab(url: string): void;
declare function showLoad(): void;
declare function mapSelection(): void;
declare module Glyma {
    function MessageListener(type: string, msg: string, id: string): void;
}
declare module Glyma {
    class NodeActionOptions {
        private _showRelatedContentWithVideo;
        public showRelatedContentWithVideo : boolean;
    }
}
declare module Glyma {
    class NodeClickOptions {
        private _showRelatedMaps;
        public showRelatedMaps : boolean;
    }
}
declare module Glyma.NodeController {
    function playVideo(index: number): void;
    function stopVideo(): void;
    function showRelatedMaps(maps: any): void;
    function showContent(index: number): void;
}
declare module Glyma {
    class NodeCornerButton {
        private _showingButton;
        private _hasExtendButton;
        private _hasCornerButton;
        constructor(node: Node);
        public hasCornerButton : boolean;
        public hasExtendButton : boolean;
        public showingButton : string;
    }
}
declare module Glyma {
    class NodeDetailButton {
        private _type;
        constructor(type: string);
    }
}
declare module Glyma {
    class ArrowRenderer {
        private _margin;
        private _panX;
        private _panY;
        private _arrowColor;
        private _canvas;
        private _context;
        constructor(canvas: HTMLCanvasElement, context: CanvasRenderingContext2D);
        private _lineLength(x1, y1, x2, y2);
        private _intersectNode(node1, node2, diff);
        private _isVisible(node);
        private _getBoundaryPoint(node, angle);
        public drawArrow(fromNode: any, toNode: any): void;
    }
}
declare module Glyma.Common.Constants {
    var isFullScreen: boolean;
    var imageSize: number;
    var imageRadius: number;
    var collapseImageSize: number;
    var collapseButtonOffset: number;
    var collapseButtonTopOffset: number;
    var extendCornerButtonWidth: number;
    var cornerExtendButtonOffset: number;
    var cornerButtonOffset: number;
    var cornerButtonTopOffset: number;
    var cornerButtonWidth: number;
    var cornerButtonHeight: number;
    var arrowMargin: number;
    var arrowHeadSize: number;
    var arrowLineWidth: number;
    var arrowMinLength: number;
    var realignTopMagin: number;
    var realignNodeWidth: number;
    var realignLeftMargin: number;
    var textSize: number;
    var maxTextWidth: number;
    var maxWordLength: number;
    var textHeight: number;
    var ProImage: HTMLElement;
    var ConImage: HTMLElement;
    var IdeaImage: HTMLElement;
    var QuestionImage: HTMLElement;
    var MapImage: HTMLElement;
    var NoteImage: HTMLElement;
    var decisionImage: HTMLElement;
    var ProImage2x: HTMLElement;
    var ConImage2x: HTMLElement;
    var IdeaImage2x: HTMLElement;
    var QuestionImage2x: HTMLElement;
    var MapImage2x: HTMLElement;
    var NoteImage2x: HTMLElement;
    var decisionImage2x: HTMLElement;
    var ProImage4x: HTMLElement;
    var ConImage4x: HTMLElement;
    var IdeaImage4x: HTMLElement;
    var QuestionImage4x: HTMLElement;
    var MapImage4x: HTMLElement;
    var NoteImage4x: HTMLElement;
    var decisionImage4x: HTMLElement;
    var ProImage8x: HTMLElement;
    var ConImage8x: HTMLElement;
    var IdeaImage8x: HTMLElement;
    var QuestionImage8x: HTMLElement;
    var MapImage8x: HTMLElement;
    var NoteImage8x: HTMLElement;
    var decisionImage8x: HTMLElement;
}
declare module Glyma {
    class MapRenderer {
        private _mapData;
        private _nodes;
        private _panX;
        private _panY;
        private _isDrag;
        private _arrowRenderer;
        private _nodeRenderer;
        private _breadcrumbControl;
        private _rTreeManager;
        private _sliderValue;
        private _topMost;
        private _bottomMost;
        private _leftMost;
        private _rightMost;
        private _rootMap;
        private _domainId;
        constructor();
        public domainId : string;
        public rTreeManager : RTreeManager;
        public arrowRendererInstance : ArrowRenderer;
        public breadcrumbControl : BreadcrumbControl;
        public nodeRendererInstance : NodeRenderer;
        public nodes : Node[];
        public arrows : any[];
        public rootMap : any;
        public rightMost : number;
        public leftMost : number;
        public bottomMost : number;
        public topMost : number;
        private _getSliderValue(scale);
        private _getScaleValue(value);
        private _getPercentage(value);
        public scaleBySlider(value: number, changeSlider?: boolean): void;
        public zoomIn(): void;
        public zoomOut(): void;
        public defaultZoom(): void;
        public scaleMap(scale: number, changeSlider?: boolean): void;
        public reRenderNode(i: number): void;
        public resetMap(): void;
        public createBreadcrumb(): void;
        public refresh(): void;
        public calculateBorder(): void;
        public drawMap(data: any): void;
        public checkIncorrectVisibility(): void;
    }
}
declare module Glyma.Common.Math {
    function scale(value: any): any;
}
declare module Glyma {
    class Node {
        private _nodeId;
        private _nodeType;
        private _xPosition;
        private _yPosition;
        private _width;
        private _height;
        private _left;
        private _top;
        private _right;
        private _bottom;
        private _imageLeft;
        private _imageTop;
        private _cornerButtonLeft;
        private _cornerButtonTop;
        private _cornerExtendButtonLeft;
        private _collapseButtonLeft;
        private _collapseButtonTop;
        private _name;
        private _image;
        private _lastImageScale;
        private _lastTextSacle;
        private _nodeTextBox;
        private _textCanvas;
        private _isSelected;
        private _hasVideo;
        private _hasContent;
        private _hasLocation;
        private _hasMap;
        private _hasFeed;
        private _hasLink;
        private _index;
        private _childIndexes;
        private _parentIndexes;
        private _nodeCornerButton;
        private _nodeClickOptions;
        private _nodeActionOptions;
        private _isVisible;
        private _collapseState;
        public rootDepth: number;
        public stackNumber: number;
        private _videoSource;
        private _startPosition;
        private _stopPosition;
        private _descriptionContent;
        private _descriptionType;
        private _descriptionUrl;
        private _descriptionWidth;
        private _descriptionHeight;
        private _videoParams;
        private _relatedMaps;
        private _link;
        private _isHoverCornerButton;
        private _isHoverCornerExtendButton;
        private _isHoverCollapseButton;
        constructor(item: any, index: number);
        public videoSource : string;
        public startPosition : string;
        public stopPosition : string;
        public descriptionContent : string;
        public videoParams : string;
        public nodeActionOptions : NodeActionOptions;
        public nodeClickOptions : NodeClickOptions;
        public isHoverCornerButton : boolean;
        public isHoverCornerExtendButton : boolean;
        public isHoverCollapseButton : boolean;
        public nodeTextBox : NodeTextBox;
        public nodeId : string;
        public childIndexes : number[];
        public parentIndexes : number[];
        public isVisible : boolean;
        public collpaseState : string;
        public nodeCornerButton : NodeCornerButton;
        public isSelected : boolean;
        public relatedMaps : any;
        public link : string;
        public clicked(x: number, y: number): void;
        public cornerButtonClicked(): void;
        public select(): void;
        public deselect(): void;
        public descriptionType : string;
        public descriptionUrl : string;
        public descriptionWidth : string;
        public descriptionHeight : string;
        public hasVideo : boolean;
        public hasContent : boolean;
        public hasLocation : boolean;
        public hasMap : boolean;
        public hasFeed : boolean;
        public hasLink : boolean;
        public nodeType : string;
        public xPosition : number;
        public yPosition : number;
        public lastImageScale : string;
        public width : number;
        public height : number;
        public imageTop : number;
        public imageLeft : number;
        public cornerButtonTop : number;
        public cornerButtonLeft : number;
        public cornerExtendButtonLeft : number;
        public collapseButtonLeft : number;
        public collapseButtonTop : number;
        public left : number;
        public top : number;
        public right : number;
        public bottom : number;
        public name : string;
        public image : any;
        public TextCanvas : TextCanvas;
    }
}
declare module Glyma {
    class NodeDetailButtonController {
        private _node;
        private _defaultXs;
        private _defaultYs;
        private _container;
        private _canvas;
        constructor();
        public showButtons(node: Node, index: number, x: number, y: number): void;
        private addButton(type, num, index, nodeId);
        public hideButtons(): void;
        private initialEvents();
    }
}
declare module Glyma {
    class NodeDetails {
        private _node;
        private _defaultLocations;
        private _detailButtons;
        private _container;
        constructor();
        public showButtons(node: Node): void;
        private addButton(type, num);
        private initialEvents();
    }
}
declare module Glyma {
    class NodeRenderer {
        private _context;
        constructor(context: CanvasRenderingContext2D);
        public drawNode(node: Node): void;
        public redrawNodeImage(node: Node, offSetX?: number, offSetY?: number): void;
        private drawCornerButton(node, offSetX?, offSetY?);
        public cornerButton(node: Node, offSetX?: number, offSetY?: number, isHover?: boolean): void;
        public cornerExtendButton(node: Node, offSetX?: number, offSetY?: number, isHover?: boolean): void;
        private drawCollapseControl(node, offSetX?, offSetY?);
    }
}
declare module Glyma {
    class NodeTextBox {
        private _text;
        private _textMeasurer;
        private _scale;
        private _hasLink;
        constructor(text: string, hasLink?: boolean);
        private NodeTextMeasurer;
        public Text : string;
        public scale : number;
        public reset(): void;
        public createTextCanvas(): TextCanvas;
    }
}
declare module Glyma {
    class RealignController {
        private _parents;
        private _nodeNeedToBePlaced;
        private _mapController;
        private _maxDepth;
        constructor(mapController: MapController);
        public horizontalRealign(): void;
        private _calculateStackNumberByDepth(rootDepth);
        private _calculateByDepth(rootDepth);
        private _calculate();
        private _addLeftMostNodeToParent();
        private _placeNodeToMap();
        private _recheckForIncorrectDepth();
    }
}
declare module Glyma.Html5.MappingTool {
    class RelatedContentPanelBridge {
        private static _instance;
        constructor();
        static getInstance(): RelatedContentPanelBridge;
        static getRootMapUrl(): string;
        static getCurrentRootMapUid(): string;
        static getCurrentDomainUid(): string;
        static getRootMapMetadataValue(metadataKey: string): string;
        static loadMapAndSelectedNode(domainId: string, mapNodeId: string, currentNode: string): void;
        static receiveGlymaMessage(message: string): void;
        static getVideoEventArg(commandXml: string, eventArgName: string): string;
        static getVideoEventName(commandXml: string): string;
    }
}
declare module Glyma {
    class RTreeManager {
        private _rTree;
        private _arrowTree;
        private _nodeLength;
        private _arrowLength;
        private rTree;
        private arrowTree;
        constructor();
        public clear(): void;
        public insertNode(x: number, y: number, w: number, h: number, object: any): void;
        public insertArrow(x: number, y: number, w: number, h: number, object: any): void;
        public searchNodes(x: number, y: number, w: number, h: number): any;
        public searchArrows(x: number, y: number, w: number, h: number): any;
        public reset(nodeNumber: number, arrowNumber: number): void;
    }
}
declare class RTree {
    constructor(length: number);
    public clear(): void;
    public insert(area: any, object: any): void;
    public search(area: any): any;
    public remove(area: any): any;
    public delete(area: any): any;
}
declare module Glyma.SharedVariables {
    var canvas: HTMLCanvasElement;
    var pinchzoom: HTMLElement;
    var context: CanvasRenderingContext2D;
    var mapController: MapController;
    var touchManager: TouchManager;
    var posX: number;
    var posY: number;
    var lastPosX: number;
    var lastPosY: number;
    var mouseX: number;
    var mouseY: number;
    var lastMouseX: number;
    var lastMouseY: number;
    var mouseRelativeX: number;
    var mouseRelativeY: number;
    var scale: number;
    var lastScale: number;
    var isHtmlElementsLoaded: boolean;
    function getMapLocationTop(): number;
    function getMapLocationLeft(): number;
    function getMapWidth(): number;
    function getMapHeight(): number;
}
declare module Glyma {
    interface TextCanvas {
        maxWidth: number;
        height: number;
        lines: TextLine[];
        left?: number;
        top?: number;
        canvas?: HTMLCanvasElement;
    }
    interface TextLine {
        width: number;
        text: string;
    }
    class TextMeasurer {
        private _fontFace;
        private _fontSize;
        private _maxWidth;
        private _maxWordLength;
        private _textCanvas;
        private _textContext;
        constructor();
        public fontSize : number;
        public fontFace : string;
        private _getSmallWords(word, maxWordLength);
        private _getWords(text, maxWordLength);
        public reset(): void;
        public measureText(text: string): TextCanvas;
    }
}
declare module Glyma {
    class TouchManager {
        private _hammertime;
        constructor();
        private _initialEvents();
        public scaleMap(): void;
    }
}
declare function Hammer(element: any, options: any): any;
declare module Glyma.Util {
}
