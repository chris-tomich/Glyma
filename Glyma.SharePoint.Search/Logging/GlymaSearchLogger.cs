using System;
using Microsoft.SharePoint.Administration;

namespace Glyma.SharePoint.Search
{
   /// <summary>
   /// Provides a singleton logging class for Glyma search that logs messages with custom log categories.
   /// </summary>
   public sealed class GlymaSearchLogger
   {
      private static readonly GlymaSearchLoggingService _loggingService = GlymaSearchLoggingService.Local;

      private GlymaSearchLogger() { }

      static GlymaSearchLogger() { }


      public static GlymaSearchLoggingService Service
      {
         get
         {
            return _loggingService;
         }
      }


      public static void WriteTrace(LogCategoryId messageCategoryId, TraceSeverity messageSeverity, string message)
      {
         SPDiagnosticsCategory category = _loggingService[messageCategoryId];
         _loggingService.WriteTrace(1, category, messageSeverity, message);
      }
   }
}
