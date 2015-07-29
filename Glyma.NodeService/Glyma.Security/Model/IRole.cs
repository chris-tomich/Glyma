using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.Security.Model
{
    public interface IRole
    {
        int RoleId { get; }
        string DisplayName { get; }
        int? SecurableContextId { get; }

        bool HasRights(params IRight[] requiredRights);
    }
}
