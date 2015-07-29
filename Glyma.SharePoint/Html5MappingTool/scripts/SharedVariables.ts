module Glyma.SharedVariables {
    export var canvas: HTMLCanvasElement = null;
    export var pinchzoom: HTMLElement = null;
    export var context: CanvasRenderingContext2D = null;
    export var mapController: MapController = null;
    export var touchManager: TouchManager = null;

    export var posX:number = 0;
    export var posY: number = 0;

    export var lastPosX: number = 0;
    export var lastPosY: number = 0;

    export var mouseX: number = 0;
    export var mouseY: number = 0;

    export var lastMouseX: number = 0;
    export var lastMouseY: number = 0;

    export var mouseRelativeX: number = 0;
    export var mouseRelativeY: number = 0;

    export var scale: number = 1;
    export var lastScale: number = 1;

    export var isHtmlElementsLoaded: boolean = false;

    export function getMapLocationTop(): number {
        return $('#pinchzoom').offset().top - $(window).scrollTop();
    };

    export function getMapLocationLeft(): number {
        return $('#pinchzoom').offset().left - $(window).scrollLeft();
    };

    export function getMapWidth(): number {
        return window.innerWidth - getMapLocationLeft();
    }

    export function getMapHeight(): number {
        return $('#pinchzoom').innerHeight();
    }
} 