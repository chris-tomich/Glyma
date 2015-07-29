module Glyma {
    export interface TextCanvas {
        maxWidth: number;
        height: number;
        lines: Array<TextLine>;
        left?: number;
        top?: number;
        canvas?: HTMLCanvasElement;
    }

    export interface TextLine {
        width: number;
        text: string;
    }

    export class TextMeasurer {
        private _fontFace: string = 'Roboto Condensed';
        private _fontSize: number = Common.Constants.textSize;
        private _maxWidth: number = Common.Constants.maxTextWidth;
        private _maxWordLength: number = Common.Constants.maxWordLength;
        private _textCanvas: HTMLCanvasElement = document.createElement("canvas");
        private _textContext: CanvasRenderingContext2D = this._textCanvas.getContext('2d');

        constructor() {
            this._textCanvas.width = this._maxWidth;
            this._textCanvas.height = Common.Constants.textHeight;
        }

        get fontSize(): number {
            return this._fontSize;
        }

        get fontFace(): string {
            return this._fontFace;
        }

        private _getSmallWords(word, maxWordLength) {
            var words = [];
            var start = 0;
            var end = maxWordLength;
            do {
                word = word.substring(start, end);
                words.push(word);
                start = end;
                end += maxWordLength;
                if (end > word.length + 1) {
                    end = word.length + 1;
                }
            } while (start < word.length);
            return words;
        }

        private _getWords(text, maxWordLength) {
            var splitWords = text.split(' ');
            var words = [];
            for (var i = 0, l = splitWords.length; i < l; i++) {
                var word = splitWords[i];
                if (word.length <= maxWordLength) {
                    words.push(word);
                }
                else {
                    var smallWords = this._getSmallWords(word, maxWordLength);
                    for (var j = 0; j < smallWords.length; j++) {
                        words.push(smallWords[j]);
                    }
                }
            }
            return words;
        }

        public reset() {
            this._fontSize = Common.Constants.textSize * SharedVariables.scale;
            this._maxWidth = Common.Constants.maxTextWidth * SharedVariables.scale;
            this._maxWordLength = Common.Constants.maxWordLength;
            this._textCanvas.width = this._maxWidth;
            this._textCanvas.height = Common.Constants.textHeight * SharedVariables.scale;
        }

        public measureText(text: string): TextCanvas {
            var context = this._textContext;
            var maxWidth = this._maxWidth;
            var maxHeight = this._textCanvas.height;
            var maxWordLength = this._maxWordLength;
            var fontSize = this._fontSize;
            var fontFace = this._fontFace;
            var words = this._getWords(text, maxWordLength);

            var width = Common.Constants.imageSize * SharedVariables.scale;
            var lines = [];
            var line = '';
            context.font = fontSize + "px '" + fontFace + "'";
            for (var n = 0; n < words.length; n++) {
                var testLine = line + words[n];
                var metrics = context.measureText(testLine);
                var testWidth = metrics.width;
                if (testWidth > maxWidth) {
                    var w = context.measureText(line.trim()).width;
                    width = Math.max(width, w);
                    textLine = {
                        text: line.trim(),
                        width: w
                    };
                    lines.push(textLine);
                    line = words[n] + ' ';
                }
                else {
                    line = testLine + ' ';
                }
            }
            width = Math.max(width, context.measureText(line.trim()).width);

            var textLine: TextLine = {
                text: line.trim(),
                width: context.measureText(line.trim()).width
            };
            lines.push(textLine);

            var height = lines.length * fontSize;
            if (width > maxWidth || height > maxHeight) {
                console.log('Panic', text, maxWidth, maxHeight, width, height);
            }

            var result: TextCanvas = {
                maxWidth: width,
                height: height + fontSize,
                lines: lines,
                left: 0,
                top: 0
            };
            return result;
        }
    }
}