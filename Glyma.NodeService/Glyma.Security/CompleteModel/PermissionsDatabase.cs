using Glyma.Security.Model;
using System;
using System.Collections.Generic;

namespace Glyma.Security.CompleteModel
{
    partial class Right : IRight
    {
    }

    partial class User : IUser
    {
        public bool IsAuthorised(Guid domainUid, params IRight[] requiredRights)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthorised(Guid? domainUid, Guid rootMapUid, params IRight[] requiredRights)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Guid> IsAuthorised(IEnumerable<Guid> domainUids, params IRight[] requiredRights)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Guid> IsAuthorised(Guid? domainUid, IEnumerable<Guid> rootMapUids, params IRight[] requiredRights)
        {
            throw new NotImplementedException();
        }


        public bool IsAuthorised(params IRight[] requiredRights)
        {
            throw new NotImplementedException();
        }
    }

    partial class RightAssociation : IRightAssociation
    {
    }

    partial class Membership : IMembership
    {
    }

    partial class SecurableContext : ISecurableContext
    {
    }

    partial class GroupAssociation : IGroupAssociation
    {
    }

    partial class Role : IRole
    {
        public bool HasRights(params IRight[] requiredRights)
        {
            throw new System.NotImplementedException();
        }
    }

    partial class Group : IGroup
    {
    }

    partial class GroupAssociation : IGroupAssociation
    {
    }
}
