using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransactionalNodeService.Proxy
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
