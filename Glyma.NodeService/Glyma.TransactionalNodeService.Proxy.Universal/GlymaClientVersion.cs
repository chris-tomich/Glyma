using TransactionalNodeService.Proxy.Universal.Service;

namespace TransactionalNodeService.Proxy.Universal
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
