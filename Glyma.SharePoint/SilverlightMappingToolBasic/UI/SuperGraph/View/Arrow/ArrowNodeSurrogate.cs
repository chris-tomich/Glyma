using System;
using System.Windows;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow
{
    public class ArrowNodeSurrogate : IPolymorphicShape
    {
        private const double EllipseRadius = 2.0;

        private Point _centre;
        private EllipseEquation _ellipse;

        public event EventHandler<ShapeMovedEventArgs> ShapeMoved;

        private EllipseEquation Ellipse
        {
            get
            {
                if (_ellipse == null)
                {
                    _ellipse = new EllipseEquation(Centre, EllipseRadius, EllipseRadius);
                }

                return _ellipse;
            }
        }

        public Point Centre
        {
            get
            {
                return _centre;
            }
            set
            {
                if (_centre == value)
                {
                    return;
                }

                _centre = value;
                Ellipse.Centre = _centre;

                if (ShapeMoved != null)
                {
                    ShapeMoved(this, new ShapeMovedEventArgs() { Location = _centre });
                }
            }
        }

        public IShapeEquation GetShapeLeftMost()
        {
            return Ellipse;
        }

        public IShapeEquation GetShapeRightMost()
        {
            return Ellipse;
        }

        public IShapeEquation GetShapeLowest()
        {
            return Ellipse;
        }

        public IShapeEquation GetShapeHighest()
        {
            return Ellipse;
        }
    }
}
