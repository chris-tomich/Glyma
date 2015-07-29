using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Glyma.Powershell.Smo
{
    public class SmoServer : SmoObjectBase
    {
        private const string SqlSmoServer = "Microsoft.SqlServer.Management.Smo.Server";

        public SmoServer(SqlAssemblies assemblies, string databaseServer)
            : base(assemblies, SqlSmoServer, databaseServer)
        {
        }

        public void SetApplicationName(string applicationName)
        {
            object connectionContext = GetPropertyValue("ConnectionContext");

            SetPropertyValue("ApplicationName", connectionContext, applicationName);
        }

        public string GetMasterDbPath()
        {
            object information = GetPropertyValue("Information");

            object masterDbPath = GetPropertyValue("MasterDBPath", information);

            return (string)masterDbPath;
        }
    }
}
