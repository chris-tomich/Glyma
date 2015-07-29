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
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu.Base
{
    public partial class ContextMenuBase : UserControl
    {
        public ContextMenuBase()
        {
            InitializeComponent();
        }

        public event EventHandler MenuClosed;

        public double SpaceForSubMenuX { get; set; }
        public double SpaceForSubMenuY { get; set; }

        public virtual void Show(NodeControl node = null)
        {
            Visibility = Visibility.Visible;
            Focus();
        }

        public virtual void OnMenuClosed(object sender, EventArgs e)
        {
            Visibility = Visibility.Collapsed;
            if (MenuClosed != null)
            {
                MenuClosed(sender, e);
            }
        }

        public void ShowSubMenu(SubContextMenu contextMenu)
        {
            ContextMenuOpenType openType;
            if (contextMenu.ActualWidth > SpaceForSubMenuX)
            {
                if ((contextMenu.ActualHeight + contextMenu.Location.Top) > (ActualHeight + SpaceForSubMenuY))
                {
                    openType = ContextMenuOpenType.LeftTop;
                }
                else
                {
                    openType = ContextMenuOpenType.LeftBottom;
                }
            }
            else
            {
                if ((contextMenu.ActualHeight + contextMenu.Location.Top) > (ActualHeight + SpaceForSubMenuY))
                {
                    openType = ContextMenuOpenType.RightTop;
                }
                else
                {
                    openType = ContextMenuOpenType.RightBottom;
                }
            }

            switch (openType)
            {
                case ContextMenuOpenType.RightBottom:
                    contextMenu.Margin = contextMenu.Location;
                    break;
                case ContextMenuOpenType.RightTop:
                    var locationRightTop = contextMenu.Location;
                    locationRightTop.Top -= (contextMenu.ActualHeight - 30);
                    contextMenu.Margin = locationRightTop;
                    break;
                case ContextMenuOpenType.LeftBottom:
                    var locationLeftBottom = contextMenu.Location;
                    locationLeftBottom.Left -= (ActualWidth + contextMenu.ActualWidth - 10);
                    contextMenu.Margin = locationLeftBottom;
                    break;
                case ContextMenuOpenType.LeftTop:
                    var locationLeftTop = contextMenu.Location;
                    locationLeftTop.Left -= (ActualWidth + contextMenu.ActualWidth - 10);
                    locationLeftTop.Top -= (contextMenu.ActualHeight - 30);
                    contextMenu.Margin = locationLeftTop;
                    break;
            }

            contextMenu.Visibility = Visibility.Visible;
        }
    }
}
