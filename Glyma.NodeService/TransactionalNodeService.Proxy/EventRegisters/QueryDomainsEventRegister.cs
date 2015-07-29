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
