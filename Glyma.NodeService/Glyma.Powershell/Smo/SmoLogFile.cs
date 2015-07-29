using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Glyma.Powershell.Smo
{
    public class SmoLogFile : SmoObjectBase
    {
        private const string SqlSmoLogFile = "Microsoft.SqlServer.Management.Smo.LogFile";

        public SmoLogFile(SqlAssemblies assemblies, SmoDatabase database, string logFileName)
            : base(assemblies, SqlSmoLogFile, database.ReflectedObject, logFileName)
        {
        }

        public void SetFileName(string filename)
        {
            SetPropertyValue("FileName", filename);
        }

        public void SetSize(double size)
        {
            SetPropertyValue("Size", size);
        }

        public void SetGrowthType(string growthType)
        {
            Type fileGrowthEnumType = Assemblies.SqlEnumAssembly.GetType("Microsoft.SqlServer.Management.Smo.FileGrowthType");
            object growthTypeEnumValue = Enum.Parse(fileGrowthEnumType, growthType);

            SetPropertyValue("GrowthType", growthTypeEnumValue);
        }

        public void SetGrowth(double growth)
        {
            SetPropertyValue("Growth", growth);
        }
    }
}
