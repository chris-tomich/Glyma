using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.Security.Model
{
    public interface IUser
    {
        int UserId { get; }
        string DisplayName { get; }
        string Claim { get; }
        int? SecurableContextId { get; }

        bool IsAuthorised(params IRight[] requiredRights);
        bool IsAuthorised(Guid domainUid, params IRight[] requiredRights);
        bool IsAuthorised(Guid? domainUid, Guid rootMapUid, params IRight[] requiredRights);
        IEnumerable<Guid> IsAuthorised(IEnumerable<Guid> domainUids, params IRight[] requiredRights);
        IEnumerable<Guid> IsAuthorised(Guid? domainUid, IEnumerable<Guid> rootMapUids, params IRight[] requiredRights);
    }
}
