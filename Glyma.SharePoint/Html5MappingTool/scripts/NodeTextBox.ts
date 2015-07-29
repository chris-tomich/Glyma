module Glyma {
    export class NodeTextBox {
        private _text: string = "";
        private _textMeasurer: TextMeasurer = null;
        private _scale: number = 1;
        private _hasLink: boolean = null;

        constructor(text: string, hasLink: boolean = false) {
            this._text = text;
            this._hasLink = hasLink;
            this._scale = SharedVariables.scale;
        }

        private get NodeTextMeasurer(): TextMeasurer {
            if (this._textMeasurer == null) {
                this._textMeasurer = new TextMeasurer();
            }

            return this._textMeasurer;
        }

        get Text(): string {
            return this._text;
        }

        get scale(): number {
            return this._scale;
        }

        public reset(): void {
            this._scale = SharedVariables.scale;
            this.NodeTextMeasurer.reset();
        }

        public createTextCanvas(): TextCanvas {
            var node: TextCanvas = this.NodeTextMeasurer.measureText(this.Text);

            var canvas = document.createElement("canvas");
            var context = canvas.getContext('2d');
            canvas.width = node.maxWidth;
            canvas.height = node.height;
            context.font = this.NodeTextMeasurer.fontSize + "px '" + this.NodeTextMeasurer.fontFace + "'";
            //context.fillStyle = 'red';
            //context.fillRect(node.Left, node.Top, node.MaxWidth, node.height);
            if (this._hasLink) {
                context.fillStyle = 'blue';
            } else {
                context.fillStyle = 'black';
            }
            var lines = node.lines;

            for (var i = 0; i < lines.length; i++) {
                var line = lines[i];
                context.fillText(line.text, (canvas.width - line.width) / 2, i * this.NodeTextMeasurer.fontSize + this.NodeTextMeasurer.fontSize);
            }

            node.canvas = canvas;
            return node;
        }
    }
}