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
using TransactionalNodeService.Proxy;
using SilverlightMappingToolBasic.MappingService;

namespace SilverlightMappingToolBasic.UI.Extensions.VideoWebPart
{
    public enum VideoSize
    {
        Small,
        Medium,
        Large,
        NotSpecified
    }

    public class VideoSizeHelper
    {
        private VideoSize _videoSize = VideoSize.NotSpecified;

        public VideoSizeHelper(INodeProxy nodeProxy)
        {
            MetadataContext videoSizeKey = new MetadataContext() { MetadataName = "Video.Size", NodeUid = nodeProxy.Id };
            string videoSize = null;
            if (nodeProxy.HasMetadata(videoSizeKey))
            {
                videoSize = nodeProxy.GetNodeMetadata(videoSizeKey).MetadataValue;
            }
            Size = ParseVideoSize(videoSize);
        }

        public VideoSizeHelper(ViewModel.INode nodeContext)
        {
            ViewModel.IMetadata videoSizeMetadata = nodeContext.Metadata["Video.Size"];
            if (videoSizeMetadata != null)
            {
                Size = ParseVideoSize(videoSizeMetadata.Value);
            }
        }

        public VideoSizeHelper(TransactionalNodeService.Proxy.INode nodeContext)
        {
            IMetadataSet videoSizeMetadata = nodeContext.Metadata.FindMetadata("Video.Size");
            if (videoSizeMetadata != null)
            {
                Size = ParseVideoSize(videoSizeMetadata.Value);
            }
        }

        public VideoSizeHelper(string value)
        {
            Size = ParseVideoSize(value);
        }

        private VideoSize ParseVideoSize(string value)
        {
            VideoSize result = VideoSize.NotSpecified;
            if (!string.IsNullOrEmpty(value))
            {
                string sizeValue = value.Trim();
                if (sizeValue.Equals(VideoSize.Small.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    result = VideoSize.Small;
                }
                else if (sizeValue.Equals(VideoSize.Medium.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    result = VideoSize.Medium;
                }
                else if (sizeValue.Equals(VideoSize.Large.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    result = VideoSize.Large;
                }
                else
                {
                    result = VideoSize.NotSpecified;
                }
            }
            return result;
        }

        public VideoSize Size
        {
            get
            {
                return _videoSize;
            }
            set
            {
                _videoSize = value;
            }
        }
    }
}
