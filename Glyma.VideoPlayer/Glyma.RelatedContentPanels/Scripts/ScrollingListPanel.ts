/// <reference path="relatedcontentcontroller.ts" />
/// <reference path="ListPanel.ts" />
module Glyma.RelatedContentPanels {
    export class ScrollingListPanel extends ListPanel {

        private _mutedScrollEvents: boolean = false;

        private _hasScrollBarsShown: boolean = false;

        private _maxHeight: number = -1;

        constructor(listName: string) {
            super(listName);
            $("#" + listName).css("overflow-y", "auto");
        }

        public setMaxListHeight(height: number): void {
            this._maxHeight = height;
        }

        public getMaxListHeight(): number {
            return this._maxHeight;
        }

        public setHasScrollbarsShown(value: boolean): void {
            this._hasScrollBarsShown = value;
        }

        public getHasScrollbarsShown(): boolean {
            return this._hasScrollBarsShown;
        }

        public setScrollEventsMuted(value: boolean): void {
            this._mutedScrollEvents = value;
        }

        public getScrollEventsMuted(): boolean {
            return this._mutedScrollEvents;
        }

        public resetListItemWidth(): void {
            if ($("#" + this.getListName() + " div.list-item").length > 0) {
                var controller = RelatedContentController.getInstance();
                var maxWidth = controller.getMaxContentWidth();
                var scrollbarwidth = 0;
                if (this._hasScrollBarsShown) {
                    scrollbarwidth = Utils.CalculateScrollbarWidth();
                }
                $("#" + this.getListName() + " div.list-item").css("width", maxWidth - scrollbarwidth + "px");
                var hoverIndicatorWidth = $("#" + this.getListName() + " div.list-item-hover-indicator").width();
                var iconWidth = $("#" + this.getListName() + " div.list-item-icon").width() + parseInt($("#" + this.getListName() + " div.list-item-icon").css("marginLeft")) + parseInt($("#" + this.getListName() + " div.list-item-icon").css("marginRight"));
                var arrowWidth = $("#" + this.getListName() + " div.list-item-arrow").width() + parseInt($("#" + this.getListName() + " div.list-item-arrow").css("marginRight"));
                var detailsWidth = $("#" + this.getListName() + " div.list-item").width() - (iconWidth + hoverIndicatorWidth + arrowWidth);
                $("#" + this.getListName() + " div.list-item-details").css("width", detailsWidth + "px");
                $("#" + this.getListName() + " div.list-show-more").css("width", maxWidth - scrollbarwidth + "px");
            }
        }

        public resetListHeight(): void {
            var maxHeight = this.getMaxListHeight();
            var listHeight = $("#" + this.getListName() + " div.list").outerHeight();
            var panelHeight = $("#" + this.getListName()).outerHeight();
            if (this._hasScrollBarsShown) {
                if (listHeight != null && maxHeight != -1 && listHeight > maxHeight) {
                    listHeight = maxHeight;
                }
                if (listHeight == null)
                {
                    listHeight = panelHeight + 5;
                }
                $("#" + this.getListName()).css("height", (listHeight - 5) + "px");
            }
            else {
                //set the height to the list height plus a few pixels that fix display issues in some browsers.
                var listHeight = listHeight + 2;
                if (panelHeight > listHeight) {
                    listHeight = panelHeight;
                }
                if (listHeight > maxHeight) {
                    listHeight = maxHeight;
                    this.setHasScrollbarsShown(true);
                    this.resetListItemWidth();
                }
                $("#" + this.getListName()).css("height", listHeight + "px");
            }
        }

        public removeListHeight(): void {
            $("#" + this.getListName()).css("height", "auto");
        }

        public addToFeedList(listItem: HTMLDivElement) {
            var topItemsDiv = $("#" + this.getListName()).find("div.list-top-items");
            $(topItemsDiv).append(listItem);
        }

        public renderList(listData: any[], panel: RelatedContentPanel, iconCallback: (item: any, element: HTMLDivElement) => void,
            itemDetailsCallback: (item: any, element: HTMLDivElement) => void, clickHandler: (list: ListPanel, item: any) => void,
            pageSize: number, emptyMessage: string): void {

            this.setParentPanel(panel);
            var listName: string = this.getListName();

            $("#" + this.getListName()).html(""); //clear the existing list
            var controllerInstance: RelatedContentController = Glyma.RelatedContentPanels.RelatedContentController.getInstance();
            var listInstance = this;

            if (listData.length > 0) {
                panel.setHasContent(true);
                var list = document.createElement("div");
                $(list).addClass("list");
                var topItemsDiv = document.createElement("div");
                $(topItemsDiv).addClass("list-top-items");
                $(list).append(topItemsDiv);
                var count = 0;
                var pageNumber = 1;
                var currentPageDiv;
                $.each(listData, function (index) {
                    var item = this;

                    if (index == 0) {
                        listInstance.setLastCssClass(ListPanel.EVEN_LIST_ITEM_CLASS);
                    }
                    if (listInstance.getLastCssClass() == ListPanel.ODD_LIST_ITEM_CLASS) {
                        listInstance.setLastCssClass(ListPanel.EVEN_LIST_ITEM_CLASS);
                    }
                    else {
                        listInstance.setLastCssClass(ListPanel.ODD_LIST_ITEM_CLASS);
                    }
                    var cssClass = listInstance.getLastCssClass();

                    var listItemDiv = listInstance.renderListItem(item, cssClass, iconCallback, itemDetailsCallback, clickHandler);
                    
                    if (index < pageSize) {
                        $(topItemsDiv).append(listItemDiv);
                    }
                    else {
                        var pageNum = Math.floor((count) / pageSize);
                        pageNum = pageNum + 1;
                        if (pageNum > pageNumber) {
                            pageNumber = pageNum;
                            var nextPageDiv = document.createElement("div");
                            $(nextPageDiv).addClass("list-page");
                            $(nextPageDiv).attr("page-number", pageNumber);
                            $(list).append(nextPageDiv);
                            currentPageDiv = nextPageDiv;
                            $(nextPageDiv).hide();
                        }
                        if (currentPageDiv != undefined && currentPageDiv != null) {
                            $(currentPageDiv).append(listItemDiv);
                        }
                    }
                    count++;
                });
                $("#" + listInstance.getListName()).append(list);
                var pageCount = pageNumber;
                $(list).attr("page-count", pageCount);

                if (pageCount > 1) {
                    this.setHasScrollbarsShown(true);
                }
                else {
                    this.setHasScrollbarsShown(false);
                }

                var noMoreContentDiv = document.createElement("div");
                $(noMoreContentDiv).addClass("no-more-list-content");
                $(noMoreContentDiv).append(document.createTextNode("end of feed"));
                $(noMoreContentDiv).hide();
                $("#" + listInstance.getListName()).append(noMoreContentDiv);

                $("#" + this.getListName()).scroll(function () {
                    if (!listInstance.getScrollEventsMuted()) {
                        var doc = $(this).find("div.list");
                        var docHeight = $(doc).height();
                        var scrollTopVal = $("#" + listName).scrollTop();
                        var listHeight = $("#" + listName).height();
                        if (scrollTopVal >= (docHeight - listHeight)) {
                            listInstance.setScrollEventsMuted(true);
                            var currentPageNumber = listInstance.getCurrentPageNumber();
                            if (currentPageNumber < pageCount) {
                                listInstance.setCurrentPageNumber(currentPageNumber + 1);
                                $("#" + listInstance.getListName() + " div.list-page[page-number='" + listInstance.getCurrentPageNumber() + "']").show();
                            }
                            else {
                                if (pageNumber == pageCount) {
                                    //$("#" + listInstance.getListName()).find("div.no-more-list-content").show();
                                }
                                listInstance.setScrollEventsMuted(false);
                            }
                        }
                    }
                });

                panel.setHasContent(true);
            }
            else {
                panel.setHasContent(false);
                this.removeListHeight();
                var emptyMessageP = document.createElement("p");
                $(emptyMessageP).addClass("list-empty-message");
                $(emptyMessageP).append(document.createTextNode(emptyMessage));
                $("#" + listInstance.getListName()).append(emptyMessageP);
                this.resetListHeight();
            }

            this.resetListItemWidth();
        }

        public renderFeedList(listData: any[], panel: RelatedContentPanel, iconCallback: (item: any, element: HTMLDivElement) => void,
            itemDetailsCallback: (item: any, element: HTMLDivElement) => void,
            clickHandler: (list: ScrollingListPanel, item: any) => void, pageSize: number, totalItemCount: number, filters: Glyma.Search.FeedFilter[],
            emptyMessage: string, navigateHandler: () => void): void {

            var listName: string = this.getListName();
            var listInstance: ScrollingListPanel = this;
            var controllerInstance: RelatedContentController = RelatedContentController.getInstance();

            //Render the list just like a regular list
            this.renderList(listData, panel, iconCallback, itemDetailsCallback, clickHandler, pageSize, emptyMessage);

            var pageCount = Math.ceil(totalItemCount / pageSize);

            if (pageCount > 1) {
                this.setHasScrollbarsShown(true);
            }
            else {
                this.setHasScrollbarsShown(false);
            }

            //Get the DomainId and RootMapId to use with the activity feed search
            var domainId = Glyma.MappingTool.MappingToolController.getInstance().GetCurrentDomainUid();
            var rootMapNodeId = Glyma.MappingTool.MappingToolController.getInstance().GetCurrentRootMapUid();

            $("#" + this.getListName()).unbind("scroll");
            $("#" + this.getListName()).scroll(function () {
                if (!listInstance.getScrollEventsMuted() && pageCount > 1) {
                    var doc = $(this).find("div.list");
                    var docHeight = $(doc).height();
                    var scrollTopVal = $("#" + listName).scrollTop();
                    var listHeight = $("#" + listName).height();
                    if (scrollTopVal >= (docHeight - listHeight)) {
                        listInstance.setScrollEventsMuted(true);
                        var currentPageNumber = listInstance.getCurrentPageNumber();
                        if (currentPageNumber < pageCount) {
                            listInstance.setCurrentPageNumber(currentPageNumber + 1);

                            navigateHandler();

                            //var nodeSearcher: Glyma.Search.NodeSearcher = new Glyma.Search.NodeSearcher({ BaseUrl: "", ConfigId: Utils.EncodeCallingUrl(window.document.URL) });
                            //var searchMapParams: Glyma.Search.SearchMapParameters = {
                            //    DomainUid: domainId,
                            //    RootMapUid: rootMapNodeId,
                            //    Filters: filters,
                            //    PageNumber: listInstance.getCurrentPageNumber(),
                            //    PageSize: pageSize,
                            //    SortOrder: Glyma.Search.SearchMapSortOrder.ModifiedDescending,
                            //    SearchOperation: Glyma.Search.SearchMapOperation.OR,
                            //    CompletedCallback: function (feed: any[], pageNumber: number, pageSize: number, totalItems: number): void {
                            //        $.each(feed, function (index) {
                            //            var item = this;

                            //            if (listInstance.getLastCssClass() == ListPanel.ODD_LIST_ITEM_CLASS) {
                            //                listInstance.setLastCssClass(ListPanel.EVEN_LIST_ITEM_CLASS);
                            //            }
                            //            else {
                            //                listInstance.setLastCssClass(ListPanel.ODD_LIST_ITEM_CLASS);
                            //            }
                            //            var cssClass = listInstance.getLastCssClass();
                            //            var listItemDiv = listInstance.renderListItem(item, cssClass, iconCallback, itemDetailsCallback, clickHandler);
                            //            listInstance.addToFeedList(listItemDiv);
                            //            if (pageSize < totalItems) {
                            //                listInstance.setHasScrollbarsShown(true);
                            //            }
                            //            else {
                            //                listInstance.setHasScrollbarsShown(false);
                            //            }
                            //            listInstance.resetListItemWidth();
                            //        });
                            //        listInstance.setScrollEventsMuted(false);
                            //    },
                            //    ErrorProcessingCompletedCallback: function (message: string): void {
                            //        panel.showError(message);
                            //        listInstance.setScrollEventsMuted(false);
                            //    },
                            //    FailCallback: function (request: string, data: string, status: string): void {
                            //        panel.showError("Communication with the Glyma services failed, try again.");
                            //        listInstance.setScrollEventsMuted(false);
                            //    }
                            //};
                            //nodeSearcher.searchMap(searchMapParams);
                        }
                        else {
                            if (currentPageNumber == pageCount) {
                                //$("#" + listInstance.getListName()).find("div.no-more-list-content").show();
                            }
                            listInstance.setScrollEventsMuted(false);
                        }
                    }
                }
            });

            RelatedContentController.getInstance().onRelatedContentPanelResize();
        }
    }
}