using System;

namespace SilverlightMappingToolBasic.UI.Extensions.VideoWebPart
{
    public class VideoInfo : ViewModelBase
    {
        private bool _hasVideo;
        private string _videoSource;
        private VideoSize _videoSize;
        private TimeSpan? _startPosition;
        private TimeSpan? _stopPosition;
        private VideoState _status;

        protected VideoInfo()
        {
            Id = Guid.NewGuid();
        }

        public VideoInfo(ViewModel.INode node)
            : this()
        {
            Context = node;

            LoadVideoDetails();
        }

        public event EventHandler SetStartPositionRequested;
        public event EventHandler SetStopPositionRequested;

        private void LoadVideoDetails()
        {
            ViewModel.IMetadata videoSourceMetadata = Context.Metadata["Video.Source"];
            if (videoSourceMetadata == null)
            {
                HasVideo = false;
            }
            else
            {
                HasVideo = true;
                VideoSource = videoSourceMetadata.Value;
            }

            if (HasVideo)
            {
                ViewModel.IMetadata startPositionMetadata = Context.Metadata["Video.StartPosition"];
                if (startPositionMetadata != null && _startPosition == null)
                {
                    if (!string.IsNullOrEmpty(startPositionMetadata.Value))
                    {
                        TimeSpan parsedStartPosition;

                        if (TimeSpan.TryParse(startPositionMetadata.Value, out parsedStartPosition))
                        {
                            StartPosition = parsedStartPosition;
                        }
                        else
                        {
                            HasVideo = false;
                            StartPosition = new TimeSpan(0);
                        }
                    }
                }

                ViewModel.IMetadata stopPositionMetadata = Context.Metadata["Video.EndPosition"];
                if (stopPositionMetadata != null && _stopPosition == null)
                {
                    if (!string.IsNullOrEmpty(stopPositionMetadata.Value))
                    {
                        TimeSpan parsedStopPosition;

                        if (TimeSpan.TryParse(stopPositionMetadata.Value, out parsedStopPosition))
                        {
                            StopPosition = parsedStopPosition;
                        }
                        else
                        {
                            StopPosition = null;
                        }
                    }
                }
            }
        }

        public Guid Id
        {
            get;
            private set;
        }

        public ViewModel.INode Context
        {
            get;
            private set;
        }

        public bool HasVideo
        {
            get
            {
                return _hasVideo;
            }
            set
            {
                _hasVideo = value;
                OnNotifyPropertyChanged("HasVideo");
            }
        }

        public VideoState Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnNotifyPropertyChanged("Status");
            }
        }

        public string VideoSource
        {
            get
            {
                return _videoSource;
            }
            set
            {
                _videoSource = value;
            }
        }


        public TimeSpan? StartPosition
        {
            get
            {
                return _startPosition;
            }
            set
            {
                _startPosition = value;
            }
        }

        public TimeSpan? StopPosition
        {
            get
            {
                return _stopPosition;
            }
            set
            {
                _stopPosition = value;
            }
        }

        public VideoSize Size
        {
            get
            {
                VideoSizeHelper sizeHelper = new VideoSizeHelper(Context);
                _videoSize = sizeHelper.Size;
                return _videoSize;
            }
        }

        public void RequestSetStartPosition()
        {
            if (SetStartPositionRequested != null)
            {
                SetStartPositionRequested(this, new EventArgs());
            }
        }

        public void RequestSetStopPosition()
        {
            if (SetStopPositionRequested != null)
            {
                SetStopPositionRequested(this, new EventArgs());
            }
        }
    }
}
