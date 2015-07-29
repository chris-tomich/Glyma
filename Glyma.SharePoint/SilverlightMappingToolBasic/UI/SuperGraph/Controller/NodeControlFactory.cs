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
using SilverlightMappingToolBasic.UI.SuperGraph.View;
using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller
{
    public class NodeControlFactory
    {
        private Dictionary<Guid, NodeControlFactorySet> _nodes = null;

        public NodeControlFactory()
        {
        }

        private Dictionary<Guid, NodeControlFactorySet> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new Dictionary<Guid, NodeControlFactorySet>();
                }

                return _nodes;
            }
        }

        public NodeControl CreateNode(ViewModel.Node viewModelNode, SuperGraphControl parentSurface)
        {
            NodeControl nodeControl = null;

            /// TODO: We will need to consider being able to have the same viewModelNode present here twice.
            if (!Nodes.ContainsKey(viewModelNode.Id))
            {
                nodeControl = new NodeControl();
                Canvas.SetZIndex(nodeControl, 50);
                nodeControl.ParentSurface = parentSurface;
                nodeControl.DataContext = viewModelNode;
                nodeControl.VideoRemoteControl.DataContext = viewModelNode.VideoInfo;
                //nodeControl.NodeCornerControl.DataContext = viewModelNode;
                NodeControlFactorySet set = new NodeControlFactorySet();
                set.Node = viewModelNode;
                set.Control = nodeControl;

                Nodes.Add(viewModelNode.Id, set);
            }
            else
            {
                nodeControl = Nodes[viewModelNode.Id].Control;
            }

            return nodeControl;
        }

        public ViewModel.Node FindNodeViewModel(Guid nodeId)
        {
            if (Nodes.ContainsKey(nodeId))
            {
                return Nodes[nodeId].Node;
            }

            return null;
        }

        public NodeControl FindNodeControl(Guid nodeId)
        {
            if (Nodes.ContainsKey(nodeId))
            {
                return Nodes[nodeId].Control;
            }

            return null;
        }

        public NodeControlFactorySet RemoveNode(ViewModel.Node viewModelNode)
        {
            if (Nodes.ContainsKey(viewModelNode.Id))
            {
                NodeControlFactorySet removedSet = Nodes[viewModelNode.Id];
                Nodes.Remove(viewModelNode.Id);

                return removedSet;
            }

            return null;
        }

        public IEnumerable<NodeControlFactorySet> Clear()
        {
            foreach (NodeControlFactorySet set in Nodes.Values)
            {
                yield return set;
            }

            Nodes.Clear();
        }
    }
}
