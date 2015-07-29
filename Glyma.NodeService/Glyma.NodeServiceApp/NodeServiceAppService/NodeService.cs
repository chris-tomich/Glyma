using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.NodeServiceApp
{
    [System.Runtime.InteropServices.Guid("6299DA8F-BD5D-4FA3-96B9-E49E1D048858")]
    public class NodeService : SPIisWebService, IServiceAdministration
    {
        public NodeService()
        {
        }

        public NodeService(SPFarm farm)
            : base(farm)
        {
        }

        public SPServiceApplication CreateApplication(string name, Type serviceApplicationType, SPServiceProvisioningContext provisioningContext)
        {
            if (serviceApplicationType != typeof(NodeServiceApplication))
            {
                throw new NotSupportedException();
            }

            if (provisioningContext == null)
            {
                throw new ArgumentNullException("provisioningContext");
            }

            // If the service doesn't already exist, create it.
            NodeServiceApplication serviceApp = this.Farm.GetObject(name, this.Id, serviceApplicationType) as NodeServiceApplication;

            if (serviceApp == null)
            {
                serviceApp = NodeServiceApplication.Create(name, this, provisioningContext.IisWebServiceApplicationPool);
            }

            return serviceApp;
        }

        public SPServiceApplicationProxy CreateProxy(string name, SPServiceApplication serviceApplication, SPServiceProvisioningContext provisioningContext)
        {
            if (serviceApplication.GetType() != typeof(NodeServiceApplication))
            {
                throw new NotSupportedException();
            }
            if (serviceApplication == null)
            {
                throw new ArgumentNullException("serviceApplication");
            }

            // Verify the service proxy exists.
            NodeServiceProxy serviceProxy = (NodeServiceProxy)this.Farm.GetObject(name, this.Farm.Id, typeof(NodeServiceProxy));
            if (serviceProxy == null)
                throw new InvalidOperationException("NodeServiceProxy does not exist in the farm.");

            // If the application proxy doesn't exist, create it.
            NodeServiceApplicationProxy applicationProxy = serviceProxy.ApplicationProxies.GetValue<NodeServiceApplicationProxy>(name);
            if (applicationProxy == null)
            {
                Uri serviceAppAddress = ((NodeServiceApplication)serviceApplication).Uri;
                applicationProxy = new NodeServiceApplicationProxy(name, serviceProxy, serviceAppAddress);
            }

            return applicationProxy;
        }

        public SPPersistedTypeDescription GetApplicationTypeDescription(Type serviceApplicationType)
        {
            if (serviceApplicationType != typeof(NodeServiceApplication))
            {
                throw new NotSupportedException();
            }

            return new SPPersistedTypeDescription("Glyma Node Service", "The Glyma Node Service provides transactional-based graph persistence and retrieval services to the Glyma web application.");
        }

        public Type[] GetApplicationTypes()
        {
            return new Type[] { typeof(NodeServiceApplication) };
        }

        public override SPAdministrationLink GetCreateApplicationLink(Type serviceApplicationType)
        {
            return new SPAdministrationLink("/_admin/GlymaNodeServiceApp/Create.aspx");
        }
    }
}
