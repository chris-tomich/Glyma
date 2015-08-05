/* This file is currently associated to an HTML file of the same name and is drawing content from it.  Until the files are disassociated, you will not be able to move, delete, rename, or make any other changes to this file. */

function DisplayTemplate_68e5c56bcb534aea85a2d2129637d1a9(ctx) {
  var ms_outHtml=[];
  var cachePreviousTemplateData = ctx['DisplayTemplateData'];
  ctx['DisplayTemplateData'] = new Object();
  DisplayTemplate_68e5c56bcb534aea85a2d2129637d1a9.DisplayTemplateData = ctx['DisplayTemplateData'];

  ctx['DisplayTemplateData']['TemplateUrl']='~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fSearch\u002fGlyma\u002fItem_Glyma_HoverPanel.js';
  ctx['DisplayTemplateData']['TemplateType']='Item';
  ctx['DisplayTemplateData']['TargetControlType']=['SearchHoverPanel'];
  this.DisplayTemplateData = ctx['DisplayTemplateData'];

  ctx['DisplayTemplateData']['ManagedPropertyMapping']={'Title':['Title'], 'Path':['Path'], 'Description':['Description'], 'EditorOWSUSER':['EditorOWSUSER'], 'LastModifiedTime':['LastModifiedTime'], 'CollapsingStatus':['CollapsingStatus'], 'DocId':['DocId'], 'HitHighlightedSummary':['HitHighlightedSummary'], 'HitHighlightedProperties':['HitHighlightedProperties'], 'FileExtension':['FileExtension'], 'ViewsLifeTime':['ViewsLifeTime'], 'ParentLink':['ParentLink'], 'FileType':['FileType'], 'IsContainer':['IsContainer'], 'SecondaryFileExtension':['SecondaryFileExtension'], 'DisplayAuthor':['DisplayAuthor']};
  var cachePreviousItemValuesFunction = ctx['ItemValues'];
  ctx['ItemValues'] = function(slotOrPropName) {
    return Srch.ValueInfo.getCachedCtxItemValue(ctx, slotOrPropName)
};

ms_outHtml.push('',''
);
   var i = 0;
   var id = ctx.CurrentItem.csr_id;
   ctx.CurrentItem.csr_ShowViewLibrary = !Srch.U.isWebPage(ctx.CurrentItem.FileExtension);
   if(ctx.CurrentItem.IsContainer)
   {
      ctx.CurrentItem.csr_FileType = Srch.Res.ct_Folder;
   }
   
   ctx.currentItem_ShowChangedBySnippet = true;
   
   var siteCollectionUrl = glyma.getSiteCollectionUrl();   
   var nodeIconUrl = glyma.getNodeTypeIconUrl(ctx.CurrentItem.GlymaNodeType);
   var mapIconUrl = glyma.getNodeTypeIconUrl("map");
   var resultNodeIndentCssClass = "";
   if (ctx.CurrentItem.GlymaParentNodes)
   {
      resultNodeIndentCssClass = "glymaSearchResultTwoLevelIndent";
   }
   else if (ctx.CurrentItem.GlymaMapName)
   {
      resultNodeIndentCssClass = "glymaSearchResultOneLevelIndent";   
   }

ms_outHtml.push(''
,'      <div class="ms-srch-hover-innerContainer ms-srch-hover-standardSize glyamSearchResultHover" id="', $htmlEncode(id + HP.ids.inner) ,'">'
,'         <div class="ms-srch-hover-arrowBorder" id="', $htmlEncode(id + HP.ids.arrowBorder) ,'"></div>'
,'         <div class="ms-srch-hover-arrow" id="', $htmlEncode(id + HP.ids.arrow) ,'"></div>'
,'         <div class="ms-srch-hover-content" id="', $htmlEncode(id + HP.ids.content) ,'" data-displaytemplate="GlymaHoverPanel">'
,'         '
,'            <div id="', $htmlEncode(id + HP.ids.header) ,'" class="ms-srch-hover-header">               '
,'               <div>'
,'                  <div class="ms-srch-hover-close">'
,'                      <a href="javascript:HP.Close()" id="', $htmlEncode(id + HP.ids.close) ,'" title="', $htmlEncode(Srch.Res.hp_Tooltip_Close) ,'">'
,'                          <img class="js-callout-closeButtonImage" src="', $urlHtmlEncode(GetThemedImageUrl('spcommon.png')) ,'" alt="', $htmlEncode(Srch.Res.hp_Tooltip_Close) ,'" />'
,'                      </a>'
,'                  </div>               '
,'                  <div class="ms-srch-hover-title ms-dlg-heading ms-srch-ellipsis" id="', $htmlEncode(id + HP.ids.title) ,'">'
,'                     Glyma Context'
,'                  </div>'
,'               </div>'
,'            </div>'
,''
,'            <div id="', $htmlEncode(id + HP.ids.body) ,'" class="ms-srch-hover-body">'
,'               <div class="glymaSearchResultContext">'
,'                  <div class="glymaSearchResultParentNode glymaSearchResultParentDomainNode">'
,'                     <span class="glymaSearchResultMetadataLabel">Project: </span><span class="glymaSearchResultMetadataValue">', ctx.CurrentItem.GlymaDomainName ,'</span>'
,'                  </div>                     '
); 
   if (ctx.CurrentItem.GlymaMapName)
   {   
ms_outHtml.push('                    '
,'                  <div class="glymaSearchResultParentNode glymaSearchResultParentMapNode">'
,'                     <img class="glymaNodeIcon" src="', mapIconUrl ,'" alt="Map Node" />'
,'                     <div class="glymaNodeName">', ctx.CurrentItem.GlymaMapName ,'</div>                        '
,'                  </div>  '
);  
   }
   
   var glymaViewerPageUrl = glyma.getViewerPageUrl(ctx.CurrentItem.GlymaRepositoryName);
   var nodeQueryString = "";
   
   var currentNodeUrl = glymaViewerPageUrl;
   nodeQueryString = glyma.getNodeQueryString(ctx.CurrentItem.GlymaDomainId, ctx.CurrentItem.GlymaMapId, ctx.CurrentItem.GlymaNodeId);
   if (nodeQueryString) {
      currentNodeUrl += "?" + nodeQueryString;
   }

   if (ctx.CurrentItem.GlymaParentNodes) 
   {
      var parentNodes = $.parseJSON(ctx.CurrentItem.GlymaParentNodes);
      var parentNodeIconUrl = "";
      var parentNodeName = "";
      var parentNodeUrl = "";
         
      for (var nodeIndex = 0; nodeIndex < parentNodes.length; nodeIndex++) 
      {
         parentNodeIconUrl = glyma.getNodeTypeIconUrl(parentNodes[nodeIndex].NodeType);
         parentNodeName = parentNodes[nodeIndex].Name;  
         parentNodeUrl = glymaViewerPageUrl;      
         nodeQueryString = glyma.getNodeQueryString(parentNodes[nodeIndex].DomainId, parentNodes[nodeIndex].MapId, parentNodes[nodeIndex].Id);
         if (nodeQueryString) {
            parentNodeUrl += "?" + nodeQueryString;
         }
ms_outHtml.push('       '
,'                  <div class="glymaSearchResultParentNode glymaSearchResultOneLevelIndent"> '
,'                     <img class="glymaNodeIcon" src="', parentNodeIconUrl ,'" alt="Parent Node Type" /> '
,'                     <div class="glymaNodeName"><a clicktype="Result" href="', $urlHtmlEncode(parentNodeUrl) ,'">', $htmlEncode(parentNodeName) ,'</a></div>'
,'                  </div>'
);                                
      }
   }
ms_outHtml.push('                                   '
,'               </div>  '
,'               <div class="glymaSearchResultNode ', resultNodeIndentCssClass ,'">'
,'                  <div class="glymaSearchResultHoverTitle">'
,'                     <img class="glymaNodeIcon" src="', nodeIconUrl ,'" alt="Node Type" />   '
,'                     <div class="glymaNodeName"> '
,'                        <a clicktype="Result" href="', $urlHtmlEncode(currentNodeUrl) ,'">', $htmlEncode(ctx.CurrentItem.Title) ,'</a>'
,'                     </div>                         '
,'                  </div>'
);  
   if (ctx.CurrentItem.GlymaChildNodes) 
   {
      var childNodes = $.parseJSON(ctx.CurrentItem.GlymaChildNodes);
      var childNodeIconUrl = "";
      var childNodeName = "";
      var childNodeUrl = "";   
      var additionalCssClass = "";
ms_outHtml.push('       '
,'                  <div class="glymaSearchResultChildNodes">'
);      
      for (var nodeIndex = 0; nodeIndex < childNodes.length; nodeIndex++) 
      {
         childNodeIconUrl = glyma.getNodeTypeIconUrl(childNodes[nodeIndex].NodeType);
         childNodeName = childNodes[nodeIndex].Name;
         additionalCssClass = nodeIndex % 2 == 0 ? "" : "glymaSearchResultAlternate";
         childNodeUrl = glymaViewerPageUrl;      
         nodeQueryString = glyma.getNodeQueryString(childNodes[nodeIndex].DomainId, childNodes[nodeIndex].MapId, childNodes[nodeIndex].Id);
         if (nodeQueryString) {
            childNodeUrl += "?" + nodeQueryString;
         }                 
ms_outHtml.push('                       '
,'                     <div class="glymaSearchResultChildNode ', additionalCssClass,'">'
,'                        <img class="glymaNodeIcon" src="', childNodeIconUrl ,'" alt="Child Node Type" />'
,'                        <div class="glymaNodeName"><a clicktype="Result" href="', $urlHtmlEncode(childNodeUrl) ,'">', $htmlEncode(childNodeName) ,'</a></div>  '
,'                     </div>               '
);                                
      }
ms_outHtml.push('       '
,'                  </div>'
);       
   }
ms_outHtml.push('         '
,'               </div>              '
,'            </div>			'
,'         </div> '
,'      </div>'
,'   '
);

  ctx['ItemValues'] = cachePreviousItemValuesFunction;
  ctx['DisplayTemplateData'] = cachePreviousTemplateData;
  return ms_outHtml.join('');
}
function RegisterTemplate_68e5c56bcb534aea85a2d2129637d1a9() {

if ("undefined" != typeof (Srch) &&"undefined" != typeof (Srch.U) &&typeof(Srch.U.registerRenderTemplateByName) == "function") {
  Srch.U.registerRenderTemplateByName("Item_Glyma_HoverPanel", DisplayTemplate_68e5c56bcb534aea85a2d2129637d1a9);
}

if ("undefined" != typeof (Srch) &&"undefined" != typeof (Srch.U) &&typeof(Srch.U.registerRenderTemplateByName) == "function") {
  Srch.U.registerRenderTemplateByName("~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fSearch\u002fGlyma\u002fItem_Glyma_HoverPanel.js", DisplayTemplate_68e5c56bcb534aea85a2d2129637d1a9);
}

}
RegisterTemplate_68e5c56bcb534aea85a2d2129637d1a9();
if (typeof(RegisterModuleInit) == "function" && typeof(Srch.U.replaceUrlTokens) == "function") {
  RegisterModuleInit(Srch.U.replaceUrlTokens("~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fSearch\u002fGlyma\u002fItem_Glyma_HoverPanel.js"), RegisterTemplate_68e5c56bcb534aea85a2d2129637d1a9);
}