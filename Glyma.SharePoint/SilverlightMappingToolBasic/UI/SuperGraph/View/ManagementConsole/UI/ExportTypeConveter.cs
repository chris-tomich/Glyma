using System;
using System.ComponentModel.Composition.Primitives;
using System.Windows.Data;
using Glyma.UtilityService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.UI
{
    public class ExportTypeConveter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var job = value as IExportJob;
            if (job != null)
            {
                var showImage = job.ExportProperties.GetPropertyBooleanValue("ShowImage");
                var showDescription = job.ExportProperties.GetPropertyBooleanValue("ShowDescription");
                var showVideo = job.ExportProperties.GetPropertyBooleanValue("ShowVideo");
                var showPages = job.ExportProperties.GetPropertyBooleanValue("ShowPages");

                if (showImage)
                {
                    if (showDescription)
                    {
                        if (showVideo)
                        {
                            if (showPages)
                            {
                                return "ImageDescriptionVideoPages";
                            }
                            else
                            {
                                return "ImageDescriptionVideo";
                            }
                        }
                        else
                        {
                            if (showPages)
                            {
                                return "ImageDescriptionPages";
                            }
                            else
                            {
                                return "ImageDescription";
                            }
                        }
                        
                    }
                    else
                    {
                        if (showVideo)
                        {
                            if (showPages)
                            {
                                return "ImageVideoPages";
                            }
                            else
                            {
                                return "ImageVideo";
                            }
                        }
                        else
                        {
                            if (showPages)
                            {
                                return "ImagePages";
                            }
                            else
                            {
                                return "Image";
                            }
                        }
                    }
                }
                else
                {
                    if (showDescription)
                    {
                        if (showVideo)
                        {
                            if (showPages)
                            {
                                return "DescriptionVideoPages";
                            }
                            else
                            {
                                return "DescriptionVideo";
                            }
                        }
                        else
                        {
                            if (showPages)
                            {
                                return "DescriptionPages";
                            }
                            else
                            {
                                return "Description";
                            }
                        }
                    }
                    else
                    {
                        if (showVideo)
                        {
                            if (showPages)
                            {
                                return "VideoPages";
                            }
                            else
                            {
                                return "Video";
                            }
                        }
                        else
                        {
                            if (showPages)
                            {
                                return "Pages";
                            }
                            else
                            {
                                return "None";
                            }
                        }
                    }
                }
            }

            return "None";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
