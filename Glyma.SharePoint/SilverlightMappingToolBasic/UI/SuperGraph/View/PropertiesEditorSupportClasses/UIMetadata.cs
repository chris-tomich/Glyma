using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Controls;
using SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses.Edit;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses
{
    public class UIMetadata : INotifyPropertyChanged
    {
        private string _name;
        private string _value;
        private string _originalName;

        public event EventHandler<NameChangedEventArgs> NameChanged;

        public bool IsInserted { get; set; }

        public string Name 
        {
            get
            {
                if (_name == null)
                {
                    _name = string.Empty;
                }
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    var args = new NameChangedEventArgs {OldText = _name, NewText = value.Trim()};
                    _name = value.Trim();
                    if (NameChanged != null)
                    {
                        NameChanged(this, args);
                    }
                    OnPropertyChanged("Name");
                }
            }
        }

        public string OriginalName
        {
            get
            {
                if (_originalName == null)
                {
                    _originalName = string.Empty;
                }
                return _originalName;
            }
            set
            {
                _originalName = value;
            }
        }


        public string Value 
        {
            get
            {
                if (_value == null)
                {
                    _value = string.Empty;
                }
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged("Value");
                }
            } 
        }

        public UIMetadata(string name, string value, bool isInserted = true)
        {
            OriginalName = name;
            Name = OriginalName;
            Value = value;
            IsInserted = isInserted;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
