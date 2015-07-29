using System;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.View.MapLoader
{
    public class MapLoaderByMapUid : MapLoaderBase
    {
        public MapLoaderByMapUid(IMapPreloader mapPreloader, 
            Guid domainUid, Guid mapUid, Guid nodeUid)
            : base(mapPreloader, domainUid, mapUid, nodeUid)
        {
            Context = mapUid;
        }

        public override void QueryCompleted(TransactionalNodeService.Proxy.NodesEventArgs e)
        {
            var context = (Guid)e.Context;

            var isMapLoaded = false;

            if (context != Guid.Empty)
            {
                if (e.Nodes != null && e.Nodes.Values != null)
                {
                    foreach (var node in e.Nodes.Values)
                    {
                        if (node.Id == context)
                        {
                            if (node.NodeType == MapManager.NodeTypes["CompendiumMapNode"])
                            {
                                ControllerContainer.SuperGraphController.ChangeContextAsync(node);

                                var viewModelNode = new Node(MapManager);
                                viewModelNode.LoadNode(null, node);
                                var breadcrumb = new BreadcrumbItem(viewModelNode);
                                Breadcrumbs.BreadcrumbTrail.Add(breadcrumb);

                                Preloader.LoadParentBreadcrumb(node);
                                isMapLoaded = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (isMapLoaded)
            {
                LoadCompleted();
            }
            else
            {
                LoadFailed();
            }

            base.QueryCompleted(e);
        }
    }
}
