using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.NodeServiceApp
{
    [System.Runtime.InteropServices.Guid("26BD10E5-FF12-4EFF-AE3C-239BD2B844FA")]
    [SupportedServiceApplication("FBAAA46D-27C0-45D2-8ACF-0466A1D08A13",
                                "1.0.0.0",
                                typeof(NodeServiceApplicationProxy))]
    public class NodeServiceProxy : SPIisWebServiceProxy, IServiceProxyAdministration
    {
        public NodeServiceProxy()
            : base()
        {
        }

        public NodeServiceProxy(SPFarm farm)
            : base(farm)
        {
        }

        public SPServiceApplicationProxy CreateProxy(Type serviceApplicationProxyType, string name, Uri serviceApplicationUri, SPServiceProvisioningContext provisioningContext)
        {
            if (serviceApplicationProxyType != typeof(NodeServiceApplicationProxy))
            {
                throw new NotSupportedException();
            }

            return new NodeServiceApplicationProxy(name, this, serviceApplicationUri);
        }

        public SPPersistedTypeDescription GetProxyTypeDescription(Type serviceApplicationProxyType)
        {
            return new SPPersistedTypeDescription("Glyma Node Service Proxy", "The Glyma Node Service provides transactional-based graph persistence and retrieval services to the Glyma web application.");
        }

        public Type[] GetProxyTypes()
        {
            return new Type[] { typeof(NodeServiceApplicationProxy) };
        }
    }
}
