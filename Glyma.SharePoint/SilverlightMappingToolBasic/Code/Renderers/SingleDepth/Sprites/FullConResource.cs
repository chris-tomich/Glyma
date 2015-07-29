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

namespace SilverlightMappingToolBasic.Compendium
{
    public class FullConResource : ISpriteResource
    {
        private const string FullConResourceName = "FullConResource";

        private ImageBrush _brushResource;

        public FullConResource()
        {
            ImageSource imageResource = (ImageSource)Application.Current.Resources[FullConResourceName];

            _brushResource = new ImageBrush();
            _brushResource.ImageSource = imageResource;
        }

        #region ISpriteResource Members

        public Brush SpriteResouce
        {
            get
            {
                return _brushResource;
            }
        }

        #endregion
    }
}
