using System;
using TransactionalNodeService.Proxy.Universal.EventArgs;

namespace TransactionalNodeService.Proxy.Universal.EventRegisters
{
    public class DeleteEventRegister : EventRegister<Guid, EventRegisterEventArgs>
    {
        public DeleteEventRegister()
            : base()
        {
        }
    }
}
