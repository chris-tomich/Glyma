using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using LoadingPanel;
using VideoPlayer.Controller;
using VideoPlayer.Controller.Interface;
using System.Windows.Browser;

namespace VideoPlayer.UI
{
    public partial class VideoPlayerMainContainer : IMediaController
    {
        public VideoPlayerMainController Controller;
        public LoadingPanel.LoadingPanel LoadingPanel;

        public VideoPlayerMainContainer()
        {
            InitializeComponent();

            Loaded += MainPage_Loaded;
            Unloaded += MainPage_Unloaded;

            //This event fires in Chrome/Firefox when the panel is hidden via CSS
            //In IE the application does not exit
            App.Current.Exit += new EventHandler(Application_Exit);
        }

        void Application_Exit(object sender, EventArgs e)
        {
            Controller.Dispose();
            try
            {
                HtmlPage.Window.Invoke("VideoPlayerDisposed");
            }
            catch (Exception)
            {
                //It is expected that if the browser is refreshed the Video Player will both exit and the JavaScript will not
                //exist anymore to call this method, it should be ignored in this case.
            }
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Controller.Dispose();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Controller = new VideoPlayerMainController(this);
            LoadingPanel = new LoadingPanel.LoadingPanel
            {
                IsProgressVisible = false,
                ColorScheme =
                {
                    StatusTextColor = new SolidColorBrush(ColorConverter.FromHex("#FFffffff")),
                    BackgroundColor = new SolidColorBrush(Colors.Black),
                    BackgroundOpacity = 0,
                    LoadingColor1 = ColorConverter.FromHex("#FF777777"),
                    LoadingColor2 = ColorConverter.FromHex("#FF111111"),
                    OverlayWidth = 400,
                    OverlayHeight = 260
                },
                Opacity = 0.5
            };
            MediaElementControl.MainUI = this;
            SliderControl.Remote = this;
            VolumeControl.Remote = this;
            PlayControl.Remote = this;
            UrlControl.Remote = this;
            ControlsPanel.Opacity = 0.0;
            VolumeControl.SetVolume(0.5);

            JavaScriptBridge jsBridge = new JavaScriptBridge(Controller);
            HtmlPage.RegisterScriptableObject("glymaVideoPlayerBridge", jsBridge);

            Controller.Initialised();
        }

        private void MediaPlayer_MouseEnter(object sender, MouseEventArgs e)
        {
            FadeInControlPanel.Begin();
            //ControlsPanel.Opacity = 1;
        }

        private void MediaPlayer_MouseLeave(object sender, MouseEventArgs e)
        {
            FadeOutControlPanel.Begin();
            //ControlsPanel.Opacity = 0;
        }

        #region ITimeLineSliderController Methods
        void ITimeLineSliderController.SeekPosition(TimeSpan sp)
        {
            MediaElementControl.SeekToMediaPosition(sp);
        }

        TimeSpan ITimeLineSliderController.GetPosition()
        {
            return MediaElementControl.MediaPlayer.Position;
        }

        void ITimeLineSliderController.ResetToDefaultNode()
        {
            MediaElementControl.ResetToDefaultNode();
        }

        #endregion

        #region IVolumeController Methods
        void IVolumeController.SetVolumeTo(double volume)
        {
            MediaElementControl.MediaPlayer.Volume = volume;
        }

        void IVolumeController.Mute()
        {
            MediaElementControl.MediaPlayer.Volume = 0;
        }

        bool IVolumeController.IsMuted()
        {
            //var isMutedReturnEvent = new Event
            //{
            //    Name = "IsMutedCallback",
            //    EventArgs = new Dictionary<string, string> { { "IsMuted", MediaElementControl.MediaPlayer.IsMuted.ToString() } }
            //};
            //Controller.SendEvent(isMutedReturnEvent);
            return MediaElementControl.MediaPlayer.IsMuted;
        }
        #endregion

        #region IMediaControllerBase Methods
        void IMediaControllerBase.Play()
        {
            MediaElementControl.Play();
        }

        void IMediaControllerBase.Pause()
        {
            MediaElementControl.Pause();
        }

        void IMediaControllerBase.Stop()
        {
            MediaElementControl.Stop();
        }

        void IMediaControllerBase.SeekBySeconds(int seconds)
        {
            MediaElementControl.SeekBySeconds(seconds);
        }
        #endregion
    }
}