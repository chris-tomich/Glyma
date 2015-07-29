using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.SharePoint.Common
{
    public static class ChoiceColumnChoices
    {
        public static string ExportTypeChoices;
        public static string ExportStatusChoices;
        public static string MapTypeChoices;

        static ChoiceColumnChoices()
        {
            ExportTypeChoices = @"<CHOICES><CHOICE>GlymaXML</CHOICE><CHOICE>Word</CHOICE><CHOICE>PDF</CHOICE><CHOICE>Compendium</CHOICE></CHOICES>";
            ExportStatusChoices = @"<CHOICES><CHOICE>Scheduled</CHOICE><CHOICE>Processing</CHOICE><CHOICE>Completed</CHOICE><CHOICE>Error</CHOICE></CHOICES>";
            MapTypeChoices = @"<CHOICES><CHOICE>IBIS</CHOICE></CHOICES>";
        }
    }
}
