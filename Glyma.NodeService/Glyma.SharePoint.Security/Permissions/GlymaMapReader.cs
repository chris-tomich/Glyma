using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Glyma.Security;

namespace Glyma.SharePoint.Security
{
    class GlymaMapReader : GlymaPermission
    {
        public GlymaMapReader(SecurityContextManager context)
        {
            SecurityContextManager = context;
        }

        public override string Name
        {
            get { return Constants.GLYMA_MAP_READERS_PERMISSION_LEVEL; }
        }

        public override string OldName
        {
            get
            {
                return Constants.GLYMA_MAP_READERS_OLD_PERMISSION_LEVEL;
            }
        }

        public override GlymaPermissionLevel PermissionLevel
        {
            get
            {
                return GlymaPermissionLevel.GlymaMapReader;
            }
        }
    }
}
