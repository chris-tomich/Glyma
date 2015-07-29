using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using SimpleIoC;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.CollapseSupportClasses
{
    public partial class CollapseControl : UserControl
    {
        public NodeControl NodeControl { get; set; }
        private MouseManager _mouseManager;

        public MouseManager MouseManager
        {
            get
            {
                if (_mouseManager == null)
                {
                    _mouseManager = new MouseManager(this);
                }
                return _mouseManager;
            }
        }

        public Node ViewModelNode
        {
            get
            {
                return DataContext as Node;
            }
        }

        public CollapseState State
        {
            get
            {
                return ViewModelNode.State;
            }
            set
            {
                ViewModelNode.State = value;
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
        }

        public CollapseControl()
        {
            InitializeComponent();
            MouseManager.LeftButtonDoubleClick += MouseManagerOnLeftButtonDoubleClick;
            MouseManager.LeftButtonSingleClick += MouseManagerOnLeftButtonSingleClick;
            MouseManager.RightButtonDragReleased += MouseManagerOnRightButtonDragReleased;
        }

        private void MouseManagerOnRightButtonDragReleased(object sender, MouseManagerEventArgs e)
        {
            if (IoCContainer.GetInjectionInstance().GetInstance<ArrowCreationManager>().InMotion)
            {
                IoCContainer.GetInjectionInstance().GetInstance<ArrowCreationManager>().SetTo(NodeControl);
            }
        }

        private void MouseManagerOnLeftButtonSingleClick(object sender, MouseManagerEventArgs mouseManagerEventArgs)
        {
            switch (State)
            {
                case CollapseState.Collapsed:
                    ExpandNodes(new ChildrenControlCollection(NodeControl, false, true));
                    break;
                case CollapseState.Expanded:
                    CollapseNodes(new ChildrenControlCollection(NodeControl));
                    break;
                case CollapseState.SemiCollapsed:
                    ExpandNodes(new ChildrenControlCollection(NodeControl, false, true));
                    break;
            }
            NodeControl.ParentSurface.Selector.Clear();
            NodeControl.ParentSurface.Selector.AddNode(NodeControl);
        }

        private void MouseManagerOnLeftButtonDoubleClick(object sender, MouseManagerEventArgs mouseManagerEventArgs)
        {
            var collection = new ChildrenControlCollection(NodeControl);
            if (collection.NodeControls.Any(q => q.IsCollapsed))
            {
                ExpandNodes(new ChildrenControlCollection(NodeControl));
            }
            else
            {
                CollapseNodes(new ChildrenControlCollection(NodeControl));
            }
            NodeControl.ParentSurface.Selector.Clear();
            NodeControl.ParentSurface.Selector.AddNode(NodeControl);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void ExpandNodes(ChildrenControlCollection collection)
        {
            State = CollapseState.Expanded;
            foreach (var nodecontrol in collection.NodeControls)
            {
                nodecontrol.IsCollapsed = false;
            }

            foreach (var arrowControl in collection.ArrowControls)
            {
                if (!arrowControl.From.IsCollapsed && !arrowControl.To.IsCollapsed)
                {
                    arrowControl.IsCollapsed = false;
                }
            }
            NodeControl.ParentSurface.ReScanForCollapseStates();
            AutoRealignIfInExplorerMode();
        }

        private void CollapseNodes(ChildrenControlCollection collection)
        {
            State = CollapseState.Collapsed;
            foreach (var nodecontrol in collection.NodeControls)
            {
                nodecontrol.IsCollapsed = true;
            }
            foreach (var arrowControl in collection.ArrowControls)
            {
                arrowControl.IsCollapsed = true;
                arrowControl.DeSelect();
            }
            NodeControl.ParentSurface.ReScanForCollapseStates();
            AutoRealignIfInExplorerMode();
        }


        private void AutoRealignIfInExplorerMode()
        {
            if (App.UserStyle == UserStyle.Reader && NodeControl.ParentSurface.AllowAutoRealign)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => NodeControl.ParentSurface.AutoRealign(NodeControl));
            }
        }

        private void OnButtonMouseLeave(object sender, MouseEventArgs e)
        {
            ExpandButton.Opacity = 1;
            CollapseButton.Opacity = 1;
            SemiCollapseButton.Opacity = 1;
        }

        private void OnButtonMouseEnter(object sender, MouseEventArgs e)
        {
            ExpandButton.Opacity = 0.6;
            CollapseButton.Opacity = 0.6;
            SemiCollapseButton.Opacity = 0.6;
        }
    }
}
