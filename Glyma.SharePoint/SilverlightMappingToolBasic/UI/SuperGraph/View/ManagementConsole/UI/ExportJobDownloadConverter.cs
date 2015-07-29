using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Glyma.UtilityService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.UI
{
    public class ExportJobDownloadConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var job = value as IExportJob;
            if (job != null)
            {
                if (job.Status == ExportStatus.Completed && !string.IsNullOrEmpty(job.Link))
                {
                    if (job.IsCurrent)
                    {
                        return "Completed";
                    }
                    else
                    {
                        return "Outdated";
                    }
                }
                else
                {
                    return "Unavailiable";
                }
            }
            return "Unavailiable";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
