using System;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu.Interface
{
    public interface INodeContextMenu
    {
        event EventHandler<ShareWithTypeEventArgs> ShareWithClicked;
        event EventHandler CopyUrlClicked;
        event EventHandler MenuClosed;
    }
}
