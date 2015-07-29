using System;
using System.Collections.Generic;
using Glyma.UtilityService.Export.Common.Control;
using Glyma.UtilityService.Export.IBIS.Common.Model.Glyma;
using TransactionalNodeService.Proxy.Universal;

namespace Glyma.UtilityService.Export.IBIS.Common.Control
{
    
    public class GlymaMapQuerier : MapQuerier
    {
        protected GlymaMap GlymaMap { get; private set; }

        public GlymaMapQuerier(IMapManager mapManager, INode map, IEnumerable<Guid> selectedNodes, GlymaMap glymaMap)
            : base(mapManager, map, selectedNodes)
        {
            GlymaMap = glymaMap;
        }

        protected override void OnNodeFound(INode node, IRelationship relationship)
        {
            var glymaNode = new GlymaNode(relationship, node);
            GlymaMap.AddNode(glymaNode);
        }

        protected override void OnRelationshipFound(IRelationship relationship)
        {
            GlymaMap.AddRelationship(new GlymaRelationship(relationship));
        }

        public override void OnQueryCompleted(INode map)
        {
            GlymaMap.Initialise();

            base.OnQueryCompleted(map);
        }


        //protected override void OnQueryMapCompleted(object sender, NodesEventArgs e)
        //{
        //    base.OnQueryMapCompleted(sender, e);

        //    var contextRelationships = Map.Relationships;
        //    var mapChildrenRelationships = contextRelationships.FindRelationships(MapManager.ConnectionTypes["To"], MapManager.RelationshipTypes["MapContainerRelationship"]);

        //    foreach (var relationship in mapChildrenRelationships)
        //    {
        //        var allRelationshipNodes = relationship.Nodes;

        //        IEnumerable<INode> connectedNodes = allRelationshipNodes.FindNodes(MapManager.ConnectionTypes["From"], true);

        //        if (connectedNodes == null)
        //        {
        //            continue;
        //        }

        //        foreach (var modelNode in connectedNodes)
        //        {
        //            if (IsIdSelected(modelNode.Id))
        //            {
        //                var glymaNode = new GlymaNode(relationship, modelNode);
        //                GlymaMap.AddNode(glymaNode);

        //                if (glymaNode.NodeType == MapManager.NodeTypes["CompendiumMapNode"])
        //                {
        //                    OnMapFound(modelNode);
        //                }


        //                IEnumerable<KeyValuePair<ConnectionType, IRelationship>> fromToRelationships = modelNode.Relationships.FindRelationships(MapManager.RelationshipTypes["FromToRelationship"]);
        //                IEnumerable<KeyValuePair<ConnectionType, IRelationship>> transclusionRelationships = modelNode.Relationships.FindRelationships(MapManager.RelationshipTypes["TransclusionFromToRelationship"]);

        //                foreach (KeyValuePair<ConnectionType, IRelationship> modelRelationshipPair in fromToRelationships)
        //                {
        //                    GlymaMap.AddRelationship(new GlymaRelationship(modelRelationshipPair.Value));
        //                }

        //                foreach (KeyValuePair<ConnectionType, IRelationship> transclusionRelationshipPair in transclusionRelationships)
        //                {
        //                    IEnumerable<INode> nodes = transclusionRelationshipPair.Value.Nodes.FindNodes(MapManager.ConnectionTypes["TransclusionMap"]);

        //                    if (nodes != null)
        //                    {
        //                        foreach (var node in nodes)
        //                        {
        //                            if (node.Id == Map.Id)
        //                            {
        //                                GlymaMap.AddRelationship(new GlymaRelationship(transclusionRelationshipPair.Value));
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
                    
        //        }
        //    }
        //    GlymaMap.Initialise();
        //    OnQueryCompleted(Map);
        //}
    }
}
