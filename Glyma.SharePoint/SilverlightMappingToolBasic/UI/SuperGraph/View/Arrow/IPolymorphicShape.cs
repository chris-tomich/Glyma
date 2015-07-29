using System;
using System.Windows;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow
{
    public interface IPolymorphicShape
    {
        Point Centre { get; }

        IShapeEquation GetShapeLeftMost();
        IShapeEquation GetShapeRightMost();
        IShapeEquation GetShapeLowest();
        IShapeEquation GetShapeHighest();

        event EventHandler<ShapeMovedEventArgs> ShapeMoved;
    }
}
