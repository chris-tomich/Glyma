using System;
using System.Windows;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow
{
    public class EllipseEquation : IShapeEquation
    {
        public EllipseEquation(Point centre, double horizontalRadius, double verticalRadius)
        {
            Centre = centre;
            HorizontalRadius = horizontalRadius;
            VerticalRadius = verticalRadius;
        }

        public double HorizontalRadius
        {
            get;
            set;
        }

        public double VerticalRadius
        {
            get;
            set;
        }

        public Point Centre
        {
            get;
            set;
        }

        public double GetCentreToBoundaryDistance(ILinearEquation line)
        {
            if (HorizontalRadius <= VerticalRadius)
            {
                return HorizontalRadius;
            }
            return VerticalRadius;
        }

        public Point[] FindIntersections(ILinearEquation line)
        {
            double xOffset;
            double antiXOffset;
            double yOffset;
            double antiYOffset;

            if (line.Gradient.Equals(0))
            {
                xOffset = HorizontalRadius;
                antiXOffset = HorizontalRadius * -1;

                yOffset = 0.0;
                antiYOffset = 0.0;
            }
            else if (double.IsInfinity(line.Gradient))
            {
                xOffset = 0.0;
                antiXOffset = 0.0;

                yOffset = VerticalRadius;
                antiYOffset = VerticalRadius * -1;
            }
            else
            {
                double hSquared = HorizontalRadius * HorizontalRadius;
                double vSquared = VerticalRadius * VerticalRadius;
                double gSquared = line.Gradient * line.Gradient;

                xOffset = Math.Sqrt((hSquared * vSquared) / (hSquared * gSquared + vSquared));
                antiXOffset = xOffset * -1;

                yOffset = line.Gradient * xOffset;
                antiYOffset = line.Gradient * antiXOffset;
            }

            double x = Centre.X + xOffset;
            double y = Centre.Y + yOffset;

            double antiX = Centre.X + antiXOffset;
            double antiY = Centre.Y + antiYOffset;

            var intersections = new Point[2];
            intersections[0] = new Point(x, y);
            intersections[1] = new Point(antiX, antiY);

            return intersections;
        }
    }
}
