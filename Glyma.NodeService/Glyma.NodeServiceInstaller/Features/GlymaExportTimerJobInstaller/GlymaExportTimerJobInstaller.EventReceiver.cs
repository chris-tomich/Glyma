using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Glyma.UtilityService.SharePoint;

namespace Glyma.NodeServiceInstaller.Features.GlymaExportTimerJobInstaller
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("51668de7-1dad-4b47-ac59-afae88af37c4")]
    public class GlymaExportTimerJobInstallerEventReceiver : SPFeatureReceiver
    {
        private void RemoveTimerJobs(SPWebApplication webApp)
        {
            foreach (SPJobDefinition job in webApp.JobDefinitions)
            {
                if (job.Name == GlymaExportWorkItemTimerJob.JobName)
                {
                    job.Delete();
                }
            }
        }

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;

            if (webApp != null)
            {
                // Remove the timer job if it exists
                RemoveTimerJobs(webApp);

                // Create schedule the timer job will process items every 15 minutes
                SPMinuteSchedule schedule = new SPMinuteSchedule();
                schedule.BeginSecond = 0;
                schedule.EndSecond = 59;
                schedule.Interval = 1;

                // Create the Timer JOb and assign it a schedule
                GlymaExportWorkItemTimerJob exportWorkItem = new GlymaExportWorkItemTimerJob(webApp);
                exportWorkItem.Schedule = schedule;
                exportWorkItem.Update();
                try
                {
                    exportWorkItem.RunNow(); //run it immediately
                }
                catch { }
            }
        }


        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;

            if (webApp != null)
            {
                // Remove the timer job if it exists
                RemoveTimerJobs(webApp);
            }
        }
    }
}
