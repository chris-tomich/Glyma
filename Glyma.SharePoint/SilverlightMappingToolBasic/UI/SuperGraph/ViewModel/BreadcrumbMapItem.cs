using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightMappingToolBasic.UI.SuperGraph.ViewModel
{
    public class BreadcrumbMapItem : ViewModelBase
    {
        private ViewModel.Node _map;
        private string _name;

        public BreadcrumbMapItem()
        {
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;

                    OnNotifyPropertyChanged("Name");
                }
            }
        }

        public ViewModel.Node Map
        {
            get
            {
                return _map;
            }
            set
            {
                if (_map != value)
                {
                    _map = value;

                    OnNotifyPropertyChanged("Id");
                }
            }
        }
    }
}
