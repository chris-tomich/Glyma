"use strict";
module Glyma {
    export class NodeCornerButton {
        private _showingButton: string;
        private _hasExtendButton = false;
        private _hasCornerButton = false;

        constructor(node: Node) {
            var count = 0;
            if (node.hasFeed) {
                count++;
                this._showingButton = "feed";
            }

            if (node.hasLocation) {
                count++;
                this._showingButton = "location";
            }

            if (node.hasMap) {
                count++;
                this._showingButton = "map";
            }

            if (node.hasContent) {
                count++;
                this._showingButton = "content";
            }

            if (node.hasVideo) {
                count++;
                this._showingButton = "play";
            }

            if (count > 0) {
                this._hasCornerButton = true;
                if (count > 1) {
                    this._hasExtendButton = true;
                }
            }
        }

        get hasCornerButton() {
            return this._hasCornerButton;
        }

        get hasExtendButton() {
            return this._hasExtendButton;
        }

        get showingButton() {
            return this._showingButton;
        }

        set showingButton(value: string) {
            this._showingButton = value;
        }
    }
}  