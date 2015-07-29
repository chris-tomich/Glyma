using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Client.Services;


using Glyma.UtilityService.Common;
using Glyma.UtilityService.Common.Model;
using Glyma.UtilityService.Common.ExportServiceClasses;
using Glyma.UtilityService.Export;


namespace Glyma.UtilityService.SharePoint
{
    [BasicHttpBindingServiceMetadataExchangeEndpoint]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
#if !DEBUG
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, Namespace = "http://sevensigma.com.au/GlymaUtilityService")]
#endif
#if DEBUG
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, Namespace = "http://sevensigma.com.au/GlymaUtilityService", IncludeExceptionDetailInFaults = true)]
#endif
    public class UtilityServiceManager : IUtilityServiceManager
    {
        /// <summary>
        /// Gets the ExportJobs for the particular allap schema types for a domain and root map.
        /// </summary>
        /// <param name="domainUid">The DomainUid for the exports</param>
        /// <param name="rootMapUid">The RootMapUid for the exports</param>
        /// <returns>A list of all the ExportJobs that match the criteria supplied as arguments</returns>
        public ExportJobsResponse GetExportJobs(Guid domainUid, Guid rootMapUid)
        {
            return GetExportJobsImp(null, domainUid, rootMapUid);
        }

        /// <summary>
        /// Gets the ExportJobs for the particular map schema type for a domain and root map.
        /// </summary>
        /// <param name="mapType">The map schema type</param>
        /// <param name="domainUid">The DomainUid for the exports</param>
        /// <param name="rootMapUid">The RootMapUid for the exports</param>
        /// <returns>A list of all the ExportJobs that match the criteria supplied as arguments</returns>
        public ExportJobsResponse GetExportJobsForMapType(MapType mapType, Guid domainUid, Guid rootMapUid)
        {
            return GetExportJobsImp(mapType, domainUid, rootMapUid);
        }

        /// <summary>
        /// Gets the ExportJobs for the particular map schema type for a domain and root map.
        /// </summary>
        /// <param name="mapType">OPTIONALLY: The map schema type</param>
        /// <param name="domainUid">The DomainUid for the exports</param>
        /// <param name="rootMapUid">The RootMapUid for the exports</param>
        /// <returns>A list of all the ExportJobs that match the criteria supplied as arguments</returns>
        private ExportJobsResponse GetExportJobsImp(MapType? mapType, Guid domainUid, Guid rootMapUid)
        {
            ExportJobsResponse response = new ExportJobsResponse();
            response.ExportJobs = new Dictionary<Guid, ExportJob>();

            try
            {
                Guid webID = SPContext.Current.Web.ID;
                Guid siteID = SPContext.Current.Site.ID;
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(siteID))
                    {
                        using (SPWeb web = site.OpenWeb(webID))
                        {
                            if (web != null)
                            {
                                SPList exportsList = web.TryGetList(web.GetServerRelativeListUrlPrefix() + "GlymaExports"); //TODO get the name from a constant
                                if (exportsList != null)
                                {
                                    SPQuery query = new SPQuery();
                                    if (mapType.HasValue)
                                    {
                                        query.Query = "<Where><And><And>" +
                                            "<Eq><FieldRef Name='DomainUid' /><Value Type='Text'>" + domainUid.ToString() + "</Value></Eq>" +
                                            "<Eq><FieldRef Name='RootMapUid' /><Value Type='Text'>" + rootMapUid.ToString() + "</Value></Eq>" +
                                            "</And><Eq><FieldRef Name='MapType' /><Value Type='Text'>" + mapType.ToString() + "</Value></Eq></And></Where>" +
                                            "<OrderBy><FieldRef Name='Created' Ascending='TRUE'></FieldRef></OrderBy>";
                                    }
                                    else
                                    {
                                        query.Query = "<Where><And>" +
                                            "<Eq><FieldRef Name='DomainUid' /><Value Type='Text'>" + domainUid.ToString() + "</Value></Eq>" +
                                            "<Eq><FieldRef Name='RootMapUid' /><Value Type='Text'>" + rootMapUid.ToString() + "</Value></Eq>" +
                                            "</And></Where>" +
                                            "<OrderBy><FieldRef Name='Created' Ascending='TRUE'></FieldRef></OrderBy>";
                                    }
                                    //get the exports for this domain/rootmap in ascending order
                                    SPListItemCollection exports = exportsList.GetItems(query);
                                    foreach (SPListItem export in exports)
                                    {
                                        ExportJob exportJob = GetExportJob(export);
                                        response.ExportJobs.Add(exportJob.Id, exportJob);

                                        if (exportJob.Status == ExportStatus.Error)
                                        {
                                            //maintenance task if the export job had an error clear the TimerJobWorkItem if it was left behind
                                            CleanupErrorWorkItems(exportJob, site, web);
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception("Failed to find the Glyma Exports list.");
                                }
                            }
                            else
                            {
                                throw new Exception("The SPWeb was not found.");
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ExportError error = new ExportError() { ErrorMessage = "Failed to get the export jobs for the site." };
                throw new FaultException<ExportError>(error, ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Creates an ExportJob and schedules the WorkItem for the timer job that processes the exports.
        /// </summary>
        /// <param name="domainUid">The DominUid for the map being exported</param>
        /// <param name="rootMapUid">The RootMapUid for the map being exported</param>
        /// <param name="exportProperties">The export properties for the export</param>
        /// <param name="mapType">The map type (schema) for the map being exported</param>
        /// <param name="exportType">The output format for the export</param>
        /// <returns>The ExportJob that was created</returns>
        public ExportJobResponse CreateExportJob(Guid domainUid, Guid rootMapUid, IDictionary<string, string> exportProperties, MapType mapType, ExportType exportType)
        {
            ExportJobResponse response = new ExportJobResponse();

            try
            {
                Guid webID = SPContext.Current.Web.ID;
                Guid siteID = SPContext.Current.Site.ID;
                SPUser currentUser = null;
                using (SPSite site = new SPSite(siteID)) 
                {
                    using (SPWeb web = site.OpenWeb(webID))
                    {
                        if (web != null)
                        {
                            currentUser = web.CurrentUser;
                        }
                    }
                }
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    int userId = -1;
                    SPList exportsList = null;
                    int listItemIdNum = -1;
                    
                    using (SPSite site = new SPSite(siteID)) 
                    {
                        using (SPWeb web = site.OpenWeb(webID))
                        {
                            if (web != null)
                            {
                                if (currentUser == null)
                                {
                                    //The current user shouldn't be null, it should have been resolved outside of this RunWithElevatedPrivileges delegate
                                    currentUser = web.CurrentUser;
                                }
                                if (currentUser != null)
                                {
                                    userId = currentUser.ID;

                                    exportsList = web.TryGetList(web.GetServerRelativeListUrlPrefix() + "GlymaExports"); //TODO get the name from a constant
                                    if (exportsList != null)
                                    {

                                        //the text payload will contain the properties serialized into a simple XML format
                                        ExportPropertiesDictionary serializableDict = new ExportPropertiesDictionary(exportProperties);
                                        string textPayload = serializableDict.ConvertToXml();

                                        Guid workItemId = Guid.NewGuid(); // Create a unique id for the work item and export job
                                        listItemIdNum = CreateExportJobListEntry(exportsList, domainUid, rootMapUid, mapType, exportType, workItemId, textPayload, userId);

                                        // if the list item was created then create the export job, if it wasn't it was because an export
                                        // for this particular root map of this type was already scheduled (changing the properties doesn't have effect)
                                        if (listItemIdNum != -1)
                                        {
                                            site.AddWorkItem(workItemId, //gWorkItemId - A Guid that identifies the work item
                                                DateTime.Now.ToUniversalTime(), //schdDateTime - represents a time in universal time for when the work item should take place
                                                GlymaExportWorkItemTimerJob.WorkItemTypeId, //gWorkItemType - this must be the GUID used in the GlymaExportWorkItemTimerJob
                                                web.ID, //gWebId - The identifier of the web containing the list
                                                exportsList.ID, //gParentId - The list ID
                                                listItemIdNum, //nItemId - The list item ID number
                                                true, //fSetWebId - true to set the Web identifier
                                                exportsList.Items.GetItemById(listItemIdNum).UniqueId, //gItemGuid - The unique identifier of the list item
                                                domainUid, //gBatchId - A Guid context identifier for the work item engine
                                                userId, //nUserId - SPUser ID number
                                                null, //rgbBinaryPayload - not used
                                                textPayload, //strTextPayload
                                                Guid.Empty); //gProcessingId - needs to be Guid.Empty

                                            ExportJob scheduledJob = new ExportJob();
                                            scheduledJob.Created = (DateTime)exportsList.Items.GetItemById(listItemIdNum)[SPBuiltInFieldId.Created];
                                            scheduledJob.CreatedBy = new GlymaUser() { Name = currentUser.Name };
                                            scheduledJob.Id = workItemId;
                                            scheduledJob.IsCurrent = true;
                                            scheduledJob.Status = ExportStatus.Scheduled;
                                            scheduledJob.Type = exportType;
                                            scheduledJob.MapType = mapType;
                                            scheduledJob.ExportProperties = exportProperties;
                                            scheduledJob.PercentageComplete = 0;
                                            response.ExportJob = scheduledJob;
                                        }
                                        else
                                        {
                                            //already scheduled so throw an exception to be handled with an error
                                            throw new Exception(string.Format("A scheduled export job already exists for the Glyma map: DomainUid: {0}, RootMapUid: {1}, Export Type: {2}, Map Type: {3}",
                                                domainUid.ToString(), rootMapUid.ToString(), exportType.ToString(), mapType.ToString()));
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Failed to find the Glyma Exports list.");
                                    }
                                }
                                else
                                {
                                    throw new Exception("The current user was not able to be determined.");
                                }
                            }
                            else
                            {
                                throw new Exception("The SPSite and/or the SPWeb were null.");
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ExportError error = new ExportError() { ErrorMessage = "Failed to create the Glyma map export job." };
                throw new FaultException<ExportError>(error, ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Deletes an ExportJob if it's in the Scheduled, Completed, or Error ExportStates.
        /// </summary>
        /// <param name="job">The ExportJob to delete</param>
        /// <returns>The ExportJob that was deleted</returns>
        public ExportJobResponse DeleteExportJob(ExportJob job)
        {
            ExportJobResponse response = new ExportJobResponse();
            
            try
            {
                Guid webID = SPContext.Current.Web.ID;
                Guid siteID = SPContext.Current.Site.ID;
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    SPList exportsList = null;
                    using (SPSite site = new SPSite(siteID))
                    {
                        using (SPWeb web = site.OpenWeb(webID))
                        {
                            if (web != null && site != null)
                            {
                                exportsList = web.TryGetList(web.GetServerRelativeListUrlPrefix() + "GlymaExports");
                                if (exportsList != null)
                                {
                                    SPQuery query = new SPQuery();
                                    query.Query = "<Where>" +
                                                        "<Eq><FieldRef Name='Title' /><Value Type='Text'>" + job.Id.ToString() + "</Value></Eq>" +
                                                    "</Where>";
                                    SPListItemCollection exports = exportsList.GetItems(query);
                                    // There can only be one ExportJob with the job ID (unique values enforced in SP list for Title column)
                                    if (exports.Count > 0)
                                    {
                                        SPListItem exportItem = exports[0];
                                        if (exportItem != null)
                                        {
                                            string exportStatusStr = exportItem["ExportStatus"] as string;
                                            ExportStatus exportStatus = (ExportStatus)Enum.Parse(typeof(ExportStatus), exportStatusStr, true);

                                            if (exportStatus == ExportStatus.Scheduled)
                                            {
                                                SPWorkItemCollection workItemsCollection = new SPWorkItemCollection(site, GlymaExportWorkItemTimerJob.WorkItemTypeId);
                                                uint colCount, rowCount = 0;
                                                object workItems = null;
                                                site.GetWorkItems(workItemsCollection, out colCount, out rowCount, out workItems); //gets all work items for this site of the type GlymaExportWorkItemTimerJob
                                                if (workItemsCollection.Count > 0)
                                                {
                                                    // Delete the work item that this export job created
                                                    SPWorkItemCollection subCollection = workItemsCollection.SubCollection(site, web, 0, (uint)workItemsCollection.Count);
                                                    subCollection.DeleteWorkItem(job.Id);
                                                }
                                            }

                                            if (exportStatus == ExportStatus.Scheduled || exportStatus == ExportStatus.Completed || exportStatus == ExportStatus.Error)
                                            {
                                                exportItem.Delete(); //delete the item after it has been cancelled
                                            }
                                            else if (exportStatus == ExportStatus.Processing)
                                            {
                                                throw new Exception("The export job is currently processing and cannot be deleted.");
                                            }
                                        }
                                        response.ExportJob = job;
                                    }
                                }
                                else
                                {
                                    throw new Exception("Failed to find the Glyma Exports list.");
                                }
                            }
                            else
                            {
                                throw new Exception("The SPSite and/or the SPWeb were null.");
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ExportError error = new ExportError() { ErrorMessage = "Failed to cancel the Glyma map export job." };
                throw new FaultException<ExportError>(error, ex.ToString());
            }

            return response;
        }

        /// <summary>
        /// Checks if the Glyma Export Timer Job exists in the current web pplicatio for the site.
        /// </summary>
        /// <returns>True if the site's Web Application has the Glyma Export Timer Job (i.e. the Web Application featrue is enabled that installs the timer job)</returns>
        public ExportAvailabilityResponse IsExportingAvailable()
        {
            ExportAvailabilityResponse response = new ExportAvailabilityResponse() { IsAvailable = false };

            try
            {
                Guid webID = SPContext.Current.Web.ID;
                Guid siteID = SPContext.Current.Site.ID;
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(siteID)) 
                    {
                        if (site != null)
                        {
                            foreach (SPJobDefinition job in site.WebApplication.JobDefinitions)
                            {
                                if (job.Name == GlymaExportWorkItemTimerJob.JobName)
                                {
                                    response.IsAvailable = true;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("The SPSite was null.");
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ExportError error = new ExportError() { ErrorMessage = "Failed to read if exporting was available." };
                throw new FaultException<ExportError>(error, ex.Message);
            }
            return response;
        }

        private ExportJob GetExportJob(SPListItem export)
        {
            ExportJob exportJob = new ExportJob();
            try
            {
                string workItemId = export[SPBuiltInFieldId.Title] as string;
                workItemId.Trim(); //ensure it is just the GUID in case anyone has messed with the list
                exportJob.Id = new Guid(workItemId);

                exportJob.Created = (DateTime)export[SPBuiltInFieldId.Created];
                string createdBy = export[SPBuiltInFieldId.Author] as string;
                exportJob.CreatedBy = new GlymaUser() { Name = createdBy };

                
                if (export.Attachments.Count > 0)
                {
                    string fileName = export.Attachments[0] as string;
                    string linkAddress = export.Attachments.UrlPrefix + fileName;
                    exportJob.Link = linkAddress;
                }
                string exportStatus = export["ExportStatus"] as string;
                exportJob.Status = (ExportStatus)Enum.Parse(typeof(ExportStatus), exportStatus, true);

                string exportType = export["ExportType"] as string;
                exportJob.Type = (ExportType)Enum.Parse(typeof(ExportType), exportType, true);

                string mapType = export["MapType"] as string;
                exportJob.MapType = (MapType)Enum.Parse(typeof(MapType), mapType, true); ;

                string exportProperties = export["ExportProperties"] as string;
                if (!string.IsNullOrEmpty(exportProperties))
                {
                    try
                    {
                        ExportPropertiesDictionary serializableDict = new ExportPropertiesDictionary(export["ExportProperties"] as string);
                        exportJob.ExportProperties = serializableDict as IDictionary<string, string>;
                    }
                    catch (Exception)
                    {
                        //failed to parse the ExportProperties so assume they are null
                    }
                }

                try
                {
                    Double exportPercentageComplete = (Double)export["PercentageComplete"];
                    exportPercentageComplete = exportPercentageComplete * 100;
                    exportJob.PercentageComplete = (int)exportPercentageComplete;
                }
                catch (Exception)
                {
                    exportJob.PercentageComplete = 0;
                }

                //TODO: determine if the export job is current (have any changes occurred to the map since last export
                exportJob.IsCurrent = true;
            }
            catch (Exception)
            {
                //Failed to read the Export Job from the SPListItem, could be a Guid ID wasn't of the correct format
            }
            
            return exportJob;
        }

        private DateTime GetCreatedTime(SPList exportsList, int listItemId)
        {
            DateTime created = DateTime.Now;
            if (exportsList != null)
            {
                bool parsedSuccessfully = DateTime.TryParse(exportsList.Items.GetItemById(listItemId)[SPBuiltInFieldId.Created] as string, out created);
                if (!parsedSuccessfully)
                {
                    throw new FormatException("The DateTime stored in the Created field for the export job was not a valid DateTime format.");
                }
            }
            return created;
        }

        /// <summary>
        /// Adds the SPListItem for the ExportJob to the GlymaExports lists if there isn't one already in the Scheduled ExportState for the exact same export.
        /// </summary>
        /// <param name="exportsList">The SPList that contains the ExportJobs</param>
        /// <param name="domainId">The DomainUid for the export to be created</param>
        /// <param name="rootmapId">The RootMapUid for the export to be created</param>
        /// <param name="mapType">The type of map (schema) that is being exported</param>
        /// <param name="type">The format of the export to be created</param>
        /// <param name="workItemId">The ID of the workitem</param>
        /// <param name="serializedExportProperties">The ExportProperties as an XML string</param>
        /// <returns>The ID of the list item created or -1 if it wasn't created because an existing item matched it in scheduled state.</returns>
        private int CreateExportJobListEntry(SPList exportsList, Guid domainId, Guid rootmapId, MapType mapType, ExportType type, Guid workItemId, string serializedExportProperties, int userId)
        {
            int listItemId = -1; //default value indicates that item wasn't created
            if (exportsList != null)
            {
                SPQuery query = new SPQuery();
                query.Query = "<Where><And><And><And><And>" +
                            "<And><Eq><FieldRef Name='DomainUid' /><Value Type='Text'>" + domainId.ToString() + "</Value></Eq>" +
                            "<Eq><FieldRef Name='RootMapUid' /><Value Type='Text'>" + rootmapId.ToString() + "</Value></Eq></And>" +
                            "<Eq><FieldRef Name='ExportStatus' /><Value Type='Choice'>" + ExportStatus.Scheduled.ToString() + "</Value></Eq></And>" +
                            "<Eq><FieldRef Name='ExportType' /><Value Type='Choice'>" + type.ToString() + "</Value></Eq></And>" +
                            "<Eq><FieldRef Name='ExportProperties' /><Value Type='Note'>" + serializedExportProperties + "</Value></Eq></And>" +
                            "<Eq><FieldRef Name='MapType' /><Value Type='Choice'>" + mapType.ToString() + "</Value></Eq>"  +
                            "</And></Where>";
                // Query for where this particular rootmap has been scheduled for export to the same type already
                SPListItemCollection existingSchedules = exportsList.GetItems(query);

                if (existingSchedules.Count == 0)
                {
                    //TODO get field names from constants
                    SPListItem exportJob = exportsList.Items.Add();
                    exportJob["ExportType"] = type.ToString();
                    exportJob["MapType"] = mapType.ToString();
                    exportJob["ExportStatus"] = ExportStatus.Scheduled.ToString();
                    exportJob["RootMapUid"] = rootmapId.ToString();
                    exportJob["DomainUid"] = domainId.ToString();
                    exportJob[SPBuiltInFieldId.Title] = workItemId.ToString();
                    exportJob["ExportProperties"] = serializedExportProperties;
                    exportJob["PercentageComplete"] = 0;
                    exportJob[SPBuiltInFieldId.Author] = userId;
                    exportJob[SPBuiltInFieldId.Editor] = userId;
                    exportJob.Update();
                    listItemId = exportJob.ID;
                }
            }
            return listItemId;
        }

        /// <summary>
        /// Deletes and of the TimerJob WorkItems for the GlymaExportWorkItemTimerJob if they still exist and are in error state. 
        /// </summary>
        /// <param name="job">The ExportJob</param>
        /// <param name="site">The SPSite</param>
        /// <param name="web">The SPWeb</param>
        private void CleanupErrorWorkItems(ExportJob job, SPSite site, SPWeb web)
        {
            //ensure that only jobs in error state are attempted to be removed
            if (job.Status == ExportStatus.Error) 
            {
                try
                {
                    SPWorkItemCollection workItemsCollection = new SPWorkItemCollection(site, GlymaExportWorkItemTimerJob.WorkItemTypeId);
                    uint colCount, rowCount = 0;
                    object workItems = null;
                    site.GetWorkItems(workItemsCollection, out colCount, out rowCount, out workItems);
                    if (workItemsCollection.Count > 0)
                    {
                        SPWorkItemCollection subCollection = workItemsCollection.SubCollection(site, web, 0, (uint)workItemsCollection.Count);
                        subCollection.DeleteWorkItem(job.Id);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
