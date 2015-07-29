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

using TransactionalNodeService.Proxy;

namespace Glyma.UtilityService.Proxy
{
    public class IsExportingAvailableEventRegister : EventRegister<Guid, ResultEventArgs<bool>>
    {
        public IsExportingAvailableEventRegister()
            : base()
        {
        }

        public void RegisterEvent(EventHandler<ResultEventArgs<bool>> callbackHandler)
        {
            this.RegisterEvent(Guid.Empty, callbackHandler);
        }

        public void FireEvent(object sender, ResultEventArgs<bool> eventArgs)
        {
            this.FireEvent(Guid.Empty, sender, eventArgs);
        }
    }
}
