using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransactionalNodeService.Common.Model;

namespace TransactionalNodeService.Common
{
    public class GlymaServerVersion
    {
        public static GlymaVersion Current
        {
            get
            {
                GlymaVersion version = new GlymaVersion();
                version.Major = 1;
                version.Minor = 5;
                version.Build = 0;
                version.Revision = 7;

                return version;
            }
        }
    }
}