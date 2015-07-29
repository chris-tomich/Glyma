using System;
using System.Collections.Generic;
using Glyma.UtilityService.Export.Common.Control;
using Glyma.UtilityService.Export.IBIS.Common.Control;
using Glyma.UtilityService.Export.IBIS.Common.Model.Glyma;
using Glyma.UtilityService.Export.IBIS.Compendium.Model;
using TransactionalNodeService.Proxy.Universal;

namespace Glyma.UtilityService.Export.IBIS.Compendium.Control
{
    public class CompendiumMapQuerier : MapQuerier
    {
        protected CompendiumXmlMap CompendiumXmlMap { get; private set; }

        public CompendiumMapQuerier(IMapManager mapManager, CompendiumXmlMap compendiumXmlMap, IEnumerable<Guid> selectedNodes, INode map)
            : base(mapManager, map, selectedNodes)
        {
            CompendiumXmlMap = compendiumXmlMap;
        }

        protected override void OnNodeFound(INode node, IRelationship relationship)
        {
            var glymaNode = new CompendiumNode(relationship, node);
            CompendiumXmlMap.AddView(Map.Id, glymaNode);
            CompendiumXmlMap.AddNode(glymaNode);


            if (glymaNode.ReferenceNode != null)
            {
                CompendiumXmlMap.AddView(Map.Id, glymaNode.ReferenceNode);
                CompendiumXmlMap.AddNode(glymaNode.ReferenceNode);
                CompendiumXmlMap.AddReferenceLink(Map.Id, glymaNode.ReferenceNode, glymaNode);
            }
        }

        protected override void OnRelationshipFound(IRelationship relationship)
        {
            CompendiumXmlMap.AddLink(Map.Id, new GlymaRelationship(relationship));
        }
    }
}
