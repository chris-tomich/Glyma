using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Glyma.Powershell.Smo
{
    public class SmoDataFile : SmoObjectBase
    {
        private const string SqlSmoDataFile = "Microsoft.SqlServer.Management.Smo.DataFile";

        public SmoDataFile(SqlAssemblies assemblies, SmoFileGroup fileGroup, string dataFileName)
            : base(assemblies, SqlSmoDataFile, fileGroup.ReflectedObject, dataFileName)
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

        public void SetIsPrimaryFile(bool isPrimaryFile)
        {
            SetPropertyValue("IsPrimaryFile", isPrimaryFile);
        }
    }
}
