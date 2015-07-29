using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Data.SqlClient;

namespace Glyma.Powershell.Smo
{
    public class EmbeddedSqlScript
    {
        Dictionary<string, string> _tokens;

        public EmbeddedSqlScript(string scriptName)
            : this(scriptName, Assembly.GetCallingAssembly())
        {
        }

        public EmbeddedSqlScript(string scriptName, Assembly assembly)
        {
            Stream scriptStream = null;

            try
            {
                scriptStream = assembly.GetManifestResourceStream(scriptName);

                using (StreamReader scriptStreamReader = new StreamReader(scriptStream))
                {
                    Script = scriptStreamReader.ReadToEnd();
                }
            }
            catch
            {
                throw new FileNotFoundException("Could not find the specified resource.", scriptName);
            }
            finally
            {
                if (scriptStream != null)
                {
                    scriptStream.Dispose();
                }
            }
        }

        private Dictionary<string, string> Tokens
        {
            get
            {
                if (_tokens == null)
                {
                    _tokens = new Dictionary<string, string>();
                }

                return _tokens;
            }
        }

        public string Script
        {
            get;
            private set;
        }

        public void AddToken(string token, string tokenValue)
        {
            Tokens[token] = tokenValue;
        }

        public int ExecuteNonQuery(SqlConnection connection)
        {
            StringBuilder query = new StringBuilder(Script);

            if (Tokens.Count > 0)
            {
                foreach (KeyValuePair<string, string> tokenPair in Tokens)
                {
                    query.Replace(tokenPair.Key, tokenPair.Value);
                }
            }

            int rowsModified;

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = query.ToString();
                rowsModified = command.ExecuteNonQuery();
            }

            return rowsModified;
        }
    }
}
