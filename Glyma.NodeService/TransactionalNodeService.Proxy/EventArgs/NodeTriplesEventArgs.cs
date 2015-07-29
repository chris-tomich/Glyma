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
using System.Collections.Generic;

namespace TransactionalNodeService.Proxy
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
