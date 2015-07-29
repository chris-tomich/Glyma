using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.EventArgs
{
    public class MetadataChangedEventArgs : EventRegisterEventArgs
    {
        public MetadataChangedEventArgs()
            : base()
        {
            Node = null;
            Relationship = null;
            ConnectionType = null;
        }

        public bool IsCommitted
        {
            get;
            set;
        }

        public bool IsCached
        {
            get;
            set;
        }

        public INode Node
        {
            get;
            set;
        }

        public IRelationship Relationship
        {
            get;
            set;
        }

        public ConnectionType ConnectionType
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }
    }
}
