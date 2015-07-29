using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using SilverlightMappingToolBasic.UI.ViewModel;
using Telerik.Windows.Controls;

namespace SilverlightMappingToolBasic.UI.SuperGraph.ViewModel
{
    public class MultiBreadcrumbItem : BreadcrumbItemBase
    {
        private ObservableCollection<IBreadcrumbItem> _items;
        public ObservableCollection<IBreadcrumbItem> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new ObservableCollection<IBreadcrumbItem>();
                }
                return _items;
            }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    OnNotifyPropertyChanged("Items");
                }
            }
        }

        public override bool IsFirst
        {
            get
            {
                return base.IsFirst;
            }
            set
            {
                base.IsFirst = value;
                SelectedBreadcrumb.IsFirst = value;
            }
        }

        public override string Name {
            get
            {
                return SelectedBreadcrumb.Name;
            }
        }

        private IBreadcrumbItem _selectedBreadcrumb;
        public IBreadcrumbItem SelectedBreadcrumb
        {
            get
            {
                return _selectedBreadcrumb;
            }
            set
            {
                if (_selectedBreadcrumb != value)
                {
                    if (_selectedBreadcrumb != null)
                    {
                        _selectedBreadcrumb.PropertyChanged -= SelectedBreadcrumbOnPropertyChanged;
                    }
                    _selectedBreadcrumb = value;
                    _selectedBreadcrumb.PropertyChanged += SelectedBreadcrumbOnPropertyChanged;
                    OnNotifyPropertyChanged("Name");
                }
                
            }
        }

        private void SelectedBreadcrumbOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                OnNotifyPropertyChanged("Items");
                OnNotifyPropertyChanged("Name");
            }
        }

        public override Node Node
        {
            get
            {
                return SelectedBreadcrumb.Node;
            }
        }

        public MultiBreadcrumbItem(List<Node> nodes)
        {
            if(nodes.Count == 0) return;
            foreach (var node in nodes)
            {
                var breadcrumb = new BreadcrumbItem(node);
                Items.Add(breadcrumb);
            }
            SelectedBreadcrumb = Items[0];
        }
    }
}
