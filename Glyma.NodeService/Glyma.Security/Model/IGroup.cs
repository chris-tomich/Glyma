using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.Security.Model
{
    public interface IGroup
    {
        int GroupId { get; }
        int? RoleId { get; }
        string DisplayName { get; }
        int? SecurableContextId { get; }
    }
}
