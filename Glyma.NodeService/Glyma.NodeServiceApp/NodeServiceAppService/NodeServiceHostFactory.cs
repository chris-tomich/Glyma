using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace Glyma.NodeServiceApp
{
    internal sealed class NodeServiceHostFactory : ServiceHostFactory
    {
        public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
        {
            ServiceHost serviceHost = new ServiceHost(typeof(NodeServiceApplication), baseAddresses);

            // Configure the service for claims.
            serviceHost.Configure(SPServiceAuthenticationMode.Claims);

            return serviceHost;
        }
    }
}
