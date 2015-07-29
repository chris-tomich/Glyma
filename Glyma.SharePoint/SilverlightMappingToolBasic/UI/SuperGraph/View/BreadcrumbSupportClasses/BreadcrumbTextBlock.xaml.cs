using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.BreadcrumbSupportClasses
{
    public partial class BreadcrumbTextBlock : UserControl
    {
        public string BreadcrumbText
        {
            get { return GetValue(BreadcrumbTextProperty).ToString(); }
            set { SetValue(BreadcrumbTextProperty, value); }
        }
        public static readonly DependencyProperty BreadcrumbTextProperty = DependencyProperty.Register(
          "BreadcrumbText", typeof(string), typeof(BreadcrumbTextBlock), null);


        public Visibility PropertiesVisibility
        {
            get { return (Visibility)GetValue(PropertiesVisibilityProperty); }
            set { SetValue(PropertiesVisibilityProperty, value); }
        }
        public static readonly DependencyProperty PropertiesVisibilityProperty = DependencyProperty.Register(
          "PropertiesVisibility", typeof(Visibility), typeof(BreadcrumbTextBlock), null);

        public event EventHandler<MouseButtonEventArgs> PropertiesClicked;
        public event EventHandler<MouseButtonEventArgs> BreadcrumbClicked;

        public BreadcrumbTextBlock()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (PropertiesClicked != null)
            {
                PropertiesClicked(sender, e);
            }
        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (BreadcrumbClicked != null)
            {
                BreadcrumbClicked(sender, e);
            }
        }
    }
}
