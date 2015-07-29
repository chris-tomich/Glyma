using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using TransactionalNodeService.Service;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Realign
{
    public class RealignMap : RealignObject
    {
        private readonly List<Relationship> _relationships;
        private readonly bool _isPartlyRealign;
        private readonly List<NodeControl> _nodeControls;
        private readonly List<RealignNode> _nodesNeedToBePlaced = new List<RealignNode>();

        public RealignMap(List<NodeControl> nodeControls, List<Relationship> relationships, RealignStyle realignStyle, bool isPartlyRealign)
        {
            _relationships = relationships;
            _isPartlyRealign = isPartlyRealign;
            _nodeControls = nodeControls;
            if (_nodeControls.Count > 0)
            {
                Initialise(realignStyle);
            }
        }

        // Find the left most node(s) -- parent nodes. Perpare all the child nodes for finding the relationships between them 
        private void Initialise(RealignStyle realignStyle = RealignStyle.Horizontal)
        {
            foreach (var nodeControl in _nodeControls)
            {
                var realignNode = new RealignNode(nodeControl);

                if (nodeControl.ParentNodes.Count == 0)
                {
                    AddChild(realignNode);
                }
                else
                {
                    _nodesNeedToBePlaced.Add(realignNode);
                }
            }

            //If no parent can be found, set the top left node to parent
            if (ChildNodes.Count == 0)
            {
                AddTopLeftNodeToParentNode();
            }
            PlaceNodesToMap(realignStyle);
            ReCheckForIncorrectDepth(realignStyle);

            if (_isPartlyRealign)
            {
                var nodesToLoop = ChildNodes.ToList();
                foreach (var realignNode in nodesToLoop)
                {
                    if (!realignNode.HasChild)
                    {
                        ChildNodes.Remove(realignNode);
                    }
                }
            }
        }

        // find the depth of all child nodes, and place them in their parents
        private void PlaceNodesToMap(RealignStyle realignStyle = RealignStyle.Horizontal)
        {
            var isUpdated = true;
            while (_nodesNeedToBePlaced.Count > 0 && isUpdated)
            {
                isUpdated = false;
                var allChildNodes = GetAllChildNodes(realignStyle);
                foreach (var node in allChildNodes)
                {
                    var realignNode = node;
                    //getting all relationships that to this node
                    var relationships = _relationships.Where(q => q.To == realignNode.Id).ToList();

                    //From the relationships found from last step, find the nodes.
                    foreach (var relationship in relationships)
                    {
                        var findedChild = _nodesNeedToBePlaced.FirstOrDefault(q => q.Id == relationship.From);
                        if (findedChild != null)
                        {
                            if (!node.ChildNodes.Contains(findedChild))
                            {
                                findedChild.RootDepth = node.RootDepth + 1;
                                node.AddChild(findedChild);
                                _nodesNeedToBePlaced.Remove(findedChild);
                                isUpdated = true;
                            }
                            _relationships.Remove(relationship);
                        }
                    }
                }


                //isUpdated = false;
                //var allChildNodes = GetAllChildNodes();
                //foreach (var node in allChildNodes)
                //{
                //    var realignNode = node;
                //    //getting all relationships that to this node
                //    var relationships = _relationships.Where(q => q.To == realignNode.Id).ToList();

                //    //From the relationships found from last step, find the nodes.
                //    foreach (var relationship in relationships)
                //    {
                //        var findedChild = _nodesNeedToBePlaced.FirstOrDefault(q => q.Id == relationship.From);
                //        if (findedChild != null)
                //        {
                //            if (!GetAllChildNodes().Contains(findedChild))
                //            {
                //                findedChild.RootDepth = node.RootDepth + 1;
                //                node.AddChild(findedChild);
                //                _nodesNeedToBePlaced.Remove(findedChild);
                //                isUpdated = true;
                //            }
                //            _relationships.Remove(relationship);
                //        }
                //    }
                //}
            }

            if (_nodesNeedToBePlaced.Count > 0)
            {
                //If there are nodes that have not been placed, select the left most and top most node as a parent.
                AddTopLeftNodeToParentNode();
                PlaceNodesToMap();
            }
        }

        // Set The Top Left Node To Parent Node
        private void AddTopLeftNodeToParentNode()
        {
            //if no parent is found, then select the left most and top most node for the parent.
            var orderedNodesOrderByLeft = _nodesNeedToBePlaced.OrderBy(q => q.OriginLeft);
            var orderedNodesOrderByLeftAndTop = orderedNodesOrderByLeft.ThenBy(q => q.OriginTop);

            //get the first node
            var node = orderedNodesOrderByLeftAndTop.FirstOrDefault();
            if (node != null)
            {
                AddChild(node);
                _nodesNeedToBePlaced.Remove(node);

            }
        }

        // Check if there is any relationship that from node and to node are in the same depth
        private void ReCheckForIncorrectDepth(RealignStyle realignStyle = RealignStyle.Horizontal)
        {
            var needToReCheck = true;
            var changedNodes = new HashSet<RealignNode>();
            while (needToReCheck)
            {
                needToReCheck = false;
                foreach (var relationship in _relationships)
                {
                    var allNodes = GetAllChildNodes(realignStyle).ToArray();
                    var from = allNodes.FirstOrDefault(q => q.Id == relationship.From);
                    var to = allNodes.FirstOrDefault(q => q.Id == relationship.To);

                    if (!changedNodes.Contains(from) && from != null && to != null && from.RootDepth <= to.RootDepth)
                    {
                        needToReCheck = true;
                        //add 1 to the node and its children rootDepth
                        foreach (var node in from.GetAllNodes(realignStyle))
                        {
                            if (node.RootDepth >= from.RootDepth)
                            {
                                node.RootDepth = node.RootDepth + 1;
                            }
                        }
                        changedNodes.Add(from);
                    }
                }
            }
        }
    }
}
