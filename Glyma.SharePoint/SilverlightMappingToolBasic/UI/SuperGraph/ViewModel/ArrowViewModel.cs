using System;
using System.Windows;

namespace SilverlightMappingToolBasic.UI.SuperGraph.ViewModel
{
    public class ArrowViewModel : ViewModelBase
    {
        private const double ArrowWidth = 2.5;
        private const double ArrowHeadLength = 7.0;

        private Point _location;
        private Point _firstCoordinate;
        private Point _secondCoordinate;
        private double _angle;
        private double _length;
        private double _centreX;
        private double _centreY;
        private double _actualLength;

        public Relationship ViewModelRelationship
        {
            get;
            set;
        }

        public Guid Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets and sets the angle of the arrow.
        /// </summary>
        public double Angle
        {
            get
            {
                return _angle;
            }
            set
            {
                if (!_angle.Equals(value))
                {
                    _angle = value;
                    OnNotifyPropertyChanged("Angle");
                }
            }
        }

        /// <summary>
        /// Gets and sets the coordinate for one end of the arrow.
        /// </summary>
        public Point FirstCoordinate
        {
            get
            {
                return _firstCoordinate;
            }
            set
            {
                _firstCoordinate = value;

                double length = _firstCoordinate.Distance(SecondCoordinate);
                Point midway = _firstCoordinate.GetMidway(SecondCoordinate);

                midway.X = midway.X - ArrowWidth / 2.0; // This is half the width of the arrow control.
                midway.Y = midway.Y - (length / 2);

                Location = midway;
            }
        }

        /// <summary>
        /// Gets and sets the coordinate for one end of the arrow.
        /// </summary>
        public Point SecondCoordinate
        {
            get
            {
                return _secondCoordinate;
            }
            set
            {
                _secondCoordinate = value;

                double length = FirstCoordinate.Distance(_secondCoordinate);
                Point midway = FirstCoordinate.GetMidway(_secondCoordinate);

                midway.X = midway.X - ArrowWidth / 2.0; // This is half the width of the arrow control.
                midway.Y = midway.Y - (length / 2);

                Location = midway;
            }
        }

        /// <summary>
        /// Gets the location of the arrow with respect to the given coordinates.
        /// </summary>
        public Point Location
        {
            get
            {
                return _location;
            }
            private set
            {
                if (_location != value)
                {
                    _location = value;
                    OnNotifyPropertyChanged("Location");
                }
            }
        }

        /// <summary>
        /// Gets and sets the desired length of the arrow, inclusive of the arrow's head.
        /// </summary>
        public double Length
        {
            get
            {
                return _length;
            }
            set
            {
                _length = value;

                double arrowLength;

                if (_length < ArrowHeadLength)
                {
                    arrowLength = 0.0;
                }
                else
                {
                    arrowLength = _length - ArrowHeadLength;
                }

                ActualLength = arrowLength;
                CentreX = ArrowWidth / 2.0;
                CentreY = _length / 2;
            }
        }

        /// <summary>
        /// Gets the length of the arrow's tail, exclusive of the arrow's head.
        /// </summary>
        public double ActualLength
        {
            get
            {
                return _actualLength;
            }
            private set
            {
                if (!_actualLength.Equals(value))
                {
                    _actualLength = value;
                    OnNotifyPropertyChanged("ActualLength");
                }
            }
        }

        /// <summary>
        /// Gets the x-axis value for the centre of the arrow control.
        /// </summary>
        public double CentreX
        {
            get
            {
                return _centreX;
            }
            private set
            {
                if (!_centreX.Equals(value))
                {
                    _centreX = value;
                    OnNotifyPropertyChanged("CentreX");
                }
            }
        }

        /// <summary>
        /// Gets the y-axis value for the centre of the arrow control.
        /// </summary>
        public double CentreY
        {
            get
            {
                return _centreY;
            }
            private set
            {
                if (!_centreY.Equals(value))
                {
                    _centreY = value;
                    OnNotifyPropertyChanged("CentreY");
                }
            }
        }

        /// <summary>
        /// Gets whether there is space for a line or not. The arrow control shouldn't draw if this is false.
        /// </summary>
        public bool HasSpaceForLine
        {
            get;
            set;
        }
    }
}
