using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.SharePoint.Administration;

namespace SevenSigma.Logging
{
    [Guid("D64DEDE4-3D1D-42CC-AF40-DB09F0DFA309")]
    public class LoggingService : SPDiagnosticsServiceBase
    {
        public static class Categories
        {
            public static string ApplicationPages = "Application Pages";
            public static string WcfServices = "WCF Services";
            public static string SilverlightApplications = "Silverlight Applications";
        }

        public static LoggingService Local
        {
            get { return SPFarm.Local.Services.GetValue<LoggingService>(DefaultName); }
        }

        public static string DefaultName
        {
            get { return "Seven Sigma Logging Service"; }
        }

        public static string AreaName
        {
            get { return "Seven Sigma"; }
        }

        public LoggingService()
            : base(DefaultName, SPFarm.Local)
        {
        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>
            {
				new SPDiagnosticsArea(AreaName, 0, 0, false, new List<SPDiagnosticsCategory> {
				                        new SPDiagnosticsCategory(Categories.ApplicationPages, null, TraceSeverity.Medium, EventSeverity.Information, 0, 0, false, true),
                                        new SPDiagnosticsCategory(Categories.WcfServices, null, TraceSeverity.Medium, EventSeverity.Information, 0, 1, false, true),
                                        new SPDiagnosticsCategory(Categories.SilverlightApplications, null, TraceSeverity.Medium, EventSeverity.Information, 0, 2, false, true)
				                       })
            };

            return areas;
        }

        public static void WriteTrace(string categoryName, TraceSeverity traceSeverity, string format, params object[] arg)
        {
            WriteTrace(categoryName, traceSeverity, string.Format(format, arg));
        }

        public static void WriteTrace(string categoryName, TraceSeverity traceSeverity, string message)
        {
            // NOTE: If the given value in the severity parameter is less than the currently configured value for 
            // the category's TraceSeverity property, the trace is not written to the log.

            if (string.IsNullOrEmpty(message))
                return;

            try
            {
                LoggingService service = Local;

                if (service != null)
                {
                    SPDiagnosticsCategory category = service.Areas[AreaName].Categories[categoryName];
                    service.WriteTrace(1, category, traceSeverity, message);
                }
            }
            catch { }
        }

        //public static void WriteEvent(string categoryName, EventSeverity eventSeverity, string format, params object[] arg)
        //{
        //    WriteEvent(categoryName, eventSeverity, string.Format(format, arg));
        //}

        //public static void WriteEvent(string categoryName, EventSeverity eventSeverity, string message)
        //{
        //    if (string.IsNullOrEmpty(message))
        //        return;

        //    try
        //    {
        //        LoggingService service = Local;

        //        if (service != null)
        //        {
        //            SPDiagnosticsCategory category = service.Areas[AreaName].Categories[categoryName];
        //            service.WriteEvent(1, category, eventSeverity, message);
        //        }
        //    }
        //    catch { }
        //}
    }
}
