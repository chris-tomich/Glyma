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
using System.Collections.Generic;

namespace SilverlightMappingToolBasic
{
    public static class FindClosestPointExtensionMethod
    {
        public static Point FindClosestPoint(this IEnumerable<Point> points, Point stationaryPoint)
        {
            Point currentClosestPoint = new Point();
            double currentClosestSize = -1.0;

            foreach (Point point in points)
            {
                double sizeSideA = stationaryPoint.X - point.X;
                double sizeSideB = stationaryPoint.Y - point.Y;
                double sizeHypotenuse = Math.Sqrt((sizeSideA * sizeSideA) + (sizeSideB * sizeSideB));

                if (currentClosestSize > 0 && sizeHypotenuse < currentClosestSize)
                {
                    currentClosestSize = sizeHypotenuse;
                    currentClosestPoint = point;
                }
                else if (currentClosestSize < 0)
                {
                    currentClosestSize = sizeHypotenuse;
                    currentClosestPoint = point;
                }
            }

            return currentClosestPoint;
        }
    }
}
