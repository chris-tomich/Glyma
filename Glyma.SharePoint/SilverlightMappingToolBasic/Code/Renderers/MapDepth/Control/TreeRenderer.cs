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

namespace SilverlightMappingToolBasic.MapDepth
{
    public class TreeRenderer
    {
        private const double VERTICAL_BUFFER = 25; //the buffer between nodes in the column

        private IDictionary<int, double> _depthYMargin = new Dictionary<int, double>(); //keeps track of the Y position as it fills
        private int _maxTreeDepth = 0; //as you move down the map you need to know the maximum depth and positions of those nodes

        public TreeRenderer(double marginFromTop)
        {
            MarginFromTop = marginFromTop;
        }

        public double MarginFromTop
        {
            get;
            set;
        }

        /// <summary>
        /// Recursively goes through the node tree until it reaches the ends and places the Y coordinate for each node
        /// Each node is moved with relation to nodes above and to the right of it.
        /// </summary>
        public void PositionNodes(TreeNodeGroup nodeGroup, int depth)
        {
            if (nodeGroup.ParentChildGroups.Count == 0) //is a leaf node
            {
                if (!_depthYMargin.ContainsKey(depth))
                {
                    //if you're positioning nodes below nodes that have already been positioned but deeper into the tree
                    //they must be lower than those nodes above, this gets that value and passes it on
                    if (depth > _maxTreeDepth && _depthYMargin.ContainsKey(_maxTreeDepth))
                    {
                        _depthYMargin[depth] = _depthYMargin[_maxTreeDepth];
                        _maxTreeDepth = depth;
                    }
                    else
                    {
                        _depthYMargin[depth] = MarginFromTop;
                        if (depth > _maxTreeDepth)
                        {
                            _maxTreeDepth = depth;
                        }
                    }
                }
                nodeGroup.ParentNode.MoveNode(new Point(nodeGroup.ParentNode.Location.X, _depthYMargin[depth]), true);
                _depthYMargin[depth] += nodeGroup.ParentNode.NodeControl.ActualHeight + VERTICAL_BUFFER;
            }
            else //is a parent node
            {
                bool firstRow = false;
                for (int i = 0; i < nodeGroup.ParentChildGroups.Count; i++)
                {
                    if (!_depthYMargin.ContainsKey(depth))
                    {
                        firstRow = true;
                        _depthYMargin[depth] = nodeGroup.ParentNode.NodeControl.ActualHeight + VERTICAL_BUFFER;
                    }
                    else
                    {
                        firstRow = false;
                    }
                    PositionNodes(nodeGroup.ParentChildGroups[i], depth+1);
                }

                double position = nodeGroup.GetYPosition();

                //if it's not the top most nodes check that it doesn't collide with a row of nodes above it
                if ((position < _depthYMargin[depth] - MarginFromTop) && !firstRow)
                {
                    OffsetNodes(nodeGroup, depth); //collision was detected, move all the nodes from this node to the depth of the tree down
                }
                else
                {
                    nodeGroup.ParentNode.MoveNode(new Point(nodeGroup.ParentNode.Location.X, position), true);
                    _depthYMargin[depth] = position + nodeGroup.ParentNode.NodeControl.ActualHeight + VERTICAL_BUFFER;
                }
            }
        }

        /// <summary>
        /// Moves the branch down to the correct levels if a collision was detected when working back down the tree towards the root node.
        /// </summary>
        /// <param name="nodeGroup">The start position (branch of the tree)</param>
        /// <param name="depth">The current node depth</param>
        private void OffsetNodes(TreeNodeGroup nodeGroup, int depth)
        {
            nodeGroup.ParentNode.MoveNode(new Point(nodeGroup.ParentNode.Location.X, _depthYMargin[depth]), true);
            _depthYMargin[depth] = nodeGroup.ParentNode.Location.Y + nodeGroup.ParentNode.NodeControl.ActualHeight + VERTICAL_BUFFER;
            foreach (TreeNodeGroup branchGroup in nodeGroup.ParentChildGroups)
            {
                OffsetNodes(branchGroup, depth+1);
            }
        }

        /// <summary>
        /// If there are multiple root nodes on the map they must be placed so they don't overlap,
        /// this gives the map plus a margin to place the next map at.
        /// </summary>
        /// <returns>The maximum value of the Y position plus a margin</returns>
        public double GetMaximumYPosition()
        {
            double maxValue = 0;
            foreach (double depth in _depthYMargin.Values)
            {
                if (depth > maxValue)
                {
                    maxValue = depth;
                }
            }
            return maxValue;
        }
    }
}
