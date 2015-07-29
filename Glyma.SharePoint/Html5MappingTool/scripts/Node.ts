"use strict";
module Glyma {
    export class Node {
        private _nodeId: string;
        private _nodeType: string;
        private _xPosition: number;
        private _yPosition: number;
        private _width: number = -1;
        private _height: number = -1;
        private _left: number = -1;
        private _top: number = -1;
        private _right: number = -1;
        private _bottom: number = -1;

        private _imageLeft: number = -1;
        private _imageTop: number = -1;

        private _cornerButtonLeft: number = -1;
        private _cornerButtonTop: number = -1;
        private _cornerExtendButtonLeft: number = -1;

        private _collapseButtonLeft: number = -1;
        private _collapseButtonTop: number = -1;

        private _name: string = "";
        private _image = null;
        private _lastImageScale = null;
        private _lastTextSacle = null;
        private _nodeTextBox: NodeTextBox = null;
        private _textCanvas: TextCanvas = null;
        private _isSelected: boolean = false;

        private _hasVideo: boolean = false;
        private _hasContent: boolean = false;
        private _hasLocation: boolean = false;
        private _hasMap: boolean = false;
        private _hasFeed: boolean = false;
        private _hasLink: boolean = false;
        private _index: number = -1;

        private _childIndexes: Array<number> = null;
        private _parentIndexes: Array<number> = null;

        private _nodeCornerButton: NodeCornerButton;

        private _nodeClickOptions: NodeClickOptions = null;
        private _nodeActionOptions: NodeActionOptions = null;

        private _isVisible: boolean = true;
        private _collapseState: string = "none";

        public rootDepth: number = 0;
        public stackNumber: number = 0;

        private _videoSource: string = null;
        private _startPosition: string = null;
        private _stopPosition: string = null;
        private _descriptionContent: string = null;
        private _descriptionType: string = null;
        private _descriptionUrl: string = null;
        private _descriptionWidth: string = null;
        private _descriptionHeight: string = null;
        private _videoParams: string = null;
        private _relatedMaps: any = null;
        private _link: string = null;

        private _isHoverCornerButton: boolean = false;
        private _isHoverCornerExtendButton: boolean = false;
        private _isHoverCollapseButton: boolean = false;

        constructor(item: any, index:number) {
            this._nodeId = item.uniqueId;
            this._index = index;
            this._name = item.name;
            this._nodeType = item.nodeType;
            this._xPosition = item.xPosition;
            this._yPosition = item.yPosition;

            if (item.Visibility == "Collapsed") {
                this.isVisible = false;
            }
            
            if (item.hasOwnProperty('Video') && item.Video.hasOwnProperty('Source')) {
                this._hasVideo = true;
                this._videoSource = item.Video.Source;
                if (item.Video.hasOwnProperty('StartPosition')) {
                    this._startPosition = item.Video.StartPosition;
                } else {
                    this._startPosition = "00:00:00.000";
                }
                if (item.Video.hasOwnProperty('EndPosition')) {
                    this._stopPosition = item.Video.EndPosition;
                } 
                var endtimecode = '';
                if (this._stopPosition != null) {
                    endtimecode = ',{"Name": "EndTimeCode","Value": "' + this._stopPosition + '"}';
                }
                this._videoParams = JSON.parse('[{"Name": "NodeId","Value": "' + this._nodeId + '"}'
                    + ',{"Name": "Source","Value": "' + encodeURIComponent(this._videoSource) + '"}'
                + ',{"Name": "StartTimeCode","Value": "' + this._startPosition + '"}'
                    + endtimecode
                + ',{"Name": "AutoPlay","Value": "true"}]');
            }
            
            if (item.hasOwnProperty('Description')) {
                this._hasContent = true;
                if (item.Description.hasOwnProperty('Type')) {
                    if (item.Description.Type == "Iframe") {
                        this._descriptionUrl = item.Description.Url;
                    } else {
                        this._descriptionContent = item.Description.Content;
                    }

                    if (item.Description.hasOwnProperty('Height')) {
                        this._descriptionHeight = item.Description.Height;
                    }
                    else {
                        this._descriptionHeight = "0"; //if no height is set
                    }
                    if (item.Description.hasOwnProperty('Width')) {
                        this._descriptionWidth = item.Description.Width;
                    }
                    else {
                        this._descriptionWidth = "0"; //if no width is set
                    }

                    this._descriptionType = item.Description.Type;
                }
                ////ensure that the Description is not an empty string or whitespace only
                //if (item.Description.trim() != "") {
                //    this._hasContent = true;
                //    this._description = item.Description;
                //    if (item.hasOwnProperty('DescriptionType')) {
                //        this._descriptionType = item.DescriptionType;
                //    } else {
                //        this._descriptionType = "None";
                //    }
                //}
            }

            this._nodeClickOptions = new NodeClickOptions();
            if (item.hasOwnProperty('NodeClickOptions')) {
                var optionsStr: string = item.NodeClickOptions;
                var options: string[] = optionsStr.split(/[,;\s]+/); //split on , ; or space
                var node: Node = this;
                $.each(options, function (index: number, option: string) {
                    if (option.trim().toLowerCase() == "relatedmaps.hide") {
                        node._nodeClickOptions.showRelatedMaps = false;
                    }
                });
            }

            this._nodeActionOptions = new NodeActionOptions();
            if (item.hasOwnProperty('NodeActionOptions')) {
                var optionsStr: string = item.NodeActionOptions;
                var options: string[] = optionsStr.split(/[,;\s]+/); //split on , ; or space
                var node: Node = this;
                $.each(options, function (index: number, option: string) {
                    if (option.trim().toLowerCase() == "video.showrelatedcontent") {
                        node._nodeActionOptions.showRelatedContentWithVideo = true;
                    }
                });
            }

            if (item.hasOwnProperty('RelatedMaps')) {
                this._relatedMaps = item.RelatedMaps;
                this._hasMap = true;
            }

            if (item.hasOwnProperty('Link')) {
                this._hasLink = true;
                this._link = item.Link;
            }
        }

        public get videoSource(): string {
            return this._videoSource;
        }

        public get startPosition(): string {
            return this._startPosition;
        }

        public get stopPosition(): string {
            return this._stopPosition;
        }

        public get descriptionContent(): string {
            return this._descriptionContent;
        }

        public get videoParams(): string {
            return this._videoParams;
        }

        public get nodeActionOptions(): NodeActionOptions {
            return this._nodeActionOptions;
        }

        public get nodeClickOptions(): NodeClickOptions {
            return this._nodeClickOptions;
        }

        public get isHoverCornerButton(): boolean {
            return this._isHoverCornerButton;
        }

        public get isHoverCornerExtendButton(): boolean {
            return this._isHoverCornerExtendButton;
        }

        public get isHoverCollapseButton(): boolean {
            return this._isHoverCollapseButton;
        }

        public set isHoverCornerButton(value: boolean) {
            this._isHoverCornerButton = value;
        }

        public set isHoverCornerExtendButton(value: boolean) {
            this._isHoverCornerExtendButton = value;
        }

        public set isHoverCollapseButton(value: boolean) {
            this._isHoverCollapseButton = value;
        }

        get nodeTextBox(): NodeTextBox {
            if (this._nodeTextBox == null) {
                this._nodeTextBox = new NodeTextBox(this.name, this.hasLink);
            }

            return this._nodeTextBox;
        }

        get nodeId(): string {
            return this._nodeId;
        }

        get childIndexes(): Array<number> {
            if (this._childIndexes == null) {
                this._childIndexes = new Array<number>();
            }
            return this._childIndexes;
        }

        get parentIndexes(): Array<number> {
            if (this._parentIndexes == null) {
                this._parentIndexes = new Array<number>();
            }
            return this._parentIndexes;
        }

        get isVisible(): boolean {
            return this._isVisible;
        }

        set isVisible(value: boolean) {
            this._isVisible = value;
        }

        get collpaseState(): string {
            return this._collapseState;
        }

        set collpaseState(value: string) {
            this._collapseState = value;
        }

        get nodeCornerButton(): NodeCornerButton {
            if (this._nodeCornerButton == null) {
                this._nodeCornerButton = new NodeCornerButton(this);
            }

            return this._nodeCornerButton;
        }

        get isSelected(): boolean {
            return this._isSelected;
        }

        get relatedMaps(): any {
            return this._relatedMaps;
        }

        get link(): string {
            return this._link;
        }

        public clicked(x: number, y: number) {
            if (x >= this.imageLeft && x <= this.imageLeft + Common.Constants.imageSize && y >= this.imageTop && y <= this.imageTop + Common.Constants.imageSize) {
                this.select();
                if (x >= this.cornerButtonLeft && x <= this.cornerButtonLeft + Common.Constants.cornerButtonWidth && y >= this.cornerButtonTop && y <= this.cornerButtonTop + Common.Constants.cornerButtonHeight) {
                    this.cornerButtonClicked();
                }
            }
        }

        public cornerButtonClicked() {
            switch (this.nodeCornerButton.showingButton) {
                case "play":
                    Glyma.NodeController.playVideo(this._index);
                    break;
                case "pause":
                    Glyma.NodeController.stopVideo();
                    break;
                case "map":
                    Glyma.NodeController.showRelatedMaps(this._relatedMaps);
                    break;
                case "content":
                    Glyma.NodeController.showContent(this._index);
                    break;
                default:
                    break;
            }
        }

        public select() {
            if (!this._isSelected) {
                this._isSelected = true;
            }
            if (this.nodeClickOptions.showRelatedMaps) {
                Glyma.NodeController.showRelatedMaps(this._relatedMaps);
            } else {
                //hide the related maps if already shown (for a different node context)
                if (typeof (Glyma.RelatedContentPanels.RelatedContentController) === "function") {
                    var controller: Glyma.RelatedContentPanels.RelatedContentController = Glyma.RelatedContentPanels.RelatedContentController.getInstance();
                    if (controller != null) {
                        var mapsPanel: Glyma.RelatedContentPanels.RelatedMapsContentPanel = <Glyma.RelatedContentPanels.RelatedMapsContentPanel>controller.getContentPanelByName("RelatedNodesPanel");
                        if (mapsPanel != null) {
                            mapsPanel.clearRelatedMaps();
                        }
                    }
                }
            }
        }

        public deselect() {
            if (this._isSelected) {
                this._isSelected = false;
            }
        }

        get descriptionType(): string {
            if (this._descriptionType == null || this._descriptionType != "Iframe") {
                return "nodeHtml";
            } else {
                return "iframeUrl";
            }
        }

        get descriptionUrl(): string {
            return this._descriptionUrl;
        }

        get descriptionWidth(): string {
            return this._descriptionWidth;
        }

        get descriptionHeight(): string {
            return this._descriptionHeight;
        }

        get hasVideo(): boolean {
            return this._hasVideo;
        }

        get hasContent(): boolean {
            return this._hasContent;
        }

        get hasLocation(): boolean {
            return this._hasLocation;
        }

        get hasMap(): boolean {
            return this._hasMap;
        }

        get hasFeed(): boolean {
            return this._hasFeed;
        }

        get hasLink(): boolean {
            return this._hasLink;
        }

        get nodeType(): string {
            return this._nodeType;
        }

        get xPosition(): number {
            return this._xPosition;
        }
        
        set xPosition(value: number) {
            this._xPosition = value;
            this._imageLeft = -1;
            this._cornerButtonLeft = -1;
            this._cornerExtendButtonLeft = -1;
            this._collapseButtonLeft = -1;
            this._left = -1;
            this._right = -1;
        }

        get yPosition(): number {
            return this._yPosition;
        }

        get lastImageScale(): string {
            if (this._lastImageScale == null) {
                this._lastImageScale = "1x";
            }
            return this._lastImageScale;
        }

        set yPosition(value: number) {
            this._yPosition = value;
            this._imageTop = -1;
            this._cornerButtonTop = -1;
            this._collapseButtonTop = -1;
            this._top = -1;
            this._bottom = -1;
        }

        get width(): number {
            if (this._width < 0) {
                this._width = Math.max(this.TextCanvas.maxWidth, Common.Constants.imageSize);
            }

            return this._width;
        }

        get height(): number {
            if (this._height < 0) {
                this._height = Common.Constants.imageSize + this.TextCanvas.height;
            }

            return this._height;
        }

        get imageTop(): number {
            if (this._imageTop < 0) {
                this._imageTop = this.yPosition - this.height / 2;
            }
            return this._imageTop;
        }

        get imageLeft(): number {
            if (this._imageLeft < 0) {
                this._imageLeft = this.xPosition - Common.Constants.imageSize / 2;
            }
            return this._imageLeft;
        }

        get cornerButtonTop(): number {
            if (this._cornerButtonTop < 0) {
                this._cornerButtonTop = this.imageTop + Common.Constants.imageSize - Common.Constants.cornerButtonHeight - Common.Constants.cornerButtonTopOffset;
            }
            return this._cornerButtonTop;
        }

        get cornerButtonLeft(): number {
            if (this._cornerButtonLeft < 0) {
                this._cornerButtonLeft = this.imageLeft + Common.Constants.imageSize - Common.Constants.cornerButtonWidth - Common.Constants.cornerButtonOffset;
            }
            return this._cornerButtonLeft;
        }

        get cornerExtendButtonLeft(): number {
            if (this._cornerExtendButtonLeft < 0) {
                this._cornerExtendButtonLeft = this.cornerButtonLeft + Common.Constants.cornerExtendButtonOffset;
            }
            return this._cornerExtendButtonLeft;
        }

        get collapseButtonLeft(): number {
            if (this._collapseButtonLeft < 0) {
                this._collapseButtonLeft = this.imageLeft + Common.Constants.imageSize - Common.Constants.collapseImageSize - Common.Constants.collapseButtonOffset;
            }
            return this._collapseButtonLeft;
        }

        get collapseButtonTop(): number {
            if (this._collapseButtonTop < 0) {
                this._collapseButtonTop = this.imageTop + Common.Constants.collapseButtonTopOffset;
            }
            return this._collapseButtonTop;
        }

        get left(): number {
            if (this._left < 0) {
                this._left = this.xPosition - this.width / 2;
            }

            return this._left;
        }

        get top(): number {
            if (this._top < 0) {
                this._top = this.yPosition - this.height / 2;
            }

            return this._top;
        }

        get right(): number {
            if (this._right < 0) {
                this._right = this.left + this.width;
            }

            return this._right;
        }

        get bottom(): number {
            if (this._bottom < 0) {
                this._bottom = this.top + this.height;
            }

            return this._bottom;
        }

        get name(): string {
            return this._name;
        }

        get image() {
            var sizeToUse = "1x";

            if (this._image != null) {
                if (SharedVariables.scale < 1.5 && this._lastImageScale == "1x") {
                    sizeToUse = "same";
                } else if (SharedVariables.scale >= 1.5 && SharedVariables.scale < 3 && this._lastImageScale == "2x") {
                    sizeToUse = "same";
                } else if (SharedVariables.scale >= 3 && SharedVariables.scale < 6 && this._lastImageScale == "4x") {
                    sizeToUse = "same";
                } else if (SharedVariables.scale >= 6 && this._lastImageScale == "8x") {
                    sizeToUse = "same";
                } else if (SharedVariables.scale < 1.5) {
                    sizeToUse = "1x";
                } else if (SharedVariables.scale < 3) {
                    sizeToUse = "2x";
                } else if (SharedVariables.scale < 6) {
                    sizeToUse = "4x";
                } else if (SharedVariables.scale >= 6) {
                    sizeToUse = "8x";
                } else {
                    sizeToUse = "same";
                }
            }

            if (sizeToUse == this._lastImageScale) {
                return this._image;
            }
            else if (sizeToUse == "1x") {
                switch (this.nodeType) {
                    case 'Pro':
                        this._image = Common.Constants.ProImage;
                        break;
                    case 'Con':
                        this._image = Common.Constants.ConImage;
                        break;
                    case 'Question':
                        this._image = Common.Constants.QuestionImage;
                        break;
                    case 'Idea':
                        this._image = Common.Constants.IdeaImage;
                        break;
                    case 'Map':
                        this._image = Common.Constants.MapImage;
                        break;
                    case 'Note':
                        this._image = Common.Constants.NoteImage;
                        break;
                    case 'Decision':
                        this._image = Common.Constants.decisionImage;
                        break;
                }
                this._lastImageScale = "1x";
            }
            else if (sizeToUse == "2x") {
                switch (this.nodeType) {
                    case 'Pro':
                        this._image = Common.Constants.ProImage2x;
                        break;
                    case 'Con':
                        this._image = Common.Constants.ConImage2x;
                        break;
                    case 'Question':
                        this._image = Common.Constants.QuestionImage2x;
                        break;
                    case 'Idea':
                        this._image = Common.Constants.IdeaImage2x;
                        break;
                    case 'Map':
                        this._image = Common.Constants.MapImage2x;
                        break;
                    case 'Note':
                        this._image = Common.Constants.NoteImage2x;
                        break;
                    case 'Decision':
                        this._image = Common.Constants.decisionImage2x;
                        break;
                }
                this._lastImageScale = "2x";
            }
            else if (sizeToUse == "4x") {
                switch (this.nodeType) {
                    case 'Pro':
                        this._image = Common.Constants.ProImage4x;
                        break;
                    case 'Con':
                        this._image = Common.Constants.ConImage4x;
                        break;
                    case 'Question':
                        this._image = Common.Constants.QuestionImage4x;
                        break;
                    case 'Idea':
                        this._image = Common.Constants.IdeaImage4x;
                        break;
                    case 'Map':
                        this._image = Common.Constants.MapImage4x;
                        break;
                    case 'Note':
                        this._image = Common.Constants.NoteImage4x;
                        break;
                    case 'Decision':
                        this._image = Common.Constants.decisionImage4x;
                        break;
                }
                this._lastImageScale = "4x";
            }
            else if (sizeToUse == "8x") {
                switch (this.nodeType) {
                    case 'Pro':
                        this._image = Common.Constants.ProImage8x;
                        break;
                    case 'Con':
                        this._image = Common.Constants.ConImage8x;
                        break;
                    case 'Question':
                        this._image = Common.Constants.QuestionImage8x;
                        break;
                    case 'Idea':
                        this._image = Common.Constants.IdeaImage8x;
                        break;
                    case 'Map':
                        this._image = Common.Constants.MapImage8x;
                        break;
                    case 'Note':
                        this._image = Common.Constants.NoteImage8x;
                        break;
                    case 'Decision':
                        this._image = Common.Constants.decisionImage8x;
                        break;
                }
                this._lastImageScale = "8x";
            }

            return this._image;
        }

        get TextCanvas(): TextCanvas {
            var sizeToUse = "1x";
            if (this._textCanvas != null) {
                if (SharedVariables.scale < 0.8) {
                    sizeToUse = "0.5x";
                } else if (SharedVariables.scale < 1) {
                    sizeToUse = "0.8x";
                } else if (SharedVariables.scale < 1.5) {
                    sizeToUse = "1x";
                } else if (SharedVariables.scale < 3) {
                    sizeToUse = "2x";
                } else if (SharedVariables.scale < 6) {
                    sizeToUse = "4x";
                }
            }

            if (sizeToUse == this._lastTextSacle) {
                return this._textCanvas;
            } else {
                if (this._textCanvas != null) {
                    this.nodeTextBox.reset();
                }
                this._textCanvas = this.nodeTextBox.createTextCanvas();
                this._lastTextSacle = sizeToUse;
            }

            return this._textCanvas;
        }
    }
}