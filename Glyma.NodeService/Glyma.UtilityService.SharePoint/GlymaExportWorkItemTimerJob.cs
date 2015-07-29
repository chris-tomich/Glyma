using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Xml.Schema;
using Glyma.UtilityService.Export.Common.Control.Interface;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

using Glyma.UtilityService.Common.Model;
using Glyma.UtilityService.Export;
using Glyma.UtilityService.Export.Common;
using Glyma.UtilityService.Export.Common.EventArgs;
using Glyma.UtilityService.Export.IBIS;
using TransactionalNodeService.Proxy.Universal;
using TransactionalNodeService.Proxy.Universal.Service;
using TransactionalNodeService.Proxy.Universal.Soap;
using System.Reflection;
using System.ComponentModel;

namespace Glyma.UtilityService.SharePoint
{
    public class GlymaExportWorkItemTimerJob : SPWorkItemJobDefinition
    {
        private static readonly string JOB_TYPE_NAME = "Glyma Export Work Item Job";
        private static readonly Guid WORK_ITEM_TYPE = new Guid("{551C9B4E-1CF3-4E24-A257-8E7A1AFC6663}");
        private static readonly int BATCH_FETCH_LIMIT = 5;
        private static readonly string DESCRIPTION = "Processes the scheduled exports of Glyma maps.";
        private static readonly int TIMEOUT_MS = 1800000; //30 minutes (1000 * 60 * 30)
        private object _writeLock = new object();

        public GlymaExportWorkItemTimerJob()
            : base()
        {
        }

        public GlymaExportWorkItemTimerJob(SPWebApplication webApp) :
            base(JobName, webApp)
        {
        }

        public override bool AutoDeleteWorkItemWhenException
        {
            get
            {
                return true;
            }
        }

        public static string JobName
        {
            get
            {
                return JOB_TYPE_NAME;
            }
        }

        /// <summary>
        /// When adding a work item to the site for processing by this work item job definition it needs
        /// to provide this ID that will match the Guid returned by the WorkItemType() method.
        /// </summary>
        public static Guid WorkItemTypeId
        {
            get
            {
                return WORK_ITEM_TYPE;
            }
        }

        /// <summary>
        /// The Guid of work items to be process by this job
        /// </summary>
        /// <returns></returns>
        public override Guid WorkItemType()
        {
            //This is a unique Guid for this type of work item
            return WORK_ITEM_TYPE;
        }

        public override string DisplayName
        {
            get
            {
                return JOB_TYPE_NAME;
            }
        }

        public override string TypeName
        {
            get
            {
                return JOB_TYPE_NAME;
            }
        }

        public override string Description
        {
            get
            {
                return DESCRIPTION;
            }
        }

        /// <summary>
        /// The number of items the timer job should process in a single run.
        /// Prevents the job from overloading the environment with a job that runs for too long.
        /// </summary>
        public override int BatchFetchLimit
        {
            get
            {
                return GlymaExportWorkItemTimerJob.BATCH_FETCH_LIMIT;
            }
        }

        /// <summary>
        /// Progress of the Work Item Timer Job, persists the value in the SPPersistedObject heirarchy
        /// </summary>
        public int Progress
        {
            get;
            set;
        }

        protected override bool ProcessWorkItems(SPContentDatabase contentDatabase,
            SPWorkItemCollection workItems, SPJobState jobState)
        {
            Progress = 0;
            return base.ProcessWorkItems(contentDatabase, workItems, jobState);
        }

        protected override bool ProcessWorkItem(SPContentDatabase contentDatabase, 
            SPWorkItemCollection workItems, SPWorkItem workItem, SPJobState jobState)
        {
            Stopwatch sw = Stopwatch.StartNew(); //used to time how long the export took
            bool processingAlready = false;
            
            try
            {
                if (workItem != null)
                {
                    //process the workItem
                    using (SPSite site = new SPSite(workItem.SiteId))
                    {
                        using (SPWeb web = site.OpenWeb(workItem.WebId))
                        {
                            // processing logic
                            // 1. SET GlymaExport list Item to ExportStatus.Processing
                            // 2. START export based on type in GlymaExport list item
                            // 3. STORE the export in the GlymaExport list against the item as an attachment
                            // 4. SET GlymaExport list item to ExportStatus.Completed
                            SPList exportsList = null;
                            SPListItem exportItem = null;
                            try
                            {
                                exportsList = web.Lists[workItem.ParentId];
                                exportItem = exportsList.GetItemByUniqueId(workItem.ItemGuid);
                                if (exportItem != null)
                                {
                                    string exportStatus = exportItem["ExportStatus"] as string;
                                    if (exportStatus == ExportStatus.Scheduled.ToString())
                                    {
                                        // If the export was still marked as scheduled then set it to processing
                                        WriteExportStatus(workItem, ExportStatus.Processing);

                                        exportItem = exportsList.GetItemByUniqueId(workItem.ItemGuid);

                                        if (exportItem != null)
                                        {
                                            // read the type of export that we should produce
                                            ExportType exportType = (ExportType)Enum.Parse(typeof(ExportType), exportItem["ExportType"] as string);
                                            MapType mapType = (MapType)Enum.Parse(typeof(MapType), exportItem["MapType"] as string);

                                            Dictionary<string, string> exportProperties = null;
                                            string exportPropertiesStr = exportItem["ExportProperties"] as string;
                                            if (!string.IsNullOrEmpty(exportPropertiesStr) && exportPropertiesStr.Trim() != string.Empty)
                                            {
                                                try
                                                {
                                                    ExportPropertiesDictionary exportDict = new ExportPropertiesDictionary(exportPropertiesStr);
                                                    exportProperties = exportDict as Dictionary<string, string>;
                                                }
                                                catch (XmlSchemaValidationException)
                                                {
                                                    exportProperties = null;
                                                }
                                            }

                                            bool useVerboseLogging = false;
                                            if (exportProperties != null)
                                            {
                                                if (exportProperties.ContainsKey("Verbose"))
                                                {
                                                    if (exportProperties["Verbose"].ToLower() == "true")
                                                    {
                                                        useVerboseLogging = true;
                                                    }
                                                }
                                            }

                                            if (useVerboseLogging)
                                            {
                                                LogMessage(workItem, "Creating Map Manager for the web: {0}.", web.Url);
                                            }

                                            // create the appropriate IExportUtility for the ExportType
                                            MapManagerFactory mapManagerFactory = new MapManagerFactory();
                                            IMapManager mapManager = mapManagerFactory.GetMapManager(web);
                                            if (mapManager != null)
                                            {
                                                if (useVerboseLogging)
                                                {
                                                    LogMessage(workItem, "Created Map Manager for the web: {0}.", web.Url);
                                                }

                                                ExportUtilityFactory exportUtilityFactory = new ExportUtilityFactory(mapManager);
                                                IExportUtility exportUtility = exportUtilityFactory.CreateExportUtility(mapType, exportType);

                                                if (exportUtility != null)
                                                {
                                                    if (useVerboseLogging)
                                                    {
                                                        LogMessage(workItem, "Created Export Utility for {0} map type to {1}.", mapType.ToString(), exportType.ToString());
                                                    }

                                                    exportItem = exportsList.GetItemByUniqueId(workItem.ItemGuid);
                                                    if (exportItem != null)
                                                    {
                                                        Guid rootMapUid = Guid.Empty;
                                                        Guid domainUid = Guid.Empty;
                                                        try
                                                        {
                                                            string rootMapUidValue = exportItem["RootMapUid"] as string;
                                                            string domainUidValue = exportItem["DomainUid"] as string;
                                                            domainUid = new Guid(domainUidValue);
                                                            rootMapUid = new Guid(rootMapUidValue);
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            //The GUIDs were not parsed correctly
                                                        }
                                                        if (rootMapUid != Guid.Empty && domainUid != Guid.Empty)
                                                        {
                                                            //The export utility will do the grunt work and provide a URL to the temp file created
                                                            GlymaExportUserState userState = new GlymaExportUserState(workItem);
                                                            userState.UseVerboseLogging = useVerboseLogging; //store the level of logging to use in the event handlers
                                                            exportUtility.ExportCompleted += exportUtility_ExportCompleted;
                                                            exportUtility.ProgressChanged += exportUtility_ProgressChanged;
                                                            exportUtility.ExceptionRaised += exportUtility_ExceptionRaised;

                                                            if (useVerboseLogging)
                                                            {
                                                                LogMessage(workItem, "Starting export.");
                                                            }

                                                            exportUtility.ExportMap(domainUid, rootMapUid, exportProperties, new List<Guid>(), userState);

                                                            if (useVerboseLogging)
                                                            {
                                                                LogMessage(workItem, "Waiting for export to complete.");
                                                            }

                                                            // Wait for the export to complete (up to the timeout)
                                                            bool signalled = userState.Completed.WaitOne(TIMEOUT_MS);
                                                            if (!signalled)
                                                            {
                                                                if (useVerboseLogging)
                                                                {
                                                                    LogMessage(workItem, "The export timed out after {0}ms", TIMEOUT_MS);
                                                                }
                                                                // The timeout occurred so don't process the completed or progress changed events
                                                                exportUtility.ExportCompleted -= exportUtility_ExportCompleted;
                                                                exportUtility.ProgressChanged -= exportUtility_ProgressChanged;
                                                                exportUtility.ExceptionRaised -= exportUtility_ExceptionRaised;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            throw new Exception(string.Format("The DomainUid and/or RootMapUid were not valid Guid's. DomainUid: {0} RootMapUid: {1}.", domainUid.ToString(), rootMapUid.ToString()));
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    throw new Exception(string.Format("Unable to create export utility for the export type: {0}.", exportType.ToString()));
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception(string.Format("Unable to build an IMapManager for the current SPWeb. {0}", mapManagerFactory.ErrorMessage));
                                            }
                                        }
                                    }
                                    else if (exportStatus == ExportStatus.Processing.ToString())
                                    {
                                        //shouldn't do anything unless it's scheduled.
                                        processingAlready = true;
                                    }
                                }
                                else
                                {
                                    throw new Exception("The Export Job did not exist in the list.");
                                }
                            }
                            catch (TargetInvocationException tiex)
                            {
                                if (workItem != null)
                                {
                                    WriteExportStatus(workItem, ExportStatus.Error);
                                    LogMessage(workItem, "Error during export: {0}", tiex.Message);

                                    if (tiex.InnerException != null)
                                    {
                                        LogMessage(workItem, "Inner Exception({0}): {1}.\r\nStackTrace: {2}", tiex.InnerException.GetType().ToString(), tiex.InnerException.Message, tiex.InnerException.StackTrace);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //exception handling
                                if (workItem != null)
                                {
                                    WriteExportStatus(workItem, ExportStatus.Error);
                                    LogMessage(workItem, "Error during export: {0}.", ex.Message);
                                }
                            }
                            finally
                            {
                                if (!processingAlready)
                                {
                                    // delete the workItem after we've processed it
                                    workItems.SubCollection(site, web, 0, (uint)workItems.Count).DeleteWorkItem(workItem.Id);
                                }

                                //Update the progress of the timer job
                                Progress += (100 / workItems.Count); //estimate only, some maps will export faster than others and different types may export faster
                                if (Progress > 100)
                                {
                                    Progress = 100;
                                }
                                this.UpdateProgress(Progress); //The base classes timer job's overall progress
                                
                                if (!processingAlready) //this is if the timer job ran and started processing the item that was already being processed
                                {
                                    exportItem = exportsList.GetItemByUniqueId(workItem.ItemGuid);
                                    if (exportItem != null)
                                    {
                                        string exportStatus = exportItem["ExportStatus"] as string;
                                        if (exportStatus == ExportStatus.Processing.ToString())
                                        {
                                            //if it's still processing and at this point then something has failed.
                                            WriteExportStatus(workItem, ExportStatus.Error);
                                            LogMessage(workItem, "The export failed as it was still in a processing state when it had apparently completed.");
                                        }

                                        LogMessage(workItem, "Completed in {0:0.000}s.", sw.Elapsed.TotalSeconds);//add in a metric for how long the export took (successful or failure)
                                    }
                                 }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //exception handling
                throw new Exception("Failed to while processing Glyma Export Work Item Timer Job.", ex);
            }

            return true;
        }


        private void exportUtility_ExceptionRaised(object sender, ExceptionRaisedEventArgs e)
        {
            GlymaExportUserState userState = e.UserState as GlymaExportUserState;
            if (userState != null)
            {
                SPWorkItem workItem = userState.StateObject as SPWorkItem;
                if (workItem != null)
                {
                    WriteExportStatus(workItem, ExportStatus.Error);
                    LogMessage(workItem, "Error during export: {0}\r\n{1}", e.ErrorMessage, e.InnerException);
                }
            }
        }


        private void WriteExportStatus(SPWorkItem workItem, ExportStatus status)
        {
            lock (_writeLock)
            {
                if (workItem != null)
                {
                    using (SPSite site = new SPSite(workItem.SiteId))
                    {
                        using (SPWeb web = site.OpenWeb(workItem.WebId))
                        {
                            try
                            {
                                SPList exportsList = web.Lists[workItem.ParentId];
                                SPListItem exportItem = exportsList.GetItemByUniqueId(workItem.ItemGuid);
                                if (exportItem != null)
                                {

                                    exportItem["ExportStatus"] = status.ToString();
                                    exportItem.SystemUpdate(true);

                                }
                            }
                            catch (Exception ex) 
                            { 
                                //failed to write ExportStatus
                            }
                        }
                    }
                }
            }
        }

        private void WriteProgress(SPWorkItem workItem, int percentageComplete)
        {
            lock (_writeLock)
            {
                if (workItem != null)
                {
                    using (SPSite site = new SPSite(workItem.SiteId))
                    {
                        using (SPWeb web = site.OpenWeb(workItem.WebId))
                        {
                            try
                            {
                                SPList exportsList = web.Lists[workItem.ParentId];
                                SPListItem exportItem = exportsList.GetItemByUniqueId(workItem.ItemGuid);
                                if (exportItem != null)
                                {
                                    try
                                    {
                                        double currentValue = (double)exportItem["PercentageComplete"];
                                        double newValue = (double)(percentageComplete / 100.0); //value between 0 and 1
                                        if (newValue > currentValue) //ensure that the % can only go up
                                        {
                                            exportItem["PercentageComplete"] = newValue;
                                            exportItem.SystemUpdate(false);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        //error processing the percentage, this can be ignored.
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //Failed to write progress
                            }
                        }
                    }
                }
            }
        }

        private void exportUtility_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            GlymaExportUserState userState = e.UserState as GlymaExportUserState;
            if (userState != null)
            {
                SPWorkItem workItem = userState.StateObject as SPWorkItem;
                if (workItem != null)
                {
                    WriteProgress(workItem, e.ProgressPercentage);
                }
            }
        }

        private void exportUtility_ExportCompleted(object sender, ExportCompletedEventArgs e)
        {
            GlymaExportUserState userState = e.UserState as GlymaExportUserState;
            if (userState != null)
            {
                SPWorkItem workItem = userState.StateObject as SPWorkItem;
                if (workItem != null)
                {
                    using (SPSite site = new SPSite(workItem.SiteId))
                    {
                        using (SPWeb web = site.OpenWeb(workItem.WebId))
                        {
                            try
                            {
                                if (e.Status == ExportStatus.Error)
                                {
                                    WriteExportStatus(workItem, ExportStatus.Error);
                                    LogMessage(workItem, e.ErrorMessage);
                                }
                                else if (e.Status == ExportStatus.Completed)
                                {
                                    SPList exportsList = web.Lists[workItem.ParentId];
                                    SPListItem exportItem = exportsList.GetItemByUniqueId(workItem.ItemGuid);

                                    if (exportItem != null)
                                    {
                                        // Read the contents of the file
                                        byte[] fileContents = null;
                                        Stream fs = null;
                                        try
                                        {
                                            fs = File.OpenRead(e.FileLocation);
                                            fileContents = new byte[fs.Length];
                                            fs.Read(fileContents, 0, (int)fs.Length);

                                            // Add the file to the ListItem
                                            SPAttachmentCollection attachments = exportItem.Attachments;
                                            string fileName = Path.GetFileName(e.FileLocation);
                                            string extention = fileName.Substring(fileName.LastIndexOf('.'));
                                            fileName = e.MapName + extention;
                                            attachments.Add(fileName, fileContents);
                                            exportItem.Update();

                                            // Mark as completed
                                            WriteExportStatus(workItem, ExportStatus.Completed);
                                            WriteProgress(workItem, 1);
                                            if (userState.UseVerboseLogging)
                                            {
                                                LogMessage(workItem, "Export file copied to list successfully.");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteExportStatus(workItem, ExportStatus.Error);
                                            LogMessage(workItem, "Failed reading the exported file: {0}. {1}", e.FileLocation, ex.Message); //this will append the log message
                                        }
                                        finally
                                        {
                                            if (fs != null)
                                            {
                                                fs.Close();
                                            }
                                        }

                                        //Try delete the temporary file, don't change the status to Error if this fails but log it.
                                        try
                                        {
                                            DeleteTempFile(e.FileLocation);
                                        }
                                        catch (Exception ex)
                                        {
                                            if (workItem != null)
                                            {
                                                //the status will still be Completed but an error occurred that should be monitored.
                                                LogMessage(workItem, "Failed to delete temp export file: {0}. ", ex.Message);
                                            }
                                        }
                                    }
                                }
                            }
                            finally
                            {
                                userState.Completed.Set(); //work item has completed
                            }
                        }
                    }
                }
            }
        }

        private void DeleteTempFile(string pathToTempFile)
        {
            if (pathToTempFile != null)
            {
                try
                {
                    File.Delete(pathToTempFile);
                }
                catch (Exception ex)
                {
                    //failed to delete the temp file
                    throw ex;
                }
            }
            else
            {
                throw new ArgumentException("Path to temporary file was null.");
            }
        }

        private void LogMessage(SPWorkItem workItem, string logMessage, params object[] formatTokens)
        {
            lock (_writeLock)
            {
                if (workItem != null)
                {
                    using (SPSite site = new SPSite(workItem.SiteId))
                    {
                        using (SPWeb web = site.OpenWeb(workItem.WebId))
                        {
                            try
                            {
                                SPList exportsList = web.Lists[workItem.ParentId];
                                SPListItem exportItem = exportsList.GetItemByUniqueId(workItem.ItemGuid);

                                if (exportItem != null)
                                {
                                    exportItem["LogMessages"] = string.Format(logMessage, formatTokens);
                                    exportItem.Update();
                                }
                            }
                            catch (Exception ex)
                            {
                                //failed to write log message
                            }
                        }
                    }
                }
            }
        }
    }
}
