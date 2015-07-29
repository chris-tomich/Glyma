using System;
using System.Windows;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu.Base;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu
{
    public partial class AuthorNodeControlContextMenu : NodeContextMenuBase, IAuthorNodeContextMenu
    {
        public event EventHandler CutClicked;
        public event EventHandler CopyClicked;
        public event EventHandler DeleteClicked;
        public event EventHandler CloneClicked;
        public event EventHandler RecordVideoStartPositionClicked;
        public event EventHandler RecordVideoStopPositionClicked;
        public event EventHandler NodePropertiesClicked;
        
        public event EventHandler CopyUrlClicked;
        public event EventHandler<ChangeNodeTypeEventArgs> ChangeNodeTypeClicked;
        public event EventHandler<ShareWithTypeEventArgs> ShareWithClicked;

        public event EventHandler<ExportClickedEventArgs> ExportClicked; 

        public AuthorNodeControlContextMenu()
        {
            InitializeComponent();
        }

        public override void Show(NodeControl node = null)
        {
            Video.Disabled = !((MainPage)Application.Current.RootVisual).SuperGraphController.VideoController.HasVideo;
            base.Show(node);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        public override void OnMenuClosed(object sender, EventArgs e)
        {
            base.OnMenuClosed(sender, e);
            VideoSubMenu.Visibility = Visibility.Collapsed;
            ChangeTypeSubMenu.Visibility = Visibility.Collapsed;
            ShareWithSubMenu.Visibility = Visibility.Collapsed;
        }

        private void OnRecordVideoStartSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (RecordVideoStartPositionClicked != null)
            {
                RecordVideoStartPositionClicked(sender, e);
            }

            OnMenuClosed(sender, e);
        }

        private void OnRecordVideoStopSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (RecordVideoStopPositionClicked != null)
            {
                RecordVideoStopPositionClicked(sender, e);
            }

            OnMenuClosed(sender, e);
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            object focussedElement = FocusManager.GetFocusedElement();

            if (focussedElement != this 
                && focussedElement != PopupMenu 
                && !PopupMenu.Items.Contains(focussedElement)
                && focussedElement != VideoSubMenu && !VideoSubMenu.Items.Contains(focussedElement)
                && focussedElement != ChangeTypeSubMenu && !ChangeTypeSubMenu.Items.Contains(focussedElement)
                && focussedElement != ShareWithSubMenu && !ShareWithSubMenu.Items.Contains(focussedElement)
                && focussedElement != ExportSubMenu && !ExportSubMenu.Items.Contains(focussedElement))
            {
                Visibility = Visibility.Collapsed;
                
                OnMenuClosed(sender, e);
            }

            var item = focussedElement as SuperContextMenuItem;
            if (item != null)
            {
                if (!item.HasSubMenu && PopupMenu.Items.Contains(item))
                {
                    VideoSubMenu.Visibility = Visibility.Collapsed;
                    ChangeTypeSubMenu.Visibility = Visibility.Collapsed;
                    ShareWithSubMenu.Visibility = Visibility.Collapsed;
                    ExportSubMenu.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void OnCopySingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (CopyClicked != null)
            {
                CopyClicked(sender, e);
            }

            OnMenuClosed(sender, e);
        }

        private void OnDeleteSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (DeleteClicked != null)
            {
                DeleteClicked(sender, e);
            }

            OnMenuClosed(sender, e);
        }

        private void OnCutSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (CutClicked != null)
            {
                CutClicked(sender, e);
            }

            OnMenuClosed(sender, e);
        }

        private void OnPropertiesSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (NodePropertiesClicked != null)
            {
                NodePropertiesClicked(sender, e);
            }

            OnMenuClosed(sender, e);
        }

        private void OnCloneSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (CloneClicked != null)
            {
                CloneClicked(sender, e);
            }

            OnMenuClosed(sender, e);
        }

        private void OnChangeNodeTypeSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            var element = sender as FrameworkElement;

            if (ChangeNodeTypeClicked != null && element != null)
            {
                var eventArgs = new ChangeNodeTypeEventArgs();

                switch (element.Name)
                {
                    case "ContextMenuChangeToQuestion":
                        eventArgs.ChangedTo = ChangeNodeTypeEnum.Question;
                        break;
                    case "ContextMenuChangeToIdea":
                        eventArgs.ChangedTo = ChangeNodeTypeEnum.Idea;
                        break;
                    case "ContextMenuChangeToPro":
                        eventArgs.ChangedTo = ChangeNodeTypeEnum.Pro;
                        break;
                    case "ContextMenuChangeToCon":
                        eventArgs.ChangedTo = ChangeNodeTypeEnum.Con;
                        break;
                    case "ContextMenuChangeToMap":
                        eventArgs.ChangedTo = ChangeNodeTypeEnum.Map;
                        break;
                    case "ContextMenuChangeToNote":
                        eventArgs.ChangedTo = ChangeNodeTypeEnum.Note;
                        break;
                    case "ContextMenuChangeToDecision":
                        eventArgs.ChangedTo = ChangeNodeTypeEnum.Decision;
                        break;
                    default:
                        eventArgs.ChangedTo = ChangeNodeTypeEnum.Empty;
                        break;
                }
                ChangeNodeTypeClicked(sender, eventArgs);
            }
            OnMenuClosed(sender, e);
        }

        private void ContextMenuRecordVideo_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubMenu(VideoSubMenu);
            ChangeTypeSubMenu.Visibility = Visibility.Collapsed;
            ShareWithSubMenu.Visibility = Visibility.Collapsed;
            ExportSubMenu.Visibility = Visibility.Collapsed;
        }

        private void VideoSubMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            Focus();
            VideoSubMenu.Visibility = Visibility.Collapsed;
        }
        private void ContextMenuExportTo_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubMenu(ExportSubMenu);
            ChangeTypeSubMenu.Visibility = Visibility.Collapsed;
            ShareWithSubMenu.Visibility = Visibility.Collapsed;
            VideoSubMenu.Visibility = Visibility.Collapsed;
        }

        private void ExportToSubMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            Focus();
            ExportSubMenu.Visibility = Visibility.Collapsed;
        }

        private void ExportToCompendiumSingleClick(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ExportToWordSingleClick(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ExportToPdfSingleClick(object sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ContextMenuChangeType_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubMenu(ChangeTypeSubMenu);
            VideoSubMenu.Visibility = Visibility.Collapsed;
            ShareWithSubMenu.Visibility = Visibility.Collapsed;
            VideoSubMenu.Visibility = Visibility.Collapsed;
        }

        private void ChangeTypeSubMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            Focus();
            ChangeTypeSubMenu.Visibility = Visibility.Collapsed;
        }

        private void ContextMenuShareWith_MouseEnter(object sender, MouseEventArgs e)
        {
            ChangeTypeSubMenu.Visibility = Visibility.Collapsed;
            VideoSubMenu.Visibility = Visibility.Collapsed;
            VideoSubMenu.Visibility = Visibility.Collapsed;
            ShowSubMenu(ShareWithSubMenu);
        }

        private void ShareWithSubMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            Focus();
            ShareWithSubMenu.Visibility = Visibility.Collapsed;
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
