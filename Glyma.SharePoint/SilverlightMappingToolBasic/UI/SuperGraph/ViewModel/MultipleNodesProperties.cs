using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses.Edit;
using SilverlightMappingToolBasic.UI.ViewModel;
using TransactionalNodeService.Proxy;


namespace SilverlightMappingToolBasic.UI.SuperGraph.ViewModel
{
    public class MultipleNodesProperties : ViewModelBase, INodeProperties
    {
        private List<NodeProperties> _nodesProperties;
        private string _name;
        private string _description;
        private NodeDescriptionType _descriptionType;
        private string _spokenBy;
        private string _note;
        private List<UpdateMetadataDetail> _updates;
        private UIMetadataCollection _uiMetadata;

        public IMapManager MapManager
        {
            get
            {
                if (NodeProperties.Count == 0)
                {
                    return null;
                }
                return NodeProperties[0].MapManager;
            }
        }

        public List<UpdateMetadataDetail> Updates
        {
            get
            {
                if (_updates == null)
                {
                    _updates = new List<UpdateMetadataDetail>();
                }
                return _updates;
            }
            private set
            {
                _updates = value;
            }
        }

        public List<NodeProperties> NodeProperties
        {
            get
            {
                if (_nodesProperties == null)
                {
                    _nodesProperties = new List<NodeProperties>();
                }
                return _nodesProperties;
            }
            set
            {
                _nodesProperties = value;
            }
        }

        public string Name
        {
            get
            {
                if (_name == null)
                {
                    _name = GetMetaDataValue("Name");
                }
                return _name;
            }
            set
            {
                //Don't allow the update of the Name metadata when multiple nodes are being editted at once
            }
        }

        public string Note
        {
            get
            {
                if (_note == null)
                {
                    _note = GetMetaDataValue("Note");
                }
                return _note;
            }
            set
            {
                if (_note != value)
                {
                    _note = value;
                    SetMetadataValue("Note", value);
                }
            }
        }

        public string Title
        {
            get
            {
                return "Editing Properties of Multiple Nodes";
            }
        }

        public string Description
        {
            get
            {
                if (_description == null)
                {
                    _description = GetMetaDataValue("Description.Content");
                }
                return _description;
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        SetMetadataValue("Description.Content", value);
                    }
                    else
                    {
                        RemoveMetadata("Description.Content");
                    }
                }
            }
        }

        public string SpokenBy
        {
            get
            {
                if (_spokenBy == null)
                {
                    _spokenBy = GetMetaDataValue("SpokenBy");
                }
                return _spokenBy;
            }
            set
            {
                if (_spokenBy != value)
                {
                    _spokenBy = value;
                    SetMetadataValue("SpokenBy", value);
                }
            }
        }

        public bool RemoveMetadata(string key)
        {
            var isRemoved = true;
            foreach (var node in NodeProperties)
            {
                if (!node.Metadata.Remove(key))
                {
                    isRemoved = false;
                }
            }
            return isRemoved;
        }


        //public MetadataCollection Metadata
        //{
        //    get
        //    {
        //        if (_metadata == null)
        //        {
        //            if (Nodes.Count == 0)
        //            {
        //                return null;
        //            }
        //            _metadata = new MetadataCollection(Nodes[0]);
        //            _metadata.CollectionChanged += MetadataOnCollectionChanged;
        //            _metadata.PropertyChanged += MetadataOnPropertyChanged;
        //            foreach (var key in Nodes[0].Metadata.Keys)
        //            {
        //                var metadataPlaceHolder = new MultipleNodeMetadata(key, Nodes[0].Metadata[key].Value, this, MapManager);
        //                _metadata.Add(key, metadataPlaceHolder);
        //            }
        //            if (Nodes.Count == 1)
        //            {
        //                return _metadata;
        //            }
        //            for (var i = 1; i < Nodes.Count; i++)
        //            {
        //                foreach (var key in Nodes[i].Metadata.Keys)
        //                {
        //                    if (_metadata.ContainsKey(key) && string.IsNullOrEmpty(_metadata[key].Value))
        //                    {
        //                        continue;
        //                    }

        //                    if (!_metadata.ContainsKey(key))
        //                    {
        //                        var item = new MultipleNodeMetadata(key, string.Empty, this, MapManager);
        //                        _metadata.Add(key, item);
        //                        break;
        //                    }

        //                    if (_metadata[key].Value != Nodes[i].Metadata[key].Value)
        //                    {
        //                        _metadata.Remove(key);
        //                        var item = new MultipleNodeMetadata(key, string.Empty, this, MapManager);
        //                        _metadata.Add(key, item);
        //                        break;
        //                    }
        //                }


        //                var keys = _metadata.Keys.ToArray();
        //                foreach (var key in keys)
        //                {
        //                    if (!Nodes[i].Metadata.ContainsKey(key))
        //                    {
        //                        _metadata.Remove(key);
        //                        var item = new MultipleNodeMetadata(key, string.Empty, this, MapManager);
        //                        _metadata.Add(key, item);
        //                    }
        //                }
        //            }
        //        }
        //        return _metadata;
        //    }
        //}

        public NodeDescriptionType DescriptionType {
            get
            {
                var descriptionType = NodeDescriptionType.None;
                if (NodeProperties.Count == 0)
                {
                    return descriptionType;
                }

                if (NodeProperties.Count > 0)
                {
                    descriptionType = NodeProperties[0].DescriptionType;
                }
                for (var i = 1; i < NodeProperties.Count; i++)
                {
                    if (descriptionType != NodeProperties[i].DescriptionType)
                    {
                        return NodeDescriptionType.None;
                    }
                }
                return descriptionType;
            }
            set
            {
                foreach (var node in NodeProperties)
                {
                    node.DescriptionType = value;
                }
            }
        }

        public UIMetadataCollection UIMetadata 
        {
            get
            {
                if (_uiMetadata == null)
                {
                    _uiMetadata = new UIMetadataCollection();
                }
                return _uiMetadata;
            }
            private set
            {
                _uiMetadata = value;
            }
        }

        public string DescriptionUrl
        {
            get
            {
                var value = string.Empty;
                if (NodeProperties.Count == 0)
                {
                    return value;
                }

                if (NodeProperties.Count > 0)
                {
                    value = NodeProperties[0].DescriptionUrl;
                }

                for (var i = 1; i < NodeProperties.Count; i++)
                {
                    if (value != NodeProperties[i].DescriptionUrl)
                    {
                        return string.Empty;
                    }
                }
                return value;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
                    {
                        string httpUrl = "http://" + value;
                        if (Uri.IsWellFormedUriString(httpUrl, UriKind.Absolute))
                        {
                            SetMetadataValue("Description.Url", httpUrl); //store the http://value version of the URI
                        }
                        else
                        {
                            SetMetadataValue("Description.Url", value); //store it even though it's not appearing as being well formed
                            throw new Exception("The URL entered appears to be invalid");
                        }
                        //foreach (var node in NodeProperties)
                        //{
                        //    node.DescriptionUrl = value;
                        //}
                    }
                    else
                    {
                        SetMetadataValue("Description.Url", value); //store it even though it's not appearing as being well formed
                    }
                }
                else
                {
                    RemoveMetadata("Description.Url");
                }
            }
        }

        public int? DescriptionWidth
        {
            get
            {
                if (NodeProperties.Count == 0)
                {
                    return 0;
                }
                int? value = 0;
                if (NodeProperties.Count > 0)
                {
                    value = NodeProperties[0].DescriptionWidth;
                }

                for (var i = 1; i < NodeProperties.Count; i++)
                {
                    if (value != NodeProperties[i].DescriptionWidth)
                    {
                        return 0;
                    }
                }
                return value;
            }
            set
            {
                foreach (var node in NodeProperties)
                {
                    node.DescriptionWidth = value;
                }
            }
        }

        public int? DescriptionHeight
        {
            get
            {
                if (NodeProperties.Count == 0)
                {
                    return 0;
                }
                int? value = 0;
                if (NodeProperties.Count > 0)
                {
                    value = NodeProperties[0].DescriptionHeight;
                }

                for (var i = 1; i < NodeProperties.Count; i++)
                {
                    if (value != NodeProperties[i].DescriptionHeight)
                    {
                        return 0;
                    }
                }
                return value;
            }
            set
            {
                foreach (var node in NodeProperties)
                {
                    node.DescriptionHeight = value;
                }
            }
        }

        public MultipleNodesProperties(List<NodeProperties> nodes)
        {
            NodeProperties = nodes;
            var count = 0;
            foreach (var node in nodes)
            {
                foreach (var uiMetadata in node.UIMetadata)
                {
                    if (!UIMetadata.ContainsKey(uiMetadata.Key))
                    {
                        if (count == 0)
                        {
                            UIMetadata.Add(uiMetadata.Key, uiMetadata.Value);
                        }
                        else
                        {
                            UIMetadata.Add(uiMetadata.Key, string.Empty);
                        }
                    }
                    else if (UIMetadata[uiMetadata.Key] != uiMetadata.Value)
                    {
                        UIMetadata[uiMetadata.Key] = string.Empty;
                    }
                }

                if (count > 0)
                {
                    var copy = UIMetadata.ToList();
                    foreach (var keyValuePair in copy)
                    {
                        if (!node.UIMetadata.ContainsKey(keyValuePair.Key))
                        {
                            UIMetadata[keyValuePair.Key] = string.Empty;
                        }
                    }
                }
                count ++;
            }
        }

        private string GetMetaDataValue(string key)
        {
            if (UIMetadata.ContainsKey(key))
            {
                return UIMetadata[key];
            }
            return string.Empty;
        }

        private void SetMetadataValue(string key, string value)
        {
            if (NodeProperties.Count == 0) return;
            foreach (var node in NodeProperties)
            {
                node.Updates.Add(new UpdateMetadataDetail { Type = UpdateMetadataType.AddOrUpdateValue, NewKey = key, NewValue = value });
                if (node.UIMetadata.ContainsKey(key))
                {
                    node.UIMetadata[key] = value;
                }
                else
                {
                    node.UIMetadata.Add(key,value);
                }
            }
        }

        void IEditableObject.BeginEdit()
        {
            foreach (var node in NodeProperties)
            {
                ((INodeProperties)node).BeginEdit();
            }
        }

        void IEditableObject.EndEdit()
        {
            foreach (var node in NodeProperties)
            {
                foreach (var updateMetadataDetail in Updates)
                {
                    node.Updates.Add(updateMetadataDetail);
                }
                ((INodeProperties)node).EndEdit();
            }
        }

        void IEditableObject.CancelEdit()
        {
            foreach (var node in NodeProperties)
            {
                ((INodeProperties)node).CancelEdit();
            }
            Updates.Clear();
        }
    }
}
