using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Selector;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public class NodeMotionHandler : INodeMotionHandler
    {
        private Point _previousMouseLocation;
        private Point _currentMouseLocation;
        private bool _isDrag;

        public event EventHandler<NodeMotionHandlerEventArgs> NodeInMotionEvent;

        public NodeMotionHandler(NodeControl userControl)
        {
            ParentSurface = userControl.ParentSurface.NodeContainer;
            UserControl = userControl;
            MouseManagingControl = userControl.NodeImage;
        }

        

        public Canvas ParentSurface
        {
            get;
            private set;
        }

        public NodeControl UserControl
        {
            get;
            private set;
        }

        public UIElement MouseManagingControl
        {
            get;
            private set;
        }

        public void OnLeftButtonDragStart(object sender, MouseManagerEventArgs e)
        {
            _isDrag = true;
            _previousMouseLocation = e.Start.PositionInformation.GetPosition(ParentSurface);
            
        }

        public void OnMapMoved(object sender, MoveTransformEventArgs e)
        {
            if (_isDrag)
            {
                _previousMouseLocation.X += e.X;
                _previousMouseLocation.Y += e.Y;
                var offset = _previousMouseLocation.Subtract(_currentMouseLocation);
                _previousMouseLocation = _currentMouseLocation;
                UserControl.Centre = UserControl.Centre.Subtract(offset);
                if (NodeInMotionEvent != null)
                {
                    NodeInMotionEvent(this, new NodeMotionHandlerEventArgs { Location = _currentMouseLocation });
                }
            }
        }

        public void OnLeftButtonDragReleased(object sender, MouseManagerEventArgs e)
        {
            _isDrag = false;

            UserControl.ViewModelNode.Location = UserControl.Centre;

            TransactionalMouseManagerEventArgs transactionalMouseManagerEventArgs = e as TransactionalMouseManagerEventArgs;

            if (transactionalMouseManagerEventArgs == null)
            {
                UserControl.ViewModelNode.CommitLocation();
            }
            else
            {
                transactionalMouseManagerEventArgs.NodesToBeCommitted.Add(UserControl.ViewModelNode);
            }
        }

        public void OnLeftButtonDragMove(object sender, MouseEventArgs e)
        {
            if (_isDrag)
            {
                _currentMouseLocation = e.GetPosition(ParentSurface);
                Point offset = _previousMouseLocation.Subtract(_currentMouseLocation);
                _previousMouseLocation = _currentMouseLocation;
                UserControl.Centre = UserControl.Centre.Subtract(offset);
                if (NodeInMotionEvent != null)
                {
                    NodeInMotionEvent(this, new NodeMotionHandlerEventArgs { Location = _currentMouseLocation });
                }
            }
        }
    }
}
