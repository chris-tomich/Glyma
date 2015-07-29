using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal.Collections.NodeContainers;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal
{
    public interface INodeQueryable
    {
        IEnumerable<NodeTuple> FindNodes();
        IEnumerable<INode> FindNodes(ConnectionType connectionType);
        IEnumerable<INode> FindNodes(NodeType nodeType);
        IEnumerable<INode> FindNodes(ConnectionType connectionType, NodeType nodeType);

        IEnumerable<NodeTuple> FindNodes(bool ignorePartiallyLoadedNodes);
        IEnumerable<INode> FindNodes(ConnectionType connectionType, bool ignorePartiallyLoadedNodes);
        IEnumerable<INode> FindNodes(NodeType nodeType, bool ignorePartiallyLoadedNodes);
        IEnumerable<INode> FindNodes(ConnectionType connectionType, NodeType nodeType, bool ignorePartiallyLoadedNodes);
    }
}
