using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Interface
{
    public interface INodeControlContainerControl
    {
        bool HasVisibleNodes();
        IEnumerable<NodeControl> GetNodeControls();
        IEnumerable<NodeControl> GetVisibleNodeControls();
    }
}
