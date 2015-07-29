using TransactionalNodeService.Proxy.Universal.Collections.NodeContainers;

namespace TransactionalNodeService.Proxy.Universal.EventArgs
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
