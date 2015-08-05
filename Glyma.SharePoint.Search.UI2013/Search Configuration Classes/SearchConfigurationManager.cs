using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.SharePoint;
using Microsoft.Office.Server.Search.Administration;

namespace Glyma.SharePoint.Search.UI
{
   public class SearchConfigurationManager
   {
      public readonly static string GlymaDisplayTemplateUrl = "~sitecollection/_catalogs/masterpage/Display Templates/Search/Glyma/Item_Glyma.js";

      SPSite _siteCollection = null;
      SearchServiceApplicationProxy _searchService = null;
      SearchObjectOwner _searchOwner = null;
      ResultItemTypeManager _resultTypeManager = null;


      public SearchConfigurationManager(SPSite siteCollection)
      {
         _siteCollection = siteCollection;

         _searchService = GetSearchServiceApplicationProxy();
         _searchOwner = new SearchObjectOwner(SearchObjectLevel.SPSite, _siteCollection.RootWeb);
         _resultTypeManager = new ResultItemTypeManager(_searchService);
      }


      public void EnsureGlymaResultTypesSynced()
      {
         string displayProperties = GetDisplayTemplateDisplayProperties(GlymaDisplayTemplateUrl);

         foreach (ResultItemType currentResultType in _resultTypeManager.GetResultItemTypes(_searchOwner, false))
         {
            if (currentResultType.DisplayTemplateUrl.Equals(GlymaDisplayTemplateUrl, StringComparison.OrdinalIgnoreCase))
            {
               // Ensure the managed properties of the result type is always in-sync with the display template referenced by the DisplayTemplateUrl property.
               if (!currentResultType.DisplayProperties.Equals(displayProperties, StringComparison.OrdinalIgnoreCase))
               {
                  currentResultType.DisplayProperties = displayProperties;
                  _resultTypeManager.UpdateResultItemType(currentResultType);
               }
            }
         }
      }


      public SearchServiceApplicationProxy GetSearchServiceApplicationProxy()
      {
         SPServiceContext serviceContext = SPServiceContext.GetContext(_siteCollection);
         SearchServiceApplicationProxy defaultProxy = serviceContext.GetDefaultProxy(typeof(SearchServiceApplicationProxy)) as SearchServiceApplicationProxy;
         if (defaultProxy == null)
         {
            throw new ApplicationException("Search service application proxy not found.");
         }
         return defaultProxy;
      }


      public string GetDisplayTemplateDisplayProperties(string displayTemplateUrl)
      {
         string displayProperties = string.Empty;

         SPListItem glymaDisplayTemplateItem = GetDisplayTemplateItem(displayTemplateUrl);
         if (glymaDisplayTemplateItem == null)
         {
            throw new ApplicationException("The display template was not found at the URL: " + displayTemplateUrl + ".");
         }

         string managedPropertyMapping = glymaDisplayTemplateItem["ManagedPropertyMapping"] as string;
         if (managedPropertyMapping == null)
         {
            throw new ApplicationException("Unable to retrieve the managed property mapping for the diplay template: " + glymaDisplayTemplateItem.Url + ".");
         }
         displayProperties = SearchConfigurationManager.ParseManagePropertyMappings(managedPropertyMapping);

         return displayProperties;
      }


      public SPListItem GetDisplayTemplateItem(string displayTemplateUrl)
      {
         SPListItem displayTemplateItem = null;

         // Using the method, GetFileOrFolderObject, provides a more consistent behaviour than GetFile because it doesn't rely on the URL used to create the SPWeb object (according to http://blog.mastykarz.nl/inconvenient-spwebgetfilestring/).
         object foundObject = _siteCollection.RootWeb.GetFileOrFolderObject(displayTemplateUrl.Replace("~sitecollection/", _siteCollection.ServerRelativeUrl.TrimEnd('/') + "/"));
         if (foundObject is SPFile)
         {
            displayTemplateItem = ((SPFile)foundObject).Item;
         }

         return displayTemplateItem;
      }


      private static string ParseManagePropertyMappings(string mappings)
      {
         string[] properties = mappings.Replace("'", "").Replace("\"", "").Split(new char[] { ',' });
         for (int propertyIndex = 0; propertyIndex < properties.Length; propertyIndex++)
         {
            if (properties[propertyIndex].Contains(":"))
            {
               int separatorIndex = properties[propertyIndex].LastIndexOf(':');
               if ((separatorIndex > 0) && (separatorIndex < (properties[propertyIndex].Length - 1)))
               {
                  properties[propertyIndex] = properties[propertyIndex].Substring(separatorIndex + 1);
               }
            }
            properties[propertyIndex] = properties[propertyIndex].Replace(";", ",");
         }
         return string.Join(",", properties);
      }

   }
}
