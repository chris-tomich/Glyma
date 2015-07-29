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
    public interface ILineSegment
    {
        bool IgnoreLength { get; }
        Point TriangulationPoint { get; }
        Point PointA { get; }
        Point PointB { get; }
        double Coefficient { get; }
        double Constant { get; }

        ILineSegment Resize(IntersectionDetector detector);
        bool TriangulateClosestEndPoint(ILineSegment intersectingLineSegment, out Point closestPoint);
    }
}
