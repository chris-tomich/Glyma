using System;

namespace TransactionalNodeService.Proxy.Universal.EventArgs
{
    public class DomainEventArgs : EventRegisterEventArgs
    {
        public DomainEventArgs()
            : base()
        {
        }

        public Guid Domain
        {
            get;
            set;
        }
    }
}
