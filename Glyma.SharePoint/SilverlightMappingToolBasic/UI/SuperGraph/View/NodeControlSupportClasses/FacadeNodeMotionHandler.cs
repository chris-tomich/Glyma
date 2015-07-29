using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MouseCursor;
using SimpleIoC;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow;
using SilverlightMappingToolBasic.UI.SuperGraph.View.CollapseSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Selector;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public class FacadeNodeMotionHandler : INodeMotionHandler, IDisposable
    {
        private readonly ISelectorControl _parent;
        private readonly INodesSelector _selector;
        private INodeMotionHandler _singleNodeMotionHandler;
        private INodeMotionHandler MultipleNodeMotionHandler
        {
            get
            {
                return _selector.MultipleNodeMotionHandler;
            }
        }

        public FacadeNodeMotionHandler(NodeControl userControl)
        {
            _parent = userControl.ParentSurface;
            _parent.MapMoved += OnMapMoved;
            UserControl = userControl;
            MouseManagingControl = userControl.NodeImage;
            _selector = _parent.NodesSelector;
        }

        public void OnMapMoved(object sender, MoveTransformEventArgs e)
        {
            NodeMotionHandler.OnMapMoved(sender, e);
        }

        public Canvas ParentSurface {
            get
            {
                return _parent.NodeContainer;
            }
        }
        public UIElement MouseManagingControl { get; private set; }
        public event EventHandler<NodeMotionHandlerEventArgs> NodeInMotionEvent;

        public INodeMotionHandler NodeMotionHandler
        {
            get { return _singleNodeMotionHandler ?? (_singleNodeMotionHandler = new NodeMotionHandler(UserControl)); }
        }

        public INodeMotionHandler MotionHandler
        {
            get { return _selector.IsMultiSelect ? MultipleNodeMotionHandler : NodeMotionHandler; }
        }

        

        public NodeControl UserControl
        {
            get;
            private set;
        }

        

        public void OnLeftButtonDragMove(object sender, MouseEventArgs e)
        {
            if (IoCContainer.GetInjectionInstance().GetInstance<ArrowCreationManager>().InMotion)
            {
                MouseManagingControl.ReleaseMouseCapture();
                var args = new MouseManagerEventArgs
                {
                    Start = new MouseStateProperties { EventOrigin = sender, PositionInformation = e },
                    End = new MouseStateProperties { EventOrigin = sender, PositionInformation = e }
                };
                MotionHandler.OnLeftButtonDragReleased(sender, args);
                return;
            }
            MotionHandler.OnLeftButtonDragMove(sender,e);
            
        }

        public void OnLeftButtonDragReleased(object sender, MouseManagerEventArgs e)
        {
            MouseManagingControl.ReleaseMouseCapture();
            //SuperCursor.SetCursorTemplate(MouseManagingControl, null);
            MotionHandler.OnLeftButtonDragReleased(sender, e);
        }

        public void OnLeftButtonDragStart(object sender, MouseManagerEventArgs e)
        {
            if (IoCContainer.GetInjectionInstance().GetInstance<ArrowCreationManager>().InMotion)
            {
                return;
            }

            MouseManagingControl.CaptureMouse();
            //SuperCursor.SetCursorTemplate(MouseManagingControl, ((DataTemplate)(Application.Current.Resources["Move"])));
            if (!_selector.HasNode(UserControl))
            {
                _selector.Clear();
                _selector.AddNode(UserControl);
            }


            var nodeToLoop = _selector.NodeControls.ToList();
            foreach (var nodeControl in nodeToLoop)
            {
                if (nodeControl.CollapseState == CollapseState.Collapsed)
                {
                    foreach (var child in nodeControl.GetAllChildNodeControls())
                    {
                        _selector.AddNode(child);
                    }
                }
                else if (nodeControl.CollapseState == CollapseState.SemiCollapsed)
                {
                    var invisivleChildNodes = nodeControl.GetAllChildNodeControls().Where(q => q.IsCollapsed);
                    foreach (var child in invisivleChildNodes)
                    {
                        _selector.AddNode(child);
                    }
                }
            }
            
            
            MotionHandler.OnLeftButtonDragStart(sender, e);
        }

        public void OnExplorerLeftButtonDragStart(object sender, MouseManagerEventArgs e)
        {
            MouseManagingControl.CaptureMouse();
            UserControl.ParentSurface.DragStart(e);
        }

        public void OnExplorerLeftButtonDragReleased(object sender, MouseManagerEventArgs e)
        {
            MouseManagingControl.ReleaseMouseCapture();
            UserControl.ParentSurface.DragRelease();
        }

        public void OnExplorerLeftButtonDragMove(object sender, MouseEventArgs e)
        {
            UserControl.ParentSurface.Explorer_OnLeftButtonDragMove(this, e);
        }

        public void OnExplorerTextLeftButtonDragStart(object sender, MouseManagerEventArgs e)
        {
            UserControl.NodeText.CaptureMouse();
            UserControl.ParentSurface.DragStart(e);
        }

        public void OnExplorerTextLeftButtonDragReleased(object sender, MouseManagerEventArgs e)
        {
            UserControl.NodeText.ReleaseMouseCapture();
            UserControl.ParentSurface.DragRelease();
        }

        public void Dispose()
        {
            if (_parent != null)
            {
                _parent.MapMoved -= OnMapMoved;
            }
        }
    }
}
