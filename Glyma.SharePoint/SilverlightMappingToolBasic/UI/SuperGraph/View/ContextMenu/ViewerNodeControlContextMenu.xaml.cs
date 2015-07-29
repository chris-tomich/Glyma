using System;
using System.Windows;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu.Base;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu.Interface;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu
{
    public partial class ViewerNodeControlContextMenu : NodeContextMenuBase, IViewerNodeContextMenu
    {
        public event EventHandler CopyUrlClicked;
        public event EventHandler<ShareWithTypeEventArgs> ShareWithClicked;
        public event EventHandler Rated;

        public ViewerNodeControlContextMenu()
        {
            InitializeComponent();
        }



        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            object focussedElement = FocusManager.GetFocusedElement();

            if (focussedElement != this && 
                focussedElement != PopupMenu 
                && !PopupMenu.Items.Contains(focussedElement))
            {
                Visibility = Visibility.Collapsed;
                OnMenuClosed(sender, e);
            }
        }

        private void OnCopyToClipboardSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (ShareWithClicked != null)
            {
                ShareWithClicked(sender, new ShareWithTypeEventArgs{ShareWith = ShareWithType.Clipboard});
            }
            OnMenuClosed(sender, e);
        }

        private void OnEmailToFriendSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (ShareWithClicked != null)
            {
                ShareWithClicked(sender, new ShareWithTypeEventArgs { ShareWith = ShareWithType.Email });
            }
            OnMenuClosed(sender, e);
        }

        private void OnFacebookSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (ShareWithClicked != null)
            {
                ShareWithClicked(sender, new ShareWithTypeEventArgs { ShareWith = ShareWithType.Facebook });
            }
            OnMenuClosed(sender, e);
        }

        private void OnTwitterSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (ShareWithClicked != null)
            {
                ShareWithClicked(sender, new ShareWithTypeEventArgs { ShareWith = ShareWithType.Twitter });
            }
            OnMenuClosed(sender, e);
        }

        private void OnGooglePlusSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (ShareWithClicked != null)
            {
                ShareWithClicked(sender, new ShareWithTypeEventArgs { ShareWith = ShareWithType.GooglePlus });
            }
            OnMenuClosed(sender, e);
        }

        private void OnLinkedInSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (ShareWithClicked != null)
            {
                ShareWithClicked(sender, new ShareWithTypeEventArgs { ShareWith = ShareWithType.LinkedIn });
            }
            OnMenuClosed(sender, e);
        }

        private void OnYammerSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (ShareWithClicked != null)
            {
                ShareWithClicked(sender, new ShareWithTypeEventArgs { ShareWith = ShareWithType.Yammer });
            }
            OnMenuClosed(sender, e);
        }
    }
}
