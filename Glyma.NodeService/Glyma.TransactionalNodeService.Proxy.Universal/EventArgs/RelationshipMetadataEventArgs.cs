using TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection;

namespace TransactionalNodeService.Proxy.Universal.EventArgs
{
    public class RelationshipMetadataEventArgs : EventRegisterEventArgs
    {
        public RelationshipMetadataEventArgs()
            : base()
        {
        }

        public RelationshipMetadataCollection Metadata
        {
            get;
            set;
        }
    }
}
