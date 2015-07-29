"use strict";
module Glyma {
    export class NodeActionOptions {
        private _showRelatedContentWithVideo: boolean = false;

        public get showRelatedContentWithVideo(): boolean {
            return this._showRelatedContentWithVideo;
        }

        public set showRelatedContentWithVideo(value: boolean) {
            this._showRelatedContentWithVideo = value;
        }
    }
}  