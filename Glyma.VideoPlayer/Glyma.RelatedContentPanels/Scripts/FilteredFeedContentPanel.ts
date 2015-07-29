/// <reference path="RelatedContentPanel.ts"/>
/// <reference path="listpanel.ts" />
module Glyma.RelatedContentPanels {
    export class FilteredFeedContentPanel extends RelatedContentPanel {
        private static _listPanel: ListPanel = null;
        private static _filters: Glyma.Search.FeedFilter[] = [];

        private static NO_FILTER_MSG: string = "No filters selected, select filter(s) to apply";
        private static NO_FILTERS_AVAILABLE_MSG: string = "There are no filters configured for this map";
        private static CONFIGURE_FILTERS_MSG: string = "Filters are configured as properties of the top-level map; no filters have been configured for this particular top-level map.";

        private static FEED_FILTER_PROPERTY_PREFIX: string = "FeedFilter.";
        private static DISPLAY_NAME_PROPERTY_POSTFIX: string = ".DisplayName";
        private static PROPERTY_NAME_PROPERTY_POSTFIX: string = ".PropertyName";
        private static DEFAULT_VALUE_PROPERTY_POSTFIX: string = ".DefaultValue";
        private static FEED_FILTER_ITEMS_PER_PAGE: string = "FeedFilter.ItemsPerPage";
        private static FEED_FILTER_MAX_LIST_HEIGHT_PROPERTY: string = "FeedFilter.MaximumHeight";
        private static FEED_FILTER_OPERATION_TYPE: string = "FeedFilter.SearchOperationType";
        private static FEED_FILTER_SHOW_SEARCH_OPERATION_OPTIONS: string = "FeedFilter.ShowSearchOperationTypeOptions";
        
        private static FEED_FILTER_NAMES_PROPERTY: string = "FeedFilter.Names";

        private static EMPTY_MESSAGE:string = "No nodes found matching the selected filters";

        private static DEFAULT_MAX_LIST_HEIGHT: number = 300;
        private static DEFAULT_ITEMS_PER_PAGE: number = 8;

        private static NO_FILTERS_AVAILALBE: boolean = false;

        private static _currentSearchOperationType: Glyma.Search.SearchMapOperation = Glyma.Search.SearchMapOperation.OR;

        private static _panelConfig: RelatedContentPanelConfig = {
            Disabled: true,
            PanelId: "FilteredFeedPanel",
            PanelTitle: "INSIGHTS FEED",
            Sortable: true,
            Init: function () {
                FilteredFeedContentPanel._listPanel = new ScrollingListPanel("filteredfeed");
                var scrollingListPanel = <ScrollingListPanel>FilteredFeedContentPanel._listPanel;
                scrollingListPanel.setMaxListHeight(FilteredFeedContentPanel.DEFAULT_MAX_LIST_HEIGHT);
                var filteredFeedContentPanel: FilteredFeedContentPanel = <FilteredFeedContentPanel>this.Panel;
                filteredFeedContentPanel.close(false);
                filteredFeedContentPanel.addFilterHeaderClickHandler();
            },
            Reset: function () {
                var filteredFeedPanel: FilteredFeedContentPanel = <FilteredFeedContentPanel>this.Panel;
                filteredFeedPanel.addFilterHeaderClickHandler();
                filteredFeedPanel.close(true);
                filteredFeedPanel.setHasContent(true);
                $("#filteredfeed").html("");
                FilteredFeedContentPanel.NO_FILTERS_AVAILALBE = false;
                var scrollingListPanel: ScrollingListPanel  = <ScrollingListPanel>filteredFeedPanel.getListPanel();
                scrollingListPanel.setMaxListHeight(FilteredFeedContentPanel.DEFAULT_MAX_LIST_HEIGHT);
                scrollingListPanel.resetPageNumber();
                scrollingListPanel.removeListHeight();
            },
            OnShow: function () {
                try {
                    var filteredFeedPanel: FilteredFeedContentPanel = <FilteredFeedContentPanel>this.Panel;
                    
                    if (filteredFeedPanel != null) {
                        filteredFeedPanel.addFilterHeaderClickHandler();
                        var listPanel: ScrollingListPanel = <ScrollingListPanel>filteredFeedPanel.getListPanel();

                        listPanel.resetListItemWidth();
                        listPanel.resetPageNumber();

                        filteredFeedPanel.doFilteredSearch(function (feed: any[], pageNumber: number, pageSize: number, totalItems: number): void {
                            listPanel.renderFeedList(feed, filteredFeedPanel, FeedUtils.SetFeedListIcon,
                                FilteredFeedContentPanel.SetFilteredFeedListItemDetails, ListPanel.NodeItemClicked, pageSize, totalItems,
                                FilteredFeedContentPanel._filters, FilteredFeedContentPanel.EMPTY_MESSAGE, filteredFeedPanel.filteredFeedNavigationEvent);
                            filteredFeedPanel.expandContentPanel(false);
                            if (pageSize < totalItems) {
                                listPanel.setHasScrollbarsShown(true);
                            }
                            else {
                                listPanel.setHasScrollbarsShown(false);
                            }
                            listPanel.resetListHeight();
                            listPanel.resetListItemWidth();
                        });

                        this.Controller.onRelatedContentPanelResize();
                    }
                    else {
                        filteredFeedPanel.expandContentPanel(false);
                        listPanel.removeListHeight();
                        $("#filteredfeed").html("<p class='content-error-message'>" + FilteredFeedContentPanel.NO_FILTERS_AVAILABLE_MSG + "</p>");
                        filteredFeedPanel.enableContentPanel();
                    }
                }
                catch (err) {
                    this.Panel.showError("Failed to load the insights feed.");
                }
            },
            OnError: function (errorMessage: string) {
                var filteredFeedPanel = <FilteredFeedContentPanel>this.Panel;
                var listPanel = <ScrollingListPanel>filteredFeedPanel.getListPanel();
                listPanel.removeListHeight();
                $("#filteredfeed").html("<p class='content-error-message'>" + errorMessage + "</p>");
            },
            SizeChanged: function () {
                var filteredFeedPanel = <FilteredFeedContentPanel>this.Panel;
                if (filteredFeedPanel != null) {
                    var listPanel: ScrollingListPanel = <ScrollingListPanel>filteredFeedPanel.getListPanel();
                    listPanel.removeListHeight();
                    listPanel.resetListItemWidth();
                    listPanel.resetListHeight();
                }
            },
            Content: "<div class='filter-feeds'>" +
                        "<div class='filters-panel-header'>" +
                            "<span class='filter-expand-indicator'>+</span>" +
                            "<span class='filter-expand-label'>Filters</span>" +
                        "</div>" +
                        "<div class='filters-panel'>" +
                        "</div>" +
                        "<div id='filteredfeed'></div>" +
                     "</div>",
            Icon: "{BASE_URL}/Style Library/Glyma/Icons/insights-feed.png",
            IconHover: "{BASE_URL}/Style Library/Glyma/Icons/insights-feed-hover.png",
            IconDisabled: "{BASE_URL}/Style Library/Glyma/Icons/insights-feed-unavailable.png"
        };

        constructor() {
            super(FilteredFeedContentPanel._panelConfig, null);          
        }

        public getListPanel(): ListPanel {
            return FilteredFeedContentPanel._listPanel;
        }

        public showFeedPanel() {
            var showFilteredFeed = Glyma.MappingTool.MappingToolController.getInstance().GetRootMapMetadataValue("ShowInsightsFeed");
            if (showFilteredFeed != null && showFilteredFeed != "") {
                if (showFilteredFeed.toLowerCase() == "true" || showFilteredFeed == "1") {
                    this.loadFilterOptions();
                    this.showFilteredFeedPanel();
                }
                else if (showFilteredFeed.toLowerCase() == "false" || showFilteredFeed == "0") {
                    this.loadFilterOptions();
                    this.collapseContentPanel(false);
                    this.enableContentPanel();
                    this.setHasContent(true);
                }
            }
            else {
                //default to showing the filtered feed if the root map property ShowFilteredFeed doesn't exist
                this.loadFilterOptions();
                this.showFilteredFeedPanel();
            }
        }

        private static GetListDisplaySize(): number {
            var maxListHeight: number = FilteredFeedContentPanel.DEFAULT_MAX_LIST_HEIGHT;
            var mappingTool: Glyma.MappingTool.MappingToolController = Glyma.MappingTool.MappingToolController.getInstance();
            if (mappingTool != null) {
                var listDisplaySizeProp = mappingTool.GetRootMapMetadataValue(FilteredFeedContentPanel.FEED_FILTER_MAX_LIST_HEIGHT_PROPERTY);
                if (listDisplaySizeProp != null && listDisplaySizeProp != "") {
                    var parsedMaxListHeight: number = parseInt(listDisplaySizeProp, 10);
                    if (parsedMaxListHeight != NaN) {
                        maxListHeight = parsedMaxListHeight;
                    }
                }
            }
            return maxListHeight;
        }

        private static GetListItemsPerPage(): number {
            var itemsPerPage: number = FilteredFeedContentPanel.DEFAULT_ITEMS_PER_PAGE;
            var mappingTool: Glyma.MappingTool.MappingToolController = Glyma.MappingTool.MappingToolController.getInstance();
            if (mappingTool != null) {
                var itemsPerPageProp = mappingTool.GetRootMapMetadataValue(FilteredFeedContentPanel.FEED_FILTER_ITEMS_PER_PAGE);
                if (itemsPerPageProp != null && itemsPerPageProp != "") {
                    var parsedItemsPerPage: number = parseInt(itemsPerPageProp, 10);
                    if (parsedItemsPerPage != NaN) {
                        itemsPerPage = parsedItemsPerPage;
                    }
                }
            }
            return itemsPerPage;
        }

        private static GetShowOperationsTypes(): boolean {
            var showOperationType: boolean = true;
            var mappingTool: Glyma.MappingTool.MappingToolController = Glyma.MappingTool.MappingToolController.getInstance();
            if (mappingTool != null) {
                var showOperationTypeProp = mappingTool.GetRootMapMetadataValue(FilteredFeedContentPanel.FEED_FILTER_SHOW_SEARCH_OPERATION_OPTIONS);
                if (showOperationTypeProp != null && showOperationTypeProp != "") {
                    if (showOperationTypeProp.toLowerCase().trim() == "false") {
                        showOperationType = false;
                    }
                }
            }
            return showOperationType;
        }

        private static GetCurrentSearchOperationType(): Glyma.Search.SearchMapOperation {
            return FilteredFeedContentPanel._currentSearchOperationType;
        }

        private static SetCurrentSearchOperationType(value: Glyma.Search.SearchMapOperation) {
            FilteredFeedContentPanel._currentSearchOperationType = value;
        }

        private static GetDefaultSearchOperationType(): Glyma.Search.SearchMapOperation {
            var searchOperationType = Glyma.Search.SearchMapOperation.OR; //default to OR
            var mappingTool: Glyma.MappingTool.MappingToolController = Glyma.MappingTool.MappingToolController.getInstance();
            if (mappingTool != null) {
                var operationTypeProp = mappingTool.GetRootMapMetadataValue(FilteredFeedContentPanel.FEED_FILTER_OPERATION_TYPE);
                if (operationTypeProp != null && operationTypeProp != "") {
                    if (operationTypeProp.toLowerCase().trim() == "and") {
                        searchOperationType = Glyma.Search.SearchMapOperation.AND;
                    }
                    else if (operationTypeProp.toLowerCase().trim() == "or") {
                        searchOperationType = Glyma.Search.SearchMapOperation.OR;
                    }
                }
            }
            return searchOperationType;
        }

        private showFilteredFeedPanel(): void {
            var listPanel = <ScrollingListPanel>this.getListPanel();
            var panel = this;
            listPanel.resetListItemWidth();
            listPanel.resetPageNumber();

            panel.doFilteredSearch(function (feed: any[], pageNumber: number, pageSize: number, totalItems: number): void {
                listPanel.renderFeedList(feed, panel, FeedUtils.SetFeedListIcon, FilteredFeedContentPanel.SetFilteredFeedListItemDetails,
                    ListPanel.NodeItemClicked, pageSize, totalItems, FilteredFeedContentPanel._filters, FilteredFeedContentPanel.EMPTY_MESSAGE,
                    panel.filteredFeedNavigationEvent);
                panel.expandContentPanel(false);
                if (pageSize < totalItems) {
                    listPanel.setHasScrollbarsShown(true);
                }
                else {
                    listPanel.setHasScrollbarsShown(false);
                }
                listPanel.resetListHeight();
                listPanel.resetListItemWidth();
            });
        }

        private filteredFeedNavigationEvent(): void {
            var mappingTool: Glyma.MappingTool.MappingToolController = Glyma.MappingTool.MappingToolController.getInstance();

            var panel = <FilteredFeedContentPanel>RelatedContentController.getInstance().getContentPanelByName("FilteredFeedPanel");
            var listInstance: ScrollingListPanel = <ScrollingListPanel>FilteredFeedContentPanel._listPanel;

            panel.doFilteredSearch(function (feed: any[], pageNumber: number, pageSize: number, totalItems: number): void {
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
                        FilteredFeedContentPanel.SetFilteredFeedListItemDetails, ListPanel.NodeItemClicked);
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
            });
        }

        private buildFiltersTable(): void {
            var panel = $("#" + this.getPanelId());
            var feedFilters = $(panel).find("div.filters-panel");
            var filtersTable = document.createElement("table");
            $(filtersTable).addClass("filters-table");

            var filters = $(feedFilters).children("div.filter");
            var currentRow = null;
            $.each(filters, function (index, filter) {
                if (index % 3 == 0) {
                    var tableRow = document.createElement("tr");
                    currentRow = tableRow;
                    $(filtersTable).append(currentRow);
                }
                var tableColumn = document.createElement("td");
                $(tableColumn).html(filter);
                $(currentRow).append(tableColumn);
            });
            $(feedFilters).append(filtersTable);
        }

        private addFilterHeaderClickHandler(): void {
            $("div.filters-panel-header").unbind("click");
            $("div.filters-panel-header").click(function () {
                var filteredFeedsPanel = $(this).parent();
                var filterPanel = $(filteredFeedsPanel).children("div.filters-panel");
                var filterExpandIndicator = $(this).children("span.filter-expand-indicator");
                if ($(filterPanel).is(":hidden")) {
                    if (FilteredFeedContentPanel.NO_FILTERS_AVAILALBE) {
                        $(filterPanel).html("<p class='content-error-message'>" + FilteredFeedContentPanel.CONFIGURE_FILTERS_MSG + "</p>");
                    }
                    $(filterExpandIndicator).html("–");
                    $(filterPanel).slideDown();
                }
                else {
                    $(filterExpandIndicator).html("+");
                    $(filterPanel).slideUp();
                }
            });
        }

        private addToFilterOptions(name: string, displayName: string, isChecked: boolean, propertyName: string): void {
            var panel = $("#" + this.getPanelId());
            var feedFilters = $(panel).find("div.filters-panel");

            var filterDiv = document.createElement("div");
            $(filterDiv).addClass("filter");

            var checkboxColumnDiv = document.createElement("div");
            $(checkboxColumnDiv).addClass("filter-column-1");

            var labelColumnDiv = document.createElement("div");
            $(labelColumnDiv).addClass("filter-column-2");

            var filterCheckbox = document.createElement("input");
            $(filterCheckbox).addClass("filter-checkbox");
            $(filterCheckbox).attr("id", name + "cb");
            $(filterCheckbox).attr("type", "checkbox");
            $(filterCheckbox).prop("checked", isChecked);

            var filteredFeedPanel = <FilteredFeedContentPanel>this;
            var controller = RelatedContentController.getInstance();
            $(filterCheckbox).change(function () {
                var checked = this.checked;
                if (checked) {
                    FilteredFeedContentPanel.AddFilter(propertyName);
                }
                else {
                    FilteredFeedContentPanel.RemoveFilter(propertyName);
                }

                if (filteredFeedPanel != null) {
                    var listPanel = <ScrollingListPanel>filteredFeedPanel.getListPanel();
                    listPanel.resetListItemWidth();
                    listPanel.resetPageNumber();

                    filteredFeedPanel.doFilteredSearch(function (feed: any[], pageNumber: number, pageSize: number, totalItems: number): void {
                        listPanel.renderFeedList(feed, filteredFeedPanel, FeedUtils.SetFeedListIcon,
                            FilteredFeedContentPanel.SetFilteredFeedListItemDetails, ListPanel.NodeItemClicked, pageSize, totalItems,
                            FilteredFeedContentPanel._filters, FilteredFeedContentPanel.EMPTY_MESSAGE, filteredFeedPanel.filteredFeedNavigationEvent);
                        filteredFeedPanel.expandContentPanel(false);
                        if (pageSize < totalItems) {
                            listPanel.setHasScrollbarsShown(true);
                        }
                        else {
                            listPanel.setHasScrollbarsShown(false);
                        }
                        listPanel.resetListHeight();
                        listPanel.resetListItemWidth();
                    });
                }

                controller.onRelatedContentPanelResize();
            });
            $(checkboxColumnDiv).append(filterCheckbox);
            $(labelColumnDiv).append(document.createTextNode(displayName));

            $(filterDiv).append(checkboxColumnDiv);
            $(filterDiv).append(labelColumnDiv);
            $(feedFilters).append(filterDiv);
        }

        private doFilteredSearch(completedCallback: (feed: any[], pageNumber: number, pageSize: number, totalItems: number) => void): void {
            var filteredFeedPanel = <FilteredFeedContentPanel>this;
            var controller = RelatedContentController.getInstance();
            var mappingTool: Glyma.MappingTool.MappingToolController = Glyma.MappingTool.MappingToolController.getInstance();
            if (filteredFeedPanel != null && mappingTool != null) {
                var listPanel = <ScrollingListPanel>filteredFeedPanel.getListPanel();

                var domainId = mappingTool.GetCurrentDomainUid();
                var rootMapNodeId = mappingTool.GetCurrentRootMapUid();

                var listDisplaySize: number = FilteredFeedContentPanel.GetListDisplaySize();
                listPanel.setMaxListHeight(listDisplaySize);
                var itemsPerPage: number = FilteredFeedContentPanel.GetListItemsPerPage();
                var searchOperationType: Glyma.Search.SearchMapOperation = FilteredFeedContentPanel.GetCurrentSearchOperationType();

                if (FilteredFeedContentPanel._filters.length > 0) {
                    var nodeSearcher: Glyma.Search.NodeSearcher = new Glyma.Search.NodeSearcher({ BaseUrl: "", ConfigId: Utils.EncodeCallingUrl(window.document.URL) });
                    var searchMapParams: Glyma.Search.SearchMapParameters = {
                        DomainUid: domainId,
                        RootMapUid: rootMapNodeId,
                        Filters: FilteredFeedContentPanel._filters,
                        PageNumber: listPanel.getCurrentPageNumber(),
                        PageSize: itemsPerPage,
                        SortOrder: Glyma.Search.SearchMapSortOrder.ModifiedDescending,
                        SearchOperation: searchOperationType,
                        CompletedCallback: completedCallback,
                        ErrorProcessingCompletedCallback: function (message: string): void {
                            filteredFeedPanel.showError(message);
                        },
                        FailCallback: function (request: string, data: string, status: string): void {
                            filteredFeedPanel.showError("Communication with the Glyma services failed, try again.");
                        }
                    };

                    nodeSearcher.searchMap(searchMapParams);
                }
                else {
                    $("#filteredfeed").html("<p class='content-error-message'>" + FilteredFeedContentPanel.NO_FILTER_MSG + "</p>");
                    filteredFeedPanel.expandContentPanel(false);
                    listPanel.removeListHeight();
                }
            }
        }

        private buildSearchOperationTypeSelector(filtersPanelDiv: JQuery, defaultSearchType: Glyma.Search.SearchMapOperation): void {
            var filteredFeedPanel: FilteredFeedContentPanel = <FilteredFeedContentPanel>this;
            var listPanel: ScrollingListPanel = <ScrollingListPanel>filteredFeedPanel.getListPanel();

            var filterOperationsDiv = document.createElement("div");
            $(filterOperationsDiv).addClass("filters-operation-type");
            $(filterOperationsDiv).append(document.createTextNode("Filtering Type:"));

            var selectEl = document.createElement("select");
            $(selectEl).addClass("filters-operation-type-selector");
            var orOption = document.createElement("option");
            $(orOption).attr("value", "OR");
            $(orOption).append(document.createTextNode("OR"));
            var andOption = document.createElement("option");
            $(andOption).attr("value", "AND");
            $(andOption).append(document.createTextNode("AND"));
            $(selectEl).append(orOption);
            $(selectEl).append(andOption);
            $(filterOperationsDiv).append(selectEl);

            //set the selected value
            switch (defaultSearchType) {
                case Glyma.Search.SearchMapOperation.OR:
                    $(selectEl).val("OR");
                    break;
                case Glyma.Search.SearchMapOperation.AND:
                    $(selectEl).val("AND");
                    break;
            }  

            $(selectEl).change(function () {
                if ($(this).val() == "OR") {
                    FilteredFeedContentPanel.SetCurrentSearchOperationType(Glyma.Search.SearchMapOperation.OR);
                }
                else {
                    FilteredFeedContentPanel.SetCurrentSearchOperationType(Glyma.Search.SearchMapOperation.AND);
                }

                listPanel.resetPageNumber();

                filteredFeedPanel.doFilteredSearch(function (feed: any[], pageNumber: number, pageSize: number, totalItems: number): void {
                    listPanel.renderFeedList(feed, filteredFeedPanel, FeedUtils.SetFeedListIcon,
                        FilteredFeedContentPanel.SetFilteredFeedListItemDetails, ListPanel.NodeItemClicked, pageSize, totalItems,
                        FilteredFeedContentPanel._filters, FilteredFeedContentPanel.EMPTY_MESSAGE, filteredFeedPanel.filteredFeedNavigationEvent);
                    filteredFeedPanel.expandContentPanel(false);
                    if (pageSize < totalItems) {
                        listPanel.setHasScrollbarsShown(true);
                    }
                    else {
                        listPanel.setHasScrollbarsShown(false);
                    }
                    listPanel.resetListHeight();
                    listPanel.resetListItemWidth();
                });
            });

            $(filtersPanelDiv).append(filterOperationsDiv);
        }

        private loadFilterOptions(): void {
            var panel = $("#" + this.getPanelId());
            var feedFilters = $(panel).find("div.filters-panel");
            $(feedFilters).empty();

            var defaultSearchType: Glyma.Search.SearchMapOperation = FilteredFeedContentPanel.GetDefaultSearchOperationType();
            FilteredFeedContentPanel.SetCurrentSearchOperationType(defaultSearchType);
            if (FilteredFeedContentPanel.GetShowOperationsTypes()) {
                this.buildSearchOperationTypeSelector(feedFilters, defaultSearchType);
            }

            FilteredFeedContentPanel._filters = [];

            var mappingTool: Glyma.MappingTool.MappingToolController = Glyma.MappingTool.MappingToolController.getInstance();
            var filteredFeedPanel = this;
            if (mappingTool != null) {
                var feedFilterNames = mappingTool.GetRootMapMetadataValue(FilteredFeedContentPanel.FEED_FILTER_NAMES_PROPERTY);
                if (feedFilterNames != null && feedFilterNames != "") {
                    var filterNames: string[] = feedFilterNames.split(/\s*\,\s*/);
                    if (filterNames.length == 0) {
                        FilteredFeedContentPanel.NO_FILTERS_AVAILALBE = true;
                        this.setHasContent(false);
                    }
                    else {
                        filterNames = filterNames.sort();
                        $.each(filterNames, function (index, name) {
                            name = name.trim();
                            var metadataName = mappingTool.GetRootMapMetadataValue(FilteredFeedContentPanel.FEED_FILTER_PROPERTY_PREFIX + name + FilteredFeedContentPanel.PROPERTY_NAME_PROPERTY_POSTFIX);
                            var displayNameProp = mappingTool.GetRootMapMetadataValue(FilteredFeedContentPanel.FEED_FILTER_PROPERTY_PREFIX + name +  FilteredFeedContentPanel.DISPLAY_NAME_PROPERTY_POSTFIX);
                            var displayName:string = name;
                            if (displayNameProp != null && displayNameProp != "") {
                                displayName = displayNameProp;
                            }
                            if (metadataName != null && metadataName != "") {
                                var defaultValue = mappingTool.GetRootMapMetadataValue(FilteredFeedContentPanel.FEED_FILTER_PROPERTY_PREFIX + name + FilteredFeedContentPanel.DEFAULT_VALUE_PROPERTY_POSTFIX);
                                if (defaultValue != null && defaultValue != "" && defaultValue.toLowerCase() == "true") {
                                    filteredFeedPanel.addToFilterOptions(name, displayName, true, metadataName.trim());
                                    FilteredFeedContentPanel.AddFilter(metadataName.trim());
                                }
                                else {
                                    filteredFeedPanel.addToFilterOptions(name, displayName, false, metadataName.trim());
                                }
                            }
                        });
                        this.buildFiltersTable();
                    }
                }
                else {
                    FilteredFeedContentPanel.NO_FILTERS_AVAILALBE = true;
                    this.setHasContent(false);
                }
            }
        }

        private static AddFilter(name: string): void {
            var filter: Glyma.Search.FeedFilter = {
                Name: name,
                Type: Glyma.Search.FeedFilterType.Boolean,
                Value: true
            };
            if ($.inArray(filter, FilteredFeedContentPanel._filters) == -1) {
                FilteredFeedContentPanel._filters.push(filter);
            }
        }

        private static RemoveFilter(name: string): void {
            FilteredFeedContentPanel._filters = $.grep(FilteredFeedContentPanel._filters, function (value) {
                return value.Name != name;
            });
        }

        private static SetFilteredFeedListItemDetails(item, listItemDiv): void {
            var nodeDetailsDiv = document.createElement("div");
            $(nodeDetailsDiv).addClass("list-item-details");

            var firstLineDiv = document.createElement("div");
            $(firstLineDiv).addClass("first-line");
            var nodeNameText = document.createElement("h4");
            $(nodeNameText).addClass("node-name");
            $(nodeNameText).append(document.createTextNode("'" + item.Name + "'"));
            $(firstLineDiv).append(nodeNameText);

            var mapNameTextLabel = document.createElement("h4");
            $(mapNameTextLabel).addClass("map-label");
            $(mapNameTextLabel).append(document.createTextNode(", in map, "));
            $(firstLineDiv).append(mapNameTextLabel);

            var mapNameText = document.createElement("h4");
            $(mapNameText).addClass("map-name");
            $(mapNameText).append(document.createTextNode("'" + item.Map + "'"));
            $(firstLineDiv).append(mapNameText);

            $(nodeDetailsDiv).append(firstLineDiv);

            $(listItemDiv).append(nodeDetailsDiv);

            FilteredFeedContentPanel.SetListItemCssClass(item, listItemDiv);
        }

        private static SetListItemCssClass(item: any, listItemDiv: HTMLDivElement): void {
            $.each(FilteredFeedContentPanel._filters, function (index:number, filter:Glyma.Search.FeedFilter) {
                if (filter.Type == Glyma.Search.FeedFilterType.Boolean) {
                    $.each(item.Metadata, function (metadataIndex, metadataItem) {
                        if (metadataItem.Key == filter.Name && metadataItem.Value.toLowerCase() == "true") {
                            $(listItemDiv).addClass("gl-filter-" + filter.Name.toLowerCase());
                        }
                    });
                }
            });
        }

    }
}  