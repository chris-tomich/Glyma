using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace Glyma.SharePoint.Search
{
   /// <summary>
   /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
   /// </summary>
   /// <remarks>
   /// The GUID attached to this class may be used during packaging and should not be modified.
   /// </remarks>

   [Guid("e03c8e6f-6ea4-4d7a-9535-881ab2e85c28")]
   public class GlymaSharePointEventReceiver : SPFeatureReceiver
   {
      public override void FeatureInstalled(SPFeatureReceiverProperties properties)
      {
         base.FeatureInstalled(properties);
         // This call registers the areas and categories of the LoggingService with SharePoint so that they appear in the 
         // diagnostic configuration screen in Central Administration.
         GlymaSearchLoggingService service = GlymaSearchLogger.Service;
      }


      public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
      {
         base.FeatureUninstalling(properties);
         // Removes the areas and categories of the LoggingService from SharePoint so that they no longer appear in the 
         // diagnostic configuration screen in Central Administration.
         GlymaSearchLoggingService service = GlymaSearchLogger.Service;
         if (service != null)
         {
            service.Delete();
         }
      }
   }
}
