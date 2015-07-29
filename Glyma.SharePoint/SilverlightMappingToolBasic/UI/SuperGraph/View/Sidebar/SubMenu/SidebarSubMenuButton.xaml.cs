using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SilverlightMappingToolBasic.Code.ColorsManagement;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Sidebar.SubMenu
{
    public partial class SidebarSubMenuButton : UserControl
    {
        public SidebarSubMenuButton()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string Text
        {
            get { return GetValue(TextProperty).ToString(); }
            set { SetValue(TextProperty, value); }
        }

        public string Icon
        {
            get { return GetValue(IconProperty).ToString(); }
            set { SetValue(IconProperty, value); }
        }

        public string HoverIcon
        {
            get { return GetValue(HoverIconProperty).ToString(); }
            set { SetValue(HoverIconProperty, value); }
        }    

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SidebarSubMenuButton), new PropertyMetadata(""));
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(SidebarSubMenuButton), new PropertyMetadata(""));
        public static readonly DependencyProperty HoverIconProperty = DependencyProperty.Register("HoverIcon", typeof(string), typeof(SidebarSubMenuButton), new PropertyMetadata(""));

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            ButtonStackPanel.Background = new SolidColorBrush(ColorConverter.FromHex("#FF57afcf"));
            ButtonText.Foreground = new SolidColorBrush(Colors.White);
            HoverIconImage.Visibility = Visibility.Visible;
            IconImage.Visibility = Visibility.Collapsed;
            
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            ButtonStackPanel.Background = new SolidColorBrush(Colors.White);
            ButtonText.Foreground = new SolidColorBrush(ColorConverter.FromHex("#ff848788"));
            HoverIconImage.Visibility = Visibility.Collapsed;
            IconImage.Visibility = Visibility.Visible;
        }
    }
}
