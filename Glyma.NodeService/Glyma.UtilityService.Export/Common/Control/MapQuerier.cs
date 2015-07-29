using System;
using System.Collections.Generic;
using System.Linq;
using Glyma.UtilityService.Export.Common.Control.Interface;
using Glyma.UtilityService.Export.IBIS.Common.Model;
using TransactionalNodeService.Proxy.Universal;
using TransactionalNodeService.Proxy.Universal.EventArgs;
using TransactionalNodeService.Proxy.Universal.Types;

namespace Glyma.UtilityService.Export.Common.Control
{
    public abstract class MapQuerier : IMapQuerier
    {
        protected INode Map { get; private set; }
        protected IEnumerable<Guid> SelectedNodes { get; private set; }

        public event EventHandler<MapEventArgs> QueryCompleted;
        public event EventHandler<MapEventArgs> MapAdded; 

        public IMapManager MapManager { get; private set; }

        protected MapQuerier(IMapManager mapManager, INode map, IEnumerable<Guid> selectedNodes)
        {
            Map = map;
            SelectedNodes = selectedNodes;
            MapManager = mapManager;
        }

        public virtual void OnMapFound(INode map)
        {
            if (MapAdded != null)
            {
                MapAdded(this, new MapEventArgs { Map = map });
            }
        }

        public virtual void OnQueryCompleted(INode map)
        {
            if (QueryCompleted != null)
            {
                QueryCompleted(this, new MapEventArgs { Map = map});
            }
        }

        public bool IsIdSelected(Guid id)
        {
            if (SelectedNodes == null || !SelectedNodes.Any())
            {
                return true;
            }
            if (SelectedNodes.Contains(id))
            {
                return true;
            }
            return false;
        }

        //public virtual void OnQueryCompleted(GlymaMap map)
        //{
        //    if (QueryCompleted != null)
        //    {
        //        QueryCompleted(this, new MapEventArgs { Map = map.Proxy, Container = map});
        //    }
        //}

        public virtual void Process()
        {
            MapManager.QueryMapByNodeCompleted.RegisterEvent(Map, OnQueryMapCompleted, Map);
            MapManager.QueryMapByNodeAsync(Map);
        }

        protected abstract void OnNodeFound(INode node, IRelationship relationship);

        protected abstract void OnRelationshipFound(IRelationship relationship);

        private void OnQueryMapCompleted(object sender, NodesEventArgs e)
        {
            if (e.State != Map)
            {
                throw new Exception("The state of the event doesn't match with this controller.");
            }

            if (Map.Status != LoadState.Full)
            {
                throw new Exception("The relationships were not loaded for the context of this map.");
            }

            var contextRelationships = Map.Relationships;
            var mapChildrenRelationships = contextRelationships.FindRelationships(MapManager.ConnectionTypes["To"], MapManager.RelationshipTypes["MapContainerRelationship"]);

            foreach (var relationship in mapChildrenRelationships)
            {
                var allRelationshipNodes = relationship.Nodes;

                IEnumerable<INode> connectedNodes = allRelationshipNodes.FindNodes(MapManager.ConnectionTypes["From"], true);

                if (connectedNodes == null)
                {
                    continue;
                }

                foreach (var modelNode in connectedNodes)
                {
                    if (IsIdSelected(modelNode.Id))
                    {
                        OnNodeFound(modelNode, relationship);


                        if (modelNode.NodeType.Equals(MapManager.NodeTypes["CompendiumMapNode"]))
                        {
                            OnMapFound(modelNode);
                        }


                        IEnumerable<KeyValuePair<ConnectionType, IRelationship>> fromToRelationships = modelNode.Relationships.FindRelationships(MapManager.RelationshipTypes["FromToRelationship"]);
                        IEnumerable<KeyValuePair<ConnectionType, IRelationship>> transclusionRelationships = modelNode.Relationships.FindRelationships(MapManager.RelationshipTypes["TransclusionFromToRelationship"]);

                        foreach (KeyValuePair<ConnectionType, IRelationship> modelRelationshipPair in fromToRelationships)
                        {
                            OnRelationshipFound(modelRelationshipPair.Value);
                        }

                        foreach (KeyValuePair<ConnectionType, IRelationship> transclusionRelationshipPair in transclusionRelationships)
                        {
                            IEnumerable<INode> nodes = transclusionRelationshipPair.Value.Nodes.FindNodes(MapManager.ConnectionTypes["TransclusionMap"]);

                            if (nodes != null)
                            {
                                foreach (var node in nodes)
                                {
                                    if (node.Id == Map.Id)
                                    {
                                        OnRelationshipFound(transclusionRelationshipPair.Value);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                }
            }
            
            OnQueryCompleted(Map);
        }
    }
}
