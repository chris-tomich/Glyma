using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Glyma.Security;

namespace Glyma.SharePoint.Security
{
    public class GlymaPermissionFactory
    {
        private SecurityContextManager Context
        {
            get;
            set;
        }

        public GlymaPermissionFactory(SecurityContextManager context)
        {
            Context = context;
        }

        public IGlymaPermission CreateGlymaPermission(GlymaPermissionLevel permissionLevel)
        {
            IGlymaPermission permissionObject = null;
            switch (permissionLevel)
            {
                case GlymaPermissionLevel.GlymaSecurityManager:
                    permissionObject = new GlymaSecurityManager(Context);
                    break;
                case GlymaPermissionLevel.GlymaProjectManager:
                    permissionObject = new GlymaProjectManager(Context);
                    break;
                case GlymaPermissionLevel.GlymaMapManager:
                    permissionObject = new GlymaMapManager(Context);
                    break;
                case GlymaPermissionLevel.GlymaMapAuthor:
                    permissionObject = new GlymaMapAuthor(Context);
                    break;
                case GlymaPermissionLevel.GlymaMapReader:
                    permissionObject = new GlymaMapReader(Context);
                    break;
            }
            return permissionObject;
        }
    }
}
