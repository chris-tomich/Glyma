using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Data.SqlClient;

namespace TransactionalNodeService
{
    public partial class MapTransaction
    {
        private class InsertQueryBuilder : IQueryBuilder
        {
            protected const string InsertTransactionSqlQuery = "INSERT INTO [Transactions] ({0}) VALUES ({1}); SELECT SCOPE_IDENTITY() AS [TRANSACTION_ID];";

            private bool _isFirst = true;
            private StringBuilder _fieldNames;
            private StringBuilder _parameterTokens;
            private List<SqlParameter> _sqlParameters;

            public InsertQueryBuilder()
            {
                _fieldNames = new StringBuilder();
                _parameterTokens = new StringBuilder();
                _sqlParameters = new List<SqlParameter>();
            }

            public void AddParameter(string parameterName, object parameterValue)
            {
                string fieldName = "[" + parameterName + "]";
                string parameterToken = "@" + parameterName;

                if (_isFirst)
                {
                    _isFirst = false;
                }
                else
                {
                    _fieldNames.Append(", ");
                    _parameterTokens.Append(", ");
                }

                _fieldNames.Append(fieldName);
                _parameterTokens.Append(parameterToken);

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
                return string.Format(InsertTransactionSqlQuery, _fieldNames.ToString(), _parameterTokens.ToString());
            }

            public SqlParameter[] GenerateSqlParameters()
            {
                return _sqlParameters.ToArray();
            }
        }
    }
}