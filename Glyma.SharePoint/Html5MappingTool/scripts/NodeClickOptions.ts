"use strict";
module Glyma {
    export class NodeClickOptions {
        private _showRelatedMaps: boolean = true;

        public get showRelatedMaps(): boolean {
            return this._showRelatedMaps;
        }

        public set showRelatedMaps(value: boolean) {
            this._showRelatedMaps = value;
        }
    }
} 