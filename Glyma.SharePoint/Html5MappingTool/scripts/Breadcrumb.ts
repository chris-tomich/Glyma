'use strict';
module Glyma {
    export class Breadcrumb {
        private _uniqueId: string = null;
        private _name: string = null;

        constructor(breadcrumbData: any) {
            this._uniqueId = breadcrumbData.uniqueId;
            this._name = breadcrumbData.name;
        }

        get name(): string {
            var text = this._name;
            if (text.length > 30) {
                return text.substring(0, 30) + "...";
            }
            return text;
        }

        get fullName(): string {
            return this._name;
        }

        get uniqueId(): string {
            return this._uniqueId;
        }
    }
} 