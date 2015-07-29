module Glyma {
    export class RealignController {
        private _parents: Array<number> = [];
        private _nodeNeedToBePlaced: Array<number> = [];
        private _mapController: MapController = null;
        private _maxDepth: number = 0;

        constructor(mapController: MapController) {
            this._mapController = mapController;
        }

        public horizontalRealign() {
            this._mapController.clearRTree();
            this._calculate();

            for (var i = 0; i < this._mapController.nodes.length; i++) {
                if (i == 0) {
                    this._maxDepth = this._mapController.nodes[i].rootDepth;
                } else if (this._maxDepth < this._mapController.nodes[i].rootDepth) {
                    this._maxDepth = this._mapController.nodes[i].rootDepth;
                }
            }

            for (var k = 0; k <= this._maxDepth; k++) {
                this._calculateStackNumberByDepth(k);
            }

            
            for (var j = this._maxDepth; j >= 0; j--) {
                this._calculateByDepth(j);
            }

            var visibleNodeIndexes = this._mapController.visibleNodeIndexes;
            for (var t = 0; t < visibleNodeIndexes.length; t++) {
                var index = visibleNodeIndexes[t];
                this._mapController.rTreeManager.insertNode(this._mapController.nodes[index].left, this._mapController.nodes[index].top, this._mapController.nodes[index].width, this._mapController.nodes[index].height, index );
            }

            for (var r = 0; r < this._mapController.arrows.length; r++) {
                var from = this._mapController.arrows[r].from, to = this._mapController.arrows[r].to;

                if (this._mapController.nodes[from].isVisible && this._mapController.nodes[to].isVisible) {
                    var x1 = this._mapController.nodes[from].xPosition;
                    var y1 = this._mapController.nodes[from].yPosition;
                    var x2 = this._mapController.nodes[to].xPosition;
                    var y2 = this._mapController.nodes[to].yPosition;

                    var x = 0, y = 0, w = 0, h = 0;
                    if (x1 <= x2) {
                        x = x1;
                        w = x2 - x1;
                    } else {
                        x = x2;
                        w = x1 - x2;
                    }

                    if (y1 <= y2) {
                        y = y1;
                        h = y2 - y1;
                    } else {
                        y = y2;
                        h = y1 - y2;
                    }
                    this._mapController.rTreeManager.insertArrow(x, y, w, h, r);
                }
            }
        }

        private _calculateStackNumberByDepth(rootDepth: number) {
            var stackNumber = 0;
            var nodesIndexes = new Array<number>();
            for (var i = 0; i < this._mapController.nodes.length; i++) {
                if (this._mapController.nodes[i].rootDepth == rootDepth) {
                    nodesIndexes.push(i);
                }
            }

            if (nodesIndexes.length > 0) {
                var sorted = nodesIndexes.sort((a, b) => {
                    var aParents = this._mapController.nodes[a].parentIndexes;
                    var bParents = this._mapController.nodes[b].parentIndexes;
                    if (aParents.length > 0 && bParents.length > 0) {
                        var aParentMinStack = 0, bParentMinStack = 0;
                        
                        for (var p = 0; p < aParents.length; p++) {
                            if (p == 0 || aParentMinStack > this._mapController.nodes[aParents[p]].stackNumber) {
                                aParentMinStack = this._mapController.nodes[aParents[p]].stackNumber;
                            }
                        }

                        for (var k = 0; k < bParents.length; k++) {
                            if (k == 0 || bParentMinStack > this._mapController.nodes[bParents[k]].stackNumber) {
                                bParentMinStack = this._mapController.nodes[bParents[k]].stackNumber;
                            }
                        }

                        if (aParentMinStack < bParentMinStack) {
                            return -1;
                        } else if (aParentMinStack > bParentMinStack) {
                            return 1;
                        } 
                    }

                    if (this._mapController.nodes[a].yPosition > this._mapController.nodes[b].yPosition) {
                        return 1;
                    } else if (this._mapController.nodes[a].yPosition < this._mapController.nodes[b].yPosition) {
                        return -1;
                    }
                    return 0;
                });

                for (var j = 0; j < sorted.length; j++) {
                    this._mapController.nodes[sorted[j]].stackNumber = stackNumber;
                    stackNumber ++;
                }
            }
        }

        private _calculateByDepth(rootDepth: number) {
            var topMargin = Common.Constants.realignTopMagin/2, width = Common.Constants.realignNodeWidth, topOffset = topMargin + 50, leftOffset = Common.Constants.realignLeftMargin;
            leftOffset += (rootDepth + 0.5) * width;

            var nodesIndexes = new Array<number>();
            for (var i = 0; i < this._mapController.nodes.length; i++) {
                if (this._mapController.nodes[i].rootDepth == rootDepth && this._mapController.nodes[i].isVisible) {
                    nodesIndexes.push(i);
                }
            }

            if (nodesIndexes.length > 0) {
                var sorted = nodesIndexes.sort((a, b) => {


                    if (this._mapController.nodes[a].stackNumber > this._mapController.nodes[b].stackNumber) {
                        return 1;
                    } else if (this._mapController.nodes[a].stackNumber < this._mapController.nodes[b].stackNumber) {
                        return -1;
                    }
                    return 0;
                });

                for (var j = 0; j < sorted.length; j++) {
                    if (this._mapController.nodes[sorted[j]].isVisible) {
                        topOffset += (this._mapController.nodes[sorted[j]].height + topMargin) / 2;

                        var childIndexes = this._mapController.nodes[sorted[j]].childIndexes;

                            if (childIndexes.length > 0) {
                                var sum = 0, count = 0;
                                for (var k = 0; k < childIndexes.length; k++) {
                                    if (this._mapController.nodes[childIndexes[k]].rootDepth == this._mapController.nodes[sorted[j]].rootDepth + 1 && this._mapController.nodes[childIndexes[k]].isVisible) {
                                        sum += this._mapController.nodes[childIndexes[k]].yPosition;
                                        count++;
                                    }
                                }
                                var centerTopOfChildNodes = count == 0? 0 :sum / count;

                                if (centerTopOfChildNodes >= topOffset) {
                                    topOffset = centerTopOfChildNodes;
                                } else if (count > 0) {
                                    //get the minimum statck number of childs
                                    var deeperStackNumber = 0;
                                    for (var l = 0; l < childIndexes.length; l++) {
                                        if ((l == 0 || this._mapController.nodes[childIndexes[l]].stackNumber < deeperStackNumber) && this._mapController.nodes[childIndexes[l]].rootDepth == this._mapController.nodes[sorted[j]].rootDepth + 1 && this._mapController.nodes[childIndexes[l]].isVisible) {
                                            deeperStackNumber = this._mapController.nodes[childIndexes[l]].stackNumber;
                                        }
                                    }

                                    var offset = topOffset - centerTopOfChildNodes;
                                    var movedIndexes = [];
                                    for (var m = 0; m < this._mapController.nodes.length; m++) {
                                        if (this._mapController.nodes[m].rootDepth == rootDepth + 1 && this._mapController.nodes[m].stackNumber >= deeperStackNumber) {
                                            if (this._mapController.nodes[m].isVisible) {
                                                this._mapController.nodes[m].yPosition += offset;
                                                var allChildIndexes = this._mapController.getAllChildIndexes(m);
                                                for (var n = 0; n < allChildIndexes.length; n++) {
                                                    if (movedIndexes.indexOf(allChildIndexes[n]) < 0
                                                        && this._mapController.nodes[m].rootDepth < this._mapController.nodes[allChildIndexes[n]].rootDepth
                                                        && this._mapController.nodes[allChildIndexes[n]].isVisible) {
                                                        this._mapController.nodes[allChildIndexes[n]].yPosition += offset;
                                                        movedIndexes.push(allChildIndexes[n]);
                                                        
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        this._mapController.nodes[sorted[j]].yPosition = topOffset;
                        this._mapController.nodes[sorted[j]].xPosition = leftOffset;
                        topOffset += (this._mapController.nodes[sorted[j]].height + topMargin) / 2;
                    }
                }
            }

        }

        private _calculate() {
            for (var i = 0; i < this._mapController.nodes.length; i++) {
                this._mapController.nodes[i].rootDepth = 0;
                this._mapController.nodes[i].stackNumber = 0;
                if (this._mapController.nodes[i].parentIndexes.length == 0) {
                    this._parents.push(i);
                } else {
                    this._nodeNeedToBePlaced.push(i);
                }
            }

            if (this._parents.length == 0) {
                this._addLeftMostNodeToParent();
            }

            this._placeNodeToMap();
            this._recheckForIncorrectDepth();
        }


        private _addLeftMostNodeToParent() {
            
            var leftMost = 0, parentIndex = -1;
            for (var j = 0; j < this._mapController.nodes.length; j++) {
                if (j == 0) {
                    leftMost = this._mapController.nodes[j].xPosition;
                    parentIndex = j;
                } else {
                if (this._mapController.nodes[j].xPosition < leftMost) {
                    leftMost = this._mapController.nodes[j].xPosition;
                        parentIndex = j;
                    }
                }
            }

            if (parentIndex >= 0) {
                this._parents.push(parentIndex);
                this._mapController.nodes[parentIndex].rootDepth = 0;
                this._nodeNeedToBePlaced.splice(this._nodeNeedToBePlaced.indexOf(parentIndex), 1);
            }
        }

        private _placeNodeToMap() {
            var isUpdated = true;
            var nodesToLoop = this._parents;
            while (this._nodeNeedToBePlaced.length > 0 && isUpdated) {
                isUpdated = false;
                var temp = [];
                for (var i = 0; i < nodesToLoop.length; i++) {
                    var rootDepth = this._mapController.nodes[nodesToLoop[i]].rootDepth;
                    var childNodes = this._mapController.nodes[nodesToLoop[i]].childIndexes;
                    if (childNodes.length > 0) {
                        for (var j = 0; j < childNodes.length; j++) {
                            var index = this._nodeNeedToBePlaced.indexOf(childNodes[j]);
                            if (index >= 0) {
                                temp.push(childNodes[j]);
                                isUpdated = true;
                                this._nodeNeedToBePlaced.splice(index, 1);
                                this._mapController.nodes[childNodes[j]].rootDepth = rootDepth + 1;
                            }
                        }
                    }
                }
                nodesToLoop = temp;
            }

            this._parents = [];

            if (this._nodeNeedToBePlaced.length > 0) {
                this._addLeftMostNodeToParent();
            }
        }

        private _recheckForIncorrectDepth() {
            var needToReCheck = true;
            var changedNodes = [];
            while (needToReCheck) {
                needToReCheck = false;


                for (var i = 0; i < this._mapController.arrows.length; i++) {
                    var arrow = this._mapController.arrows[i];
                    if (changedNodes.indexOf(this._mapController.arrows[i].from) < 0 && this._mapController.nodes[arrow.from].rootDepth <= this._mapController.nodes[arrow.to].rootDepth) {
                        needToReCheck = true;
                        var allNodes = this._mapController.getAllChildIndexes(this._mapController.arrows[i].from);
                        allNodes.push(this._mapController.arrows[i].from);
                        for (var j = 0; j < allNodes.length; j++) {
                            if (this._mapController.nodes[allNodes[j]].rootDepth >= this._mapController.nodes[arrow.from].rootDepth) {
                                this._mapController.nodes[allNodes[j]].rootDepth = this._mapController.nodes[allNodes[j]].rootDepth + 1;
                            }
                        }
                        changedNodes.push(arrow.from);
                    }
                }
            }
        }
    }
} 