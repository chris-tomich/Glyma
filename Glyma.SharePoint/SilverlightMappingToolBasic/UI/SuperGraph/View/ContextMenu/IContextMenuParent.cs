using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using SilverlightMappingToolBasic.UI.ViewModel;
using Relationship = SilverlightMappingToolBasic.UI.SuperGraph.ViewModel.Relationship;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu
{
    public interface IContextMenuParent : IMapMoveControl, IZoomControl
    {
        double ContainerHeight { get; }
        double ContainerWidth { get; }

        IEnumerable<NodeControl> GetNodeControls();

        IEnumerable<Relationship> GetRelationships();

        INode Context { get; }
    }
}
