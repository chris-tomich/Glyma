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

namespace TransactionalNodeService.Proxy
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
