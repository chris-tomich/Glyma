using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.Extensions.Security;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole
{
    public class PermissionGroupCollection : ObservableCollectionEx<PermissionGroup>
    {
        public string Name
        {
            get { return PermissionLevel.ToString(); }
        }

        public override string ToString()
        {
            return Name;
        }

        public PermissionLevel PermissionLevel { get; private set; }

        public PermissionGroupCollection(PermissionLevel permissionLevel, IEnumerable<PermissionGroup> template)
            : base(template)
        {
            PermissionLevel = permissionLevel;
        }

        public PermissionGroupCollection(PermissionLevel permissionLevel)
            : base()
        {
            PermissionLevel = permissionLevel;
        }
    }
}
