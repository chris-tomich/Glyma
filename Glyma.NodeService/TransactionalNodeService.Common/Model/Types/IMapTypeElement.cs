using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionalNodeService.Common.Model
{
    public interface IMapTypeElement : IPersistableSessionObject
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }
}