using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FluxJpeg.Core;
using SilverlightMappingToolBasic.UI.SuperGraph.View;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;

namespace SilverlightMappingToolBasic.UI.Extensions.ScreenCapture
{
    public class ScreenCaptureUtility
    {
        private static ScreenCaptureUtility _instance;

        private ScreenCaptureUtility()
        {
        }

        public static ScreenCaptureUtility Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ScreenCaptureUtility();
                }
                return _instance;
            }
        }

        private double CalculateScale(double mapWidth, double mapHeight, double containerX, double containerY)
        {
            var deltaX = mapWidth / containerX;
            var deltaY = mapHeight / containerY;

            double delta;

            if (deltaX > deltaY)
            {
                delta = deltaX;
            }
            else
            {
                delta = deltaY;
            }

            if (delta < 1)
            {
                return 1;
            }
            return delta;
        }

        public void SaveToImage(SuperGraphControl mapCanvas) 
        {
            
            var saveDlg = new SaveFileDialog {Filter = "JPEG Files (*.jpg)|*.jpg", DefaultExt = ".jpg"};

            var showDialog = saveDlg.ShowDialog();
            if (showDialog != null && (bool)showDialog)
            {
                WriteableBitmap bitmap;
                try
                {
                    
                    double left, right, top, bottom;
                    mapCanvas.GetMapBounds(out left, out right, out top, out bottom);
                    left -= 70;
                    right += 70;
                    top -= 170;
                    bottom += 170;
                    double mapHeight = bottom - top;
                    double mapWidth = right - left;

                    var transform = new TransformGroup();
                    ScaleTransform st;
                    TranslateTransform tt;
                    
                    var moved = mapCanvas.MoveGraphTransform;
                    double xMove, yMove;

                    const ScreenCaptureType type = ScreenCaptureType.ZoomedFullScale;
                    Canvas canvas;
                    switch (type)
                    {
                        case ScreenCaptureType.FullScale:

                            xMove = moved.X + left;
                            yMove = moved.Y + top;

                            st = new ScaleTransform
                            {
                                ScaleX = 1 / mapCanvas.Zoom,
                                ScaleY = 1 / mapCanvas.Zoom,
                                CenterX = 0,
                                CenterY = 0
                            };
                            transform.Children.Add(st);


                            tt = new TranslateTransform
                            {
                                X = -xMove,
                                Y = -yMove,
                            };
                            transform.Children.Add(tt);
                            canvas = new Canvas
                            {
                                Background = new SolidColorBrush(Colors.White),
                                Width = (int) mapWidth,
                                Height = (int) mapHeight
                            };
                            bitmap = new WriteableBitmap(canvas, null);
                            break;
                        case ScreenCaptureType.ZoomedFullScale:
                            xMove = moved.X + left;
                            yMove = moved.Y + top;

                            tt = new TranslateTransform
                            {
                                X = -xMove * mapCanvas.Zoom,
                                Y = -yMove * mapCanvas.Zoom,
                            };
                            transform.Children.Add(tt);
                            canvas = new Canvas
                            {
                                Background = new SolidColorBrush(Colors.White),
                                Width = (int) (mapWidth*mapCanvas.Zoom),
                                Height = (int) (mapHeight*mapCanvas.Zoom)
                            };
                            bitmap = new WriteableBitmap(canvas, null);
                            break;
                        case ScreenCaptureType.CurrentScreenOnly:
                            canvas = new Canvas
                            {
                                Background = new SolidColorBrush(Colors.White),
                                Width = (int) mapCanvas.ActualWidth,
                                Height = (int) mapCanvas.ActualHeight
                            };
                            bitmap = new WriteableBitmap(canvas, null);
                            break;
                        default:
                            canvas = new Canvas
                            {
                                Background = new SolidColorBrush(Colors.White),
                                Width = (int) mapCanvas.ActualWidth,
                                Height = (int) mapCanvas.ActualHeight
                            };
                            bitmap = new WriteableBitmap(canvas, null);
                            break;
                    }

                    bitmap.Render(mapCanvas, transform);
                    bitmap.Invalidate();


                    using (var fs = saveDlg.OpenFile())
                    {
                        var stream = GetImageStream(bitmap);

                        //Get Bytes from memory stream and write into IO stream
                        var binaryData = new Byte[stream.Length];
                        var bytesRead = stream.Read(binaryData, 0, (int)stream.Length);
                        fs.Write(binaryData, 0, binaryData.Length);
                    }

                    

                }
                catch (Exception e)
                {
                    SuperMessageBoxService.ShowError("Export Failed", "We are unable to export your map.\r\nPlease zoom out and try again.");
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        
        }

        /// <summary>
        /// Get image MemoryStream from WriteableBitmap
        /// </summary>
        /// <param name="bitmap">WriteableBitmap</param>
        /// <returns>MemoryStream</returns>
        public static MemoryStream GetImageStream(WriteableBitmap bitmap)
        {
            byte[][,] raster = ReadRasterInformation(bitmap);
            return EncodeRasterInformationToStream(raster, ColorSpace.RGB);
        } 

        /// <summary>
        /// Reads raster information from WriteableBitmap
        /// </summary>
        /// <param name="bitmap">WriteableBitmap</param>
        /// <returns>Array of bytes</returns>
        public static byte[][,] ReadRasterInformation(WriteableBitmap bitmap)
        {
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int bands = 3;
            byte[][,] raster = new byte[bands][,];

            for (int i = 0; i < bands; i++)
            {
                raster[i] = new byte[width, height];
            }

            for (int row = 0; row < height; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    int pixel = bitmap.Pixels[width * row + column];
                    raster[0][column, row] = (byte)(pixel >> 16);
                    raster[1][column, row] = (byte)(pixel >> 8);
                    raster[2][column, row] = (byte)pixel;
                }
            }

            return raster;
        }

        /// <summary>
        /// Encode raster information to MemoryStream
        /// </summary>
        /// <param name="raster">Raster information (Array of bytes)</param>
        /// <param name="colorSpace">ColorSpace used</param>
        /// <returns>MemoryStream</returns>
        public static MemoryStream EncodeRasterInformationToStream(byte[][,] raster, ColorSpace colorSpace)
        {
            ColorModel model = new ColorModel { colorspace = ColorSpace.RGB };
            FluxJpeg.Core.Image img = new FluxJpeg.Core.Image(model, raster);

            //Encode the Image as a JPEG
            MemoryStream stream = new MemoryStream();
            FluxJpeg.Core.Encoder.JpegEncoder encoder = new FluxJpeg.Core.Encoder.JpegEncoder(img, 100, stream);
            encoder.Encode();

            // Back to the start
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}
