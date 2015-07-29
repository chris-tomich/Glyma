using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal.Collections.NodeContainers;

namespace TransactionalNodeService.Proxy.Universal.EventArgs
{
    public class NodeTriplesEventArgs : EventRegisterEventArgs
    {
        private List<NodeTriple> _triples = null;

        public NodeTriplesEventArgs()
            : base()
        {
        }

        public List<NodeTriple> Triples
        {
            get
            {
                if (_triples == null)
                {
                    _triples = new List<NodeTriple>();
                }

                return _triples;
            }
            set
            {
                _triples = value;
            }
        }
    }
}
