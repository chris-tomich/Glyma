using System;
using System.Collections.Generic;

namespace TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework
{
    public class SoapTransactionCompletedEventArgs : System.EventArgs
    {
        public SoapTransactionCompletedEventArgs()
        {
        }

        public Guid TransactionSessionId
        {
            get;
            set;
        }

        public List<NodeChangedTuple> Nodes
        {
            get;
            set;
        }

        public List<RelationshipChangedTuple> Relationships
        {
            get;
            set;
        }
    }
}
