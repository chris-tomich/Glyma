using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TransactionalNodeService.Proxy
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
