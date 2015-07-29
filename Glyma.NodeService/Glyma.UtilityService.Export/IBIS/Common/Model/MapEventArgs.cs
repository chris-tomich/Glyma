using System;
using Glyma.UtilityService.Export.IBIS.Common.Model.Interface;
using TransactionalNodeService.Proxy.Universal;

namespace Glyma.UtilityService.Export.IBIS.Common.Model
{
    public class MapEventArgs : EventArgs
    {
        public INode Map { get; set; }

        public IGlymaNode Container { get; set; }
    }
}
