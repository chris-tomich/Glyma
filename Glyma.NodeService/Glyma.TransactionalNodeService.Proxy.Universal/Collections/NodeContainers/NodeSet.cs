using System.Collections.Generic;
using System.Linq;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Collections.NodeContainers
{
    public sealed class NodeSet : INodeQueryable, INodeManager
    {
        private HashSet<ConnectionSet> _connectionSets = null;

        public NodeSet(IRelationship relationshipContext)
        {
            RelationshipContext = relationshipContext;
        }

        private IRelationship RelationshipContext
        {
            get;
            set;
        }

        private HashSet<ConnectionSet> Connections
        {
            get
            {
                if (_connectionSets == null)
                {
                    _connectionSets = new HashSet<ConnectionSet>();
                }

                return _connectionSets;
            }
        }

        void INodeManager.Load(ConnectionSet connectionSet)
        {
            ConnectionSet setToUpdate = null;

            foreach (ConnectionSet set in Connections)
            {
                if (set.ConnectionType.Equals(connectionSet.ConnectionType))
                {
                    setToUpdate = set;

                    break;
                }
            }

            if (setToUpdate != null)
            {
                Connections.Remove(setToUpdate);
            }

            Connections.Add(connectionSet);
        }

        void INodeManager.Clear()
        {
            Connections.Clear();
        }

        void INodeManager.UnionWith(NodeSet nodes)
        {
            if (this != nodes)
            {
                Connections.UnionWith(nodes.Connections);
            }
        }

        public IEnumerable<NodeTuple> FindNodes(bool ignorePartiallyLoadedNodes)
        {
            IEnumerable<NodeTuple> nodes = null;

            if (ignorePartiallyLoadedNodes)
            {
                nodes = from nodePair in Connections
                        where nodePair.Node.Status == LoadState.Full
                        select (new NodeTuple() { ConnectionType = nodePair.ConnectionType, Node = nodePair.Node });
            }
            else
            {
                nodes = from nodePair in Connections
                        select (new NodeTuple() { ConnectionType = nodePair.ConnectionType, Node = nodePair.Node });
            }

            return nodes;
        }

        public IEnumerable<INode> FindNodes(ConnectionType connectionType, bool ignorePartiallyLoadedNodes)
        {
            List<INode> nodes = new List<INode>();

            foreach (ConnectionSet connectionSet in Connections)
            {
                if (connectionSet.ConnectionType.Equals(connectionType))
                {
                    if (ignorePartiallyLoadedNodes)
                    {
                        if (connectionSet.Node.Status == LoadState.Full)
                        {
                            nodes.Add(connectionSet.Node);
                        }
                    }
                    else
                    {
                        nodes.Add(connectionSet.Node);
                    }
                }
            }

            if (nodes.Count > 0)
            {
                return nodes;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<INode> FindNodes(NodeType nodeType, bool ignorePartiallyLoadedNodes)
        {
            List<INode> nodes = new List<INode>();

            foreach (ConnectionSet connectionSet in Connections)
            {
                if (connectionSet.Node.NodeType.Equals(nodeType))
                {
                    if (ignorePartiallyLoadedNodes)
                    {
                        if (connectionSet.Node.Status == LoadState.Full)
                        {
                            nodes.Add(connectionSet.Node);
                        }
                    }
                    else
                    {
                        nodes.Add(connectionSet.Node);
                    }
                }
            }

            if (nodes.Count > 0)
            {
                return nodes;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<INode> FindNodes(ConnectionType connectionType, NodeType nodeType, bool ignorePartiallyLoadedNodes)
        {
            List<INode> nodes = new List<INode>();

            foreach (ConnectionSet connectionSet in Connections)
            {
                if (connectionSet.ConnectionType.Equals(connectionType) && connectionSet.Node.NodeType.Equals(nodeType))
                {
                    if (ignorePartiallyLoadedNodes)
                    {
                        if (connectionSet.Node.Status == LoadState.Full)
                        {
                            nodes.Add(connectionSet.Node);
                        }
                    }
                    else
                    {
                        nodes.Add(connectionSet.Node);
                    }
                }
            }

            if (nodes.Count > 0)
            {
                return nodes;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable<NodeTuple> FindNodes()
        {
            return FindNodes(false);
        }

        public IEnumerable<INode> FindNodes(ConnectionType connectionType)
        {
            return FindNodes(connectionType, false);
        }

        public IEnumerable<INode> FindNodes(NodeType nodeType)
        {
            return FindNodes(nodeType, false);
        }

        public IEnumerable<INode> FindNodes(ConnectionType connectionType, NodeType nodeType)
        {
            return FindNodes(connectionType, nodeType, false);
        }

        internal void UpdateConnection(ConnectionType connectionType, INode node)
        {
            ConnectionSet matchingSet = null;

            foreach (ConnectionSet connectionSet in Connections)
            {
                if (connectionSet.ConnectionType.Equals(connectionType))
                {
                    matchingSet = connectionSet;
                    break;
                }
            }

            if (matchingSet != null && matchingSet.ConnectionType != null)
            {
                matchingSet.Node = node;
            }
            else
            {
                // This may occur if the type has just been changed for the relationship to which this NodeSet belongs.
                INodeManager nodeManager = this;

                matchingSet = ConnectionSetFactory.Instance.GetConnection(node, RelationshipContext, connectionType);
                nodeManager.Load(matchingSet);
            }
        }
    }
}
