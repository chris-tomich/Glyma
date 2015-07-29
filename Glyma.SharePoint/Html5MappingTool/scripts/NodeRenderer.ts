'use strict';
module Glyma {
    export class NodeRenderer {
        private _context: CanvasRenderingContext2D;

        constructor(context: CanvasRenderingContext2D) {
            this._context = context;
        }

        public drawNode(node: Node): void {
            var left = node.left;

            //if the TextCanvas is narrower than the image center it in the image
            if (node.TextCanvas.maxWidth <= Common.Constants.imageSize) {
                left += (Common.Constants.imageSize - node.TextCanvas.maxWidth) / 2;
            }

            //Draw the node image
            this.redrawNodeImage(node);

            //Draw the TextCanvas for the node
            if (node.name.length > 0) {
                this._context.drawImage(node.TextCanvas.canvas,
                    left,
                    node.top + Common.Constants.imageSize,
                    node.TextCanvas.maxWidth / node.nodeTextBox.scale,
                    node.TextCanvas.height / node.nodeTextBox.scale); 
            }
        }

        public redrawNodeImage(node: Node, offSetX: number = 0, offSetY: number = 0): void {

            if (node.isSelected) {
                var x = node.imageLeft + offSetX + Common.Constants.imageSize / 2;
                var y = node.imageTop + offSetY + Common.Constants.imageSize / 2;

                // Radii of the white glow.
                var innerRadius = 0;

                var outerRadius = Common.Constants.imageSize * 1.5;
                
                // Radius of the entire circle.
                var radius = Common.Constants.imageSize;

                this._context.beginPath();
                var gradient = this._context.createRadialGradient(x, y, 0, x, y, outerRadius);
                gradient.addColorStop(0.000, 'rgba(0, 0, 255, 1.000)');
                gradient.addColorStop(1.000, 'rgba(255, 255, 255, 0.000)');
                //this._context.arc(x, y, outerRadius * 0.5, 0, 2 * Math.PI);

                this._context.fillStyle = gradient;
                this._context.fillRect(x - outerRadius, y - outerRadius, outerRadius * 2, outerRadius * 2);
            }
            
            //Draw the node icon
            this._context.drawImage(node.image,
                node.imageLeft + offSetX, node.imageTop + offSetY,
                Common.Constants.imageSize,
                Common.Constants.imageSize);

            this.drawCornerButton(node, offSetX, offSetY);
            this.drawCollapseControl(node, offSetX, offSetY);
        }

        private drawCornerButton(node: Node, offSetX: number = 0, offSetY: number = 0) {
            if (node.nodeCornerButton.hasCornerButton) {
                var id = "button-" + (node.isHoverCornerButton ? "hover-" : "") + node.nodeCornerButton.showingButton + node.lastImageScale;
                var button = document.getElementById(id);
                this._context.drawImage(button, node.cornerButtonLeft + offSetX, node.cornerButtonTop + offSetY, Common.Constants.cornerButtonWidth, Common.Constants.cornerButtonHeight);
                if (node.nodeCornerButton.hasExtendButton) {
                    var extendButton = document.getElementById("extendbutton-" + (node.isHoverCornerExtendButton ? "hover" : "static"));
                    this._context.drawImage(extendButton, node.cornerExtendButtonLeft + offSetX, node.cornerButtonTop + offSetY, Common.Constants.extendCornerButtonWidth, Common.Constants.cornerButtonHeight);
                }
            }
        }

        public cornerButton(node: Node, offSetX: number = 0, offSetY: number = 0, isHover = false) {
            if (node.nodeCornerButton.hasCornerButton) {
                var id = "button-" + (isHover ? "hover-" :"") + node.nodeCornerButton.showingButton + node.lastImageScale;
                var button = document.getElementById(id);
                this._context.drawImage(button, node.cornerButtonLeft + offSetX, node.cornerButtonTop + offSetY, Common.Constants.cornerButtonWidth * SharedVariables.scale, Common.Constants.cornerButtonHeight * SharedVariables.scale);
            }
        }

        public cornerExtendButton(node: Node, offSetX: number = 0, offSetY: number = 0, isHover = false) {
            if (node.nodeCornerButton.hasExtendButton) {
                var id = isHover ? "extendbutton-hover" : "extendbutton-static";
                var extendButton = document.getElementById(id);
                this._context.drawImage(extendButton, node.cornerExtendButtonLeft + offSetX, node.cornerButtonTop + offSetY, Common.Constants.extendCornerButtonWidth * SharedVariables.scale, Common.Constants.cornerButtonHeight * SharedVariables.scale);
            }
        }

        private drawCollapseControl(node: Node, offSetX: number = 0, offSetY: number = 0) {
            if (node.collpaseState != "none") {
                var button;
                switch (node.collpaseState) {
                    case "collapsed":
                        button = document.getElementById("collapsed" + node.lastImageScale);
                        break;
                    case "semicollapsed":
                        button = document.getElementById("semicollapsed" + node.lastImageScale);
                        break;
                    case "expanded":
                        button = document.getElementById("expanded" + node.lastImageScale);
                        break;
                    default:
                    break;
                }
                if (button !== "undefined") {
                    if (node.isHoverCollapseButton) {
                        this._context.save();
                        this._context.globalAlpha = 0.4;
                        this._context.drawImage(button, node.collapseButtonLeft + offSetX, node.collapseButtonTop + offSetY, Common.Constants.collapseImageSize, Common.Constants.collapseImageSize);
                        this._context.restore();
                    } else {

                        this._context.drawImage(button, node.collapseButtonLeft + offSetX, node.collapseButtonTop + offSetY, Common.Constants.collapseImageSize, Common.Constants.collapseImageSize);
                    }
                }
            }
        }
    }
}