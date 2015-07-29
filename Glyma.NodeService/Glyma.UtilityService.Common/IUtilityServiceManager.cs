using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

using Glyma.UtilityService.Common.ExportServiceClasses;
using Glyma.UtilityService.Common.Model;

namespace Glyma.UtilityService.Common
{
    [ServiceContract(Namespace = "http://sevensigma.com.au/GlymaUtilityService")]
    public interface IUtilityServiceManager
    {
        [OperationContract]
        [FaultContract(typeof(ExportError))]
        ExportJobsResponse GetExportJobs(Guid domainUid, Guid rootMapUid);

        [OperationContract]
        [FaultContract(typeof(ExportError))]
        ExportJobsResponse GetExportJobsForMapType(MapType mapType, Guid domainUid, Guid rootMapUid);

        [OperationContract]
        [FaultContract(typeof(ExportError))]
        ExportJobResponse CreateExportJob(Guid domainUid, Guid rootMapUid, IDictionary<string, string> exportProperties, MapType mapType, ExportType exportType);

        [OperationContract]
        [FaultContract(typeof(ExportError))]
        ExportJobResponse DeleteExportJob(ExportJob exportJob);

        [OperationContract]
        [FaultContract(typeof(ExportError))]
        ExportAvailabilityResponse IsExportingAvailable();
    }
}
