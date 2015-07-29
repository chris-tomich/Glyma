using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Glyma.Powershell.Smo;
using System.Data.SqlClient;

namespace Glyma.Powershell.Base
{
    internal class New_GLMapSecurityDatabaseBase : IGLCmdletBase
    {
        private string ConnectionString
        {
            get
            {
                return "Data Source=" + DatabaseServer + ";Initial Catalog=" + DatabaseName + ";Integrated Security=True";
            }
        }

        public string DatabaseServer
        {
            get;
            set;
        }

        public string DatabaseName
        {
            get;
            set;
        }

        public string ApplicationPoolAccount
        {
            get;
            set;
        }

        public string SearchCrawlAccount
        {
           get;
           set;
        }

        public void ExecuteCmdletBase(PSCmdlet callingCmdlet)
        {
            SqlAssemblies sqlAssemblies = new SqlAssemblies();

            if (!sqlAssemblies.LoadedSuccessfully)
            {
                callingCmdlet.WriteWarning(sqlAssemblies.ErrorMessage);
                return;
            }

            SmoServer server = new SmoServer(sqlAssemblies, DatabaseServer);
            server.SetApplicationName("GlymaNewMapSecurityDbInstaller");

            SmoDatabase database = new SmoDatabase(sqlAssemblies, server, DatabaseName);

            SmoFileGroup fileGroup = new SmoFileGroup(sqlAssemblies, database, "PRIMARY");
            database.AddFileGroup(fileGroup);

            SmoDataFile dataFile = new SmoDataFile(sqlAssemblies, fileGroup, DatabaseName);
            fileGroup.AddDataFile(dataFile);
            dataFile.SetFileName(server.GetMasterDbPath() + "\\" + DatabaseName + ".mdf");
            dataFile.SetSize(50.0 * 1024.0);
            dataFile.SetGrowthType("KB");
            dataFile.SetGrowth(1024.0);
            dataFile.SetIsPrimaryFile(true);

            SmoLogFile logFile = new SmoLogFile(sqlAssemblies, database, DatabaseName + "_Log");
            database.AddLogFile(logFile);
            logFile.SetFileName(server.GetMasterDbPath() + "\\" + DatabaseName + "_Log.ldf");
            logFile.SetSize(164.0 * 1024.0);
            logFile.SetGrowthType("Percent");
            logFile.SetGrowth(10.0);

            database.Create();

            fileGroup = database.GetFileGroup("PRIMARY");
            fileGroup.SetIsDefault(true);
            fileGroup.Alter();
            database.Alter();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                EmbeddedSqlScript createUserScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapSecurityDB.CreateUser.sql");
                createUserScript.AddToken("[ACCOUNT_NAME]", ApplicationPoolAccount);
                createUserScript.ExecuteNonQuery(connection);

                EmbeddedSqlScript grantUserRoleScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapSecurityDB.GrantUserRole.sql");
                grantUserRoleScript.AddToken("[ACCOUNT_NAME]", ApplicationPoolAccount);
                grantUserRoleScript.AddToken("[ROLE]", "db_owner");
                grantUserRoleScript.ExecuteNonQuery(connection);

                // TODO Create security database.

                EmbeddedSqlScript createGetRootMapsSPScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapSecurityDB.CreateGetRootMapsSP.sql");
                createGetRootMapsSPScript.ExecuteNonQuery(connection);

                EmbeddedSqlScript createSearchUserScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapSecurityDB.CreateUser.sql");
                createSearchUserScript.AddToken("[ACCOUNT_NAME]", SearchCrawlAccount);
                createSearchUserScript.ExecuteNonQuery(connection);

                EmbeddedSqlScript grantSearchPermissionsScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapSecurityDB.GrantSearchPermissions.sql");
                grantSearchPermissionsScript.AddToken("[ACCOUNT_NAME]", SearchCrawlAccount);
                grantSearchPermissionsScript.ExecuteNonQuery(connection);

                connection.Close();
            }
        }
    }
}
