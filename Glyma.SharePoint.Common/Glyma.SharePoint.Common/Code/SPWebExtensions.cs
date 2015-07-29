using System;
using Microsoft.SharePoint;

namespace Glyma.SharePoint.Common
{
   /// <summary>
   /// Provides extension methods for a SharePoint site.
   /// </summary>
   public static class SPWebExtensions
   {
      /// <summary>
      /// Returns the server relative URL prefix for lists within a site.
      /// </summary>
      /// <param name="site">The site to use.</param>
      /// <returns>A string representing the server relative URL prefix for lists within the site.</returns>
      public static string GetServerRelativeListUrlPrefix(this SPWeb site)
      {
         string listServerRelativeUrlPrefix = site.ServerRelativeUrl.TrimEnd('/') + "/Lists/";
         return listServerRelativeUrlPrefix;
      }


      /// <summary>
      /// Returns the server relative URL prefix for lists within a site.
      /// </summary>
      /// <param name="site">The site to use.</param>
      /// <returns>A string representing the server relative URL prefix for lists within the site.</returns>
      public static string GetListUrlPrefix(this SPWeb site)
      {
         string listUrlPrefix = site.Url.TrimEnd('/') + "/Lists/";
         return listUrlPrefix;
      }


      /// <summary>
      /// Return a reference to a list within the site using its server-relative URL.
      /// </summary>
      /// <param name="site">The site to retrieve the list from.</param>
      /// <param name="serverRelativeUrl">The server relative URL of the list.</param>
      /// <returns>A SPList object representing the list if it found; otherwise, null.</returns>
      public static SPList TryGetList(this SPWeb site, string serverRelativeUrl)
      {
         SPList referenceList = null;

         // Check if the list exists using its URL instead of its Title.
         try
         {
            referenceList = site.GetList(serverRelativeUrl);
         }
         catch (System.IO.FileNotFoundException)
         {
            // The list doesn't exist.
         }
         return referenceList;
      }


      /// <summary>
      /// Checks if a site column exists in a site and creates it if it doesn't exist.
      /// </summary>
      /// <param name="site">The site to check.</param>
      /// <param name="columnDetail">The details of the site column.</param>
      /// <param name="isNewColumn">A boolean that is set to true, if a new column is created; otherwise, it is set to false.</param>
      /// <returns>A SPField object that references an existing or newly created site column.</returns>
      public static SPField EnsureSiteColumn(this SPWeb site, ColumnDetails columnDetail, ref bool isNewColumn)
      {
         isNewColumn = false;
         SPField column = null;
         if (!site.Fields.ContainsField(columnDetail.InternalName))
         {
            site.Fields.AddFieldAsXml(columnDetail.GetXml());
            column = site.Fields.GetFieldByInternalName(columnDetail.InternalName);
            isNewColumn = true;
         }
         else
         {
            column = site.Fields.GetFieldByInternalName(columnDetail.InternalName);
         }
         return column;
      }


      /// <summary>
      /// Checks if a site column exists in a site and creates it if it doesn't exist.
      /// </summary>
      /// <param name="site">The site to check.</param>
      /// <param name="columnId">The GUID of the column to use if it needs to be created.</param>
      /// <param name="columnInternalName">The internal name of the column to check.</param>
      /// <param name="columnDisplayName">The display name of the column to use if it needs to be created.</param>
      /// <param name="columnDescription">The description of the column to use if it needs to be created.</param>
      /// <param name="columnGroup">The column group to add the site column to if it needs to be created.</param>
      /// <param name="columnXml">The XML definition of the site column.</param>
      /// <param name="isNewColumn">A boolean that is set to true, if a new column is created; otherwise, it is set to false.</param>
      /// <returns>A SPField object that references an existing or newly created site column.</returns>
      public static SPField EnsureSiteColumn(this SPWeb site, Guid columnId, string columnInternalName, string columnDisplayName, string columnDescription, SPFieldType columnType, string columnTypeAsString, string columnGroup, ref bool isNewColumn)
      {
         ColumnDetails columnDetail = new ColumnDetails(columnId, columnInternalName, columnDisplayName, columnDescription, columnType, string.Empty, columnTypeAsString, columnGroup);
         return EnsureSiteColumn(site, columnDetail, ref isNewColumn);
      }

      /// <summary>
      /// Checks if a site column exists in a site and creates it if it doesn't exist.
      /// </summary>
      /// <param name="site">The site to check.</param>
      /// <param name="columnId">The GUID of the column to use if it needs to be created.</param>
      /// <param name="columnInternalName">The internal name of the column to check.</param>
      /// <param name="columnDisplayName">The display name of the column to use if it needs to be created.</param>
      /// <param name="columnDescription">The description of the column to use if it needs to be created.</param>
      /// <param name="columnType">The type of the column</param>
      /// <param name="columnChoices">If the type of the column is choice this is the xml to put in the choices</param>
      /// <param name="columnGroup">The column group to add the site column to if it needs to be created.</param>
      /// <param name="columnXml">The XML definition of the site column.</param>
      /// <param name="isNewColumn">A boolean that is set to true, if a new column is created; otherwise, it is set to false.</param>
      /// <returns>A SPField object that references an existing or newly created site column.</returns>
      public static SPField EnsureSiteColumn(this SPWeb site, Guid columnId, string columnInternalName, string columnDisplayName, string columnDescription, SPFieldType columnType, string columnChoices, string columnTypeAsString, string columnGroup, ref bool isNewColumn)
      {
          ColumnDetails columnDetail = new ColumnDetails(columnId, columnInternalName, columnDisplayName, columnDescription, columnType, columnChoices, columnTypeAsString, columnGroup);
          return EnsureSiteColumn(site, columnDetail, ref isNewColumn);
      }


      /// <summary>
      /// Checks if a site column exists in a site and creates it if it doesn't exist.
      /// </summary>
      /// <param name="site">The site to check.</param>
      /// <param name="columnDetail">The details of the site column.</param>
      /// <param name="isNewColumn">A boolean that is set to true, if a new column is created; otherwise, it is set to false.</param>
      /// <returns>A SPField object that references an existing or newly created site column.</returns>
      public static SPField EnsureSiteColumn(this SPWeb site, ReadOnlyColumnDetails columnDetail, ref bool isNewColumn)
      {
         return EnsureSiteColumn(site, new ColumnDetails(columnDetail), ref isNewColumn);
      }


      /// <summary>
      /// Checks if a list exists in a site using its "internal name" and creates it if it doesn't exist.
      /// </summary>
      /// <param name="site">The site to check.</param>
      /// <param name="listUrlName">The "internal name" of the list to check.  This is the name that is used to create the URL for the list.</param>
      /// <param name="listTemplateType">A SPListTemplateType object specifying the type of list to create if it needs to be created.</param>
      /// <param name="listTitle">The display name of the list to use if it needs to be created.</param>
      /// <param name="listDescription">The description of the list to be used if it needs to be created.</param>
      /// <param name="isNewList">A boolean that is set to true, if a new list is created; otherwise, it is set to false.</param>
      /// <returns>A SPList object that references an existing or newly created list.</returns>
      public static SPList EnsureList(this SPWeb site, string listUrlName, SPListTemplateType listTemplateType, string listTitle, string listDescription, ref bool isNewList)
      {
         SPList list = site.TryGetList(site.GetServerRelativeListUrlPrefix() + listUrlName);
         isNewList = false;

         if (list == null)
         {
            isNewList = true;
            Guid listGuid = site.Lists.Add(listUrlName, listDescription, listTemplateType);
            list = site.Lists[listGuid];
            list.Title = listTitle;
            list.Update();
         }
         return list;
      }


      /// <summary>
      /// Checks if a list exists in a site using its "internal name" and creates it if it doesn't exist.
      /// </summary>
      /// <param name="site">The site to check.</param>
      /// <param name="listDetail">The details of the list.</param>
      /// <param name="isNewList">A boolean that is set to true, if a new list is created; otherwise, it is set to false.</param>
      /// <returns>A SPList object that references an existing or newly created list.</returns>
      public static SPList EnsureList(this SPWeb site, ListDetails listDetail, ref bool isNewList)
      {
         return EnsureList(site, listDetail.UrlName, listDetail.TemplateType, listDetail.Title, listDetail.Description, ref isNewList);
      }


      /// <summary>
      /// Checks if a list exists in a site using its "internal name" and creates it if it doesn't exist.
      /// </summary>
      /// <param name="site">The site to check.</param>
      /// <param name="listDetail">The details of the list.</param>
      /// <param name="isNewList">A boolean that is set to true, if a new list is created; otherwise, it is set to false.</param>
      /// <returns>A SPList object that references an existing or newly created list.</returns>
      public static SPList EnsureList(this SPWeb site, ReadOnlyListDetails listDetail, ref bool isNewList)
      {
         return EnsureList(site, listDetail.UrlName, listDetail.TemplateType, listDetail.Title, listDetail.Description, ref isNewList);
      }
   }
}
