using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using VideoPlayer.Controller.Interface;

namespace VideoPlayer.UI
{
    public enum SeekDirection
    {
        Forward,
        Backward
    }
    public partial class PlayControl : UserControl
    {
        private const int DELAY_COUNT = 3;
        private int _seekSeconds;
        private int _clickCount = 0;
        private int _resumeDelayCount = 0;
        private bool _isLeftMouseDown = false;
        private readonly DispatcherTimer _myDispatcherTimer;
        private SeekDirection _seekDirection;

        public PlayControl()
        {
            InitializeComponent();
            _myDispatcherTimer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 1) };
            _myDispatcherTimer.Tick += Each_Tick;

            ResetSeekValues();
        }

        private void Each_Tick(object o, EventArgs sender)
        {
            if (Remote != null)
            {
                if (!_isLeftMouseDown)
                {
                    ResetSeekValues();
                    _resumeDelayCount++;
                    if (_resumeDelayCount >= DELAY_COUNT)
                    {
                        Remote.Play();
                        _resumeDelayCount = 0;
                        _myDispatcherTimer.Stop();
                    }
                }
                else
                {
                    Remote.Pause();

                    _clickCount++;

                    if (_clickCount % 10 == 0)
                    {
                        //every 10 seconds increment the seek seconds by a factor of 2
                        _seekSeconds = _seekSeconds * 2;
                    }

                    if (_seekDirection == SeekDirection.Backward && _seekSeconds > 0)
                    {
                        //seek backwards with a negative amount
                        _seekSeconds = _seekSeconds * -1; 
                    }

                    Remote.SeekBySeconds(_seekSeconds);
                }
            }
            _isLeftMouseDown = false; 
        }

        public IMediaControllerBase Remote
        {
            get;
            set;
        }

        private void PlayButton_Clicked(object sender, RoutedEventArgs e)
        {
            Remote.Play();
        }

        private void PauseButton_Clicked(object sender, RoutedEventArgs e)
        {
            Remote.Pause();
        }

        private void StopButton_Clicked(object sender, RoutedEventArgs e)
        {
            Remote.Stop();
        }

        public void PlayState()
        {
            PauseButton.Visibility = Visibility.Visible;
            PlayButton.Visibility = Visibility.Collapsed;
        }

        public void StopState()
        {
            PauseButton.Visibility = Visibility.Collapsed;
            PlayButton.Visibility = Visibility.Visible;
        }

        private void StepBackButton_Clicked(object sender, RoutedEventArgs e)
        {
            _isLeftMouseDown = true;
            _seekDirection = SeekDirection.Backward;
            if (!_myDispatcherTimer.IsEnabled)
            {
                _myDispatcherTimer.Start();
            }
        }

        private void StepForwardButton_Clicked(object sender, RoutedEventArgs e)
        {
            _isLeftMouseDown = true;
            _seekDirection = SeekDirection.Forward;
            if (!_myDispatcherTimer.IsEnabled)
            {
                _myDispatcherTimer.Start();
            }
        }

        private void ResetSeekValues()
        {
            _seekSeconds = 1;
            _clickCount = 0;
        }
    }
}
