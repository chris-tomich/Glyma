using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.RichTextSupportClasses;
using Telerik.Windows.Controls;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Interface
{
    public interface INodeProperiesControl : IMainpageControl
    {
        NodePropertiesDialog NodePropertiesDialog { get; }
        NodeTextInput NodeTextInput { get; }
    }
}
