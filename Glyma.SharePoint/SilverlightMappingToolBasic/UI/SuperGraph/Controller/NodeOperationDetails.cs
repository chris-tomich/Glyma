using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using SilverlightMappingToolBasic.UI.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller
{
    public class NodeOperationDetails
    {
        private IEnumerable<Node> _nodes;
        private IEnumerable<Relationship> _relationships;

        public NodeOperation Operation
        {
            get;
            set;
        }

        public INode Map
        {
            get;
            set;
        }

        public Guid DomainId
        {
            get
            {
                return Map.DomainId;
            }
        }

        public IEnumerable<Node> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new List<Node>();
                }
                return _nodes;
            }
            set
            {
                _nodes = value;
            }
        }

        public IEnumerable<Relationship> Relationships
        {
            get
            {
                if (_relationships == null)
                {
                    _relationships = new List<Relationship>();
                }
                return _relationships;
            }
            set
            {
                _relationships = value;
            }
        }

        public bool HasNodes
        {
            get
            {
                return Nodes.Any();
            }
        }

        public Point? OldLocation { get; set; }

        public NodeOperationDetails(NodeOperation operation, INode map, Node node)
        {
            Operation = operation;
            Map = map;
            Nodes = new [] {node};
            OldLocation = node.Location;
        }

        public NodeOperationDetails(NodeOperation operation, INode map, IEnumerable<Node> nodes, IEnumerable<Relationship> relationships)
        {
            Operation = operation;
            Map = map;
            Nodes = nodes;
            Relationships = relationships;

            if (OldLocation == null)
            {
                OldLocation = Nodes.Select(q => q.Location).OrderBy(q => q.X).ThenBy(q => q.Y).FirstOrDefault();
            }
        }
    }
}
