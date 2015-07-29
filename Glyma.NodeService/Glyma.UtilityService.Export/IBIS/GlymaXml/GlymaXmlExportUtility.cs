using System;
using System.ComponentModel;
using System.Linq;
using Glyma.UtilityService.Export.IBIS.Common.Control;
using Glyma.UtilityService.Export.IBIS.Compendium.Control;
using Glyma.UtilityService.Export.IBIS.Compendium.Model;
using TransactionalNodeService.Proxy.Universal;

namespace Glyma.UtilityService.Export.IBIS.GlymaXml
{
    public class GlymaXmlExportUtility : ExportUtility
    {
        private CompendiumXmlMap CompendiumXmlMap { get; set; }
        public GlymaXmlExportUtility(IMapManager mapManager): base(mapManager)
        {
            FileExtension = "xml";
        }

        protected override bool CreateFile()
        {
            return false;
        }

        protected override void OnContainerMapLoaded(INode node)
        {
            base.OnContainerMapLoaded(node);
            CompendiumXmlMap = new CompendiumXmlMap(RootMapId);
            var mapQuerier = new CompendiumMapQuerier(MapManager, CompendiumXmlMap, SelectedNodes, node);
            MapQueue.Add(node.Id, node);
            mapQuerier.QueryCompleted += MapQuerierOnQueryCompleted;
            mapQuerier.MapAdded += MapQuerierOnMapAdded;
            mapQuerier.Process();
        }

        protected override void ReadNextMap(INode nextMap)
        {
            var mapQuerier = new CompendiumMapQuerier(MapManager, CompendiumXmlMap, null, nextMap);
            mapQuerier.QueryCompleted += MapQuerierOnQueryCompleted;
            mapQuerier.MapAdded += MapQuerierOnMapAdded;
            mapQuerier.Process();
        }
    }
}
