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
using SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller
{
    public class ArrowController : ILinearEquation
    {
        public ArrowController(ViewModel.Relationship relationship, IPolymorphicShape from, IPolymorphicShape to)
        {
            From = from;
            To = to;

            ViewModel = new ViewModel.ArrowViewModel();
            ViewModel.ViewModelRelationship = relationship;

            RefreshViewModel();

            From.ShapeMoved += OnFromShapeMoved;
            To.ShapeMoved += OnToShapeMoved;
        }

        public IPolymorphicShape From
        {
            get;
            set;
        }

        public IPolymorphicShape To
        {
            get;
            set;
        }

        public ViewModel.ArrowViewModel ViewModel
        {
            get;
            set;
        }

        public IShapeEquation LeftMostShape
        {
            get
            {
                if (From.Centre.X <= To.Centre.X)
                {
                    return From.GetShapeLeftMost();
                }
                else
                {
                    return To.GetShapeLeftMost();
                }
            }
        }

        public IShapeEquation RightMostShape
        {
            get
            {
                if (From.Centre.X <= To.Centre.X)
                {
                    return To.GetShapeRightMost();
                }
                else
                {
                    return From.GetShapeRightMost();
                }
            }
        }

        public IShapeEquation LowestShape
        {
            get
            {
                if (From.Centre.Y <= To.Centre.Y)
                {
                    return From.GetShapeLowest();
                }
                else
                {
                    return To.GetShapeLowest();
                }
            }
        }

        public IShapeEquation HighestShape
        {
            get
            {
                if (From.Centre.Y <= To.Centre.Y)
                {
                    return To.GetShapeHighest();
                }
                else
                {
                    return From.GetShapeHighest();
                }
            }
        }

        public bool HasSpaceForLine
        {
            get
            {
                double distance = From.Centre.Distance(To.Centre);

                if (distance <= (LowestShape.GetCentreToBoundaryDistance(this) + HighestShape.GetCentreToBoundaryDistance(this)))
                {
                    return false;
                }
                return true;
            }
        }

        public double Gradient
        {
            get
            {
                double gradient;

                double xDelta = To.Centre.X - From.Centre.X;
                double yDelta = To.Centre.Y - From.Centre.Y;

                if (xDelta == 0)
                {
                    // If xDelta is 0 then this is a vertical line.
                    if (yDelta >= 0)
                    {
                        gradient = double.PositiveInfinity;
                    }
                    else
                    {
                        gradient = double.NegativeInfinity;
                    }
                }
                else
                {
                    gradient = yDelta / xDelta;
                }

                return gradient;
            }
        }

        double ILinearEquation.Gradient
        {
            get
            {
                return Gradient;
            }
        }

        public double Direction
        {
            get
            {
                if (LeftMostShape == From.GetShapeLeftMost())
                {
                    return 1;
                }
                else if (LeftMostShape == To.GetShapeLeftMost())
                {
                    return -1;
                }

                return 0;
            }
        }

        private void OnFromShapeMoved(object sender, ShapeMovedEventArgs e)
        {
            RefreshViewModel();
        }

        private void OnToShapeMoved(object sender, ShapeMovedEventArgs e)
        {
            RefreshViewModel();
        }

        private void RefreshViewModel()
        {
            Point lowestShapeIntersection = GetLowestShapeIntersection();
            Point highestShapeIntersection = GetHighestShapeIntersection();

            double distanceBetweenBoundaryIntersections = lowestShapeIntersection.Distance(highestShapeIntersection);

            ViewModel.HasSpaceForLine = HasSpaceForLine;
            ViewModel.Angle = GetAngle();
            ViewModel.FirstCoordinate = lowestShapeIntersection;
            ViewModel.SecondCoordinate = highestShapeIntersection;
            ViewModel.Length = distanceBetweenBoundaryIntersections;
        }

        private Point GetIntersection(IShapeEquation shape)
        {
            Point[] intersections = shape.FindIntersections(this);

            Point chosenIntersection = intersections[0];

            foreach (Point intersection in intersections)
            {
                bool isInBoundary = intersection.IsInBoundaryInclusively(LeftMostShape.Centre, RightMostShape.Centre, LowestShape.Centre, HighestShape.Centre);

                if (isInBoundary)
                {
                    chosenIntersection = intersection;
                    break;
                }
            }

            return chosenIntersection;
        }

        public Point GetLowestShapeIntersection()
        {
            return GetIntersection(LowestShape);
        }

        public Point GetHighestShapeIntersection()
        {
            return GetIntersection(HighestShape);
        }

        public double GetAngle()
        {
            double angle = 0.0;
            double angleModifier = 0.0;
            double tangent = Math.Abs(Gradient);

            // As our Y-axis is upside down, we need to also treat this upside down too.
            if (Gradient > 0)
            {
                angleModifier += 90;
                tangent = 1 / tangent;
            }

            if (Direction < 0)
            {
                angleModifier += 180;
            }

            angle = Math.Atan(tangent);
            angle = angle * 180 / Math.PI;
            angle = 90 - angle; // In reference to a triangle, we're actually wanting the internal angle but what we're currently getting is the external angle.
            angle += angleModifier;

            return angle;
        }

        public double GetLength()
        {
            Point fromIntersection = GetLowestShapeIntersection();
            Point toIntersection = GetHighestShapeIntersection();

            return fromIntersection.Distance(toIntersection);
        }
    }
}
