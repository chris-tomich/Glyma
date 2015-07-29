using System;
using System.Collections.Generic;

namespace Glyma.UtilityService.Proxy
{
    public interface IExportServiceManager
    {
        Proxy.GetExportJobsEventRegister GetExportJobsCompleted { get; }

        Proxy.GetExportJobsForMapTypeEventRegister GetExportJobsForMapTypeCompleted { get; }

        Proxy.CreateExportJobEventRegister CreateExportJobCompleted { get; }

        Proxy.DeleteExportJobEventRegister DeleteExportJobCompleted { get; }

        Proxy.IsExportingAvailableEventRegister IsExportingAvailableCompleted { get; }

        void GetExportJobsAsync(Guid domainId, Guid rootMapId);

        void GetExportJobsForMapTypeAsync(MapType mapType, Guid domainId, Guid rootMapId);

        void CreateExportJobAsync(Guid domainId, Guid rootMapId, Dictionary<string, string> exportProperties, Proxy.MapType mapType, Proxy.ExportType exportType);

        void DeleteExportJobAsync(Proxy.IExportJob exportJob);

        void IsExportingAvailableAsync();
    }
}