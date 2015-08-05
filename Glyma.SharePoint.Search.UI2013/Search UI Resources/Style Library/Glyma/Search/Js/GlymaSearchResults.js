(function (glyma, $, undefined) {

   glyma.trimEnd = function (inputString, character) {
      if (inputString) {
         if (inputString.slice(-1) === character) {
            inputString = inputString.substr(0, inputString.length - 1);
         }
      }
      return inputString;
   };


   glyma.getSiteCollectionUrl = function () {
      var url = "";

      if (_spPageContextInfo && _spPageContextInfo.siteServerRelativeUrl) {
         url = _spPageContextInfo.siteServerRelativeUrl;
      }

      return url;
   };


   glyma.getViewerPageUrl = function (repositoryName) {
      var viewerPageUrl = "";
      var siteCollectionUrl = glyma.getSiteCollectionUrl()
      siteCollectionUrl = siteCollectionUrl === '/' ? '' : siteCollectionUrl

      if (glyma.viewerPageUrl) {
         viewerPageUrl = glyma.viewerPageUrl.replace(/~sitecollection/gi, siteCollectionUrl);
      }
      else if (glyma.viewerPageUrls && repositoryName) {
         var repositoryViewerPageUrl = glyma.viewerPageUrls[repositoryName];
         if (repositoryViewerPageUrl) {
            viewerPageUrl = repositoryViewerPageUrl.replace(/~sitecollection/gi, siteCollectionUrl);
         }
      }

      return viewerPageUrl;
   };


   glyma.getNodeTypeIconFileName = function (nodeType) {
      var fileName = "";
      switch (nodeType.toUpperCase()) {
         case 'MAP':
            fileName = 'map-21x21';
            break;
         case 'QUESTION':
            fileName = 'question-21x21';
            break;
         case 'IDEA':
            fileName = 'idea-21x21';
            break;
         case 'DECISION':
            fileName = 'decision-21x21';
            break;
         case 'PRO':
            fileName = 'pro-21x21';
            break;
         case 'CON':
            fileName = 'cons-21x21';
            break;
         case 'NOTE':
            fileName = 'note-21x21';
            break;
         default:
            fileName = 'note-21x21';
      }
      fileName += '.png';

      return fileName;
   };


   glyma.isRootMap = function (mapId) {
      return mapId === "00000000-0000-0000-0000-000000000000";
   };


   glyma.getNodeTypeIconUrl = function (nodeType) {
      var siteCollectionUrl = glyma.getSiteCollectionUrl();
      var iconUrl = glyma.trimEnd(siteCollectionUrl, "/") + "/Style Library/Glyma/Icons/" + glyma.getNodeTypeIconFileName(nodeType);

      return iconUrl;
   };


   glyma.getNodeQueryString = function (domainId, mapId, nodeId) {
      var queryString = "";

      if (domainId && mapId && nodeId) {
         queryString = "DomainUid=" + domainId;

         if (glyma.isRootMap(mapId)) {
            queryString += "&MapUid=" + nodeId;
         }
         else {
            queryString += "&MapUid=" + mapId + "&NodeUid=" + nodeId;
         }
      }
      return queryString;
   };

}(window.glyma = window.glyma || {}, jQuery));
