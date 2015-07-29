using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace TransactionalNodeService.Proxy
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
