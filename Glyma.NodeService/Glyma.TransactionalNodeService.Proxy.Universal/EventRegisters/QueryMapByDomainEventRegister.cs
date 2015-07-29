using System;
using TransactionalNodeService.Proxy.Universal.EventArgs;

namespace TransactionalNodeService.Proxy.Universal.EventRegisters
{
    public class QueryMapByDomainEventRegister : EventRegister<Guid, NodesEventArgs>
    {
        public QueryMapByDomainEventRegister()
            : base()
        {
        }
    }
}
