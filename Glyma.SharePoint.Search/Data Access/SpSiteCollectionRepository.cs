using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Glyma.SharePoint.Search
{
   /// <summary>
   /// Provides data access to a SharePoint site collection.
   /// </summary>
   /// <remarks>
   /// Data access to the site collecion is performed using the SharePoint web services because this avoids having to grant the SharePoint search crawl account access permissions
   /// on the content database. 
   /// </remarks>
   public class SpSiteCollectionRepository
   {
      private SpSiteCollectionConnectionSetting _connectionDetails;


      public SpSiteCollectionRepository(Guid siteCollectionId)
      {
         ConnectionSetting = new SpSiteCollectionConnectionSetting(siteCollectionId);
      }


      public SpSiteCollectionRepository(string url)
      {
         ConnectionSetting = new SpSiteCollectionConnectionSetting(url);
      }


      public SpSiteCollectionRepository(RepositoryConnectionSetting connectionSetting)
      {
         ConnectionSetting = new SpSiteCollectionConnectionSetting(connectionSetting);   
      }


      public SpSiteCollectionConnectionSetting ConnectionSetting
      {
         get
         {
            return _connectionDetails;
         }
         set
         {
            _connectionDetails = value;
         }
      }


      public string GetSiteCollectionKey()
      {
         return (string.IsNullOrEmpty(ConnectionSetting.SiteCollectionUrl) ? ConnectionSetting.SiteCollectionId.ToString() : ConnectionSetting.SiteCollectionUrl);
      }


      public virtual Dictionary<int, SpGroupDetail> GetGroups()
      {
         Dictionary<int, SpGroupDetail> groups = new Dictionary<int, SpGroupDetail>();

         SPSite siteCollection = null;
         try
         {
            if (!ConnectionSetting.SiteCollectionId.Equals(Guid.Empty))
            {
               siteCollection = new SPSite(ConnectionSetting.SiteCollectionId);
            }
            else
            {
               siteCollection = new SPSite(ConnectionSetting.SiteCollectionUrl);
            }

            groups = GetGroups(siteCollection);
         }
         finally
         {
            if (siteCollection != null)
            {
               siteCollection.Dispose();
               siteCollection = null;
            }
         }

         return groups;
      }


      protected Dictionary<int, SpGroupDetail> GetGroups(SPSite siteCollection)
      {
         Dictionary<int, SpGroupDetail> groups = new Dictionary<int, SpGroupDetail>();

         SharePointUserGroup.UserGroupSoapClient client = GetUserGroupServiceClient(siteCollection.Url);
         XElement groupsXml = client.GetGroupCollectionFromSite();
         SpGroupDetailContainer groupsContainer;
         using (StringReader groupStringReader = new StringReader(groupsXml.FirstNode.ToString(SaveOptions.DisableFormatting)))
         {
            XmlSerializer groupsSerialiser = new XmlSerializer(typeof(SpGroupDetailContainer), "http://schemas.microsoft.com/sharepoint/soap/directory/");
            groupsContainer = (SpGroupDetailContainer)groupsSerialiser.Deserialize(groupStringReader);
         }
         foreach (SpGroupDetail group in groupsContainer.Items)
         {
            groups.Add(group.Id, group);
         }

         return groups;
      }


      public virtual Dictionary<string, SpUserDetail> GetUsersInGroup(int groupId)
      {
         Dictionary<string, SpUserDetail> users = new Dictionary<string, SpUserDetail>();

         SPSite siteCollection = null;
         try
         {
            if (!ConnectionSetting.SiteCollectionId.Equals(Guid.Empty))
            {
               siteCollection = new SPSite(ConnectionSetting.SiteCollectionId);
            }
            else
            {
               siteCollection = new SPSite(ConnectionSetting.SiteCollectionUrl);
            }

            users = GetUsersInGroup(siteCollection, groupId);
         }
         finally
         {
            if (siteCollection != null)
            {
               siteCollection.Dispose();
               siteCollection = null;
            }
         }

         return users;
      }


      protected Dictionary<string, SpUserDetail> GetUsersInGroup(SPSite siteCollection, int groupId)
      {
         Dictionary<string, SpUserDetail> users;

         Dictionary<int, SpGroupDetail> groups = GetGroups(siteCollection);
         SpGroupDetail requiredGroup = groups[groupId];
         users = GetUsersInGroup(siteCollection, requiredGroup.Name);

         return users;
      }


      public virtual Dictionary<string, SpUserDetail> GetUsersInGroup(string groupName)
      {
         Dictionary<string, SpUserDetail> users = new Dictionary<string, SpUserDetail>();

         SPSite siteCollection = null;
         try
         {
            if (!ConnectionSetting.SiteCollectionId.Equals(Guid.Empty))
            {
               siteCollection = new SPSite(ConnectionSetting.SiteCollectionId);
            }
            else
            {
               siteCollection = new SPSite(ConnectionSetting.SiteCollectionUrl);
            }

            users = GetUsersInGroup(siteCollection, groupName);
         }
         finally
         {
            if (siteCollection != null)
            {
               siteCollection.Dispose();
               siteCollection = null;
            }
         }

         return users;
      }


      protected Dictionary<string, SpUserDetail> GetUsersInGroup(SPSite siteCollection, string groupName)
      {
         Dictionary<string, SpUserDetail> users = new Dictionary<string, SpUserDetail>();

        SharePointUserGroup.UserGroupSoapClient client = GetUserGroupServiceClient(siteCollection.Url);
         XElement usersXml = client.GetUserCollectionFromGroup(groupName);
         SpUserDetailContainer usersContainer;
         using (StringReader userStringReader = new StringReader(usersXml.FirstNode.ToString(SaveOptions.DisableFormatting)))
         {
            XmlSerializer userSerialiser = new XmlSerializer(typeof(SpUserDetailContainer), "http://schemas.microsoft.com/sharepoint/soap/directory/");
            usersContainer = (SpUserDetailContainer)userSerialiser.Deserialize(userStringReader);
         }
         foreach (SpUserDetail user in usersContainer.Items)
         {
            users.Add(user.LoginName, user);
         }

         return users;
      }


      public virtual Dictionary<string, SpUserDetail> GetSiteCollectionAdministrators()
      {
         Dictionary<string, SpUserDetail> users = new Dictionary<string, SpUserDetail>();

         SPSite siteCollection = null;
         try
         {
            if (!ConnectionSetting.SiteCollectionId.Equals(Guid.Empty))
            {
               siteCollection = new SPSite(ConnectionSetting.SiteCollectionId);
            }
            else
            {
               siteCollection = new SPSite(ConnectionSetting.SiteCollectionUrl);
            }

            SharePointUserGroup.UserGroupSoapClient client = GetUserGroupServiceClient(siteCollection.Url);
            XElement usersXml = client.GetUserCollectionFromSite();
            SpUserDetailContainer usersContainer;
            using (StringReader userStringReader = new StringReader(usersXml.FirstNode.ToString(SaveOptions.DisableFormatting)))
            {
               XmlSerializer userSerialiser = new XmlSerializer(typeof(SpUserDetailContainer), "http://schemas.microsoft.com/sharepoint/soap/directory/");
               usersContainer = (SpUserDetailContainer)userSerialiser.Deserialize(userStringReader);
            }

            foreach (SpUserDetail user in usersContainer.Items)
            {
               if (user.IsSiteAdmin)
               {
                  users.Add(user.LoginName, user);
               }
            }
         }
         finally
         {
            if (siteCollection != null)
            {
               siteCollection.Dispose();
               siteCollection = null;
            }
         }

         return users;
      }


      protected SharePointUserGroup.UserGroupSoapClient GetUserGroupServiceClient(string siteCollectionUrl)
      {
         BasicHttpBinding binding = GetUserGroupServiceBinding();
         EndpointAddress endPoint = new EndpointAddress(siteCollectionUrl + "/_vti_bin/usergroup.asmx");
         SharePointUserGroup.UserGroupSoapClient client = new SharePointUserGroup.UserGroupSoapClient(binding, endPoint);
         client.ClientCredentials.Windows.ClientCredential = System.Net.CredentialCache.DefaultNetworkCredentials;
         client.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;

         return client;
      }


      protected BasicHttpBinding GetUserGroupServiceBinding()
      {
         BasicHttpBinding binding = new BasicHttpBinding();
         binding.Name = "UserGroupSoap";
         binding.CloseTimeout = TimeSpan.FromMinutes(1);
         binding.OpenTimeout = TimeSpan.FromMinutes(1);
         binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
         binding.SendTimeout = TimeSpan.FromMinutes(1);
         binding.AllowCookies = false;
         binding.BypassProxyOnLocal = false;
         binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
         binding.MaxBufferSize = 65536;
         binding.MaxBufferPoolSize = 524288;
         binding.MessageEncoding = WSMessageEncoding.Text;
         binding.TextEncoding = System.Text.Encoding.UTF8;
         binding.TransferMode = TransferMode.Buffered;
         binding.UseDefaultWebProxy = true;
         binding.ReaderQuotas.MaxDepth = 32;
         binding.ReaderQuotas.MaxStringContentLength = 8192;
         binding.ReaderQuotas.MaxArrayLength = 16384;
         binding.ReaderQuotas.MaxBytesPerRead = 4096;
         binding.ReaderQuotas.MaxNameTableCharCount = 16384;
         binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
         binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;

         return binding;
      }
   }
}
