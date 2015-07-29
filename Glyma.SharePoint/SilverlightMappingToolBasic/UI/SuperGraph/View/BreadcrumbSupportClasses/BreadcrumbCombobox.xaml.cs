using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.BreadcrumbSupportClasses
{
    public partial class BreadcrumbComboBox : UserControl
    {
        private bool _isDisableSelectionChangedEvent;
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        public event EventHandler<MouseButtonEventArgs> BreadcrumbClicked;
        public event EventHandler<MouseButtonEventArgs> PropertiesClicked;

        public BreadcrumbComboBox()
        {
            InitializeComponent();
            TextBlock.PropertiesClicked += OnPropertiesClicked;
            TextBlock.BreadcrumbClicked += OnBreadcrumbClicked;
        }

        private void MapList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isDisableSelectionChangedEvent && SelectionChanged != null)
            {
                SelectionChanged(sender, e);
            }
            _isDisableSelectionChangedEvent = false;
        }

        private MultiBreadcrumbItem BreadcrumbData
        {
            get
            {
                return DataContext as MultiBreadcrumbItem;
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var breadcrumbData = e.NewValue as MultiBreadcrumbItem;
            if (breadcrumbData != null)
            {
                breadcrumbData.PropertyChanged += BreadcrumbDataOnPropertyChanged;
                foreach (var item in breadcrumbData.Items)
                {
                    var comboItem = new ComboBoxItem();
                    comboItem.Content = item.Name;
                    comboItem.DataContext = item;
                    comboItem.Style = Application.Current.Resources["SuperComboBoxItemStyle"] as Style;
                    MapList.Items.Add(comboItem);
                }
                TextBlock.BreadcrumbText = breadcrumbData.Name;
                TextBlock.PropertiesVisibility = breadcrumbData.PropertiesVisibility;
                //MapList.Width = TextCanvas.ActualWidth + 20;
            }
        }

        private void BreadcrumbDataOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "PropertiesVisibility")
            {
                TextBlock.PropertiesVisibility = BreadcrumbData.PropertiesVisibility;
            }
            else if (e.PropertyName == "Name")
            {
                TextBlock.BreadcrumbText = BreadcrumbData.Name;
            }
            //else if (e.PropertyName == "SelectedBreadcrumb")
            //{
            //    TextBlock.BreadcrumbText = BreadcrumbData.Name;
            //    _isDisableSelectionChangedEvent = true;
            //    var index =
            //        MapList.Items.IndexOf(
            //            MapList.Items.FirstOrDefault(q => ReferenceEquals(((ComboBoxItem) q).DataContext, BreadcrumbData)));
            //    if (index >= 0)
            //    {
            //        MapList.SelectedIndex = index;
            //    }
            //}
        }

        private void OnPropertiesClicked(object sender, MouseButtonEventArgs e)
        {
            if (PropertiesClicked != null)
            {
                PropertiesClicked(sender, e);
            }
            
        }

        private void OnBreadcrumbClicked(object sender, MouseButtonEventArgs e)
        {
            if (BreadcrumbClicked != null)
            {
                BreadcrumbClicked(sender, e);
            }
        }
    }
}
