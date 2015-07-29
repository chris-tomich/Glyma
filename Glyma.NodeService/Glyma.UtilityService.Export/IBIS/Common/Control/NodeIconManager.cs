using System.Collections.Generic;
using System.IO;
using System.Management.Instrumentation;
using System.Net;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Telerik.Windows.Documents.Model;
using TransactionalNodeService.Proxy.Universal;
using Size = System.Windows.Size;

namespace Glyma.UtilityService.Export.IBIS.Common.Control
{
    public static class NodeIconManager
    {
        private static Dictionary<string, ImageInline> _images;
        private static object _imagesLock = new object();

        public static Dictionary<string, ImageInline> Images
        {
            get
            {
                if (_images == null)
                {
                    lock (_imagesLock)
                    {
                        if (_images == null)
                        {
                            _images = new Dictionary<string, ImageInline>();
                        }
                    }
                }
                return _images;
            }
        }

        private static object _getImageLock = new object();

        public static ImageInline GetImage(string nodeType)
        {
            lock (_getImageLock)
            {
                if (Images.ContainsKey(nodeType))
                {
                    return new ImageInline(Images[nodeType]);
                }
                
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Glyma.UtilityService.Export.Images." + nodeType + ".png"))
                {
                    var image = new ImageInline(stream, new Size(21, 21), "png");
                    image.ImageSource.Freeze();
                    Images.Add(nodeType, image);
                    return image;
                }
            }
        }
    }
}
