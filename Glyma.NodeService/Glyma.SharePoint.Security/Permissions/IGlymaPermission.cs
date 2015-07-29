using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Glyma.Security;

namespace Glyma.SharePoint.Security
{
    public interface IGlymaPermission
    {
        string Name
        {
            get;
        }

        GlymaPermissionLevel PermissionLevel
        {
            get;
        }

        bool CurrentUserHasPermission();
    }
}
