using System;
using System.Net;
using System.Xml;
using System.Collections.Generic;
using IoC;

namespace NodeService
{
    public class CompendiumStorage : INodesReader
    {
        private const string COMPENDIUM_DOCUMENT_TAG = "model";
        private const string NODE_DETAILS_TAG = "node";
        private const string LINK_RELATIONSHIP_TAG = "link";
        private const string VIEW_RELATIONSHIP_TAG = "view";

        private XmlReader _xml;
        private Dictionary<string, INode> _nodes;
        private List<CompendiumViewRelationship> _rootViewRelationships;

        public CompendiumStorage()
        {
            _nodes = new Dictionary<string, INode>();
            _rootViewRelationships = new List<CompendiumViewRelationship>();
        }

        public void Load(string uri)
        {
        }

        public bool Load(XmlReader xml, string documentLibraryUrl)
        {
            _xml = xml;

            List<CompendiumViewRelationship> viewRelationships = new List<CompendiumViewRelationship>();
            bool isDomainNodeRootMapNode = false;

            if (_xml != null && _xml.NodeType == XmlNodeType.Element && _xml.Name == COMPENDIUM_DOCUMENT_TAG)
            {
                _xml.MoveToAttribute("rootview");
                string domainNodeId = _xml.Value;
                
                while (_xml.Read())
                {
                    if (_xml.NodeType == XmlNodeType.Element)
                    {
                        switch (_xml.Name)
                        {
                            case VIEW_RELATIONSHIP_TAG:
                                CompendiumViewRelationship viewRelationship = new CompendiumViewRelationship();
                                viewRelationship.ConsumeViewXml(_xml, domainNodeId);
                                viewRelationships.Add(viewRelationship);
                                break;

                            case NODE_DETAILS_TAG:
                                XmlCompendiumNode node = new XmlCompendiumNode(_xml, domainNodeId, documentLibraryUrl);
                                _nodes.Add(node.Id, node);
                                break;

                            case LINK_RELATIONSHIP_TAG:
                                CompendiumLinkRelationship nodeRelationship = new CompendiumLinkRelationship();
                                if (nodeRelationship.ConsumeLinkXml(_nodes, _xml, domainNodeId))
                                {
                                    isDomainNodeRootMapNode = true;
                                }
                                break;

                            default:
                                break;
                        }
                    }
                    else if (_xml.NodeType == XmlNodeType.EndElement)
                    {
                        if (_xml.Name == COMPENDIUM_DOCUMENT_TAG)
                        {
                            break;
                        }
                    }
                }
                if (isDomainNodeRootMapNode)
                {
                    _nodes[domainNodeId].NodeType = IoCContainer.GetInjectionInstance().GetInstance<MapNode>();
                }
            }

            foreach (CompendiumViewRelationship viewRelationship in viewRelationships)
            {
                viewRelationship.BuildRelationships(_nodes);
            }

            Dictionary<CompendiumNodeAddress, CompendiumViewRelationship> allViewRelationships = new Dictionary<CompendiumNodeAddress, CompendiumViewRelationship>();
            Dictionary<string, CompendiumViewRelationship> originalRelationships = new Dictionary<string, CompendiumViewRelationship>();
            Dictionary<CompendiumNodeAddress, CompendiumViewRelationship> transcludedRelationships = new Dictionary<CompendiumNodeAddress, CompendiumViewRelationship>();

            foreach (CompendiumViewRelationship viewRelationship in viewRelationships)
            {
                if (!_rootViewRelationships.Contains(viewRelationship) && viewRelationship.IsRootView)
                {
                    _rootViewRelationships.Add(viewRelationship);
                    originalRelationships.Add(viewRelationship.FromNode.Id, viewRelationship);
                }
                else
                {
                    CompendiumNodeAddress nodeAddress = new CompendiumNodeAddress();
                    nodeAddress.viewId = viewRelationship.ToNode.Id;
                    nodeAddress.nodeId = viewRelationship.FromNode.Id;
                    allViewRelationships.Add(nodeAddress, viewRelationship);

                    if (originalRelationships.ContainsKey(viewRelationship.FromNode.Id))
                    {
                        // Perform the test to see if this node is the original or transcluded node.

                        CompendiumViewRelationship originalRelationship = originalRelationships[viewRelationship.FromNode.Id];

                        if (originalRelationship.Created < viewRelationship.Created)
                        {
                            CompendiumNodeAddress transcludedNodeAddress = new CompendiumNodeAddress();
                            transcludedNodeAddress.viewId = viewRelationship.ToNode.Id;
                            transcludedNodeAddress.nodeId = viewRelationship.FromNode.Id;

                            transcludedRelationships.Add(transcludedNodeAddress, viewRelationship);
                        }
                        else
                        {
                            originalRelationships.Remove(originalRelationship.FromNode.Id);
                            originalRelationships.Add(viewRelationship.FromNode.Id, viewRelationship);

                            CompendiumNodeAddress transcludedNodeAddress = new CompendiumNodeAddress();
                            transcludedNodeAddress.viewId = originalRelationship.ToNode.Id;
                            transcludedNodeAddress.nodeId = originalRelationship.FromNode.Id;

                            transcludedRelationships.Add(transcludedNodeAddress, originalRelationship);
                        }
                    }
                    else
                    {
                        // The node doesn't exist so add it to our list of original relationships continue.
                        originalRelationships.Add(viewRelationship.FromNode.Id, viewRelationship);
                    }
                }
            }

            foreach (CompendiumViewRelationship transcludedRelationship in transcludedRelationships.Values)
            {
                List<IDescriptor> linkRelationshipsToRemove = new List<IDescriptor>();

                // Inspect all the nodes linked to this node.
                foreach (IDescriptor localDescriptor in transcludedRelationship.FromNode.Descriptors)
                {
                    // Make sure we're looking at only link relationships and not views or transcluded relationships.
                    if (localDescriptor.Relationship is CompendiumLinkRelationship)
                    {
                        CompendiumTransclusionRelationship transclusionRelationship = null;

                        foreach (IDescriptor foreignDescriptor in localDescriptor.Relationship.Descriptors)
                        {
                            // Make sure we aren't looking at ourself.
                            if (foreignDescriptor != localDescriptor)
                            {
                                // Find the original non-transclusion relationship of the connected node. If this is not in the same view as the original view of the node we're
                                // currently building, then it means this is a transluded node and we need to build the relationship to represent that.
                                if (originalRelationships[foreignDescriptor.Node.Id].ToNode.Id != originalRelationships[localDescriptor.Node.Id].ToNode.Id)//transcludedRelationship.ToNode.Id)
                                {
                                    INode fromNode;
                                    INode toNode;
                                    INode transclusionNode = localDescriptor.Node;
                                    INode mapNode = originalRelationships[foreignDescriptor.Node.Id].ToNode;

                                    if (localDescriptor.DescriptorType.Name == "From")
                                    {
                                        toNode = foreignDescriptor.Node;
                                        fromNode = localDescriptor.Node;
                                    }
                                    else
                                    {
                                        toNode = localDescriptor.Node;
                                        fromNode = foreignDescriptor.Node;
                                    }

                                    transclusionRelationship = new CompendiumTransclusionRelationship(localDescriptor.Relationship, transcludedRelationship, toNode, fromNode, transclusionNode, mapNode);
                                    linkRelationshipsToRemove.Add(localDescriptor);
                                    linkRelationshipsToRemove.Add(foreignDescriptor);
                                }
                            }
                        }
                    }
                }

                foreach (IDescriptor descriptor in linkRelationshipsToRemove)
                {
                    descriptor.Node.RemoveDescriptor(descriptor);
                }

                foreach (IDescriptor descriptor in transcludedRelationship.Descriptors)
                {
                    descriptor.Node.RemoveDescriptor(descriptor);
                }
            }
            return isDomainNodeRootMapNode;
        }

        #region INodesReader Members

        public INode[] GetAllNodes()
        {
            Dictionary<string, INode>.ValueCollection values = _nodes.Values;

            INode[] allNodes = new INode[values.Count];

            values.CopyTo(allNodes, 0);

            return allNodes;
        }

        #endregion

        public List<CompendiumViewRelationship> RootViewRelationships
        {
            get
            {
                return _rootViewRelationships;
            }
        } 
    }

    public struct CompendiumNodeAddress
    {
        public string viewId;
        public string nodeId;
    }
}
