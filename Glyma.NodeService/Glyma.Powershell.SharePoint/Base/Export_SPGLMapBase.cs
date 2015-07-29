using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.ServiceModel;
using System.Text;
using System.Threading;

using Glyma.UtilityService.Common;
using Glyma.UtilityService.Common.Model;
using Glyma.UtilityService.Export;
using Glyma.UtilityService.Export.Common.Control.Interface;
using Glyma.UtilityService.Export.Common.EventArgs;

using TransactionalNodeService.Proxy.Universal;
using TransactionalNodeService.Proxy.Universal.Service;
using TransactionalNodeService.Proxy.Universal.Soap;
using TransactionalNodeService.Proxy.Universal.EventArgs;

using Glyma.Powershell.Base;
using Glyma.UtilityService.Export.IBIS;


namespace Glyma.Powershell.SharePoint.Base
{
    public class Export_SPGLMapBase : IGLCmdletBase
    {
        private IMapManager MapManager
        {
            get;
            set;
        }

        public string Identity
        {
            get;
            set;
        }

        public string ServiceUrl
        {
            get;
            set;
        }

        public Guid Domain
        {
            get;
            set;
        }

        public Guid Map
        {
            get;
            set;
        }

        public string Output
        {
            get;
            set;
        }

        public string SharePointUsername
        {
            get;
            set;
        }

        public string SharePointPassword
        {
            get;
            set;
        }

        public string MapSchema
        {
            get;
            set;
        }

        public string ExportFormat
        {
            get;
            set;
        }

        /// <summary>
        /// Param used for Word and PDF exports that determines if images are included in the exported document (defaults to true)
        /// </summary>
        public bool ShowImages
        {
            get;
            set;
        }

        /// <summary>
        /// Param used for Word and PDF exports that determines if descriptions are included in the exported document (defaults to false)
        /// </summary>
        public bool ShowDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Param used for Word and PDF exports that determines if details of any videos are included in the exported document (defaults to false)
        /// </summary>
        public bool ShowVideo
        {
            get;
            set;
        }

        /// <summary>
        /// Param used for Word and PDF exports that determines if page numbers are added (defaults to false)
        /// </summary>
        public bool ShowPages
        {
            get;
            set;
        }

        private PSCmdlet CallingCmdlet 
        {
            get;
            set;
        }


        /// <summary>
        /// Event used for when a message is to be logged
        /// </summary>
        private AutoResetEvent LogMessageEvent
        {
            get;
            set;
        }

        private object msg_lock = new object();
        private string Message
        {
            get;
            set;
        }

        /// <summary>
        /// Event and associated lock for when progress changed events occur
        /// </summary>
        private AutoResetEvent ProgressEvent
        {
            get;
            set;
        }

        private object pr_lock = new object();
        private ProgressRecord ProgressRecord
        {
            get;
            set;
        }

        /// <summary>
        /// Event used for the end of the Cmdlet
        /// </summary>
        private ManualResetEvent Completed
        {
            get;
            set;
        }

        private WaitHandle[] Events
        {
            get;
            set;
        }

        public void ExecuteCmdletBase(PSCmdlet callingCmdlet)
        {
            CallingCmdlet = callingCmdlet;
            Completed = new ManualResetEvent(false);
            ProgressEvent = new AutoResetEvent(false);
            LogMessageEvent = new AutoResetEvent(false);

            Events = new WaitHandle[] { Completed, ProgressEvent, LogMessageEvent };

            //The init of the MapManager and the setting up of the binding takes time,
            //starting it async gives instant progress feedback to the user in the cmdlet.
            ThreadStart initMapManagerThread = new ThreadStart(InitMapManager);
            initMapManagerThread.Invoke();

            int index = -1;
            do
            {
                index = WaitHandle.WaitAny(Events);

                if (index == 1)
                {
                    lock (pr_lock)
                    {
                        WriteProgress(ProgressRecord);
                    }
                }
                else if (index == 2)
                {
                    lock(msg_lock) 
                    {
                        WriteWarningMessage(Message);
                    }
                }
            }
            while (index != 0); //0 is the Completed event
        }

        private void InitMapManager()
        {
            ProgressRecord progressRecord = new ProgressRecord(0, "Exporting Glyma Map", "Initialising web service client");
            progressRecord.SecondsRemaining = -1;
            progressRecord.PercentComplete = 0;
            progressRecord.RecordType = ProgressRecordType.Processing;

            lock (pr_lock)
            {
                this.ProgressRecord = progressRecord;
                ProgressEvent.Set(); //notify of change to progress
            }

            var binding = new BasicHttpBinding();
            binding.ReaderQuotas.MaxStringContentLength = 2147483647;
            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
            binding.MaxReceivedMessageSize = 2147483647;

            var address = new EndpointAddress(new Uri(ServiceUrl));
            CallingUrl.Url = Identity;

            var client = new TransactionalMappingToolServiceClient(binding, address);
            System.Net.NetworkCredential credential = new System.Net.NetworkCredential(SharePointUsername, SharePointPassword);
            client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            client.ClientCredentials.Windows.ClientCredential = credential;

            MapManager = new SoapMapManager(client, true);

            MapManager.InitialiseMapManagerCompleted += OnInitialiseMapManagerCompleted;
            MapManager.InitialiseMapManagerAsync();
        }

        private void OnInitialiseMapManagerCompleted(object sender, InitialiseMapManagerEventArgs e)
        {
            if (e.IsInitialised)
            {
                ProgressRecord progressRecord = new ProgressRecord(0, "Exporting Glyma Map", string.Format("Initialising {0} export processor", ExportFormat.ToString()));
                progressRecord.SecondsRemaining = -1;
                progressRecord.PercentComplete = 5;
                progressRecord.RecordType = ProgressRecordType.Processing;

                lock (pr_lock)
                {
                    this.ProgressRecord = progressRecord;
                    ProgressEvent.Set(); //notify of change to progress
                }

                ExportUtilityFactory exportFactory = new ExportUtilityFactory(MapManager);
                IExportUtility exportUtil = null;
                switch (MapSchema)
                {
                    case "IBIS":
                        switch (ExportFormat)
                        {
                            case "Compendium":
                                exportUtil = exportFactory.CreateExportUtility(MapType.IBIS, ExportType.Compendium);
                                break;
                            case "Word":
                                exportUtil = exportFactory.CreateExportUtility(MapType.IBIS, ExportType.Word);
                                break;
                            case "PDF":
                                exportUtil = exportFactory.CreateExportUtility(MapType.IBIS, ExportType.PDF);
                                break;
                            case "GlymaXML":
                                //exportUtil = exportFactory.CreateExportUtility(MapType.IBIS, ExportType.GlymaXml);
                                lock (msg_lock)
                                {
                                    Message = "Exporting to GlymaXML format is currently not supported";
                                    LogMessageEvent.Set();
                                }
                                break;
                        }
                        break;
                }
                exportUtil.ExportCompleted += OnExportCompleted;
                exportUtil.ProgressChanged += OnProgressChanged;


                ExportPropertiesDictionary exportProps = CreateExportProperties();
                GlymaExportUserState userState = new GlymaExportUserState(Output);

                progressRecord = new ProgressRecord(0, "Exporting Glyma Map", "Export Progress: 0%");
                progressRecord.SecondsRemaining = -1;
                progressRecord.PercentComplete = 10;
                progressRecord.RecordType = ProgressRecordType.Processing;

                lock (pr_lock)
                {
                    this.ProgressRecord = progressRecord;
                    ProgressEvent.Set(); //notify of change to progress
                }

                exportUtil.ExportMap(Domain, Map, exportProps, null, userState);
                bool signalled = userState.Completed.WaitOne(Timeout.Infinite); //wait for the export to completed
                if (!signalled)
                {
                    Completed.Set(); //since it's inifinite timeout this shouldn't happen
                }
            }
            else
            {
                lock (msg_lock)
                {
                    this.Message = string.Format("Failed to initialise the web service client: {0}", e.ErrorMessage);
                    LogMessageEvent.Set();
                }

                Completed.Set();
            }
        }

        private ExportPropertiesDictionary CreateExportProperties()
        {
            ExportPropertiesDictionary exportProps = new ExportPropertiesDictionary(string.Empty);
            if (ShowImages)
            {
                exportProps.Add("ShowImage", "true");
            }
            if (ShowDescription)
            {
                exportProps.Add("ShowDescription", "true");
            }
            if (ShowVideo)
            {
                exportProps.Add("ShowVideo", "true");
            }
            if (ShowPages)
            {
                exportProps.Add("ShowPages", "true");
            }

            return exportProps;
        }

        private void OnProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            //ensure that the completed percentage only goes up
            if (e.ProgressPercentage > ProgressRecord.PercentComplete)
            {
                ProgressRecord progressRecord = new ProgressRecord(0, "Exporting Glyma Map", string.Format("Export Progress: {0}%", e.ProgressPercentage));
                progressRecord.SecondsRemaining = -1;
                progressRecord.PercentComplete = e.ProgressPercentage;

                lock (pr_lock)
                {
                    this.ProgressRecord = progressRecord;
                    ProgressEvent.Set(); //notify of change to progress
                }
            }
        }

        private void OnExportCompleted(object sender, ExportCompletedEventArgs e)
        {
            GlymaExportUserState userState = e.UserState as GlymaExportUserState;
            if (userState != null)
            {
                try
                {
                    ProgressRecord progressRecord = new ProgressRecord(0, "Exporting Glyma Map", string.Format("Creating output file: {0}", Output));
                    progressRecord.SecondsRemaining = -1;
                    progressRecord.PercentComplete = 100;

                    lock (pr_lock)
                    {
                        this.ProgressRecord = progressRecord;
                        ProgressEvent.Set(); //notify of change to progress
                    }

                    //Delete the file if it already exists before the move operation
                    if (File.Exists(Output))
                    {
                        File.Delete(Output);
                    }
                    File.Move(e.FileLocation, Output);

                    progressRecord = new ProgressRecord(0, "Exporting Glyma Map", "Export completed");
                    progressRecord.SecondsRemaining = -1;
                    progressRecord.PercentComplete = 100;
                    progressRecord.RecordType = ProgressRecordType.Completed;

                    lock (pr_lock)
                    {
                        this.ProgressRecord = progressRecord;
                        ProgressEvent.Set(); //notify of change to progress
                    }
                }
                catch (Exception ex) 
                {
                    lock (msg_lock)
                    {
                        this.Message = string.Format("Failed to move the temporary file '{0}' to the output file location.\r\n\r\nException: {1}", e.FileLocation, ex);
                        LogMessageEvent.Set();
                    }
                }
                finally
                {
                    userState.Completed.Set(); //signal that the export has completed
                    Completed.Set(); //signal that the Cmdlet has completed
                }
            }
        }

        private void WriteWarningMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                CallingCmdlet.WriteWarning(message);
            }
        } 

        private void WriteProgress(ProgressRecord progressRecord)
        {
            if (progressRecord != null)
            {
                CallingCmdlet.WriteProgress(progressRecord);
            }
        }

    }
}
