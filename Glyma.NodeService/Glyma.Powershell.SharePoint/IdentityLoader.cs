using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Glyma.Powershell.Base;
using System.Management.Automation;

namespace Glyma.Powershell.SharePoint
{
    internal class IdentityLoader
    {
        public IdentityLoader(object identity)
        {
            Identity = identity;
        }

        public object Identity
        {
            get;
            set;
        }

        private bool GetDatabaseDetails(SPWeb web, ref Get_GLDomainBase cmdletBase)
        {
            if (!web.Properties.ContainsKey("Glyma.DatabaseServer") || !web.Properties.ContainsKey("Glyma.DatabaseName"))
            {
                return false;
            }

            cmdletBase.DatabaseServer = web.Properties["Glyma.DatabaseServer"];
            cmdletBase.DatabaseName = web.Properties["Glyma.DatabaseName"];

            return true;
        }

        public bool PopulateCmdletBase(PSCmdlet cmdlet, ref Get_GLDomainBase cmdletBase)
        {
            bool isSuccessful = false;

            object identityAsObject = Identity;

            if (Identity is PSObject)
            {
                PSObject identityAsPSObject = Identity as PSObject;

                identityAsObject = identityAsPSObject.BaseObject;
            }

            if (identityAsObject is SPSite)
            {
                SPSite site = identityAsObject as SPSite;

                using (SPWeb web = site.RootWeb)
                {
                    isSuccessful = GetDatabaseDetails(web, ref cmdletBase);
                }
            }
            else if (identityAsObject is SPWeb)
            {
                SPWeb web = identityAsObject as SPWeb;

                isSuccessful = GetDatabaseDetails(web, ref cmdletBase);
            }
            else if (identityAsObject is string)
            {
                string siteUrl = identityAsObject as string;

                using (SPSite site = new SPSite(siteUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        isSuccessful = GetDatabaseDetails(web, ref cmdletBase);
                    }
                }
            }
            else if (identityAsObject is Guid)
            {
                Guid siteId = (Guid)identityAsObject;

                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.RootWeb)
                    {
                        isSuccessful = GetDatabaseDetails(web, ref cmdletBase);
                    }
                }
            }
            else
            {
                cmdlet.WriteWarning("No valid 'Identity' specified. Identity must be a SPSite, SPWeb, site/web URL, or site colleciton ID.");

                return isSuccessful;
            }

            if (!isSuccessful)
            {
                cmdlet.WriteWarning("Provided 'Identity' has no Glyma Server details declared for it. Try a parent if it's a sub-site.");

                return isSuccessful;
            }

            return isSuccessful;
        }
    }
}
