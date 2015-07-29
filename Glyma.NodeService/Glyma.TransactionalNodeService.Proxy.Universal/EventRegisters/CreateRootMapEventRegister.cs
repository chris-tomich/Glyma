using System;
using TransactionalNodeService.Proxy.Universal.EventArgs;

namespace TransactionalNodeService.Proxy.Universal.EventRegisters
{
    public class CreateRootMapEventRegister : EventRegister<Guid, NodesEventArgs>
    {
        public CreateRootMapEventRegister()
            : base()
        {
        }

        public void RegisterEvent(EventHandler<NodesEventArgs> CallbackEventhandler)
        {
            base.RegisterEvent(Guid.Empty, CallbackEventhandler);
        }

        public void FireEvent(object sender, NodesEventArgs eventArgs)
        {
            base.FireEvent(Guid.Empty, sender, eventArgs);
        }
    }
}
