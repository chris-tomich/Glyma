using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public static class NodeCornerButtonLoader
    {
        private static Dictionary<NodeCornerButtonType, ImageSource> _images;
        private static Dictionary<NodeCornerButtonType, ImageSource> Images
        {
            get
            {
                if (_images == null)
                {
                    _images = new Dictionary<NodeCornerButtonType, ImageSource>();
                }

                return _images;
            }
        }

        private static string GetSkinImageUri(NodeCornerButtonType buttonType)
        {
            switch (buttonType)
            {
                case NodeCornerButtonType.Content:
                    return App.Params.GlymaIconLibraryUrl + "/button-content-transparent.png";
                case NodeCornerButtonType.Play:
                    return App.Params.GlymaIconLibraryUrl + "/button-play-transparent.png";
                case NodeCornerButtonType.Pause:
                    return App.Params.GlymaIconLibraryUrl + "/button-pause-transparent.png";
                case NodeCornerButtonType.Map:
                    return App.Params.GlymaIconLibraryUrl + "/button-map-transparent.png";
                case NodeCornerButtonType.Feed:
                    return App.Params.GlymaIconLibraryUrl + "/button-feed-transparent.png";
            }
            return string.Empty;
        }

        public static void DressButton(NodeCornerButtonViewModel button)
        {
            ImageSource skinImage;
            if (Images.ContainsKey(button.ButtonType))
            {
                skinImage = Images[button.ButtonType];
            }
            else
            {
                var skinImageAddress = GetSkinImageUri(button.ButtonType);
                var skinImageUri = new Uri(skinImageAddress);

                skinImage = new BitmapImage(skinImageUri);
                Images[button.ButtonType] = skinImage;
            }

            button.Icon = skinImage;
        }
    }
}
