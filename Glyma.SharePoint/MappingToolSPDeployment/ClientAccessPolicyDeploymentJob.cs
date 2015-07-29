using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;

namespace SevenSigma.MappingTool
{
    public class ClientAccessPolicyDeploymentJob : SPJobDefinition
    {

        public ClientAccessPolicyDeploymentJob()
            : base()
        {
        }
        public ClientAccessPolicyDeploymentJob(string jobName, SPService service, SPServer server, SPJobLockType targetType)
            : base(jobName, service, server, targetType)
        {
        }

        public ClientAccessPolicyDeploymentJob(string jobName, SPWebApplication webApplication)
            : base(jobName, webApplication, null, SPJobLockType.None)
        {
        }

        public override void Execute(Guid targetInstanceId)
        {
            var webApp = this.Parent as SPWebApplication;
#if SP2013
            if (FeatureCompatibilityLevel == 0)
            {
               FeatureCompatibilityLevel = webApp.Farm.BuildVersion.Major;
            }
#endif
            foreach (KeyValuePair<SPUrlZone, SPIisSettings> setting in webApp.IisSettings)
            {
                var webRootPolicyLocation = setting.Value.Path.FullName + @"\clientaccesspolicy.xml";
#if SP2010
                var featuresPolicyLocation = SPUtility.GetGenericSetupPath(@"TEMPLATE\FEATURES\DeployClientAccessPolicyFeature\ClientAccessPolicy\clientaccesspolicy.xml");
#endif
#if SP2013
                var featuresPolicyLocation = SPUtility.GetVersionedGenericSetupPath(@"TEMPLATE\FEATURES\DeployClientAccessPolicyFeature\ClientAccessPolicy\clientaccesspolicy.xml", FeatureCompatibilityLevel); 
#endif
                File.Copy(featuresPolicyLocation, webRootPolicyLocation, true);
            }

            base.Execute(targetInstanceId);
        }

#if SP2013
        public int FeatureCompatibilityLevel
        {
           get;
           set;
        }
#endif
    }
}