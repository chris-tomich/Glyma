using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace SilverlightMappingToolBasic.Controls
{
    public enum ArrowDirection
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

    [TemplatePart(Name = ArrowControl.ArrowPart, Type = typeof(PathGeometry))]
    [TemplatePart(Name = ArrowControl.ArrowFigurePart, Type = typeof(PathFigure))]
    [TemplatePart(Name = ArrowControl.ArrowBodyPart, Type = typeof(LineSegment))]
    [TemplatePart(Name = ArrowControl.ArrowPathPart, Type = typeof(Path))]
    [TemplatePart(Name = ArrowControl.ArrowHeadPart, Type = typeof(Line))]
    [TemplatePart(Name = ArrowControl.ToSelectionRectPart, Type = typeof(Rectangle))]
    [TemplatePart(Name = ArrowControl.FromSelectionRectPart, Type = typeof(Rectangle))]
    [TemplateVisualState(Name = ArrowControl.MouseOverStateName, GroupName = ArrowControl.CommonStatesGroupName)]
    [TemplateVisualState(Name = ArrowControl.NormalStateName, GroupName = ArrowControl.CommonStatesGroupName)]
    [TemplateVisualState(Name = ArrowControl.SelectedStateName, GroupName = ArrowControl.FocusStatesGroupName)]
    [TemplateVisualState(Name = ArrowControl.NotSelectedStateName, GroupName = ArrowControl.FocusStatesGroupName)]
    [TemplateVisualState(Name = ArrowControl.EdittingStateName, GroupName = ArrowControl.EditStatesGroupName)]
    [TemplateVisualState(Name = ArrowControl.NotEdittingStateName, GroupName = ArrowControl.EditStatesGroupName)]
    public class ArrowControl : Control
    {
        //Template Parts
        public const string ArrowPart = "Arrow";
        public const string ArrowFigurePart = "ArrowFigure";
        public const string ArrowBodyPart = "ArrowBody";
        public const string ArrowPathPart = "ArrowPath";
        public const string ArrowHeadPart = "ArrowHead";
        public const string ToSelectionRectPart = "ToSelectionRectangle";
        public const string FromSelectionRectPart = "FromSelectionRectangle";

        //Visual states
        public const string CommonStatesGroupName = "CommonStates";
        public const string EditStatesGroupName = "EditStates";
        public const string FocusStatesGroupName = "FocusStates";
        public const string MouseOverStateName = "MouseOver";
        public const string NormalStateName = "Normal";
        public const string EdittingStateName = "Editting";
        public const string NotEdittingStateName = "NotEditting";
        public const string DefaultStateName = "Default";
        public const string SelectedStateName = "Selected";
        public const string NotSelectedStateName = "NotSelected";

        private Path _arrowPath;
        private PathFigure _arrowFigure;
        private LineSegment _arrowBody;
        private PathGeometry _arrow;
        private Line _arrowHead;
        private Rectangle _toSelectionRect;
        private Rectangle _fromSelectionRect;

        private bool _isTemplateApplied;
        private bool _isSelected = false;
        private bool _isDraggingToAnchorPoint = false;
        private bool _isDraggingFromAnchorPoint = false;
        private bool _isEditting = false;

        public event RoutedEventHandler Selected;
        public event RoutedEventHandler Unselected;

        public event RoutedEventHandler DragFromAnchorEnded;
        public event RoutedEventHandler DragToAnchorEnded;

        public ArrowControl()
        {
            this._isTemplateApplied = false;

            this.DefaultStyleKey = typeof(ArrowControl);

            this.MouseEnter += new MouseEventHandler(ArrowControl_MouseEnter);
            this.MouseLeave += new MouseEventHandler(ArrowControl_MouseLeave);
        }

        private void ArrowControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!IsSelected)
            {
                GoToState(ArrowControl.NormalStateName, true);
            }
        }

        private void ArrowControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsSelected)
            {
                GoToState(ArrowControl.MouseOverStateName, true);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this._isTemplateApplied) return;

            _arrowPath = GetTemplateChild(ArrowControl.ArrowPathPart) as Path;
            _arrow = GetTemplateChild(ArrowControl.ArrowPart) as PathGeometry;
            _arrowFigure = GetTemplateChild(ArrowControl.ArrowFigurePart) as PathFigure;
            _arrowBody = GetTemplateChild(ArrowControl.ArrowBodyPart) as LineSegment;
            
            _arrowHead = GetTemplateChild(ArrowControl.ArrowHeadPart) as Line;

            _toSelectionRect = GetTemplateChild(ArrowControl.ToSelectionRectPart) as Rectangle;
            _toSelectionRect.MouseLeftButtonDown += new MouseButtonEventHandler(_toSelectionRect_MouseLeftButtonDown);
            _toSelectionRect.MouseLeftButtonUp += new MouseButtonEventHandler(_toSelectionRect_MouseLeftButtonUp);
            _fromSelectionRect = GetTemplateChild(ArrowControl.FromSelectionRectPart) as Rectangle;
            _fromSelectionRect.MouseLeftButtonDown += new MouseButtonEventHandler(_fromSelectionRect_MouseLeftButtonDown);
            _fromSelectionRect.MouseLeftButtonUp += new MouseButtonEventHandler(_fromSelectionRect_MouseLeftButtonUp);

            this.UpdateArrow();

            this._isTemplateApplied = true;
        }

        void _fromSelectionRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _fromSelectionRect.CaptureMouse();
            _isDraggingFromAnchorPoint = true;
            IsEditting = true;
            e.Handled = true;
        }

        void _fromSelectionRect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _fromSelectionRect.ReleaseMouseCapture();
            IsEditting = false;
            _isDraggingFromAnchorPoint = false;
            if (DragFromAnchorEnded != null)
            {
                DragFromAnchorEnded.Invoke(this, new RoutedEventArgs());
            }
        }

        void _toSelectionRect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _toSelectionRect.CaptureMouse();
            _isDraggingToAnchorPoint = true;
            IsEditting = true;
            e.Handled = true;
        }

        void _toSelectionRect_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _toSelectionRect.ReleaseMouseCapture();
            IsEditting = false;
            _isDraggingToAnchorPoint = false;
            if (DragToAnchorEnded != null)
            {
                DragToAnchorEnded.Invoke(this, new RoutedEventArgs());
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
                if (value)
                {
                    GoToState(ArrowControl.SelectedStateName, true);
                    if (Selected != null)
                    {
                        Selected.Invoke(this, new RoutedEventArgs());
                    }
                }
                else
                {
                    GoToState(ArrowControl.NotSelectedStateName, true);
                    if (Unselected != null)
                    {
                        Unselected.Invoke(this, new RoutedEventArgs());
                    }
                }
            }
        }

        public bool IsEditting
        {
            get
            {
                return _isEditting;
            }
            set
            {
                _isEditting = value;
            }
        }

        public ArrowDirection Direction
        {
            get;
            set;
        }

        public Point StartPoint
        {
            get;
            set;
        }

        public Point EndPoint
        {
            get;
            set;
        }

        public double Coeffiencient
        {
            get;
            set;
        }

        public double Angle
        {
            get;
            set;
        }

        public INodeRenderer To
        {
            get
            {
                INodeRenderer result = null;
                IRelationshipProxy relationship = DataContext as IRelationshipProxy;
                foreach (IDescriptorProxy descriptor in relationship.Descriptors.GetByDescriptorTypeName("To"))
                {
                    // TODO: Needs to be replaced with a proper ID check from the database through the use of the IoC.
                    if (descriptor.Node != null && ParentNavigatorView.NodeRenderers.ContainsKey(descriptor.NodeId))
                    {
                        result = ParentNavigatorView.NodeRenderers[descriptor.NodeId];
                        break;
                    }
                }

                return result;
            }
        }

        public INodeRenderer From
        {
            get
            {
                INodeRenderer result = null;
                IRelationshipProxy relationship = DataContext as IRelationshipProxy;
                foreach (IDescriptorProxy descriptor in relationship.Descriptors.GetByDescriptorTypeName("From"))
                {
                    // TODO: Needs to be replaced with a proper ID check from the database through the use of the IoC.
                    if (descriptor.Node != null && ParentNavigatorView.NodeRenderers.ContainsKey(descriptor.NodeId))
                    {
                        result = ParentNavigatorView.NodeRenderers[descriptor.NodeId];
                        break;
                    }
                }
              
                return result;
            }
        }

        /// <summary>
        /// The selection color/glow around the arrow and color of the arrow/line
        /// </summary>
        public static readonly DependencyProperty GlowColorProperty = DependencyProperty.Register("GlowColor", typeof(Color), typeof(ArrowControl), new PropertyMetadata(Colors.Red));
        public Color GlowColor
        {
            get { return (Color)GetValue(GlowColorProperty); }
            set { SetValue(GlowColorProperty, value); }
        }

        public static readonly DependencyProperty MouseOverColorProperty = DependencyProperty.Register("MouseOverColor", typeof(Color), typeof(ArrowControl), new PropertyMetadata(Colors.Yellow));
        public Color MouseOverColor
        {
            get { return (Color)GetValue(MouseOverColorProperty); }
            set { SetValue(MouseOverColorProperty, value); }
        }

        public static readonly DependencyProperty ParentNavigatorViewProperty = DependencyProperty.Register("ParentNavigatorView", typeof(NavigatorView), typeof(ArrowControl), null);
        public NavigatorView ParentNavigatorView
        {
            get { return (NavigatorView)GetValue(ParentNavigatorViewProperty); }
            set { SetValue(ParentNavigatorViewProperty, value); }
        }

        /// <summary>
        /// The distance between the arrow head and the node or the line end and the node
        /// </summary>
        public static readonly DependencyProperty ArrowPaddingProperty = DependencyProperty.Register("ArrowPadding", typeof(double), typeof(ArrowControl), new PropertyMetadata(10d));
        public double ArrowPadding
        {
            get { return (double)GetValue(ArrowPaddingProperty); }
            set { SetValue(ArrowPaddingProperty, value); }
        }

        private void GoToState(string stateName, bool useTransitions)
        {
            VisualStateManager.GoToState(this, stateName, useTransitions);
        }

        public void UpdateArrow()
        {
            UpdateArrow(default(Point));
        }

        public void UpdateArrow(Point movingPoint)
        {
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

                    nodeBCentreX += To.Location.X;
                    nodeBCentreY += To.Location.Y;

                    Point startPoint = movingPoint;
                    Point endPoint = movingPoint;
                    if (!_isDraggingFromAnchorPoint)
                    {
                        startPoint = new Point(nodeACentreX, nodeACentreY);
                    }
                    if (!_isDraggingToAnchorPoint)
                    {
                         endPoint = new Point(nodeBCentreX, nodeBCentreY);
                    }

                    Direction = CalculateArrowDirection(startPoint, endPoint);
                    double horizontalEdgeLength = 0;
                    double verticalEdgeLength = 0;
                    Angle = 0.0;
                    double a = 0, b = 0, c = 0, d = 0, e = 0, f = 0;
                    Coeffiencient = 0;
                    double startBuffer = (From.Skin.NodeSkinWidth / 2) + ArrowPadding;
                    double endBuffer = (To.Skin.NodeSkinWidth / 2) + ArrowPadding;

                    switch (Direction)
                    {
                        case ArrowDirection.North:
                            if (!IsEditting)
                            {
                                startPoint.Y = startPoint.Y - (From.Skin.NodeSkinHeight / 2) - ArrowPadding;
                                endPoint.Y = endPoint.Y + ((To.Skin.NodeSkinHeight / 2) + To.NodeControl.ActualTextHeight + ArrowPadding);
                            }
                            _arrowHead.X1 = endPoint.X;
                            _arrowHead.X2 = endPoint.X;
                            _arrowHead.Y1 = endPoint.Y + 1;
                            _arrowHead.Y2 = endPoint.Y;

                            _toSelectionRect.SetValue(Canvas.LeftProperty, endPoint.X - (_toSelectionRect.Width / 2));
                            _toSelectionRect.SetValue(Canvas.TopProperty, endPoint.Y);

                            _fromSelectionRect.SetValue(Canvas.LeftProperty, startPoint.X - (_fromSelectionRect.Width / 2));
                            _fromSelectionRect.SetValue(Canvas.TopProperty, startPoint.Y);
                            break;
                        case ArrowDirection.South:
                            if (!IsEditting)
                            {
                                startPoint.Y = startPoint.Y + ((From.Skin.NodeSkinHeight / 2) + From.NodeControl.ActualTextHeight + ArrowPadding);
                                endPoint.Y = endPoint.Y - (To.Skin.NodeSkinHeight / 2) - ArrowPadding;
                            }
                            _arrowHead.X1 = endPoint.X;
                            _arrowHead.X2 = endPoint.X;
                            _arrowHead.Y1 = endPoint.Y - 1;
                            _arrowHead.Y2 = endPoint.Y;

                            _toSelectionRect.SetValue(Canvas.LeftProperty, endPoint.X - (_toSelectionRect.Width / 2));
                            _toSelectionRect.SetValue(Canvas.TopProperty, endPoint.Y);

                            _fromSelectionRect.SetValue(Canvas.LeftProperty, startPoint.X - (_fromSelectionRect.Width / 2));
                            _fromSelectionRect.SetValue(Canvas.TopProperty, startPoint.Y);
                            break;
                        case ArrowDirection.West:
                            if (!IsEditting)
                            {
                                startPoint.X = startPoint.X - (To.Skin.NodeSkinWidth / 2) - ArrowPadding;
                                endPoint.X = endPoint.X + (From.Skin.NodeSkinWidth / 2) + ArrowPadding;
                            }
                            _arrowHead.X1 = endPoint.X + 1;
                            _arrowHead.X2 = endPoint.X;
                            _arrowHead.Y1 = endPoint.Y;
                            _arrowHead.Y2 = endPoint.Y;

                            _toSelectionRect.SetValue(Canvas.LeftProperty, endPoint.X);
                            _toSelectionRect.SetValue(Canvas.TopProperty, endPoint.Y - (_toSelectionRect.Height / 2));

                            _fromSelectionRect.SetValue(Canvas.LeftProperty, startPoint.X);
                            _fromSelectionRect.SetValue(Canvas.TopProperty, startPoint.Y - (_fromSelectionRect.Height / 2));
                            break;
                        case ArrowDirection.East:
                            if (!IsEditting)
                            {
                                startPoint.X = startPoint.X + (To.Skin.NodeSkinWidth / 2) + ArrowPadding;
                                endPoint.X = endPoint.X - (From.Skin.NodeSkinWidth / 2) - ArrowPadding;
                            }
                            _arrowHead.X1 = endPoint.X - 1;
                            _arrowHead.X2 = endPoint.X;
                            _arrowHead.Y1 = endPoint.Y;
                            _arrowHead.Y2 = endPoint.Y;

                            _toSelectionRect.SetValue(Canvas.LeftProperty, endPoint.X);
                            _toSelectionRect.SetValue(Canvas.TopProperty, endPoint.Y - (_toSelectionRect.Height / 2));

                            _fromSelectionRect.SetValue(Canvas.LeftProperty, startPoint.X);
                            _fromSelectionRect.SetValue(Canvas.TopProperty, startPoint.Y - (_fromSelectionRect.Height / 2));
                            break;
                        case ArrowDirection.NorthWest:
                            horizontalEdgeLength = Math.Abs(startPoint.X - endPoint.X);
                            verticalEdgeLength = Math.Abs(startPoint.Y - endPoint.Y);
                            Angle = Math.Atan(verticalEdgeLength / horizontalEdgeLength);

                            if (!_isDraggingToAnchorPoint)
                            {
                                if (Angle > 1.25)
                                {
                                    endBuffer = To.NodeControl.ActualTextHeight + (To.Skin.NodeSkinHeight / 2) + ArrowPadding;
                                }
                                else if (Angle < 1.25 && Angle > 0.85)
                                {
                                    endBuffer = ((To.NodeControl.ActualTextHeight + To.NodeControl.ActualTextWidth) / 2) + (ArrowPadding * 3);
                                }
                                else if (Angle < 0.85 && Angle > 0.6)
                                {
                                    endBuffer = (((To.NodeControl.ActualTextHeight * 0.6) + To.NodeControl.ActualTextWidth) / 2) + (ArrowPadding * 3);
                                }
                                else if (Angle < 0.6 && Angle > 0.18)
                                {
                                    endBuffer = (To.NodeControl.ActualTextWidth / 2) + (ArrowPadding * 3);
                                }
                                else if (Angle < 0.18)
                                {
                                    endBuffer = (To.Skin.NodeSkinWidth / 2) + ArrowPadding;
                                }
                            }

                            a = Math.Abs(Math.Sin(Angle) * endBuffer);
                            Coeffiencient = (startPoint.X - endPoint.X) / (startPoint.Y - endPoint.Y);
                            b = a * Coeffiencient;
                            if (!_isDraggingToAnchorPoint)
                            {
                                endPoint.X = endPoint.X + b;
                                endPoint.Y = endPoint.Y + a;
                            }

                            c = Math.Abs(Math.Sin(Angle) * startBuffer);
                            d = c * Coeffiencient;
                            if (!_isDraggingFromAnchorPoint)
                            {
                                startPoint.X = startPoint.X - d;
                                startPoint.Y = startPoint.Y - c;
                            }

                            e = Math.Abs(Math.Cos(Angle) * 1);
                            f = Math.Abs(Math.Sin(Angle) * 1);
                            _arrowHead.X1 = endPoint.X + e;
                            _arrowHead.X2 = endPoint.X;
                            _arrowHead.Y1 = endPoint.Y + f;
                            _arrowHead.Y2 = endPoint.Y;

                            _toSelectionRect.SetValue(Canvas.LeftProperty, endPoint.X - (_toSelectionRect.Width / 2));
                            _toSelectionRect.SetValue(Canvas.TopProperty, endPoint.Y - (_toSelectionRect.Height / 2));

                            _fromSelectionRect.SetValue(Canvas.LeftProperty, startPoint.X - (_fromSelectionRect.Width / 2));
                            _fromSelectionRect.SetValue(Canvas.TopProperty, startPoint.Y - (_fromSelectionRect.Height / 2));
                            break;
                        case ArrowDirection.NorthEast:
                            horizontalEdgeLength = Math.Abs(startPoint.X - endPoint.X);
                            verticalEdgeLength = Math.Abs(startPoint.Y - endPoint.Y);
                            Angle = Math.Atan(verticalEdgeLength / horizontalEdgeLength);

                            if (!_isDraggingToAnchorPoint)
                            {
                                if (Angle > 1.25)
                                {
                                    endBuffer = To.NodeControl.ActualTextHeight + (To.Skin.NodeSkinHeight / 2) + ArrowPadding;
                                }
                                else if (Angle < 1.25 && Angle > 0.85)
                                {
                                    endBuffer = ((To.NodeControl.ActualTextHeight + To.NodeControl.ActualTextWidth) / 2) + (ArrowPadding * 3);
                                }
                                else if (Angle < 0.85 && Angle > 0.6)
                                {
                                    endBuffer = (((To.NodeControl.ActualTextHeight * 0.6) + To.NodeControl.ActualTextWidth) / 2) + (ArrowPadding * 3);
                                }
                                else if (Angle < 0.6 && Angle > 0.18)
                                {
                                    endBuffer = (To.NodeControl.ActualTextWidth / 2) + (ArrowPadding * 3);
                                }
                                else if (Angle < 0.18)
                                {
                                    endBuffer = (To.Skin.NodeSkinWidth / 2) + ArrowPadding;
                                }
                            }

                            a = Math.Abs(Math.Sin(Angle) * endBuffer);
                            Coeffiencient = (endPoint.X - startPoint.X) / (startPoint.Y - endPoint.Y);
                            b = a * Coeffiencient;
                            if (!_isDraggingToAnchorPoint)
                            {
                                endPoint.X = endPoint.X - b;
                                endPoint.Y = endPoint.Y + a;
                            }

                            c = Math.Abs(Math.Sin(Angle) * startBuffer);
                            d = c * Coeffiencient;
                            if (!_isDraggingFromAnchorPoint)
                            {
                                startPoint.X = startPoint.X + d;
                                startPoint.Y = startPoint.Y - c;
                            }

                            e = Math.Abs(Math.Cos(Angle) * 1);
                            f = Math.Abs(Math.Sin(Angle) * 1);
                            _arrowHead.X1 = endPoint.X - e;
                            _arrowHead.X2 = endPoint.X;
                            _arrowHead.Y1 = endPoint.Y + f;
                            _arrowHead.Y2 = endPoint.Y;

                            _toSelectionRect.SetValue(Canvas.LeftProperty, endPoint.X - (_toSelectionRect.Width / 2));
                            _toSelectionRect.SetValue(Canvas.TopProperty, endPoint.Y - (_toSelectionRect.Height / 2));

                            _fromSelectionRect.SetValue(Canvas.LeftProperty, startPoint.X - (_fromSelectionRect.Width / 2));
                            _fromSelectionRect.SetValue(Canvas.TopProperty, startPoint.Y - (_fromSelectionRect.Height / 2));
                            break;
                        case ArrowDirection.SouthWest:
                            horizontalEdgeLength = Math.Abs(startPoint.X - endPoint.X);
                            verticalEdgeLength = Math.Abs(startPoint.Y - endPoint.Y);
                            Angle = Math.Atan(verticalEdgeLength / horizontalEdgeLength);

                            if (!_isDraggingFromAnchorPoint)
                            {
                                if (Angle > 1.25)
                                {
                                    startBuffer = From.NodeControl.ActualTextHeight + (From.Skin.NodeSkinHeight / 2) + ArrowPadding;
                                }
                                else if (Angle < 1.25 && Angle > 0.85)
                                {
                                    startBuffer = ((From.NodeControl.ActualTextHeight + From.NodeControl.ActualTextWidth) / 2) + (ArrowPadding * 3);
                                }
                                else if (Angle < 0.85 && Angle > 0.6)
                                {
                                    startBuffer = (((From.NodeControl.ActualTextHeight*0.6) + From.NodeControl.ActualTextWidth) / 2) + ArrowPadding;
                                }
                                else if (Angle < 0.6 && Angle > 0.18)
                                {
                                    startBuffer = (From.NodeControl.ActualTextWidth / 2) + (ArrowPadding * 3);
                                }
                                else if (Angle < 0.18)
                                {
                                    startBuffer = (From.Skin.NodeSkinWidth / 2) + ArrowPadding;
                                }
                            }

                            a = Math.Abs(Math.Sin(Angle) * endBuffer);
                            Coeffiencient = (startPoint.X - endPoint.X) / (endPoint.Y - startPoint.Y);
                            b = a * Coeffiencient;
                            if (!_isDraggingToAnchorPoint)
                            {
                                endPoint.X = endPoint.X + b;
                                endPoint.Y = endPoint.Y - a;
                            }

                            c = Math.Abs(Math.Sin(Angle) * startBuffer);
                            d = c * Coeffiencient;
                            if (!_isDraggingFromAnchorPoint)
                            {
                                startPoint.X = startPoint.X - d;
                                startPoint.Y = startPoint.Y + c;
                            }

                            e = Math.Abs(Math.Cos(Angle) * 1);
                            f = Math.Abs(Math.Sin(Angle) * 1);
                            _arrowHead.X1 = endPoint.X + e;
                            _arrowHead.X2 = endPoint.X;
                            _arrowHead.Y1 = endPoint.Y - f;
                            _arrowHead.Y2 = endPoint.Y;

                            _toSelectionRect.SetValue(Canvas.LeftProperty, endPoint.X - (_toSelectionRect.Width / 2));
                            _toSelectionRect.SetValue(Canvas.TopProperty, endPoint.Y - (_toSelectionRect.Height / 2));

                            _fromSelectionRect.SetValue(Canvas.LeftProperty, startPoint.X - (_fromSelectionRect.Width / 2));
                            _fromSelectionRect.SetValue(Canvas.TopProperty, startPoint.Y - (_fromSelectionRect.Height / 2));
                            break;
                        case ArrowDirection.SouthEast:
                            horizontalEdgeLength = Math.Abs(startPoint.X - endPoint.X);
                            verticalEdgeLength = Math.Abs(startPoint.Y - endPoint.Y);
                            Angle = Math.Atan(verticalEdgeLength / horizontalEdgeLength);

                            if (!_isDraggingFromAnchorPoint)
                            {
                                if (Angle > 1.25)
                                {
                                    startBuffer = From.NodeControl.ActualTextHeight + (From.Skin.NodeSkinHeight / 2) + ArrowPadding;
                                }
                                else if (Angle < 1.25 && Angle > 0.85)
                                {
                                    startBuffer = ((From.NodeControl.ActualTextHeight + From.NodeControl.ActualTextWidth) / 2) + (ArrowPadding * 3);
                                }
                                else if (Angle < 0.85 && Angle > 0.6)
                                {
                                    startBuffer = (((From.NodeControl.ActualTextHeight * 0.6) + From.NodeControl.ActualTextWidth) / 2) + ArrowPadding;
                                }
                                else if (Angle < 0.6 && Angle > 0.18)
                                {
                                    startBuffer = (From.NodeControl.ActualTextWidth / 2) + (ArrowPadding * 3);
                                }
                                else if (Angle < 0.18)
                                {
                                    startBuffer = (From.Skin.NodeSkinWidth / 2) + ArrowPadding;
                                }
                            }

                            a = Math.Abs(Math.Sin(Angle) * endBuffer);
                            Coeffiencient = (endPoint.X - startPoint.X) / (endPoint.Y - startPoint.Y);
                            b = a * Coeffiencient;
                            if (!_isDraggingToAnchorPoint)
                            {
                                endPoint.X = endPoint.X - b;
                                endPoint.Y = endPoint.Y - a;
                            }

                            c = Math.Abs(Math.Sin(Angle) * startBuffer);
                            d = c * Coeffiencient;
                            if (!_isDraggingFromAnchorPoint)
                            {
                                startPoint.X = startPoint.X + d;
                                startPoint.Y = startPoint.Y + c;
                            }

                            e = Math.Abs(Math.Cos(Angle) * 1);
                            f = Math.Abs(Math.Sin(Angle) * 1);
                            _arrowHead.X1 = endPoint.X - e;
                            _arrowHead.X2 = endPoint.X;
                            _arrowHead.Y1 = endPoint.Y - f;
                            _arrowHead.Y2 = endPoint.Y;

                            _toSelectionRect.SetValue(Canvas.LeftProperty, endPoint.X - (_toSelectionRect.Width / 2));
                            _toSelectionRect.SetValue(Canvas.TopProperty, endPoint.Y - (_toSelectionRect.Height / 2));

                            _fromSelectionRect.SetValue(Canvas.LeftProperty, startPoint.X - (_fromSelectionRect.Width / 2));
                            _fromSelectionRect.SetValue(Canvas.TopProperty, startPoint.Y - (_fromSelectionRect.Height / 2));
                            break;
                        default:
                            break;
                    }

                    _arrowBody.Point = endPoint;
                    _arrowFigure.StartPoint = startPoint;
                    StartPoint = startPoint;
                    EndPoint = endPoint;
                    //Debug.WriteLine("Angle: {0:0.0000}", Angle);
                    //Debug.WriteLine("StartBuffer: {0:0.0000}\n", startBuffer);
                }
            }
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
