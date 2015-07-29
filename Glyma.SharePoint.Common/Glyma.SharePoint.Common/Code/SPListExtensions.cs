using System;
using System.Collections.Generic;
using Microsoft.SharePoint;

namespace Glyma.SharePoint.Common
{
   /// <summary>
   /// Provides extension methods for SharePoint lists.
   /// </summary>
   public static class SPListExtensions
   {
      /// <summary>
      /// Checks if a column exists in a list and creates it if it doesn't exist.
      /// </summary>
      /// <param name="site">The site to check.</param>
      /// <param name="columnDetail">The details of the site column.</param>
      /// <param name="isNewColumn">A boolean that is set to true, if a new column is created; otherwise, it is set to false.</param>
      /// <returns>A SPField object that references an existing or newly created site column.</returns>
      public static SPField EnsureColumn(this SPList list, ColumnDetails columnDetail, bool isSiteColumn, ref bool isNewColumn)
      {
         isNewColumn = false;
         SPField column = null;
         if (!list.Fields.ContainsField(columnDetail.InternalName))
         {
            if (!isSiteColumn)
            {
               // When a column is created using AddAsXml in a list (using it in a site isn't affected), set the display name to the internal name to workaround a bug that creates the column using the display name.
               ColumnDetails tempDetail = new ColumnDetails(columnDetail);
               if (tempDetail.Id.Equals(Guid.Empty))
               {
                  tempDetail.Id = Guid.NewGuid();
               }
               tempDetail.DisplayName = tempDetail.InternalName;

               string internalName = list.Fields.AddFieldAsXml(tempDetail.GetXml());
               column = list.Fields.GetFieldByInternalName(internalName);
               column.Title = columnDetail.DisplayName;
               column.Update();
               column = list.Fields.GetFieldByInternalName(internalName); //refresh the context
            }
            else
            {
               SPField siteColumn = list.ParentWeb.AvailableFields.GetFieldByInternalName(columnDetail.InternalName);
               string internalName = list.Fields.Add(siteColumn);
               column = list.Fields.GetFieldByInternalName(internalName);
            }
            isNewColumn = true;
         }
         else
         {
            column = list.Fields.GetFieldByInternalName(columnDetail.InternalName);
         }
         return column;
      }


      /// <summary>
      /// Checks if a column exists in a list and creates it if it doesn't exist.
      /// </summary>
      /// <param name="list">The site to check.</param>
      /// <param name="columnId">The GUID of the column to use if it needs to be created.</param>
      /// <param name="columnInternalName">The internal name of the column to check.</param>
      /// <param name="columnDisplayName">The display name of the column to use if it needs to be created.</param>
      /// <param name="columnDescription">The description of the column to use if it needs to be created.</param>
      /// <param name="columnGroup">The column group to add the site column to if it needs to be created.</param>
      /// <param name="columnXml">The XML definition of the site column.</param>
      /// <param name="isNewColumn">A boolean that is set to true, if a new column is created; otherwise, it is set to false.</param>
      /// <returns>A SPField object that references an existing or newly created site column.</returns>
      public static SPField EnsureColumn(this SPList list, Guid columnId, string columnInternalName, string columnDisplayName, string columnDescription, SPFieldType columnType, string columnTypeAsString, string columnGroup, bool isSiteColumn, ref bool isNewColumn)
      {
         ColumnDetails columnDetail = new ColumnDetails(columnId, columnInternalName, columnDisplayName, columnDescription, columnType, string.Empty, columnTypeAsString, columnGroup);
         return EnsureColumn(list, columnDetail, isSiteColumn, ref isNewColumn);
      }


      /// <summary>
      /// Checks if a column exists in a list and creates it if it doesn't exist.
      /// </summary>
      /// <param name="site">The site to check.</param>
      /// <param name="columnDetail">The details of the site column.</param>
      /// <param name="isNewColumn">A boolean that is set to true, if a new column is created; otherwise, it is set to false.</param>
      /// <returns>A SPField object that references an existing or newly created site column.</returns>
      public static SPField EnsureColumn(this SPList list, ReadOnlyColumnDetails columnDetail, bool isSiteColumn, ref bool isNewColumn)
      {
         return EnsureColumn(list, new ColumnDetails(columnDetail), isSiteColumn, ref isNewColumn);        
      }

      /// <summary>
      /// Unregisters a specified event receiver from the list.
      /// </summary>
      /// <param name="list">The list to use.</param>
      /// <param name="eventReceiverClassName">The name of the event receiver to remove.</param>
      public static void UnregisterEventReceivers(this SPList list, string eventReceiverClassName)
      {
         List<Guid> eventReceiverIdsToDelete = new List<Guid>();

         foreach (SPEventReceiverDefinition eventReceiver in list.EventReceivers)
         {
            if (eventReceiver.Class.Equals(eventReceiverClassName, StringComparison.OrdinalIgnoreCase))
            {
               eventReceiverIdsToDelete.Add(eventReceiver.Id);
            }
         }

         foreach (Guid eventReceiverId in eventReceiverIdsToDelete)
         {
            SPEventReceiverDefinition eventReceiver = list.EventReceivers[eventReceiverId];
            eventReceiver.Delete();
         }
      }
   }
}
