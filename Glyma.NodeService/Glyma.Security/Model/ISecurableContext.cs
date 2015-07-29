using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.Security.Model
{
    public interface ISecurableContext
    {
        int SecurableContextId { get; }
        string SecurableContextName { get; }
        Guid? SecurableContextUid { get; }
    }
}
