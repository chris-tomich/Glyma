/// <reference path="RelatedContentPanel.ts"/>
module Glyma.RelatedContentPanels {
    export class PageContentPanel extends RelatedContentPanel {

        public pagePanelContent: string = "";
        public pagePanelWidth: number;

        private static IFRAME_WIDTH_PADDING: number = 4;
        private _muteSizeChangedEvents: boolean = false;
        private _isIFrameContent: boolean = false;

        private static _panelConfig: RelatedContentPanelConfig = {
            PanelId: "PagePanel",
            PanelTitle: "RELATED CONTENT",
            Sortable: true,
            Disabled: true,
            OnShow: function () {
                var pagePanel: PageContentPanel = <PageContentPanel>this.Panel;
                if (pagePanel != null) {
                    $("#relatedcontent").html(pagePanel.pagePanelContent); //reset the last content to the panel

                    pagePanel.setWidth(pagePanel.getContentWidth(), true);
                    
                    if (pagePanel.pagePanelContent == "") {
                        $("#relatedcontent").html("<p class='content-empty-message'>There is no related content available</p>");
                    }
                }
            },
            OnClose: function () {
                var pagePanel: PageContentPanel = <PageContentPanel>this.Panel;
                if (pagePanel != null) {
                    if (pagePanel.isWidestPanel()) {
                        this.Controller.setMaxWidth(this.Controller.DEFAULT_CONTENT_WIDTH, this.PanelId); //reset the max panel width if the large content is closed
                    }
                    pagePanel.pagePanelContent = $("#relatedcontent").html(); //cache what was being displayed before minimised
                }
                $("#relatedcontent").html(""); //forces viewers like Adobe Acrobat to unload themselves
            },
            OnError: function (errorMessage: string) {
                var pagePanel: PageContentPanel = <PageContentPanel>this.Panel;
                if (pagePanel != null) {
                    if (pagePanel.isWidestPanel()) {
                        this.Controller.setMaxWidth(this.Controller.DEFAULT_CONTENT_WIDTH, this.PanelId);
                    }
                    $("#relatedcontent").html("<p class='content-error-message'>" + errorMessage + "</p>");
                    pagePanel.pagePanelContent = ""; //clear the stored panel content
                }
            },
            SizeChanged: function () {
                var pagePanel: PageContentPanel = <PageContentPanel>this.Panel;
                if (pagePanel != null && pagePanel.pagePanelWidth != undefined) {
                    if (!pagePanel._muteSizeChangedEvents) {
                        if (!pagePanel.isWidestPanel()) {
                            var contentsWidth = pagePanel.getContentWidth();
                            var maxWidth = this.Controller.getMaxContentWidth();
                            if (contentsWidth > maxWidth) {
                                pagePanel.setWidth(contentsWidth, true);
                            }
                            else {
                                pagePanel.setWidth(maxWidth, false);
                            }
                        }
                    }
                }
            },
            Init: function () {
                this.Panel.close(false);
            },
            Reset: function () {
                var pagePanel: PageContentPanel = <PageContentPanel>this.Panel;
                if (pagePanel != null) {
                    pagePanel._isIFrameContent = false;
                    pagePanel.pagePanelWidth = this.Controller.DEFAULT_CONTENT_WIDTH;
                    pagePanel.setContentWidth(this.Controller.DEFAULT_CONTENT_WIDTH);
                    pagePanel.removePopoutLink();
                    pagePanel.disableContentPanel(false);
                    pagePanel.resetWidthHeightContent();
                    pagePanel.close(true);
                }
            },
            Content: "<div id='relatedcontent' style='margin-top: 0px;'></div>",
            Icon: "{BASE_URL}/Style Library/Glyma/Icons/related-content.png",
            IconHover: "{BASE_URL}/Style Library/Glyma/Icons/related-content-hover.png",
            IconDisabled: "{BASE_URL}/Style Library/Glyma/Icons/related-content-unavailable.png",
            SortableStop: function (ui) {
                //when the sort finishes if it's the related content (#PagePanel) refresh since adobe can fail its redraw
                if (ui.item.attr("id") == this.PanelId) {
                    ui.item.find("#relatedcontent").html(""); //force unload
                    ui.item.find("#relatedcontent").html(this.Panel.pagePanelContent); //reload content
                }
            }
        }

        constructor() {
            super(PageContentPanel._panelConfig, null);
        }

        private resetWidthHeightContent(): void {
            $("#relatedcontent").css("height", "");
            $("#relatedcontent").css("width", "");
            $("#relatedcontent").html("");
        }

        private setWidth(width: number, storeMax: boolean): void {
            if (this._isIFrameContent) {
                var controller: RelatedContentController = RelatedContentController.getInstance();
                var currentWidestWidth: number = controller.getMaxContentWidth();
                if (!this.isWidestPanel() && width < currentWidestWidth) {
                    width = currentWidestWidth; //override the value with the wider value
                }
                $("#relatedcontent").css("width", width + "px");
                if ($("#RelatedContentIFrame").length > 0) {
                    $("#RelatedContentIFrame").css("width", width + "px");
                }
                if (storeMax) {
                    controller.setMaxWidth(width, this.getPanelId());
                }
            }
            else {
                this.conditionallySetWidth(width, true, storeMax); //this will automatically size to the max if need be
            }
        }

        private static PopOutIframeWindow(panel: RelatedContentPanel, params) {
            var panelEl = $("#" + panel.getPanelId());
            var icons = $(panelEl).find("span.panel-icons");
            var closeIcon = $(icons).find("div.close-icon");
            window.open(params.Url, "_blank");
            $(closeIcon).click();
        }

        public loadIframeContent(url: string, width: number, height: number): void {
            try {
                this._isIFrameContent = true;
                this._muteSizeChangedEvents = true; 
                var controller: RelatedContentController = RelatedContentController.getInstance();
                
                //set defaults first
                this.setContentWidth(controller.DEFAULT_CONTENT_WIDTH);
                this.pagePanelWidth = controller.DEFAULT_CONTENT_WIDTH;
                if (this.isWidestPanel()) {
                    controller.setMaxWidth(this.pagePanelWidth, this.getPanelId());
                }

                //reset any width styling
                $("#relatedcontent").css("width", "");
                $("#relatedcontent").css("height", "");

                if (width != 0 && height != 0) {
                    // A URL has dimensions specified
                    this.embedIFrame(url, width, height);
                }
                else if (width != 0 && height == 0) {
                    // A URL has a width specified but no height
                    this.embedIFrame(url, width, 300);
                }
                else if (width == 0 && height != 0) {
                    // A URL has a height specified but no width
                    this.embedIFrame(url, controller.DEFAULT_CONTENT_WIDTH, height);
                }
                else {
                    // A URL is set without specifying any dimensions
                    this.embedIFrame(url, controller.DEFAULT_CONTENT_WIDTH, 300);
                }
                $("#relatedcontent").css("overflow", "visible");
                $("#relatedcontent").css("overflow-y", "visible");

                this.setHasContent(true);
                this._muteSizeChangedEvents = false; 
                this.expandContentPanel(false);
            }
            catch (err) {
                this._muteSizeChangedEvents = false; 
                this.showError("Failed to load the embedded web page within the panel.");
            }
        }

        private embedIFrame(url: string, width: number, height: number): void {
            var controller: RelatedContentController = RelatedContentController.getInstance();
            var iframeElement = document.createElement("iframe");
            $(iframeElement).attr("id", "RelatedContentIFrame");

            this.setContentWidth(width);
            this.pagePanelWidth = width;// + PageContentPanel.IFRAME_WIDTH_PADDING;

            var currentWidestWidth: number = controller.getMaxContentWidth();
            if (this.pagePanelWidth >= currentWidestWidth) {
                controller.setMaxWidth(this.pagePanelWidth, this.getPanelId());
            }
            else {
                width = currentWidestWidth;// + PageContentPanel.IFRAME_WIDTH_PADDING;
            }

            iframeElement.src = url;
            iframeElement.style.width = width + "px";
            iframeElement.style.height = height + "px";
            $("#relatedcontent").html("");
            $("#relatedcontent").append(iframeElement);
            this.pagePanelContent = $("#relatedcontent").html(); //cache what the content was set to for maximise/minimise purposes

            this.addPopoutLink(PageContentPanel.PopOutIframeWindow, { "Url": url });
        }

        public loadHtmlContent(content: any, width: number, height: number): void {
            try {
                this._isIFrameContent = false;
                this._muteSizeChangedEvents = true; 
                var controller: RelatedContentController = RelatedContentController.getInstance();
                this.removePopoutLink();

                //set defaults first
                this.setContentWidth(controller.DEFAULT_CONTENT_WIDTH);
                this.pagePanelWidth = controller.DEFAULT_CONTENT_WIDTH;
                if (this.isWidestPanel()) {
                    controller.setMaxWidth(this.pagePanelWidth, this.getPanelId());
                }

                //reset any width styling
                $("#relatedcontent").css("width", ""); //clear any previously set width value
                $("#relatedcontent").css("height", ""); //clear any previously set height value

                $("#relatedcontent").css("overflow", "visible");
                $("#relatedcontent").css("overflow-y", "visible");
                $("#relatedcontent").html(content);
                this.pagePanelContent = $("#relatedcontent").html(); //cache what the content was set to for maximise/minimise purposes

                this.expandContentPanel(false);

                if (width != 0) {
                    //The width was specified for the content
                    $("#relatedcontent").css("overflow", "auto");
                    this.conditionallySetWidth(width, true, true);

                    this.conditionallySetHeight(height);
                }
                else {
                    //The width was not specified for the content
                    var actualContentWidth = $("#relatedcontent")[0].scrollWidth;
                    this.conditionallySetWidth(actualContentWidth, false, true);
                    $("#relatedcontent").css("overflow", "auto");

                    this.conditionallySetHeight(height);
                }

                this.setHasContent(true);

                this._muteSizeChangedEvents = false; 
                controller.onRelatedContentPanelResize();
            }
            catch (err) {
                this._muteSizeChangedEvents = false; 
                this.showError("Failed to load the custom text related content within the panel.");
            }
        }

        private conditionallySetWidth(width: number, widthSpecified: boolean, storeMax: boolean): void {
            if (storeMax) {
                this.setContentWidth(width);
                this.pagePanelWidth = width; //store the value that was configured with the metadata
            }

            var controller: RelatedContentController = RelatedContentController.getInstance();
            var currentWidestWidth: number = controller.getMaxContentWidth();
            if (!this.isWidestPanel() && width < currentWidestWidth) {
                width = currentWidestWidth; //override the value with the wider value
            }

            //If these conditions aren't met the default width is going to be used
            if ((widthSpecified && (width >= controller.DEFAULT_CONTENT_WIDTH)) ||
                (!widthSpecified && ((width >= controller.DEFAULT_CONTENT_WIDTH) && (width <= (2 * controller.DEFAULT_CONTENT_WIDTH))))) {
                //If the widthSpecified == false it ensures the content's natural width isn't more than twice the default width, 
                //if the user wants wide content they should specify a width rather than allowing it to be determined automatically
                if (this.getIsVisible() && this.pagePanelWidth == width && storeMax) {
                    controller.setMaxWidth(width, this.getPanelId());
                }
                //this.pagePanelWidth = width;
                $("#relatedcontent").css("width", width + "px");
            }
        }

        private conditionallySetHeight(height: number): void {
            var actualContentHeight: number = Utils.CalculateActualHeight($("#relatedcontent"));
            if (actualContentHeight <= height || height == 0) {
                //Hide the vertical scroll bars if the content is shorter than the height we are setting
                $("#relatedcontent").css("overflow-y", "hidden");
            }
            if (height != 0) {
                //A height was speficied so the height should be limited to that
                $("#relatedcontent").css("height", height.toString() + "px");
            }
        }
    }
}