using System;
using System.Globalization;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SilverlightMappingToolBasic.Code.ColorsManagement;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.StatusBar
{
    public partial class StatusBar : UserControl
    {
        private bool _isRunning;
        private int _inProgressActionCount;

        public event EventHandler ForcePushClicked;

        public int InProgressActionCount
        {
            get
            {
                return _inProgressActionCount;
            }
            set
            {
                if (_inProgressActionCount != value)
                {
                    _inProgressActionCount = value;
                    UpdateStatus();
                }
            }
        }

        public StatusBar()
        {
            InitializeComponent();
        }

        private void Start()
        {
            CountBackground.Background = new SolidColorBrush(ColorConverter.FromHex("#FFe64c65"));
            ProcessingRingImage.Visibility = Visibility.Visible;
            RotatingLine.Visibility = Visibility.Visible;
            IdleImage.Visibility = Visibility.Collapsed;
            StatusText.Text = "actions saving - please do not close the browser";
            SpinAnimation.Begin();
            _isRunning = true;
        }

        private void End()
        {
            CountBackground.Background = new SolidColorBrush(ColorConverter.FromHex("#ffA3D89E"));
            ProcessingRingImage.Visibility = Visibility.Collapsed;
            RotatingLine.Visibility = Visibility.Collapsed;
            IdleImage.Visibility = Visibility.Visible;
            StatusText.Text = "saving completed - you may safely close the browser";
            SpinAnimation.Stop();
            _isRunning = false;
        }

        private void UpdateStatus()
        {
            if (_inProgressActionCount > 0)
            {
                CountText.Text = _inProgressActionCount.ToString(CultureInfo.InvariantCulture);
                if (!_isRunning)
                {
                    Start();
                    HtmlPage.Window.Invoke("SetBusy",true);
                }
            }
            else
            {
                CountText.Text = "";
                if (_isRunning)
                {
                    End();
                    HtmlPage.Window.Invoke("SetBusy", false);
                }
            }
        }

        private void CountBackground_MouseRightButtonUp(object sender, MouseEventArgs e)
        {
            if (_isRunning)
            {
                PushButton.Visibility = Visibility.Visible;
            }
        }

        private void LayoutRoot_OnMouseLeave(object sender, MouseEventArgs e)
        {
            PushButton.Visibility = Visibility.Collapsed;
        }

        private void PushButton_OnMouseEnter(object sender, MouseEventArgs e)
        {
            PushButton.Background = new SolidColorBrush(ColorConverter.FromHex("#FF003f69"));
        }

        private void PushButton_OnMouseLeave(object sender, MouseEventArgs e)
        {
            PushButton.Background = new SolidColorBrush(ColorConverter.FromHex("#FF57afcf"));
        }

        private void PushButton_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ForcePushClicked != null)
            {
                PushButton.Visibility = Visibility.Collapsed;
                PushButton.Background = new SolidColorBrush(ColorConverter.FromHex("#FF57afcf"));
                ForcePushClicked(sender, null);
            }
        }
    }
}
