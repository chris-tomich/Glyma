using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.Extensions.VideoWebPart
{
    public partial class VideoControl : UserControl, INodeCornerButton
    {
        public event EventHandler PlayClicked;
        public event EventHandler<VisibilityChangedEventArgs> VisibilityChanged;

        protected virtual void OnPlayClicked()
        {
            if (PlayClicked != null)
            {
                PlayClicked(this, EventArgs.Empty);
            }
        }

        public VideoControl()
        {
            InitializeComponent();
            if (!App.IsDesignTime)
            {
                var playViewModel = new NodeCornerButtonViewModel { ButtonType = NodeCornerButtonType.Play };
                var pauseViewModel = new NodeCornerButtonViewModel { ButtonType = NodeCornerButtonType.Pause };
                NodeCornerButtonLoader.DressButton(playViewModel);
                NodeCornerButtonLoader.DressButton(pauseViewModel);
                PlayButton.DataContext = playViewModel;
                PauseButton.DataContext = pauseViewModel;
            }
        }

        public VideoInfo ViewModel
        {
            get
            {
                return DataContext as VideoInfo;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged += OnViewModelChanged;
            }
        }

        private void OnViewModelChanged(object sender, PropertyChangedEventArgs e)
        {
            var viewModelVideoInfo = sender as VideoInfo;

            if (viewModelVideoInfo != null)
            {
                switch (e.PropertyName)
                {
                    case "HasVideo":
                        if (VisibilityChanged != null)
                        {
                            VisibilityChanged(sender, new VisibilityChangedEventArgs{Visibility = Visibility.Visible});
                        }
                        break;
                    case "Status":
                        if (viewModelVideoInfo.Status == VideoState.Playing)
                        {
                            PlayButton.Visibility = Visibility.Collapsed;
                            PauseButton.Visibility = Visibility.Visible;
                        }
                        else if (viewModelVideoInfo.Status == VideoState.Pause)
                        {
                            PlayButton.Visibility = Visibility.Visible;
                            PauseButton.Visibility = Visibility.Collapsed;
                        }
                        break;
                }
            }
        }

        public void RecordVideoStartPosition()
        {
            ViewModel.RequestSetStartPosition();
        }

        public void RecordVideoStopPosition()
        {
            ViewModel.RequestSetStopPosition();
        }

        private void PauseButton_OnButtonClicked(object sender, RoutedEventArgs e)
        {
            ViewModel.Status = VideoState.Pause;
        }

        private void PlayButton_OnButtonClicked(object sender, RoutedEventArgs e)
        {
            ViewModel.Status = VideoState.Playing;
            if (PlayClicked != null)
            {
                PlayClicked(sender, null);
            }
        }
    }
}
