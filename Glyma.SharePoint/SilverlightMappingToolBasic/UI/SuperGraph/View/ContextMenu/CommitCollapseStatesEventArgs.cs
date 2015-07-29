using System;
using SilverlightMappingToolBasic.UI.Extensions.Security;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu
{
    public class CommitCollapseStatesEventArgs : EventArgs
    {
        public PermissionLevel Group { get; set; }
    }
}
