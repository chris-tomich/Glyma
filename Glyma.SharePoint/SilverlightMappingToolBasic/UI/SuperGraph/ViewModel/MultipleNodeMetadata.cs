using System;
using System.ComponentModel;
using System.Linq;
using SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses;
using SilverlightMappingToolBasic.UI.ViewModel;
using TransactionalNodeService.Proxy;
using TransactionalNodeService.Service;
using TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.SuperGraph.ViewModel
{
    public class MultipleNodeMetadata : IMetadata
    {
        private string _name;
        private string _value;

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler MetadataUpdating;
        public event EventHandler MetadataUpdateCompleted;

        public MultipleNodeMetadata(string name, string value, MultipleNodesProperties nodeContainer,
            IMapManager mapManager)
        {
            NodeContainer = nodeContainer;
            MapManager = mapManager;
            _name = name;
            _value = value;
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

        protected IMapManager MapManager
        {
            get;
            private set;
        }

        public MultipleNodesProperties NodeContainer
        {
            get;
            private set;
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

        public void Delete()
        {
            var metadataCollection = NodeContainer.NodeProperties.Where(node => node.Metadata.ContainsKey(Name)).Select(node => node.Metadata[Name].MetadataSet).ToList();
            if (metadataCollection.Count > 0)
            {
                var chain = new TransactionChain();
                chain.TransactionCompleted += OnTransactionCompleted;
                foreach (var metadataSet in metadataCollection)
                {
                    metadataSet.Delete(ref chain);
                }
                MapManager.ExecuteTransaction(chain);
            }
        }

        private void OnTransactionCompleted(object sender, CompleteTransactionCompletedEventArgs e)
        {
            if (MetadataUpdateCompleted != null)
            {
                MetadataUpdateCompleted(sender, null);
            }
        }
    }
}
