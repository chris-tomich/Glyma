using System;
using System.Net;
using System.Xml;
using System.Collections.Generic;
using SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.Nodes;
using SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel.Relationships;
using SimpleIoC;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.CompendiumMapProcessor.XmlModel
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
                    _nodes[domainNodeId].NodeType = IoCContainer.GetInjectionInstance().GetInstance<IMapManager>().NodeTypes["CompendiumMapNode"];
                }
            }

            foreach (CompendiumViewRelationship viewRelationship in viewRelationships)
            {
                viewRelationship.BuildRelationships(_nodes);

                if (!_rootViewRelationships.Contains(viewRelationship) && viewRelationship.IsRootView)
                {
                    _rootViewRelationships.Add(viewRelationship);
                }
            }

            List<string> transcludedNodes = new List<string>();
            List<string> nonTranscludedNodes = new List<string>();

            foreach (CompendiumViewRelationship viewRelationship in viewRelationships)
            {
                if (nonTranscludedNodes.Contains(viewRelationship.FromNode.Id))
                {
                    /// If this node is in the non-transcluded nodes list then it means it IS a transclusion and needs to be moved out of the transcluded nodes list.
                    transcludedNodes.Add(viewRelationship.FromNode.Id);
                    nonTranscludedNodes.Remove(viewRelationship.FromNode.Id);

                    /// We need to notify all the link relationships for this node that they are part of a transclusion so their "Map" property means something.
                    foreach (IDescriptor descriptor in viewRelationship.FromNode.Descriptors)
                    {
                        if (descriptor.Relationship is CompendiumLinkRelationship)
                        {
                            CompendiumLinkRelationship linkRelationship = descriptor.Relationship as CompendiumLinkRelationship;

                            linkRelationship.ConvertToTransclusionRelationship(_nodes);
                        }
                    }
                }
                else if (!transcludedNodes.Contains(viewRelationship.FromNode.Id))
                {
                    /// If this ID isn't in the non-transcluded nodes list and it's not in the transcluded nodes list, then it means we haven't added yet.
                    nonTranscludedNodes.Add(viewRelationship.FromNode.Id);
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
