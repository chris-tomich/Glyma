using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Glyma.Security;

namespace Glyma.SharePoint.Security
{
    class GlymaSecurityManager : GlymaPermission
    {
        public GlymaSecurityManager(SecurityContextManager context)
        {
            SecurityContextManager = context;
        }

        public override string Name
        {
            get { return Constants.GLYMA_SECURITY_MAGAGERS_PERMISSION_LEVEL; }
        }
        public override GlymaPermissionLevel PermissionLevel
        {
            get
            {
                return GlymaPermissionLevel.GlymaSecurityManager;
            }
        }

        public override bool CurrentUserHasPermission()
        {
            bool result = false;
            result = SecurityContextManager.CurrentUser.IsSiteAdmin;
            return result;
        }
    }
}
