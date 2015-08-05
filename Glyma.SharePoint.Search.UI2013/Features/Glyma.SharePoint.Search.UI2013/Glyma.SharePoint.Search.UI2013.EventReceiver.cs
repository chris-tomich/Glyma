using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;

namespace Glyma.SharePoint.Search.UI
{
   /// <summary>
   /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
   /// </summary>
   /// <remarks>
   /// The GUID attached to this class may be used during packaging and should not be modified.
   /// </remarks>

   [Guid("78df4acc-c1fa-463a-a497-410ff0c3b589")]
   public class SearchPrereqFeatureReceiver : SPFeatureReceiver
   {
      public override void FeatureActivated(SPFeatureReceiverProperties properties)
      {
         SPSite siteCollection = properties.Feature.Parent as SPSite;
         if (siteCollection == null)
         {
            throw new SPException("This feature can only be activated in a site collection.");
         }

         SearchConfigurationManager searchConfigManager = new SearchConfigurationManager(siteCollection);
         searchConfigManager.EnsureGlymaResultTypesSynced();
      }
   }
}
