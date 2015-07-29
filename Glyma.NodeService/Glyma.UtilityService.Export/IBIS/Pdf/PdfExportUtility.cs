using System.IO;
using Glyma.UtilityService.Export.IBIS.Common.Control;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using TransactionalNodeService.Proxy.Universal;

namespace Glyma.UtilityService.Export.IBIS.Pdf
{
    public class PdfExportUtility : DocumentExportUtility
    {
        public PdfExportUtility(IMapManager mapmanager) : base(mapmanager)
        {
            FileExtension = "pdf";
        }

        protected override void WriteToFile()
        {
            using (var pdfStream = File.Open(OutputFileLocation, FileMode.Create))
            {
                var pdfFormatProvider = new PdfFormatProvider();

                var pdfExportSettings = new PdfExportSettings();
                pdfExportSettings.ImagesCompressionMode = PdfImagesCompressionMode.Automatic;
                pdfExportSettings.ImagesDeflaterCompressionLevel = -1;
                pdfExportSettings.ContentsCompressionMode = PdfContentsCompressionMode.Automatic;
                var pdfDocInfo = new PdfDocumentInfo();
                pdfDocInfo.Title = this.RootMap.Name;
                pdfDocInfo.Subject = this.RootMap.Name;
                pdfDocInfo.Producer = "Glyma Export";
                pdfExportSettings.DocumentInfo = pdfDocInfo;
                pdfFormatProvider.ExportSettings = pdfExportSettings;
                pdfFormatProvider.Export(Document, pdfStream);
            }
        }
    }
}
