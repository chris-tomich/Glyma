using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace TransactionalNodeService.Common.NodeServiceOperations
{
    internal class MapParameterSqlBuilder
    {
        //"UPDATE [Parameters] SET [Value] = @Value, [SessionUid] = @SessionUid, [IsDelayed] = @IsDelayed, [ParameterType] = @ParameterType WHERE [ParameterUid] = @ParameterUid"
        protected const string UpdateParameter = "UPDATE [Parameters] SET {0} WHERE [ParameterUid] = @ParameterUid";

        Dictionary<string, string> _parameters;

        public MapParameterSqlBuilder()
        {
            _parameters = new Dictionary<string, string>();
        }

        public void Add(string parameterName, string parameterToken)
        {
            _parameters[parameterName] = parameterToken;
        }

        public string BuildSql()
        {
            bool isFirst = true;
            StringBuilder sqlStringBuilder = new StringBuilder();

            foreach (KeyValuePair<string, string> parameterDetails in _parameters)
            {
                if (!isFirst)
                {
                    sqlStringBuilder.Append(", ");
                }

                sqlStringBuilder.Append(parameterDetails.Key);
                sqlStringBuilder.Append(" = ");
                sqlStringBuilder.Append(parameterDetails.Value);
            }

            string sqlString = string.Format(UpdateParameter, sqlStringBuilder.ToString());

            return sqlString;
        }
    }
}