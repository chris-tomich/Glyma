using System;
using System.Collections.Generic;
using Microsoft.SharePoint;

namespace Glyma.SharePoint.Common
{
   public static class GlymaLists
   {
       public static readonly ReadOnlyListDetails GlymaExportsList;

       static GlymaLists()
       {
           GlymaExportsList = new ReadOnlyListDetails("GlymaExports", SPListTemplateType.GenericList, "Glyma Exports",
               "A list used to manage the processing of exports from Glyma and storing the results of the export operation.", false);
       }
   }
}
