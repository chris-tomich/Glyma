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

namespace SilverlightMappingToolBasic.UI
{
    public static class ShapeExtensionMethods
    {
        public static bool IsInBoundaryInclusively(this Point test, Point lowerX, Point upperX, Point lowerY, Point upperY)
        {
            if (test.X >= lowerX.X && test.X <= upperX.X)
            {
                if (test.Y >= lowerY.Y && test.Y <= upperY.Y)
                {
                    return true;
                }
            }

            return false;
        }

        public static double Distance(this Point origin, Point destination)
        {
            double xDelta = origin.X - destination.X;
            double yDelta = origin.Y - destination.Y;

            double xDeltaSquared = xDelta * xDelta;
            double yDeltaSquared = yDelta * yDelta;

            double hypotenuse = Math.Sqrt(xDeltaSquared + yDeltaSquared);

            return hypotenuse;
        }

        public static Point GetMidway(this Point firstVertex, Point secondVertex)
        {
            double xOffset = (secondVertex.X - firstVertex.X) / 2;
            double yOffset = (secondVertex.Y - firstVertex.Y) / 2;

            Point midway = new Point();
            midway.X = firstVertex.X + xOffset;
            midway.Y = firstVertex.Y + yOffset;

            return midway;
        }

        public static Point Add(this Point firstVertex, Point secondVertex)
        {
            Point summedVertex = new Point();
            summedVertex.X = firstVertex.X + secondVertex.X;
            summedVertex.Y = firstVertex.Y + secondVertex.Y;

            return summedVertex;
        }

        public static Point Subtract(this Point firstVertex, Point secondVertex)
        {
            Point difference = new Point();
            difference.X = firstVertex.X - secondVertex.X;
            difference.Y = firstVertex.Y - secondVertex.Y;

            return difference;
        }
    }
}
