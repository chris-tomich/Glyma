using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Glyma.NodeServiceApp;
using Microsoft.SharePoint.Administration;

namespace Glyma.NodeServiceInstaller.Features.GlymaNodeServiceInstaller
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("256db29e-6769-40bb-9cc2-2d5b1fd9bcf9")]
    public class GlymaNodeServiceInstallerEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            // Install the service.
            NodeService service = SPFarm.Local.Services.GetValue<NodeService>();
            if (service == null)
            {
                service = new NodeService(SPFarm.Local);
                service.Update();
            }

            // Install the service proxy.
            NodeServiceProxy serviceProxy = SPFarm.Local.ServiceProxies.GetValue<NodeServiceProxy>();
            if (serviceProxy == null)
            {
                serviceProxy = new NodeServiceProxy(SPFarm.Local);
                serviceProxy.Update(true);
            }

            // With service added to the farm, install instance.
            foreach (SPServer server in SPFarm.Local.Servers)
            {
                if (server.Role != SPServerRole.Invalid)
                {
                    NodeServiceInstance serviceInstance = new NodeServiceInstance(server, service);
                    serviceInstance.Update(true);
                }
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            // Uninstall the instance.
            //NodeServiceInstance serviceInstance = SPFarm.Local.Services.GetValue<NodeServiceInstance>();
            foreach (SPServer server in SPFarm.Local.Servers)
            {
                if (server.Role != SPServerRole.Invalid)
                {
                    NodeServiceInstance serviceInstance = server.ServiceInstances.GetValue<NodeServiceInstance>();

                    if (serviceInstance != null)
                    {
                        serviceInstance.Delete();
                    }
                }
            }

            // Uninstall the service proxy.
            NodeServiceProxy serviceProxy = SPFarm.Local.ServiceProxies.GetValue<NodeServiceProxy>();
            if (serviceProxy != null)
            {
                serviceProxy.Delete();
            }

            // Uninstall the service.
            NodeService service = SPFarm.Local.Services.GetValue<NodeService>();
            if (service != null)
            {
                service.Delete();
            }
        }
    }
}
