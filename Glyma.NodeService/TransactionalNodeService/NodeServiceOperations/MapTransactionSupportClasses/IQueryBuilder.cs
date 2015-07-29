using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace TransactionalNodeService
{
    public partial class MapTransaction
    {
        private interface IQueryBuilder
        {
            void AddParameter(string parameterName, object parameterValue);
            string GenerateSqlQuery();
            SqlParameter[] GenerateSqlParameters();
        }
    }
}