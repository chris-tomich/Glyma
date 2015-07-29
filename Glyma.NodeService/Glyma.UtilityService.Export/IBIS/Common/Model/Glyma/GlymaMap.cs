using System.Collections.Generic;
using System.Linq;
using Glyma.UtilityService.Export.IBIS.Common.Model.Interface;
using TransactionalNodeService.Proxy.Universal;

namespace Glyma.UtilityService.Export.IBIS.Common.Model.Glyma
{
    public class GlymaMap: GlymaNode
    {
        private List<IGlymaRelationship> _relationships;

        private List<IGlymaNode> _nodesNeedToBePlaced;

        public bool IsInitialised { get; private set; }

        public List<IGlymaNode> NodesNeedToBePlaced
        {
            get
            {
                if (_nodesNeedToBePlaced == null)
                {
                    _nodesNeedToBePlaced = new List<IGlymaNode>();
                }
                return _nodesNeedToBePlaced;
            }
        }

        public List<IGlymaRelationship> Relationships
        {
            get
            {
                if (_relationships == null)
                {
                    _relationships = new List<IGlymaRelationship>();
                }
                return _relationships;
            }
        }


        public GlymaMap(INode node) : base(node)
        {
            
        }

        public void AddNode(IGlymaNode node)
        {
            if (!Nodes.Contains(node))
            {
                Nodes.Add(node);
            }
        }

        public void AddRelationship(IGlymaRelationship relationship)
        {
            if (!Relationships.Contains(relationship))
            {
                Relationships.Add(relationship);
            }
        }

        public void Initialise()
        {
            if (!IsInitialised)
            {
                foreach (var node in Nodes)
                {
                    if (Relationships.Any(q => q.From == node.Id))
                    {
                        NodesNeedToBePlaced.Add(node);
                    }
                    else
                    {
                        ChildNodes.Add(node);
                    }
                }

                //If no parent can be found, set the top left node to parent
                if (ChildNodes.Count == 0)
                {
                    AddTopLeftNodeToParentNode();
                }
                PlaceNodesToMap();
                ReCheckForIncorrectDepth();
                IsInitialised = true;
            }
        }

        private void ReCheckForIncorrectDepth()
        {
            var needToReCheck = true;
            var changedNodes = new HashSet<IGlymaNode>();
            while (needToReCheck)
            {
                needToReCheck = false;
                foreach (var relationship in Relationships)
                {
                    var allNodes = GetAllChildNodes().ToArray();
                    var from = allNodes.FirstOrDefault(q => q.Id == relationship.From);
                    var to = allNodes.FirstOrDefault(q => q.Id == relationship.To);

                    if (from != null && to != null && !changedNodes.Contains(from) && from.Depth <= to.Depth)
                    {
                        needToReCheck = true;
                        //add 1 to the node and its children rootDepth
                        foreach (var node in from.GetAllChildNodes())
                        {
                            if (node.Depth >= from.Depth)
                            {
                                node.Depth = node.Depth + 1;
                            }
                        }


                        
                        changedNodes.Add(from);
                    }
                }
            }
        }

        private void PlaceNodesToMap()
        {
            var isUpdated = true;
            while (NodesNeedToBePlaced.Count > 0 && isUpdated)
            {
                isUpdated = false;
                var allChildNodes = GetAllChildNodes().ToList();
                foreach (var node in allChildNodes)
                {
                    var currentNode = node;
                    //getting all relationships that to this node
                    var relationships = Relationships.Where(q => q.To == currentNode.Id).ToList();

                    //From the relationships found from last step, find the nodes.
                    foreach (var relationship in relationships)
                    {
                        var findedChild = NodesNeedToBePlaced.FirstOrDefault(q => q.Id == relationship.From);
                        if (findedChild != null)
                        {
                            if (!node.ChildNodes.Contains(findedChild))
                            {
                                findedChild.Depth = node.Depth + 1;
                                node.ChildNodes.Add(findedChild);
                                NodesNeedToBePlaced.Remove(findedChild);
                                isUpdated = true;
                            }
                            Relationships.Remove(relationship);
                        }
                        else
                        {
                            //Add another child
                        }
                    }
                }
            }

            if (_nodesNeedToBePlaced.Count > 0)
            {
                //If there are nodes that have not been placed, select the left most and top most node as a parent.
                AddTopLeftNodeToParentNode();
                PlaceNodesToMap();
            }
        }

        

        private void AddTopLeftNodeToParentNode()
        {
            //if no parent is found, then select the left most and top most node for the parent.
            var orderedNodesOrderByLeft = NodesNeedToBePlaced.OrderBy(q => q.XPosition);
            var orderedNodesOrderByLeftAndTop = orderedNodesOrderByLeft.ThenBy(q => q.YPosition);

            //get the first node
            var node = orderedNodesOrderByLeftAndTop.FirstOrDefault();
            if (node != null)
            {
                ChildNodes.Add(node);
                NodesNeedToBePlaced.Remove(node);

            }
        }
    }
}
