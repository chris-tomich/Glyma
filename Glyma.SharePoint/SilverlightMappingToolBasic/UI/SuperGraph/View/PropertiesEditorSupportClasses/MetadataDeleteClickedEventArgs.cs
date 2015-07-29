using System;
using SilverlightMappingToolBasic.UI.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses
{
    public class MetadataDeleteClickedEventArgs : EventArgs
    {
        public IMetadata Metadata { get; set; }
    }
}
