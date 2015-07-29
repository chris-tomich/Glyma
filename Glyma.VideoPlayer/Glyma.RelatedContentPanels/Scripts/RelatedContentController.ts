/// <reference path="RelatedContentPanel.ts"/>
/// <reference path="PageContentPanel.ts"/>
module Glyma.RelatedContentPanels {
    export class RelatedContentController {
        private ICON_PANEL_ID = "#IconPanel";
        private ICON_PANEL_WIDTH: number;
        private EXPANDER_PANEL_WIDTH: number;
        private BORDER_WIDTH: number = 2;
        private CONTENT_PANEL_RIGHT_MARGIN: number;
        public DEFAULT_CONTENT_WIDTH: number = 400;

        private _maxContentWidth: number;
        private _panelDefiningMaxWidth: string;

        private resizeCallbacks = new Array(); //callbacks registered to call

        private contentPanels: { [key: string]: RelatedContentPanel } = {};
        public maxButtonEnabledStates: { [key: string]: boolean } = {};
        public maxButtonEnabledImages: { [key: string]: string } = {};
        public maxButtonDisabledImages: { [key: string]: string } = {};

        public inAuthorMode: boolean = false;

        private static _instance: RelatedContentController = null;
        private static _baseUrl: string = null;

        constructor() {
            if (RelatedContentController._instance != null) {
                throw new Error("Error: Instantiation failed: Use RelatedContentController.getInstance() instead of new");
            }
            RelatedContentController._instance = this;
            this.setDefaultsFromCss();
            this.initShowAllButton();
            this.setPanelsSortable();
            this.initExpander();
        }

        public static getInstance(): RelatedContentController {
            if (RelatedContentController._instance === null) {
                RelatedContentController._instance = new RelatedContentController();
            }
            return RelatedContentController._instance;
        }

        public mapLoadCompleted(rootMapLoaded) {
            if (Glyma.RelatedContentPanels.YammerContentPanel.IsYammerEnabled()) {
                var yammerPanel = new Glyma.RelatedContentPanels.YammerContentPanel();
                this.addRelatedContentPanel(yammerPanel);
            }
            else {
                var yammerPanel = this.contentPanels["YammerPanel"];
                if (yammerPanel != null) {
                    yammerPanel.disableContentPanel(true);
                }
            }
            if (!rootMapLoaded) {
                var activityFeedPanel: ActivityFeedContentPanel = <ActivityFeedContentPanel>this.contentPanels["ActivityFeedPanel"];
                var filteredFeedPanel: FilteredFeedContentPanel = <FilteredFeedContentPanel>this.contentPanels["FilteredFeedPanel"];
                if (activityFeedPanel != null) {
                    activityFeedPanel.showFeedPanel();
                }
                if (filteredFeedPanel != null) {
                    filteredFeedPanel.showFeedPanel();
                }
            }
        }

        public static onWindowResized():void {
            if ($("#glyma-body").length > 0) {
                //it's our minimal page layout
                var fullHeight = $("#glyma-body").height();
                $("#ContentPanel").css("height", fullHeight + "px");

                //if the window is resized the scroll bars might need to be added/removed
                RelatedContentController.getInstance().onRelatedContentPanelResize();
            }
            else {
                //it's a web part page - do nothing it's got a size set in CSS
            }
        }

        public static setBaseUrl(baseUrl: string): void {
            //only allow the baseUrl to be set once
            if (RelatedContentController._baseUrl === null) {
                RelatedContentController._baseUrl = baseUrl;
            }
            //Run a function that will request images used so they are cached before needed
            RelatedContentController.preloadImages(baseUrl);
        }

        public static getBaseUrl(): string {
            return RelatedContentController._baseUrl;
        }

        public getContentPanelByName(name: string): RelatedContentPanel {
            var contentPanel: RelatedContentPanel = this.contentPanels[name];
            return contentPanel;
        }

        private static preloadImages(baseUrl: string): void {
            if (baseUrl.charAt(baseUrl.length - 1) == '/') {
                baseUrl = baseUrl.substr(0, baseUrl.length - 1); //trim the trailing slash
            }
            var preloadImage = new Image();
            preloadImage.src = baseUrl + "/Style Library/Glyma/Icons/close-window.png";
            preloadImage.src = baseUrl + "/Style Library/Glyma/Icons/close-window-hover.png";
            preloadImage.src = baseUrl + "/Style Library/Glyma/Icons/pop-out-window.png";
            preloadImage.src = baseUrl + "/Style Library/Glyma/Icons/pop-out-window-hover.png";
            preloadImage.src = baseUrl + "/Style Library/Glyma/Icons/show-all-hover.png";
            preloadImage.src = baseUrl + "/Style Library/Glyma/Icons/arrow-hover.png";
            preloadImage.src = baseUrl + "/Style Library/Glyma/Icons/arrow-left-hover.png";
        }

        private initShowAllButton():void {
            var controllerInstance = this;
            //Show All Icon handling
            $("#ShowAllIcon").mouseover(function () {
                $(this).addClass("hover");
                $(this).find(".icon").addClass("hover");
            });

            $("#ShowAllIcon").mouseout(function () {
                $(this).removeClass("hover");
                $(this).find(".icon").removeClass("hover");
            });

            $("#ShowAllIcon").click(function () {
                controllerInstance.showAllPanels();
            });
        }

        public showAllPanels():void {
            var controllerInstance = this;
            $.each(this.contentPanels, function (index, panel) {
                if (panel instanceof RelatedContentPanel) {
                    //only show panels that are enabled
                    if (controllerInstance.maxButtonEnabledStates[panel.getPanelId()] && panel.getHasContent()) {
                        $("#ContentPanels").show();
                        panel.show(true);
                        $("div.maximise-icon[panel-id='" + panel.getPanelId() + "']").show();
                    }
                }
            });
            this.onRelatedContentPanelResize();
        }

        private setPanelsSortable():void {
            // Make the content panels sortable
            $("#ContentPanels").sortable({
                distance: 10,
                handle: "div.related-content-header",
                cursor: "move",
                opactiy: 0.6,
                axis: "y",
                cancel: ".non-sort-item",
                stop: function (event, ui) {
                    $.each(RelatedContentController._instance.contentPanels, function (i, panel) {
                        if (panel instanceof RelatedContentPanel) {
                            panel.sortStopped(ui);
                        }
                    });
                }
            });
        }

        private initExpander():void {
            $("#RelatedContentPanelWrapper").parent().css("float", "right");
            var width = this.EXPANDER_PANEL_WIDTH;
            $("#RelatedContentPanelWrapper").parent().css("width", width + "px");

            $("#Ellipsis").mouseover(function () {
                $(this).addClass("hover");
            });

            $("#Ellipsis").mouseout(function () {
                $(this).removeClass("hover");
            });

            //Sidebar ellipsis click handler
            $("#Ellipsis").click(function () {
                var width: number;
                var fullWidth: number;

                if ($("#IconPanel").is(":visible")) {
                    width = RelatedContentController.getInstance().EXPANDER_PANEL_WIDTH;

                    //hide the icon panel
                    $("#IconPanel").animate({
                        width: 0
                    }, {
                            duration: 200,
                            complete: function () {
                                $("#IconPanel").hide();
                            }
                        });
                    
                    $("#RelatedContentPanel").animate({
                        width: width
                    }, {
                            duration: 200,
                            complete: function () {
                                //needs to be called from within the completed callback
                                RelatedContentController.getInstance().completeEllipsisClick(width);
                            }
                        });

                } else {
                    //show the icon panel
                    width = RelatedContentController.getInstance().EXPANDER_PANEL_WIDTH +
                    RelatedContentController.getInstance().ICON_PANEL_WIDTH;

                    $("#IconPanel").show();
                    $("#IconPanel").animate({
                        width: RelatedContentController.getInstance().ICON_PANEL_WIDTH
                    }, 200);

                    $("#RelatedContentPanel").animate({ width: width },
                    {
                        duration: 200,
                        complete: function () {
                            $("#IconPanel").show();

                            RelatedContentController.getInstance().completeEllipsisClick(width);
                        }
                    });
                }
            });
        }

        private completeEllipsisClick(baseWidth: number):void {
            // move all the content panels to the right by the amount of the expander panel
            $("#ContentPanel").css("right", baseWidth.toString() + "px");

            var fullWidth = baseWidth;
            if (RelatedContentController.getInstance().isAnyPanelVisible()) {
                fullWidth += RelatedContentController.getInstance().getMaxContentWidth() +
                RelatedContentController.getInstance().CONTENT_PANEL_RIGHT_MARGIN +
                RelatedContentController.getInstance().BORDER_WIDTH;
            }

            // Adjust the panel width for web part and minimal page layouts to new width required,
            // it should be plus or minus 70px which is the width of the icon panel
            RelatedContentController.getInstance().adjustPanelDimensions(fullWidth);
        }

        /*
         * Sets some values based on values set in CSS
         */
        private setDefaultsFromCss():void {
            this.ICON_PANEL_WIDTH = parseInt($("#IconPanel").css("width"));
            this.EXPANDER_PANEL_WIDTH = parseInt($("#ExpanderPanel").css("width"));
            this.CONTENT_PANEL_RIGHT_MARGIN = parseInt($("#ContentPanel").css("margin-right"));
            this.DEFAULT_CONTENT_WIDTH = parseInt($("div.related-content-header").css("width"));
            this._maxContentWidth = this.DEFAULT_CONTENT_WIDTH;
        }

        /*
         * Gets the current maximum content panel width
         */
        public getMaxContentWidth(): number {
            return this._maxContentWidth;
        }

        public getWidestPanelsId(): string {
            return this._panelDefiningMaxWidth;
        }

        public setWidestPanelsId(panelId: string): void {
            this._panelDefiningMaxWidth = panelId;
        }

        /*
         * Set the related content panels to be in author mode (all panelss enabled)
         */
        public setAuthorMode():void {
            this.inAuthorMode = true;
            $.each(this.contentPanels, function (index, panel) {
                if (panel instanceof RelatedContentPanel) {
                    panel.enableContentPanel();
                }
            });
            this.resetAndHidePanels();
        }

        /*
         * Get the current state of the Author/Reader mode of the related content panels
         */
        public getAuthorMode(): boolean {
            return this.inAuthorMode;
        }

        /*
         * Set the related content panels to be in reader mode
         */
        public setReaderMode():void {
            this.inAuthorMode = false;
            this.resetAndHidePanels();
        }

        /*
         * Collapses the panels down to just the expander panel.
         * This is the reset state when loading a new map (ie press the Home button on the breadcrumb changing between reader and author modes)
         */
        public resetAndHidePanels():void {
            var controllerInstance:RelatedContentController = this;
            this.setMaxWidthImpl(this.DEFAULT_CONTENT_WIDTH);  //reset the to the default width

            $.each(this.contentPanels, function (i, panel) {
                if (panel instanceof RelatedContentPanel) {
                    panel.reset();
                }
            });

            //collapse the sidebar
            $("#IconPanel").hide();
            $("#ContentPanels").hide();
            $("#RelatedContentPanel").css("width", this.EXPANDER_PANEL_WIDTH + "px");

            // Adjust the panel width for web part and minimal page layouts to new width required,
            // it should be taking it down to just the width of the expander panel since all panels are hidden
            // and the icon panel is collapsed.
            this.adjustPanelDimensions(controllerInstance.EXPANDER_PANEL_WIDTH);
        }

        /*
         * Builds the related content panel HTML and adds the item to the related content panel icon panel.
         * If the specific content panel has already been added it will simply show the icon in the icon panel in the correct state.
         * Params:
         *   panel - The RelatedContentPanel paramaters for the panel to add.
         */
        public addRelatedContentPanel(panel: RelatedContentPanel) {
            if (RelatedContentController._baseUrl === null) {
                throw new Error("The Base URL must be set before use. Use RelatedContentController.setBaseUrl(baseUrl).");
            }

            //only add the panel if it doesn't already exist.
            var existingPanel = this.contentPanels[panel.getPanelId()];
            if (panel != null && existingPanel == null) {
                var controllerInstance: RelatedContentController = this;

                panel.setIconBaseUrls(RelatedContentController.getBaseUrl());

                var maximiseIconPanel = $(this.ICON_PANEL_ID);

                this.addMaximiseIcon(maximiseIconPanel, panel);

                this.contentPanels[panel.getPanelId()] = panel;

                var contentPanelsContainerDiv = $("#ContentPanels");
                var contentPanel = document.createElement("div");
                $(contentPanel).attr("id", panel.getPanelId());
                $(contentPanel).addClass("content-panel"); //css for the default width TODO: Read the div.content-panel width to set the default value
                if (panel.getIsSortable()) {
                    $(contentPanel).addClass("sort-item");
                }
                else {
                    $(contentPanel).addClass("non-sort-item");
                    $(contentPanel).css("cursor", "default"); //default cursor rather than hand
                }

                var panelHeaderDiv = document.createElement("div");
                $(panelHeaderDiv).addClass("related-content-header");

                var iconImg = document.createElement("img");
                $(iconImg).addClass("panel-icon");
                $(iconImg).addClass("unselectable");
                $(iconImg).attr("panel-id", panel.getPanelId());
                $(iconImg).attr("src", panel.getMaximiseIcon());
                $(iconImg).attr("alt", panel.getPanelTitle());
                $(panelHeaderDiv).append(iconImg);

                var titleText = document.createElement("h3");
                $(titleText).addClass("unselectable");
                $(titleText).append(document.createTextNode(panel.getPanelTitle()));
                $(panelHeaderDiv).append(titleText);

                var panelMinCloseSpan = document.createElement("span");
                $(panelMinCloseSpan).addClass("panel-icons");

                var closeIconDiv = document.createElement("div");
                $(closeIconDiv).addClass("close-icon");
                $(closeIconDiv).addClass("unselectable");

                //when the close-icon is clicked close the panel and remove the icon from the side panel
                $(closeIconDiv).click(function () {
                    panel.close(true);
                    $("div.maximise-icon[panel-id='" + panel.getPanelId() + "']").show(); //show the maximise icon in the sidebar
                    controllerInstance.onRelatedContentPanelResize();
                });

                $(panelMinCloseSpan).append(closeIconDiv);
                $(panelHeaderDiv).append(panelMinCloseSpan);
                $(contentPanel).append(panelHeaderDiv);

                var relatedContentBody = document.createElement("div");
                $(relatedContentBody).addClass("related-content-body");
                $(contentPanel).append(relatedContentBody);

                $(relatedContentBody).append(panel.getPanelContent()); //Add the content panels custom content

                $(contentPanelsContainerDiv).append(contentPanel);

                panel.setHasContent(false);

                panel.init();
            }
            if (existingPanel != null) {
                if (existingPanel.getIsDisabled()) {
                    existingPanel.disableContentPanel(false); //this will show the icon but in the disabled state
                }
                else {
                    existingPanel.enableContentPanel(); //will show the icon and enable it
                }
            }
        }

        /*
         * Adds the maximise icon to the related content panel icon panel
         * Params:
         *   iconPanel - The icon panel to add the maximise icon to.
         *   panel - The RelatedContentPanel parameters for the panel that it controls.
         */
        private addMaximiseIcon(iconPanel: JQuery, panel: RelatedContentPanel) {
            var controllerInstance = this;
            var maximiseDiv = document.createElement("div");
            $(maximiseDiv).attr("panel-id", panel.getPanelId());
            $(maximiseDiv).addClass("maximise-icon");

            var iconDiv = document.createElement("div");
            $(iconDiv).addClass("icon");
            this.maxButtonEnabledStates[panel.getPanelId()] = !panel.getIsDisabled();
            this.maxButtonDisabledImages[panel.getPanelId()] = panel.getMaximiseIconDisabled();
            this.maxButtonEnabledImages[panel.getPanelId()] = panel.getMaximiseIcon();

            //Preload the images
            var imageObj = new Image();
            imageObj.src = panel.getMaximiseIcon();
            imageObj.src = panel.getMaximiseIconDisabled();
            imageObj.src = panel.getMaximiseIconHover();

            if (panel.getIsDisabled()) {
                $(maximiseDiv).addClass("disabled");
                $(iconDiv).css("background-image", "url('" + panel.getMaximiseIconDisabled() + "')");
            }
            else {
                $(iconDiv).css("background-image", "url('" + panel.getMaximiseIcon() + "')");
            }
            $(maximiseDiv).append(iconDiv);

            $(maximiseDiv).mouseenter(function () {
                if (controllerInstance.maxButtonEnabledStates[panel.getPanelId()]) {
                    $(this).addClass("hover");
                    $(this).find(".icon").addClass("hover"); //allow for other CSS functionality other than the image change
                    $(this).find(".icon").css("background-image", "url('" + panel.getMaximiseIconHover() + "')");
                }
            });

            $(maximiseDiv).mouseleave(function () {
                if (controllerInstance.maxButtonEnabledStates[panel.getPanelId()]) {
                    $(this).removeClass("hover");
                    $(this).find(".icon").removeClass("hover");
                    $(this).find(".icon").css("background-image", "url('" + panel.getMaximiseIcon() + "')");
                }
            });

            $(maximiseDiv).click(function () {
                if (controllerInstance.maxButtonEnabledStates[panel.getPanelId()]) {
                    $("#ContentPanels").show();
                    if (panel.getIsVisible()) {
                        panel.close(true);
                    }
                    else {
                        panel.show(true);
                    }
                    controllerInstance.onRelatedContentPanelResize();
                }
            });

            iconPanel.append(maximiseDiv);
        }

        /*
         * Called when any related content panel resizes, passes on the resize to other displayed content panels and
         * adjusts the width of the panel width for the web part and minimal page layouts.
         */
        public onRelatedContentPanelResize(): void {
            var width;
            if ($("#IconPanel").is(":visible")) {
                //the sidebar icons are visible
                width = this.EXPANDER_PANEL_WIDTH + this.ICON_PANEL_WIDTH;
            }
            else {
                //the sidebar icons are collapsed
                width = this.EXPANDER_PANEL_WIDTH;
            }
            $("#RelatedContentPanel").css("width", width.toString() + "px");
            $("#ContentPanel").css("right", width.toString() + "px");

            var fullWidth = width;
            if (this.isAnyPanelVisible()) {
                fullWidth += this.getMaxContentWidth() + this.CONTENT_PANEL_RIGHT_MARGIN + this.BORDER_WIDTH;
            }

            // Adjust the panel width for web part and minimal page layouts to new width required,
            // it should be taking it down to just the width of the widest content panel plus the width of either the
            // expander panel or the expander panel and the icon panel depending on it's state
            this.adjustPanelDimensions(fullWidth);

            $.each(this.contentPanels, function (index, relatedContentPanel) {
                if (relatedContentPanel instanceof RelatedContentPanel) {
                    if (relatedContentPanel.getIsVisible()) {
                        //if the panel is visible and it has a size changed callback call it
                        relatedContentPanel.sizeChanged();
                    }
                }
            });
        }

        /*
         * Returns true if any content panels are currently visibile.
         */
        private isAnyPanelVisible(): boolean {
            var result = false;
            $.each(this.contentPanels, function (index, relatedContentPanel) {
                if (relatedContentPanel instanceof RelatedContentPanel) {
                    if (relatedContentPanel.getIsVisible()) {
                        result = true;
                        return false;
                    }
                }
            });
            return result;
        }

        /*
         * Sets the value to be used for the maximum width of all content panels
         */
        public setMaxWidth(width: number, panelId: string): void {
            if (this._panelDefiningMaxWidth == undefined || this._panelDefiningMaxWidth == panelId) {
                if (this._panelDefiningMaxWidth == undefined) {
                    this.setWidestPanelsId(panelId);
                }
                this.setMaxWidthImpl(width);
            }
            else {
                var currentWidth = this.getMaxContentWidth();
                if (width >= currentWidth) {
                    this.setWidestPanelsId(panelId);
                    this.setMaxWidthImpl(width);
                }
            }
        }

        private setMaxWidthImpl(width: number): void {
            this._maxContentWidth = width;
            $("div.related-content-header").css("width", this._maxContentWidth.toString() + "px");

            var baseWidth: number;
            var fullWidth: number;
            if ($("#IconPanel").is(":visible")) {
                //the sidebar icons are visible
                baseWidth = this.EXPANDER_PANEL_WIDTH + this.ICON_PANEL_WIDTH;
            }
            else {
                //the sidebar icons are collapsed
                baseWidth = this.EXPANDER_PANEL_WIDTH;
            }
            fullWidth = baseWidth;
            if (!this.isAnyPanelVisible()) {
                fullWidth += this._maxContentWidth + this.BORDER_WIDTH + this.CONTENT_PANEL_RIGHT_MARGIN;
            }
            this.adjustPanelDimensions(fullWidth);

            $.each(this.contentPanels, function (index, relatedContentPanel) {
                if (relatedContentPanel instanceof RelatedContentPanel) {
                    relatedContentPanel.sizeChanged();
                }
            });
        }

        /*
         * Adjusts the width of the Web Part's container (which is different for the minimal page and regular web part zoned pages)
         * Params:
         *  width: The total width to display the related content panel icon bar and the content panels
         */
        public adjustPanelDimensions(width: number) {
            var contentHeight = $("#ContentPanel").children().height();
            if ($("#ContentPanel").height() > contentHeight) {
                $("#ContentPanel").css("overflow-y", "hidden");
            }
            else {
                $("#ContentPanel").css("overflow-y", "scroll");
                var scrollbarWidth = Utils.CalculateScrollbarWidth(); //tests the width in any browser
                width += scrollbarWidth; //add the width of the scrollbars
            }

            if (this.resizeCallbacks.length != 0) {
                $.each(this.resizeCallbacks, function (index, resizeCallback) {
                    if (resizeCallback != undefined) {
                        resizeCallback(width);
                    }
                });
            }
            else {
                width += this.CONTENT_PANEL_RIGHT_MARGIN;
                $("#RelatedContentPanelWrapper").parent().css("width", width.toString() + "px");
            }
        }

        public expandIconPanel() {
            $("#IconPanel").css("width", this.ICON_PANEL_WIDTH + "px"); //expand the icon sidebar
            $("#IconPanel").show();
        }

        public loadRelatedContent(typeOfInfo, info, param1: any, param2: any): void {
            if (typeOfInfo == "iframeUrl") {
                var pagePanel: PageContentPanel = <PageContentPanel>this.contentPanels["PagePanel"];
                if (pagePanel != null) {
                    pagePanel.loadIframeContent(info, <number>param1, <number>param2);
                }
            }

            else if (typeOfInfo == "nodeHtml") {
                var pagePanel: PageContentPanel = <PageContentPanel>this.contentPanels["PagePanel"];
                if (pagePanel != null) {
                    pagePanel.loadHtmlContent(info, <number>param1, <number>param2);
                }
            }

            else if (typeOfInfo == "gps") {
                var mapPanel: RelatedContentPanel = this.contentPanels["MapPanel"];
                if (mapPanel != null) {
                    mapPanel.expandContentPanel(true);
                    mapPanel.setHasContent(true);
                }
                //TODO: Load a Google map with the map position marker or path drawn
            }

            else if (typeOfInfo == "video") {
                var videoController: VideoController = VideoController.getInstance();
                if (videoController != null) {
                    videoController.loadVideoContent(info, <string>param1);
                }
            }
        }
    }

} 