using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Glyma.Security;

namespace Glyma.SharePoint.Security
{
    class GlymaMapAuthor : GlymaPermission
    {
        public GlymaMapAuthor(SecurityContextManager context)
        {
            SecurityContextManager = context;
        }

        public override string Name
        {
            get { return Constants.GLYMA_MAP_AUTHORS_PERMISSION_LEVEL; }
        }

        public override string OldName
        {
            get
            {
                return Constants.GLYMA_MAP_AUTHORS_OLD_PERMISSION_LEVEL;
            }
        }

        public override GlymaPermissionLevel PermissionLevel
        {
            get
            {
                return GlymaPermissionLevel.GlymaMapAuthor;
            }
        }
    }
}
