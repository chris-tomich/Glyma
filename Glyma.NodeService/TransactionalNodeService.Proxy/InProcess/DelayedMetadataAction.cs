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

namespace TransactionalNodeService.InProcess
{
    internal class DelayedMetadataAction
    {
        public Proxy.TransactionActionType Action
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

        public Proxy.INode Node
        {
            get;
            set;
        }

        public Proxy.IRelationship Relationship
        {
            get;
            set;
        }

        public Proxy.ConnectionType ConnectionType
        {
            get;
            set;
        }
    }
}
