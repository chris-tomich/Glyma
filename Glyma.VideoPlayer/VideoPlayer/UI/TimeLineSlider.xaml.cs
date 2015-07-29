using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using VideoPlayer.Controller.Interface;

namespace VideoPlayer.UI
{
    public partial class TimeLineSlider : UserControl
    {
        private readonly DispatcherTimer _myDispatcherTimer;
        public bool HasSeekedBeyondEnd { set; get; }
        public ITimeLineSliderController Remote { get; set; }
        public double Maximum { get; set; }

        public TimeLineSlider()
        {
            InitializeComponent();
            _myDispatcherTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 100) };
            _myDispatcherTimer.Tick += Each_Tick;
            StartTimer();
            HasSeekedBeyondEnd = false;
        }

        public void StartTimer()
        {
            _myDispatcherTimer.Start();
        }

        public void StopTimer()
        {
            _myDispatcherTimer.Stop();
        }


        private void Each_Tick(object o, EventArgs sender)
        {
            if (Remote != null)
            {
                PlayValue.Width = SliderValue.Width * Remote.GetPosition().TotalMilliseconds / Maximum;
                Remote.GetPosition();
                TimeTextBlock.Text = string.Format("{0:#0}:{1:00}", Remote.GetPosition().Minutes, Remote.GetPosition().Seconds);
            }
        }


        private void TimeLineSliderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StopTimer();
        }

        private void TimeLineSliderMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(Slider);
            if (position.X > Slider.ActualWidth || position.X < 0) return;
            var mouseUpValue = (int)(Maximum / SliderValue.ActualWidth * (position.X - SliderValue.Margin.Left));
            if (mouseUpValue < 0) mouseUpValue = 0;
            if (mouseUpValue > Maximum) mouseUpValue = (int)(Maximum);
            var ts = new TimeSpan(0, 0, 0, 0, mouseUpValue);
            Remote.SeekPosition(ts);
            Remote.ResetToDefaultNode();
            StartTimer();
            
        }

    }
}
