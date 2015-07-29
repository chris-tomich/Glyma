interface HTMLElement {
    requestFullscreen: Function;
    mozRequestFullScreen: Function;
    webkitRequestFullscreen: Function;

}

interface Document {
    exitFullscreen: Function;
    mozCancelFullScreen: Function;
    msCancelFullScreen: Function;
    webkitExitFullscreen: Function;
}

interface MSStyleCSSProperties {
    oTransform: any;
    mozTransform: any;
    webkitTransform: any;
}