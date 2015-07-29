using System;

namespace SilverlightMappingToolBasic.UI.View.MapLoader
{
    public interface IMapLoader
    {
        event EventHandler MapLoadStarted;
        event EventHandler MapLoadFailed;
        event EventHandler MapLoadCompleted;

        Guid DomainUid { get; }
        Guid Context { get; }

        void Load();
        void QueryCompleted(TransactionalNodeService.Proxy.NodesEventArgs e);
    }
}
