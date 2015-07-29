using Glyma.Powershell.Smo;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace Glyma.Powershell.Update.v1_5_0_r3
{
    /// <summary>
    /// This class will update a v1.5.0rev2 of the Glyma server to be v1.5.0rev3 compliant.
    /// </summary>
    internal class Update_v1_5_0_r3Base : IUpdateGLDatabase
    {
        private string _transactionDatabaseServer = null;

        private string MapDatabaseConnectionString
        {
            get
            {
                return "Data Source=" + DatabaseServer + ";Initial Catalog=" + MapDatabaseName + ";Integrated Security=True";
            }
        }

        private string TransactionDatabaseConnectionString
        {
            get
            {
                return "Data Source=" + TransactionDatabaseServer + ";Initial Catalog=" + TransactionDatabaseName + ";Integrated Security=True";
            }
        }

        /// <summary>
        /// This is mandatory field. It will be used for both the map database and the transaction database (if none is separately declared).
        /// </summary>
        public string DatabaseServer
        {
            get;
            set;
        }

        /// <summary>
        /// The name of the map database.
        /// </summary>
        public string MapDatabaseName
        {
            get;
            set;
        }

        /// <summary>
        /// If this field is set, it will be used as the database server for the transaction database. If it is not set, the DatabaseServer property will be used.
        /// </summary>
        public string TransactionDatabaseServer
        {
            get
            {
                if (_transactionDatabaseServer == null)
                {
                    return DatabaseServer;
                }

                return _transactionDatabaseServer;
            }
            set
            {
                _transactionDatabaseServer = value;
            }
        }

        /// <summary>
        /// The name of the transaction database.
        /// </summary>
        public string TransactionDatabaseName
        {
            get;
            set;
        }

        private bool UpdateMappingToolDatabase(PSCmdlet callingCmdlet)
        {
            using (MappingTool.MappingToolDatabaseDataContext mapDatabaseContext = new MappingTool.MappingToolDatabaseDataContext(MapDatabaseConnectionString))
            {
                var glymaDbVersion = mapDatabaseContext.GetGlymaDbVersion();

                foreach (var row in glymaDbVersion)
                {
                    if (row.Column1 != "v1.5.0r2")
                    {
                        callingCmdlet.WriteWarning(string.Format("This update can only update the v1.5.0r2 database to v1.5.0r3 database. The version of this database is {0}.", row.Column1));

                        return false;
                    }
                }
            }

            using (SqlConnection mapDbConnection = new SqlConnection(MapDatabaseConnectionString))
            {
                mapDbConnection.Open();

                try
                {
                    EmbeddedSqlScript updateGlymaDbVersion = new EmbeddedSqlScript("Glyma.Powershell.Update.v1_5_0_r3.UpdateGlymaDbVersion.sql");
                    updateGlymaDbVersion.ExecuteNonQuery(mapDbConnection);
                }
                catch
                {
                    return false;
                }

                mapDbConnection.Close();
            }

            return true;
        }

        private bool UpdateTransactionDatabase(PSCmdlet callingCmdlet)
        {
            using (Transaction.TransactionDatabaseDataContext transactionDatabaseContext = new Transaction.TransactionDatabaseDataContext(TransactionDatabaseConnectionString))
            {
                var glymaDbVersion = transactionDatabaseContext.GetGlymaDbVersion();

                foreach (var row in glymaDbVersion)
                {
                    if (row.Column1 != "v1.5.0r2")
                    {
                        callingCmdlet.WriteWarning(string.Format("This update can only update the v1.5.0r2 database to v1.5.0r3 database. The version of this database is {0}.", row.Column1));

                        return false;
                    }
                }
            }

            using (SqlConnection transactionDbConnection = new SqlConnection(TransactionDatabaseConnectionString))
            {
                transactionDbConnection.Open();

                try
                {
                    EmbeddedSqlScript updateGlymaDbVersion = new EmbeddedSqlScript("Glyma.Powershell.Update.v1_5_0_r3.UpdateGlymaDbVersion.sql");
                    updateGlymaDbVersion.ExecuteNonQuery(transactionDbConnection);
                }
                catch
                {
                    return false;
                }

                try
                {
                    EmbeddedSqlScript insertNewTransactionOperations = new EmbeddedSqlScript("Glyma.Powershell.Update.v1_5_0_r3.InsertTransactionOperations.sql");
                    insertNewTransactionOperations.ExecuteNonQuery(transactionDbConnection);
                }
                catch
                {
                    return false;
                }

                transactionDbConnection.Close();
            }

            return true;
        }

        public void ExecuteCmdletBase(PSCmdlet callingCmdlet)
        {
            UpdateMappingToolDatabase(callingCmdlet);
            UpdateTransactionDatabase(callingCmdlet);
        }
    }
}
