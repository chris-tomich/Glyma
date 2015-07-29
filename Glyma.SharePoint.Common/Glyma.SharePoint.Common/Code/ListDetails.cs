using System;
using Microsoft.SharePoint;

namespace Glyma.SharePoint.Common
{
   /// <summary>
   /// Provides details for a SharePoint list.
   /// </summary>
   public class ListDetails
   {
      public string UrlName { get; set; }
      public SPListTemplateType TemplateType { get; set; }
      public string Title { get; set; }
      public string Description { get; set; }
      public bool EnforceUniqueValues { get; set; }


      public ListDetails(string urlName, SPListTemplateType templateType, string title, string description, bool enforceUniqueValues)
      {
         UrlName = urlName;
         TemplateType = templateType;
         Title = title;
         Description = description;
         EnforceUniqueValues = enforceUniqueValues;
      }


      /// <summary>
      /// Gets the server-relative URL for the list in a specified site.
      /// </summary>
      /// <param name="site">An SPWeb object of a site to use to create the server-relative URL.</param>
      /// <returns>A string representing the server-relative URL of the list in the specified site.</returns>
      public string GetServerRelativeUrl(SPWeb site)
      {
         return site.GetServerRelativeListUrlPrefix() + UrlName;
      }
   }
}
