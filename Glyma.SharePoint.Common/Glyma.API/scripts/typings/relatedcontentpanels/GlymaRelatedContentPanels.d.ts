declare module Glyma.RelatedContentPanels {
    interface RelatedContentPanelConfig {
        Disabled?: boolean;
        PanelId: string;
        PanelTitle: string;
        Sortable?: boolean;
        Init?: () => void;
        Reset?: () => void;
        OnShow?: () => void;
        OnClose?: () => void;
        OnError?: (errorMessage: string) => void;
        SizeChanged?: () => void;
        SortableStop?: (uiElement: any) => void;
        Content: string;
        Icon: string;
        IconHover: string;
        IconDisabled: string;
        Controller?: RelatedContentController;
        Panel?: RelatedContentPanel;
    }
    class RelatedContentPanel {
        private _config;
        private _hasContent;
        constructor(panelConfig: RelatedContentPanelConfig, transformParams: any);
        private setConfig(config);
        public applyConfigTransform(panelConfig: RelatedContentPanelConfig, transformParams: any): RelatedContentPanelConfig;
        public getPanelId(): string;
        public getPanelTitle(): string;
        public getPanelContent(): string;
        public getIsDisabled(): boolean;
        public show(callOnShow: boolean): void;
        public close(callOnClose: boolean): void;
        public showError(errorMessage: string): void;
        private resetPanelContent();
        public getIsVisible(): boolean;
        public getIsSortable(): boolean;
        public sizeChanged(): void;
        public sortStopped(uiElement: any): void;
        public init(): void;
        public reset(): void;
        public getMaximiseIcon(): string;
        public getMaximiseIconDisabled(): string;
        public getMaximiseIconHover(): string;
        public getHasContent(): boolean;
        public setHasContent(value: boolean): void;
        public enableContentPanel(): void;
        public disableContentPanel(hideMaximiseIcon: boolean): void;
        public expandContentPanel(callOnShow: boolean): void;
        public collapseContentPanel(callOnClose: boolean): void;
        public setIconBaseUrls(baseUrl: string): void;
        public addPopoutLink(callback: any, callbackParams: any): void;
        public removePopoutLink(): void;
    }
}
declare module Glyma.RelatedContentPanels {
    class PageContentPanel extends RelatedContentPanel {
        public pagePanelContent: string;
        public pagePanelWidth: number;
        private static IFRAME_WIDTH_PADDING;
        private static _panelConfig;
        constructor();
        private resetWidthHeightContent();
        private static PopOutIframeWindow(panel, params);
        public loadIframeContent(url: string, width: number, height: number): void;
        private embedIFrame(url, width, height);
        public loadHtmlContent(content: any, width: number, height: number): void;
        private conditionallySetWidth(width, widthSpecified);
        private conditionallySetHeight(height);
    }
}
declare module Glyma.RelatedContentPanels {
    class RelatedContentController {
        private ICON_PANEL_ID;
        private ICON_PANEL_WIDTH;
        private EXPANDER_PANEL_WIDTH;
        private BORDER_WIDTH;
        private CONTENT_PANEL_RIGHT_MARGIN;
        public DEFAULT_CONTENT_WIDTH: number;
        private _maxContentWidth;
        private resizeCallbacks;
        private contentPanels;
        public maxButtonEnabledStates: {
            [key: string]: boolean;
        };
        public maxButtonEnabledImages: {
            [key: string]: string;
        };
        public maxButtonDisabledImages: {
            [key: string]: string;
        };
        public inAuthorMode: boolean;
        private static _instance;
        private static _baseUrl;
        constructor();
        static getInstance(): RelatedContentController;
        public mapLoadCompleted(rootMapLoaded: any): void;
        static onWindowResized(): void;
        static setBaseUrl(baseUrl: string): void;
        static getBaseUrl(): string;
        public getContentPanelByName(name: string): RelatedContentPanel;
        private static preloadImages(baseUrl);
        private initShowAllButton();
        public showAllPanels(): void;
        private setPanelsSortable();
        private initExpander();
        private completeEllipsisClick(baseWidth);
        private setDefaultsFromCss();
        public getMaxContentWidth(): number;
        public setAuthorMode(): void;
        public getAuthorMode(): boolean;
        public setReaderMode(): void;
        public resetAndHidePanels(): void;
        public addRelatedContentPanel(panel: RelatedContentPanel): void;
        private addMaximiseIcon(iconPanel, panel);
        public onRelatedContentPanelResize(): void;
        private isAnyPanelVisible();
        public setMaxWidth(width: number): void;
        public adjustPanelDimensions(width: number): void;
        public expandIconPanel(): void;
        public loadRelatedContent(typeOfInfo: any, info: any, width: number, height: number): void;
    }
}
declare module Glyma.RelatedContentPanels {
    class ListPanel {
        static ODD_LIST_ITEM_CLASS: string;
        static EVEN_LIST_ITEM_CLASS: string;
        private _listName;
        private _currentPageNumber;
        private _parentPanel;
        private _lastCssClass;
        constructor(listName: string);
        public getListName(): string;
        public getLastCssClass(): string;
        public setLastCssClass(value: string): void;
        public setParentPanel(parentPanel: RelatedContentPanel): void;
        public getParentPanel(): RelatedContentPanel;
        public setCurrentPageNumber(page: number): void;
        public getCurrentPageNumber(): number;
        public renderListItem(listItem: any, cssClass: string, iconCallback: (item: any, element: HTMLDivElement) => void, itemDetailsCallback: (item: any, element: HTMLDivElement) => void, clickHandler: (list: ListPanel, item: any) => void): HTMLDivElement;
        public resetListItemWidth(): void;
        public resetListHeight(): void;
        public renderList(listData: any[], panel: RelatedContentPanel, iconCallback: (item: any, element: HTMLDivElement) => void, itemDetailsCallback: (item: any, element: HTMLDivElement) => void, clickHandler: (list: ListPanel, item: any) => void, pageSize: number, emptyMessage: string): void;
        public renderFeedList(listData: any[], panel: RelatedContentPanel, iconCallback: (item: any, element: HTMLDivElement) => void, itemDetailsCallback: (item: any, element: HTMLDivElement) => void, clickHandler: (list: ListPanel, item: any) => void, pageSize: number, totalItemCount: number, filters: Search.FeedFilter[], emptyMessage: string, navigateHandler: () => void): void;
        public resetPageNumber(): void;
        static NodeItemClicked(list: ListPanel, item: any): void;
    }
}
declare module Glyma.RelatedContentPanels {
    class FeedUtils {
        static SetFeedListIcon(item: any, iconElement: any): void;
        static GetActionText(wasCreated: boolean, nodeType: NodeType): {
            [key: string]: string;
        };
        static ProcessUserName(userName: string): string;
    }
}
declare module Glyma.RelatedContentPanels {
    class ActivityFeedContentPanel extends RelatedContentPanel {
        private static _listPanel;
        private static EMPTY_MESSAGE;
        private static _panelConfig;
        constructor();
        public getListPanel(): ListPanel;
        public showFeedPanel(): void;
        private showActivityFeedPanel();
        private activityFeedNavigationEvent();
        private static SetActivityFeedListItemDetails(item, listItemDiv);
    }
}
declare module Glyma.RelatedContentPanels {
    class PagingListPanel extends ListPanel {
        constructor(listName: string);
        public resetListItemWidth(): void;
        public renderList(listData: any[], panel: RelatedContentPanel, iconCallback: (item: any, element: HTMLDivElement) => void, itemDetailsCallback: (item: any, element: HTMLDivElement) => void, clickHandler: (list: ListPanel, item: any) => void, pageSize: number, emptyMessage: string): void;
        public renderFeedList(listData: any[], panel: RelatedContentPanel, iconCallback: (item: any, element: HTMLDivElement) => void, itemDetailsCallback: (item: any, element: HTMLDivElement) => void, clickHandler: (list: ListPanel, item: any) => void, pageSize: number, totalItemCount: number, filters: Search.FeedFilter[], emptyMessage: string, navigateHandler: () => void): void;
        private showArrow(arrowElement);
        public resetPageNumber(): void;
    }
}
declare module Glyma.RelatedContentPanels {
    class ScrollingListPanel extends ListPanel {
        private _mutedScrollEvents;
        private _hasScrollBarsShown;
        private _maxHeight;
        constructor(listName: string);
        public setMaxListHeight(height: number): void;
        public getMaxListHeight(): number;
        public setHasScrollbarsShown(value: boolean): void;
        public getHasScrollbarsShown(): boolean;
        public setScrollEventsMuted(value: boolean): void;
        public getScrollEventsMuted(): boolean;
        public resetListItemWidth(): void;
        public resetListHeight(): void;
        public removeListHeight(): void;
        public addToFeedList(listItem: HTMLDivElement): void;
        public renderList(listData: any[], panel: RelatedContentPanel, iconCallback: (item: any, element: HTMLDivElement) => void, itemDetailsCallback: (item: any, element: HTMLDivElement) => void, clickHandler: (list: ListPanel, item: any) => void, pageSize: number, emptyMessage: string): void;
        public renderFeedList(listData: any[], panel: RelatedContentPanel, iconCallback: (item: any, element: HTMLDivElement) => void, itemDetailsCallback: (item: any, element: HTMLDivElement) => void, clickHandler: (list: ScrollingListPanel, item: any) => void, pageSize: number, totalItemCount: number, filters: Search.FeedFilter[], emptyMessage: string, navigateHandler: () => void): void;
    }
}
declare module Glyma.RelatedContentPanels {
    class FilteredFeedContentPanel extends RelatedContentPanel {
        private static _listPanel;
        private static _filters;
        private static NO_FILTER_MSG;
        private static NO_FILTERS_AVAILABLE_MSG;
        private static CONFIGURE_FILTERS_MSG;
        private static FEED_FILTER_PROPERTY_PREFIX;
        private static DISPLAY_NAME_PROPERTY_POSTFIX;
        private static PROPERTY_NAME_PROPERTY_POSTFIX;
        private static DEFAULT_VALUE_PROPERTY_POSTFIX;
        private static FEED_FILTER_ITEMS_PER_PAGE;
        private static FEED_FILTER_MAX_LIST_HEIGHT_PROPERTY;
        private static FEED_FILTER_OPERATION_TYPE;
        private static FEED_FILTER_DEFAULT_OPERATION_TYPE;
        private static FEED_FILTER_SHOW_SEARCH_OPERATION_OPTIONS;
        private static FEED_FILTER_NAMES_PROPERTY;
        private static EMPTY_MESSAGE;
        private static DEFAULT_MAX_LIST_HEIGHT;
        private static DEFAULT_ITEMS_PER_PAGE;
        private static NO_FILTERS_AVAILALBE;
        private static _currentSearchOperationType;
        private static _panelConfig;
        constructor();
        public getListPanel(): ListPanel;
        public showFeedPanel(): void;
        private static GetListDisplaySize();
        private static GetListItemsPerPage();
        private static GetShowOperationsTypes();
        private static GetCurrentSearchOperationType();
        private static SetCurrentSearchOperationType(value);
        private static GetDefaultSearchOperationType();
        private showFilteredFeedPanel();
        private filteredFeedNavigationEvent();
        private buildFiltersTable();
        private addFilterHeaderClickHandler();
        private addToFilterOptions(name, displayName, isChecked, propertyName);
        private doFilteredSearch(completedCallback);
        private buildSearchOperationTypeSelector(filtersPanelDiv);
        private loadFilterOptions();
        private static AddFilter(name);
        private static RemoveFilter(name);
        private static SetFilteredFeedListItemDetails(item, listItemDiv);
        private static SetListItemCssClass(item, listItemDiv);
    }
}
declare module Glyma.RelatedContentPanels {
    enum VideoPlayerType {
        SILVERLIGHT = 0,
        YOUTUBE = 1,
        HTML5 = 2,
    }
    enum NodeType {
        UnknownType = 0,
        Idea = 1,
        Argument = 2,
        Map = 3,
        Generic = 4,
        Decision = 5,
        Con = 6,
        List = 7,
        Reference = 8,
        Question = 9,
        Note = 10,
        Pro = 11,
    }
    class Utils {
        static EncodeCallingUrl(callingUrl: string): string;
        static GetPlayerType(videoUrl: string): VideoPlayerType;
        static GetYouTubeVideoId(videoUrl: string): any;
        static ConvertToYouTubeEmbedUrl(videoSourceUrl: any): string;
        static ConvertSecondsToTimeSpanString(secondsStr: string): string;
        static ConvertTimeSpanToSeconds(timeSpanString: string): number;
        static QueryString(key: any): any[];
        static GetDateTime(dateString: any): Date;
        private static PadDigits(num, digits);
        static FormatDateString(date: Date): string;
        static CalculateScrollbarWidth(): number;
        static CalculateActualHeight(element: JQuery): number;
        static GetNodeType(nodeTypeUid: string): NodeType;
        static NodeTypeToString(nodeType: NodeType): string;
    }
}
interface String {
    insertAt(index: number, value: string): string;
    trimEnds(chars: string): string;
}
declare module Glyma.RelatedContentPanels {
    class RelatedMapsContentPanel extends RelatedContentPanel {
        private static _listPanel;
        private static _panelConfig;
        constructor();
        public getListPanel(): ListPanel;
        public showRelatedMaps(maps: any): void;
        public clearRelatedMaps(): void;
        static SetRelatedMapsListIcon(item: any, iconElement: any): void;
        static SetRelatedMapsListItemDetails(item: any, listItemDiv: any): void;
    }
}
declare module Glyma.RelatedContentPanels {
    class VideoContentPanel extends RelatedContentPanel {
        private static videoPlayerJSBridge;
        private static _isSilverlightInstalled;
        private static _panelConfig;
        constructor(serverRelativeVersionedLayoutsFolder: string, silverlightInstalled: boolean);
        public applyConfigTransform(panelConfig: RelatedContentPanelConfig, serverRelativeVersionedLayoutsFolder: any): RelatedContentPanelConfig;
        public stopSilverlightVideoPlayer(): void;
        public videoPlayerDisposed(): void;
        static SetVideoPlayerJSBridge(videoPlayerJSBridge: any): void;
        static SendSilverlightVideoPlayerMessage(message: string): void;
        private static RetrySendSilverlightVideoPlayerMessage(message);
    }
}
declare module Glyma.RelatedContentPanels {
    class YouTubeContentPanel extends RelatedContentPanel {
        private static _youTubePlayer;
        private static _youTubePlayerReady;
        private static _youTubePlayerState;
        private static _cachedYouTubeCommand;
        static CurrentNode: {
            Source: any;
            NodeId: string;
            EndTimeCodeProvided: boolean;
            StartTimeCodeProvided: boolean;
            EndTime: number;
            StartTime: number;
        };
        private static _panelConfig;
        constructor();
        private static InjectYouTubeAPI();
        static PopOutIframeWindow(panel: RelatedContentPanel, params: any): void;
        static onYouTubePlayerReady(event: any): void;
        static onYouTubePlayerStateChange(event: any): void;
        static onYouTubePlayerError(event: any): void;
        private static sendYouTubePlayerInitialised();
        static SendYouTubePlayerMessage(xmlMessage: string): void;
        private static HandleYouTubePause();
        private static HandleYouTubePlay(command);
        private static LoadOrCueYouTubeUrl(params);
        private static NotifyYouTubeNodeStopped();
        private static HandleYouTubeStop();
        private static HandleYouTubeGetSourceAndPosition(xmlMessage);
        static GetCurrentVideoUrl(): string;
        private static isApplePortableDevice();
    }
}
declare module Glyma.RelatedContentPanels {
    class VideoController {
        private static _instance;
        static CurrentVideoPlayer: {
            currentPlayerType: any;
        };
        constructor();
        static getInstance(): VideoController;
        static CreateVideoEventXml(eventName: string, eventArgs: any): string;
        static CreateVideoCommandXml(commandName: string, paramArgs: any): string;
        static SendVideoPlayerMessage(message: string): void;
        static GetVideoCommandName(commandXML: string): string;
        static GetVideoCommandParam(commandXML: string, paramName: string): string;
        static loadVideoContent(params: any): void;
    }
}
declare module Glyma.RelatedContentPanels {
    class YammerContentPanel extends RelatedContentPanel {
        private static _panelConfig;
        constructor();
        static IsYammerEnabled(): boolean;
        static SetYammerFeed(): void;
        static ClearYammerFeed(): void;
    }
}
