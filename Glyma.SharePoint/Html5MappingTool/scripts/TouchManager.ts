"use strict";
module Glyma {
    export class TouchManager {
        private _hammertime: any;

        constructor() {
            //if (!this._hammer.HAS_TOUCHEVENTS && !this._hammer.HAS_POINTEREVENTS) {
            //    //if using IE
            //    var ua = window.navigator.userAgent;
            //    if (ua.indexOf("MSIE ") > 0) {
            //        this._hammer.plugins.showTouches();
            //    }
            //    this._hammer.plugins.fakeMultitouch();
            //}

            this._hammertime = Hammer(Glyma.SharedVariables.pinchzoom, {
                preventDefault: true,
                transformMinScale: 0.1,
                dragBlockHorizontal: true,
                dragBlockVertical: true,
                dragMinDistance: 10
            });

            this._initialEvents();
        }
  
        private _initialEvents() {
            //$("#mapCanvas").mousemove(function (e) {
            //    Glyma.Common.Constants.mouseX = e.pageX + 15;
            //    Glyma.Common.Constants.mouseY = e.pageY - 50;

            //    Glyma.Common.Constants.lastMouseX = Glyma.Common.Constants.mouseX;
            //    Glyma.Common.Constants.lastMouseY = Glyma.Common.Constants.mouseY;

            //    mapRenderer.mouseMoved(Glyma.Common.Constants.mouseX, Glyma.Common.Constants.mouseY);
            //});

            var doubleTapped = false, isTransform = false, isDrag = false;
            

            this._hammertime.on('mousemove dragstart doubletap touch dragend drag transform transformstart transformend', ev => {
                switch (ev.type) {
                    case "mousemove":
                        SharedVariables.mouseX = ev.pageX - SharedVariables.getMapLocationLeft();
                        SharedVariables.mouseY = ev.pageY - SharedVariables.getMapLocationTop();

                        SharedVariables.lastMouseX = SharedVariables.mouseX;
                        SharedVariables.lastMouseY = SharedVariables.mouseY;

                        Glyma.SharedVariables.mapController.mouseMoved(SharedVariables.mouseX, SharedVariables.mouseY);
                        return;
                    case 'touch':
                        SharedVariables.lastScale = SharedVariables.scale;
                        var clickedX = ev.gesture.center.pageX - SharedVariables.getMapLocationLeft();
                        var clickedY = ev.gesture.center.pageY - SharedVariables.getMapLocationTop();
                        setTimeout(() => {
                            if (!doubleTapped) {
                                Glyma.SharedVariables.mapController.clicked(clickedX, clickedY);
                            }

                            setTimeout(() => {
                                doubleTapped = false;
                            }, 300);

                        }, 300);
                        return;
                    case 'doubletap':
                        doubleTapped = true;
                        var doubletapX = ev.gesture.center.pageX - SharedVariables.getMapLocationLeft();
                        var doubletapY = (ev.gesture.center.pageY - SharedVariables.getMapLocationTop());
                        Glyma.SharedVariables.mapController.clicked(doubletapX, doubletapY, "double");
                        return;
                    case 'dragstart':
                        if (!isTransform) {
                            isDrag = true;
                        }
                        return;
                    case 'drag':
                        if (!isTransform && isDrag) {
                            SharedVariables.posX = ev.gesture.deltaX / SharedVariables.scale + SharedVariables.lastPosX;
                            SharedVariables.posY = ev.gesture.deltaY / SharedVariables.scale + SharedVariables.lastPosY;

                            SharedVariables.mouseX = ev.gesture.deltaX / SharedVariables.scale + SharedVariables.lastMouseX;
                            SharedVariables.mouseY = ev.gesture.deltaY / SharedVariables.scale + SharedVariables.lastMouseY;

                            Glyma.SharedVariables.mapController.refresh();
                        }
                        ev.preventDefault();
                        return;

                    case 'dragend':
                        if (!isTransform) {
                            SharedVariables.lastPosX = SharedVariables.posX;
                            SharedVariables.lastPosY = SharedVariables.posY;

                            SharedVariables.lastMouseX = SharedVariables.mouseX;
                            SharedVariables.lastMouseY = SharedVariables.mouseY;
                        } else {
                            isTransform = false;
                        }
                        isDrag = false;
                        return;
                    case 'transform':
                        ev.preventDefault();
                        if (isTransform) {
                            Glyma.SharedVariables.mapController.mapRenderer.scaleMap(Math.min(SharedVariables.lastScale * ev.gesture.scale, 10));
                            Glyma.SharedVariables.mapController.refresh();
                        }
                        return;
                    case 'transformstart':
                        ev.preventDefault();
                        isTransform = true;
                        SharedVariables.mouseX = ev.gesture.center.pageX - SharedVariables.getMapLocationLeft();
                        SharedVariables.mouseY = ev.gesture.center.pageY - SharedVariables.getMapLocationTop();
                        return;
                    case 'transformend':
                        ev.preventDefault();
                        return;
                }
            });
        }

        public scaleMap() {
            var transform = "scale3d(" + SharedVariables.scale + "," + SharedVariables.scale + ", 1) ";

            Glyma.SharedVariables.canvas.style.transform = transform;
            Glyma.SharedVariables.canvas.style.oTransform = transform;
            Glyma.SharedVariables.canvas.style.msTransform = transform;
            Glyma.SharedVariables.canvas.style.mozTransform = transform;
            Glyma.SharedVariables.canvas.style.webkitTransform = transform;
            Glyma.SharedVariables.mapController.hideNodeDetailButtons();
        }
    }
}

declare function Hammer(element: any, options: any);