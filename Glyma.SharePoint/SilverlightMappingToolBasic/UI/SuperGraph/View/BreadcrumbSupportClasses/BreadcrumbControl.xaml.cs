using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using System.Windows;
using SilverlightMappingToolBasic.UI.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.BreadcrumbSupportClasses
{
    public partial class BreadcrumbControl : UserControl
    {

        public IBreadcrumbItem BreadcrumbData
        {
            get
            {
                return DataContext as IBreadcrumbItem;
            }
        }


        public event EventHandler PropertiesClicked;
        public event EventHandler<BreadcrumbChangedEventArgs> BreadcrumbChanged;
        public event EventHandler BreadcrumbClicked;

        public BreadcrumbControl()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
            MapList.SelectionChanged += MapList_SelectionChanged;
            MapList.BreadcrumbClicked += TextBlock_MouseLeftButtonUp;
            MapList.PropertiesClicked += Image_MouseLeftButtonUp;
            TextBlock.BreadcrumbClicked += TextBlock_MouseLeftButtonUp;
            TextBlock.PropertiesClicked += Image_MouseLeftButtonUp;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var value = e.NewValue as IBreadcrumbItem;
            if (value != null)
            {
                if (value is MultiBreadcrumbItem)
                {
                    TextBlock.Visibility = Visibility.Collapsed;
                    MapList.Visibility = Visibility.Visible;
                    MapList.DataContext = null;
                    MapList.DataContext = value;
                }

                var item = value as BreadcrumbItem;
                if (item != null)
                {
                    item.PropertyChanged += OnPropertyChanged;
                    TextBlock.BreadcrumbText = value.Name;
                    TextBlock.PropertiesVisibility = value.PropertiesVisibility;
                }

                
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "PropertiesVisibility":
                    TextBlock.PropertiesVisibility = BreadcrumbData.PropertiesVisibility;
                    break;
                case "Name":
                    TextBlock.BreadcrumbText = BreadcrumbData.Name;
                    break;
                case "Items":
                    MapList.DataContext = null;
                    MapList.DataContext = DataContext;
                    break;
            }
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (BreadcrumbClicked != null)
            {
                BreadcrumbClicked(this, new EventArgs());
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (PropertiesClicked != null)
            {
                PropertiesClicked(this, new EventArgs());
            }
        }

        private void MapList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var mapList = sender as ComboBox;
            if (mapList != null && BreadcrumbChanged != null)
            {
                var item = mapList.SelectedItem as ComboBoxItem;

                if (item != null)
                {
                    var multiBreadcrumb = BreadcrumbData as MultiBreadcrumbItem;
                    if (multiBreadcrumb != null)
                    {
                        RaiseBreadcrumbChangedEvent(multiBreadcrumb, item.DataContext as IBreadcrumbItem);
                    }

                }
            }
        }

        private void RaiseBreadcrumbChangedEvent(MultiBreadcrumbItem breadcrumb, IBreadcrumbItem item)
        {
            breadcrumb.SelectedBreadcrumb = item;
            BreadcrumbChanged(this, new BreadcrumbChangedEventArgs { NewItem = breadcrumb.SelectedBreadcrumb });
        }
    }
}
