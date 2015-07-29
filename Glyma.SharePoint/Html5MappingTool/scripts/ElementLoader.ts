module Glyma {
    export class ElementLoader {
        private _hiddenContainer: JQuery = null;
        private _baseUrl: string = null;

        constructor(baseUrl: string) {
            this._baseUrl = baseUrl;
            this._hiddenContainer = $("#hidePanels");
        }

        public load() {
            if (!SharedVariables.isHtmlElementsLoaded) {
                this._loadImages();
                this._loadOthers();
                SharedVariables.isHtmlElementsLoaded = true;
            }
        }

        private _loadImages() {
            this._hiddenContainer.append('<img id="proImage" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes1x/pro.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="conImage" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes1x/cons.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="ideaImage" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes1x/idea.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="questionImage" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes1x/question.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="noteImage" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes1x/note.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="mapImage" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes1x/map.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="decisionImage" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes1x/decision.png" style="display: none;" />');

            this._hiddenContainer.append('<img id="proImage2x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes2x/pro.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="conImage2x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes2x/cons.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="ideaImage2x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes2x/idea.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="questionImage2x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes2x/question.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="noteImage2x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes2x/note.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="mapImage2x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes2x/map.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="decisionImage2x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes2x/decision.png" style="display: none;" />');

            this._hiddenContainer.append('<img id="proImage4x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes4x/pro.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="conImage4x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes4x/cons.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="ideaImage4x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes4x/idea.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="questionImage4x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes4x/question.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="noteImage4x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes4x/note.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="mapImage4x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes4x/map.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="decisionImage4x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes4x/decision.png" style="display: none;" />');

            this._hiddenContainer.append('<img id="proImage8x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes8x/pro.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="conImage8x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes8x/cons.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="ideaImage8x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes8x/idea.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="questionImage8x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes8x/question.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="noteImage8x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes8x/note.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="mapImage8x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes8x/map.png" style="display: none;" />');
            this._hiddenContainer.append('<img id="decisionImage8x" src="' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodes8x/decision.png" style="display: none;" />');
            
        
            this._hiddenContainer.append('<img id = "button-content1x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu1x/button-content-static.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-feed1x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu1x/button-feed-static.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-map1x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu1x/button-map-static.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-pause1x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu1x/button-pause-static.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-play1x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu1x/button-play-static.png" style ="display: none;" / >');

            this._hiddenContainer.append('<img id = "button-content2x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu2x/button-content-static.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-feed2x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu2x/button-feed-static.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-map12x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu2x/button-map-static.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-pause2x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu2x/button-pause-static.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-play2x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu2x/button-play-static.png" style ="display: none;" / >');

            this._hiddenContainer.append('<img id = "button-content4x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu4x/button-content-static.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-feed4x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu4x/button-feed-static.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-map4x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu4x/button-map-static.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-pause4x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu4x/button-pause-static.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-play4x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu4x/button-play-static.png" style ="display: none;" / >');

            this._hiddenContainer.append('<img id = "button-content8x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu8x/button-content-static.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-feed8x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu8x/button-feed-static.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-map8x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu8x/button-map-static.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-pause8x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu8x/button-pause-static.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-play8x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu8x/button-play-static.png" style ="display: none;" / >');

            this._hiddenContainer.append('<img id = "button-hover-content1x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu1x/button-content-hover.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-hover-feed1x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu1x/button-feed-hover.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-hover-map1x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu1x/button-map-hover.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-hover-pause1x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu1x/button-pause-hover.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-hover-play1x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu1x/button-play-hover.png" style ="display: none;" / >');

            this._hiddenContainer.append('<img id = "button-hover-content2x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu2x/button-content-hover.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-hover-feed2x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu2x/button-feed-hover.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-hover-map12x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu2x/button-map-hover.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-hover-pause2x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu2x/button-pause-hover.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-hover-play2x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu2x/button-play-hover.png" style ="display: none;" / >');

            this._hiddenContainer.append('<img id = "button-hover-content4x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu4x/button-content-hover.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-hover-feed4x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu4x/button-feed-hover.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-hover-map4x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu4x/button-map-hover.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-hover-pause4x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu4x/button-pause-hover.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-hover-play4x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu4x/button-play-hover.png" style ="display: none;" / >');

            this._hiddenContainer.append('<img id = "button-hover-content8x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu8x/button-content-hover.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-hover-feed8x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu8x/button-feed-hover.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-hover-map8x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu8x/button-map-hover.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-hover-pause8x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu8x/button-pause-hover.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "button-hover-play8x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu8x/button-play-hover.png" style ="display: none;" / >');

            this._hiddenContainer.append('<img id = "extendbutton-hover" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/nodemenu/menubutton-hover.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "extendbutton-static" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/nodemenu/menubutton-static.png" style ="display: none;" / >');

            this._hiddenContainer.append('<img id = "semicollapsed1x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu1x/semicollapsed.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "collapsed1x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu1x/collapsed.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "expanded1x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu1x/expanded.png" style ="display: none;" / >');
        
            this._hiddenContainer.append('<img id = "semicollapsed2x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu2x/semicollapsed.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "collapsed2x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu2x/collapsed.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "expanded2x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu2x/expanded.png" style ="display: none;" / >');

            this._hiddenContainer.append('<img id = "semicollapsed4x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu4x/semicollapsed.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "collapsed4x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu4x/collapsed.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "expanded4x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu4x/expanded.png" style ="display: none;" / >');

            this._hiddenContainer.append('<img id = "semicollapsed8x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu8x/semicollapsed.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "collapsed8x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu8x/collapsed.png" style ="display: none;" / >');
            this._hiddenContainer.append('<img id = "expanded8x" src = "' + this._baseUrl + '/Style Library/Glyma/Icons/scaled/nodemenu8x/expanded.png" style ="display: none;" / >');


            
        }

        private _loadOthers() {
            this._hiddenContainer.append('<div id="nodeDetailButtonContainer" class="unselectable" style="position: absolute;z-index: 100;display: none;">'
                + '< img id="nodedetailshowall" class="nodedetailbutton unselectable" height = "35" width = "35" src ="' + this._baseUrl + '/Style Library/Glyma/Icons/nodemenu/showall-static.png" / >'
                + '<img id="nodedetailcontent" class="nodedetailbutton unselectable" height = "35" width = "35" src ="' + this._baseUrl + '/Style Library/Glyma/Icons/nodemenu/content-static.png" / >'
                + '<img id="nodedetailfeed" class="nodedetailbutton unselectable" height = "35" width = "35" src ="' + this._baseUrl + '/Style Library/Glyma/Icons/nodemenu/feed-static.png" / >'
                + '<img id="nodedetaillocate" class="nodedetailbutton unselectable" height = "35" width = "35" src ="' + this._baseUrl + '/Style Library/Glyma/Icons/nodemenu/locate-static.png" / >'
                + '<img id="nodedetailmap" class="nodedetailbutton unselectable" height = "35" width = "35" src ="' + this._baseUrl + '/Style Library/Glyma/Icons/nodemenu/map-static.png" / >'
                + '<img id="nodedetailvideo" class="nodedetailbutton unselectable" height = "35" width = "35" src ="' + this._baseUrl + '/Style Library/Glyma/Icons/nodemenu/video-static.png" / >'
                +'< / div >');

            this._hiddenContainer.append('<li class="breadcrumb-item unselectable" id="breadcrumb-template" map-id="" domain-id="" style="display: none;"><a href="#"></a></li>');

            this._hiddenContainer.append('<div id="errorPanel" class="noti"><h2 class="whatnow" id = "errormessage" ><b>We are unable to find your map, please try again later.</b >< / h2 ><div class="logos" ><div class="b bg" ><a class="l" href = "http://www.glyma.co/" target = "_blank" ><span class="bro" >< / span ><span class="vendor" ></span></a >< / div >< / div ><h2 class="whatnow" >For more information, please < a href = "http://glyma.co/About/HowCanIHaveThisInMyOrganisation" target = "_blank" > contact us </a >.</h2 >< / div >');

            this._hiddenContainer.append('<div id="noti" class="noti"><h2 class="whatnow" ><b>Unfortunately, your browser does not support Glyma.You have following options available to get it working.</b >< / h2 ><div class="update-options" ><div class="update-option" id = "install-plugin" ><div class="update-option-title" > Install Browser Plugin </div ><div class="update-option-text" > For authoring maps < /div></div ><div class="update-option" id = "upgrade-browser" ><div class="update-option-title" > Upgrade Browser </div ><div class="update-option-text" > For all other users < /div></div >< / div ><div class="logos" ><div class="b bs" ><a class="l" href = "http://www.microsoft.com/silverlight/" target = "_blank" ><span class="bro" > Microsoft Silverlight </span >   <span class="vendor" > Microsoft < /span></a >< / div ><div class="b bor" ><span class="orspan" > OR < /span></div ><div class="b bf" ><a class="l" href = "http://www.mozilla.com/firefox/" target = "_blank" ><span class="bro" > Firefox </span >   <span class="vendor" > Mozilla Foundation < /span></a >< / div ><div class="b bc" ><a class="l" href = "http://www.google.com/chrome?hl=en" target = "_blank" ><span class="bro" > Chrome </span >   <span class="vendor" > Google < /span>           </a >< / div ><div class="b bi" ><a class="l" href = "http://windows.microsoft.com/en-US/internet-explorer/downloads/ie" target = "_blank" ><span class="bro" > Internet Explorer </span >   <span class="vendor" > Microsoft < /span>        </a >< / div >< / div ><h2 class="whatnow" >For more information, please < a href = "http://glyma.co/About/HowCanIHaveThisInMyOrganisation" target = "_blank" > contact us < /a>.</h2 >< / div >');

            this._hiddenContainer.append('<div id="mapselect" class="noti unselectable"><div class="div-row dialog-head unselectable" ><div class="float-left dialog-text unselectable" ><span>Map Selection </span ></div><div class="float-left  unselectable"><div class="close-icon unselectable" onclick="$(\'#loader\').fadeOut(\'slow\');"></div >< / div >< / div ><div class="div-row dialog-body" ><div class="dialog-content" ><div class="dialog-content-head" > <span>Select map to load < /span></div ><div class="div-row dialog-content-row" ><div class="float-left select-label" > Project Name </div ><div class="float-left select-box" ><select id = "domains" name = "projects" ></select></div >< / div ><div class="div-row dialog-content-row" ><div class="float-left select-label" > Map Name </div ><div class="float-left select-box" ><select id = "maps" name = "maps" ></select></div >< / div >< / div ><div class="dialog-actions" ><a href = "javascript:void(0)" onclick="alert($(\'#domains\').val() + \' \' + $(\'#maps\').val());" class="glyma-button ui-link" > Load </a >< / div >< / div >< / div >');
        }
    }
}