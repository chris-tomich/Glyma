using TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection;

namespace TransactionalNodeService.Proxy.Universal.EventArgs
{
    public class NodeMetadataEventArgs : EventRegisterEventArgs
    {
        public NodeMetadataEventArgs()
            : base()
        {
        }

        public NodeMetadataCollection Metadata
        {
            get;
            set;
        }
    }
}
