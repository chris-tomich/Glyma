using System.Collections.Generic;
using System.Windows.Controls;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow;
using SilverlightMappingToolBasic.UI.SuperGraph.View.CollapseSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses.Interface;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Selector
{
    public interface INodesSelector
    {
        List<ISelectableNode> NodeControls { get; }
        List<ArrowControl> ArrowControls { get; }
        bool IsMultiSelect { get; }
        INodeMotionHandler MultipleNodeMotionHandler { get; }

        bool HasNode(ISelectableNode nodeControl);
        void RemoveNode(ISelectableNode nodeControl);
        void AddNode(ISelectableNode nodeControl, bool isShowDetails = true);

        bool HasArrow(ArrowControl arrowControl);
        void AddArrow(ArrowControl arrowControl);
        void Clear();

        void DeleteArrows(UserControl sender = null);
        void DeleteNodes(UserControl sender = null);
        void Delete();

        bool HasInvisibleNodesSelected();
    }
}
