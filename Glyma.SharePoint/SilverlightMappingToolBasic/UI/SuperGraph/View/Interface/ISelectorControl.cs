using System;
using System.Windows;
using System.Windows.Controls;
using SilverlightMappingToolBasic.UI.SuperGraph.Controller;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Selector;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Interface
{
    public interface ISelectorControl : IArrowControlContainerControl, INodeControlContainerControl, IMapMoveControl, IMapControl, INodeProperiesControl
    {
        INodesSelector NodesSelector { get; }
        MultiNodesContextMenu MultiNodesContextMenu { get; }
        AuthorNodeControlContextMenu AuthorNodeControlContextMenu { get; }
        ViewerNodeControlContextMenu ViewerNodeControlContextMenu { get; }
        ArrowContextMenu ArrowContextMenu { get; }
        Canvas NodeContainer { get; }

        NodeOperationDetails NodeOperationDetails { set; }
        Border DragSelectionBorder { get; }

        void SelectNodeByGuid(Guid id);
        void CentraliseMostImportantParent();

        void RecheckIncorrectVisibility();
        void ReScanForCollapseStates();

        Point LastLocation { get; }
    }
}
