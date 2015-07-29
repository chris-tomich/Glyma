using System;
using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.SuperGraph.View.BreadcrumbSupportClasses;
using SilverlightMappingToolBasic.UI.View.MapLoader;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.View
{
    public interface IMapPreloader : IBreadcrumbLoader
    {
        event EventHandler ReadyToInitialiseControllers;
        event EventHandler LoadingStarted;
        event EventHandler MapLoadFailed;

        ISuperGraphControllerContainer ControllerContainer { get; }
        IMapManager MapManager { get; }
        BreadcrumbBarControl Breadcrumbs { get; }

        void InitialiseControllers();
        void Load(Guid domainId, Guid nodeId, Guid mapNodeId);
    }
}
