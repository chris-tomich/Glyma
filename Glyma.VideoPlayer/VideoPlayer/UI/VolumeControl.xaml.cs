using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VideoPlayer.Controller.Interface;

namespace VideoPlayer.UI
{
    public partial class VolumeControl : UserControl
    {
        private double _lastIndicatedVolume;
        private bool _isMute;

        public IVolumeController Remote
        {
            get;
            set;
        }

        private bool _isActivated;

        public VolumeControl()
        {
            InitializeComponent();
            _isActivated = false;
        }


        public void SetVolume(double volume)
        {
            if (volume < 0 || volume > 1) return;
            if (Remote != null)
            {
                _lastIndicatedVolume = volume;
                Remote.SetVolumeTo(volume);
                _isMute = false;
                ShowVolume();
            }
        }

        public void ShowVolume(double volume)
        {
            if (volume < 0 || volume > 1) return;
            VolumeValue.Width = VolumeMax.Width * volume;
        }

        public void ShowVolume()
        {
            if (Remote != null && Remote.IsMuted())
            {
                VolumeValue.Width = 0;
            }
            else
            {
                VolumeValue.Width = VolumeMax.Width * _lastIndicatedVolume;
            }
        }

        private void SetVolumeFromPosition(Point position)
        {
            var volume = position.X / VolumeMax.Width;
            if (volume < 0) volume = 0;
            if (volume > 10) volume = 10;
            SetVolume(volume);
        }

        private void MuteBox_Click(object sender, RoutedEventArgs e)
        {
            if (Remote != null)
            {
                _isMute = !_isMute;
                if (_isMute)
                {
                    Remote.Mute();
                    ShowVolume(0);
                    MuteButton.Visibility = Visibility.Collapsed;
                    UnMuteButton.Visibility = Visibility.Visible;
                }
                else
                {
                    SetVolume(_lastIndicatedVolume);
                    MuteButton.Visibility = Visibility.Visible;
                    UnMuteButton.Visibility = Visibility.Collapsed;
                }

            }
        }

        public void IsMuted()
        {
            Remote.IsMuted();
        }


        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isActivated = false;
            var position = e.GetPosition(Slider);
            SetVolumeFromPosition(position);
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isActivated = true;
        }

        private void Slider_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isActivated)
            {
                var position = e.GetPosition(Slider);
                SetVolumeFromPosition(position);
            }
        }

        private void Slider_MouseLeave(object sender, MouseEventArgs e)
        {
            _isActivated = false;
        }
    }
}
