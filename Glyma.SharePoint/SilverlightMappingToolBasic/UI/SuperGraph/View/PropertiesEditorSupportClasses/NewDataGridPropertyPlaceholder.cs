using System;
using System.ComponentModel;
using SilverlightMappingToolBasic.UI.ViewModel;
using TransactionalNodeService.Proxy;
using TransactionalNodeService.Service;
using TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses
{
    public sealed class NewDataGridPropertyPlaceholder : IMetadata
    {
        private string _name = " ";
        private string _value = " ";

        public event PropertyChangedEventHandler PropertyChanged;

        public NewDataGridPropertyPlaceholder()
        {
            IsNew = true;
        }

        public IMapManager MapManager
        {
            get;
            set;
        }

        public bool IsNew
        {
            get;
            set;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                var chain = new TransactionChain();

                SetName(value, ref chain);
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                var chain = new TransactionChain();

                SetValue(value, ref chain);
            }
        }

        public IMetadataSet MetadataSet
        {
            get;
            set;
        }

        

        public void SetNameWithoutTrigger(string name)
        {
            _name = name;
        }

        public void SetValueWithoutTrigger(string value)
        {
            _value = value;
        }

        public void Delete()
        {
            if (MetadataSet != null)
            {
                var chain = new TransactionChain();
                MetadataSet.Delete(ref chain);
                MapManager.ExecuteTransaction(chain);
            }
        }

        public bool SetName(string name, ref TransactionChain chain)
        {
            if (_name != name)
            {
                _name = name;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }

                return true;
            }

            return false;
        }

        public bool SetValue(string value, ref TransactionChain chain)
        {
            if (_value != value)
            {
                _value = value;

                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                }

                return true;
            }

            return false;
        }
    }
}
