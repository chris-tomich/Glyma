using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Glyma.Powershell.Base;

namespace Glyma.Powershell
{
    [Cmdlet(VerbsCommon.New, "GLDatabase")]
    public class New_GLDatabase : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true)]
        public string DatabaseServer
        {
            get;
            set;
        }

        [Parameter(Position = 1, Mandatory = false)]
        public string MapDatabaseName
        {
            get;
            set;
        }

        [Parameter(Position = 2, Mandatory = false)]
        public string TransactionDatabaseName
        {
            get;
            set;
        }

        [Parameter(Position = 3, Mandatory = false)]
        public string SecurityDatabaseName
        {
            get;
            set;
        }

        [Parameter(Position = 4, Mandatory = false)]
        public string WebApplicationPoolAccount
        {
            get;
            set;
        }

        [Parameter(Position = 5, Mandatory = false)]
        public string GlymaServiceApplicationPoolAccount
        {
            get;
            set;
        }

        [Parameter(Position = 6, Mandatory = false)]
        public string SearchCrawlAccount
        {
            get;
            set;
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            if ((!string.IsNullOrEmpty(MapDatabaseName) && MapDatabaseName == TransactionDatabaseName) ||
                (!string.IsNullOrEmpty(MapDatabaseName) && MapDatabaseName == SecurityDatabaseName) ||
                (!string.IsNullOrEmpty(TransactionDatabaseName) && TransactionDatabaseName == SecurityDatabaseName))
            {
                throw new ArgumentException("The names provided for all the databases must be unique and no two can be the same.");
            }

            string currentUser = string.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName);
            bool isWebAppPoolAccountDbo = false;
            bool isSearchCrawlAccountDbo = false;
            bool isGlymaServiceAccountDbo = false;
            if (!string.IsNullOrEmpty(WebApplicationPoolAccount) && currentUser.Equals(WebApplicationPoolAccount, StringComparison.OrdinalIgnoreCase))
            {
                isWebAppPoolAccountDbo = true;
                this.WriteWarning("The user running this script will be the dbo of the databases created, it matches the account you specified as the WebApplicationPoolAccount.");
            }
            if (!string.IsNullOrEmpty(SearchCrawlAccount) && currentUser.Equals(SearchCrawlAccount, StringComparison.OrdinalIgnoreCase))
            {
                isSearchCrawlAccountDbo = true;
                this.WriteWarning("The user running this script will be the dbo of the databases created, it matches the account you specified as the SearchCrawlAccount.");
            }
            if (!string.IsNullOrEmpty(GlymaServiceApplicationPoolAccount) && currentUser.Equals(GlymaServiceApplicationPoolAccount, StringComparison.OrdinalIgnoreCase))
            {
                isGlymaServiceAccountDbo = true;
                this.WriteWarning("The user running this script will be the dbo of the databases created, it matches the account you specified as the GlymaServiceApplicationPoolAccount.");
            }

            if (!string.IsNullOrEmpty(MapDatabaseName))
            {
                this.WriteVerbose("Creating map database with the name '" + MapDatabaseName.Trim() + "'.");

                New_GLMapDatabaseBase newGLMapDatabaseBase = new New_GLMapDatabaseBase();
                newGLMapDatabaseBase.DatabaseServer = DatabaseServer;
                newGLMapDatabaseBase.DatabaseName = MapDatabaseName.Trim();
                newGLMapDatabaseBase.WebApplicationPoolAccount = WebApplicationPoolAccount;
                newGLMapDatabaseBase.GlymaServiceApplicationPoolAccount = GlymaServiceApplicationPoolAccount;
                newGLMapDatabaseBase.SearchCrawlAccount = SearchCrawlAccount;
                newGLMapDatabaseBase.IsWebApplicationPoolAccountDbo = isWebAppPoolAccountDbo;
                newGLMapDatabaseBase.IsGlymaServiceApplicationPoolAccountDbo = isGlymaServiceAccountDbo;
                newGLMapDatabaseBase.IsSearchCrawlAccountDbo = isSearchCrawlAccountDbo;

                newGLMapDatabaseBase.ExecuteCmdletBase(this);
            }
            else
            {
                this.WriteVerbose("Skipping map database creation as no name was given.");
            }

            if (!string.IsNullOrEmpty(TransactionDatabaseName))
            {
                this.WriteVerbose("Creating transaction database with the name '" + TransactionDatabaseName.Trim() + "'.");

                New_GLTransactionsDatabaseBase newGLTransactionsDatabaseBase = new New_GLTransactionsDatabaseBase();
                newGLTransactionsDatabaseBase.DatabaseServer = DatabaseServer;
                newGLTransactionsDatabaseBase.DatabaseName = TransactionDatabaseName.Trim();
                newGLTransactionsDatabaseBase.WebApplicationPoolAccount = WebApplicationPoolAccount;
                newGLTransactionsDatabaseBase.GlymaServiceApplicationPoolAccount = GlymaServiceApplicationPoolAccount;
                newGLTransactionsDatabaseBase.IsWebApplicationPoolAccountDbo = isWebAppPoolAccountDbo;
                newGLTransactionsDatabaseBase.IsGlymaServiceApplicationPoolAccountDbo = isGlymaServiceAccountDbo;

                newGLTransactionsDatabaseBase.ExecuteCmdletBase(this);
            }
            else
            {
                this.WriteVerbose("Skipping transaction database creation as no name was given.");
            }

            if (!string.IsNullOrEmpty(SecurityDatabaseName))
            {
                this.WriteVerbose("Creating security database with the name '" + SecurityDatabaseName.Trim() + "'.");

                New_GLSecurityDatabaseBase newGLSecurityDatabaseBase = new New_GLSecurityDatabaseBase();
                newGLSecurityDatabaseBase.DatabaseServer = DatabaseServer;
                newGLSecurityDatabaseBase.DatabaseName = SecurityDatabaseName.Trim();
                newGLSecurityDatabaseBase.WebApplicationPoolAccount = WebApplicationPoolAccount;
                newGLSecurityDatabaseBase.GlymaServiceApplicationPoolAccount = GlymaServiceApplicationPoolAccount;
                newGLSecurityDatabaseBase.SearchCrawlAccount = SearchCrawlAccount;
                newGLSecurityDatabaseBase.IsWebApplicationPoolAccountDbo = isWebAppPoolAccountDbo;
                newGLSecurityDatabaseBase.IsGlymaServiceApplicationPoolAccountDbo = isGlymaServiceAccountDbo;
                newGLSecurityDatabaseBase.IsSearchCrawlAccountDbo = isSearchCrawlAccountDbo;

                newGLSecurityDatabaseBase.ExecuteCmdletBase(this);
            }
            else
            {
                this.WriteVerbose("Skipping security database creation as no name was given.");
            }
        }
    }
}
