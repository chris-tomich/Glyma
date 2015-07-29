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
    public class NodesEventArgs : EventRegisterEventArgs
    {
        private NodeCollection _nodes = null;

        public NodesEventArgs()
            : base()
        {
        }

        public NodeCollection Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new NodeCollection();
                }

                return _nodes;
            }
            set
            {
                _nodes = value;
            }
        }
    }
}
