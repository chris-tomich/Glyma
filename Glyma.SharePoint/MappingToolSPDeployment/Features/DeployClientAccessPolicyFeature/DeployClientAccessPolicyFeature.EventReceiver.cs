using System;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SevenSigma.MappingTool.Features.DeployClientAccessPolicyFeature
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("c10e212b-a445-4de4-9546-4116d2b482ae")]
    public class ClientAccessPolicyFeatureEventReceiver : SPFeatureReceiver
    {
        private const string JobName = "ClientAccessPolicyJob";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var site = properties.Feature.Parent as SPWebApplication;
            RemoveJobIfRegistered(site);

            var clientAccessPolicyJob = new ClientAccessPolicyDeploymentJob(JobName, site);
            var schedule = new SPOneTimeSchedule(DateTime.Now);
            clientAccessPolicyJob.Schedule = schedule;
#if SP2013
            clientAccessPolicyJob.FeatureCompatibilityLevel = properties.Definition.CompatibilityLevel;
#endif
            clientAccessPolicyJob.Update();

            site.JobDefinitions.Add(clientAccessPolicyJob);
            site.Update();

            //Disabled running this job for now, the administrator can choose to run it or copy the file manually
            //there are considerations with deploying this file, what if one exists already, does the administrator approve of it without seeing it
            //clientAccessPolicyJob.RunNow(); 
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            var site = properties.Feature.Parent as SPWebApplication;

            RemoveJobIfRegistered(site);
        }

        private void RemoveJobIfRegistered(SPWebApplication site)
        {
            foreach (SPJobDefinition job in site.JobDefinitions)
            {
                if (job.Title == JobName)
                {
                    job.Delete();
                }
            }
        }
    }
}
