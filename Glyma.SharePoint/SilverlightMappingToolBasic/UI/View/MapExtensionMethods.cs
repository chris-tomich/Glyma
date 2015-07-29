using System.Collections.Generic;
using System.Linq;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.View
{
    public static class MapExtensionMethods
    {
        public static List<INode> FindContainerMapsOfNode(this INode node, IMapManager mapManager)
        {
            var output = new List<INode>();
            var relationships = node.Relationships.FindRelationships(mapManager.ConnectionTypes["From"], mapManager.RelationshipTypes["MapContainerRelationship"]);
            foreach (var relationship in relationships)
            {
                var maps = relationship.Nodes.FindNodes(mapManager.ConnectionTypes["To"]);
                if (maps != null)
                {
                    var map = maps.FirstOrDefault();
                    if (map != null && map.NodeType == mapManager.NodeTypes["CompendiumMapNode"] && map.Id != node.Id)
                    {
                        output.Add(map);
                    }
                }
            }
            return output;
        }
    }
}
