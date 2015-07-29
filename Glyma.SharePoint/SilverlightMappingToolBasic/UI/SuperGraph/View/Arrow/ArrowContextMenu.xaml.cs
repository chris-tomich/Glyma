using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu.Base;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow
{
    public partial class ArrowContextMenu : ContextMenuBase
    {
        public ArrowContextMenu()
        {
            InitializeComponent();
        }

        public IArrowControl ArrowControl
        {
            get;
            set;
        }

        private void OnDeleteSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Visibility = Visibility.Collapsed;
            ArrowControl.Delete();
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            object focussedElement = FocusManager.GetFocusedElement();

            if (focussedElement != this && focussedElement != PopupMenu && !PopupMenu.Items.Contains(focussedElement))
            {
                Visibility = Visibility.Collapsed;
            }
        }
    }
}
