using System;
using System.Windows;
using System.Collections.Generic;

namespace SilverlightMappingToolBasic
{
    public class LineSegment : ILineSegment
    {
        private bool _ignoreLength = false;
        private bool _isCoefficientCalculated = false;
        private bool _isConstantCalculated = false;
        private double _coefficient;
        private double _constant;
        private Point _pointA;
        private Point _pointB;
        private Point _triangulationPoint;

        public LineSegment(Point pointA, Point pointB)
        {
            _pointA = pointA;
            _pointB = pointB;
            _triangulationPoint = new Point(0.0, 0.0);
        }

        public LineSegment(Point pointA, Point pointB, Point triangulationPoint)
            : this(pointA, pointB)
        {
            _triangulationPoint = triangulationPoint;
        }

        public LineSegment(Point pointA, Point pointB, bool ignoreLength)
            : this(pointA, pointB)
        {
            _ignoreLength = ignoreLength;
        }

        public LineSegment(Point pointA, Point pointB, Point triangulationPoint, bool ignoreLength)
            : this(pointA, pointB)
        {
            _ignoreLength = ignoreLength;
            _triangulationPoint = triangulationPoint;
        }

        public string Id
        {
            get;
            set;
        }

        public ILineSegment Resize(IntersectionDetector detector)
        {
            LineSegment lineSegment = new LineSegment(PointA, PointB, new Point(0, 0));
            lineSegment.Id = Id;
            Dictionary<ILineSegment, Point> intersections = detector.FindIntersections(lineSegment);

            foreach (KeyValuePair<ILineSegment, Point> intersection in intersections)
            {
                if (intersection.Key.TriangulationPoint.X != 0 && intersection.Key.TriangulationPoint.Y != 0)
                {
                    Point closestPoint;

                    if (intersection.Key.TriangulateClosestEndPoint(this, out closestPoint))
                    {
                        if (lineSegment.PointA == closestPoint)
                        {
                            lineSegment.PointA = intersection.Value;
                        }
                        else
                        {
                            lineSegment.PointB = intersection.Value;
                        }
                    }
                }
            }

            return lineSegment;
        }

        #region ILineSegment Members

        public bool IgnoreLength
        {
            get
            {
                return _ignoreLength;
            }
            protected set
            {
                _ignoreLength = value;
            }
        }

        public Point TriangulationPoint
        {
            get
            {
                return _triangulationPoint;
            }
            protected set
            {
                _triangulationPoint = value;
            }
        }

        public Point PointA
        {
            get
            {
                return _pointA;
            }
            protected set
            {
                _pointA = value;
            }
        }

        public Point PointB
        {
            get
            {
                return _pointB;
            }
            protected set
            {
                _pointB = value;
            }
        }

        public double Coefficient
        {
            get
            {
                if (!_isCoefficientCalculated)
                {
                    if (PointA.X != PointB.X)
                    {
                        _coefficient = ((PointA.Y - PointB.Y) / (PointA.X - PointB.X));
                    }
                    else
                    {
                        _coefficient = 0;
                    }

                    _isCoefficientCalculated = true;
                }

                return _coefficient;
            }
        }

        public double Constant
        {
            get
            {
                if (!_isConstantCalculated)
                {
                    _constant = PointA.Y - (PointA.X * Coefficient);
                    _isConstantCalculated = true;
                }

                return _constant;
            }
        }

        public bool TriangulateClosestEndPoint(ILineSegment intersectingLineSegment, out Point closestPoint)
        {
            ILineSegment pointASegment = new LineSegment(intersectingLineSegment.PointA, TriangulationPoint);
            ILineSegment pointBSegment = new LineSegment(intersectingLineSegment.PointB, TriangulationPoint);

            IntersectionDetector detector = new IntersectionDetector();

            detector.Add(pointASegment);
            detector.Add(pointBSegment);

            ILineSegment closestSegment;
            Point closestSegmentIntersectionPoint;

            ILineSegment segment = new LineSegment(this.PointA, this.PointB, this.TriangulationPoint, true);

            if (detector.FindIntersection(segment, out closestSegment, out closestSegmentIntersectionPoint))
            {
                closestPoint = closestSegment.PointA;

                return true;
            }
            else
            {
                closestPoint = new Point(0, 0);

                return false;
            }
        }

        #endregion
    }
}
