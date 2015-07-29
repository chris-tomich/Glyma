using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections;
using System.Linq;
using Proxy = TransactionalNodeService.Proxy;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.ViewModel
{
    public class MetadataCollection : IDictionary<string, IMetadata>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private struct MetadataDictionaryPair
        {
            public int Index;
            public IMetadata Metadata;

            public static MetadataDictionaryPair Empty
            {
                get
                {
                    return new MetadataDictionaryPair() { Index = -1, Metadata = null };
                }
            }
        }

        private Dictionary<string, MetadataDictionaryPair> _metadata;

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private MetadataCollection()
            : base()
        {
        }

        public MetadataCollection(INode parentNode)
            : this()
        {
            ParentNode = parentNode;
        }


        private Dictionary<string, MetadataDictionaryPair> Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    _metadata = new Dictionary<string, MetadataDictionaryPair>();
                }

                return _metadata;
            }
        }


        private ICollection<KeyValuePair<string, MetadataDictionaryPair>> Collection
        {
            get
            {
                return Metadata;
            }
        }

        private IEnumerator<KeyValuePair<string, MetadataDictionaryPair>> Enumerator
        {
            get
            {
                return (IEnumerator<KeyValuePair<string, MetadataDictionaryPair>>)Metadata;
            }
        }

        protected INode ParentNode
        {
            get;
            set;
        }

        public Proxy.IMetadataSet Add(string name, string value, ref TransactionFramework.TransactionChain chain)
        {
            return Add(null, null, name, value, ref chain);
        }

        public Proxy.IMetadataSet Add(Proxy.IRelationship relationship, Proxy.ConnectionType connectionType, string name, string value, ref TransactionFramework.TransactionChain chain)
        {
            Proxy.IMetadataSet newMetadataSet = ParentNode.Proxy.Metadata.Add(relationship, connectionType, name, value, ref chain);

            if (!Metadata.ContainsKey(name))
            {
                IMetadata newMetadata = ParentNode.ViewModelMetadataFactory.CreateMetadata(newMetadataSet);

                Add(name, newMetadata);
                return newMetadata.MetadataSet;
            }
            return newMetadataSet;
        }

        public void Delete(string key, ref TransactionFramework.TransactionChain chain)
        {
            if (Metadata.ContainsKey(key))
            {
                Metadata[key].Metadata.MetadataSet.Delete(ref chain);
                Remove(key);
            }
        }

        #region IDictionary<string, IMetadata> Implementation
        public void Add(string key, IMetadata value)
        {
            int index;
            IMetadata oldMetadata = null;
            value.PropertyChanged += ValueOnPropertyChanged;
            if (Metadata.ContainsKey(key))
            {
                MetadataDictionaryPair metadataPair = Metadata[key];

                index = metadataPair.Index;
                oldMetadata = metadataPair.Metadata;

                
                if (key != value.Name)
                {
                    Metadata.Remove(key);
                    Metadata.Add(value.Name, metadataPair);
                }
                else
                {
                    metadataPair.Metadata = value;
                }
            }
            else
            {
                index = Metadata.Count; // An 'index' is zero-based. Count is not zero-based meaning it is already 1 higher than the last index.

                MetadataDictionaryPair metadataPair = new MetadataDictionaryPair();
                metadataPair.Index = index;
                metadataPair.Metadata = value;

                Metadata.Add(key, metadataPair);
            }

            if (CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs changeEventArgs;

                if (oldMetadata == null)
                {
                    changeEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index);
                }
                else
                {
                    changeEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldMetadata, index);
                }

                CollectionChanged(value, changeEventArgs);
            }
        }

        private void ValueOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                UpdateKeys();
            }
        }

        private void UpdateKeys()
        {
            var items = Metadata.ToList();
            foreach (var metadata in items)
            {
                if (metadata.Key != metadata.Value.Metadata.Name)
                {
                    var newMetadata = metadata.Value;
                    Metadata.Remove(metadata.Key);
                    Metadata.Add(newMetadata.Metadata.Name, newMetadata);
                }
            }
        }

        public bool ContainsKey(string key)
        {
            return Metadata.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get
            {
                return Metadata.Keys;
            }
        }

        public bool Remove(string key)
        {
            bool isRemoved = false;

            if (Metadata.ContainsKey(key))
            {
                var removedMetadataDictionaryPair = Metadata[key];
                var removedIndex = removedMetadataDictionaryPair.Index;
                var removedMetadata = removedMetadataDictionaryPair.Metadata;

                isRemoved = Metadata.Remove(key);

                if (CollectionChanged != null)
                {
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedMetadata, removedIndex);
                }
            }

            return isRemoved;
        }

        

        public bool TryGetValue(string key, out IMetadata value)
        {
            MetadataDictionaryPair metadataPair;

            bool gotValue = Metadata.TryGetValue(key, out metadataPair);

            value = metadataPair.Metadata;

            return gotValue;
        }

        public ICollection<IMetadata> Values
        {
            get
            {
                var metadataValues = from metadata in Metadata.Values select metadata.Metadata;

                return metadataValues.ToList();
            }
        }

        public IMetadata this[string key]
        {
            get
            {
                if (Metadata.ContainsKey(key))
                {
                    return Metadata[key].Metadata;
                }

                return null;
            }
            set
            {
                Add(key, value);
            }
        }
        #endregion

        #region ICollection<KeyValuePair<string, IMetadata>> Implementation
        void ICollection<KeyValuePair<string, IMetadata>>.Add(KeyValuePair<string, IMetadata> item)
        {
            Add(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<string, IMetadata>>.Clear()
        {
            Collection.Clear();

            if (CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs changeEventArgs;

                changeEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            }
        }

        bool ICollection<KeyValuePair<string, IMetadata>>.Contains(KeyValuePair<string, IMetadata> item)
        {
            if (ContainsKey(item.Key))
            {
                MetadataDictionaryPair metadataDictionaryPair = Metadata[item.Key];

                if (metadataDictionaryPair.Metadata == item.Value)
                {
                    return true;
                }
            }

            return false;
        }

        void ICollection<KeyValuePair<string, IMetadata>>.CopyTo(KeyValuePair<string, IMetadata>[] array, int arrayIndex)
        {
            int maxNumberOfItemsToCopy = array.Length - arrayIndex;

            if (Metadata.Count > maxNumberOfItemsToCopy)
            {
                throw new ArgumentException("This collection is too big for the given array.");
            }

            int currentIndex = arrayIndex;

            foreach (KeyValuePair<string, MetadataDictionaryPair> metadataDictionaryPair in Metadata)
            {
                array[currentIndex] = new KeyValuePair<string, IMetadata>(metadataDictionaryPair.Key, metadataDictionaryPair.Value.Metadata);
            }
        }

        int ICollection<KeyValuePair<string, IMetadata>>.Count
        {
            get
            {
                return Metadata.Count;
            }
        }

        bool ICollection<KeyValuePair<string, IMetadata>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool ICollection<KeyValuePair<string, IMetadata>>.Remove(KeyValuePair<string, IMetadata> item)
        {
            return Remove(item.Key);
        }
        #endregion

        #region IEnumerable Implementation and IEnumerable<KeyValuePair<string, IMetadata>> Implementation
        IEnumerator<KeyValuePair<string, IMetadata>> IEnumerable<KeyValuePair<string, IMetadata>>.GetEnumerator()
        {
            foreach (KeyValuePair<string, MetadataDictionaryPair> metadataDictionaryPair in Metadata)
            {
                yield return new KeyValuePair<string, IMetadata>(metadataDictionaryPair.Key, metadataDictionaryPair.Value.Metadata);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            IEnumerable<KeyValuePair<string, IMetadata>> genericEnumerator = this;

            return genericEnumerator.GetEnumerator();
        }
        #endregion
    }
}
