"use strict";
module Glyma {
    export class ArrowRenderer {
        private _margin: number = Common.Constants.arrowMargin;
        private _panX: number = 0;
        private _panY: number = 0;
        private _arrowColor: string = '#000000';

        private _canvas: HTMLCanvasElement;
        private _context: CanvasRenderingContext2D;

        constructor(canvas: HTMLCanvasElement, context: CanvasRenderingContext2D) {
            this._canvas = canvas;
            this._context = context;
        }

        private _lineLength(x1, y1, x2, y2) {
            return Math.sqrt(Math.pow(x2 - x1, 2) + Math.pow(y2 - y1, 2));
        }

        private _intersectNode(node1, node2, diff) {
            return !(node2.left - diff > node1.right + diff ||
                node2.right + diff < node1.left - diff ||
                node2.top - diff > node1.bottom + diff ||
                node2.bottom + diff < node1.top - diff);
        }

        private _isVisible(node) {
            return node.right > this._panX &&
                node.left < this._canvas.width + this._panX &&
                node.top > this._panY &&
                node.bottom < this._canvas.height + this._panY;
        }

        private _getBoundaryPoint(node, angle): any {
            var halfWidth = node.width / 2 + this._margin;
            var halfHeight = node.height / 2 + this._margin;
            var rectangleAngle = Math.atan2(halfHeight, halfWidth);
            angle = angle % (Math.PI * 2);
            if (angle < 0) {
                angle += Math.PI * 2;
            }
            var region = 4;
            if (angle >= Math.PI * 2 - rectangleAngle || angle <= rectangleAngle) {
                region = 1;
            }
            else if (angle <= Math.PI  - rectangleAngle) {
                region = 2;
            }
            else if (angle <= Math.PI + rectangleAngle) {
                region = 3;
            }
            switch (region) {
                case 1:
                    return {
                        x: node.xPosition + halfWidth,
                        y: node.yPosition - halfWidth * Math.tan(angle)
                    };
                case 2:
                    return {
                        x: node.xPosition + halfHeight / Math.tan(angle),
                        y: node.yPosition - halfHeight
                    };
                case 3:
                    return {
                        x: node.xPosition - halfWidth,
                        y: node.yPosition + halfWidth * Math.tan(angle)
                    };
                case 4:
                    return {
                        x: node.xPosition - halfHeight / Math.tan(angle),
                        y: node.yPosition + halfHeight
                    };
                default:
                    return {
                        x: node.xPosition - halfWidth / Math.tan(angle),
                        y: node.yPosition + halfHeight
                    };
            }
        }

        public drawArrow(fromNode, toNode) {
            if (!this._intersectNode(fromNode, toNode, 12)) {
                var fromAngle = Math.atan2(fromNode.yPosition - toNode.yPosition, toNode.xPosition - fromNode.xPosition);
                var toAngle = fromAngle - Math.PI;
                var fromPoint = this._getBoundaryPoint(fromNode, fromAngle);
                var toPoint = this._getBoundaryPoint(toNode, toAngle);
                var arrowAngle = Math.atan2(toPoint.y - fromPoint.y, toPoint.x - fromPoint.x);
                var length = this._lineLength(fromPoint.x, fromPoint.y, toPoint.x, toPoint.y);
                if (length > Common.Constants.arrowMinLength) {
                    var headlen = Common.Constants.arrowHeadSize;   // length of head in pixels
                    //angle = Math.atan2(toNode.yPosition - fromNode.yPosition, toNode.xPosition - fromNode.xPosition);
                    this._context.lineWidth = Common.Constants.arrowLineWidth;
                    this._context.strokeStyle = this._arrowColor;
                    this._context.beginPath();
                    this._context.moveTo(fromPoint.x, fromPoint.y);
                    this._context.lineTo(toPoint.x, toPoint.y);
                    //this.context.lineTo(toPoint.x - headlen * Math.cos(arrowAngle - Math.PI / 6), toPoint.y - headlen * Math.sin(arrowAngle - Math.PI / 6));
                    //this.context.moveTo(toPoint.x, toPoint.y);
                    //this.context.lineTo(toPoint.x - headlen * Math.cos(arrowAngle + Math.PI / 6), toPoint.y - headlen * Math.sin(arrowAngle + Math.PI / 6));
                    this._context.stroke();
                    this._context.beginPath();
                    this._context.moveTo(toPoint.x, toPoint.y);
                    this._context.lineTo(toPoint.x - headlen * Math.cos(arrowAngle - Math.PI / 6), toPoint.y - headlen * Math.sin(arrowAngle - Math.PI / 6));
                    this._context.lineTo(toPoint.x - headlen * Math.cos(arrowAngle + Math.PI / 6), toPoint.y - headlen * Math.sin(arrowAngle + Math.PI / 6));
                    this._context.fill();
                }
            }
        }
    }
}