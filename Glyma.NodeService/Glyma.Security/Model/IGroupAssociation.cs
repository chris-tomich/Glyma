using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.Security.Model
{
    public interface IGroupAssociation
    {
        long GroupAssociationId { get; }
        int? GroupId { get; }
        Guid? SecurableParentUid { get; }
        Guid? SecurableObjectUid { get; }
        int? SecurableContextId { get; }
        bool? BreakInheritance { get; }
    }
}
