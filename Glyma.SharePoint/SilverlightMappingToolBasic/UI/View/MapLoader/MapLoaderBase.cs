using System;
using SilverlightMappingToolBasic.UI.SuperGraph.View.BreadcrumbSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.View.MapLoader
{
    public abstract class MapLoaderBase : IMapLoader
    {
        public event EventHandler MapLoadStarted;
        public event EventHandler MapLoadFailed;
        public event EventHandler MapLoadCompleted;

        protected Guid MapUid { get; private set; }
        protected Guid NodeUid { get; private set; }

        public IMapPreloader Preloader { get; private set; }
        public Guid DomainUid { get; private set; }
        public Guid Context { get; set; }

        public void Load()
        {
            LoadStarted();
            MapManager.QueryMapByIdCompleted.RegisterEvent(Context, OnQueryMapCompleted);
            MapManager.QueryMapByIdAsync(DomainUid, Context);
        }

        protected IMapManager MapManager
        {
            get
            {
                return Preloader.MapManager;
            }
        }

        protected ISuperGraphControllerContainer ControllerContainer
        {
            get
            {
                return Preloader.ControllerContainer;
            }
        }

        protected BreadcrumbBarControl Breadcrumbs
        {
            get
            {
                return Preloader.Breadcrumbs;
            }
        }

        protected MapLoaderBase(IMapPreloader preloader,
            Guid domainUid, Guid mapUid, Guid nodeUid)
        {
            Preloader = preloader;
            DomainUid = domainUid;
            MapUid = mapUid;
            NodeUid = nodeUid;
        }


        private void OnQueryMapCompleted(object sender, TransactionalNodeService.Proxy.NodesEventArgs e)
        {
            Preloader.InitialiseControllers();
            QueryCompleted(e);
        }

        public virtual void QueryCompleted(TransactionalNodeService.Proxy.NodesEventArgs e)
        {
            
        }

        protected void LoadStarted()
        {
            if (MapLoadStarted != null)
            {
                MapLoadStarted(this, null);
            }
        }

        protected void LoadCompleted()
        {
            if (MapLoadCompleted != null)
            {
                MapLoadCompleted(this, null);
            }
        }

        protected void LoadFailed()
        {
            SuperMessageBoxService.ShowError("Load Map","Oops, we are unable to find the map.", () =>
            {
                if (MapLoadFailed != null)
                {
                    MapLoadFailed(this, null);
                }
            });
        }
    }
}
