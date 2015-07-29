using System.IO;
using Glyma.UtilityService.Export.IBIS.Common.Control;
using Telerik.Windows.Documents.FormatProviders.Html;
using TransactionalNodeService.Proxy.Universal;

namespace Glyma.UtilityService.Export.IBIS.Html
{
    public class HtmlExportUtility : DocumentExportUtility
    {
        public HtmlExportUtility(IMapManager mapmanager)
            : base(mapmanager)
        {
            FileExtension = "html";
        }

        protected override void WriteToFile()
        {
            using (var pdfStream = File.Open(OutputFileLocation, FileMode.Create))
            {
                var htmlFormatProvider = new HtmlFormatProvider();
                htmlFormatProvider.Export(Document, pdfStream);
            }
        }
    }
}
