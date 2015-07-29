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
    public class IntersectionDetector
    {
        private List<ILineSegment> _lineSegments;

        public IntersectionDetector()
        {
            _lineSegments = new List<ILineSegment>();
        }

        public void Add(ILineSegment lineSegment)
        {
            _lineSegments.Add(lineSegment);
        }

        public void Remove(ILineSegment lineSegment)
        {
            _lineSegments.Remove(lineSegment);
        }

        public void Clear()
        {
            _lineSegments.Clear();
        }

        public bool Contains(ILineSegment lineSegment)
        {
            return _lineSegments.Contains(lineSegment);
        }

        public ILineSegment[] Segments
        {
            get
            {
                return _lineSegments.ToArray();
            }
        }

        public bool FindIntersection(ILineSegment testSegment, out ILineSegment intersection, out Point intersectionCoordinates)
        {
            double testSegmentCoefficient = testSegment.Coefficient;
            double testSegmentConstant = testSegment.Constant;

            foreach (ILineSegment lineSegment in _lineSegments)
            {
                double lineSegmentCoefficient = lineSegment.Coefficient;

                if (testSegmentCoefficient == lineSegmentCoefficient)
                {
                    continue;
                }

                double lineSegmentConstant = lineSegment.Constant;

                double constantDifference = lineSegmentConstant - testSegmentConstant;
                double coefficientDifference = testSegmentCoefficient - lineSegmentCoefficient;

                double yIntersectCoordinate = ((testSegmentCoefficient * constantDifference) / coefficientDifference) + testSegmentConstant;
                double xIntersectCoordinate = constantDifference / coefficientDifference;

                //if (!((yIntersectCoordinate > testSegment.PointA.Y && yIntersectCoordinate > testSegment.PointB.Y) || (yIntersectCoordinate < testSegment.PointA.Y && yIntersectCoordinate < testSegment.PointB.Y)) || testSegment.IgnoreLength)
                //{
                //    if (!((xIntersectCoordinate > testSegment.PointA.X && xIntersectCoordinate > testSegment.PointB.X) || (xIntersectCoordinate < testSegment.PointA.X && xIntersectCoordinate < testSegment.PointB.X)) || testSegment.IgnoreLength)
                //    {
                //        if (!((yIntersectCoordinate > lineSegment.PointA.Y && yIntersectCoordinate > lineSegment.PointB.Y) || (yIntersectCoordinate < lineSegment.PointA.Y && yIntersectCoordinate < lineSegment.PointB.Y)) || lineSegment.IgnoreLength)
                //        {
                            //if (!((xIntersectCoordinate > lineSegment.PointA.X && xIntersectCoordinate > lineSegment.PointB.X) || (xIntersectCoordinate < lineSegment.PointA.X && xIntersectCoordinate < lineSegment.PointB.X)) || lineSegment.IgnoreLength)
                            //{
                if ((yIntersectCoordinate < lineSegment.PointA.Y && yIntersectCoordinate > lineSegment.PointB.Y) || (yIntersectCoordinate > lineSegment.PointA.Y && yIntersectCoordinate < lineSegment.PointB.Y) || lineSegment.IgnoreLength)
                {
                    if ((xIntersectCoordinate < lineSegment.PointA.X && xIntersectCoordinate > lineSegment.PointB.X) || (xIntersectCoordinate > lineSegment.PointA.X && xIntersectCoordinate < lineSegment.PointB.X) || lineSegment.IgnoreLength)
                    {
                        intersection = lineSegment;
                        intersectionCoordinates = new Point(xIntersectCoordinate, yIntersectCoordinate);

                        return true;
                    }
                }
                            //}
                //        }
                //    }
                //}
            }

            intersection = null;
            intersectionCoordinates = new Point();
            return false;
        }

        public Dictionary<ILineSegment, Point> FindIntersections(ILineSegment testSegment)
        {
            Dictionary<ILineSegment, Point> intersectingLineSegments = new Dictionary<ILineSegment, Point>();

            double testSegmentCoefficient = testSegment.Coefficient;
            double testSegmentConstant = testSegment.Constant;

            foreach (ILineSegment lineSegment in _lineSegments)
            {
                double lineSegmentCoefficient = lineSegment.Coefficient;

                if (testSegmentCoefficient == lineSegmentCoefficient)
                {
                    continue;
                }

                double lineSegmentConstant = lineSegment.Constant;

                double constantDifference = lineSegmentConstant - testSegmentConstant;
                double coefficientDifference = testSegmentCoefficient - lineSegmentCoefficient;

                double yIntersectCoordinate = ((testSegmentCoefficient * constantDifference) / coefficientDifference) + testSegmentConstant;
                double xIntersectCoordinate = constantDifference / coefficientDifference;

                if ((yIntersectCoordinate > testSegment.PointA.Y && yIntersectCoordinate < testSegment.PointB.Y) || (yIntersectCoordinate < testSegment.PointA.Y && yIntersectCoordinate > testSegment.PointB.Y))
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
                }
            }

            return intersectingLineSegments;
        }
    }
}
