using System;
using System.ComponentModel;
using System.Linq;
using Glyma.UtilityService.Export.IBIS.Common.Control;
using Glyma.UtilityService.Export.IBIS.Compendium.Control;
using Glyma.UtilityService.Export.IBIS.Compendium.Model;
using TransactionalNodeService.Proxy.Universal;

namespace Glyma.UtilityService.Export.IBIS.Compendium
{
    public class CompendiumExportUtility : Common.Control.ExportUtility
    {
        private CompendiumXmlMap CompendiumXmlMap { get; set; }

        public CompendiumExportUtility(IMapManager mapManager) : base(mapManager)
        {
            FileExtension = "xml";
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

        protected override bool CreateFile()
        {
            bool fileCreated = false;
            try 
            {
                CompendiumXmlMap.Create(OutputFileLocation);
                fileCreated = true;
            }
            catch (Exception ex)
            {
                OnExceptionRaised(this, "Error occurred when writing Compendium export to file.", ex);
            }
            return fileCreated;
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