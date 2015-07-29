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
    public class CompactIdeaResource : ISpriteResource
    {
        private const string CompactIdeaResourceName = "CompactIdeaResource";

        private ImageBrush _brushResource;

        public CompactIdeaResource()
        {
            ImageSource imageResource = (ImageSource)Application.Current.Resources[CompactIdeaResourceName];

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
