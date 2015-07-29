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
using System.Windows.Threading;

namespace SilverlightMappingToolBasic.UI.Extensions
{
    public class MarginAnimation
    {
        public event EventHandler AnimationEnded;

        private readonly Thickness _from;
        private readonly Thickness _to;
        private readonly FrameworkElement _control;
        private readonly DispatcherTimer _timer;
        private bool _isForward;
        private readonly int _total;
        private int _count;

        private Thickness From
        {
            get { return _isForward ? _from : _to; }
        }

        private Thickness To
        {
            get { return _isForward ? _to : _from; }
        }

        

        public MarginAnimation(Thickness from, Thickness to, double sec, FrameworkElement control)
        {
            _from = @from;
            _to = to;
            _control = control;
            _timer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 0, 0, 50)};
            _total = (int)(sec/0.05);
            if (_total <= 0)
            {
                _total = 1;
            }
            _timer.Tick += TimerTick;
        }

        private void TimerTick(object sender, EventArgs eventArgs)
        {
            _count++;
            if (_count <= _total)
            {
                _control.Margin = new Thickness((To.Left - From.Left) * _count / _total + From.Left,
                                                          (To.Top - From.Top) * _count / _total + From.Top,
                                                          (To.Right - From.Right) * _count / _total + From.Right,
                                                          (To.Bottom - From.Bottom) * _count / _total + From.Bottom);
            }
            else
            {
                _timer.Stop();
                _count = 0;
                if (AnimationEnded != null)
                {
                    AnimationEnded(this, null);
                }
            }
        }

        public void Start(bool isForward = true)
        {
            if (!_timer.IsEnabled)
            {
                _isForward = isForward;
                _timer.Start();
            }
        }

    }
}
