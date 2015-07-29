using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Glyma.SharePoint.Common
{
    public static class GlymaColumns
    {
        public static readonly string ColumnGroupName = "Glyma";
        public static readonly ReadOnlyColumnDetails ExportStatus;
        public static readonly ReadOnlyColumnDetails ExportType;
        public static readonly ReadOnlyColumnDetails MapType;
        public static readonly ReadOnlyColumnDetails PercentageComplete;
        public static readonly ReadOnlyColumnDetails RootMapUid;
        public static readonly ReadOnlyColumnDetails DomainUid;
        public static readonly ReadOnlyColumnDetails LogMessages;
        public static readonly ReadOnlyColumnDetails ExportProperties;

        static GlymaColumns()
        {
            ExportStatus = new ReadOnlyColumnDetails(new Guid("{E4097649-F580-43B4-8E34-1B8934B44829}"), "ExportStatus", "Export Status", "The current status of the export job.", SPFieldType.Choice, ChoiceColumnChoices.ExportStatusChoices, ColumnGroupName);
            ExportType = new ReadOnlyColumnDetails(new Guid("{5F87BA3D-3C8F-4BBE-9C46-9C0A00A4E356}"), "ExportType", "Export Type", "The format of the Glyma export produced.", SPFieldType.Choice, ChoiceColumnChoices.ExportTypeChoices, ColumnGroupName);
            MapType = new ReadOnlyColumnDetails(new Guid("{8E14F8AB-CC81-487D-BA7B-3D87C6C9E578}"), "MapType", "Map Type", "The type of map that is being exported.", SPFieldType.Choice, ChoiceColumnChoices.MapTypeChoices, ColumnGroupName);
            PercentageComplete = new ReadOnlyColumnDetails(new Guid("{1399F348-7E39-4DFE-B352-B1D59C8547F3}"), "PercentageComplete", "Percentage Complete", "The progress of the Glyma export.", SPFieldType.Number, ColumnGroupName);
            RootMapUid = new ReadOnlyColumnDetails(new Guid("{EBEAA658-0EBB-4C0A-8D43-EC8A67E97FC8}"), "RootMapUid", "RootMapUid", "The identifier of the Glyma root map node.", SPFieldType.Text, ColumnGroupName);
            DomainUid = new ReadOnlyColumnDetails(new Guid("{D330A509-FB89-46CE-9E4D-37CC3E1F6C8D}"), "DomainUid", "DomainUid", "The identifier of the Glyma domain.", SPFieldType.Text, ColumnGroupName);
            LogMessages = new ReadOnlyColumnDetails(new Guid("{17A35953-55D4-4029-A78F-0C261E4E4AC5}"), "LogMessages", "Log Messages", "Contains any logged messages about the export.", SPFieldType.Note, ColumnGroupName);
            ExportProperties = new ReadOnlyColumnDetails(new Guid("{1FCA3DB1-8D87-4EEB-8BDF-528DC5489146}"), "ExportProperties", "Export Properties", "Contains an XML representation of extra properties sent with the export job.", SPFieldType.Note, ColumnGroupName);
        }
    }
}
