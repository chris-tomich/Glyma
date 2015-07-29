using System;
using System.Windows.Data;
using Glyma.UtilityService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.UI
{
    public class ExportStatusConveter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var job = value as IExportJob;
            if (job != null)
            {
                if (job.Status == ExportStatus.Scheduled)
                {
                    return "Cancellable";
                }

                if (job.Status == ExportStatus.Completed || job.Status == ExportStatus.Error)
                {
                    return "Deletable";
                }

                if (job.Status == ExportStatus.Processing)
                {
                    return "Processing";
                }
            }

            return "Error";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
