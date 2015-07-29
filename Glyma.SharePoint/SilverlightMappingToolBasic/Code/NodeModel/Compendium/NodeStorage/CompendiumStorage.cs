using System;
using System.Net;
using System.Xml;
using System.Collections.Generic;

namespace SilverlightMappingToolBasic.Compendium
{
    public class CompendiumStorage : INodesReader
    {
        private const string CompendiumDocumentTag = "model";
        private const string NodeDetailsTag = "node";
        private const string LinkRelationshipTag = "link";
        private const string ViewRelationshipTag = "view";

        private XmlReader _xml;
        private Dictionary<string, INode> _nodes;

        public CompendiumStorage()
        {
            _nodes = new Dictionary<string, INode>();
        }

        public void Load(string uri)
        {
        }

        public void Load(XmlReader xml)
        {
            _xml = xml;

            List<CompendiumViewRelationship> viewRelationships = new List<CompendiumViewRelationship>();

            if (_xml != null && _xml.NodeType == XmlNodeType.Element && _xml.Name == CompendiumDocumentTag)
            {
                while (_xml.Read())
                {
                    if (_xml.NodeType == XmlNodeType.Element)
                    {
                        switch (_xml.Name)
                        {
                            case NodeDetailsTag:
                                XmlCompendiumNode node = new XmlCompendiumNode(_xml);
                                _nodes.Add(node.Id, node);
                                break;

                            case LinkRelationshipTag:
                                CompendiumLinkRelationship nodeRelationship = new CompendiumLinkRelationship();
                                nodeRelationship.ConsumeLinkXml(_nodes, _xml);
                                break;

                            case ViewRelationshipTag:
                                CompendiumViewRelationship viewRelationship = new CompendiumViewRelationship();
                                viewRelationship.ConsumeViewXml(_xml);
                                viewRelationships.Add(viewRelationship);
                                break;

                            default:
                                break;
                        }
                    }
                    else if (_xml.NodeType == XmlNodeType.EndElement)
                    {
                        if (_xml.Name == CompendiumDocumentTag)
                        {
                            break;
                        }
                    }
                }
            }

            foreach (CompendiumViewRelationship viewRelationship in viewRelationships)
            {
                viewRelationship.BuildRelationships(_nodes);
            }
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
    }
}
