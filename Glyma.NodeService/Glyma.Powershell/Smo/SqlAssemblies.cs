using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Glyma.Powershell.Smo
{
    public class SqlAssemblies
    {
        private const string SqlServer2014SmoAssembly = "Microsoft.SqlServer.Smo, version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
        private const string SqlServer2012SmoAssembly = "Microsoft.SqlServer.Smo, version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
        private const string SqlServer2008AndR2SmoAssembly = "Microsoft.SqlServer.Smo, version=10.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";

        private const string SqlServer2014EnumAssembly = "Microsoft.SqlServer.SqlEnum, version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
        private const string SqlServer2012EnumAssembly = "Microsoft.SqlServer.SqlEnum, version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
        private const string SqlServer2008AndR2EnumAssembly = "Microsoft.SqlServer.SqlEnum, version=10.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";

        public SqlAssemblies()
        {
            try
            {
                /// Try loading SQL Server 2014 SMO assemblies first as SQL Server 2014 will install SQL Server 2012 and 2008 SMO objects too.
                SmoAssembly = Assembly.Load(SqlServer2014SmoAssembly);
                SqlEnumAssembly = Assembly.Load(SqlServer2014EnumAssembly);
            }
            catch
            {
                SmoAssembly = null;
                SqlEnumAssembly = null;
            }

            if (SmoAssembly == null || SqlEnumAssembly == null)
            {
                try
                {
                    /// Try loading SQL Server 2012 SMO assemblies first as SQL Server 2012 will install SQL Server 2008 SMO objects too.
                    SmoAssembly = Assembly.Load(SqlServer2012SmoAssembly);
                    SqlEnumAssembly = Assembly.Load(SqlServer2012EnumAssembly);
                }
                catch
                {
                    SmoAssembly = null;
                    SqlEnumAssembly = null;
                }
            }

            if (SmoAssembly == null || SqlEnumAssembly == null)
            {
                try
                {
                    /// Try loading SQL Server 2012 SMO assemblies first as SQL Server 2012 will install SQL Server 2008 SMO objects too.
                    SmoAssembly = Assembly.Load(SqlServer2008AndR2SmoAssembly);
                    SqlEnumAssembly = Assembly.Load(SqlServer2008AndR2EnumAssembly);
                }
                catch
                {
                    SmoAssembly = null;
                    SqlEnumAssembly = null;
                }
            }

            if (SmoAssembly == null || SqlEnumAssembly == null)
            {
                LoadedSuccessfully = false;
                ErrorMessage = "No compatible version of SQL Server was found (This command is compatible with 2008, 2008 R2, 2012, and 2014).";
            }
            else
            {
                LoadedSuccessfully = true;
                ErrorMessage = string.Empty;
            }
        }

        public bool LoadedSuccessfully
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public Assembly SmoAssembly
        {
            get;
            private set;
        }

        public Assembly SqlEnumAssembly
        {
            get;
            private set;
        }
    }
}
