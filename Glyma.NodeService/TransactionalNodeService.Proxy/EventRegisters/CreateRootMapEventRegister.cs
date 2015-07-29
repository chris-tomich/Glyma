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
