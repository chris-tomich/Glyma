/// <reference path="relatedcontentcontroller.ts" />
module Glyma.RelatedContentPanels {
    export class ListPanel {
        public static ODD_LIST_ITEM_CLASS: string = "odd-list-item";
        public static EVEN_LIST_ITEM_CLASS: string = "even-list-item";

        private _listName: string;
        private _currentPageNumber: number;
        private _parentPanel: RelatedContentPanel = null;

        private _lastCssClass: string;

        constructor(listName: string) {
            this._listName = listName;
            this._currentPageNumber = 1;
        }

        public getListName(): string {
            return this._listName;
        }

        public getLastCssClass():string {
            return this._lastCssClass;
        }

        public setLastCssClass(value: string):void {
            this._lastCssClass = value;
        }

        public setParentPanel(parentPanel: RelatedContentPanel): void {
            this._parentPanel = parentPanel;
        }

        public getParentPanel(): RelatedContentPanel {
            return this._parentPanel;
        }

        public setCurrentPageNumber(page: number): void {
            this._currentPageNumber = page;
        }

        public getCurrentPageNumber(): number {
            return this._currentPageNumber;
        }

        public renderListItem(listItem, cssClass: string, iconCallback: (item: any, element: HTMLDivElement) => void, itemDetailsCallback: (item: any, element: HTMLDivElement) => void,
            clickHandler: (list: ListPanel, item: any) => void): HTMLDivElement {
            var listInstance = this;
            var listItemDiv = document.createElement("div");
            $(listItemDiv).addClass(cssClass);
            $(listItemDiv).addClass("list-item");

            var hoverIndicator = document.createElement("div");
            $(hoverIndicator).addClass("list-item-hover-indicator");
            $(listItemDiv).append(hoverIndicator);

            var iconDiv = document.createElement("div");
            $(iconDiv).addClass("list-item-icon");
            iconCallback(listItem, iconDiv);
            $(listItemDiv).append(iconDiv);

            //insert the list item details into the list item
            itemDetailsCallback(listItem, listItemDiv);

            var arrowDiv = document.createElement("div");
            $(arrowDiv).addClass("list-item-arrow");
            $(listItemDiv).append(arrowDiv);

            $(listItemDiv).mouseover(function () {
                $(this).addClass("hover");
            });
            $(listItemDiv).mouseout(function () {
                $(this).removeClass("hover");
            });
            $(listItemDiv).click(function () {
                clickHandler(listInstance, listItem);
            });
            return listItemDiv;
        }

        public resetListItemWidth(): void {
        }

        public resetListHeight(): void {
        }

        public renderList(listData: any[], panel: RelatedContentPanel, iconCallback: (item: any, element: HTMLDivElement) => void,
            itemDetailsCallback: (item: any, element: HTMLDivElement) => void, clickHandler: (list: ListPanel, item: any) => void,
            pageSize: number, emptyMessage: string): void {
        }


        public renderFeedList(listData: any[], panel: RelatedContentPanel, iconCallback: (item: any, element: HTMLDivElement) => void,
            itemDetailsCallback: (item: any, element: HTMLDivElement) => void,
            clickHandler: (list: ListPanel, item: any) => void, pageSize: number, totalItemCount: number, filters: Glyma.Search.FeedFilter[],
            emptyMessage: string, navigateHandler: () => void): void {
        }
           

        public resetPageNumber() {
            this.setCurrentPageNumber(1);
        }

        static NodeItemClicked(list: ListPanel, item) {
            list._parentPanel.close(false);
            list._parentPanel.show(true);
            var domainId = item.DomainId;
            var mapNodeId = item.MapNodeId;
            var selectedNode = item.NodeId;
            var rootMapId = item.RootMapId;
            if (selectedNode == rootMapId) {
                selectedNode = null;
            }
            Glyma.MappingTool.MappingToolController.getInstance().LoadMapAndSelectNode(domainId, mapNodeId, selectedNode);
        }
    }
} 