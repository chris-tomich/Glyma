(function (glyma, $, undefined) {

   glyma.renderSearchResults = function() {  
      glyma.renderParentNodes();
      $("#CSR").on("click", "a.glymaResultChildrenToggle", glyma.renderChildNodes);
   };
   
   
   glyma.renderParentNodes = function () {
      // Render the parent nodes of search results.
      $(".glymaResultContext").each(
         function(index) {
            var resultId = $(this).attr("id");
            
            if (glymaResultsData) {
               var resultData = glymaResultsData[resultId];
               if (resultData && resultData.parentNodes) {
                  var parentNodes = resultData.parentNodes;
                  var indentCssClass = parentNodes ? "glymaResultOneLevelIndent" : "";
                  var parentNodesHtml = "";
                  for (var nodeIndex = 0; nodeIndex < parentNodes.length; nodeIndex++) {
                     parentNodesHtml += "<div class='glymaParentNode " + indentCssClass + "'>" + 
                                             "<img class='glymaNodeIcon' src='" + glymaIconPath + "/" + glyma.getNodeTypeIconFileName(parentNodes[nodeIndex].NodeType) + "' alt='Parent Node Type' />" + 
                                             "<div class='glymaNodeName'>" + parentNodes[nodeIndex].Name + "</div>" +
                                          "</div>";
                  }
                  $(this).append(parentNodesHtml);
               }
            }
         }
      );
   };
     
   
   glyma.renderChildNodes = function (event) {
      var resultId = $(this).attr("data-resultid");
      var childNodesContainer = $("#" + resultId + "Children");
      
      if (childNodesContainer) {
         if (childNodesContainer.attr("data-isrendered") === "false") {                                
            if (glymaResultsData) {
               var resultData = glymaResultsData[resultId];
               if (resultData && resultData.childNodes) {
                  var childNodes = resultData.childNodes;
                  var childNodesHtml = "";
                  for (var nodeIndex = 0; nodeIndex < childNodes.length; nodeIndex++) {
                     childNodesHtml += "<div class='glymaChildNode'>" + 
                                          "<img class='glymaNodeIcon' src='" + glymaIconPath + "/" + glyma.getNodeTypeIconFileName(childNodes[nodeIndex].NodeType) + "' alt='Child Node Type' />" + 
                                          "<div class='glymaNodeName'>" + childNodes[nodeIndex].Name + "</div>" + 
                                       "</div>";
                  }
                  childNodesContainer.append(childNodesHtml);
                  childNodesContainer.attr("data-isrendered", "true"); 
                  childNodesContainer.show();
               }
            }            
         }
         else {
            childNodesContainer.toggle();
         }
         
         if (childNodesContainer.is(":visible")) {
            $(this).children("img").attr("src", "/_layouts/images/minus.gif");   
         }
         else {
            $(this).children("img").attr("src", "/_layouts/images/plus.gif");
         }
      }
   };
   
   
   glyma.getNodeTypeIconFileName = function(nodeType) {
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
   
} (window.glyma = window.glyma || {}, jQuery));
_spBodyOnLoadFunctionNames.push("glyma.renderSearchResults");