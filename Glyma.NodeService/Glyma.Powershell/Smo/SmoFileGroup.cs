using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Glyma.Powershell.Smo
{
    public class SmoFileGroup : SmoObjectBase
    {
        private const string SqlSmoFileGroup = "Microsoft.SqlServer.Management.Smo.FileGroup";

        public SmoFileGroup(object smoFileGroupObj)
        {
            if (smoFileGroupObj.GetType().FullName != SqlSmoFileGroup)
            {
                throw new NotSupportedException("The provided object is not a SQL SMO FileGroup object.");
            }

            ReflectedObject = smoFileGroupObj;
        }

        public SmoFileGroup(SqlAssemblies assemblies, SmoDatabase database, string fileGroupName)
            : base(assemblies, SqlSmoFileGroup, database.ReflectedObject, fileGroupName)
        {
        }

        public void AddDataFile(SmoDataFile dataFile)
        {
            object filesCollection = GetPropertyValue("Files");

            InvokeMethod("Add", filesCollection, dataFile.ReflectedObject);
        }

        public void SetIsDefault(bool isDefault)
        {
            SetPropertyValue("IsDefault", isDefault);
        }

        public void Alter()
        {
            InvokeMethod("Alter");
        }
    }
}
