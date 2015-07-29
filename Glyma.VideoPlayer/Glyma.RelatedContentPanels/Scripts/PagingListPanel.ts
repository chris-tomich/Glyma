/// <reference path="relatedcontentcontroller.ts" />
/// <reference path="ListPanel.ts" />
module Glyma.RelatedContentPanels {
    export class PagingListPanel extends ListPanel {

        constructor(listName: string) {
            super(listName);
        }

        public resetListItemWidth() {
            if ($("#" + this.getListName() + " div.list-item").length > 0) {
                var controller = RelatedContentController.getInstance();
                var maxWidth = controller.getMaxContentWidth();
                $("#" + this.getListName() + " div.list-item").css("width", maxWidth + "px");
                var hoverIndicatorWidth = $("#" + this.getListName() + " div.list-item-hover-indicator").width();
                var iconWidth = $("#" + this.getListName() + " div.list-item-icon").width() + parseInt($("#" + this.getListName() + " div.list-item-icon").css("marginLeft")) + parseInt($("#" + this.getListName() + " div.list-item-icon").css("marginRight"));
                var arrowWidth = $("#" + this.getListName() + " div.list-item-arrow").width() + parseInt($("#" + this.getListName() + " div.list-item-arrow").css("marginRight"));
                var detailsWidth = $("#" + this.getListName() + " div.list-item").width() - (iconWidth + hoverIndicatorWidth + arrowWidth);
                $("#" + this.getListName() + " div.list-item-details").css("width", detailsWidth + "px");
                $("#" + this.getListName() + " div.list-show-more").css("width", maxWidth + "px");
            }
        }

        public renderList(listData: any[], panel: RelatedContentPanel, iconCallback: (item: any, element: HTMLDivElement) => void,
            itemDetailsCallback: (item: any, element: HTMLDivElement) => void, clickHandler: (list: ListPanel, item: any) => void,
            pageSize: number, emptyMessage: string): void {

            this.setParentPanel(panel);

            $("#" + this.getListName()).html(""); //clear the existing list
            var controllerInstance: RelatedContentController = Glyma.RelatedContentPanels.RelatedContentController.getInstance();
            var listInstance: ListPanel = <ListPanel>this;
            var pagingListInstance: PagingListPanel = this;

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

                var showMoreDiv = document.createElement("div");
                $(showMoreDiv).addClass("list-show-more");

                var pagingDiv = document.createElement("div");
                $(pagingDiv).addClass("list-paging");

                var backArrowDiv = document.createElement("div");
                $(backArrowDiv).addClass("list-back-arrow");
                $(backArrowDiv).hide();
                var pageNumberDiv = document.createElement("div");
                $(pageNumberDiv).addClass("list-page-number");
                $(pageNumberDiv).append(document.createTextNode(listInstance.getCurrentPageNumber().toString()));
                var forwardArrowDiv = document.createElement("div");
                $(forwardArrowDiv).addClass("list-forward-arrow");
                pagingListInstance.showArrow(forwardArrowDiv);

                $(pagingDiv).append(backArrowDiv);
                $(pagingDiv).append(pageNumberDiv);
                $(pagingDiv).append(forwardArrowDiv);
                $(showMoreDiv).append(pagingDiv);
                $("#" + listInstance.getListName()).append(showMoreDiv);
                $(showMoreDiv).hide();

                $(backArrowDiv).click(function () {
                    if (listInstance.getCurrentPageNumber() <= 1) {
                        $(backArrowDiv).hide();
                    }
                    var pageNumber = listInstance.getCurrentPageNumber() - 1;
                    if (pageNumber > 0) {
                        listInstance.setCurrentPageNumber(pageNumber);
                    }
                    $(pageNumberDiv).html("");
                    $(pageNumberDiv).append(document.createTextNode(listInstance.getCurrentPageNumber().toString()));
                    if (pageNumber == 1) {
                        $("#" + listInstance.getListName() + " div.list-top-items").show();
                        $("#" + listInstance.getListName() + " div.list-page").hide();
                        $(backArrowDiv).hide();
                        pagingListInstance.showArrow(forwardArrowDiv);
                    }
                    else {
                        $("#" + listInstance.getListName() + " div.list-top-items").hide();
                        $("#" + listInstance.getListName() + " div.list-page").hide();
                        $("#" + listInstance.getListName() + " div.list-page[page-number='" + listInstance.getCurrentPageNumber() + "']").show();
                    }

                    RelatedContentController.getInstance().onRelatedContentPanelResize();
                });

                $(forwardArrowDiv).click(function () {
                    var pageNumber = listInstance.getCurrentPageNumber() + 1;
                    if (pageNumber <= pageCount) {
                        listInstance.setCurrentPageNumber(pageNumber);
                    }
                    $(pageNumberDiv).html("");
                    $(pageNumberDiv).append(document.createTextNode(listInstance.getCurrentPageNumber().toString()));
                    $("#" + listInstance.getListName() + " div.list-top-items").hide();
                    $("#" + listInstance.getListName() + " div.list-page").hide();
                    $("#" + listInstance.getListName() + " div.list-page[page-number='" + listInstance.getCurrentPageNumber() + "']").show();
                    pagingListInstance.showArrow(backArrowDiv);
                    if (pageNumber == parseInt($(list).attr("page-count"))) {
                        $(forwardArrowDiv).hide();
                    }

                    RelatedContentController.getInstance().onRelatedContentPanelResize();
                });

                if (count > pageSize) {
                    $(showMoreDiv).show();
                }

                panel.setHasContent(true);
            }
            else {
                panel.setHasContent(false);
                var emptyMessageP = document.createElement("p");
                $(emptyMessageP).addClass("list-empty-message");
                $(emptyMessageP).append(document.createTextNode(emptyMessage));
                $("#" + listInstance.getListName()).append(emptyMessageP);
            }

            this.resetListItemWidth();

            RelatedContentController.getInstance().onRelatedContentPanelResize();
        }


        public renderFeedList(listData: any[], panel: RelatedContentPanel, iconCallback: (item: any, element: HTMLDivElement) => void,
            itemDetailsCallback: (item: any, element: HTMLDivElement) => void,
            clickHandler: (list: ListPanel, item: any) => void, pageSize: number, totalItemCount: number, filters: Glyma.Search.FeedFilter[],
            emptyMessage: string, navigateHandler: () => void): void {

            var isFilterFeed = filters.length > 0;

            //var emptyMessage = "The activity feed is empty";
            //if (isFilterFeed) {
            //    emptyMessage = "The insights feed is empty";
            //}

            var listName: string = this.getListName();
            var listInstance = this;
            var controllerInstance: RelatedContentController = RelatedContentController.getInstance();
            //Render the list just like a regular list
            this.renderList(listData, panel, iconCallback, itemDetailsCallback, clickHandler, pageSize, emptyMessage);

            var pageCount = Math.ceil(totalItemCount / pageSize);

            //Always show the forward/back buttons since we are always able to request
            var showMoreDiv = $("#" + listName + " div.list-show-more");
            $(showMoreDiv).show();

            //Turn off the default click handlers
            var backArrowDiv = $("#" + listName + " div.list-back-arrow");
            $(backArrowDiv).off("click");
            var forwardArrowDiv = $("#" + listName + " div.list-forward-arrow");
            $(forwardArrowDiv).off("click");

            var pageNumberDiv = $("#" + listName + " div.list-page-number");
            var pageNumber = listInstance.getCurrentPageNumber();
            if (pageNumber <= 1) {
                $(backArrowDiv).hide();
                if (totalItemCount <= pageSize) {
                    $(showMoreDiv).hide(); //1 page or under of data, no need to show the paging controls
                }
            }
            else {
                listInstance.showArrow(backArrowDiv);
            }
            if ((pageNumber * pageSize) >= totalItemCount) {
                $(forwardArrowDiv).hide(); //on the last page so no forward button needed
            }

            //Get the DomainId and RootMapId to use with the activity feed search
            var domainId = Glyma.MappingTool.MappingToolController.getInstance().GetCurrentDomainUid();
            var rootMapNodeId = Glyma.MappingTool.MappingToolController.getInstance().GetCurrentRootMapUid();

            //Override the click handlers
            $(backArrowDiv).click(function () {
                if (listInstance.getCurrentPageNumber() <= 1) {
                    $(backArrowDiv).hide();
                }
                var pageNumber = listInstance.getCurrentPageNumber() - 1;
                if (pageNumber > 0) {
                    listInstance.setCurrentPageNumber(pageNumber);
                }

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
                //    ErrorProcessingCompletedCallback: function (message: string): void {
                //        panel.showError(message);
                //    },
                //    FailCallback: function (request: string, data: string, status: string): void {
                //        panel.showError("Communication with the Glyma services failed, try again.");
                //    }
                //};
                //var emptyMessage = "The activity feed is empty";
                //if (isFilterFeed) {
                //    emptyMessage = "The insights feed is empty";
                //    searchMapParams.CompletedCallback = function (feed: any[], pageNumber: number, pageSize: number, totalItems: number): void {
                //        listInstance.renderFeedList(feed, panel, DomainSearcher.SetFeedListIcon, FilteredFeedContentPanel.SetFilteredFeedListItemDetails, ListPanel.NodeItemClicked, pageSize, totalItems, filters, emptyMessage);
                //    };
                //}
                //else {
                //    searchMapParams.CompletedCallback = function (feedList, pageNumber, pageSize, totalItems) {
                //        listInstance.renderFeedList(feedList, panel, DomainSearcher.SetFeedListIcon, ActivityFeedContentPanel.SetActivityFeedListItemDetails, ListPanel.NodeItemClicked, pageSize, totalItems, filters, emptyMessage);
                //    };
                //}
                //nodeSearcher.searchMap(searchMapParams);
            });
            $(forwardArrowDiv).click(function () {
                var pageNumber = listInstance.getCurrentPageNumber() + 1;
                if (pageNumber <= pageCount) {
                    listInstance.setCurrentPageNumber(pageNumber);
                }

                navigateHandler();

                //var nodeSearcher: Glyma.Search.NodeSearcher = new Glyma.Search.NodeSearcher({ BaseUrl: "", ConfigId: window.document.URL });
                //var searchMapParams: Glyma.Search.SearchMapParameters = {
                //    DomainUid: domainId,
                //    RootMapUid: rootMapNodeId,
                //    Filters: filters,
                //    PageNumber: listInstance.getCurrentPageNumber(),
                //    PageSize: pageSize,
                //    SortOrder: Glyma.Search.SearchMapSortOrder.ModifiedDescending,
                //    SearchOperation: Glyma.Search.SearchMapOperation.OR,
                //    ErrorProcessingCompletedCallback: function (message: string): void {
                //        panel.showError(message);
                //    },
                //    FailCallback: function (request: string, data: string, status: string): void {
                //        panel.showError("Communication with the Glyma services failed, try again.");
                //    }
                //};

                //var emptyMessage = "The activity feed is empty";
                //if (isFilterFeed) {
                //    emptyMessage = "The insights feed is empty";
                //    searchMapParams.CompletedCallback = function (feed: any[], pageNumber: number, pageSize: number, totalItems: number): void {
                //        listInstance.renderFeedList(feed, panel, DomainSearcher.SetFeedListIcon, FilteredFeedContentPanel.SetFilteredFeedListItemDetails, ListPanel.NodeItemClicked, pageSize, totalItems, filters, emptyMessage);
                //    };
                //}
                //else {
                //    searchMapParams.CompletedCallback = function (feedList, pageNumber, pageSize, totalItems) {
                //        listInstance.renderFeedList(feedList, panel, DomainSearcher.SetFeedListIcon, ActivityFeedContentPanel.SetActivityFeedListItemDetails, ListPanel.NodeItemClicked, pageSize, totalItems, filters, emptyMessage);
                //    };
                //}
                //nodeSearcher.searchMap(searchMapParams);
            });

            RelatedContentController.getInstance().onRelatedContentPanelResize();
        }

        private showArrow(arrowElement: any): void {
            $(arrowElement).show();
            $(arrowElement).css("display", "inline-block"); //the showArrow function makes sure the right display value is set
        }

        public resetPageNumber() {
            this.setCurrentPageNumber(1);
        }
    }
}