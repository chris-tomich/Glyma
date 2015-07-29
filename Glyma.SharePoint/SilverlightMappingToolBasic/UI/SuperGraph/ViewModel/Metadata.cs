using System;
using SilverlightMappingToolBasic.UI.ViewModel;
using TransactionalNodeService.Proxy;
using TransactionalNodeService.Service;
using TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.SuperGraph.ViewModel
{
    public class Metadata : ViewModelBase, IMetadata
    {
        public Metadata(IMetadataSet metadataSet, IMapManager mapManager)
        {
            MetadataSet = metadataSet;
            MapManager = mapManager;
        }

        public IMetadataSet MetadataSet
        {
            get;
            private set;
        }

        protected IMapManager MapManager
        {
            get;
            private set;
        }

        public void Delete()
        {
            var chain = new TransactionChain();
            MetadataSet.Delete(ref chain);
            MapManager.ExecuteTransaction(chain);
        }

        public string Name
        {
            get
            {
                return MetadataSet.Name;
            }
            set
            {
                var chain = new TransactionChain();
                
                if (SetName(value, ref chain))
                {
                    MapManager.ExecuteTransaction(chain);
                }
            }
        }

        public string Value
        {
            get
            {
                return MetadataSet.Value;
            }
            set
            {
                var chain = new TransactionChain();

                if (SetValue(value, ref chain))
                {
                    MapManager.ExecuteTransaction(chain);
                }
            }
        }

        public bool SetName(string name, ref TransactionChain chain)
        {
            if (MetadataSet.Name != name)
            {
                MetadataSet.Update(name, null, null, null, null, ref chain);

                OnNotifyPropertyChanged("Name");

                return true;
            }

            return false;
        }

        public bool SetValue(string value, ref TransactionChain chain)
        {
            if (MetadataSet.Value != value)
            {
                MetadataSet.Update(null, value, null, null, null, ref chain);

                OnNotifyPropertyChanged("Value");

                return true;
            }

            return false;
        }
    }
}
