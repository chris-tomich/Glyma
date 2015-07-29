using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses.Edit;
using SilverlightMappingToolBasic.UI.ViewModel;
using TransactionalNodeService.Proxy;
using TransactionalNodeService.Soap.TransactionFramework;
using MetadataCollection = SilverlightMappingToolBasic.UI.ViewModel.MetadataCollection;
using System.ComponentModel.DataAnnotations;

namespace SilverlightMappingToolBasic.UI.SuperGraph.ViewModel
{
    public class NodeProperties : ViewModelBase, INodeProperties
    {
        private readonly Node _node;
        private MetadataCollection _metadata;
        private UIMetadataCollection _uiMetadata;
        private List<UpdateMetadataDetail> _updates;
        private bool _hasNoneDescriptionType = false;

        private bool _isRated;

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

        public double Rate
        {
            get
            {
                var value = GetMetadataValue("Rate");
                double doubleValue;
                double.TryParse(value, out doubleValue);
                return doubleValue;
            }
            set
            {
                Update("Rate", value.ToString());
            }
        }

        public int RateCount
        {
            get
            {
                var value = GetMetadataValue("RateCount");
                int doubleValue;
                int.TryParse(value, out doubleValue);
                return doubleValue;
            }
            set
            {
                Update("RateCount", value.ToString());
            }
        }

        public double UIRate
        {
            get
            {
                return Rate;
            }
            set
            {
                Rate = (Rate*RateCount + value)/(RateCount + 1);
                RateCount ++;
                ((IEditableObject)this).EndEdit();
                IsRated = true;
                OnNotifyPropertyChanged("UIRate");
            }
        }

        public int Height
        {
            get
            {
                if (DescriptionHeight.HasValue)
                {
                    return DescriptionHeight.Value;
                }
                return 0;
            }
        }

        public int Width
        {
            get
            {
                if (DescriptionWidth.HasValue)
                {
                    return DescriptionWidth.Value;
                }
                return 0;
            }
        }

        public bool IsRated
        {
            get
            {
                return _isRated;
            }
            set
            {
                if (_isRated != value)
                {
                    _isRated = value;
                    OnNotifyPropertyChanged("IsRated");
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
        }

        public NodeDescriptionType DescriptionType
        {
            get
            {
                if (_hasNoneDescriptionType)
                {
                    return NodeDescriptionType.None;
                }
                else
                {
                    var value = GetMetadataValue("Description.Type");
                    NodeDescriptionType result;
                    if (Enum.TryParse(value, out result))
                    {
                        return result;
                    }
                    _hasNoneDescriptionType = true;
                    return NodeDescriptionType.None;
                }
            }
            set
            {
                if (value != NodeDescriptionType.None)
                {
                    _hasNoneDescriptionType = false;
                    Update("Description.Type", value.ToString());
                }
                else
                {
                    _hasNoneDescriptionType = true;
                }
            }
        }

        public IMapManager MapManager {
            get
            {
                if (_node != null)
                {
                    return _node.MapManager;
                }
                return IoC.IoCContainer.GetInjectionInstance().GetInstance<IMapManager>();
            }
        }

        public string Name
        {
            get
            {
                return GetMetadataValue("Name");
            }
            set
            {
                Update("Name", value);
            }
        }

        public string SpokenBy
        {
            get
            {
                return GetMetadataValue("SpokenBy");
            }
            set
            {
                Update("SpokenBy", value);
            }
        }

        public string Description
        {
            get
            {
                return GetMetadataValue("Description.Content");
            }
            set
            {
                if (UIMetadata.ContainsKey("Description.Content"))
                {
                    if (UIMetadata["Description.Content"] != value)
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            Update("Description.Content", value);
                        }
                        else
                        {
                            RemoveMetadata("Description.Content");
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(value))
                {
                    Update("Description.Content", value);
                }
            }
        }

        public string Note
        {
            get
            {
                return GetMetadataValue("Note");
            }
            set
            {
                Update("Note", value);
            }
        }
        
        public string DescriptionUrl
        {
            get
            {
                return GetMetadataValue("Description.Url");
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    //we can only accept absolute URL's, check if it's valid
                    if (!Uri.IsWellFormedUriString(value, UriKind.Absolute))
                    {
                        //if the Url wasn't valid try prepend the HTTP scheme and test it again before storing it
                        string httpUrl = "http://" + value;
                        if (Uri.IsWellFormedUriString(httpUrl, UriKind.Absolute))
                        {
                            Update("Description.Url", httpUrl); //store the http://value version of the URI
                        }
                        else
                        {
                            Update("Description.Url", value); //store it even though it's not appearing as being well formed
                            throw new Exception("The URL entered appears to be invalid");
                        }
                    }
                    else
                    {
                        Update("Description.Url", value); //appears to be a well formed Uri so store it
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
                return GetIntMetadataValue("Description.Width");
            }
            set
            {
                if (value.HasValue)
                {
                    Update("Description.Width", value.Value <= 400 ? 400 : value.Value);
                }
                else
                {
                    RemoveMetadata("Description.Width");
                }
            }
        }

        public int? DescriptionHeight
        {
            get
            {
                return GetIntMetadataValue("Description.Height");
            }
            set
            {
                if (value.HasValue)
                {
                    Update("Description.Height", value.Value <= 20 ? 20 : value.Value);
                }
                else
                {
                    RemoveMetadata("Description.Height");
                }
            }
        }

        public string Title
        {
            get
            {
                return string.Format("Editing Properties of '{0}'", Name);
            }
        }

        public MetadataCollection Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    _metadata = new MetadataCollection(_node);
                }

                return _metadata;
            }
            set
            {
                _metadata = value;
            }
        }

        public NodeProperties(Node node)
        {
            _node = node;
        }

        public NodeProperties(TransactionalNodeService.Proxy.INode node)
        {
            _node = new Node(node);
        }

        private void Update(string key, string value)
        {
            Updates.Add(new UpdateMetadataDetail { Type = UpdateMetadataType.AddOrUpdateValue, NewKey = key, NewValue = value });
        }

        private void Update(string key, int value)
        {
            Update(key, value.ToString(CultureInfo.InvariantCulture));
        }

        private string GetMetadataValue(string key)
        {
            if (UIMetadata.ContainsKey(key))
            {
                return UIMetadata[key];
            }
            return string.Empty;
        }

        private int? GetIntMetadataValue(string key)
        {
            if (UIMetadata.ContainsKey(key))
            {
                var stringValue = UIMetadata[key];
                int result;

                if (int.TryParse(stringValue, out result))
                {
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// Compacts the Updates list so that when multiple operations exist for a single    
        /// piece of metadata only the last operation is left in the Updates list.
        /// </summary>
        private void CompactUpdates()
        {
            //Iterate through the Updates list in reverse building a list of updates that
            //can be removed as they were going to be overriden by latter updates.
            List<string> uniqueMetaDataUpdateKeys = new List<string>();
            List<UpdateMetadataDetail> updatesToRemove = new List<UpdateMetadataDetail>();
            for (int i = this.Updates.Count - 1; i >= 0; i--)
            {
                UpdateMetadataDetail umd = this.Updates[i];
                if (!uniqueMetaDataUpdateKeys.Contains(umd.NewKey))
                {
                    uniqueMetaDataUpdateKeys.Add(umd.NewKey);
                }
                else
                {
                    updatesToRemove.Add(umd);
                }
            }

            //Remove the no longer relevant updates
            foreach (UpdateMetadataDetail umd in updatesToRemove)
            {
                this.Updates.Remove(umd);
            }
        }

        public bool RemoveMetadata(string key)
        {
            Updates.Add(new UpdateMetadataDetail { Type = UpdateMetadataType.Delete, NewKey = key });
            return true;
        }

        #region IEditableObject Methods

        void IEditableObject.BeginEdit()
        {

        }

        void IEditableObject.EndEdit()
        {
            if (Updates.Count > 0)
            {
                //removes any operations that act on the same metadata key, only the last update is added to the chain
                CompactUpdates(); 

                var chain = new TransactionChain();
                foreach (var detail in Updates)
                {
                    switch (detail.Type)
                    {
                        case UpdateMetadataType.AddOrUpdateValue:
                            Metadata.Add(detail.NewKey, detail.NewValue, ref chain);
                            if (UIMetadata.ContainsKey(detail.NewKey))
                            {
                                UIMetadata[detail.NewKey] = detail.NewValue;
                            }
                            else
                            {
                                UIMetadata.Add(detail.NewKey, detail.NewValue);
                            }
                            OnNotifyPropertyChanged(detail.NewKey);
                            break;
                        case UpdateMetadataType.Delete:
                            if (Metadata.ContainsKey(detail.NewKey))
                            {

                                Metadata[detail.NewKey].MetadataSet.Delete(ref chain);
                                Metadata.Remove(detail.NewKey);
                            }

                            if (UIMetadata.ContainsKey(detail.NewKey))
                            {
                                UIMetadata.Remove(detail.NewKey);
                            }
                            break;
                        case UpdateMetadataType.UpdateKey:
                            if (Metadata.ContainsKey(detail.Key))
                            {
                                var metadataSet = Metadata[detail.Key];
                                metadataSet.SetName(detail.NewKey, ref chain);
                                if (!metadataSet.Value.Equals(detail.NewValue))
                                {
                                    metadataSet.SetValue(detail.NewValue, ref chain);
                                }
                                Metadata.Remove(detail.Key);
                                Metadata.Add(detail.NewKey, metadataSet);
                                UIMetadata.Remove(detail.Key);
                                UIMetadata.Add(detail.NewKey, detail.NewValue);
                            }
                            break;
                    }

                }
                MapManager.ExecuteTransaction(chain);
                Updates = new List<UpdateMetadataDetail>();
            }
        }

        

        void IEditableObject.CancelEdit()
        {
            Updates.Clear();
            UIMetadata.Clear();
            foreach (var metadata in Metadata)
            {
                UIMetadata.Add(metadata.Key,metadata.Value.Value);
            }
        }

        #endregion
    }
}
