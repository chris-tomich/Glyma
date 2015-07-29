using System;
using System.Collections.Generic;
using System.ComponentModel;
using SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses.Edit;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.ViewModel
{
    public interface INodeProperties: INotifyPropertyChanged, IEditableObject
    {
        IMapManager MapManager { get; }
        string Name { get; set; }
        string Description { get; set; }
        string SpokenBy { get; set; }
        string Note { get; set; }
        string Title { get; }
        bool RemoveMetadata(string key);

        string DescriptionUrl { get; set; }
        int? DescriptionWidth { get; set; }
        int? DescriptionHeight { get; set; }
        NodeDescriptionType DescriptionType { get; set; }

        UIMetadataCollection UIMetadata { get; }
        List<UpdateMetadataDetail> Updates { get; }
    }
}
