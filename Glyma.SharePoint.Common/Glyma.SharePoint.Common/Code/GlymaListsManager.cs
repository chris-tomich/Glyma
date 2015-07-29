using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.SharePoint.Common
{
    public class GlymaListsManager
    {
        private SPWeb Site
        {
            get;
            set;
        }

        public GlymaListsManager(SPWeb site)
        {
            Site = site;
        }

        public void EnsureLists()
        {
            SPList glymaExports = EnsureGlymaExportsList();
            glymaExports.EnableAttachments = true; //make sure attachments are enabled
            glymaExports.EnableVersioning = true;
            glymaExports.Update();
        }

        private SPList EnsureGlymaExportsList()
        {
            bool isNewList = false;
            SPList list = Site.EnsureList(GlymaLists.GlymaExportsList, ref isNewList);

            SPField titleColumn = list.Fields[SPBuiltInFieldId.Title];
            titleColumn.Indexed = true;
            titleColumn.Title = "WorkItem ID";
            titleColumn.EnforceUniqueValues = true;
            titleColumn.Update();

            bool isNewColumn = false;
            SPField exportStatus = list.EnsureColumn(GlymaColumns.ExportStatus, false, ref isNewColumn);
            exportStatus.Required = true;
            exportStatus.Update();

            isNewColumn = false;
            SPField exportType = list.EnsureColumn(GlymaColumns.ExportType, false, ref isNewColumn);
            exportType.Required = true;
            exportType.Update();

            isNewColumn = false;
            SPField mapType = list.EnsureColumn(GlymaColumns.MapType, false, ref isNewColumn);
            mapType.Required = true;
            mapType.Update();

            isNewColumn = false;
            SPField percentageCompletedField = list.EnsureColumn(GlymaColumns.PercentageComplete, false, ref isNewColumn);
            SPFieldNumber percentComplete = percentageCompletedField as SPFieldNumber;
            percentComplete.Required = true;
            percentComplete.ShowAsPercentage = true;
            percentComplete.MaximumValue = 1;
            percentComplete.MinimumValue = 0;
            percentComplete.Update();

            isNewColumn = false;
            SPField domainUid = list.EnsureColumn(GlymaColumns.DomainUid, false, ref isNewColumn);
            domainUid.Required = true;
            domainUid.Update();

            isNewColumn = false;
            SPField rootMapUid = list.EnsureColumn(GlymaColumns.RootMapUid, false, ref isNewColumn);
            rootMapUid.Required = true;
            rootMapUid.Update();

            isNewColumn = false;
            SPField exportPropertiesField = list.EnsureColumn(GlymaColumns.ExportProperties, false, ref isNewColumn);
            SPFieldMultiLineText exportProperties = exportPropertiesField as SPFieldMultiLineText;
            exportProperties.Required = false;
            exportProperties.AppendOnly = false;
            exportProperties.RichText = false;
            exportProperties.Update();

            isNewColumn = false;
            SPField logMessagesField = list.EnsureColumn(GlymaColumns.LogMessages, false, ref isNewColumn);
            SPFieldMultiLineText logMessages = logMessagesField as SPFieldMultiLineText;
            logMessages.Required = false;
            logMessages.AppendOnly = true;
            logMessages.RichText = false;
            logMessages.Update();

            SPField lastModifiedField = list.Fields[SPBuiltInFieldId.Modified];

            // Configure the default view for the list.
            SPView defaultView = list.DefaultView;
            defaultView.ViewFields.DeleteAll();
            defaultView.ViewFields.Add(titleColumn);
            defaultView.ViewFields.Add(exportStatus);
            defaultView.ViewFields.Add(exportType);
            defaultView.ViewFields.Add(mapType);
            defaultView.ViewFields.Add(percentComplete);
            defaultView.ViewFields.Add(domainUid);
            defaultView.ViewFields.Add(rootMapUid);
            defaultView.ViewFields.Add(exportProperties);
            defaultView.ViewFields.Add(logMessages);
            defaultView.Query = "<OrderBy><FieldRef Name=\"" + lastModifiedField.InternalName + "\" Ascending=\"FALSE\" /></OrderBy>";
            defaultView.Update();

            return list;
        }
    }
}
