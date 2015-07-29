using System;

namespace SilverlightMappingToolBasic.UI.Extensions.Security
{
    public class UserPermissionEventArgs : EventArgs
    {
        public UserPermissionEventArgs()
            : base()
        {
        }

        public PermissionLevel Permission
        {
            get;
            set;
        }
    }
}
