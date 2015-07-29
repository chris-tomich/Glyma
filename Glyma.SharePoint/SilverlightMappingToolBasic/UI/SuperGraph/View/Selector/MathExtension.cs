using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Selector
{
    public static class MathExtension
    {
        public static bool IsLineHasIntersectWithRectangle(Point lineStart, Point lineEnd, Point rectangleStart, Point rectangleEnd)
        {
            var rectanglePoint3 = new Point(rectangleStart.X, rectangleEnd.Y);
            var rectanglePoint4 = new Point(rectangleEnd.X, rectangleStart.Y);
            if (HasIntersection(lineStart, lineEnd, rectangleStart, rectanglePoint3))
            {
                Debug.WriteLine("*** Found Intersect With Line {0} - {1}  and {2} - {3}", lineStart, lineEnd, rectangleStart, rectanglePoint3);
                return true; 
            }
            if (HasIntersection(lineStart, lineEnd, rectangleStart, rectanglePoint4))
            {
                Debug.WriteLine("*** Found Intersect With Line {0} - {1}  and {2} - {3}", lineStart, lineEnd, rectanglePoint3, rectangleEnd);
                return true;
            }
            if (HasIntersection(lineStart, lineEnd, rectangleEnd, rectanglePoint3))
            {
                Debug.WriteLine("*** Found Intersect With Line {0} - {1}  and {2} - {3}", lineStart, lineEnd, rectangleEnd, rectanglePoint4);
                return true;
            }
            if (HasIntersection(lineStart, lineEnd, rectangleEnd, rectanglePoint4))
            {
                Debug.WriteLine("*** Found Intersect With Line {0} - {1}  and {2} - {3}", lineStart, lineEnd, rectanglePoint4, rectangleStart);
                return true;
            }
            return false;
        }

        public static bool HasIntersection(Point line1Start, Point line1End, Point line2Start, Point line2End)
        {
            var a1 = line1End.Y - line1Start.Y;
            var b1 = line1Start.X - line1End.X;
            var c1 = line1End.X * line1Start.Y - line1Start.X * line1End.Y;

            var r3 = a1 * line2Start.X + b1 * line2Start.Y + c1;
            var r4 = a1 * line2End.X + b1 * line2End.Y + c1;
 

            if (!r3.Equals(0) && !r4.Equals(0) && Math.Sign(r3) == Math.Sign(r4))
            {
                return false;
            }

            var a2 = line2End.Y - line2Start.Y;
            var b2 = line2Start.X - line2End.X;
            var c2 = line2End.X * line2Start.Y - line2Start.X * line2End.Y;

            var r1 = a2 * line1Start.X + b2 * line1Start.Y + c2;
            var r2 = a2 * line1End.X + b2 * line1End.Y + c2;

            if (!r1.Equals(0) && !r2.Equals(0) && Math.Sign(r1) == Math.Sign(r2))
            {
                return false;
            }

            var denom = a1 * b2 - a2 * b1;
            if (denom.Equals(0))
            {
                return false;
            }

            //var offset = denom < 0 ? -denom / 2 : denom / 2;
            //var num = b1 * c2 - b2 * c1;
            //var x = (num < 0 ? num - offset : num + offset) / denom;
            //num = a2 * c1 - a1 * c2;
            //var y = (num < 0 ? num - offset : num + offset) / denom;
            return true;
        }

        public static bool IsBetween(this double target, double start, double end)
        {
            bool result;
            if (start >= end)
            {
                result = target <= start && target >= end;
            }
            else
            {
                result = target >= start && target <= end;
            }
            Debug.WriteLine(result ? "{0} is between {1} and {2}" : "{0} is not between {1} and {2}",target, start, end);
            return result;
        }

        public static bool IsPointInRectangle(this Point point, Point retangleStart, Point retangleEnd)
        {
            if (retangleEnd.X >= retangleStart.X)
            {
                if (Comparer<double>.Default.Compare(point.X, retangleStart.X) < 0 ||
                    Comparer<double>.Default.Compare(point.X, retangleEnd.X) > 0) return false;
            }
            else
            {
                if (Comparer<double>.Default.Compare(point.X, retangleStart.X) > 0 ||
                    Comparer<double>.Default.Compare(point.X, retangleEnd.X) < 0) return false;
            }

            if (retangleEnd.Y >= retangleStart.Y)
            {
                if (Comparer<double>.Default.Compare(point.Y, retangleStart.Y) >= 0 &&
                    Comparer<double>.Default.Compare(point.Y, retangleEnd.Y) <= 0)
                {
                    return true;
                }
            }
            else
            {
                if (Comparer<double>.Default.Compare(point.Y, retangleStart.Y) <= 0 &&
                    Comparer<double>.Default.Compare(point.Y, retangleEnd.Y) >= 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
