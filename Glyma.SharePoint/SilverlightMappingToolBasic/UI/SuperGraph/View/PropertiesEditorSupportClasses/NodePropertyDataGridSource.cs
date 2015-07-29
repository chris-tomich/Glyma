using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses.Edit;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses
{
    public class NodePropertyDataGridSource : IEnumerable
    {
        private ObservableCollection<UIMetadata> _augmentedCollection;

        public ObservableCollection<UIMetadata> AugmentedCollection
        {
            get
            {
                if (_augmentedCollection == null)
                {
                    _augmentedCollection = new ObservableCollection<UIMetadata>();
                }
                return _augmentedCollection;
            }
            set
            {
                _augmentedCollection = value;
            }
        }

        public NodePropertyDataGridSource(Dictionary<string,string> uiMetadata)
        {
            InitialiseAugementedCollection(uiMetadata);
        }

        private void InitialiseAugementedCollection(Dictionary<string, string> uiMetadata)
        {
            foreach (var keyPair in uiMetadata)
            {
#if DEBUG
                var metadata = new UIMetadata(keyPair.Key, keyPair.Value);
                metadata.NameChanged += MetadataOnNameChanged;
                AugmentedCollection.Add(metadata);
#else
                if (keyPair.Key != "Name" && keyPair.Key != "Description.Content" && keyPair.Key != "Description.Type"
                     && keyPair.Key != "Description.Url" && keyPair.Key != "Description.Width" && keyPair.Key != "Description.Height"  && keyPair.Key != "SpokenBy")
                {
                    var metadata = new UIMetadata(keyPair.Key, keyPair.Value);
                    metadata.NameChanged += MetadataOnNameChanged;
                    AugmentedCollection.Add(metadata);
                }
#endif

            }
            NewLine();
        }

        private void MetadataOnNameChanged(object sender, NameChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewText))
            {
                throw new ValidationException("Name Cannot be Empty");
            }

            if (AugmentedCollection.Count(q => q.Name == e.NewText.Trim()) > 1)
            {
                throw new ValidationException("Duplicate Name Found");
            }
        }

        public void NewLine()
        {
            var metadata = new UIMetadata("", " ", false);
            metadata.NameChanged += MetadataOnNameChanged;
            AugmentedCollection.Add(metadata);
        }

        public IEnumerator GetEnumerator()
        {
            return AugmentedCollection.GetEnumerator();
        }

        public void Remove(UIMetadata metadata)
        {
            if (AugmentedCollection.Contains(metadata))
            {
                metadata.NameChanged -= MetadataOnNameChanged;
                AugmentedCollection.Remove(metadata);
            }
        }
    }
}
