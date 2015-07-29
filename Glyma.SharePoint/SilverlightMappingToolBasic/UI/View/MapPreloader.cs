using System;
using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.View.MapLoader;
using TransactionalNodeService.Proxy;
using SuperGraphViewModel = SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using BreadcrumbView = SilverlightMappingToolBasic.UI.SuperGraph.View.BreadcrumbSupportClasses;

namespace SilverlightMappingToolBasic.UI.View
{
    public class MapPreloader : IMapPreloader
    {
        private bool _isInitialised;
        private IBreadcrumbLoader _breadcrumbLoader;
        private MapLoaderFactory _mapLoaderFactory;

        public event EventHandler ReadyToInitialiseControllers;
        public event EventHandler LoadingStarted;
        public event EventHandler MapLoadFailed;

        public MapPreloader(IMapManager mapManager, ISuperGraphControllerContainer superGraphControllerContainer, BreadcrumbView.BreadcrumbBarControl breadcrumbControl)
        {
            MapManager = mapManager;
            ControllerContainer = superGraphControllerContainer;
            Breadcrumbs = breadcrumbControl;
        }

        private IBreadcrumbLoader BreadcrumbLoader
        {
            get
            {
                if (_breadcrumbLoader == null)
                {
                    _breadcrumbLoader = new BreadcrumbLoader(MapManager, Breadcrumbs);
                }
                return _breadcrumbLoader;
            }
        }

        void IBreadcrumbLoader.LoadParentMaps(List<INode> maps, Guid mapUid)
        {
            BreadcrumbLoader.LoadParentMaps(maps, mapUid);
        }

        void IBreadcrumbLoader.LoadParentBreadcrumb(INode node, int index)
        {
            BreadcrumbLoader.LoadParentBreadcrumb(node, index);
        }

        private MapLoaderFactory MapLoaderFactory
        {
            get
            {
                if (_mapLoaderFactory == null)
                {
                    _mapLoaderFactory = new MapLoaderFactory(this);
                }
                return _mapLoaderFactory;
            }
        }

        public ISuperGraphControllerContainer ControllerContainer
        {
            get;
            private set;
        }

        public IMapManager MapManager
        {
            get;
            private set;
        }

        public BreadcrumbView.BreadcrumbBarControl Breadcrumbs
        {
            get;
            private set;
        }

        public void Load(Guid domainId, Guid nodeId, Guid mapId)
        {
            var mapLoader = MapLoaderFactory.CreateMapLoader(domainId, mapId, nodeId);
            mapLoader.MapLoadStarted += MapLoaderOnMapLoadStarted;
            mapLoader.MapLoadFailed += MapLoaderOnMapLoadFailed;
            mapLoader.Load();
        }

        private void MapLoaderOnMapLoadFailed(object sender, EventArgs e)
        {
            if (MapLoadFailed != null)
            {
                MapLoadFailed(sender, e);
            }
        }

        private void MapLoaderOnMapLoadStarted(object sender, EventArgs e)
        {
            if (LoadingStarted != null)
            {
                LoadingStarted(sender, e);
            }
        }

        public void InitialiseControllers()
        {
            if (!_isInitialised)
            {
                _isInitialised = true;

                if (ReadyToInitialiseControllers != null)
                {
                    ReadyToInitialiseControllers(this, new EventArgs());
                }
            }
        }
    }
}
