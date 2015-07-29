using System;
using System.Windows;
using System.Collections.Generic;
using SilverlightMappingToolBasic.MappingService;
using System.ComponentModel;

namespace SilverlightMappingToolBasic
{
    public interface INodeProxy : IStorageElement, INotifyPropertyChanged
    {
        bool IsLoaded { get; }

        Guid Domain { get; }
        int TransclusionCount { get; }
        INodeTypeProxy NodeType { get; set; }
        DescriptorCollection Descriptors { get; set; }
        bool IsTransclusion { get; set; }
        Guid ParentMapNodeUid { get; }
        Guid[] ParentNodes { get; set; }
        Guid[] ChildNodes { get; set; }
        Dictionary<MetadataContext, SoapMetadata> Metadata { get; set; }
        SoapNode BaseSoapNode { get; }

        bool HasMetadata(MetadataContext key);
        SoapMetadata GetNodeMetadata(MetadataContext key);
        void SetNodeMetadata(MetadataContext key, string metadataValue, IMetadataTypeProxy metadataType);
        void DeleteNodeMetadata(MetadataContext key);
        void RenameNodeMetadata(MetadataContext key, string newName);
    }
}
