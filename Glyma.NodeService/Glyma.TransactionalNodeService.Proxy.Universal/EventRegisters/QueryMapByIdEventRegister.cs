using System;
using TransactionalNodeService.Proxy.Universal.EventArgs;

namespace TransactionalNodeService.Proxy.Universal.EventRegisters
{
    public class QueryMapByIdEventRegister : EventRegister<Guid, NodesEventArgs>
    {
        public QueryMapByIdEventRegister()
            : base()
        {
        }
    }
}
