using System;
using SilverlightMappingToolBasic.UI.Extensions.Security;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole
{
    public class PermissionValueChangedEventArgs : EventArgs
    {
        public PermissionGroup ChangedItem { get; set; }
        public PermissionLevel PermissionLevel { get; set; }
    }
}
