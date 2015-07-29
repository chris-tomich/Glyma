using System.Collections.Generic;

namespace TransactionalNodeService.Proxy.Universal.EventArgs
{
    public class MetadataEventArgs : EventRegisterEventArgs
    {
        public MetadataEventArgs()
            : base()
        {
        }

        public IDictionary<string, object> Metadata
        {
            get;
            set;
        }
    }
}
