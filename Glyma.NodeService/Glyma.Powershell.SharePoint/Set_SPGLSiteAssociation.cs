using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace Glyma.Powershell.SharePoint
{
    [Cmdlet(VerbsCommon.Set, "SPGLSiteAssociation")]
    public class Set_SPGLSiteAssociation : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public object Identity
        {
            get;
            set;
        }

        [Parameter(Position = 1, Mandatory = true)]
        public string DefaultPageUrl
        {
            get;
            set;
        }

        [Parameter(Position = 2, Mandatory = true)]
        public string DatabaseServer
        {
            get;
            set;
        }

        [Parameter(Position = 3, Mandatory = true)]
        public string MapDatabaseName
        {
            get;
            set;
        }

        [Parameter(Position = 4, Mandatory = true)]
        public string TransactionDatabaseName
        {
            get;
            set;
        }

        [Parameter(Position = 5, Mandatory = true)]
        public string SecurityDatabaseName
        {
            get;
            set;
        }

        [Parameter(Position = 6, Mandatory = false)]
        public string TransactionDatabaseServer
        {
            get;
            set;
        }

        [Parameter(Position = 7, Mandatory = false)]
        public string SecurityDatabaseServer
        {
            get;
            set;
        }

        private SPWeb GetWebFromIdentity(object identity)
        {
            object identityAsObject = Identity;

            if (Identity is PSObject)
            {
                PSObject identityAsPSObject = Identity as PSObject;

                identityAsObject = identityAsPSObject.BaseObject;
            }

            if (identityAsObject is SPSite)
            {
                SPSite site = identityAsObject as SPSite;

                return site.RootWeb;
            }
            else if (identityAsObject is SPWeb)
            {
                SPWeb web = identityAsObject as SPWeb;

                return web;
            }
            else if (identityAsObject is string)
            {
                string siteUrl = identityAsObject as string;

                SPSite site = new SPSite(siteUrl);
                SPWeb web = site.OpenWeb();

                return web;
            }
            else if (identityAsObject is Guid)
            {
                Guid siteId = (Guid)identityAsObject;

                SPSite site = new SPSite(siteId);
                SPWeb web = site.RootWeb;

                return web;
            }
            else
            {
                return null;
            }
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            SPWeb webToConfigure = GetWebFromIdentity(Identity);

            if (webToConfigure == null)
            {
                WriteWarning("No valid 'Identity' specified. Identity must be a SPSite, SPWeb, site/web URL, or site colleciton ID.");
            }

            string transactionDatabaseServer;

            if (string.IsNullOrEmpty(TransactionDatabaseServer))
            {
                transactionDatabaseServer = DatabaseServer;
            }
            else
            {
                transactionDatabaseServer = TransactionDatabaseServer;
            }

            string securityDatabaseServer;

            if (string.IsNullOrEmpty(SecurityDatabaseServer))
            {
                securityDatabaseServer = DatabaseServer;
            }
            else
            {
                securityDatabaseServer = TransactionDatabaseServer;
            }

            SPPropertyBag propertyBag = webToConfigure.Properties;

            propertyBag["Glyma.DatabaseServer"] = DatabaseServer;
            propertyBag["Glyma.DatabaseName"] = MapDatabaseName;
            propertyBag["Glyma.TransactionDatabaseServer"] = transactionDatabaseServer;
            propertyBag["Glyma.TransactionDatabaseName"] = TransactionDatabaseName;
            propertyBag["Glyma.SecurityDatabaseServer"] = securityDatabaseServer;
            propertyBag["Glyma.SecurityDatabaseName"] = SecurityDatabaseName;
            propertyBag["Glyma.DefaultPage"] = DefaultPageUrl;

            propertyBag.Update();
            webToConfigure.Update();
        }
    }
}
