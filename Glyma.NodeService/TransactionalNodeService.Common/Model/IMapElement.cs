using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionalNodeService.Common.Model
{
    public interface IMapElement
    {
        Guid Id { get; set; }
        Guid TypeId { get; set; }
        Guid DomainId { get; set; }
        string OriginalId { get; set; }
        Dictionary<MetadataContext, Metadata> Metadata { get; set; }
    }
}