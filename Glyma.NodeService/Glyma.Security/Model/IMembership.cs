using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.Security.Model
{
    public interface IMembership
    {
        long MembershipId { get; }
        int? GroupId { get; }
        int? UserId { get; }
        int? SecurableContextId { get; }
    }
}
