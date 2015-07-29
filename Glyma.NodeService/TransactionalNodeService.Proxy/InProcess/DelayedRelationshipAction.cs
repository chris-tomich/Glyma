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
    internal class DelayedRelationshipAction
    {
        public Proxy.TransactionActionType Action
        {
            get;
            set;
        }

        public Proxy.RelationshipType RelationshipType
        {
            get;
            set;
        }

        public Proxy.ConnectionType ConnectionType
        {
            get;
            set;
        }

        public Proxy.INode Node
        {
            get;
            set;
        }
    }
}
