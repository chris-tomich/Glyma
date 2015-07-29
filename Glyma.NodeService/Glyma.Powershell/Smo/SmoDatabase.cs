using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Glyma.Powershell.Smo
{
    public class SmoDatabase : SmoObjectBase
    {
        private const string SqlSmoDatabase = "Microsoft.SqlServer.Management.Smo.Database";

        public SmoDatabase(SqlAssemblies assemblies, SmoServer server, string databaseName)
            : base(assemblies, SqlSmoDatabase, server.ReflectedObject, databaseName)
        {
        }

        public void AddFileGroup(SmoFileGroup fileGroup)
        {
            object dbFileGroupCollection = GetPropertyValue("FileGroups");

            InvokeMethod("Add", dbFileGroupCollection, fileGroup.ReflectedObject);
        }

        public void AddLogFile(SmoLogFile logFile)
        {
            object dbLogFileCollection = GetPropertyValue("LogFiles");

            InvokeMethod("Add", dbLogFileCollection, logFile.ReflectedObject);
        }

        public SmoFileGroup GetFileGroup(string fileGroupName)
        {
            object fileGroupsCollection = GetPropertyValue("FileGroups");

            object fileGroup = GetPropertyValue("Item", fileGroupsCollection, fileGroupName);

            return new SmoFileGroup(fileGroup);
        }

        public void Create()
        {
            InvokeMethod("Create");
        }

        public void Alter()
        {
            InvokeMethod("Alter");
        }
    }
}
