using System;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu.Interface
{
    public interface IAuthorNodeContextMenu : INodeContextMenu
    {
        event EventHandler CutClicked;
        event EventHandler CopyClicked;
        event EventHandler DeleteClicked;
        event EventHandler CloneClicked;
        event EventHandler RecordVideoStartPositionClicked;
        event EventHandler RecordVideoStopPositionClicked;
        event EventHandler NodePropertiesClicked;
        event EventHandler<ChangeNodeTypeEventArgs> ChangeNodeTypeClicked;
    }
}
