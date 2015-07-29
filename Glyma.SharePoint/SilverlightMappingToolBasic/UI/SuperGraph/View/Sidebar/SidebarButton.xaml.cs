using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Sidebar
{
    public partial class SidebarButton : SidebarButtonBase
    {
        public SidebarButton()
        {
            InitializeComponent();
        }



        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Over", true);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Out", true);
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, !IsClicked ? "Over" : "Clicked", true);
        }

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsClicked = true;
            VisualStateManager.GoToState(this, "Clicked", true);
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsClicked = false;
            VisualStateManager.GoToState(this, "Over", true);
        }     
    }
}
