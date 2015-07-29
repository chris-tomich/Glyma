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
using System.Diagnostics;

namespace SilverlightMappingToolBasic
{
    public static class IntersectionCalculator
    {
        private static double GetCoefficient(Point startPoint, Point endPoint)
        {
            double coefficient;

            if (startPoint.X != endPoint.X)
            {
                coefficient = ((startPoint.Y - endPoint.Y) / (startPoint.X - endPoint.X));
            }
            else
            {
                coefficient = 0;
            }

            return coefficient;
        }

        private static double GetConstant(double coefficient, Point startPoint)
        {
            double constant;

            constant = startPoint.Y - (startPoint.X * coefficient);

            return constant;
        }

        public static Point[] FindIntersections(this PathFigure pathFigure, PathFigure pathFigureToCompare)
        {
            List<Point> intersections = new List<Point>();

            Point startPointA = pathFigure.StartPoint;

            foreach (PathSegment segmentA in pathFigure.Segments)
            {
                LineSegment lineSegmentA = segmentA as LineSegment;

                if (lineSegmentA != null)
                {
                    Point endPointA = lineSegmentA.Point;

                    double testSegmentCoefficient = GetCoefficient(startPointA, endPointA);
                    double testSegmentConstant = GetConstant(testSegmentCoefficient, startPointA);

                    Point startPointB = pathFigureToCompare.StartPoint;

                    foreach (PathSegment segmentB in pathFigureToCompare.Segments)
                    {
                        LineSegment lineSegmentB = segmentB as LineSegment;

                        if (lineSegmentB != null)
                        {
                            Point endPointB = lineSegmentB.Point;

                            double lineSegmentCoefficient = GetCoefficient(startPointB, endPointB);

                            if (testSegmentCoefficient == lineSegmentCoefficient)
                            {
                                continue;
                            }

                            double lineSegmentConstant = GetConstant(lineSegmentCoefficient, startPointB);

                            double constantDifference = lineSegmentConstant - testSegmentConstant;
                            double coefficientDifference = testSegmentCoefficient - lineSegmentCoefficient;

                            double yIntersectCoordinate = ((testSegmentCoefficient * constantDifference) / coefficientDifference) + testSegmentConstant;
                            double xIntersectCoordinate = constantDifference / coefficientDifference;

                            bool intersectWithinLineSegementA = false;
                            bool intersectWithinLineSegementB = false;

                            if (startPointA.X < endPointA.X)
                            {
                                intersectWithinLineSegementA = xIntersectCoordinate > startPointA.X && xIntersectCoordinate < endPointA.X;
                            }
                            else if (startPointA.X > endPointA.X)
                            {
                                intersectWithinLineSegementA = xIntersectCoordinate < startPointA.X && xIntersectCoordinate > endPointA.X;
                            }
                            else
                            {
                                // Need to do something if the line segment is vertical.
                            }

                            if (startPointB.X < endPointB.X)
                            {
                                intersectWithinLineSegementB = xIntersectCoordinate > startPointB.X && xIntersectCoordinate < endPointB.X;
                            }
                            else if (startPointB.X > endPointB.X)
                            {
                                intersectWithinLineSegementB = xIntersectCoordinate < startPointB.X && xIntersectCoordinate > endPointB.X;
                            }
                            else
                            {
                                // Need to do something if the line segment is vertical.
                            }

                            if (intersectWithinLineSegementA && intersectWithinLineSegementB)
                            {
                                Point intersectionPoint = new Point(xIntersectCoordinate, yIntersectCoordinate);
                                intersections.Add(intersectionPoint);
                            }

                            /*if ((yIntersectCoordinate > testSegment.PointA.Y && yIntersectCoordinate < testSegment.PointB.Y) || (yIntersectCoordinate < testSegment.PointA.Y && yIntersectCoordinate > testSegment.PointB.Y))
                            {
                                if ((xIntersectCoordinate > testSegment.PointA.X && xIntersectCoordinate < testSegment.PointB.X) || (xIntersectCoordinate < testSegment.PointA.X && xIntersectCoordinate > testSegment.PointB.X))
                                {
                                    if ((yIntersectCoordinate > lineSegment.PointA.Y && yIntersectCoordinate < lineSegment.PointB.Y) || (yIntersectCoordinate < lineSegment.PointA.Y && yIntersectCoordinate > lineSegment.PointB.Y))
                                    {
                                        if ((xIntersectCoordinate > lineSegment.PointA.X && xIntersectCoordinate < lineSegment.PointB.X) || (xIntersectCoordinate < lineSegment.PointA.X && xIntersectCoordinate > lineSegment.PointB.X))
                                        {
                                            Point intersectionPoint = new Point(xIntersectCoordinate, yIntersectCoordinate);

                                            LineSegment segment = (LineSegment)lineSegment;

                                            intersectingLineSegments.Add(lineSegment, intersectionPoint);
                                        }
                                    }
                                }
                            }*/
                        }
                    }
                }
            }

            return intersections.ToArray();
        }
    }
}
