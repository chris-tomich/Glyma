using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SilverlightMappingToolBasic.Code.ColorsManagement;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu
{
    public partial class SuperContextMenuItem : MenuItem
    {
        private readonly static DependencyProperty IconColourPerProperty = DependencyProperty.Register("IconColour", typeof(SolidColorBrush), typeof(SuperContextMenuItem), new PropertyMetadata(new SolidColorBrush(ColorConverter.FromHex("#FF6F6F6F"))));
        private readonly static DependencyProperty IconHoverColourPerProperty = DependencyProperty.Register("IconHoverColour", typeof(SolidColorBrush), typeof(SuperContextMenuItem), new PropertyMetadata(new SolidColorBrush(Colors.White)));
        public static readonly DependencyProperty IconPathProperty = DependencyProperty.Register("IconPath", typeof(string), typeof(SuperContextMenuItem), new PropertyMetadata(""));
        public static readonly DependencyProperty HasSubMenuProperty = DependencyProperty.Register("HasSubMenu", typeof(Boolean), typeof(SuperContextMenuItem), new PropertyMetadata(false));
        public static readonly DependencyProperty IconAngleProperty = DependencyProperty.Register("IconAngle", typeof(int), typeof(SuperContextMenuItem), new PropertyMetadata(0));

        public int IconAngle
        {
            get { return (int)GetValue(IconAngleProperty); }
            set { SetValue(IconAngleProperty, value); }
        }

        public Brush IconHoverColour
        {
            get { return GetValue(IconHoverColourPerProperty) as SolidColorBrush; }
            set { SetValue(IconHoverColourPerProperty, value); }
        }

        public Brush IconColour
        {
            get { return GetValue(IconColourPerProperty) as SolidColorBrush; }
            set { SetValue(IconColourPerProperty, value); }
        }

        public string IconPath
        {
            get { return GetValue(IconPathProperty).ToString(); }
            set { SetValue(IconPathProperty, value); }
        }

        public Boolean HasSubMenu
        {
            get { return (Boolean)GetValue(HasSubMenuProperty); }
            set
            {
                SetValue(HasSubMenuProperty, value);
                if (value)
                {
                    Style = Application.Current.Resources["SuperMenuSubMenuItemStyle"] as Style;
                }
            } 
        }


        public SuperContextMenuItem()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void ContextMenuCut_MouseEnter(object sender, MouseEventArgs e)
        {
            Icon.Fill = IconHoverColour;
            Focus();
        }

        private void ContextMenuCut_MouseLeave(object sender, MouseEventArgs e)
        {
            Icon.Fill = IconColour;
        }


        public bool Disabled
        {
            get
            {
                return !IsEnabled;
                
            }
            set
            {
                IsEnabled = !value;
                if (IsEnabled)
                {
                    Icon.Fill = IconColour;
                    Opacity = 1;
                }
                else
                {
                    Icon.Fill = IconColour;
                    Opacity = 0.5;
                }
            }

        }
    }
}
