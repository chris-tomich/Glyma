using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace TransactionalNodeService.Proxy.Universal.HeaderHandler
{
    public class ForceWindowsAuthMessageInspector : IClientMessageInspector
    {
        public void AfterReceiveReply(ref Message reply, object correlationState) { }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            HttpRequestMessageProperty httpRequestMessage;
            object httpRequestMessageObject;
            if (request.Properties.TryGetValue(HttpRequestMessageProperty.Name, out httpRequestMessageObject))
            {
                httpRequestMessage = httpRequestMessageObject as HttpRequestMessageProperty;
                if (string.IsNullOrEmpty(httpRequestMessage.Headers["X-FORMS_BASED_AUTH_ACCEPTED"]))
                {
                    httpRequestMessage.Headers["X-FORMS_BASED_AUTH_ACCEPTED"] = "f";
                }
            }
            else
            {
                httpRequestMessage = new HttpRequestMessageProperty();
                httpRequestMessage.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
                request.Properties.Add(HttpRequestMessageProperty.Name, httpRequestMessage);
            }

            return null;
        }
    }
}
