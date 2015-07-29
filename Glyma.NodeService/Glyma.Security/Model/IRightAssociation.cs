using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.Security.Model
{
    public interface IRightAssociation
    {
        long RightAssociationId { get; }
        int? RightId { get; }
        int? RoleId { get; }
        int? SecurableContextId { get; }
    }
}
