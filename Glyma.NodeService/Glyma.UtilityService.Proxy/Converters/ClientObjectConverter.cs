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
    public class ClientObjectConverter
    {
        internal static ObservableCollection<Service.ExportJob> ConvertExportJobsCollection(IDictionary<Guid, Proxy.IExportJob> exportJobs)
        {
            ObservableCollection<Service.ExportJob> result = new ObservableCollection<Service.ExportJob>();
            foreach (Proxy.IExportJob exportJob in exportJobs.Values)
            {
                Service.ExportJob convertedExportJob = ConvertClientExportJob(exportJob);
                result.Add(convertedExportJob);
            }
            return result;
        }

        internal static Service.ExportJob ConvertClientExportJob(Proxy.IExportJob exportJob) 
        {
            Service.ExportJob result = new Service.ExportJob();
            result.Id = exportJob.Id;
            result.IsCurrent = exportJob.IsCurrent;
            result.Link = exportJob.Link;
            result.Status = ConvertClientExportStatus(exportJob.Status);
            result.Created = exportJob.Created;
            result.CreatedBy = new Service.GlymaUser();
            result.CreatedBy.Name = exportJob.CreatedBy.Name;
            result.CreatedBy.DisplayName = exportJob.CreatedBy.DisplayName;
            result.Type = ConvertClientExportType(exportJob.Type);
            result.MapType = ConvertClientMapType(exportJob.MapType);
            result.ExportProperties = exportJob.ExportProperties;
            result.PercentageComplete = exportJob.PercentageComplete;
            return result;
        }

        internal static Service.ExportStatus ConvertClientExportStatus(Proxy.ExportStatus exportStatus)
        {
            Service.ExportStatus status = Service.ExportStatus.Completed;
            switch (exportStatus)
            {
                case Proxy.ExportStatus.Completed:
                    status = Service.ExportStatus.Completed;
                    break;
                case Proxy.ExportStatus.Processing:
                    status = Service.ExportStatus.Processing;
                    break;
                case Proxy.ExportStatus.Scheduled:
                    status = Service.ExportStatus.Scheduled;
                    break;
                case Proxy.ExportStatus.Error:
                    status = Service.ExportStatus.Error;
                    break;
            }
            return status;
        }

        internal static Service.ExportType ConvertClientExportType(Proxy.ExportType exportType)
        {
            Service.ExportType type = Service.ExportType.GlymaXml;
            switch (exportType)
            {
                case Proxy.ExportType.Compendium:
                    type = Service.ExportType.Compendium;
                    break;
                case Proxy.ExportType.GlymaXml:
                    type = Service.ExportType.GlymaXml;
                    break;
                case Proxy.ExportType.PDF:
                    type = Service.ExportType.PDF;
                    break;
                case Proxy.ExportType.Word:
                    type = Service.ExportType.Word;
                    break;
            }
            return type;
        }

        internal static Service.MapType ConvertClientMapType(Proxy.MapType mapType)
        {
            Service.MapType type = Service.MapType.IBIS;
            switch (mapType)
            {
                case Proxy.MapType.IBIS:
                    type = Service.MapType.IBIS;
                    break;
            }
            return type;
        }
    }
}
