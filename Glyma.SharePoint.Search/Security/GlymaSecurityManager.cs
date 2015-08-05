using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Administration.Claims;

namespace Glyma.SharePoint.Search
{
   public abstract class GlymaSecurityManager
   {
      public abstract byte[] GetAllowAllNodeAcl(string repositoryName, Guid domainId, Guid rootMapId);
      public abstract byte[] GetNodeAcl(string mapDatabaseName, string repositoryName, Guid domainId, Guid rootMapId);


      public static string GetNodeAclKey(string repositoryName, Guid domainId, Guid rootMapId)
      {
         return "NodeAcl:" + domainId.ToString() + "/" + rootMapId.ToString();
      }


      public static SecurityIdentifier GetSidFromClaim(string claimValue)
      {
         SecurityIdentifier sid = null;

         SPClaimProviderManager claimManager = SPClaimProviderManager.Local;
         if (claimManager == null)
         {
            throw new ApplicationException("Unable to access the claims provider manager.");
         }
         try
         {
            SPClaim claim = claimManager.DecodeClaim(claimValue);
            if (claim.OriginalIssuer.Equals("Windows", StringComparison.OrdinalIgnoreCase))
            {
               if (claim.ClaimType.Equals(Microsoft.IdentityModel.Claims.ClaimTypes.GroupSid, StringComparison.OrdinalIgnoreCase))
               {
                  sid = new SecurityIdentifier(claim.Value);
               }
               else if (claim.ClaimType.Equals(Microsoft.SharePoint.Administration.Claims.SPClaimTypes.UserLogonName, StringComparison.OrdinalIgnoreCase))
               {
                  NTAccount userAccount = new NTAccount(claim.Value);
                  sid = (SecurityIdentifier)userAccount.Translate(typeof(SecurityIdentifier));
               }
            }
         }
         catch (ArgumentException currentException)
         {
            GlymaSearchLogger.WriteTrace(LogCategoryId.Security, TraceSeverity.Unexpected, "The following exception occured when attempting to decode the claim, " + claimValue + " : " + currentException.ToString());
         }

         return sid;
      }
   }
}
