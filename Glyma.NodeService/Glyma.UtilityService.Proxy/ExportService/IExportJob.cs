using System;
using System.Collections.Generic;

namespace Glyma.UtilityService.Proxy
{
    public interface IExportJob
    {
        Guid Id { get; }

        ExportType Type { get; }

        DateTime Created { get; }

        IGlymaUser CreatedBy { get; }

        ExportStatus Status { get; }

        MapType MapType { get; }

        Dictionary<string, string> ExportProperties { get; }

        bool IsCurrent { get; }

        string Link { get; }

        int PercentageComplete { get; }
    }
}
