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
using SilverlightMappingToolBasic.UI.SuperGraph.View.Sidebar.Interface;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Sidebar.SubMenu
{
    public partial class MapFilterSubMenu : UserControl, ISubMenu
    {
        public MapFilterSubMenu()
        {
            InitializeComponent();
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
