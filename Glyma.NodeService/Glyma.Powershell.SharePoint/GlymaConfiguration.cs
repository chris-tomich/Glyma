using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Glyma.Powershell.SharePoint
{
    internal class GlymaConfiguration : IDisposable
    {
        public GlymaConfiguration(object identity, PSCmdlet cmdlet)
        {
            Identity = identity;

            Load(cmdlet);
            FindTransactionDbConnectionParameters();
            FindMapDbConnectionParameters();
            FindSecurityDbConnectionParameters();
        }

        private SPWeb Web
        {
            get;
            set;
        }

        private void Load(PSCmdlet cmdlet)
        {
            object identityAsObject = Identity;

            if (Identity is PSObject)
            {
                PSObject identityAsPSObject = Identity as PSObject;

                identityAsObject = identityAsPSObject.BaseObject;
            }

            if (identityAsObject is SPSite)
            {
                Site = identityAsObject as SPSite;

                Web = Site.RootWeb;
            }
            else if (identityAsObject is SPWeb)
            {
                SPWeb web = identityAsObject as SPWeb;

                Site = web.Site;
                Web = web;
            }
            else if (identityAsObject is string)
            {
                string siteUrl = identityAsObject as string;

                Site = new SPSite(siteUrl);
                Web = Site.OpenWeb();
            }
            else if (identityAsObject is Guid)
            {
                Guid siteId = (Guid)identityAsObject;

                Site = new SPSite(siteId);
                Web = Site.RootWeb;
            }
        }

        private void FindTransactionDbConnectionParameters()
        {
            string transactionDatabaseServer = "";
            string transactionDatabaseName = "";

            bool isExhausted = false;
            SPWeb web = Web;

            while (!isExhausted)
            {
                try
                {
                    if (web.Properties.ContainsKey("Glyma.TransactionDatabaseServer") && web.Properties.ContainsKey("Glyma.TransactionDatabaseName"))
                    {
                        transactionDatabaseServer = web.Properties["Glyma.TransactionDatabaseServer"];
                        transactionDatabaseName = web.Properties["Glyma.TransactionDatabaseName"];
                        ParametersDbServer = transactionDatabaseServer;
                        ParametersDbName = transactionDatabaseName;
                        SessionDbServer = transactionDatabaseServer;
                        SessionDbName = transactionDatabaseName;
                        isExhausted = true;
                        break;
                    }
                    else
                    {
                        if (web.IsRootWeb)
                        {
                            isExhausted = true;
                        }
                    }
                }
                finally
                {
                    if (web != null)
                    {
                        SPWeb nextWeb = null;

                        if (!isExhausted)
                        {
                            nextWeb = web.ParentWeb;
                        }

                        /// We don't want to dispose the main Web for this bootstrapper object.
                        if (web != Web)
                        {
                            web.Dispose();
                        }

                        web = nextWeb;
                    }
                }
            }
        }

        private void FindMapDbConnectionParameters()
        {
            string databaseServer = "";
            string databaseName = "";

            bool isExhausted = false;
            SPWeb web = Web;

            while (!isExhausted)
            {
                try
                {
                    if (web.Properties.ContainsKey("Glyma.DatabaseServer") && web.Properties.ContainsKey("Glyma.DatabaseName"))
                    {
                        databaseServer = web.Properties["Glyma.DatabaseServer"];
                        databaseName = web.Properties["Glyma.DatabaseName"];
                        MapDbServer = databaseServer;
                        MapDbName = databaseName;
                        isExhausted = true;
                        break;
                    }
                    else
                    {
                        if (web.IsRootWeb)
                        {
                            isExhausted = true;
                        }
                    }
                }
                finally
                {
                    if (web != null)
                    {
                        SPWeb nextWeb = null;

                        if (!isExhausted)
                        {
                            nextWeb = web.ParentWeb;
                        }

                        /// We don't want to dispose the main Web for this bootstrapper object.
                        if (web != Web)
                        {
                            web.Dispose();
                        }

                        web = nextWeb;
                    }
                }
            }
        }

        private void FindSecurityDbConnectionParameters()
        {
            int securableContextId = -1;
            string securityDatabaseServer = "";
            string securityDatabaseName = "";

            bool isExhausted = false;
            SPWeb web = Web;

            while (!isExhausted)
            {
                try
                {
                    if (web.Properties.ContainsKey("Glyma.SecurableContextId"))
                    {
                        securableContextId = int.Parse(web.Properties["Glyma.SecurableContextId"]);
                    }

                    if (web.Properties.ContainsKey("Glyma.SecurityDatabaseServer") && web.Properties.ContainsKey("Glyma.SecurityDatabaseName"))
                    {
                        SecurityWeb = web;
                        securityDatabaseServer = web.Properties["Glyma.SecurityDatabaseServer"];
                        securityDatabaseName = web.Properties["Glyma.SecurityDatabaseName"];
                        SecurityDbServer = securityDatabaseServer;
                        SecurityDbName = securityDatabaseName;
                        SecurableContextId = securableContextId;
                        isExhausted = true;
                        break;
                    }
                    else
                    {
                        if (web.IsRootWeb)
                        {
                            isExhausted = true;
                        }
                    }
                }
                finally
                {
                    if (web != null)
                    {
                        SPWeb nextWeb = null;

                        if (!isExhausted)
                        {
                            nextWeb = web.ParentWeb;
                        }

                        /// We don't want to dispose the main Web or SecurityWeb for this bootstrapper object.
                        if (web != Web && web != SecurityWeb)
                        {
                            web.Dispose();
                        }

                        web = nextWeb;
                    }
                }
            }
        }

        public SPSite Site
        {
            get;
            private set;
        }

        public SPWeb SecurityWeb
        {
            get;
            private set;
        }

        public object Identity
        {
            get;
            private set;
        }

        public string ParametersDbServer
        {
            get;
            set;
        }

        public string ParametersDbName
        {
            get;
            set;
        }

        public string SessionDbServer
        {
            get;
            set;
        }

        public string SessionDbName
        {
            get;
            set;
        }

        public string MapDbServer
        {
            get;
            set;
        }

        public string MapDbName
        {
            get;
            set;
        }

        public string SecurityDbServer
        {
            get;
            set;
        }

        public string SecurityDbName
        {
            get;
            set;
        }

        public int SecurableContextId
        {
            get;
            set;
        }

        public void Dispose()
        {
            if (Web != null)
            {
                Web.Dispose();
                Web = null;
            }

            if (SecurityWeb != null)
            {
                SecurityWeb.Dispose();
                SecurityWeb = null;
            }

            if (Site != null)
            {
                Site.Dispose();
                Site = null;
            }
        }
    }
}
