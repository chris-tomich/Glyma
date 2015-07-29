using System.ComponentModel;
using SilverlightMappingToolBasic.GlymaSecurityService;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole
{
    public class PermissionGroup : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool? _defaultValue;
        private bool? _isSelected;
        private bool _isEneabled;

        public string Name
        {
            get { return Group.DisplayName; }
        }

        public GlymaSecurityGroup Group
        {
            get; 
            private set; 
        }

        public int Id
        {
            get { return Group.GroupId; }
        }

        public PermissionGroup(GlymaSecurityGroup group, bool? isSelected, bool isEnabled = true)
        {
            Group = group;
            _defaultValue = isSelected;
            _isSelected = isSelected;
            _isEneabled = isEnabled;
        }

        public bool? IsSelected
        {
            get
            {
                return _isSelected;
                
            }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public bool IsEnabled
        {
            get
            {
                return _isEneabled;
            }
            set
            {
                if (_isEneabled != value)
                {
                    _isEneabled = value;
                    OnPropertyChanged("IsEnabled");
                }
            }
        }

        public bool? DefaultValue
        {
            get
            {
                return _defaultValue;
            }
        }

        public void ResetValue(bool? value = null)
        {
            if (value == null)
            {
                _isSelected = _defaultValue;
            }
            else
            {
                _defaultValue = value;
                _isSelected = value;
            }
            OnPropertyChanged("DefaultValue");
        }


        public void SetValue(bool? value)
        {
            _isSelected = value;
        }

        

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
