using System;
using TransactionalNodeService.Proxy.Universal.EventArgs;

namespace TransactionalNodeService.Proxy.Universal.EventRegisters
{
    public class QueryDomainsEventRegister : EventRegister<Guid, NodesEventArgs>
    {
        public QueryDomainsEventRegister()
            : base()
        {
        }

        private new void RegisterEvent(Guid context, EventHandler<NodesEventArgs> AcquireRelationshipMetadataCompleted)
        {
            base.RegisterEvent(context, AcquireRelationshipMetadataCompleted);
        }

        public void RegisterEvent(EventHandler<NodesEventArgs> AcquireRelationshipMetadataCompleted)
        {
            this.RegisterEvent(Guid.Empty, AcquireRelationshipMetadataCompleted);
        }

        private new void FireEvent(Guid context, object sender, NodesEventArgs eventArgs)
        {
            base.FireEvent(context, sender, eventArgs);
        }

        public void FireEvent(object sender, NodesEventArgs eventArgs)
        {
            this.FireEvent(Guid.Empty, sender, eventArgs);
        }
    }
}
