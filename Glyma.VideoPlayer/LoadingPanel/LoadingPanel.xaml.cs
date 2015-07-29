using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LoadingPanel
{
    public partial class LoadingPanel : ChildWindow
    {

        public ColorScheme ColorScheme;

        private int _value;
        public int Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value >= 0 && value <= 100)
                    _value = value;
                else if (value > 100)
                    _value = 100;
                else
                    _value = 0;
                Progress.Text = string.Format("{0}%", _value);
            }
        }

        public bool IsProgressVisible
        {
            get
            {
                return Progress.Visibility == Visibility.Visible;
            }
            set
            {
                Progress.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public string LeftBottomStatusText
        {
            set
            {
                LeftBottomStatus.Text = value;
            }
            get
            {
                return LeftBottomStatus.Text;
            }
        }

        public string RightBottomStatusText
        {
            set
            {
                RightBottomStatus.Text = value;
            }
            get
            {
                return RightBottomStatus.Text;
            }
        }

        public LoadingPanel()
        {
            ColorScheme = new ColorScheme
            {
                LoadingColor1 = ColorConverter.FromHex("#990D96D8"),
                LoadingColor2 = ColorConverter.FromHex("#000D96D8"),
                BackgroundOpacity = 1,
                StatusTextColor = new SolidColorBrush(Colors.White),
                ProgressTextColor = new SolidColorBrush(Colors.White),
                BackgroundColor = new SolidColorBrush(Colors.Black),
                OverlayHeight = 300,
                OverlayWidth = 400
            };
            DataContext = ColorScheme;
            InitializeComponent();
        }
    }


}
