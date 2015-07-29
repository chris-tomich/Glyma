using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu.Base;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu
{
    public partial class MultiNodesContextMenu : ContextMenuBase
    {
        public MultiNodesContextMenu()
        {
            InitializeComponent();
        }

        public event EventHandler CutClicked;
        public event EventHandler CopyClicked;
        public event EventHandler DeleteClicked;
        public event EventHandler CloneClicked;
        public event EventHandler NodePropertiesClicked;

        public event EventHandler<ExportClickedEventArgs> ExportClicked;
        public event EventHandler<ChangeNodeTypeEventArgs> ChangeNodeTypeClicked;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }


        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            object focussedElement = FocusManager.GetFocusedElement();

            if (focussedElement != this && focussedElement != PopupMenu && !PopupMenu.Items.Contains(focussedElement)
                && focussedElement != ExportSubMenu && !ExportSubMenu.Items.Contains(focussedElement)
                && focussedElement != ChangeTypeSubMenu && !ChangeTypeSubMenu.Items.Contains(focussedElement))
            {
                Visibility = Visibility.Collapsed;
                OnMenuClosed(sender, e);
            }

            var item = focussedElement as SuperContextMenuItem;
            if (item != null)
            {
                if (!item.HasSubMenu && PopupMenu.Items.Contains(item))
                {
                    ExportSubMenu.Visibility = Visibility.Collapsed;
                    ChangeTypeSubMenu.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void OnCopySingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Visibility = Visibility.Collapsed;

            if (CopyClicked != null)
            {
                CopyClicked(sender, e);
            }

            OnMenuClosed(sender, e);
        }

        private void OnDeleteSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Visibility = Visibility.Collapsed;

            if (DeleteClicked != null)
            {
                DeleteClicked(sender, e);
            }

            OnMenuClosed(sender, e);
        }

        private void OnCutSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Visibility = Visibility.Collapsed;

            if (CutClicked != null)
            {
                CutClicked(sender, e);
            }

            OnMenuClosed(sender, e);
        }


        private void OnCloneSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Visibility = Visibility.Collapsed;

            if (CloneClicked != null)
            {
                CloneClicked(sender, e);
            }

            OnMenuClosed(sender, e);
            
        }

        private void OnPropertiesSingleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Visibility = Visibility.Collapsed;

            if (NodePropertiesClicked != null)
            {
                NodePropertiesClicked(sender, e);
            }

            OnMenuClosed(sender, e);
        }

        private void ContextMenuExportTo_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubMenu(ExportSubMenu);
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

        private void ChangeTypeSubMenu_MouseLeave(object sender, MouseEventArgs e)
        {
            Focus();
            ChangeTypeSubMenu.Visibility = Visibility.Collapsed;
        }

        private void ContextMenuChangeType_MouseEnter(object sender, MouseEventArgs e)
        {
            ShowSubMenu(ChangeTypeSubMenu);
        }
    }
}
