using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using TransactionalNodeService.Common;

namespace Glyma.NodeServiceApp
{
    public class GlymaDbConnectionFactory : IGlymaConnectionFactory
    {
        private GlymaSessionConfiguration _configuration;

        public GlymaDbConnectionFactory(GlymaSessionConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnectionAbstraction CreateParametersDbConnection()
        {
            string connectionString = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;", _configuration.ParametersDbServer, _configuration.ParametersDbName);

            DbConnectionAbstraction connectionAbstraction = new DbConnectionAbstraction();
            connectionAbstraction.Connection = new SqlConnection(connectionString);

            return connectionAbstraction;
        }

        public IDbConnectionAbstraction CreateSessionDbConnection()
        {
            string connectionString = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;", _configuration.SessionDbServer, _configuration.SessionDbName);

            DbConnectionAbstraction connectionAbstraction = new DbConnectionAbstraction();
            connectionAbstraction.Connection = new SqlConnection(connectionString);

            return connectionAbstraction;
        }

        public IDbConnectionAbstraction CreateMapDbConnection()
        {
            string connectionString = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;", _configuration.MapDbServer, _configuration.MapDbName);

            DbConnectionAbstraction connectionAbstraction = new DbConnectionAbstraction();
            connectionAbstraction.Connection = new SqlConnection(connectionString);

            return connectionAbstraction;
        }

        public IDbConnectionAbstraction CreateSecurityDbConnection()
        {
            string connectionString = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;", _configuration.SecurityDbServer, _configuration.SecurityDbName);

            DbConnectionAbstraction connectionAbstraction = new DbConnectionAbstraction();
            connectionAbstraction.Connection = new SqlConnection(connectionString);

            return connectionAbstraction;
        }
    }
}
