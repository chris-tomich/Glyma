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

namespace SilverlightMappingToolBasic.Test
{
    public class TestView : INodeView, INodeActions
    {
        private INode _focalNode;
        private INode[] _nodes;

        public TestView()
        {
            _focalNode = new TestNode() { Id = "1", Name = "Main Focal Node" };

            _nodes = new TestNode[] { new TestNode() { Id = "2", Name = "1. Test" },
                                      new TestNode() { Id = "3", Name = "Test 1" },
                                      new TestNode() { Id = "4", Name = "Testo Uno" },
                                      new TestNode() { Id = "5", Name = "Test - 1" },
                                      new TestNode() { Id = "6", Name = "A single test." },
                                      new TestNode() { Id = "4", Name = "Testo Uno" },
                                      new TestNode() { Id = "5", Name = "Test - 1" },
                                      new TestNode() { Id = "6", Name = "A single test." }};
        }

        #region INodeView Members

        public INodeViewInfo[] GetNodes()
        {
            List<INodeViewInfo> nodeInfoList = new List<INodeViewInfo>();

            foreach (INode node in _nodes)
            {
                NodeViewInfo nodeInfo = new NodeViewInfo();
                nodeInfo.Node = node;

                nodeInfoList.Add(nodeInfo);
            }

            return nodeInfoList.ToArray();
        }

        public INodeViewInfo[] GetNodes(Func<INodeViewInfo, bool> selector)
        {
            List<INodeViewInfo> nodeInfoList = new List<INodeViewInfo>();

            foreach (INode node in _nodes)
            {
                NodeViewInfo nodeInfo = new NodeViewInfo();
                nodeInfo.Node = node;

                if (selector(nodeInfo))
                {
                    nodeInfoList.Add(nodeInfo);
                }
            }

            return nodeInfoList.ToArray();
        }

        #endregion

        #region INodeActions Members

        public void RegisterSurface(INodeRenderer surface)
        {
            throw new NotImplementedException();
        }

        public void SetFocalNode(INode node)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
