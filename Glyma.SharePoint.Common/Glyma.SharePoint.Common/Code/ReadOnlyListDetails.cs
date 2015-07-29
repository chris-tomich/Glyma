using System;
using Microsoft.SharePoint;

namespace Glyma.SharePoint.Common
{
   /// <summary>
   /// Provides read-only details for a SharePoint list.
   /// </summary>
   public class ReadOnlyListDetails
   {
      private ListDetails _listDetails;


      public ReadOnlyListDetails(string urlName, SPListTemplateType templateType, string title, string description, bool enforceUniqueValues)
      {
         _listDetails = new ListDetails(urlName, templateType, title, description, enforceUniqueValues);
      }


      public string UrlName
      {
         get
         {
            return _listDetails.UrlName;
         }
      }


      public SPListTemplateType TemplateType
      {
         get
         {
            return _listDetails.TemplateType;
         }
      }


      public string Title
      {
         get
         {
            return _listDetails.Title;
         }
      }


      public string Description
      {
         get
         {
            return _listDetails.Description;
         }
      }


      public bool EnforceUniqueValues
      {
         get
         {
            return _listDetails.EnforceUniqueValues;
         }
      }


      /// <summary>
      /// Gets the server-relative URL for the list in a specified site.
      /// </summary>
      /// <param name="site">An SPWeb object of a site to use to create the server-relative URL.</param>
      /// <returns>A string representing the server-relative URL of the list in the specified site.</returns>
      public string GetServerRelativeUrl(SPWeb site)
      {
         return _listDetails.GetServerRelativeUrl(site);
      }
   }
}
