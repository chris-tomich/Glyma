using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Glyma.Security;

namespace Glyma.SharePoint.Security
{
    class GlymaProjectManager : GlymaPermission
    {
        public GlymaProjectManager(SecurityContextManager context)
        {
            SecurityContextManager = context;
        }

        public override string Name
        {
            get { return Constants.GLYMA_PROJECT_MANAGERS_PERMISSION_LEVEL; }
        }

        public override GlymaPermissionLevel PermissionLevel
        {
            get
            {
                return GlymaPermissionLevel.GlymaProjectManager;
            }
        }
    }
}
