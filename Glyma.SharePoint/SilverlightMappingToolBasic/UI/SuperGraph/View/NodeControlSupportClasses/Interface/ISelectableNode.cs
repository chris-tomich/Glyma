using System.Collections.Generic;
using System.Windows;
using SilverlightMappingToolBasic.UI.SuperGraph.View.CollapseSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses.Interface
{
    public interface ISelectableNode : IViewNode
    {
        void Select(bool isShowDetail = true);
        void DeSelect();
        void RefreshNodeControl();

        bool IsCollapsed { get; }
        FacadeNodeMotionHandler MotionHandler { get; }
        CollapseState CollapseState { get; }


        IEnumerable<NodeControl> GetAllChildNodeControls();
        IEnumerable<NodeControl> GetAllNodeControls();

        Point Centre { get; }

        void ChangeNodeType(ChangeNodeTypeEventArgs e);
        void Focus();
    }
}
