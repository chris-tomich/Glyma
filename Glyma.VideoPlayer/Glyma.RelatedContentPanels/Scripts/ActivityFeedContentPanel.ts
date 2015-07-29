/// <reference path="listpanel.ts" />
/// <reference path="RelatedContentPanel.ts"/>
/// <reference path="FeedUtils.ts"/>
module Glyma.RelatedContentPanels {
    export class ActivityFeedContentPanel extends RelatedContentPanel {

        private static _listPanel: ListPanel = null;

        private static EMPTY_MESSAGE:string = "The activity feed is empty";

        private static _panelConfig: RelatedContentPanelConfig = {
            Disabled: true,
            PanelId: "ActivityFeedPanel",
            PanelTitle: "ACTIVITY FEED",
            Sortable: true,
            Init: function () {
                ActivityFeedContentPanel._listPanel = new ScrollingListPanel("activityfeed");
                var scrollingListPanel = <ScrollingListPanel>ActivityFeedContentPanel._listPanel;
                scrollingListPanel.setMaxListHeight(250);
                this.Panel.close(false);
                ActivityFeedContentPanel._listPanel.resetPageNumber();
            },
            Reset: function () {
                var activityFeedPanel = <ActivityFeedContentPanel>this.Panel;
                this.Panel.close(true);
                this.Panel.setHasContent(true);
                $("#activityfeed").html("");
                var listPanel = <ScrollingListPanel>activityFeedPanel.getListPanel();
                listPanel.resetPageNumber();
                listPanel.removeListHeight();
            },
            OnShow: function () {
                try {
                    var activityFeedPanel = <ActivityFeedContentPanel>this.Panel;
                    var mappingTool: Glyma.MappingTool.MappingToolController = Glyma.MappingTool.MappingToolController.getInstance();
                    if (activityFeedPanel != null && mappingTool != null) {
                        var listPanel = <ScrollingListPanel>activityFeedPanel.getListPanel();
                        listPanel.resetListItemWidth();
                        listPanel.resetPageNumber();

                        var domainId:string = mappingTool.GetCurrentDomainUid();
                        var rootMapNodeId:string = mappingTool.GetCurrentRootMapUid();
                        var feedFilters: Glyma.Search.FeedFilter[] = [];
                        var nodeSearcher: Glyma.Search.NodeSearcher = new Glyma.Search.NodeSearcher({ BaseUrl: "", ConfigId: Utils.EncodeCallingUrl(window.document.URL) });
                        var searchMapParams: Glyma.Search.SearchMapParameters = {
                            DomainUid: domainId,
                            RootMapUid: rootMapNodeId,
                            Filters: feedFilters,
                            PageNumber: 1,
                            PageSize: 5,
                            SortOrder: Glyma.Search.SearchMapSortOrder.ModifiedDescending,
                            SearchOperation: Glyma.Search.SearchMapOperation.OR,
                            CompletedCallback: function (feed: any[], pageNumber: number, pageSize: number, totalItems: number): void {
                                listPanel.renderFeedList(feed, activityFeedPanel, FeedUtils.SetFeedListIcon,
                                    ActivityFeedContentPanel.SetActivityFeedListItemDetails, ListPanel.NodeItemClicked,
                                    pageSize, totalItems, feedFilters, ActivityFeedContentPanel.EMPTY_MESSAGE, activityFeedPanel.activityFeedNavigationEvent);
                                activityFeedPanel.expandContentPanel(false);
                                if (pageSize < totalItems) {
                                    listPanel.setHasScrollbarsShown(true);
                                }
                                else {
                                    listPanel.setHasScrollbarsShown(false);
                                }
                                listPanel.resetListHeight();
                                listPanel.resetListItemWidth();
                            },
                            ErrorProcessingCompletedCallback: function (message: string): void {
                                activityFeedPanel.showError(message);
                            },
                            FailCallback: function (request: string, data: string, status: string): void {
                                activityFeedPanel.showError("Communication with the Glyma services failed, try again.");
                            }
                        };

                        nodeSearcher.searchMap(searchMapParams);
                    }

                    this.Controller.onRelatedContentPanelResize();
                }
                catch (err) {
                    this.Panel.showError("Failed to load the activity feed.");
                }
            },
            OnError: function (errorMessage: string) {
                var activityFeedPanel = <ActivityFeedContentPanel>this.Panel;
                var listPanel = <ScrollingListPanel>activityFeedPanel.getListPanel();
                listPanel.resetListHeight();
                $("#activityfeed").html("<p class='content-error-message'>" + errorMessage + "</p>");
            },
            SizeChanged: function () {
                var activityFeedPanel = <ActivityFeedContentPanel>this.Panel;
                if (activityFeedPanel != null) {
                    var listPanel: ScrollingListPanel = <ScrollingListPanel>activityFeedPanel.getListPanel();
                    listPanel.removeListHeight();
                    listPanel.resetListItemWidth();
                    listPanel.resetListHeight();
                }
            },
            Content: "<div id='activityfeed'></div>",
            Icon: "{BASE_URL}/Style Library/Glyma/Icons/activity-feed.png",
            IconHover: "{BASE_URL}/Style Library/Glyma/Icons/activity-feed-hover.png",
            IconDisabled: "{BASE_URL}/Style Library/Glyma/Icons/activity-feed-unavailable.png"
        };

        constructor() {
            super(ActivityFeedContentPanel._panelConfig, null);
        }

        public getListPanel(): ListPanel {
            return ActivityFeedContentPanel._listPanel;
        }

        public showFeedPanel() {
            var showActivityFeed = Glyma.MappingTool.MappingToolController.getInstance().GetRootMapMetadataValue("ShowActivityFeed");
            if (showActivityFeed != null && showActivityFeed != "") {
                if (showActivityFeed.toLowerCase() == "true" || showActivityFeed == "1") {
                    this.showActivityFeedPanel();
                }
                else if (showActivityFeed.toLowerCase() == "false" || showActivityFeed == "0") {
                    this.collapseContentPanel(false);
                    this.enableContentPanel();
                    this.setHasContent(true);
                }
            }
            else {
                //default to showing the activity feed if the root map property ShowActivityFeed doesn't exist
                this.showActivityFeedPanel();
            }
        }

        private showActivityFeedPanel() {
            var mappingTool: Glyma.MappingTool.MappingToolController = Glyma.MappingTool.MappingToolController.getInstance();
            if (mappingTool != null) {
                var listPanel = <ScrollingListPanel>this.getListPanel();
                var panel = this;
                listPanel.resetListItemWidth();
                listPanel.resetPageNumber();

                var domainId = mappingTool.GetCurrentDomainUid();
                var rootMapNodeId = mappingTool.GetCurrentRootMapUid();
                var feedFilters: Glyma.Search.FeedFilter[] = [];

                var nodeSearcher: Glyma.Search.NodeSearcher = new Glyma.Search.NodeSearcher({ BaseUrl: "", ConfigId: Utils.EncodeCallingUrl(window.document.URL) });
                var searchMapParams: Glyma.Search.SearchMapParameters = {
                    DomainUid: domainId,
                    RootMapUid: rootMapNodeId,
                    Filters: feedFilters,
                    PageNumber: 1,
                    PageSize: 5,
                    SortOrder: Glyma.Search.SearchMapSortOrder.ModifiedDescending,
                    SearchOperation: Glyma.Search.SearchMapOperation.OR,
                    CompletedCallback: function (feed: any[], pageNumber: number, pageSize: number, totalItems: number): void {
                        listPanel.renderFeedList(feed, panel, FeedUtils.SetFeedListIcon, ActivityFeedContentPanel.SetActivityFeedListItemDetails,
                            ListPanel.NodeItemClicked, pageSize, totalItems, feedFilters, ActivityFeedContentPanel.EMPTY_MESSAGE, panel.activityFeedNavigationEvent);
                        panel.expandContentPanel(false);
                        if (pageSize < totalItems) {
                            listPanel.setHasScrollbarsShown(true);
                        }
                        else {
                            listPanel.setHasScrollbarsShown(false);
                        }
                        listPanel.resetListHeight();
                        listPanel.resetListItemWidth();
                    },
                    ErrorProcessingCompletedCallback: function (message: string): void {
                        panel.showError(message);
                    },
                    FailCallback: function (request: string, data: string, status: string): void {
                        panel.showError("Communication with the Glyma services failed, try again.");
                    }
                };
                nodeSearcher.searchMap(searchMapParams);
            }
        }

        private activityFeedNavigationEvent():void {
            var mappingTool: Glyma.MappingTool.MappingToolController = Glyma.MappingTool.MappingToolController.getInstance();

            var domainId = mappingTool.GetCurrentDomainUid();
            var rootMapNodeId = mappingTool.GetCurrentRootMapUid();

            var listInstance: ScrollingListPanel = <ScrollingListPanel>ActivityFeedContentPanel._listPanel
            var panel = <ActivityFeedContentPanel>RelatedContentController.getInstance().getContentPanelByName("ActivityFeedPanel");
            var feedFilters: Glyma.Search.FeedFilter[] = [];

            var nodeSearcher: Glyma.Search.NodeSearcher = new Glyma.Search.NodeSearcher({ BaseUrl: "", ConfigId: Utils.EncodeCallingUrl(window.document.URL) });
            var searchMapParams: Glyma.Search.SearchMapParameters = {
                DomainUid: domainId,
                RootMapUid: rootMapNodeId,
                Filters: feedFilters,
                PageNumber: listInstance.getCurrentPageNumber(),
                PageSize: 5,
                SortOrder: Glyma.Search.SearchMapSortOrder.ModifiedDescending,
                SearchOperation: Glyma.Search.SearchMapOperation.OR,
                CompletedCallback: function (feed: any[], pageNumber: number, pageSize: number, totalItems: number): void {
                    $.each(feed, function (index) {
                        var item = this;

                        if (listInstance.getLastCssClass() == ListPanel.ODD_LIST_ITEM_CLASS) {
                            listInstance.setLastCssClass(ListPanel.EVEN_LIST_ITEM_CLASS);
                        }
                        else {
                            listInstance.setLastCssClass(ListPanel.ODD_LIST_ITEM_CLASS);
                        }
                        var cssClass = listInstance.getLastCssClass();
                        var listItemDiv = listInstance.renderListItem(item, cssClass, FeedUtils.SetFeedListIcon,
                            ActivityFeedContentPanel.SetActivityFeedListItemDetails, ListPanel.NodeItemClicked);
                        listInstance.addToFeedList(listItemDiv);
                        if (pageSize < totalItems) {
                            listInstance.setHasScrollbarsShown(true);
                        }
                        else {
                            listInstance.setHasScrollbarsShown(false);
                        }
                        listInstance.resetListItemWidth();
                    });
                    listInstance.setScrollEventsMuted(false);
                },
                ErrorProcessingCompletedCallback: function (message: string): void {
                    panel.showError(message);
                    listInstance.setScrollEventsMuted(false);
                },
                FailCallback: function (request: string, data: string, status: string): void {
                    panel.showError("Communication with the Glyma services failed, try again.");
                    listInstance.setScrollEventsMuted(false);
                }
            };
            nodeSearcher.searchMap(searchMapParams);
        }

        private static SetActivityFeedListItemDetails(item, listItemDiv) {
            var nodeDetailsDiv = document.createElement("div");
            $(nodeDetailsDiv).addClass("list-item-details");

            var firstLineDiv = document.createElement("div");
            $(firstLineDiv).addClass("first-line");
            var nodeNameText = document.createElement("h4");
            $(nodeNameText).addClass("node-name");
            $(nodeNameText).append(document.createTextNode("'" + item.Name + "'"));

            var mapNameTextLabel = document.createElement("h4");
            $(mapNameTextLabel).addClass("map-label");
            var mapNameText = document.createElement("h4");
            $(mapNameText).addClass("map-name");
            $(mapNameText).append(document.createTextNode("'" + item.Map + "'"));

            var actionStartText = document.createElement("h4");
            $(actionStartText).addClass("action-start");
            var actionEndText = document.createElement("h4");
            $(actionEndText).addClass("action-end");
            var userNameText = document.createElement("h4");
            $(userNameText).addClass("user-display-name");
            var displayName;
            var wasCreated = false;
            if (item.ModifiedDateObj.format("yyyyMMddhhmmss") == item.CreatedDateObj.format("yyyyMMddhhmmss")) {
                wasCreated = true;
                displayName = item.CreatedBy;
            }
            else {
                wasCreated = false;
                displayName = item.ModifiedBy;
            }
            if (displayName != "") {
                displayName = FeedUtils.ProcessUserName(displayName);
                $(userNameText).append(document.createTextNode(displayName))
                $(firstLineDiv).append(userNameText);
                var actionText = FeedUtils.GetActionText(wasCreated, item.NodeType);
                $(actionStartText).append(document.createTextNode(actionText["start"]));
                $(firstLineDiv).append(actionStartText);
                $(firstLineDiv).append(nodeNameText);
                $(actionEndText).append(document.createTextNode(actionText["end"]));
                $(firstLineDiv).append(actionEndText);

                $(mapNameTextLabel).append(document.createTextNode(" map, "));
                $(firstLineDiv).append(mapNameTextLabel);
                $(firstLineDiv).append(mapNameText);
                if (item.Map == "") {
                    //TODO: Fix bug on server that returns the Map name as an empty string and remove this if block
                    $(mapNameText).hide();
                    $(actionEndText).hide();
                    $(mapNameTextLabel).hide();
                }

                $(nodeDetailsDiv).append(firstLineDiv);
            } else {
                $(firstLineDiv).append(nodeNameText);
                $(mapNameTextLabel).append(document.createTextNode(" in map, "));
                $(firstLineDiv).append(mapNameTextLabel);
                $(firstLineDiv).append(mapNameText);
                $(nodeDetailsDiv).append(firstLineDiv);
            }

            var updatedDiv = document.createElement("div");
            $(updatedDiv).addClass("second-line");
            var updatedText = document.createElement("h4");
            $(updatedText).append(document.createTextNode("Updated " + item.Modified));
            $(updatedDiv).append(updatedText);
            $(nodeDetailsDiv).append(updatedDiv);

            $(listItemDiv).append(nodeDetailsDiv);
        }
    }
} 