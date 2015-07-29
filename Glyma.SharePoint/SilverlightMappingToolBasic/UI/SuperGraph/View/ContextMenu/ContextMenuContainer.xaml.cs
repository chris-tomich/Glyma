using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu.Base;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Selector;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu
{
    public partial class ContextMenuContainer : UserControl
    {
        private ContextMenuBase _showingContextMenu;

        public event EventHandler<CommitCollapseStatesEventArgs> CommitCollapseStatesClicked;

        public event EventHandler<CreateNewNodeClickedEventArgs> CreateNewNodeClicked;

        public event EventHandler ImportClicked;

        public event EventHandler<ExportClickedEventArgs> ExportClicked; 

        public IContextMenuParent ContextMenuParent {
            get { return ContextMenu.ContextMenuParent; }
            set
            {
                if (value != null)
                {
                    ContextMenu.ContextMenuParent = value;
                    ContextMenu.ContextMenuParent.MapMoved += ContextMenuParentOnMapMoved;
                }
            }
        }

        private NodeControl _parentNodeControl;

        public ContextMenuContainer()
        {
            InitializeComponent();
            ContextMenu.CommitCollapseStatesClicked += ContextMenuOnCommitCollapseStatesClicked;
            ContextMenu.ImportClicked += ContextMenuOnImportClicked;
            ContextMenu.CreateNewNodeClicked += ContextMenuOnCreateNewNodeClicked;
            MultiNodesContextMenu.ExportClicked += MultiNodesOnExportClicked;
            AuthorContextMenu.ShareWithClicked += ContextMenuOnShareWithClicked;
            AuthorContextMenu.CopyUrlClicked += ContextMenuOnCopyUrlClicked;
            AuthorContextMenu.MenuClosed += ContextMenuOnMenuClosed;
            AuthorContextMenu.ExportClicked += SingleNodeOnExportClicked;

            ViewerContextMenu.ShareWithClicked += ContextMenuOnShareWithClicked;
            ViewerContextMenu.CopyUrlClicked += ContextMenuOnCopyUrlClicked;
            ViewerContextMenu.MenuClosed += ContextMenuOnMenuClosed;

            AuthorContextMenu.CutClicked += AuthorContextMenuOnCutClicked;
            AuthorContextMenu.CopyClicked += AuthorContextMenuOnCopyClicked;
            AuthorContextMenu.CloneClicked += AuthorContextMenuOnCloneClicked;
            AuthorContextMenu.DeleteClicked += AuthorContextMenuOnDeleteClicked;
            AuthorContextMenu.RecordVideoStartPositionClicked += AuthorContextMenuOnRecordVideoStartPositionClicked;
            AuthorContextMenu.RecordVideoStopPositionClicked += AuthorContextMenuOnRecordVideoStopPositionClicked;
            AuthorContextMenu.NodePropertiesClicked += AuthorContextMenuOnNodePropertiesClicked;
            AuthorContextMenu.ChangeNodeTypeClicked += AuthorContextMenuOnChangeNodeTypeClicked;
        }

        private void ContextMenuOnCreateNewNodeClicked(object sender, CreateNewNodeClickedEventArgs e)
        {
            if (CreateNewNodeClicked != null)
            {
                CreateNewNodeClicked(sender, e);
            }
        }

        private void SingleNodeOnExportClicked(object sender, ExportClickedEventArgs e)
        {
            e.SelectedNodes.Add(_parentNodeControl.ViewModelNode.Proxy.Id);
            //if (_parentNodeControl != null)
            //{
            //    _parentNodeControl.ShareWith(e.ShareWith);
            //}
            if (ExportClicked != null)
            {
                ExportClicked(sender, e);
            }
        }

        private void MultiNodesOnExportClicked(object sender, ExportClickedEventArgs e)
        {
            foreach (var node in _parentNodeControl.ParentSurface.Selector.NodeControls)
            {
                e.SelectedNodes.Add(node.ViewModelNode.Proxy.Id);
            }
            if (ExportClicked != null)
            {
                ExportClicked(sender, e);
            }
        }

        private void ContextMenuOnImportClicked(object sender, EventArgs eventArgs)
        {
            if (ImportClicked != null)
            {
                ImportClicked(sender, eventArgs);
            }
        }


        private void ContextMenuParentOnMapMoved(object sender, MoveTransformEventArgs e)
        {
            if (_showingContextMenu != null && _showingContextMenu.Visibility == Visibility.Visible)
            {
                var x = Canvas.GetLeft(_showingContextMenu);
                var y = Canvas.GetTop(_showingContextMenu);
                Canvas.SetLeft(_showingContextMenu, x + e.X * ContextMenuParent.Zoom);
                Canvas.SetTop(_showingContextMenu, y + e.Y * ContextMenuParent.Zoom);
            }
        }


        private void ContextMenuOnCommitCollapseStatesClicked(object sender, CommitCollapseStatesEventArgs e)
        {
            if (CommitCollapseStatesClicked != null)
            {
                CommitCollapseStatesClicked(sender, e);
            }
        }


        private void AuthorContextMenuOnNodePropertiesClicked(object sender, EventArgs e)
        {
            if (_parentNodeControl != null)
            {
                _parentNodeControl.NodeProperties();
            }
        }

        private void AuthorContextMenuOnRecordVideoStopPositionClicked(object sender, EventArgs e)
        {
            if (_parentNodeControl != null)
            {
                _parentNodeControl.VideoRemoteControl.RecordVideoStopPosition();
            }
        }

        private void AuthorContextMenuOnRecordVideoStartPositionClicked(object sender, EventArgs e)
        {
            if (_parentNodeControl != null)
            {
                _parentNodeControl.VideoRemoteControl.RecordVideoStartPosition();
            }
        }

        private void AuthorContextMenuOnDeleteClicked(object sender, EventArgs e)
        {
            if (_parentNodeControl != null)
            {
                var parentSurface = _parentNodeControl.ParentSurface;
                parentSurface.Selector.DeleteNodes(_parentNodeControl);
                parentSurface.Focus();
            }
        }

        private void AuthorContextMenuOnCloneClicked(object sender, EventArgs e)
        {
            if (_parentNodeControl != null)
            {
                _parentNodeControl.Clone();
            }
        }

        private void AuthorContextMenuOnCopyClicked(object sender, EventArgs e)
        {
            if (_parentNodeControl != null)
            {
                _parentNodeControl.Copy();
            }
        }

        private void ContextMenuOnShareWithClicked(object sender, ShareWithTypeEventArgs e)
        {
            if (_parentNodeControl != null)
            {
                _parentNodeControl.ShareWith(e.ShareWith);
            }
        }

        private void AuthorContextMenuOnCutClicked(object sender, EventArgs e)
        {
            if (_parentNodeControl != null)
            {
                _parentNodeControl.CutNode();
            }
        }

        private void ContextMenuOnMenuClosed(object sender, EventArgs e)
        {
            if (_parentNodeControl != null)
            {
                _parentNodeControl.MenuClosed(sender, e);
            }
        }

        private void ContextMenuOnCopyUrlClicked(object sender, EventArgs e)
        {
            if (_parentNodeControl != null)
            {
                _parentNodeControl.CopyUrl();
            }
        }

        private void AuthorContextMenuOnChangeNodeTypeClicked(object sender, ChangeNodeTypeEventArgs e)
        {
            if (_parentNodeControl != null)
            {
                if (_parentNodeControl.ViewModelNode.IsMapNode && e.ChangedTo != ChangeNodeTypeEnum.Map)
                {
                    SuperMessageBoxService.ShowWarning("Change Node Type",
                        "You are changing a map node, you might lose any content within the map. Are you sure you wish to continue? \r\n\r\n" +
                        "Press YES to change node type anyway\r\n" +
                        "Press NO to Cancel",
                        "Yes",
                        "No",
                        () => _parentNodeControl.ChangeNodeType(e));
                }
                else
                {
                    _parentNodeControl.ChangeNodeType(e);
                }
            }
        }

        public Visibility ContextMenuVisibility 
        {
            get
            {
                return LayoutRoot.Children.Any(q => q.Visibility == Visibility.Visible)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            set
            {
                if (value == Visibility.Collapsed)
                {
                    foreach (var child in LayoutRoot.Children)
                    {
                        child.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public void ShowContextMenu<T>(Point location, NodeControl node = null) where T : ContextMenuBase
        {
            _parentNodeControl = node;

            var contextMenu = LayoutRoot.Children.OfType<T>().FirstOrDefault();
            if (contextMenu != null)
            {
                if (node != null)
                {
                    contextMenu.Show(node);
                }
                else
                {
                    contextMenu.Show();
                }

                contextMenu.UpdateLayout();
                _showingContextMenu = contextMenu;

                var width = contextMenu.ActualWidth;
                var height = contextMenu.ActualHeight;

                ContextMenuOpenType openType;
                if (location.X > ContextMenuParent.ContainerWidth - width)
                {
                    if (location.Y > ContextMenuParent.ContainerHeight - height)
                    {
                        openType = ContextMenuOpenType.LeftTop;
                        contextMenu.SpaceForSubMenuY = ContextMenuParent.ContainerHeight - location.Y;
                    }
                    else
                    {
                        openType = ContextMenuOpenType.LeftBottom;
                        contextMenu.SpaceForSubMenuY = ContextMenuParent.ContainerHeight - location.Y - height;
                    }
                    contextMenu.SpaceForSubMenuX = ContextMenuParent.ContainerWidth - location.X;
                }
                else
                {
                    if (location.Y > ContextMenuParent.ContainerHeight - height)
                    {
                        openType = ContextMenuOpenType.RightTop;
                        contextMenu.SpaceForSubMenuY = ContextMenuParent.ContainerHeight - location.Y;
                    }
                    else
                    {
                        openType = ContextMenuOpenType.RightBottom;
                        contextMenu.SpaceForSubMenuY = ContextMenuParent.ContainerHeight - location.Y - height;
                    }
                    contextMenu.SpaceForSubMenuX = ContextMenuParent.ContainerWidth - location.X - width;
                }

                switch (openType)
                {
                    case ContextMenuOpenType.RightBottom:
                        Canvas.SetLeft(contextMenu, location.X);
                        Canvas.SetTop(contextMenu, location.Y);
                        break;
                    case ContextMenuOpenType.RightTop:
                        Canvas.SetLeft(contextMenu, location.X);
                        Canvas.SetTop(contextMenu, location.Y - height);
                        break;
                    case ContextMenuOpenType.LeftBottom:
                        Canvas.SetLeft(contextMenu, location.X - width);
                        Canvas.SetTop(contextMenu, location.Y);
                        break;
                    case ContextMenuOpenType.LeftTop:
                        Canvas.SetLeft(contextMenu, location.X - width);
                        Canvas.SetTop(contextMenu, location.Y - height);
                        break;
                }





            }
        }
    }
}
