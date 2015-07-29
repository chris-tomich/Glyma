using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Reflection;
using Glyma.Powershell.Smo;
using System.Data.SqlClient;

namespace Glyma.Powershell.Base
{
    internal class New_GLMapDatabaseBase : IGLCmdletBase
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

        public string SearchCrawlAccount
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

        public bool IsSearchCrawlAccountDbo
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
                server.SetApplicationName("GlymaNewMapDbInstaller");

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
                            EmbeddedSqlScript createWebUserScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateUser.sql");
                            createWebUserScript.AddToken("[ACCOUNT_NAME]", WebApplicationPoolAccount);
                            createWebUserScript.ExecuteNonQuery(connection);

                            EmbeddedSqlScript grantWebUserRoleScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.GrantUserRole.sql");
                            grantWebUserRoleScript.AddToken("[ACCOUNT_NAME]", WebApplicationPoolAccount);
                            grantWebUserRoleScript.AddToken("[ROLE]", "db_owner");
                            grantWebUserRoleScript.ExecuteNonQuery(connection);
                        }
                    }
                    else
                    {
                        callingCmdlet.WriteWarning("There was no web application pool account provided. As we are skipping this step, the web application pool account will need to be manually given db_owner rights to this Glyma Map DB.");
                    }

                    if (!string.IsNullOrEmpty(GlymaServiceApplicationPoolAccount))
                    {
                        if (!IsGlymaServiceApplicationPoolAccountDbo)
                        {
                            EmbeddedSqlScript createGlymaServiceUserScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateUser.sql");
                            createGlymaServiceUserScript.AddToken("[ACCOUNT_NAME]", GlymaServiceApplicationPoolAccount);
                            createGlymaServiceUserScript.ExecuteNonQuery(connection);

                            EmbeddedSqlScript grantGlymaServiceUserRoleScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.GrantUserRole.sql");
                            grantGlymaServiceUserRoleScript.AddToken("[ACCOUNT_NAME]", GlymaServiceApplicationPoolAccount);
                            grantGlymaServiceUserRoleScript.AddToken("[ROLE]", "db_owner");
                            grantGlymaServiceUserRoleScript.ExecuteNonQuery(connection);
                        }
                    }
                    else
                    {
                        callingCmdlet.WriteWarning("There was no Glyma service application pool account provided. As we are skipping this step, the Glyma application pool account will need to be manually given db_owner rights to this Glyma Map DB.");
                    }

                    EmbeddedSqlScript createTablesScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateTables.sql");
                    createTablesScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createConstraintsScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateConstraints.sql");
                    createConstraintsScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript insertTypesDataScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.InsertTypesData.sql");
                    insertTypesDataScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createFullTextCatalogScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateFullTextCatalog.sql");
                    createFullTextCatalogScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createStoredProceduresScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateStoredProcedures.sql");
                    createStoredProceduresScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createQueryMapSPScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateQueryMapSP.sql");
                    createQueryMapSPScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createBasicSearchSPScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateBasicSearchSP.sql");
                    createBasicSearchSPScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createContainsUDFScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateContainsUserDefinedFunction.sql");
                    createContainsUDFScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createCreateFreeTextUDFScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateFreeTextUserDefinedFunction.sql");
                    createCreateFreeTextUDFScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createNodesListTableType = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateNodesListTableType.sql");
                    createNodesListTableType.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createQueryNodesSP = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateQueryNodesSP.sql");
                    createQueryNodesSP.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createGetGlymaDbVersionScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateGetGlymaDbVersion.sql");
                    createGetGlymaDbVersionScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createGetChildNodesSPScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateGetChildNodesSP.sql");
                    createGetChildNodesSPScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createGetNodeInMapDetailsSPScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateGetNodeInMapDetailsSP.sql");
                    createGetNodeInMapDetailsSPScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createGetNodesInMapSPScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateGetNodesInMapSP.sql");
                    createGetNodesInMapSPScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createGetParentNodesSPScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateGetParentNodesSP.sql");
                    createGetParentNodesSPScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript createGetRootMapsSPScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateGetRootMapsSP.sql");
                    createGetRootMapsSPScript.ExecuteNonQuery(connection);

                    EmbeddedSqlScript addNewColumnsScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateAuditLogsTable.sql");
                    addNewColumnsScript.ExecuteNonQuery(connection);

                    if (!string.IsNullOrEmpty(SearchCrawlAccount))
                    {
                        if (!IsSearchCrawlAccountDbo)
                        {
                            EmbeddedSqlScript createSearchUserScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.CreateUser.sql");
                            createSearchUserScript.AddToken("[ACCOUNT_NAME]", SearchCrawlAccount);
                            createSearchUserScript.ExecuteNonQuery(connection);

                            EmbeddedSqlScript grantSearchPermissionsScript = new EmbeddedSqlScript("Glyma.Powershell.Resources.New_GLMapDB.GrantSearchPermissions.sql");
                            grantSearchPermissionsScript.AddToken("[ACCOUNT_NAME]", SearchCrawlAccount);
                            grantSearchPermissionsScript.ExecuteNonQuery(connection);
                        }
                    }
                    else
                    {
                        callingCmdlet.WriteWarning("There was no search crawl account provided. As we are skipping this step, the Glyma search crawl account will need to be manually granted execute permissions on the GetChildNodes, GetNodeInMapDetails, GetNodesInMap, GetParentNodes and GetRootMaps stored proecedures.");
                    }

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
