using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Glyma.Powershell.Smo;
using System.Data.SqlClient;
using System.Reflection;

namespace Glyma.Powershell.Base
{
    internal class New_GLTransactionsDatabaseBase : IGLCmdletBase
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

        public string WebApplicationPoolAccount
        {
            get;
            set;
        }

        public string GlymaServiceApplicationPoolAccount
        {
            get;
            set;
        }

        public bool IsWebApplicationPoolAccountDbo
        {
            get;
            set;
        }

        public bool IsGlymaServiceApplicationPoolAccountDbo
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

            try
            {
                SmoServer server = new SmoServer(sqlAssemblies, DatabaseServer);
                server.SetApplicationName("GlymaNewMapTransactionsDbInstaller");

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

                    if (!string.IsNullOrEmpty(WebApplicationPoolAccount))
                    {
                        if (!IsWebApplicationPoolAccountDbo)
                        {
                            EmbeddedSqlScript createWebUserScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapTransactionsDB.CreateUser.sql");
                            createWebUserScript.AddToken("[ACCOUNT_NAME]", WebApplicationPoolAccount);
                            createWebUserScript.ExecuteNonQuery(connection);

                            EmbeddedSqlScript grantWebUserRoleScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapTransactionsDB.GrantUserRole.sql");
                            grantWebUserRoleScript.AddToken("[ACCOUNT_NAME]", WebApplicationPoolAccount);
                            grantWebUserRoleScript.AddToken("[ROLE]", "db_owner");
                            grantWebUserRoleScript.ExecuteNonQuery(connection);
                        }
                    }
                    else
                    {
                        callingCmdlet.WriteWarning("There was no web application pool account provided. As we are skipping this step, the web application pool account will need to be manually given db_owner rights to this Glyma Transaction DB.");
                    }

                    if (!string.IsNullOrEmpty(GlymaServiceApplicationPoolAccount))
                    {
                        if (!IsGlymaServiceApplicationPoolAccountDbo)
                        {
                            EmbeddedSqlScript createGlymaServiceUserScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapTransactionsDB.CreateUser.sql");
                            createGlymaServiceUserScript.AddToken("[ACCOUNT_NAME]", GlymaServiceApplicationPoolAccount);
                            createGlymaServiceUserScript.ExecuteNonQuery(connection);

                            EmbeddedSqlScript grantGlymaServiceUserRoleScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapTransactionsDB.GrantUserRole.sql");
                            grantGlymaServiceUserRoleScript.AddToken("[ACCOUNT_NAME]", GlymaServiceApplicationPoolAccount);
                            grantGlymaServiceUserRoleScript.AddToken("[ROLE]", "db_owner");
                            grantGlymaServiceUserRoleScript.ExecuteNonQuery(connection);
                        }
                    }
                    else
                    {
                        callingCmdlet.WriteWarning("There was no Glyma service application pool account provided. As we are skipping this step, the Glyma application pool account will need to be manually given db_owner rights to this Glyma Transaction DB.");
                    }

                    EmbeddedSqlScript createOperationsTableScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapTransactionsDB.CreateOperationsTable.sql");
                    createOperationsTableScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createParametersTableScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapTransactionsDB.CreateParametersTable.sql");
                    createParametersTableScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createTransactionsTableScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapTransactionsDB.CreateTransactionsTable.sql");
                    createTransactionsTableScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript insertOperationsScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapTransactionsDB.InsertOperations.sql");
                    insertOperationsScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createGetGlymaDbVersionScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapTransactionsDB.CreateGetGlymaDbVersion.sql");
                    createGetGlymaDbVersionScript.ExecuteNonQuery(connection);

                    connection.Close();
                }
            }
            catch (TargetInvocationException tiex)
            {
                //If it was a Target Invocation Exception find the last InnerException and throw it so the user gets a useful message to troubleshoot.
                Exception ex = Util.FindLastException(tiex);
                throw ex;
            }
        }
    }
}
