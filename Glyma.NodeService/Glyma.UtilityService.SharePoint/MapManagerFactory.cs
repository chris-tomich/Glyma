using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;

using Microsoft.SharePoint;

using TransactionalNodeService.Proxy.Universal;
using TransactionalNodeService.Proxy.Universal.EventArgs;
using TransactionalNodeService.Proxy.Universal.Service;
using TransactionalNodeService.Proxy.Universal.Soap;


namespace Glyma.UtilityService.SharePoint
{
    internal class MapManagerFactory
    {
        private int _mapManagerTimeout = 60000; //60 seconds
        private IMapManager _mapManager = null;
        private string _errorMsg = string.Empty;

        private ManualResetEvent Completed
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get { return _errorMsg; }
            private set { _errorMsg = value; }
        }

        public IMapManager GetMapManager(SPWeb spWeb)
        {
            _mapManager = null;
            Init(spWeb);
            return _mapManager;
        }

        public MapManagerFactory()
        {
            Completed = new ManualResetEvent(false);
        }

        private void Init(SPWeb spWeb)
        {
            IMapManager mapManager = null;
            if (spWeb != null && spWeb.Site != null)
            {
                var binding = new BasicHttpBinding();
                binding.ReaderQuotas.MaxStringContentLength = 2147483647;
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
                binding.MaxReceivedMessageSize = 2147483647;

                //Note in the future it may be necessary to handle in a configuration file what zone to use if the Default zone doesn't support Windows NTLM authentication.
                Uri siteUri = new Uri(spWeb.Site.Url + "/");
                Uri combinedUri = new Uri(siteUri, "_vti_bin/sevensigma/transactionalmappingtoolservice.svc");
                var address = new EndpointAddress(combinedUri);

                CallingUrl.Url = spWeb.Url;

                var client = new TransactionalMappingToolServiceClient(binding, address);
                NetworkCredential credentials = CredentialCache.DefaultNetworkCredentials;
                client.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;
                client.ClientCredentials.Windows.ClientCredential = credentials;

                mapManager = new SoapMapManager(client, true);
                mapManager.InitialiseMapManagerCompleted += mapManager_InitialiseMapManagerCompleted;
                mapManager.InitialiseMapManagerAsync();
                Completed.WaitOne(_mapManagerTimeout);
            }
        }

        private void mapManager_InitialiseMapManagerCompleted(object sender, InitialiseMapManagerEventArgs e)
        {
            if (e.IsInitialised)
            {
                _mapManager = sender as IMapManager;
            }
            else
            {
                ErrorMessage = e.ErrorMessage;
            }
            Completed.Set();
        }
    }
}
