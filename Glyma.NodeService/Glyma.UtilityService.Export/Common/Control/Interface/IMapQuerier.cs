using System;
using Glyma.UtilityService.Export.IBIS.Common.Model;
using TransactionalNodeService.Proxy.Universal;

namespace Glyma.UtilityService.Export.Common.Control.Interface
{
    public interface IMapQuerier
    {
        event EventHandler<MapEventArgs> QueryCompleted;
        event EventHandler<MapEventArgs> MapAdded;

        IMapManager MapManager { get; }

        void OnMapFound(INode map);

        void OnQueryCompleted(INode map);

        void Process();

        bool IsIdSelected(Guid id);
    }
}
