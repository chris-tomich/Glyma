using System;

namespace TransactionalNodeService.Proxy.Universal.Types
{
    public interface IMapType
    {
        Guid Id { get; set; }
        string Name { get; set; }
    }
}
