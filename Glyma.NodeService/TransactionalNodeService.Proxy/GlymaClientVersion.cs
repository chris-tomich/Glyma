using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TransactionalNodeService.Service;

namespace TransactionalNodeService
{
    public class GlymaClientVersion
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

        public static bool RequiresRefresh(GlymaVersion serverVersion)
        {
            if (serverVersion.Major != GlymaClientVersion.Current.Major || serverVersion.Minor != GlymaClientVersion.Current.Minor || serverVersion.Build != GlymaClientVersion.Current.Build || serverVersion.Revision != GlymaClientVersion.Current.Revision)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
