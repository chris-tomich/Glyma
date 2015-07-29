using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
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

namespace SilverlightMappingToolBasic.UI.SuperGraph.ViewModel
{
    public class SidebarModel : INotifyPropertyChanged
    {
        private UserStyle _userStyle = UserStyle.Reader;

        public UserStyle UserStyle
        {
            get
            {
                return _userStyle;
            }
            set
            {
                if (_userStyle != value)
                {
                    _userStyle = value;
                    OnPropertyChanged("UserStyle");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
