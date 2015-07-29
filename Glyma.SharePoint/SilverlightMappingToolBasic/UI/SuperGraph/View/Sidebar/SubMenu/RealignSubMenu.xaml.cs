using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Sidebar.Interface;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Sidebar.SubMenu
{
    public partial class RealignSubMenu : UserControl, ISubMenu
    {
        public RealignSubMenu()
        {
            InitializeComponent();
        }

        private Point _location;

        public Point Centre
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
                var newPosition = new Point
                {
                    X = _location.X - SubMenu.ActualWidth/2,
                    Y = _location.Y - SubMenu.ActualHeight/2
                };
                Canvas.SetLeft(this, newPosition.X);
                Canvas.SetTop(this, newPosition.Y);
            }
        }

        public void Hide()
        {
            Visibility = Visibility.Collapsed;
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
            if (OnOpen != null)
            {
                OnOpen(this, null);
            }
        }

        public event EventHandler OnOpen;

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Hide();
        }
    }
}
