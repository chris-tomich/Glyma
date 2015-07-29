using System;
using Soap = TransactionalNodeService.Soap;
using Service = TransactionalNodeService.Service;
using System.ServiceModel;

namespace SilverlightMappingToolBasic
{
    public class SoapEndPointFactory : Soap.ISoapEndPointFactory
    {
        public SoapEndPointFactory(Uri serviceUrl)
        {
            ServiceUrl = serviceUrl;
        }

        public Uri ServiceUrl
        {
            get;
            private set;
        }

        public Service.TransactionalMappingToolServiceClient Create()
        {
            var binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
            binding.MaxReceivedMessageSize = 2147483647;

            var address = new EndpointAddress(ServiceUrl);
            var client = new Service.TransactionalMappingToolServiceClient(binding, address);

            return client;
        }
    }
}
