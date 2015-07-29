using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransactionalNodeService.Common.Model;

namespace TransactionalNodeService.Common
{
    public interface IQueryableMapObject
    {
        Guid ObjectId { get; }
        Guid TypeId { get; }
        Dictionary<Guid, Metadata> Metadata { get; }
        Dictionary<Guid, IQueryableMapObject> ConnectedObjects { get; }
    }
}