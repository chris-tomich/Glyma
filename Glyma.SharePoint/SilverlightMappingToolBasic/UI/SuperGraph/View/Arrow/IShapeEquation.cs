using System.Windows;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow
{
    public interface IShapeEquation
    {
        Point Centre { get; }

        Point[] FindIntersections(ILinearEquation line);
        double GetCentreToBoundaryDistance(ILinearEquation line);
    }
}
