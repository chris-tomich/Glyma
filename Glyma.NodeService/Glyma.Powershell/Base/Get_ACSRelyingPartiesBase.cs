using Glyma.Powershell.ACSManagementService;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Services.Client;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace Glyma.Powershell.Base
{
    public class Get_ACSRelyingPartiesBase : IGLCmdletBase
    {
        //"ManagementClient";
        public string ServiceIdentityUsernameForManagement
        {
            get;
            set;
        }

        //"Kur2Xc24RJT9cqkQxa5D6A0NmZUbBTBUYuYJ764oWhE=";
        public string ServiceIdentityPasswordForManagement
        {
            get;
            set;
        }

        // = "glymademo";
        public string ServiceNamespace
        {
            get;
            set;
        }

        // = "accesscontrol.windows.net";
        public string AcsHostName
        {
            get;
            set;
        }

        // = "v2/mgmt/service/";
        public string AcsManagementServicesRelativeUrl
        {
            get;
            set;
        }

        private string CachedSwtToken
        {
            get;
            set;
        }

        public void ExecuteCmdletBase(PSCmdlet callingCmdlet)
        {
            ManagementService client = CreateManagementServiceClient();
            var relyingParties = client.RelyingParties;

            List<string> relyingPartyNames = new List<string>();

            foreach (var relyingParty in relyingParties)
            {
                string name = relyingParty.Name;
                relyingPartyNames.Add(name);
            }

            callingCmdlet.WriteObject(relyingPartyNames, true);
        }

        public ManagementService CreateManagementServiceClient()
        {
            string managementServiceEndpoint = String.Format(CultureInfo.InvariantCulture, "https://{0}.{1}/{2}",
                ServiceNamespace,
                AcsHostName,
                AcsManagementServicesRelativeUrl);
            ManagementService managementService = new ManagementService(new Uri(managementServiceEndpoint));

            managementService.SendingRequest += GetTokenWithWritePermission;

            return managementService;
        }

        public void GetTokenWithWritePermission(object sender, SendingRequestEventArgs args)
        {
            GetTokenWithWritePermission((HttpWebRequest)args.Request);
        }

        public void GetTokenWithWritePermission(HttpWebRequest args)
        {
            if (CachedSwtToken == null)
            {
                CachedSwtToken = GetTokenFromACS();
            }

            args.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + CachedSwtToken);
        }

        private string GetTokenFromACS()
        {
            //
            // Request a token from ACS
            //
            WebClient client = new WebClient();
            client.BaseAddress = string.Format(CultureInfo.CurrentCulture,
                                               "https://{0}.{1}",
                                               ServiceNamespace,
                                               AcsHostName);

            NameValueCollection values = new NameValueCollection();
            values.Add("grant_type", "client_credentials");
            values.Add("client_id", ServiceIdentityUsernameForManagement);
            values.Add("client_secret", ServiceIdentityPasswordForManagement);
            values.Add("scope", client.BaseAddress + AcsManagementServicesRelativeUrl);

            byte[] responseBytes = client.UploadValues("/v2/OAuth2-13", "POST", values);

            string response = Encoding.UTF8.GetString(responseBytes);

            // Parse the JSON response and return the access token 
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            Dictionary<string, object> decodedDictionary = serializer.DeserializeObject(response) as Dictionary<string, object>;

            return decodedDictionary["access_token"] as string;
        }
    }
}
