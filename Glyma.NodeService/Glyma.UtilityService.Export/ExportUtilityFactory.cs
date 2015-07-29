using Glyma.UtilityService.Common.Model;
using Glyma.UtilityService.Export.Common.Control.Interface;
using Glyma.UtilityService.Export.IBIS.Compendium;
using Glyma.UtilityService.Export.IBIS.GlymaXml;
using Glyma.UtilityService.Export.IBIS.Pdf;
using Glyma.UtilityService.Export.IBIS.Word;
using TransactionalNodeService.Proxy.Universal;

namespace Glyma.UtilityService.Export
{
    public class ExportUtilityFactory
    {
        private IMapManager MapManager { get; set; }

        public ExportUtilityFactory(IMapManager mapManager)
        {
            MapManager = mapManager;
        }

        public IExportUtility CreateExportUtility(MapType mapType, ExportType exportType)
        {
            IExportUtility exportUtil = null;
            switch (mapType)
            {
                case MapType.IBIS:
                    switch (exportType)
                    {
                        case ExportType.Compendium:
                            exportUtil = new CompendiumExportUtility(MapManager);
                            break;
                        case ExportType.GlymaXml:
                            exportUtil = new GlymaXmlExportUtility(MapManager);
                            break;
                        case ExportType.PDF:
                            exportUtil = new PdfExportUtility(MapManager);
                            break;
                        case ExportType.Word:
                            exportUtil = new WordExportUtility(MapManager);
                            break;
                    }
                    break;
                //TODO: Handle other map types with other export utilities.
            }
            
            return exportUtil;
        }
    }
}
