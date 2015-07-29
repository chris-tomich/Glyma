/// <reference path="RelatedContentPanel.ts"/>
/// <reference path="listpanel.ts" />
module Glyma.RelatedContentPanels {
    export class RelatedMapsContentPanel extends RelatedContentPanel {

        private static _listPanel: ScrollingListPanel = null;

        private static _panelConfig: RelatedContentPanelConfig = {
            PanelId: "RelatedNodesPanel",
            PanelTitle: "RELATED MAPS",
            Sortable: true,
            Disabled: true,
            Init: function () {
                this.Panel.close(false);
            },
            Reset: function () {
                $("#relatedmaps").html("");
                var relatedMapsPanel = <RelatedMapsContentPanel>this.Panel;
                this.Panel.close(true);
                this.Panel.disableContentPanel(false);
                this.Panel.setHasContent(false);
                var listPanel = <ScrollingListPanel>relatedMapsPanel.getListPanel();
                if (listPanel != null) {
                    listPanel.resetPageNumber();
                    listPanel.removeListHeight();
                }
            },
            OnShow: function () { 
                if (RelatedMapsContentPanel._listPanel != null) {
                    RelatedMapsContentPanel._listPanel.resetListItemWidth();
                    RelatedMapsContentPanel._listPanel.resetPageNumber();
                    RelatedMapsContentPanel._listPanel.removeListHeight();
                    RelatedMapsContentPanel._listPanel.resetListHeight();
                }
                if ($("#relatedmaps").html() == "") {
                    $("#relatedmaps").css("height", "auto");
                    $("#relatedmaps").html("<p class='list-empty-message'>A transcluded node has not been selected</p>");
                }
            },
            OnError: function (errorMessage: string) {
                var relatedMapsPanel = <RelatedMapsContentPanel>this.Panel;
                var listPanel = <ScrollingListPanel>relatedMapsPanel.getListPanel();
                if (listPanel != null) {
                    listPanel.removeListHeight();
                    listPanel.resetListHeight();
                }
                RelatedMapsContentPanel._listPanel = null;
                $("#relatedmaps").html("<p class='content-error-message'>" + errorMessage + "</p>");
            },
            SizeChanged: function () {
                if (RelatedMapsContentPanel._listPanel != null) {
                    RelatedMapsContentPanel._listPanel.resetListItemWidth();
                    RelatedMapsContentPanel._listPanel.removeListHeight();
                    RelatedMapsContentPanel._listPanel.resetListHeight();
                }
            },
            Content: "<div id='relatedmaps'></div>",
            Icon: "{BASE_URL}/Style Library/Glyma/Icons/maps.png",
            IconHover: "{BASE_URL}/Style Library/Glyma/Icons/maps-hover.png",
            IconDisabled: "{BASE_URL}/Style Library/Glyma/Icons/maps-unavailable.png"
        }

        constructor() {
            super(RelatedMapsContentPanel._panelConfig, null);
        }

        public getListPanel(): ListPanel {
            return RelatedMapsContentPanel._listPanel;
        }

        public showRelatedMaps(maps): void {
            try {
                var mapsList = null;
                if (typeof (maps) === "string") {
                    //if coming from silverlight it's a string
                    mapsList = jQuery.parseJSON(maps);
                }
                else {
                    //if coming from the HTML5 mapping tool it's a json object
                    mapsList = maps;
                }

                //sort the list by map name
                mapsList.sort(function (a, b) {
                    if (a.Name.toLowerCase() < b.Name.toLowerCase()) {
                        return -1;
                    }
                    else if (a.Name.toLowerCase() > b.Name.toLowerCase()) {
                        return 1;
                    }
                    return 0;
                });

                RelatedMapsContentPanel._listPanel = new ScrollingListPanel("relatedmaps");
                RelatedMapsContentPanel._listPanel.setMaxListHeight(155);
                //RelatedMapsContentPanel._listPanel.removeListHeight();
                RelatedMapsContentPanel._listPanel.renderList(mapsList, this, RelatedMapsContentPanel.SetRelatedMapsListIcon, RelatedMapsContentPanel.SetRelatedMapsListItemDetails, ListPanel.NodeItemClicked, 5, "A transcluded node has not been selected");
                this.expandContentPanel(false);
                //RelatedMapsContentPanel._listPanel.resetListHeight();

                RelatedContentController.getInstance().onRelatedContentPanelResize();
            }
            catch (err) {
                this.showError("Failed to load the related maps for this node is transcluded into.");
            }
        }

        public clearRelatedMaps() {
            $("#relatedmaps").html("");
            $("#RelatedNodesPanel").hide();
            RelatedMapsContentPanel._listPanel = null;
            this.disableContentPanel(false);
            RelatedContentController.getInstance().onRelatedContentPanelResize();
        }

        static SetRelatedMapsListIcon(item, iconElement) {
            $(iconElement).addClass("map"); //it's always a map node
        }

        static SetRelatedMapsListItemDetails(item, listItemDiv) {
            var nodeDetailsDiv = document.createElement("div");
            $(nodeDetailsDiv).addClass("list-item-details");

            var firstLineDiv = document.createElement("div");
            $(firstLineDiv).addClass("first-line");
            var nodeNameText = document.createElement("h4");
            $(nodeNameText).append(document.createTextNode(decodeURIComponent(item.Name)));
            $(firstLineDiv).append(nodeNameText);
            $(nodeDetailsDiv).append(firstLineDiv);

            $(listItemDiv).append(nodeDetailsDiv);
        }
    }
} 