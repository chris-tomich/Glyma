using System;
using System.Collections.Generic;

namespace Glyma.UtilityService.Proxy
{
    public class ExportJob : IExportJob
    {
        public Guid Id 
        { 
            get; 
            set; 
        }

        public ExportType Type
        {
            get; 
            set;
        }

        public MapType MapType
        {
            get;
            set;
        }

        public DateTime Created
        {
            get; 
            set;
        }

        public IGlymaUser CreatedBy
        {
            get; 
            set;
        }

        public ExportStatus Status
        {
            get; 
            set;
        }

        public Dictionary<string, string> ExportProperties
        {
            get;
            set;
        }

        public bool IsCurrent
        {
            get;
            set;
        }

        public string Link
        {
            get; 
            set;
        }

        public int PercentageComplete
        {
            get;
            set;
        }
    }
}
