using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Glyma.UtilityService.Proxy
{
    public class ServerObjectConverter
    {
        internal static IDictionary<Guid, IExportJob> ConvertExportJobsCollection(IDictionary<Guid, Service.ExportJob> exportJobs)
        {
            Dictionary<Guid, Proxy.IExportJob> result = new Dictionary<Guid, Proxy.IExportJob>();
            foreach (Service.ExportJob exportJob in exportJobs.Values)
            {
                Proxy.IExportJob convertedExportJob = ConvertServerExportJob(exportJob);
                result.Add(convertedExportJob.Id, convertedExportJob);
            }
            return result;
        }

        internal static IExportJob ConvertServerExportJob(Service.ExportJob exportJob) 
        {
            Proxy.ExportJob result = new Proxy.ExportJob();
            result.Id = exportJob.Id;
            result.IsCurrent = exportJob.IsCurrent;
            result.Link = exportJob.Link;
            result.Status = ConvertServerExportStatus(exportJob.Status);
            if (exportJob.Created.HasValue) {
                result.Created = exportJob.Created.Value;
            }
            result.CreatedBy = new GlymaUser(exportJob.CreatedBy.Name);
            result.Type = ConvertServerExportType(exportJob.Type);
            result.MapType = ConvertServerMapType(exportJob.MapType);
            result.ExportProperties = exportJob.ExportProperties;
            result.PercentageComplete = exportJob.PercentageComplete;
            return result;
        }

        internal static Proxy.ExportStatus ConvertServerExportStatus(Service.ExportStatus exportStatus)
        {
            Proxy.ExportStatus status = Proxy.ExportStatus.Unknown;
            switch (exportStatus)
            {
                case Service.ExportStatus.Completed:
                    status = Proxy.ExportStatus.Completed;
                    break;
                case Service.ExportStatus.Processing:
                    status = Proxy.ExportStatus.Processing;
                    break;
                case Service.ExportStatus.Scheduled:
                    status = Proxy.ExportStatus.Scheduled;
                    break;
                case Service.ExportStatus.Error:
                    status = Proxy.ExportStatus.Error;
                    break;
                default:
                    status = Proxy.ExportStatus.Unknown;
                    break;
            }
            return status;
        }

        internal static Proxy.ExportType ConvertServerExportType(Service.ExportType exportType)
        {
            Proxy.ExportType type = Proxy.ExportType.Unknown;
            switch (exportType)
            {
                case Service.ExportType.Compendium:
                    type = Proxy.ExportType.Compendium;
                    break;
                case Service.ExportType.GlymaXml:
                    type = Proxy.ExportType.GlymaXml;
                    break;
                case Service.ExportType.PDF:
                    type = Proxy.ExportType.PDF;
                    break;
                case Service.ExportType.Word:
                    type = Proxy.ExportType.Word;
                    break;
                default:
                    type = Proxy.ExportType.Unknown;
                    break;
            }
            return type;
        }

        internal static Proxy.MapType ConvertServerMapType(Service.MapType mapType)
        {
            Proxy.MapType type = Proxy.MapType.Unknown;
            switch (mapType)
            {
                case Service.MapType.IBIS:
                    type = Proxy.MapType.IBIS;
                    break;
                default:
                    type = Proxy.MapType.Unknown;
                    break;
            }
            return type;
        }

    }
}
