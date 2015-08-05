/* This file is currently associated to an HTML file of the same name and is drawing content from it.  Until the files are disassociated, you will not be able to move, delete, rename, or make any other changes to this file. */

function DisplayTemplate_ff8f5958569c49048f09acb6df5b5c9f(ctx) {
  var ms_outHtml=[];
  var cachePreviousTemplateData = ctx['DisplayTemplateData'];
  ctx['DisplayTemplateData'] = new Object();
  DisplayTemplate_ff8f5958569c49048f09acb6df5b5c9f.DisplayTemplateData = ctx['DisplayTemplateData'];

  ctx['DisplayTemplateData']['TemplateUrl']='~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fSearch\u002fGlyma\u002fItem_Glyma.js';
  ctx['DisplayTemplateData']['TemplateType']='Item';
  ctx['DisplayTemplateData']['TargetControlType']=['SearchResults'];
  this.DisplayTemplateData = ctx['DisplayTemplateData'];

  ctx['DisplayTemplateData']['ManagedPropertyMapping']={'Title':['Title'], 'Path':['Path'], 'Description':['Description'], 'EditorOWSUSER':['EditorOWSUSER'], 'LastModifiedTime':['LastModifiedTime'], 'CollapsingStatus':['CollapsingStatus'], 'DocId':['DocId'], 'HitHighlightedSummary':['HitHighlightedSummary'], 'HitHighlightedProperties':['HitHighlightedProperties'], 'FileExtension':['FileExtension'], 'ViewsLifeTime':['ViewsLifeTime'], 'ParentLink':['ParentLink'], 'FileType':['FileType'], 'IsContainer':['IsContainer'], 'SecondaryFileExtension':['SecondaryFileExtension'], 'DisplayAuthor':['DisplayAuthor'], 'GlymaRepositoryName':['GlymaRepositoryName'], 'GlymaNodeType':['GlymaNodeType'], 'GlymaDomainId':['GlymaDomainId'], 'GlymaDomainName':['GlymaDomainName'], 'GlymaMapId':['GlymaMapId'], 'GlymaMapName':['GlymaMapName'], 'GlymaNodeId':['GlymaNodeId'], 'GlymaContent':['GlymaContent'], 'GlymaDescription':['GlymaDescription'], 'GlymaDescriptionType':['GlymaDescriptionType'], 'GlymaParentNodes':['GlymaParentNodes'], 'GlymaChildNodes':['GlymaChildNodes'], 'GlymaNote':['GlymaNote']};
  var cachePreviousItemValuesFunction = ctx['ItemValues'];
  ctx['ItemValues'] = function(slotOrPropName) {
    return Srch.ValueInfo.getCachedCtxItemValue(ctx, slotOrPropName)
};

ms_outHtml.push('','	'
);

   if(!$isNull(ctx.CurrentItem) && !$isNull(ctx.ClientControl))
   {
      var id = ctx.ClientControl.get_nextUniqueId();
      var itemId = id + Srch.U.Ids.item;
	   var hoverId = id + Srch.U.Ids.hover;
	   var hoverUrl = "~sitecollection/_catalogs/masterpage/Display Templates/Search/Glyma/Item_Glyma_HoverPanel.js";
      $setResultItem(itemId, ctx.CurrentItem);
      if(ctx.CurrentItem.IsContainer)
      {
		   ctx.CurrentItem.csr_Icon = Srch.U.getFolderIconUrl();
	   }
	   ctx.currentItem_ShowHoverPanelCallback = Srch.U.getShowHoverPanelCallback(itemId, hoverId, hoverUrl);
      ctx.currentItem_HideHoverPanelCallback = Srch.U.getHideHoverPanelCallback();
              
      var appAttribs = "";
      if (!$isEmptyString(ctx.CurrentItem.csr_OpenApp)) 
      {
         appAttribs += "openApp=\"" + $htmlEncode(ctx.CurrentItem.csr_OpenApp) + "\""; 
      }
      
      if (!$isEmptyString(ctx.CurrentItem.csr_OpenControl)) 
      {
         appAttribs += " openControl=\"" + $htmlEncode(ctx.CurrentItem.csr_OpenControl) + "\""; 
      }
      
      var showHoverPanelCallback = ctx.currentItem_ShowHoverPanelCallback;
      if (Srch.U.n(showHoverPanelCallback)) 
      {
         var itemId = id + Srch.U.Ids.item;
         var hoverId = id + Srch.U.Ids.hover;
         showHoverPanelCallback = Srch.U.getShowHoverPanelCallback(itemId, hoverId, hoverUrl);
      }
                 
      var siteCollectionUrl = glyma.getSiteCollectionUrl();      
      var glymaViewerPageUrl = glyma.getViewerPageUrl(ctx.CurrentItem.GlymaRepositoryName);
      var nodeQueryString = glyma.getNodeQueryString(ctx.CurrentItem.GlymaDomainId, ctx.CurrentItem.GlymaMapId, ctx.CurrentItem.GlymaNodeId);
      if (nodeQueryString) {
         glymaViewerPageUrl += "?" + nodeQueryString;
      }
      var nodeIconUrl = glyma.getNodeTypeIconUrl(ctx.CurrentItem.GlymaNodeType);              
      var mapIconUrl = glyma.getNodeTypeIconUrl("map"); 
         
      var clickType = ctx.CurrentItem.csr_ClickType;
      if(!clickType) 
      {
         clickType = "Result";
      } 
         
      var maxTitleLengthInChars = Srch.U.titleTruncationLength;
      var termsToUse = 2;
      if(ctx.CurrentItem.csr_PreviewImage != null)
      {
         maxTitleLengthInChars = Srch.U.titleTruncationLengthWithPreview;
         termsToUse = 1;
      }
         
      var title = Srch.U.getHighlightedProperty(id, ctx.CurrentItem, "Title");
      if ($isEmptyString(title)) 
      {
         title = $htmlEncode(ctx.CurrentItem.Title);
      }     
      var titleHtml = String.format('<a clicktype="{0}" id="{1}" href="{2}" class="ms-srch-item-link" title="{3}" onfocus="{4}" {5}>{6}</a>',
   			                  $htmlEncode(clickType), $htmlEncode(id + Srch.U.Ids.titleLink), $urlHtmlEncode(glymaViewerPageUrl), $htmlEncode(ctx.CurrentItem.Title), 
                              showHoverPanelCallback, appAttribs, Srch.U.trimTitle(title, maxTitleLengthInChars, termsToUse));                                                                      
ms_outHtml.push('			'
,'   '
,'      <div id="', $htmlEncode(itemId) ,'" name="Item" data-displaytemplate="GlymaItem" class="ms-srch-item glymaSearchResult" onmouseover="', ctx.currentItem_ShowHoverPanelCallback ,'" onmouseout="', ctx.currentItem_HideHoverPanelCallback ,'">'
,'   '
,'         <div id="', $htmlEncode(id + Srch.U.Ids.body) ,'" class="ms-srch-item-body" onclick="', showHoverPanelCallback ,'">'
,'         '
,'            <div class="glymaSearchResultTitle">'
,'               <img id="', $htmlEncode(id + Srch.U.Ids.icon) ,'" onload="this.style.display=\'inline\'" src="', $urlHtmlEncode(nodeIconUrl) ,'" class="glymaNodeIcon" />'
,'             '
,'               <div id="', $htmlEncode(id + Srch.U.Ids.title) ,'" class="ms-srch-item-title"> '
,'                  <h3 class="ms-srch-ellipsis">'
,'      					', titleHtml ,''
,'      				</h3>'
,'      			</div>	'
,'   			</div>'
,'   			'
,'   			<div class="glymaSearchResultMetadata">'
,'   			'
,'      			<div class="glymaSearchResultMetadataItem">'
,'      			   <span class="glymaSearchResultMetadataLabel">Project: </span><span class="glymaSearchResultMetadataValue">', ctx.CurrentItem.GlymaDomainName ,'</span>'
,'        			</div>		'
); 
   	if (!$isEmptyString(ctx.CurrentItem.GlymaMapName)) 
   	{ 
ms_outHtml.push('    			'
,'      			<div class="glymaSearchResultMetadataItem">'
,'   		         <img class="glymaNodeIcon" src="', mapIconUrl ,'" alt="Map Node" />'
,'                  <div class="glymaNodeName">', ctx.CurrentItem.GlymaMapName ,'</div> '
,'        			</div>     		'
); 
      }
   	if (!$isEmptyString(ctx.CurrentItem.GlymaNote)) 
   	{ 
ms_outHtml.push(''
,'      			<div class="glymaSearchResultMetadataItem">'
,'      			   <span class="glymaSearchResultMetadataValue">'
,'      			   ', ctx.CurrentItem.GlymaNote.substr(0,200) ,''
); 
   	if (ctx.CurrentItem.GlymaNote.length > 200) 
   	{ 
ms_outHtml.push(' 			   '
,'   			   ...'
); 
      }
ms_outHtml.push('    			   '
,'      			   </span>'
,'        			</div> 	'
); 
      }
   	if (!$isEmptyString(ctx.CurrentItem.HitHighlightedSummary)) 
   	{ 
ms_outHtml.push(''
,'      			<div id="', $htmlEncode(id + Srch.U.Ids.summary) ,'" class="ms-srch-item-summary glymaSearchResultMetadataItem">'
,'      			   ', Srch.U.processHHXML(ctx.CurrentItem.HitHighlightedSummary) ,''
,'      			</div>'
); 
	   }
ms_outHtml.push('  	    '
,'   		   </div>'
,'         </div>'
,'         <div id="', $htmlEncode(hoverId) ,'" class="ms-srch-hover-outerContainer"></div>'
,'      </div>'
);
	}
ms_outHtml.push(''
,'   '
);

  ctx['ItemValues'] = cachePreviousItemValuesFunction;
  ctx['DisplayTemplateData'] = cachePreviousTemplateData;
  return ms_outHtml.join('');
}
function RegisterTemplate_ff8f5958569c49048f09acb6df5b5c9f() {

if ("undefined" != typeof (Srch) &&"undefined" != typeof (Srch.U) &&typeof(Srch.U.registerRenderTemplateByName) == "function") {
  Srch.U.registerRenderTemplateByName("Item_Glyma", DisplayTemplate_ff8f5958569c49048f09acb6df5b5c9f);
}

if ("undefined" != typeof (Srch) &&"undefined" != typeof (Srch.U) &&typeof(Srch.U.registerRenderTemplateByName) == "function") {
  Srch.U.registerRenderTemplateByName("~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fSearch\u002fGlyma\u002fItem_Glyma.js", DisplayTemplate_ff8f5958569c49048f09acb6df5b5c9f);
}
//     
      $includeScript("~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fSearch\u002fGlyma\u002fItem_Glyma.js", "~sitecollection/Style Library/Glyma/Search/Js/GlymaSearchResults.js");
      $includeScript("~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fSearch\u002fGlyma\u002fItem_Glyma.js", "~sitecollection/Style Library/Glyma/Search/Js/GlymaSearchResultsConfig.js");          
   //
}
RegisterTemplate_ff8f5958569c49048f09acb6df5b5c9f();
if (typeof(RegisterModuleInit) == "function" && typeof(Srch.U.replaceUrlTokens) == "function") {
  RegisterModuleInit(Srch.U.replaceUrlTokens("~sitecollection\u002f_catalogs\u002fmasterpage\u002fDisplay Templates\u002fSearch\u002fGlyma\u002fItem_Glyma.js"), RegisterTemplate_ff8f5958569c49048f09acb6df5b5c9f);
}