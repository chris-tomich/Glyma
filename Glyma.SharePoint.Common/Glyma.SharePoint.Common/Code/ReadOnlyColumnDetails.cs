using System;
using Microsoft.SharePoint;

namespace Glyma.SharePoint.Common
{
   /// <summary>
   /// Provides read-only details for a SharePoint column.
   /// </summary>
   public class ReadOnlyColumnDetails
   {
      private ColumnDetails _columnDetails;


      public ReadOnlyColumnDetails(Guid columnId, string columnInternalName, string columnDisplayName, string columnDescription, SPFieldType columnType, string columnChoices, string columnTypeAsString, string columnGroup)
      {
          _columnDetails = new ColumnDetails(columnId, columnInternalName, columnDisplayName, columnDescription, columnType, columnChoices, columnTypeAsString, columnGroup);
      }


      public ReadOnlyColumnDetails(Guid columnId, string columnInternalName, string columnDisplayName, string columnDescription, SPFieldType columnType, string columnGroup)
         : this(columnId, columnInternalName, columnDisplayName, columnDescription, columnType, string.Empty, columnGroup)
      {     
      }

      public ReadOnlyColumnDetails(Guid columnId, string columnInternalName, string columnDisplayName, string columnDescription, SPFieldType columnType, string columnChoices, string columnGroup)
          : this(columnId, columnInternalName, columnDisplayName, columnDescription, columnType, columnChoices, string.Empty, columnGroup)
      {
      }


      public Guid Id
      {
         get
         {
            return _columnDetails.Id;
         }
      }


      public string InternalName
      {
         get
         {
            return _columnDetails.InternalName;
         }
      }


      public string DisplayName
      {
         get
         {
            return _columnDetails.DisplayName;
         }
      }


      public string Description
      {
         get
         {
            return _columnDetails.Description;
         }
      }

      public string ColumnChoices
      {
          get
          {
              return _columnDetails.ColumnChoices;
          }
      }


      public SPFieldType Type
      {
         get
         {
            return _columnDetails.Type;
         }
      }


      public string TypeAsString
      {
         get
         {
            return _columnDetails.TypeAsString;
         }
      }


      public string Group
      {
         get
         {
            return _columnDetails.Group;
         }
      }


      public string GetXml()
      {
         return _columnDetails.GetXml();
      }     
   }
}
