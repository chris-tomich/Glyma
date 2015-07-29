using System;
using System.Linq;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.View.MapLoader
{
    public class MapLoaderByNodeUid : MapLoaderBase
    {
        public MapLoaderByNodeUid(IMapPreloader preloader, Guid domainUid, Guid mapUid, Guid nodeUid)
            : base(preloader, domainUid, mapUid, nodeUid)
        {
            Context = nodeUid;
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
                            if (node.RootMapId == context)
                            {
                                ControllerContainer.SuperGraphController.ChangeContextAsync(node);
                                var viewModelNode = new SuperGraph.ViewModel.Node(MapManager);
                                viewModelNode.LoadNode(null, node);
                                var breadcrumb = new BreadcrumbItem(viewModelNode);
                                Breadcrumbs.BreadcrumbTrail.Add(breadcrumb);
                                isMapLoaded = true;
                            }
                            else
                            {
                                var maps = node.FindContainerMapsOfNode(MapManager);

                                if (maps.Count > 0)
                                {
                                    INode firstMapLoaded;
                                    if (MapUid == Guid.Empty)
                                    {
                                        firstMapLoaded = maps.FirstOrDefault();
                                    }
                                    else
                                    {
                                        firstMapLoaded = maps.FirstOrDefault(q => q.Id == MapUid);
                                    }

                                    if (firstMapLoaded != null)
                                    {
                                        ControllerContainer.SuperGraphController.ChangeContextAsync(firstMapLoaded);

                                        var index = maps.IndexOf(firstMapLoaded);

                                        if (index > 0)
                                        {
                                            maps.RemoveAt(index);
                                            maps.Insert(0, firstMapLoaded);
                                        }

                                        Preloader.LoadParentMaps(maps, MapUid);

                                        Preloader.LoadParentBreadcrumb(firstMapLoaded);

                                        isMapLoaded = true;
                                        break;
                                    }
                                }
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
