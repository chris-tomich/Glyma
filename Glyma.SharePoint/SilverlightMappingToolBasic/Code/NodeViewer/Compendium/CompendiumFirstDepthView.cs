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

namespace SilverlightMappingToolBasic.Compendium
{
    public class CompendiumFirstDepthView : INodeView, INodeActions
    {
        private INode _currentNode;
        private INode[] _nodes;
        private INodesReader _nodeReader;
        private List<INodeRenderer> _registeredSurfaces;

        public CompendiumFirstDepthView()
        {
            _registeredSurfaces = new List<INodeRenderer>();
        }

        public CompendiumFirstDepthView(INodesReader nodeReader)
            : this()
        {
            _nodeReader = nodeReader;

            _nodes = _nodeReader.GetAllNodes();
            _currentNode = _nodes[1];
        }

        #region INodeView Members

        public INodeViewInfo[] GetNodes()
        {
            return GetNodes(x => true);
        }

        public INodeViewInfo[] GetNodes(Func<INodeViewInfo, bool> selector)
        {
            List<INodeViewInfo> nodes = new List<INodeViewInfo>();

            NodeViewInfo focalNodeViewInfo = new NodeViewInfo();
            focalNodeViewInfo.Node = _currentNode;
            focalNodeViewInfo.IsFocalNode = true;

            if (selector(focalNodeViewInfo))
            {
                nodes.Add(focalNodeViewInfo);
            }

            foreach (IDescriptor descriptor in _currentNode.Descriptors)
            {
                foreach (IDescriptor nodeDescriptor in descriptor.Relationship.Descriptors)
                {
                    if (_currentNode.NodeType is MapNode)
                    {
                        if (!nodeDescriptor.Node.Equals(_currentNode))
                        {
                            NodeViewInfo nodeViewInfo = new NodeViewInfo();
                            nodeViewInfo.Node = nodeDescriptor.Node;

                            if (selector(nodeViewInfo))
                            {
                                nodes.Add(nodeViewInfo);
                            }
                        }
                    }
                    else
                    {
                        if (!nodeDescriptor.Node.Equals(_currentNode))
                        {
                            NodeViewInfo nodeViewInfo = new NodeViewInfo();
                            nodeViewInfo.Node = nodeDescriptor.Node;

                            if (selector(nodeViewInfo))
                            {
                                nodes.Add(nodeViewInfo);
                            }
                        }
                    }
                }
            }

            return nodes.ToArray();
        }

        #endregion

        #region INodeActions Members

        public void RegisterSurface(INodeRenderer surface)
        {
            _registeredSurfaces.Add(surface);
        }

        public void SetFocalNode(INode node)
        {
            _currentNode = node;

            foreach (INodeRenderer registeredSurface in _registeredSurfaces)
            {
                registeredSurface.RefreshView();
            }
        }

        #endregion
    }
}
