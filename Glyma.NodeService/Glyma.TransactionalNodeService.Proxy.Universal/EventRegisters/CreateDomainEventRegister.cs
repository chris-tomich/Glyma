using System;
using TransactionalNodeService.Proxy.Universal.EventArgs;

namespace TransactionalNodeService.Proxy.Universal.EventRegisters
{
    public class CreateDomainEventRegister : EventRegister<Guid, DomainEventArgs>
    {
        public CreateDomainEventRegister()
            : base()
        {
        }

        public void RegisterEvent(EventHandler<DomainEventArgs> CallbackEventhandler)
        {
            base.RegisterEvent(Guid.Empty, CallbackEventhandler);
        }

        public void FireEvent(object sender, DomainEventArgs eventArgs)
        {
            base.FireEvent(Guid.Empty, sender, eventArgs);
        }
    }
}
