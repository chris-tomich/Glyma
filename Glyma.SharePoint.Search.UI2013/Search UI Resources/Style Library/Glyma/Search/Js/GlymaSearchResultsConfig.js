(function (glyma, $, undefined) {

   // The following definition is used in scenarios where all search results navigate to a single Glyma page to show the map containing it.
   glyma.viewerPageUrl = "~sitecollection/Pages/glyma.aspx";
   
   // The following definition is used in scenarios where search results can navigate to one of multiple Glyma pages (depending on which repository it came from) to show the map containing it.
   //glyma.viewerPageUrls = {};
   //glyma.viewerPageUrls["RepositoryName"] = "~sitecollection/Pages/glyma.aspx";
     
} (window.glyma = window.glyma || {}, jQuery));
