using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

using Glyma.SharePoint.Common;

namespace Glyma.SharePoint.Common.Features.GlymaPrerequisites
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("93257196-24fb-4367-b14f-9e7c18783cfa")]
    public class GlymaPrerequisitesEventReceiver : SPFeatureReceiver
    {
        private const string GlymaProjectManagerRoleName = "Glyma Project Manager";
        private const string GlymaMapManagerRoleName = "Glyma Map Manager";
        private const string GlymaAuthorRoleName = "Glyma Map Author";
        private const string GlymaReaderRoleName = "Glyma Map Reader";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSite site = properties.Feature.Parent as SPSite;

            if (site != null)
            {
                using (SPWeb web = site.RootWeb)
                {
                    //Add the SP Role Definitions
                    GlymaRoleManager roleManager = new GlymaRoleManager(web);
                    roleManager.AddRoleDefinitions();

                    //Create any lists required by Glyma
                    GlymaListsManager listManager = new GlymaListsManager(web);
                    listManager.EnsureLists();
                }
            }
        }
    }
}
