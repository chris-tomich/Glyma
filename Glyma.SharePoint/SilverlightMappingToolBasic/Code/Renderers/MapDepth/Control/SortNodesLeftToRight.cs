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
using System.Linq;

namespace SilverlightMappingToolBasic.MapDepth
{
    public class SortNodesLeftToRight
    {
        private IDictionary<int, List<NodeRenderer>> _columns = new Dictionary<int, List<NodeRenderer>>();
        private List<TreeNodeGroup> _trees = new List<TreeNodeGroup>();

        public SortNodesLeftToRight(INodeNavigator navigator, MapDepthViewManager viewManager)
        {
            Navigator = navigator;
            ViewManager = viewManager;
        }

        private INodeNavigator Navigator
        {
            get;
            set;
        }

        private MapDepthViewManager ViewManager
        {
            get;
            set;
        }

        public void SortLeftToRight()
        {
            lock (_columns)
            {
                _columns.Clear();
                _trees.Clear();

                List<NodeRenderer> leftMostNodes = GetLeftMostNodes();
                leftMostNodes.Sort(new Comparison<NodeRenderer>(NodePositionComparison));
                _columns[1] = leftMostNodes;

                PopulateColumns(2, leftMostNodes, true);

                //position the X coord of each column
                SetNodeXLocations();

                foreach (NodeRenderer leftMostNode in leftMostNodes)
                {
                    TreeNodeGroup treeStructure = BuildTreeNodeGroup(leftMostNode);
                    _trees.Add(treeStructure);
                }

                double marginFromTop = 20; //the margin between the breadcrumb and top of the map
                foreach (TreeNodeGroup treeStructure in _trees)
                {
                    TreeRenderer renderer = new TreeRenderer(marginFromTop);
                    renderer.PositionNodes(treeStructure, 1);
                    marginFromTop = renderer.GetMaximumYPosition();
                }

                //Store the new locations
                foreach (NodeRenderer nodeRenderer in ViewManager.CurrentView.NodeRenderers.Values)
                {
                    nodeRenderer.StoreLocation();
                }
            }
        }

        private TreeNodeGroup BuildTreeNodeGroup(NodeRenderer leftMostNode)
        {
            TreeNodeGroup nodeGroup = new TreeNodeGroup();
            nodeGroup.ParentNode = leftMostNode;
            nodeGroup.ChildNodes = GetChildNodeRenderers(leftMostNode);
            nodeGroup.ChildNodes.Sort(new Comparison<NodeRenderer>(NodePositionComparison));
            nodeGroup.ParentChildGroups = new List<TreeNodeGroup>();

            foreach (NodeRenderer nodeRenderer in nodeGroup.ChildNodes)
            {
                TreeNodeGroup childGroup = BuildTreeNodeGroup(nodeRenderer);
                nodeGroup.ParentChildGroups.Add(childGroup);
            }
            return nodeGroup;
        }

        private void PopulateColumns(int columnNumber, List<NodeRenderer> parentNodes, bool useLocationOrder)
        {
            foreach (NodeRenderer nodeRenderer in parentNodes)
            {
                List<NodeRenderer> childNodes = GetChildNodeRenderers(nodeRenderer);
                if (useLocationOrder)
                {
                    childNodes.Sort(new Comparison<NodeRenderer>(NodePositionComparison));
                }

                if (!_columns.ContainsKey(columnNumber))
                {
                    _columns[columnNumber] = new List<NodeRenderer>();
                }
                _columns[columnNumber].AddRange(childNodes);
            }
            if (_columns[columnNumber].Count() != 0)
            {
                int nextColumn = columnNumber + 1;
                PopulateColumns(nextColumn, _columns[columnNumber], true);
            }
            else
            {
                _columns.Remove(columnNumber); //trim the empty column off
            }
        }

        private List<NodeRenderer> GetLeftMostNodes()
        {
            List<NodeRenderer> result = new List<NodeRenderer>();
            foreach (NodeRenderer nodeRenderer in this.ViewManager.CurrentView.NodeRenderers.Values)
            {
                bool hasFromDescriptor = false;
                foreach (IDescriptorProxy descriptor in nodeRenderer.Node.Descriptors.Where(
                    x => x.Relationship.RelationshipType.Name == "FromToRelationship" || x.Relationship.RelationshipType.Name == "TransclusionRelationship"))
                {
                    if (descriptor.DescriptorType.Name == "From")
                    {
                        hasFromDescriptor = true;
                        break;
                    }
                }
                if (!hasFromDescriptor)
                {
                    result.Add(nodeRenderer);
                }
            }
            return result;
        }

        /// <summary>
        /// Used as the comparison for sorting the nodes based on their initial position.
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        private int NodePositionComparison(NodeRenderer node1, NodeRenderer node2)
        {
            double node1YPosition = (double)node1.GetValue(Canvas.TopProperty);
            double node2YPosition = (double)node2.GetValue(Canvas.TopProperty);
            if (node1YPosition > node2YPosition)
            {
                return 1;
            }
            else if (node1YPosition < node2YPosition)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private double CalculateColumnWidth(List<NodeRenderer> nodeRenderers)
        {
            double result = 0;
            foreach (NodeRenderer nodeRenderer in nodeRenderers)
            {
                if (nodeRenderer.NodeControl.ActualWidth > result)
                {
                    result = nodeRenderer.NodeControl.ActualWidth;
                }
            }
            return result;
        }

        private double GetColumnXPosition(int columnNumber)
        {
            double result = 0;
            for (int c = 1; c < columnNumber; c++)
            {
                result += CalculateColumnWidth(_columns[c]);
                result += 75; //nominal spacing between columns
            }
            result += 50; //left margin
            return result;
        }

        private void SetNodeXLocations()
        {
            for (int columnNumber = 1; columnNumber <= _columns.Count; columnNumber++)
            {
                foreach (NodeRenderer nodeRenderer in _columns[columnNumber])
                {
                    double position = GetColumnXPosition(columnNumber);
                    nodeRenderer.MoveNode(new Point(position, nodeRenderer.Location.Y), true);
                }
            }
        }

        private List<NodeRenderer> GetChildNodeRenderers(NodeRenderer parentNode)
        {
            List<NodeRenderer> result = new List<NodeRenderer>();
            foreach (IDescriptorProxy descriptor in parentNode.Node.Descriptors.Where(d => d.DescriptorType.Name == "To"
                && d.Relationship.RelationshipType.Name != "MapContainerRelationship"))
            {
                if (descriptor.Relationship.RelationshipType.Name == "TransclusionRelationship")
                {
                    IDescriptorProxy transMapDescriptor = descriptor.Relationship.Descriptors.Where(d => d.DescriptorType.Name == "TransclusionMap").First();
                    if (transMapDescriptor.NodeId != this.Navigator.FocalNodeId ||
                        descriptor.Relationship.Descriptors.Where(d => d.DescriptorType.Name == "From").Count() == 0)
                    {
                        continue;
                    }
                }
                IDescriptorProxy fromDescriptor = descriptor.Relationship.Descriptors.Where(d => d.DescriptorType.Name == "From").First();
                if (this.ViewManager.CurrentView.NodeRenderers.ContainsKey(fromDescriptor.NodeId))
                {
                    NodeRenderer childNodeRenderer = this.ViewManager.CurrentView.NodeRenderers[fromDescriptor.NodeId];
                    result.Add(childNodeRenderer);
                }
            }
            return result;
        }
    }
}
