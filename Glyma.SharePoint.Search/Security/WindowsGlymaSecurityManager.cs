using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Administration.Claims;

namespace Glyma.SharePoint.Search
{
   public class WindowsGlymaSecurityManager : GlymaSecurityManager
   {
      private static BasicCache<string, byte[]> _nodeAclCache = new BasicCache<string, byte[]>();
      private static RunningTasksQueue<string> _nodeAclTasks = new RunningTasksQueue<string>();

      private IGlymaSecurityRepository _securityRepository;
      private bool _isCacheEnabled = false;


      public WindowsGlymaSecurityManager(IGlymaSecurityRepository securityRepository)
      {
         _securityRepository = securityRepository;
      }


      public WindowsGlymaSecurityManager(IGlymaSecurityRepository securityRepository, TimeSpan autoExpireItemsPeriod, int maxItems, TimeSpan nodeAclCacheDuration, TimeSpan nodeAclTaskWaitDuration)
      {
         _isCacheEnabled = true;
         _securityRepository = securityRepository;
         AutoExpireItemsPeriod = autoExpireItemsPeriod;
         MaxItems = maxItems;
         CacheDuration = nodeAclCacheDuration;
         TaskWaitDuration = nodeAclTaskWaitDuration;
      }


      public TimeSpan AutoExpireItemsPeriod
      {
         get
         {
            return _nodeAclCache.AutoExpireItemsPeriod;
         }

         set
         {
            _nodeAclCache.AutoExpireItemsPeriod = value;
         }
      }


      public int MaxItems
      {
         get
         {
            return _nodeAclCache.MaxItems;
         }

         set
         {
            _nodeAclCache.MaxItems = value;
         }
      }


      public TimeSpan CacheDuration { get; set; }


      public TimeSpan TaskWaitDuration { get; set; }


      public override byte[] GetAllowAllNodeAcl(string repositoryName, Guid domainId, Guid rootMapId)
      {
         // Return an empty Windows node ACL.
         SecurityIdentifier farmAccountSid = SPFarm.Local.DefaultServiceAccount.SecurityIdentifier;
         CommonSecurityDescriptor securityDescriptor = new CommonSecurityDescriptor(false, false, ControlFlags.None, farmAccountSid, null, null, null);
         securityDescriptor.SetDiscretionaryAclProtection(true, false);

         byte[] itemAcl = new byte[securityDescriptor.BinaryLength];
         securityDescriptor.GetBinaryForm(itemAcl, 0);

         return itemAcl;
      }


      public override byte[] GetNodeAcl(string mapDatabaseName, string repositoryName, Guid domainId, Guid rootMapId)
      {
         byte[] nodeAcl;

         if (_isCacheEnabled)
         {
            // Ensure requests submitted in multiple threads for ACL's of the same root map don't result in multiple executions of the ACL generation logic (particularly when
            // the ACL is expired out of the cache and needs to be re-generated). 
            string nodeAclKey = GetNodeAclKey(repositoryName, domainId, rootMapId);
            using (RunUniqueTask<string> runTask = new RunUniqueTask<string>(_nodeAclTasks, nodeAclKey, TaskWaitDuration))
            {
               nodeAcl = _nodeAclCache.Get(nodeAclKey);
               if (nodeAcl == null)
               {
                  nodeAcl = GetNodeAclWorker(mapDatabaseName, repositoryName, domainId, rootMapId, _isCacheEnabled);
                  _nodeAclCache.Insert(nodeAclKey, nodeAcl, BasicCacheConstants.NoAbsoluteExpiration, CacheDuration);
               }
            }
         }
         else
         {
            nodeAcl = GetNodeAclWorker(mapDatabaseName, repositoryName, domainId, rootMapId, _isCacheEnabled);
         }
         return nodeAcl;
      }


      protected byte[] GetNodeAclWorker(string mapDatabaseName, string repositoryName, Guid domainId, Guid rootMapId, bool isCacheEnabled)
      {
         SecurityIdentifier farmAccountSid = SPFarm.Local.DefaultServiceAccount.SecurityIdentifier;
         CommonSecurityDescriptor securityDescriptor = new CommonSecurityDescriptor(false, false, ControlFlags.None, farmAccountSid, null, null, null);
         securityDescriptor.SetDiscretionaryAclProtection(true, false);

         // Deny access to all users.
         SecurityIdentifier everyoneSid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
         securityDescriptor.DiscretionaryAcl.RemoveAccess(AccessControlType.Allow, everyoneSid, unchecked((int)0xffffffffL), InheritanceFlags.None, PropagationFlags.None);

         // Grant access to specified users.
         securityDescriptor.DiscretionaryAcl.AddAccess(AccessControlType.Allow, farmAccountSid, unchecked((int)0xffffffffL), InheritanceFlags.None, PropagationFlags.None);

         List<SpUserDetail> allowedUsers = GetAllowedUsers(mapDatabaseName, repositoryName, domainId, rootMapId);
         List<string> addedSids = new List<string>();
         foreach (SpUserDetail user in allowedUsers)
         {
            SecurityIdentifier userSid = null;
            if (!string.IsNullOrEmpty(user.Sid))
            {
               userSid = new SecurityIdentifier(user.Sid);
            }
            else
            {
               userSid = GetSidFromClaim(user.LoginName);
            }

            if (userSid != null && !addedSids.Contains(userSid.Value))
            {
               securityDescriptor.DiscretionaryAcl.AddAccess(AccessControlType.Allow, userSid, unchecked((int)0xffffffffL), InheritanceFlags.None, PropagationFlags.None);
               addedSids.Add(userSid.Value);
            }
         }

         byte[] itemAcl = new byte[securityDescriptor.BinaryLength];
         securityDescriptor.GetBinaryForm(itemAcl, 0);

         return itemAcl;
      }


      protected List<SpUserDetail> GetAllowedUsers(string mapDatabaseName, string repositoryName, Guid domainId, Guid rootMapId)
      {
         List<SpUserDetail> allowedUsers = new List<SpUserDetail>();

         // Get the non-admninistrator users defined in the security database.
         List<SpFarmGroupIdentifier> allowedGroupIdentifiers = _securityRepository.GetAllowedGroups(repositoryName, domainId, rootMapId);
         foreach (SpFarmGroupIdentifier groupIdentifier in allowedGroupIdentifiers)
         {
            try
            {
               SpSiteCollectionRepository siteCollectionRepository = new SpSiteCollectionRepository(groupIdentifier.SiteCollectionId);
               Dictionary<string, SpUserDetail> users = siteCollectionRepository.GetUsersInGroup(groupIdentifier.GroupId);
               allowedUsers = allowedUsers.Union(users.Values).ToList();
            }
            catch (FileNotFoundException currentException)
            {
               GlymaSearchLogger.WriteTrace(LogCategoryId.Security, TraceSeverity.Medium, "Unable to connect to the site collection with ID, " + groupIdentifier.SiteCollectionId + " : " + currentException.ToString());
            }
         }

         // Get the administators for the site collections that are associated with the map database.
         List<Guid> siteCollectionIds = _securityRepository.GetSiteCollectionAssociations(mapDatabaseName);
         foreach (Guid siteCollectionId in siteCollectionIds)
         {
            try
            {
               SpSiteCollectionRepository siteCollectionRepository = new SpSiteCollectionRepository(siteCollectionId);
               Dictionary<string, SpUserDetail> siteCollectionAdministrators = siteCollectionRepository.GetSiteCollectionAdministrators();
               allowedUsers = allowedUsers.Union(siteCollectionAdministrators.Values).ToList();
            }
            catch (FileNotFoundException currentException)
            {
               GlymaSearchLogger.WriteTrace(LogCategoryId.Security, TraceSeverity.Medium, "Unable to connect to the site collection with ID, " + siteCollectionId + " : " + currentException.ToString());
            }
         }

         return allowedUsers;
      }
   }
}
