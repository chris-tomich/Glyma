using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Administration;

namespace Glyma.SharePoint.Search
{
   /// <summary>
   /// Specifies the custom log categories for the QMS.
   /// </summary>
   public enum LogCategoryId
   {
      Connector = 0,
      DataAccess = 1,
      Security = 2
   }


   /// <summary>
   /// Provides a logging service that logs messages with custom log categories for Glyma search.
   /// </summary>
   public class GlymaSearchLoggingService : SPDiagnosticsServiceBase
   {
      private static Dictionary<int, string> _categoryDisplayNames;

      public static readonly string DiagnosticAreaName = "Glyma Search";

      static GlymaSearchLoggingService()
      {
         _categoryDisplayNames = new Dictionary<int, string>();
         _categoryDisplayNames.Add((int)LogCategoryId.Connector, "Connector");
         _categoryDisplayNames.Add((int)LogCategoryId.DataAccess, "Data Access");
         _categoryDisplayNames.Add((int)LogCategoryId.Security, "Security");
      }


      public GlymaSearchLoggingService()
         : base(DiagnosticAreaName, SPFarm.Local)
      {
      }


      public GlymaSearchLoggingService(string name, SPFarm farm)
         : base(name, farm)
      {
      }


      protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
      {
         List<SPDiagnosticsCategory> categories = new List<SPDiagnosticsCategory>();
         foreach (int categoryId in Enum.GetValues(typeof(LogCategoryId)))
         {
            categories.Add(new SPDiagnosticsCategory(_categoryDisplayNames[categoryId], TraceSeverity.Unexpected, EventSeverity.Error, 0, (uint)categoryId));
         }

         yield return new SPDiagnosticsArea(DiagnosticAreaName, categories);      
      }


      public static GlymaSearchLoggingService Local
      {
         get
         {
            return SPDiagnosticsServiceBase.GetLocal<GlymaSearchLoggingService>();
         }
      }


      public SPDiagnosticsCategory this[LogCategoryId categoryId]
      {
         get
         {
            return Areas[DiagnosticAreaName].Categories[_categoryDisplayNames[(int)categoryId]];
         }
      }

   }
}
