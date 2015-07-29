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
using SilverlightMappingToolBasic.MappingService;
using System.Linq;
using System.Threading;
using SilverlightMappingToolBasic.Controls;
using System.Windows.Messaging;

namespace SilverlightMappingToolBasic.MapDepth
{
    public class MouseKeyboardEvents
    {
        public CompendiumMapDepthMap caller;
        public ScaleTransform scaleTransform;
        public TranslateTransform translateTransform;
        public MapDepthViewManager ViewManager;
        private TypeManager _typeManager;
        private MapDepthNavigator _navigator;
        private DatabaseMappingService _nodeService;

        public MouseKeyboardEvents(CompendiumMapDepthMap _caller, MapDepthViewManager viewManager, TypeManager typeManager, MapDepthNavigator navigator, DatabaseMappingService nodeService, ScaleTransform _scaleTransform, TranslateTransform _translateTransform)
        {
            caller = _caller;
            ViewManager = viewManager;
            _typeManager = typeManager;
            _navigator = navigator;
            _nodeService = nodeService;
            scaleTransform = _scaleTransform;
            translateTransform = _translateTransform;
        }

        private Point _currentMousePosition;
        private bool _isRightMouseButtonDown;
        private Line _tempRelationshipLine;
        private bool _mouseMovedWhileRMBDown;
        private bool _isLeftMouseButtonDown;
        private Rectangle _selectionRectangle;
        private Point _selectionStartPoint;
        private Point _startLinePosition;
        private Point _originalPanPosition;

        public INodeNavigator Navigator
        {
            get
            {
                return _navigator;
            }
        }

        public void CompendiumMapDepthMap_MouseMove(object sender, MouseEventArgs e)
        {
            _currentMousePosition = scaleTransform.Inverse.Transform(translateTransform.Inverse.Transform(e.GetPosition(caller)));
            object fe = FocusManager.GetFocusedElement();
            if (!(fe is TextBox))
            {
                caller.Focus();
            }
            if (_isRightMouseButtonDown && _tempRelationshipLine != null)
            {
                _tempRelationshipLine.X2 = _currentMousePosition.X;
                _tempRelationshipLine.Y2 = _currentMousePosition.Y;
            }
            if (_isRightMouseButtonDown)
            {
                _mouseMovedWhileRMBDown = true;
                //double xMovement = _originalPanPosition.X - _currentMousePosition.X;
                //double yMovement = _originalPanPosition.Y - _currentMousePosition.Y;
                //translateTransform.Y += yMovement;
                //translateTransform.X += xMovement;
                //_originalPanPosition = _currentMousePosition;
            }
            if (_isLeftMouseButtonDown && _selectionRectangle != null)
            {
                _selectionRectangle.Width = Math.Abs(_selectionStartPoint.X - _currentMousePosition.X);
                _selectionRectangle.Height = Math.Abs(_selectionStartPoint.Y - _currentMousePosition.Y);
                if (_selectionStartPoint.X > _currentMousePosition.X)
                {
                    _selectionRectangle.SetValue(Canvas.LeftProperty, _currentMousePosition.X);
                }
                if (_selectionStartPoint.Y > _currentMousePosition.Y)
                {
                    _selectionRectangle.SetValue(Canvas.TopProperty, _currentMousePosition.Y);
                }
            }
            if (ViewManager != null && ViewManager.Relationships != null)
            {
                foreach (IRelationshipRenderer relationshipRenderer in ViewManager.Relationships)
                {
                    if (relationshipRenderer.IsEditting)
                    {
                        relationshipRenderer.UpdateRelationship(_currentMousePosition);
                    }
                }
            }
        }

        public void CompendiumMapDepthMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isLeftMouseButtonDown = true;
            _selectionStartPoint = translateTransform.Inverse.Transform(e.GetPosition(caller));
            _selectionStartPoint = scaleTransform.Inverse.Transform(_selectionStartPoint);
            if (_selectionRectangle != null)
            {
                caller.uxMapSurface.Children.Remove(_selectionRectangle);
                _selectionRectangle = null;
            }
            if (e.ClickCount != 2)
            {
                _selectionRectangle = new Rectangle();
                _selectionRectangle.Stroke = new SolidColorBrush(Colors.Black);
                _selectionRectangle.StrokeThickness = 1.0;
                _selectionRectangle.Opacity = 0.7;
                DoubleCollection dashArray = new DoubleCollection();
                dashArray.Add(2);
                dashArray.Add(4);
                _selectionRectangle.StrokeDashArray = dashArray;
                _selectionRectangle.SetValue(Canvas.LeftProperty, _selectionStartPoint.X);
                _selectionRectangle.SetValue(Canvas.TopProperty, _selectionStartPoint.Y);
                caller.uxMapSurface.Children.Add(_selectionRectangle);
            }

            ViewManager.CommitNodeName();
        }

        public void CompendiumMapDepthMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isLeftMouseButtonDown = false;
            INodeRenderer nr = ViewManager.GetNodeRenderer(_currentMousePosition);
            NodeRelationshipHelper nrh = IoC.IoCContainer.GetInjectionInstance().GetInstance<NodeRelationshipHelper>();

            if (nrh != null)
            {
                if (nrh.IsEditting)
                {
                    if (nr != null)
                    {
                        IRelationshipProxy originalRelationship = nrh.Relationship.DataContext as IRelationshipProxy;
                        _nodeService.DeleteRelationship(Navigator.DomainId, originalRelationship.Id);

                        IDescriptorTypeProxy toDescriptorTypeProxy = _typeManager.GetDescriptorType("To");
                        IDescriptorTypeProxy fromDescriptorTypeProxy = _typeManager.GetDescriptorType("From");
                        IDescriptorTypeProxy transMapDescriptorTypeProxy = _typeManager.GetDescriptorType("TransclusionMap");

                        Dictionary<IDescriptorTypeProxy, Guid> nodes = new Dictionary<IDescriptorTypeProxy, Guid>();
                        INodeProxy fromNode = null, toNode = null;

                        switch (nrh.EdittingSide)
                        {
                            case RelationshipSide.From:
                                fromNode = nr.Node;
                                toNode = originalRelationship.Descriptors.GetByDescriptorTypeName("To").First().Node;

                                break;
                            case RelationshipSide.To:
                                fromNode = originalRelationship.Descriptors.GetByDescriptorTypeName("From").First().Node;
                                toNode = nr.Node;

                                break;
                            default:
                                break;
                        }

                        nodes.Add(toDescriptorTypeProxy, toNode.Id);
                        nodes.Add(fromDescriptorTypeProxy, fromNode.Id);
                        IRelationshipTypeProxy relationshipTypeProxy = null;

                        if (fromNode.ParentMapNodeUid != this.Navigator.FocalNodeId || toNode.ParentMapNodeUid != this.Navigator.FocalNodeId)
                        {
                            nodes.Add(transMapDescriptorTypeProxy, this.Navigator.FocalNodeId);
                            relationshipTypeProxy = _typeManager.GetRelationshipType("TransclusionRelationship");
                        }
                        else
                        {
                            relationshipTypeProxy = _typeManager.GetRelationshipType("FromToRelationship");
                        }

                        _navigator.ConnectNodesAsync(nodes, relationshipTypeProxy, originalRelationship.Id.ToString());
                        _navigator.GetCurrentNodesAsync();
                    }
                    else
                    {
                        nrh.Relationship.UpdateArrow();
                    }
                }
                nrh.IsEditting = false;
            }

            if (_selectionRectangle != null)
            {
                caller.uxMapSurface.Children.Remove(_selectionRectangle);
                double topLeftX = (double)_selectionRectangle.GetValue(Canvas.LeftProperty);
                double topLeftY = (double)_selectionRectangle.GetValue(Canvas.TopProperty);
                double bottomRightX = _selectionRectangle.Width + topLeftX;
                double bottomRightY = _selectionRectangle.Height + topLeftY;
                _selectionRectangle = null;
                if (!(Double.IsNaN(bottomRightX) && Double.IsNaN(bottomRightY)))
                {
                    ViewManager.SelectAllWithinBounds(new Point(topLeftX, topLeftY), new Point(bottomRightX, bottomRightY));
                }
                else
                {
                    ViewManager.UnselectAllNodes();
                    ViewManager.UnselectAllRelationships();
                }
            }
        }

        public void CompendiumMapDepthMap_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _currentMousePosition = e.GetPosition(caller);
            _isRightMouseButtonDown = true;
            _mouseMovedWhileRMBDown = false;
            NodeRelationshipHelper nrh = IoC.IoCContainer.GetInjectionInstance().GetInstance<NodeRelationshipHelper>();
            if (nrh.FromNode != null)
            {
                _startLinePosition = nrh.GetCenterOfFromNode();
                _tempRelationshipLine = new Line();
                _tempRelationshipLine.StrokeThickness = 1.25;
                _tempRelationshipLine.Stroke = new SolidColorBrush(Colors.Black);
                _tempRelationshipLine.Opacity = 0.40;
                _tempRelationshipLine.X1 = _startLinePosition.X;
                _tempRelationshipLine.Y1 = _startLinePosition.Y;
                _tempRelationshipLine.X2 = _startLinePosition.X;
                _tempRelationshipLine.Y2 = _startLinePosition.Y;
                _tempRelationshipLine.SetValue(Canvas.ZIndexProperty, -1);
                caller.uxMapSurface.Children.Add(_tempRelationshipLine);

                Thread rightClickDelayThread = new Thread(this.ShowNodeContextMenu);
                rightClickDelayThread.Start(nrh.FromNode.DataContext as INodeProxy);
            }
            else if (nrh.Relationship != null)
            {
                Thread rightCLickDelayThread = new Thread(this.ShowRelationshipContextMenu);
                rightCLickDelayThread.Start(nrh.Relationship.DataContext as IRelationshipProxy);
            }
            else
            {
                _originalPanPosition = _currentMousePosition;
                Thread rightClickDelayThread = new Thread(this.ShowCanvasContextMenu);
                rightClickDelayThread.Start();
            }
            e.Handled = true;
        }

        private void ShowNodeContextMenu(object nodeProxy)
        {
            Thread.Sleep(300);

            // put it on the UI thread to execute.
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (!_mouseMovedWhileRMBDown)
                {
                    NodeRelationshipHelper nrh = IoC.IoCContainer.GetInjectionInstance().GetInstance<NodeRelationshipHelper>();
                    if (nrh.FromNode != null)
                    {
                        nrh.FromNode = null;
                    }
                    if (caller.uxMapSurface.Children.Contains(_tempRelationshipLine))
                    {
                        caller.uxMapSurface.Children.Remove(_tempRelationshipLine);
                    }
                    NodeContextMenu contextMenu = new NodeContextMenu(nodeProxy as INodeProxy, caller, _nodeService, _currentMousePosition);
                    ContextMenuService.SetContextMenu(caller, contextMenu);

                    contextMenu.IsOpen = true;
                    contextMenu.HorizontalOffset = _currentMousePosition.X;
                    contextMenu.VerticalOffset = _currentMousePosition.Y + 30;
                }
            });
        }

        private void ShowRelationshipContextMenu(object relationshipProxy)
        {
            Thread.Sleep(300);

            // put it on the UI thread to execute.
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (!_mouseMovedWhileRMBDown)
                {
                    RelationshipContextMenu contextMenu = new RelationshipContextMenu(relationshipProxy as IRelationshipProxy, caller, _nodeService);
                    ContextMenuService.SetContextMenu(caller, contextMenu);

                    contextMenu.IsOpen = true;
                    contextMenu.HorizontalOffset = _currentMousePosition.X;
                    contextMenu.VerticalOffset = _currentMousePosition.Y + 30;
                }
            });
        }

        private void ShowCanvasContextMenu(object state)
        {
            Thread.Sleep(300);

            // put it on the UI thread to execute.
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (!_mouseMovedWhileRMBDown)
                {
                    CanvasContextMenu contextMenu = new CanvasContextMenu(_typeManager, caller.MessageSender, Navigator, _currentMousePosition);
                    ContextMenuService.SetContextMenu(caller, contextMenu);

                    contextMenu.IsOpen = true;
                    contextMenu.HorizontalOffset = _currentMousePosition.X;
                    contextMenu.VerticalOffset = _currentMousePosition.Y + 30;
                }
            });
        }

        public void CompendiumMapDepthMap_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isRightMouseButtonDown = false;
            _mouseMovedWhileRMBDown = false;
            NodeRelationshipHelper nrh = IoC.IoCContainer.GetInjectionInstance().GetInstance<NodeRelationshipHelper>();
            if (nrh.ToNode == null)
            {
                nrh.FromNode = null;
            }
            if (caller.uxMapSurface.Children.Contains(_tempRelationshipLine))
            {
                caller.uxMapSurface.Children.Remove(_tempRelationshipLine);
            }
        }
    }
}
