using System;
using System.Collections.Generic;

namespace TransactionalNodeService.Proxy.Universal.ServerObjects
{
    public class QueryResponse
    {
        private Dictionary<Guid, Node> _nodes = null;
        private Dictionary<Guid, Metadata> _metadata = null;
        private Dictionary<Guid, Descriptor> _descriptors = null;
        private Dictionary<Guid, Relationship> _relationships = null;

        public QueryResponse()
        {
            StartingObjectIndex = 0;
            LastObjectIndex = 0;
            FinalObjectIndex = 0;
            ErrorId = 0;
            ErrorMessage = string.Empty;
        }

        public int ErrorId
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public Guid Domain
        {
            get;
            set;
        }

        public Node NodeContext
        {
            get;
            set;
        }

        public int StartingObjectIndex
        {
            get;
            set;
        }

        public int LastObjectIndex
        {
            get;
            set;
        }

        public int FinalObjectIndex
        {
            get;
            set;
        }

        public Dictionary<Guid, Node> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new Dictionary<Guid, Node>();
                }

                return _nodes;
            }
            set
            {
                _nodes = value;
            }
        }

        public Dictionary<Guid, Relationship> Relationships
        {
            get
            {
                if (_relationships == null)
                {
                    _relationships = new Dictionary<Guid, Relationship>();
                }

                return _relationships;
            }
            set
            {
                _relationships = value;
            }
        }
    }
}