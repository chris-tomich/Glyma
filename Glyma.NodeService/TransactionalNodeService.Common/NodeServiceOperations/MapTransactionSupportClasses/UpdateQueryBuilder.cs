using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.SqlClient;

namespace TransactionalNodeService.Common
{
    public partial class MapTransaction
    {
        private class UpdateQueryBuilder : IQueryBuilder
        {
            protected const string UpdateTransactionSqlQuery = "UPDATE [Transactions] SET {0} WHERE [TransactionId] = @TransactionId";

            private bool _isFirst = true;
            private StringBuilder _parameterSetList;
            private List<SqlParameter> _sqlParameters;

            public UpdateQueryBuilder()
            {
                _parameterSetList = new StringBuilder();
                _sqlParameters = new List<SqlParameter>();
            }

            public void AddParameter(string parameterName, object parameterValue)
            {
                string parameterToken = "@" + parameterName;
                string parameterSetPair = "[" + parameterName + "] = " + parameterToken;

                if (_isFirst)
                {
                    _isFirst = false;
                }
                else
                {
                    _parameterSetList.Append(", ");
                }

                _parameterSetList.Append(parameterSetPair);

                SqlParameter parameter;

                if (parameterValue == null)
                {
                    parameter = new SqlParameter(parameterToken, DBNull.Value);
                }
                else
                {
                    parameter = new SqlParameter(parameterToken, parameterValue);
                }

                _sqlParameters.Add(parameter);
            }

            public string GenerateSqlQuery()
            {
                return string.Format(UpdateTransactionSqlQuery, _parameterSetList.ToString());
            }

            public SqlParameter[] GenerateSqlParameters()
            {
                return _sqlParameters.ToArray();
            }
        }
    }
}