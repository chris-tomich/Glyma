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
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MouseCursor;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public partial class NodeDetailIndicatorButton : UserControl
    {
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

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(NodeDetailIndicatorButton), new PropertyMetadata(""));
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(NodeDetailIndicatorButton), new PropertyMetadata(""));
        public static readonly DependencyProperty HoverIconProperty = DependencyProperty.Register("HoverIcon", typeof(string), typeof(NodeDetailIndicatorButton), new PropertyMetadata(""));

        public NodeDetailIndicatorButton()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void LayoutRoot_MouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Hand;
            IconImage.Visibility = Visibility.Collapsed;
            HoverIconImage.Visibility = Visibility.Visible;
        }

        private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            IconImage.Visibility = Visibility.Visible;
            HoverIconImage.Visibility = Visibility.Collapsed;
        }

        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool) e.NewValue)
            {
                Opacity = 0.2;
            }
            else
            {
                Opacity = 1;
            }
        }


        private void NodeDetailIndicatorButton_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}
