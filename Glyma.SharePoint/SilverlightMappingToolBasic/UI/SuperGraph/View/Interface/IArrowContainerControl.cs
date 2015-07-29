using SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Interface
{
    public interface IArrowContainerControl : IMapMoveControl
    {
        void Add(ArrowControl arrowControl);
        void Remove(ArrowControl arrowControl);

        bool IsArrowExist(NodeControl from, NodeControl to);
    }
}
