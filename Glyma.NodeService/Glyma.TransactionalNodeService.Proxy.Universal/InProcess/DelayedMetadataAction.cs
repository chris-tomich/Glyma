using System;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.InProcess
{
    internal class DelayedMetadataAction
    {
        public TransactionActionType Action
        {
            get;
            set;
        }

        public Guid DomainId
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
    }
}
