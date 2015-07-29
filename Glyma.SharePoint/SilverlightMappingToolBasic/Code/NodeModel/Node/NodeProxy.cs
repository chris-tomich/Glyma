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
using SilverlightMappingToolBasic.MappingService;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;

namespace SilverlightMappingToolBasic
{
    public class NodeProxy : INodeProxy
    {
        private SoapNode _baseSoapNode = null;
        private INodeTypeProxy _nodeType = null;
        private Guid[] _parentNodes = null;
        private Guid[] _childNodes = null;
        private Dictionary<MetadataContext, SoapMetadata> _localProperties;
        private Guid _localId;
        private string _localName;

        public NodeProxy()
        {
            Descriptors = new DescriptorCollection();
            _localProperties = new Dictionary<MetadataContext, SoapMetadata>(); 
        }

        public NodeProxy(SoapNode node)
            : this()
        {
            BaseSoapNode = node;
        }

        public SoapNode BaseSoapNode
        {
            get
            {
                return _baseSoapNode;
            }
            protected set
            {
                _baseSoapNode = value;

                foreach (SoapRelationship relationship in _baseSoapNode.Relationships.Values)
                {
                    RelationshipProxy relationshipProxy = new RelationshipProxy(relationship);

                    foreach (DescriptorProxy descriptor in relationshipProxy.Descriptors)
                    {
                        if (descriptor.NodeId == this.Id)
                        {
                            descriptor.Node = this;
                            this.Descriptors.Add(descriptor);
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Format("Node: {0}, Id: {1}, Type: {2}", this.Name, this.Id, this.NodeType.Name);
        }

        #region INodeProxy Members

        public bool IsLoaded
        {
            get
            {
                return (BaseSoapNode != null);
            }
        }

        public Guid Domain
        {
            get
            {
                if (BaseSoapNode != null)
                {
                    return BaseSoapNode.Domain;
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }

        public INodeTypeProxy NodeType
        {
            get
            {
                if (_nodeType == null)
                {
                    _nodeType = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>().GetNodeType(BaseSoapNode.NodeType);
                }

                return _nodeType;
            }
            set
            {
                _nodeType = value;
                OnPropertyChanged("NodeType");
            }
        }

        public bool IsTransclusion
        {
            get;
            set;
        }

        public Guid ParentMapNodeUid
        {
            get
            {
                Guid mapNodeUid = Guid.Empty;
                if (Descriptors != null)
                {
                    foreach (DescriptorProxy descriptorProxy in this.Descriptors.GetByDescriptorTypeName("From"))
                    {
                        if (descriptorProxy.Relationship.RelationshipType.Name == "MapContainerRelationship")
                        {
                            foreach (IDescriptorProxy alternateDescriptor in descriptorProxy.Relationship.Descriptors.GetByDescriptorTypeName("To"))
                            {
                                mapNodeUid = alternateDescriptor.NodeId;
                                break;
                            }
                        }
                        if (mapNodeUid != Guid.Empty)
                        {
                            break;
                        }
                    }
                }
                return mapNodeUid;
            }
        }

        public Guid[] ParentNodes
        {
            get
            {
                List<Guid> nodes = new List<Guid>();
                if (Descriptors != null)
                {
                    foreach (DescriptorProxy descriptorProxy in this.Descriptors.GetByDescriptorTypeName("From"))
                    {
                        foreach (DescriptorProxy dp in descriptorProxy.Relationship.Descriptors.GetByDescriptorTypeName("To"))
                        {
                            nodes.Add(dp.NodeId);
                        }
                    }
                }
                ParentNodes = nodes.ToArray();
                return _parentNodes;
            }
            set
            {
                _parentNodes = value;
                OnPropertyChanged("ParentNodes");
            }
        }

        public Guid[] ChildNodes
        {
            get
            {
                List<Guid> nodes = new List<Guid>();
                if (Descriptors != null)
                {
                    foreach (DescriptorProxy descriptorProxy in this.Descriptors.GetByDescriptorTypeName("To"))
                    {
                        if (this.IsTransclusion && descriptorProxy.Relationship.RelationshipType.Name == "TransclusionRelationship")
                        {
                            foreach (DescriptorProxy dp in descriptorProxy.Relationship.Descriptors.GetByDescriptorTypeName("From"))
                            {
                                nodes.Add(dp.NodeId);
                            }
                        }
                        else if (!this.IsTransclusion && descriptorProxy.Relationship.RelationshipType.Name != "TransclusionRelationship")
                        {
                            foreach (DescriptorProxy dp in descriptorProxy.Relationship.Descriptors.GetByDescriptorTypeName("From"))
                            {
                                nodes.Add(dp.NodeId);
                            }
                        }
                    }
                }
                ChildNodes = nodes.ToArray();
                return _childNodes;
            }
            set
            {
                _childNodes = value;
                OnPropertyChanged("ChildNodes");
            }
        }

        public Dictionary<MetadataContext, SoapMetadata> Metadata
        {
            get
            {
                if (BaseSoapNode == null)
                {
                    return _localProperties;
                }
                else
                {
                    return BaseSoapNode.Metadata;
                }
            }
            set
            {
                if (BaseSoapNode == null)
                {
                    _localProperties = value;
                }
                else
                {
                    BaseSoapNode.Metadata = value;
                }
                OnPropertyChanged("Metadata");
            }
        }

        public DescriptorCollection Descriptors
        {
            get;
            set;
        }

        public void SetNodeMetadata(MetadataContext key, string metadataValue, IMetadataTypeProxy metadataType)
        {
            if (BaseSoapNode.Metadata != null)
            {
                if (HasMetadata(key))
                {
                    GetNodeMetadata(key).MetadataValue = metadataValue;
                }
                else
                {
                    BaseSoapNode.Metadata.Add(key, new SoapMetadata()
                    {
                        MetadataName = key.MetadataName,
                        MetadataType = new SoapMetadataType() { Id = metadataType.Id, Name = metadataType.Name },
                        MetadataValue = metadataValue
                    });
                }

                INodeService nodeService = IoC.IoCContainer.GetInjectionInstance().GetInstance<INodeService>();
                TypeManager typeManager = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>();
                IDescriptorTypeProxy descriptorType = null;
                if (key.DescriptorTypeUid.HasValue)
                {
                    descriptorType = typeManager.GetDescriptorType(key.DescriptorTypeUid.Value);
                }
                Guid relationshipId = Guid.Empty;
                if (key.RelationshipUid.HasValue)
                {
                    relationshipId = key.RelationshipUid.Value;
                }
                
                nodeService.UpdateNodeMetadataAsync(Domain, this.Id, relationshipId, descriptorType, key.MetadataName, metadataValue, metadataType);
            }
        }

        public void DeleteNodeMetadata(MetadataContext key)
        {
            if (HasMetadata(key))
            {
                INodeService nodeService = IoC.IoCContainer.GetInjectionInstance().GetInstance<INodeService>();
                nodeService.DeleteMetadata(key);

                RemoveMetadata(key); //assumes the service call works for speed of updating the data cached
            }
        }

        public void RenameNodeMetadata(MetadataContext key, string newName)
        {
            if (HasMetadata(key))
            {
                INodeService nodeService = IoC.IoCContainer.GetInjectionInstance().GetInstance<INodeService>();
                TypeManager typeManager = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>();
                IDescriptorTypeProxy descriptorType = null;
                if (key.DescriptorTypeUid.HasValue)
                {
                    descriptorType = typeManager.GetDescriptorType(key.DescriptorTypeUid.Value);
                }
                Guid relationshipId = Guid.Empty;
                if (key.RelationshipUid.HasValue)
                {
                    relationshipId = key.RelationshipUid.Value;
                }
                nodeService.RenameNodeMetadataAsync(Domain, Id, relationshipId, descriptorType, key.MetadataName, newName);
            }
        }

        private void RemoveMetadata(MetadataContext key)
        {
            if (HasMetadata(key))
            {
                MetadataContext actualKeyToRemove = null;
                foreach (MetadataContext contextKey in Metadata.Keys)
                {
                    if (contextKey.MetadataName == key.MetadataName)
                    {
                        if (!key.NodeUid.HasValue && !key.RelationshipUid.HasValue && !key.DescriptorTypeUid.HasValue)
                        {
                            actualKeyToRemove = contextKey;
                            break;
                        }
                        if (key.NodeUid.HasValue && key.RelationshipUid.HasValue && key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.NodeUid.Value == key.NodeUid.Value && contextKey.RelationshipUid.Value == key.RelationshipUid.Value
                                && contextKey.DescriptorTypeUid.Value == key.DescriptorTypeUid.Value)
                            {
                                actualKeyToRemove = contextKey;
                                break;
                            }
                        }
                        else if (!key.NodeUid.HasValue && key.RelationshipUid.HasValue && key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.RelationshipUid.Value == key.RelationshipUid.Value &&
                                contextKey.DescriptorTypeUid.Value == key.DescriptorTypeUid.Value)
                            {
                                actualKeyToRemove = contextKey;
                                break;
                            }
                        }
                        else if (key.NodeUid.HasValue && !key.RelationshipUid.HasValue && key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.NodeUid.Value == key.NodeUid.Value &&
                                contextKey.DescriptorTypeUid.Value == key.DescriptorTypeUid.Value)
                            {
                                actualKeyToRemove = contextKey;
                                break;
                            }
                        }
                        else if (key.NodeUid.HasValue && key.RelationshipUid.HasValue && !key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.NodeUid.Value == key.NodeUid.Value &&
                                contextKey.RelationshipUid.Value == key.RelationshipUid.Value)
                            {
                                actualKeyToRemove = contextKey;
                                break;
                            }
                        }
                        else if (key.NodeUid.HasValue && !key.RelationshipUid.HasValue && !key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.NodeUid.Value == key.NodeUid.Value)
                            {
                                actualKeyToRemove = contextKey;
                                break;
                            }
                        }
                        else if (!key.NodeUid.HasValue && key.RelationshipUid.HasValue && !key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.RelationshipUid.Value == key.RelationshipUid.Value)
                            {
                                actualKeyToRemove = contextKey;
                                break;
                            }
                        }
                        else if (!key.NodeUid.HasValue && !key.RelationshipUid.HasValue && key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.DescriptorTypeUid.Value == key.DescriptorTypeUid.Value)
                            {
                                actualKeyToRemove = contextKey;
                                break;
                            }
                        }
                    }
                }
                if (actualKeyToRemove != null)
                {
                    Metadata.Remove(actualKeyToRemove);
                }
            }
        }

        public SoapMetadata GetNodeMetadata(MetadataContext key)
        {
            SoapMetadata metadata = null;
            if (HasMetadata(key))
            {
                foreach (MetadataContext contextKey in Metadata.Keys)
                {
                    if (contextKey.MetadataName == key.MetadataName)
                    {
                        if (!key.NodeUid.HasValue && !key.RelationshipUid.HasValue && !key.DescriptorTypeUid.HasValue)
                        {
                            metadata = Metadata[contextKey];
                            break;
                        }
                        if (key.NodeUid.HasValue && key.RelationshipUid.HasValue && key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.NodeUid.Value == key.NodeUid.Value && contextKey.RelationshipUid.Value == key.RelationshipUid.Value
                                && contextKey.DescriptorTypeUid.Value == key.DescriptorTypeUid.Value)
                            {
                                metadata = Metadata[contextKey];
                                break;
                            }
                        }
                        else if (!key.NodeUid.HasValue && key.RelationshipUid.HasValue && key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.RelationshipUid.Value == key.RelationshipUid.Value &&
                                contextKey.DescriptorTypeUid.Value == key.DescriptorTypeUid.Value)
                            {
                                metadata = Metadata[contextKey];
                                break;
                            }
                        }
                        else if (key.NodeUid.HasValue && !key.RelationshipUid.HasValue && key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.NodeUid.Value == key.NodeUid.Value &&
                                contextKey.DescriptorTypeUid.Value == key.DescriptorTypeUid.Value)
                            {
                                metadata = Metadata[contextKey];
                                break;
                            }
                        }
                        else if (key.NodeUid.HasValue && key.RelationshipUid.HasValue && !key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.NodeUid.Value == key.NodeUid.Value &&
                                contextKey.RelationshipUid.Value == key.RelationshipUid.Value)
                            {
                                metadata = Metadata[contextKey];
                                break;
                            }
                        }
                        else if (key.NodeUid.HasValue && !key.RelationshipUid.HasValue && !key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.NodeUid.Value == key.NodeUid.Value)
                            {
                                metadata = Metadata[contextKey];
                                break;
                            }
                        }
                        else if (!key.NodeUid.HasValue && key.RelationshipUid.HasValue && !key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.RelationshipUid.Value == key.RelationshipUid.Value)
                            {
                                metadata = Metadata[contextKey];
                                break;
                            }
                        }
                        else if (!key.NodeUid.HasValue && !key.RelationshipUid.HasValue && key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.DescriptorTypeUid.Value == key.DescriptorTypeUid.Value)
                            {
                                metadata = Metadata[contextKey];
                                break;
                            }
                        }
                    }
                }
            }

            return metadata;
        }

        public bool HasMetadata(MetadataContext key)
        {
            bool result = false;
            if (key != null)
            {
                foreach (MetadataContext contextKey in Metadata.Keys)
                {
                    if (contextKey.MetadataName == key.MetadataName)
                    {
                        if (!key.NodeUid.HasValue && !key.RelationshipUid.HasValue && !key.DescriptorTypeUid.HasValue)
                        {
                            result = true;
                            break;
                        }
                        if (key.NodeUid.HasValue && key.RelationshipUid.HasValue && key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.NodeUid.Value == key.NodeUid.Value && contextKey.RelationshipUid.Value == key.RelationshipUid.Value
                                && contextKey.DescriptorTypeUid.Value == key.DescriptorTypeUid.Value)
                            {
                                result = true;
                                break;
                            }
                        }
                        else if (!key.NodeUid.HasValue && key.RelationshipUid.HasValue && key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.RelationshipUid.Value == key.RelationshipUid.Value &&
                                contextKey.DescriptorTypeUid.Value == key.DescriptorTypeUid.Value)
                            {
                                result = true;
                                break;
                            }
                        }
                        else if (key.NodeUid.HasValue && !key.RelationshipUid.HasValue && key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.NodeUid.Value == key.NodeUid.Value &&
                                contextKey.DescriptorTypeUid.Value == key.DescriptorTypeUid.Value)
                            {
                                result = true;
                                break;
                            }
                        }
                        else if (key.NodeUid.HasValue && key.RelationshipUid.HasValue && !key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.NodeUid.Value == key.NodeUid.Value &&
                                contextKey.RelationshipUid.Value == key.RelationshipUid.Value)
                            {
                                result = true;
                                break;
                            }
                        }
                        else if (key.NodeUid.HasValue && !key.RelationshipUid.HasValue && !key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.NodeUid.Value == key.NodeUid.Value)
                            {
                                result = true;
                                break;
                            }
                        }
                        else if (!key.NodeUid.HasValue && key.RelationshipUid.HasValue && !key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.RelationshipUid.Value == key.RelationshipUid.Value)
                            {
                                result = true;
                                break;
                            }
                        }
                        else if (!key.NodeUid.HasValue && !key.RelationshipUid.HasValue && key.DescriptorTypeUid.HasValue)
                        {
                            if (contextKey.DescriptorTypeUid.Value == key.DescriptorTypeUid.Value)
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                }
            }
            return result;
        }

        #endregion

        #region IStorageElement Members

        public Guid Id
        {
            get
            {
                if (BaseSoapNode == null)
                {
                    return _localId;
                }
                else
                {
                    return (Guid)BaseSoapNode.Id;
                }
            }
            set
            {
                if (BaseSoapNode == null)
                {
                    _localId = value;
                }
                else
                {
                    BaseSoapNode.Id = value;
                }
                OnPropertyChanged("Id");
            }
        }

        public string Name
        {
            get
            {
                if (BaseSoapNode == null)
                {
                    return _localName;
                }
                else
                {
                    MetadataContext key = new MetadataContext() { MetadataName = "Name", NodeUid = this.Id };
                    if (BaseSoapNode.Metadata != null && HasMetadata(key))
                    {
                        return GetNodeMetadata(key).MetadataValue;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
            set
            {
                if (BaseSoapNode == null)
                {
                    _localName = value;
                }
                else
                {
                    MetadataContext key = new MetadataContext() { MetadataName = "Name", NodeUid = this.Id };
                    MetadataTypeProxy stringType = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>().GetMetadataType("string") as MetadataTypeProxy;
                    SetNodeMetadata(key, value, stringType);
                }
                OnPropertyChanged("Name");
            }
        }

        public string CreatedBy
        {
            get
            {
                MetadataContext key = new MetadataContext() { MetadataName = "CreatedBy", NodeUid = this.Id };
                if (BaseSoapNode.Metadata != null && HasMetadata(key))
                {
                    return GetNodeMetadata(key).MetadataValue;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                MetadataContext key = new MetadataContext() { MetadataName = "CreatedBy", NodeUid = this.Id };
                MetadataTypeProxy stringType = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>().GetMetadataType("string") as MetadataTypeProxy;
                SetNodeMetadata(key, value, stringType);
                OnPropertyChanged("CreatedBy");
            }
        }

        public string LastModifiedBy
        {
            get
            {
                MetadataContext key = new MetadataContext() { MetadataName = "LastModifiedBy", NodeUid = this.Id };
                if (BaseSoapNode.Metadata != null && HasMetadata(key))
                {
                    return GetNodeMetadata(key).MetadataValue;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                MetadataContext key = new MetadataContext() { MetadataName = "LastModifiedBy", NodeUid = this.Id };
                MetadataTypeProxy stringType = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>().GetMetadataType("string") as MetadataTypeProxy;
                SetNodeMetadata(key, value, stringType);
                OnPropertyChanged("LastModifiedBy");
            }
        }

        public DateTime Created
        {
            get
            {
                MetadataContext key = new MetadataContext() { MetadataName = "Created", NodeUid = this.Id };
                if (BaseSoapNode.Metadata != null && HasMetadata(key))
                {
                    return DateTime.Parse(GetNodeMetadata(key).MetadataValue);
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
            set
            {
                MetadataContext key = new MetadataContext() { MetadataName = "Created", NodeUid = this.Id };
                MetadataTypeProxy dateTimeType = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>().GetMetadataType("datetime") as MetadataTypeProxy;
                //Store the DateTime in the UniversalSortableDateTimePattern
                SetNodeMetadata(key, value.ToString("u"), dateTimeType);
                OnPropertyChanged("Created");
            }
        }

        public DateTime LastModified
        {
            get
            {
                MetadataContext key = new MetadataContext() { MetadataName = "LastModified", NodeUid = this.Id };
                if (BaseSoapNode.Metadata != null && HasMetadata(key))
                {
                    return DateTime.Parse(GetNodeMetadata(key).MetadataValue);
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
            set
            {
                MetadataContext key = new MetadataContext() { MetadataName = "LastModified", NodeUid = this.Id };
                MetadataTypeProxy dateTimeType = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>().GetMetadataType("datetime") as MetadataTypeProxy;
                //Store the DateTime in the UniversalSortableDateTimePattern
                SetNodeMetadata(key, value.ToString("u"), dateTimeType);
                OnPropertyChanged("LastModified");
            }
        }

        public int TransclusionCount
        {
            get
            {
                List<Guid> transclusionMaps = new List<Guid>();

                foreach (IDescriptorProxy descriptor in Descriptors)
                {
                    if (descriptor.Relationship.RelationshipType.Name == "TransclusionRelationship")
                    {
                        foreach (IDescriptorProxy d in descriptor.Relationship.Descriptors.GetByDescriptorTypeName("TransclusionMap"))
                        {
                            if (!transclusionMaps.Contains(d.NodeId) && d.NodeId != this.ParentMapNodeUid)
                            {
                                transclusionMaps.Add(d.NodeId);
                            }
                        }
                    }
                }
                return transclusionMaps.Count + 1;
            }
        }

        public bool Equals(IStorageElement secondElement)
        {
            if (secondElement != null)
            {
                if (this.Id == secondElement.Id)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
