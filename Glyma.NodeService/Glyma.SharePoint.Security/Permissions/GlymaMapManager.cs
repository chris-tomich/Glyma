using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Glyma.Security;

namespace Glyma.SharePoint.Security
{
    class GlymaMapManager : GlymaPermission
    {
        public GlymaMapManager(SecurityContextManager context)
        {
            SecurityContextManager = context;
        }

        public override string Name
        {
            get { return Constants.GLYMA_MAP_MANAGERS_PERMISSION_LEVEL; }
        }

        public override GlymaPermissionLevel PermissionLevel
        {
            get
            {
                return GlymaPermissionLevel.GlymaMapManager;
            }
        }
    }
}
