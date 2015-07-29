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

namespace SilverlightMappingToolBasic
{
    public class Arrow
    {
        private Line _sprite;
        private ILineSegment _arrowCoordinates;

        public Arrow(ILineSegment arrowCoordinates)
        {
            _arrowCoordinates = arrowCoordinates;

            _sprite = new Line();
            _sprite.Tag = this;
            _sprite.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            _sprite.StrokeThickness = 2.0;
            _sprite.X1 = _arrowCoordinates.PointA.X;
            _sprite.Y1 = _arrowCoordinates.PointA.Y;
            _sprite.X2 = _arrowCoordinates.PointB.X;
            _sprite.Y2 = _arrowCoordinates.PointB.Y;
        }

        public ILineSegment ArrowCoordinates
        {
            get
            {
                return _arrowCoordinates;
            }
        }

        public FrameworkElement Sprite
        {
            get
            {
                return _sprite;
            }
        }
    }
}
