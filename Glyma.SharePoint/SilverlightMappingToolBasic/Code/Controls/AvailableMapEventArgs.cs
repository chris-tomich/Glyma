using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SilverlightMappingToolBasic.Controls
{
    public class AvailableMapEventArgs : EventArgs
    {
        public AvailableMapEventArgs()
        {
        }

        public AvailableMapEventArgs(ReturnedMapNodesEventArgs eventArgs)
        {
            DomainId = eventArgs.DomainId;
            Nodes = eventArgs.Nodes;
        }

        public Guid DomainId
        {
            get;
            private set;
        }

        public INodeProxy[] Nodes
        {
            get;
            private set;
        }
    }
}
