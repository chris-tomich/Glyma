module Glyma {
    export class BreadcrumbControl {
        private _breadcrumbs: Array<Breadcrumb> = null;
        private _domainId: string = null;
        constructor(domainId) {
            this._domainId = domainId;
        }

        public get breadcrumbs(): Array<Breadcrumb>{
            if (this._breadcrumbs == null) {
                this._breadcrumbs = new Array<Breadcrumb>();
            }
            return this._breadcrumbs;
        }

        get currentBreadcrumb(): Breadcrumb {
            if (this.breadcrumbs.length < 1) {
                return null;
            } else {
                return this.breadcrumbs[this.breadcrumbs.length - 1];
            }
        }

        public refreshBreadcrumb(): void {
            $("#breadcrumb-container").html('');
            for (var i = 0; i < this._breadcrumbs.length; i++) {
                var breadcrumbElement = this._createBreadcrumbElement(this._breadcrumbs[i]);
                if (i == 0) {
                    breadcrumbElement.addClass("first");
                }
                $("#breadcrumb-container").append(breadcrumbElement);
            }
        }

        public removeToIndex(index: number): void {
            this._breadcrumbs.splice(index + 1, this._breadcrumbs.length - index - 1);
            this.refreshBreadcrumb();
        }

        public getIndexByNodeId(nodeId: string): number {
            for (var i = 0; i < this.breadcrumbs.length; i++) {
                if (this.breadcrumbs[i].uniqueId == nodeId) {
                    return i;
                }
            }
            return -1;
        }

        public addBreadcrumb(breadcrumbData: any) {
            var breadcrumb = new Breadcrumb(breadcrumbData);
            this.breadcrumbs.push(breadcrumb);
        }

        public insertBreadcrumb(breadcrumbData: any, index: number = 0) {
            var breadcrumb = new Breadcrumb(breadcrumbData);
            this.breadcrumbs.splice(index, 0, breadcrumb);
        }

        private _createBreadcrumbElement(breadcrumb: Breadcrumb) {
            var breadcrumbElement = jQuery('#breadcrumb-template').clone();
            breadcrumbElement.attr('title', breadcrumb.fullName);
            breadcrumbElement.attr('map-id', breadcrumb.uniqueId);
            breadcrumbElement.html('<a href="#">' + breadcrumb.name + '</a>');
            breadcrumbElement.css("display", "block");
            breadcrumbElement.click(() => {
                Glyma.SharedVariables.mapController.getMapData(this._domainId, breadcrumb.uniqueId);
            });
            return breadcrumbElement;
        }
    }
}