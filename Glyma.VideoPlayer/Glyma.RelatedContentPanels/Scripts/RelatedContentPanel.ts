/// <reference path="relatedcontentcontroller.ts" />
module Glyma.RelatedContentPanels {
    export interface RelatedContentPanelConfig {
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

    export class RelatedContentPanel {
        private _config: RelatedContentPanelConfig;
        private _hasContent: boolean = false;
        private _contentWidth: number;

        constructor(panelConfig: RelatedContentPanelConfig, transformParams: any) {
            var defaultConfig = {
                Disabled: false,
                PanelId: "",
                PanelTitle: "",
                Content: "",
                Icon: "",
                IconHover: "",
                IconDisabled: "",
                Sortable: true,
                Init: function () { },
                Reset: function () { },
                OnShow: function () { },
                OnClose: function () { },
                OnError: function (errorMessage) { },
                SizeChanged: function () { },
                SortableStop: function (uiElement) { }
            };
            this._config = defaultConfig;
            var transformedConfig = this.applyConfigTransform(panelConfig, transformParams);
            this.setConfig(transformedConfig);
        }

        private setConfig(config: RelatedContentPanelConfig) {
            this._config.Panel = this;
            this._config.Controller = RelatedContentController.getInstance();
            this._config.PanelId = config.PanelId;
            this._config.PanelTitle = config.PanelTitle;
            this._config.Content = config.Content;
            this._config.Icon = config.Icon;
            this._config.IconDisabled = config.IconDisabled;
            this._config.IconHover = config.IconHover;
            if (config.Disabled) {
                this._config.Disabled = config.Disabled;
            }
            if (config.Init) {
                this._config.Init = config.Init;
            }
            if (config.Reset) {
                this._config.Reset = config.Reset;
            }
            if (config.OnShow) {
                this._config.OnShow = config.OnShow;
            }
            if (config.OnClose) {
                this._config.OnClose = config.OnClose;
            }
            if (config.OnError) {
                this._config.OnError = config.OnError;
            }
            if (config.SizeChanged) {
                this._config.SizeChanged = config.SizeChanged;
            }
            if (config.Sortable) {
                this._config.Sortable = config.Sortable;

                //The SortableStop callback only makes sense on sortable panels
                if (config.SortableStop) {
                    this._config.SortableStop = config.SortableStop;
                }
            }
        }

        public applyConfigTransform(panelConfig: RelatedContentPanelConfig, transformParams: any): RelatedContentPanelConfig {
            return panelConfig;  //default is for no transformation to occur
        }

        public getPanelId(): string {
            return this._config.PanelId;
        }

        public getPanelTitle(): string {
            return this._config.PanelTitle;
        }

        public getPanelContent(): string {
            return this._config.Content;
        }

        public getIsDisabled(): boolean {
            return this._config.Disabled;
        }

        public getContentHasWidth(): boolean {
            return this._contentWidth != undefined;
        }

        public getContentWidth(): number {
            return this._contentWidth;
        }

        public setContentWidth(width: number): void {
            this._contentWidth = width;
        }

        public show(callOnShow: boolean): void {
            $("#" + this._config.PanelId).show();
            if (this._config.OnShow && callOnShow) {
                this._config.OnShow();
            }
        }

        public close(callOnClose: boolean): void {
            $("#" + this._config.PanelId).hide();
            
            if (this._config.OnClose && callOnClose) {
                try {
                    this._config.OnClose();
                } catch (err) {
                }
            }

            this.removeAsWidestPanel();
        }

        private removeAsWidestPanel() {
            var controller: RelatedContentController = RelatedContentController.getInstance();
            if (controller != null) {
                if (this.isWidestPanel()) {
                    controller.setWidestPanelsId(undefined);
                }
            }
        }

        public showError(errorMessage: string): void {
            if (this._config.OnError) {
                //reset the content panel to it's default contents
                this.resetPanelContent();

                //allow the panel to display the error message
                this._config.OnError(errorMessage);

                this.setHasContent(true);

                this.expandContentPanel(false);
            }
        }

        private resetPanelContent(): void {
            var panel = $("#" + this.getPanelId());

            //reset the content panel to it's default contents
            var relatedContentBody = $(panel).find("div.related-content-body");
            $(relatedContentBody).empty();
            $(relatedContentBody).append(this.getPanelContent());
            this.setHasContent(false);
        }

        public getIsVisible(): boolean {
            if ($("#" + this._config.PanelId).is(":visible")) {
                return true;
            }
            else {
                return false;
            }
        }

        public getIsSortable(): boolean {
            return this._config.Sortable;
        }

        public sizeChanged(): void {
            if (this._config.SizeChanged) {
                this._config.SizeChanged();
            }
        }

        public sortStopped(uiElement: any): void {
            if (this._config.SortableStop) {
                this._config.SortableStop(uiElement);
            }
        }

        public init(): void {
            if (this._config.Init) {
                this._config.Init();
            }
        }

        public reset(): void {
            if (this._config.Reset) {
                this._config.Reset();
            }
        }

        public getMaximiseIcon(): string {
            return this._config.Icon;
        }

        public getMaximiseIconDisabled(): string {
            return this._config.IconDisabled;
        }

        public getMaximiseIconHover(): string {
            return this._config.IconHover;
        }

        public getHasContent(): boolean {
            return this._hasContent;
        }

        public setHasContent(value: boolean) {
            this._hasContent = value;
        }

        public isWidestPanel(): boolean {
            var controller: RelatedContentController = RelatedContentController.getInstance();
            var widestPanelId = controller.getWidestPanelsId();
            if (widestPanelId == this._config.PanelId) {
                return true;
            }
            else {
                return false;
            }
        }

        public enableContentPanel(): void {
            var controller: RelatedContentController = RelatedContentController.getInstance();
            controller.maxButtonEnabledStates[this.getPanelId()] = true;
            $("div.maximise-icon[panel-id='" + this.getPanelId() + "']").removeClass("disabled");
            $("div.maximise-icon[panel-id='" + this.getPanelId() + "']").find(".icon").css("background-image", "url('" + controller.maxButtonEnabledImages[this.getPanelId()] + "')");
            $("div.maximise-icon[panel-id='" + this.getPanelId() + "']").show(); //make sure it is shown
        }

        public disableContentPanel(hideMaximiseIcon: boolean): void {
            var controller: RelatedContentController = RelatedContentController.getInstance();
            if (!controller.inAuthorMode) {
                controller.maxButtonEnabledStates[this.getPanelId()] = false;
                $("div.maximise-icon[panel-id='" + this.getPanelId() + "']").addClass("disabled");
                $("div.maximise-icon[panel-id='" + this.getPanelId() + "']").find(".icon").css("background-image", "url('" + controller.maxButtonDisabledImages[this.getPanelId()] + "')");
                if (hideMaximiseIcon) {
                    $("div.maximise-icon[panel-id='" + this.getPanelId() + "']").hide(); //hide the icon if wanted
                }
                else {
                    $("div.maximise-icon[panel-id='" + this.getPanelId() + "']").show(); //ensure the icon is shown if wanted (though disabled)
                }
            }
            this.setHasContent(false);
        }

        public expandContentPanel(callOnShow: boolean): void {
            var controller: RelatedContentController = RelatedContentController.getInstance();

            $("#ContentPanels").show();
            $("div.maximise-icon[panel-id='" + this.getPanelId() + "']").show();

            this.show(callOnShow);

            this.enableContentPanel();

            controller.expandIconPanel();
            controller.onRelatedContentPanelResize();
        }

        public collapseContentPanel(callOnClose: boolean): void {
            var controller: RelatedContentController = RelatedContentController.getInstance();
            this.close(callOnClose);
            controller.onRelatedContentPanelResize();
        }

        public setIconBaseUrls(baseUrl: string):void {
            if (baseUrl.charAt(baseUrl.length - 1) == '/') {
                baseUrl = baseUrl.substr(0, baseUrl.length - 1); //trim the trailing slash
            }
            var BASE_URL_MATCH = "{BASE_URL}";
            this._config.Icon = this._config.Icon.replace(BASE_URL_MATCH, baseUrl);
            this._config.IconHover = this._config.IconHover.replace(BASE_URL_MATCH, baseUrl);
            this._config.IconDisabled = this._config.IconDisabled.replace(BASE_URL_MATCH, baseUrl);
        }

        public addPopoutLink(callback, callbackParams): void {
            var panelInstance = this;
            var panelEl = $("#" + this.getPanelId());
            var icons = $(panelEl).find("span.panel-icons");
            var popout = $(icons).find("div.pop-out-link");

            if (popout != undefined && popout.length == 0) {
                var popOutDiv = document.createElement("div");
                $(popOutDiv).addClass("pop-out-link");
                $(icons).prepend(popOutDiv);

                $(popOutDiv).click(function () {
                    callback(panelInstance, callbackParams);
                });
            }
            else if (popout != undefined && popout.length == 1) {
                //update to a different iFrame popout url
                $(popout).unbind().click(function () {
                    callback(panelInstance, callbackParams);
                });
            }
        }

        public removePopoutLink() {
            var panelEl = $("#" + this.getPanelId());
            var icons = $(panelEl).find("span.panel-icons");
            var popout = $(icons).find("div.pop-out-link");
            if (popout != undefined && popout.length > 0) {
                $(popout).remove();
            }
        }
    }
} 