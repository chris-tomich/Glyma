using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using SilverlightMappingToolBasic.UI.ViewModel;
using SimpleIoC;
using TransactionalNodeService.Proxy;
using TransactionalNodeService.Service;
using TransactionalNodeService.Soap.TransactionFramework;
using MetadataCollection = SilverlightMappingToolBasic.UI.ViewModel.MetadataCollection;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses
{
    public sealed class DataGridPropertyCollection1 : IEnumerable
    {
        private ObservableCollection<IMetadata> _augmentedCollection;

        public event EventHandler MetadataUpdating;
        public event EventHandler MetadataUpdateCompleted;

        public DataGridPropertyCollection1(MetadataCollection metadataCollection)
        {
            MetadataCollection = metadataCollection;

            InitialiseAugementedCollection(MetadataCollection);
        }

        public void Remove(IMetadata item)
        {
            if (AugmentedCollection.Contains(item))
            {
                item.Delete();
                AugmentedCollection.Remove(item);
            }
        }

        private MetadataCollection MetadataCollection
        {
            get;
            set;
        }

        private IMapManager MapManager
        {
            get
            {
                return IoCContainer.GetInjectionInstance().GetInstance<IMapManager>();
            }
        }

        private ObservableCollection<IMetadata> AugmentedCollection
        {
            get
            {
                if (_augmentedCollection == null)
                {
                    _augmentedCollection = new ObservableCollection<IMetadata>();
                }

                return _augmentedCollection;
            }
        }

        private void AddNewLine()
        {
            var newDataGridPropertyPlaceholder = new NewDataGridPropertyPlaceholder();
            newDataGridPropertyPlaceholder.PropertyChanged += OnPlaceholderPropertyChanged;
            newDataGridPropertyPlaceholder.MapManager = MapManager;
            AugmentedCollection.Add(newDataGridPropertyPlaceholder);
        }

        private void InitialiseAugementedCollection(MetadataCollection metadataCollection)
        {
            foreach (var metadata in metadataCollection.Values)
            {
                if (metadata.Name != "Description")
                {
                    AugmentedCollection.Add(metadata);
                }
            }

            AddNewLine();
        }

        private void OnPlaceholderPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propertyPlaceholder = sender as NewDataGridPropertyPlaceholder;

            if (propertyPlaceholder == null )
            {
                return;
            }

            if (string.IsNullOrEmpty(propertyPlaceholder.Name.Trim()))
            {
                return;
            }

            if (propertyPlaceholder.IsNew && MetadataCollection.ContainsKey(propertyPlaceholder.Name))
            {
                throw new Exception("Duplicate Name Inserted");
            }
            

            if (MetadataUpdating != null)
            {
                MetadataUpdating(this, null);
            }
            var chain = new TransactionChain();
            chain.TransactionCompleted += OnTransactionCompleted;
            string propertyValue = "";
            if (!string.IsNullOrEmpty(propertyPlaceholder.Value))
            {
                propertyValue = propertyPlaceholder.Value;
            }

            var multipleNodeMetadata = MetadataCollection["Name"] as MultipleNodeMetadata;
            if (multipleNodeMetadata != null)
            {
                foreach (var node in multipleNodeMetadata.NodeContainer.NodeProperties)
                {
                    node.Metadata.Add(propertyPlaceholder.Name, propertyValue, ref chain);
                }
            }
            else
            {
                propertyPlaceholder.MetadataSet = MetadataCollection.Add(propertyPlaceholder.Name, propertyValue, ref chain);
            }

            MapManager.ExecuteTransaction(chain);

            if (propertyPlaceholder.IsNew)
            {
                propertyPlaceholder.IsNew = false;

                AddNewLine();
            }
            
        }

        private void OnTransactionCompleted(object sender, CompleteTransactionCompletedEventArgs completeTransactionCompletedEventArgs)
        {
            if (MetadataUpdateCompleted != null)
            {
                MetadataUpdateCompleted(sender, null);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return AugmentedCollection.GetEnumerator();
        }
    }
}
