using System.IO;
using Glyma.UtilityService.Export.IBIS.Common.Control;
using Telerik.Windows.Documents.FormatProviders.OpenXml.Docx;
using TransactionalNodeService.Proxy.Universal;

namespace Glyma.UtilityService.Export.IBIS.Word
{
    public class WordExportUtility : DocumentExportUtility
    {
        public WordExportUtility(IMapManager mapmanager)
            : base(mapmanager)
        {
            FileExtension = "docx";
        }

        protected override void WriteToFile()
        {
            using (var wordStream = File.Open(OutputFileLocation, FileMode.Create))
            {
                var docxFormatProvider = new DocxFormatProvider();
                docxFormatProvider.Export(Document, wordStream);
            }
        }
    }
}
