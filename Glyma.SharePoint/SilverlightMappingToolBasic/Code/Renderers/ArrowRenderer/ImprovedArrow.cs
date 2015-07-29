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
using System.Windows.Media.Effects;

using SilverlightMappingToolBasic.Controls;

namespace SilverlightMappingToolBasic
{
    public class ImprovedArrow : Canvas, IRelationshipRenderer
    {
        private Path _arrowPath;
        private PathFigure _arrowFigure;
        private LineSegment _arrowBody;
        private PathGeometry _arrow;
        private INodeRenderer _to = null;
        private INodeRenderer _from = null;
        private Line _arrowHead;
        private Rectangle _toSelectionRect;
        private Rectangle _fromSelectionRect;
        
        private bool _isSelected;

        public ImprovedArrow(NavigatorView parentNavigatorView, IRelationshipProxy relationship)
            : base()
        {
            Relationship = relationship;
            ParentNavigatorView = parentNavigatorView;
            
            this.SetValue(Canvas.ZIndexProperty, -1);

            DropShadowEffect selectionEffect = new DropShadowEffect();
            selectionEffect.BlurRadius = 0;
            selectionEffect.ShadowDepth = 0;
            selectionEffect.Color = Colors.Red;
            selectionEffect.Direction = 0;
            this.Effect = selectionEffect;

            _arrowBody = new LineSegment();

            _arrowFigure = new PathFigure();
            _arrowFigure.Segments.Add(_arrowBody);

            _arrow = new PathGeometry();
            _arrow.Figures.Add(_arrowFigure);

            _arrowPath = new Path();
            _arrowPath.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            _arrowPath.StrokeEndLineCap = PenLineCap.Triangle;
            _arrowPath.StrokeThickness = 1.25;
            _arrowPath.Data = _arrow;

            _arrowHead = new Line();
            _arrowHead.StrokeEndLineCap = PenLineCap.Triangle;
            _arrowHead.StrokeThickness = 8;
            _arrowHead.Stroke = new SolidColorBrush(Colors.Black);

            this.Children.Add(_arrowHead);
            this.Children.Add(_arrowPath);

            _toSelectionRect = new Rectangle();
            _toSelectionRect.Width = 6;
            _toSelectionRect.Height = 6;
            _toSelectionRect.Stroke = new SolidColorBrush(Colors.Black);
            _toSelectionRect.StrokeThickness = 1;
            _toSelectionRect.Visibility = Visibility.Collapsed;
            _toSelectionRect.Fill = new SolidColorBrush(Colors.White);
            _toSelectionRect.Opacity = 0.7;
            _toSelectionRect.MouseLeftButtonDown += new MouseButtonEventHandler(_toSelectionRect_MouseLeftButtonDown);
            _toSelectionRect.MouseLeftButtonUp += new MouseButtonEventHandler(_toSelectionRect_MouseLeftButtonUp);

            _fromSelectionRect = new Rectangle();
            _fromSelectionRect.Width = 6;
            _fromSelectionRect.Height = 6;
            _fromSelectionRect.Stroke = new SolidColorBrush(Colors.Black);
            _fromSelectionRect.StrokeThickness = 1;
            _fromSelectionRect.Visibility = Visibility.Collapsed;
            _fromSelectionRect.Fill = new SolidColorBrush(Colors.White);
            _fromSelectionRect.Opacity = 0.7;
            _fromSelectionRect.MouseLeftButtonDown += new MouseButtonEventHandler(_fromSelectionRect_MouseLeftButtonDown);
            _fromSelectionRect.MouseLeftButtonUp += new MouseButtonEventHandler(_fromSelectionRect_MouseLeftButtonUp);

            this.Children.Add(_toSelectionRect);
            this.Children.Add(_fromSelectionRect);

            this.Loaded += new RoutedEventHandler(OnLoaded);
        }

        private void _toSelectionRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Debug.WriteLine("Left Down in To Selection Rectangle");
        }

        private void _toSelectionRect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Debug.WriteLine("Left Up in To Selection Rectangle");
        }

        private void _fromSelectionRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Debug.WriteLine("Left Down in From Selection Rectangle");
        }

        private void _fromSelectionRect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Debug.WriteLine("Left Up in From Selection Rectangle");
        }

        protected virtual void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateRelationship();
            _arrowPath.MouseLeftButtonDown += new MouseButtonEventHandler(_arrowPath_MouseLeftButtonDown);
            _arrowHead.MouseLeftButtonDown += new MouseButtonEventHandler(_arrowPath_MouseLeftButtonDown);
            _arrowPath.MouseRightButtonDown += new MouseButtonEventHandler(_arrowPath_MouseRightButtonDown);
            _arrowHead.MouseRightButtonDown += new MouseButtonEventHandler(_arrowPath_MouseRightButtonDown);
            _arrowPath.MouseLeftButtonUp += new MouseButtonEventHandler(_arrowPath_MouseLeftButtonUp);
            _arrowHead.MouseLeftButtonUp += new MouseButtonEventHandler(_arrowPath_MouseLeftButtonUp);
        }

        private void _arrowPath_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void _arrowPath_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //INodeService service = IoC.IoCContainer.GetInjectionInstance().GetInstance<INodeService>();
           // RelationshipContextMenu contextMenu = new RelationshipContextMenu(this.Relationship, service);
            //ContextMenuService.SetContextMenu(Parent, contextMenu);
            //e.Handled = true;
        }

        private void _arrowPath_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsSelected)
            {
                IsSelected = false;
            }
            else
            {
                IsSelected = true;
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                if (!value)
                {
                    ((DropShadowEffect)this.Effect).BlurRadius = 0;
                    _arrowHead.Stroke = new SolidColorBrush(Colors.Black);
                    _arrowPath.Stroke = new SolidColorBrush(Colors.Black);
                    _fromSelectionRect.Visibility = Visibility.Collapsed;
                    _toSelectionRect.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ((DropShadowEffect)this.Effect).BlurRadius = 100;
                    _arrowHead.Stroke = new SolidColorBrush(Colors.Red);
                    _arrowPath.Stroke = new SolidColorBrush(Colors.Red);
                    _fromSelectionRect.Visibility = Visibility.Visible;
                    _toSelectionRect.Visibility = Visibility.Visible;
                }
            }
        }

        public INodeRenderer To
        {
            get
            {
                foreach (IDescriptorProxy descriptor in Relationship.Descriptors.GetByDescriptorTypeName("To"))
                {
                    // TODO: Needs to be replaced with a proper ID check from the database through the use of the IoC.
                    if (descriptor.Node != null)
                    {
                        _to = ParentNavigatorView.NodeRenderers[descriptor.Node.Id];
                    }
                }

                return _to;
            }
        }

        public INodeRenderer From
        {
            get
            {
                foreach (IDescriptorProxy descriptor in Relationship.Descriptors.GetByDescriptorTypeName("From"))
                {
                    // TODO: Needs to be replaced with a proper ID check from the database through the use of the IoC.
                    if (descriptor.Node != null)
                    {
                        _from = ParentNavigatorView.NodeRenderers[descriptor.Node.Id];
                    }
                }

                return _from;
            }
        }

        #region IRelationshipRenderer Members

        public IRelationshipProxy Relationship
        {
            get;
            protected set;
        }

        public NavigatorView ParentNavigatorView
        {
            get;
            protected set;
        }

        public virtual void UpdateRelationship()
        {
            //PathGeometry nodeAClippingRegion = NodeA.Skin.SkinClippingGeometry;
            //PathGeometry nodeBClippingRegion = NodeB.Skin.SkinClippingGeometry;
            lock (this)
            {
                if (To != null && From != null)
                {
                    double nodeACentreX = (From.Skin.NodeSkinWidth / 2);
                    double nodeACentreY = (From.Skin.NodeSkinHeight / 2);

                    double nodeBCentreX = (To.Skin.NodeSkinWidth / 2);
                    double nodeBCentreY = (To.Skin.NodeSkinHeight / 2);

                    nodeACentreX += From.Location.X;
                    nodeACentreY += From.Location.Y;

                    //Debug.WriteLine("Improved Arrow A: (" + NodeA.Location.X + ", " + NodeA.Location.Y + ")");

                    nodeBCentreX += To.Location.X;
                    nodeBCentreY += To.Location.Y;


                    //Debug.WriteLine("Improved Arrow B: (" + NodeB.Location.X + ", " + NodeB.Location.Y + ")");

                    Point startPoint = new Point(nodeACentreX, nodeACentreY);
                    Point endPoint = new Point(nodeBCentreX, nodeBCentreY);

                    ArrowDirection arrowDirection = CalculateArrowDirection(startPoint, endPoint);
                    double horizontalEdgeLength = 0;
                    double verticalEdgeLength = 0;
                    double angle = 0.0;
                    double a = 0, b = 0, c = 0, d = 0, e = 0, f = 0, coefficient = 0;
                    double startBuffer = (From.Skin.NodeSkinWidth / 2) + 10;
                    double endBuffer = (To.Skin.NodeSkinWidth / 2) + 10;
                    //Debug.WriteLine("Arrow Direction: {0}", Enum.GetName(typeof(ArrowDirection), arrowDirection));
                    switch (arrowDirection)
                    {
                        case ArrowDirection.North:
                            startPoint.Y = startPoint.Y - (To.Skin.NodeSkinHeight / 2) - 10;
                            endPoint.Y = endPoint.Y + (From.Skin.NodeSkinHeight / 2) + 10;
                            _arrowHead.X1 = endPoint.X;
                            _arrowHead.X2 = endPoint.X;
                            _arrowHead.Y1 = endPoint.Y + 1;
                            _arrowHead.Y2 = endPoint.Y;

                            _toSelectionRect.SetValue(Canvas.LeftProperty, endPoint.X - 3);
                            _toSelectionRect.SetValue(Canvas.TopProperty, endPoint.Y);

                            _fromSelectionRect.SetValue(Canvas.LeftProperty, startPoint.X - 3);
                            _fromSelectionRect.SetValue(Canvas.TopProperty, startPoint.Y);
                            break;
                        case ArrowDirection.South:
                            startPoint.Y = startPoint.Y + (To.Skin.NodeSkinHeight / 2) + 10;
                            endPoint.Y = endPoint.Y - (From.Skin.NodeSkinHeight / 2) - 10;
                            _arrowHead.X1 = endPoint.X;
                            _arrowHead.X2 = endPoint.X;
                            _arrowHead.Y1 = endPoint.Y - 1;
                            _arrowHead.Y2 = endPoint.Y;

                            _toSelectionRect.SetValue(Canvas.LeftProperty, endPoint.X - 3);
                            _toSelectionRect.SetValue(Canvas.TopProperty, endPoint.Y);

                            _fromSelectionRect.SetValue(Canvas.LeftProperty, startPoint.X - 3);
                            _fromSelectionRect.SetValue(Canvas.TopProperty, startPoint.Y);
                            break;
                        case ArrowDirection.West:
                            startPoint.X = startPoint.X - (To.Skin.NodeSkinWidth / 2) - 10;
                            endPoint.X = endPoint.X + (From.Skin.NodeSkinWidth / 2) + 10;
                            _arrowHead.X1 = endPoint.X + 1;
                            _arrowHead.X2 = endPoint.X;
                            _arrowHead.Y1 = endPoint.Y;
                            _arrowHead.Y2 = endPoint.Y;

                            _toSelectionRect.SetValue(Canvas.LeftProperty, endPoint.X);
                            _toSelectionRect.SetValue(Canvas.TopProperty, endPoint.Y - 3);

                            _fromSelectionRect.SetValue(Canvas.LeftProperty, startPoint.X);
                            _fromSelectionRect.SetValue(Canvas.TopProperty, startPoint.Y - 3);
                            break;
                        case ArrowDirection.East:
                            startPoint.X = startPoint.X + (To.Skin.NodeSkinWidth / 2) + 10;
                            endPoint.X = endPoint.X - (From.Skin.NodeSkinWidth / 2) - 10;
                            _arrowHead.X1 = endPoint.X - 1;
                            _arrowHead.X2 = endPoint.X;
                            _arrowHead.Y1 = endPoint.Y;
                            _arrowHead.Y2 = endPoint.Y;

                            _toSelectionRect.SetValue(Canvas.LeftProperty, endPoint.X);
                            _toSelectionRect.SetValue(Canvas.TopProperty, endPoint.Y - 3);

                            _fromSelectionRect.SetValue(Canvas.LeftProperty, startPoint.X);
                            _fromSelectionRect.SetValue(Canvas.TopProperty, startPoint.Y - 3);
                            break;
                        case ArrowDirection.NorthWest:
                            horizontalEdgeLength = Math.Abs(startPoint.X - endPoint.X);
                            verticalEdgeLength = Math.Abs(startPoint.Y - endPoint.Y);
                            angle = Math.Atan(verticalEdgeLength / horizontalEdgeLength);
                            a = Math.Abs(Math.Sin(angle) * endBuffer);
                            coefficient = (startPoint.X - endPoint.X) / (startPoint.Y - endPoint.Y);
                            b = a * coefficient;
                            endPoint.X = endPoint.X + b;
                            endPoint.Y = endPoint.Y + a;

                            c = Math.Abs(Math.Sin(angle) * startBuffer);
                            d = c * coefficient;
                            startPoint.X = startPoint.X - d;
                            startPoint.Y = startPoint.Y - c;

                            e =  Math.Abs(Math.Cos(angle) * 1);
                            f = Math.Abs(Math.Sin(angle) * 1);
                            _arrowHead.X1 = endPoint.X + e;
                            _arrowHead.X2 = endPoint.X;
                            _arrowHead.Y1 = endPoint.Y + f;
                            _arrowHead.Y2 = endPoint.Y;

                            _toSelectionRect.SetValue(Canvas.LeftProperty, endPoint.X - 3);
                            _toSelectionRect.SetValue(Canvas.TopProperty, endPoint.Y - 3);

                            _fromSelectionRect.SetValue(Canvas.LeftProperty, startPoint.X - 3);
                            _fromSelectionRect.SetValue(Canvas.TopProperty, startPoint.Y - 3);
                            break;
                        case ArrowDirection.NorthEast:
                            horizontalEdgeLength = Math.Abs(startPoint.X - endPoint.X);
                            verticalEdgeLength = Math.Abs(startPoint.Y - endPoint.Y);
                            angle = Math.Atan(verticalEdgeLength / horizontalEdgeLength);
                            a = Math.Abs(Math.Sin(angle) * endBuffer);
                            coefficient = (endPoint.X - startPoint.X) / (startPoint.Y - endPoint.Y);
                            b = a * coefficient;
                            endPoint.X = endPoint.X - b;
                            endPoint.Y = endPoint.Y + a;

                            c = Math.Abs(Math.Sin(angle) * startBuffer);
                            d = c * coefficient;
                            startPoint.X = startPoint.X + d;
                            startPoint.Y = startPoint.Y - c;

                            e =  Math.Abs(Math.Cos(angle) * 1);
                            f = Math.Abs(Math.Sin(angle) * 1);
                            _arrowHead.X1 = endPoint.X - e;
                            _arrowHead.X2 = endPoint.X;
                            _arrowHead.Y1 = endPoint.Y + f;
                            _arrowHead.Y2 = endPoint.Y;

                            _toSelectionRect.SetValue(Canvas.LeftProperty, endPoint.X - 3);
                            _toSelectionRect.SetValue(Canvas.TopProperty, endPoint.Y - 3);

                            _fromSelectionRect.SetValue(Canvas.LeftProperty, startPoint.X - 3);
                            _fromSelectionRect.SetValue(Canvas.TopProperty, startPoint.Y - 3);
                            break;
                        case ArrowDirection.SouthWest:
                            horizontalEdgeLength = Math.Abs(startPoint.X - endPoint.X);
                            verticalEdgeLength = Math.Abs(startPoint.Y - endPoint.Y);
                            angle = Math.Atan(verticalEdgeLength / horizontalEdgeLength);
                            a = Math.Abs(Math.Sin(angle) * endBuffer);
                            coefficient = (startPoint.X - endPoint.X) / (endPoint.Y - startPoint.Y);
                            b = a * coefficient;
                            endPoint.X = endPoint.X + b;
                            endPoint.Y = endPoint.Y - a;

                            c = Math.Abs(Math.Sin(angle) * startBuffer);
                            d = c * coefficient;
                            startPoint.X = startPoint.X - d;
                            startPoint.Y = startPoint.Y + c;

                            e =  Math.Abs(Math.Cos(angle) * 1);
                            f = Math.Abs(Math.Sin(angle) * 1);
                            _arrowHead.X1 = endPoint.X + e;
                            _arrowHead.X2 = endPoint.X;
                            _arrowHead.Y1 = endPoint.Y - f;
                            _arrowHead.Y2 = endPoint.Y;

                            _toSelectionRect.SetValue(Canvas.LeftProperty, endPoint.X - 3);
                            _toSelectionRect.SetValue(Canvas.TopProperty, endPoint.Y - 3);

                            _fromSelectionRect.SetValue(Canvas.LeftProperty, startPoint.X - 3);
                            _fromSelectionRect.SetValue(Canvas.TopProperty, startPoint.Y - 3);
                            break;
                        case ArrowDirection.SouthEast:
                            horizontalEdgeLength = Math.Abs(startPoint.X - endPoint.X);
                            verticalEdgeLength = Math.Abs(startPoint.Y - endPoint.Y);
                            angle = Math.Atan(verticalEdgeLength / horizontalEdgeLength);
                            a = Math.Abs(Math.Sin(angle) * endBuffer);
                            coefficient = (endPoint.X - startPoint.X) / (endPoint.Y - startPoint.Y);
                            b = a * coefficient;
                            endPoint.X = endPoint.X - b;
                            endPoint.Y = endPoint.Y - a;

                            c = Math.Abs(Math.Sin(angle) * startBuffer);
                            d = c * coefficient;
                            startPoint.X = startPoint.X + d;
                            startPoint.Y = startPoint.Y + c;

                            e =  Math.Abs(Math.Cos(angle) * 1);
                            f = Math.Abs(Math.Sin(angle) * 1);
                            _arrowHead.X1 = endPoint.X - e;
                            _arrowHead.X2 = endPoint.X;
                            _arrowHead.Y1 = endPoint.Y - f;
                            _arrowHead.Y2 = endPoint.Y;

                            _toSelectionRect.SetValue(Canvas.LeftProperty, endPoint.X - 3);
                            _toSelectionRect.SetValue(Canvas.TopProperty, endPoint.Y - 3);

                            _fromSelectionRect.SetValue(Canvas.LeftProperty, startPoint.X - 3);
                            _fromSelectionRect.SetValue(Canvas.TopProperty, startPoint.Y - 3);
                            break;
                        default:
                            break;
                    }
                    //Debug.WriteLine("Angle: {0:0.0000}", angle.ToString());
                    //Debug.WriteLine("StartPoint: {0:0.0000}", startPoint.ToString());
                    //Debug.WriteLine("EndPoint: {0:0.0000}", endPoint.ToString());
                    //Debug.WriteLine("To Node Centre: {0}", new Point(nodeACentreX, nodeACentreY).ToString());
                    //Debug.WriteLine("From Node Centre: {0}", new Point(nodeBCentreX, nodeBCentreY).ToString());
                    //Debug.WriteLine("Coeffient: {0:0.0000}", coefficient.ToString());
                    //Debug.WriteLine("StartBuffer: {0}", startBuffer.ToString());
                    //Debug.WriteLine("EndBuffer: {0}", endBuffer.ToString());
                    //Debug.WriteLine("Horizontal: {0}", horizontalEdgeLength.ToString());
                    //Debug.WriteLine("Vertical: {0}", verticalEdgeLength.ToString());
                    //Debug.WriteLine("Hypotenuse Should Be: {0:0.0000}", (Math.Sqrt(Math.Pow(verticalEdgeLength, 2) + Math.Pow(horizontalEdgeLength, 2)) - (2*endBuffer)));
                    //double newHor = Math.Abs(startPoint.X - endPoint.X);
                    //double newVer = Math.Abs(startPoint.Y - endPoint.Y);
                    //Debug.WriteLine("Hypotenuse Actual: {0:0.0000}", Math.Sqrt(Math.Pow(newHor, 2) + Math.Pow(newVer, 2)));
                    //Debug.WriteLine("A: {0:0.0000} B:{1:0.0000} C:{2:0.0000} D:{3:0.0000}", a, b, c, d);

                    // Debug.WriteLine("To: {0} - {1}", this.To.Node.Name, To.Location.ToString());
                    // Debug.WriteLine("From: {0} - {1}", this.From.Node.Name, From.Location.ToString());

                    _arrowBody.Point = endPoint;

                    _arrowFigure.StartPoint = startPoint;

                    //Debug.WriteLine("Arrow Draw: " + ((Canvas)this.Parent).Name);
                }
            }
        }
        #endregion

        private enum ArrowDirection
        {
            North,
            NorthEast,
            NorthWest,
            South,
            SouthEast,
            SouthWest,
            West,
            East
        }

        private ArrowDirection CalculateArrowDirection(Point fromNodePos, Point toNodePos)
        {
            ArrowDirection result;
            if (toNodePos.X > fromNodePos.X)
            {
                // arrow drawn east
                if (toNodePos.Y > fromNodePos.Y)
                {
                    //arrow drawn south (east)
                    result = ArrowDirection.SouthEast;
                }
                else if (toNodePos.Y < fromNodePos.Y)
                {
                    //arrow drawn north (east)
                    result = ArrowDirection.NorthEast;
                }
                else
                {
                    //arrow is horizontal (east)
                    result = ArrowDirection.East;
                }
            }
            else if (toNodePos.X < fromNodePos.X)
            {
                // arrow drawn west
                if (toNodePos.Y > fromNodePos.Y)
                {
                    //arrow drawn south (west)
                    result = ArrowDirection.SouthWest;
                }
                else if (toNodePos.Y < fromNodePos.Y)
                {
                    //arrow drawn north (west)
                    result = ArrowDirection.NorthWest;
                }
                else
                {
                    //arrow drawn due west
                    result = ArrowDirection.West;
                }
            }
            else
            {
                //arrow drawn north or south
                if (toNodePos.Y > fromNodePos.Y)
                {
                    //arrow drawn south
                    result = ArrowDirection.South;
                }
                else
                {
                    //arrow drawn north
                    result = ArrowDirection.North;
                }
            }

            return result;
        }
    }
}
