using System.ComponentModel;
using System.Windows.Media;

namespace LoadingPanel
{
    public class ColorScheme : INotifyPropertyChanged
    {
        private Color _loadingColor1;
        public Color LoadingColor1
        {
            get { return _loadingColor1; }
            set
            {
                _loadingColor1 = value;
                OnPropertyChanged("LoadingColor1");
            }
        }

        private Color _loadingColor2;
        public Color LoadingColor2
        {
            get { return _loadingColor2; }
            set
            {
                _loadingColor2 = value;
                OnPropertyChanged("LoadingColor2");
            }
        }

        private Brush _backgroundColor;
        public Brush BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                OnPropertyChanged("BackgroundColor");
            }
        }

        private double _backgroundOpacity;
        public double BackgroundOpacity
        {
            get { return _backgroundOpacity; }
            set
            {
                _backgroundOpacity = value;
                OnPropertyChanged("BackgroundOpacity");
            }
        }

        private Brush _statusTextColor;
        public Brush StatusTextColor
        {
            get { return _statusTextColor; }
            set
            {
                _statusTextColor = value;
                OnPropertyChanged("StatusTextColor");
            }
        }


        private Brush _progressTextColor;
        public Brush ProgressTextColor
        {
            get { return _progressTextColor; }
            set
            {
                _progressTextColor = value;
                OnPropertyChanged("ProgressTextColor");
            }
        }

        private int _overlayHeight;
        public int OverlayHeight
        {
            get { return _overlayHeight; }
            set
            {
                _overlayHeight = value;
                OnPropertyChanged("OverlayHeight");
            }
        }


        private int _overlayWidth;
        public int OverlayWidth
        {
            get { return _overlayWidth; }
            set
            {
                _overlayWidth = value;
                OnPropertyChanged("OverlayWidth");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
