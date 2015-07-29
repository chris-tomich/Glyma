using System;
using System.Collections.Generic;
using System.Linq;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.View.MapLoader
{
    public class BreadcrumbLoader : IBreadcrumbLoader
    {
        private int _parentIndex;

        public BreadcrumbLoader(IMapManager mapManager,
            SuperGraph.View.BreadcrumbSupportClasses.BreadcrumbBarControl breadcrumbs)
        {
            MapManager = mapManager;
            Breadcrumbs = breadcrumbs;
        }

        public IMapManager MapManager
        {
            get;
            private set;
        }

        public SuperGraph.View.BreadcrumbSupportClasses.BreadcrumbBarControl Breadcrumbs
        {
            get;
            private set;
        }

        public void LoadParentBreadcrumb(INode node, int index = 0)
        {
            _parentIndex = index;
            MapManager.QueryMapByNodeCompleted.RegisterEvent(node, OnParentBreadcrumbQueryCompleted);
            MapManager.QueryMapByNodeAsync(node);
        }

        private void OnParentBreadcrumbQueryCompleted(object sender, TransactionalNodeService.Proxy.NodesEventArgs e)
        {
            var context = (INode)e.Context;

            if (context == null)
            {
                throw new ArgumentNullException("Context shouldn't be empty.");
            }

            if (e.Nodes != null && e.Nodes.Values != null)
            {
                foreach (INode node in e.Nodes.Values)
                {
                    if (node.Id == context.Id)
                    {
                        var maps = node.FindContainerMapsOfNode(MapManager);

                        if (maps.Count > 0)
                        {
                            LoadParentMaps(maps, Guid.Empty);
                        }
                        else
                        {
                            Breadcrumbs.BreadcrumbTrail.RootMapFound();
                        }
                        break;
                    }
                }
            }
        }

        public void LoadParentMaps(List<INode> maps, Guid mapId)
        {
            var viewModelMaps = new List<SuperGraph.ViewModel.Node>();

            foreach (var map in maps)
            {
                if (viewModelMaps.All(q => q.Proxy.Id != map.Id))
                {
                    var viewModelNode = new SuperGraph.ViewModel.Node(MapManager);
                    viewModelNode.LoadNode(null, map);

                    viewModelMaps.Add(viewModelNode);
                }
            }

            if (viewModelMaps.Count > 1)
            {
                var breadcrumb = new MultiBreadcrumbItem(viewModelMaps);
                if (mapId != Guid.Empty)
                {
                    var currentMap = breadcrumb.Items.FirstOrDefault(q => q.Node.Proxy.Id == mapId);
                    if (currentMap != null)
                    {
                        breadcrumb.SelectedBreadcrumb = currentMap;
                    }
                }
                Breadcrumbs.BreadcrumbTrail.Insert(breadcrumb, _parentIndex);
            }
            else if (viewModelMaps.Count == 1)
            {
                var breadcrumb = new BreadcrumbItem(viewModelMaps[0]);
                Breadcrumbs.BreadcrumbTrail.Insert(breadcrumb, _parentIndex);
            }
        }
    }
}
