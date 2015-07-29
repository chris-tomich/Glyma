using System.Windows;
using SilverlightMappingToolBasic.UI.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.ViewModel
{
    public class BreadcrumbItemBase : ViewModelBase, IBreadcrumbItem
    {
        private bool _hasVisibleProperties;
        private Visibility _propertiesVisibility = Visibility.Collapsed;
        private bool _isFirst;

        public virtual bool IsFirst 
        {
            get
            {
                return _isFirst;
            }
            set
            {
                if (_isFirst != value)
                {
                    _isFirst = value;
                    OnNotifyPropertyChanged("IsFirst");
                    OnNotifyPropertyChanged("ArrowVisibility");
                }
            }
        }

        public Visibility ArrowVisibility
        {
            get
            {
                return IsFirst ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public bool HasVisibleProperties
        {
            get
            {
                return _hasVisibleProperties;
            }
            set
            {
                if (_hasVisibleProperties != value)
                {
                    _hasVisibleProperties = value;

                    if (_hasVisibleProperties)
                    {
                        PropertiesVisibility = Visibility.Visible;
                    }
                    else
                    {
                        PropertiesVisibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public Visibility PropertiesVisibility
        {
            get
            {
                return _propertiesVisibility;
            }
            private set
            {
                if (_propertiesVisibility != value)
                {
                    _propertiesVisibility = value;

                    OnNotifyPropertyChanged("PropertiesVisibility");
                }
            }
        }


        public virtual string Name { get; set; }
        public virtual Node Node { get; set; }
    }
}
