using System;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;

namespace TransactionalNodeService.Proxy.Universal.HeaderHandler
{
    public class ForceWindowsAuthHeaderBehaviour : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint serviceEndpoint, BindingParameterCollection bindingParameters) { }

        public void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime behavior)
        {
            behavior.MessageInspectors.Add(new ForceWindowsAuthMessageInspector());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher) { }

        public void Validate(ServiceEndpoint serviceEndpoint) { }
    }
}
