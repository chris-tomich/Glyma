using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

using SilverlightMappingToolBasic.MappingService;

namespace SilverlightMappingToolBasic
{
    public class MetadataViewModel : INotifyPropertyChanged
    {
        private string _metadataName;
        private string _originalMetadataName;
        private Guid _nodeUid = Guid.Empty;
        private Guid _relationshipUid = Guid.Empty;
        private Guid _descriptorTypeUid = Guid.Empty;
        private Guid _metadataTypeUid = Guid.Empty;
        private string _metadataValue;

        public MetadataViewModel(MetadataContext context, SoapMetadata metadata)
        {
            if (context == null)
            {
                throw new ArgumentException("The metadata context was null.", "context");
            }
            if (metadata == null)
            {
                throw new ArgumentException("The metadata was null.", "metadata");
            }
            if (context.MetadataName != metadata.MetadataName)
            {
                throw new ArgumentException("The MetadataContext.MetadataName did not match the SoapMetadata.MetadataName, the context was not for this metadata.");
            }
            this._metadataName = context.MetadataName;
            this._originalMetadataName = context.MetadataName;
            if (context.NodeUid.HasValue)
            {
                this._nodeUid = context.NodeUid.Value;
            }
            if (context.RelationshipUid.HasValue)
            {
                this._relationshipUid = context.RelationshipUid.Value;
            }
            if (context.DescriptorTypeUid.HasValue)
            {
                this._descriptorTypeUid = context.DescriptorTypeUid.Value;
            }
            this._metadataTypeUid = metadata.MetadataType.Id;
            this._metadataValue = metadata.MetadataValue;
        }

        public string OriginalMetadataName
        {
            get
            {
                return _originalMetadataName;
            }
            set
            {
                _originalMetadataName = value;
            }
        }

        public string MetadataName
        {
            set
            {
                if (_metadataName != value)
                {
                    NotifyPropertyChanging("MetadataName");
                    _metadataName = value;
                    NotifyPropertyChanged("MetadataName");
                }
            }
            get
            {
                return _metadataName;
            }
        }

        public Guid NodeUid
        {
            set
            {
                if (_nodeUid != value)
                {
                    NotifyPropertyChanging("NodeUid");
                    _nodeUid = value;
                    NotifyPropertyChanged("NodeUid");
                }
            }
            get
            {
                return _nodeUid;
            }
        }

        public Guid RelationshipUid
        {
            set
            {
                if (_relationshipUid != value)
                {
                    NotifyPropertyChanging("RelationshipUid");
                    _relationshipUid = value;
                    NotifyPropertyChanged("RelationshipUid");
                }
            }
            get
            {
                return _relationshipUid;
            }
        }

        public Guid DescriptorTypeUid
        {
            set
            {
                if (_descriptorTypeUid != value)
                {
                    NotifyPropertyChanging("DescriptorTypeUid");
                    _descriptorTypeUid = value;
                    NotifyPropertyChanged("DescriptorTypeUid");
                }
            }
            get
            {
                return _descriptorTypeUid;
            }
        }

        public Guid MetadataTypeUid
        {
            set
            {
                if (_metadataTypeUid != value)
                {
                    NotifyPropertyChanging("MetadataTypeUid");
                    _metadataTypeUid = value;
                    NotifyPropertyChanged("MetadataTypeUid");
                }
            }
            get
            {
                return _metadataTypeUid;
            }
        }

        public string MetadataValue
        {
            set
            {
                if (_metadataValue != value)
                {
                    NotifyPropertyChanging("MetadataValue");
                    _metadataValue = value;
                    NotifyPropertyChanged("MetadataValue");
                }
            }
            get
            {
                return _metadataValue;
            }
        }

        public MetadataViewModel Clone()
        {
            return (MetadataViewModel)this.MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            MetadataViewModel compareTo = obj as MetadataViewModel;
            if (compareTo != null)
            {
                if (MetadataValue == compareTo.MetadataValue &&
                    MetadataName == compareTo.MetadataName &&
                    MetadataTypeUid == compareTo.MetadataTypeUid &&
                    this.NodeUid == compareTo.NodeUid &&
                    this.RelationshipUid == compareTo.RelationshipUid &&
                    this.DescriptorTypeUid == compareTo.DescriptorTypeUid)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public MetadataContext GetContext()
        {
            MetadataContext context = new MetadataContext();
            context.NodeUid = this.NodeUid;
            context.MetadataName = this.OriginalMetadataName;
            if (this.DescriptorTypeUid != Guid.Empty)
            {
                context.DescriptorTypeUid = this.DescriptorTypeUid;
            }
            if (this.RelationshipUid != Guid.Empty)
            {
                context.RelationshipUid = this.RelationshipUid;
            }
            return context;
        }

        public void UpdateName()
        {
            this.OriginalMetadataName = this.MetadataName;
        }

        #region INotifyPropertyChanged Members
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;
        #endregion
    }
}
