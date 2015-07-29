using System.Linq;
using System.Windows;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.Extensions.JavascriptCallback;
using SilverlightMappingToolBasic.UI.Extensions.Json;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu.Base
{
    public partial class NodeContextMenuBase : ContextMenuBase
    {
        private NodeControl _nodeControl;
        private bool IsAdded { get; set; }
        public NodeContextMenuBase()
        {
            InitializeComponent();
        }

        public override void Show(NodeControl node = null)
        {
            _nodeControl = node;
            base.Show(node);
            if (node != null && !IsAdded)
            {
                if (ViewerNodeControlContextMenu != null)
                {
                    if (JavascriptCallbackRegister.Instance.HasViewerCustomContextMenuItem)
                    {
                        ViewerNodeControlContextMenu.CustomContextMenuItemSeparator.Visibility = Visibility.Visible;
                        var items = ViewerNodeControlContextMenu.PopupMenu.Items.Select(item => item as FrameworkElement).ToList();
                        var index = 0;
                        foreach (var item in items)
                        {
                            if (item.Name == "CustomContextMenuItemSeparator")
                            {
                                break;
                            }
                            index++;
                        }

                        foreach (var viewerItem in JavascriptCallbackRegister.Instance.ViewerItems)
                        {
                            var button = new SuperContextMenuItem();
                            button.IconPath = string.Empty;
                            button.Name = viewerItem;
                            button.Header = viewerItem;
                            button.MouseLeftButtonUp += ButtonOnMouseLeftButtonUp;
                            index++;
                            items.Insert(index, button);
                        }
                        IsAdded = true;
                        ViewerNodeControlContextMenu.PopupMenu.Items.Clear();
                        ViewerNodeControlContextMenu.PopupMenu.ItemsSource = items;
                    }
                    else
                    {
                        ViewerNodeControlContextMenu.CustomContextMenuItemSeparator.Visibility = Visibility.Collapsed;
                    }
                } 
                else if (AuthorNodeControlContextMenu != null)
                {
                    if (JavascriptCallbackRegister.Instance.HasAuthorCustomContextMenuItem)
                    {
                        AuthorNodeControlContextMenu.CustomContextMenuItemSeparator.Visibility = Visibility.Visible;
                        var items = AuthorNodeControlContextMenu.PopupMenu.Items.Select(item => item as FrameworkElement).ToList();
                        var index = 0;
                        foreach (var item in items)
                        {
                            if (item.Name == "CustomContextMenuItemSeparator")
                            {
                                break;
                            }
                            index ++;
                        }

                        foreach (var viewerItem in JavascriptCallbackRegister.Instance.AuthorItems)
                        {
                            var button = new SuperContextMenuItem();
                            button.IconPath = string.Empty;
                            button.Name = viewerItem;
                            button.Header = viewerItem;
                            button.MouseLeftButtonUp += ButtonOnMouseLeftButtonUp;
                            index ++;
                            items.Insert(index, button);
                        }
                        IsAdded = true;
                        AuthorNodeControlContextMenu.PopupMenu.Items.Clear();
                        AuthorNodeControlContextMenu.PopupMenu.ItemsSource = items;
                    }
                    else
                    {
                        AuthorNodeControlContextMenu.CustomContextMenuItemSeparator.Visibility = Visibility.Collapsed;
                    }
                }
            }
            
        }

        private void ButtonOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_nodeControl != null )
            {
                var item = sender as SuperContextMenuItem;
                if (item != null)
                {
                    var node = new NodeJson(_nodeControl.ViewModelNode);
                    var map = new NodeJson(_nodeControl.ParentSurface.Context);
                    var rootMap = new NodeJson(RootMapProperties.Instance.Metadata, RootMapProperties.Instance.Id, RootMapProperties.Instance.DomainId);

                    var nodeJson = node.ToJson();
                    var mapJson = map.ToJson();
                    var rootMapJson = rootMap.ToJson();

                    JavascriptCallbackRegister.Instance.CallBack(Type, item.Name, rootMapJson, mapJson, nodeJson);
                }

                
            }
            OnMenuClosed(this, null);
        }

        private ContextMenuType Type
        {
            get { return this is AuthorNodeControlContextMenu ? ContextMenuType.Author : ContextMenuType.Viewer; }
        }

        public AuthorNodeControlContextMenu AuthorNodeControlContextMenu
        {
            get
            {
                var menu = this as AuthorNodeControlContextMenu;
                if (menu != null)
                {
                    return menu;
                }
                return null;
            }
        }

        public ViewerNodeControlContextMenu ViewerNodeControlContextMenu
        {
            get
            {
                var contextMenu = this as ViewerNodeControlContextMenu;
                if (contextMenu != null)
                {
                    return contextMenu;
                }
                return null;
            }
        }
    }
}
