using System;
using System.Windows;
using System.Windows.Controls;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu
{
    public partial class SingleNodeContextMenu : UserControl
    {
        public NodeControl ParentNode { get; set; }

        public SingleNodeContextMenu()
        {
            InitializeComponent();

            //AuthorContextMenu.ShareWithClicked += ContextMenuOnShareWithClicked;
            //AuthorContextMenu.CopyUrlClicked += ContextMenuOnCopyUrlClicked;
            //AuthorContextMenu.MenuClosed += ContextMenuOnMenuClosed;

            //ViewerContextMenu.ShareWithClicked += ContextMenuOnShareWithClicked;
            //ViewerContextMenu.CopyUrlClicked += ContextMenuOnCopyUrlClicked;
            //ViewerContextMenu.MenuClosed += ContextMenuOnMenuClosed;

            //AuthorContextMenu.CutClicked += AuthorContextMenuOnCutClicked;
            //AuthorContextMenu.CopyClicked += AuthorContextMenuOnCopyClicked;
            //AuthorContextMenu.CloneClicked += AuthorContextMenuOnCloneClicked;
            //AuthorContextMenu.DeleteClicked += AuthorContextMenuOnDeleteClicked;
            //AuthorContextMenu.RecordVideoStartPositionClicked += AuthorContextMenuOnRecordVideoStartPositionClicked;
            //AuthorContextMenu.RecordVideoStopPositionClicked += AuthorContextMenuOnRecordVideoStopPositionClicked;
            //AuthorContextMenu.NodePropertiesClicked += AuthorContextMenuOnNodePropertiesClicked;
            //AuthorContextMenu.ChangeNodeTypeClicked += AuthorContextMenuOnChangeNodeTypeClicked;
        }

        

        public void ShowContextMenuForNodeControl(Point location, NodeControl nodeControl)
        {
            ParentNode = nodeControl;
            Canvas.SetLeft(this, location.X);
            Canvas.SetTop(this, location.Y);
            Visibility = Visibility.Visible;
            if (App.UserStyle == UserStyle.Author)
            {
                AuthorContextMenu.Visibility = Visibility.Visible;
                AuthorContextMenu.Focus();
                
            }
            else
            {
                ViewerContextMenu.Visibility = Visibility.Visible;
                ViewerContextMenu.Focus();
                ViewerContextMenu.Rating.DataContext = ParentNode.ViewModelNode.NodeProperties;
            }
        }

        public bool IsContextMenuOpened()
        {
            if (ViewerContextMenu.Visibility == Visibility.Visible || AuthorContextMenu.Visibility == Visibility.Visible)
            {
                return true;
            }
            return false;
        }
    }
}
