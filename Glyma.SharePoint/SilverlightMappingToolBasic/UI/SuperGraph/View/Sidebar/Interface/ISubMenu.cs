using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Sidebar.Interface
{
    public interface ISubMenu
    {
        Visibility Visibility { get; set; }

        void Hide();
        void Show();

        event EventHandler OnOpen;
    }
}
