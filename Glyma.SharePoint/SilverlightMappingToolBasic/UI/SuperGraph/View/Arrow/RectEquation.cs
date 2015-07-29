using System;
using System.Windows;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow
{
    public class RectEquation : IShapeEquation
    {

        public RectEquation(Point centre, double width, double height)
        {
            Centre = centre;
            Width = width;
            Height = height;
        }

        public double Width
        {
            get;
            set;
        }

        private double HalfWidth
        {
            get
            {
                return Width / 2;
            }
        }

        public double Height
        {
            get;
            set;
        }

        private double HalfHeight
        {
            get
            {
                return Height / 2;
            }
        }

        public Point Centre
        {
            get;
            set;
        }

        private Point GetRelativeIntersectionPoint(ILinearEquation line)
        {
            double yValueOnXAxisBoundary = line.Gradient * HalfWidth;
            double xValueOnYAxisBoundary = HalfHeight / line.Gradient;

            var intersection = new Point();

            if (Math.Abs(yValueOnXAxisBoundary) <= HalfHeight)
            {
                // This line intersects with the x-axis boundary.
                intersection.X = HalfWidth;
                intersection.Y = yValueOnXAxisBoundary;
            }
            else
            {
                // This line intersects with the y-axis boundary.
                intersection.X = xValueOnYAxisBoundary;
                intersection.Y = HalfHeight;
            }

            return intersection;
        }

        public double GetCentreToBoundaryDistance(ILinearEquation line)
        {
            var relativeOrigin = new Point(0, 0);
            var relativeBoundaryIntersection = GetRelativeIntersectionPoint(line);

            double distanceToBoundary = relativeOrigin.Distance(relativeBoundaryIntersection);

            return distanceToBoundary;
        }

        public Point[] FindIntersections(ILinearEquation line)
        {
            var boundaryIntersectionOffset = new Point();
            var antiBoundaryIntersectionOffset = new Point();

            if (line.Gradient == 0)
            {
                boundaryIntersectionOffset.X = HalfWidth;
                antiBoundaryIntersectionOffset.X = HalfWidth * -1;

                boundaryIntersectionOffset.Y = 0.0;
                antiBoundaryIntersectionOffset.Y = 0.0;
            }
            else if (double.IsInfinity(line.Gradient))
            {
                boundaryIntersectionOffset.X = 0.0;
                antiBoundaryIntersectionOffset.X = 0.0;

                boundaryIntersectionOffset.Y = HalfHeight;
                antiBoundaryIntersectionOffset.Y = HalfHeight * -1;
            }
            else
            {
                boundaryIntersectionOffset = GetRelativeIntersectionPoint(line);

                antiBoundaryIntersectionOffset.X = boundaryIntersectionOffset.X * -1;
                antiBoundaryIntersectionOffset.Y = boundaryIntersectionOffset.Y * -1;
            }

            var boundaryIntersection = Centre.Add(boundaryIntersectionOffset);
            var antiBoundaryIntersection = Centre.Add(antiBoundaryIntersectionOffset);

            var intersections = new Point[2];
            intersections[0] = boundaryIntersection;
            intersections[1] = antiBoundaryIntersection;

            return intersections;
        }
    }
}
