using System;
using System.Linq;
using System.Xml;
using Glyma.UtilityService.Export.IBIS.Common.Model.Interface;
using Glyma.UtilityService.Export.IBIS.Common.Model.Xml;
using Glyma.UtilityService.Export.IBIS.Compendium.Extensions;
using Glyma.UtilityService.Export.IBIS.Compendium.Model.XmlLink;
using Glyma.UtilityService.Export.IBIS.Compendium.Model.XmlView;

namespace Glyma.UtilityService.Export.IBIS.Compendium.Model
{
    public class CompendiumXmlMap : XmlMapBase
    {
        private readonly XmlElement _xmlElementViews;
        private readonly XmlElement _xmlElementNodes;
        private readonly XmlElement _xmlElementLinks;

        public CompendiumXmlMap(Guid mapId)
        {
            var doctype = Doc.CreateDocumentType("model", null, "Compendium.dtd", null);
            Doc.AppendChild(doctype);

            var model = Doc.CreateElement(string.Empty, "model", string.Empty);
            var attr = Doc.CreateAttribute("rootview");
            attr.Value = mapId.ToLongString();
            model.Attributes.Append(attr);
            Doc.AppendChild(model);


            _xmlElementViews = Doc.CreateElement(string.Empty, "views", string.Empty);
            model.AppendChild(_xmlElementViews);

            _xmlElementNodes = Doc.CreateElement(string.Empty, "nodes", string.Empty);
            model.AppendChild(_xmlElementNodes);

            _xmlElementLinks = Doc.CreateElement(string.Empty, "links", string.Empty);
            model.AppendChild(_xmlElementLinks);

        }

        private bool IsRelationshipExist(IGlymaRelationship relationship)
        {
            return Relationships.Any(q => q.ProxyRelationship.Id == relationship.ProxyRelationship.Id);
        }


        public void AddNode(CompendiumNode node)
        {
            if (!IsNodeExist(node))
            {
                var xmlNode = new XmlNode.XmlNode(node, Doc, _xmlElementNodes);
                xmlNode.CreateElement();
                Nodes.Add(node);
            }
        }

        private bool IsNodeExist(IGlymaNode node)
        {
            var foundNode = Nodes.FirstOrDefault(q => q.Id == node.Id);
            if (foundNode == null)
            {
                return false;
            }
            return true;
        }

        public void AddView(Guid mapId, CompendiumNode node)
        {
            var xmlView = new View(mapId, node, Doc, _xmlElementViews);
            xmlView.CreateElement();
        }

        public void AddLink(Guid mapId, IGlymaRelationship relationship)
        {
            if (!IsRelationshipExist(relationship))
            {
                var xmlLink = new Link(mapId, relationship, Doc, _xmlElementLinks);
                xmlLink.CreateElement();
                Relationships.Add(relationship);
            }
        }

        public void AddReferenceLink(Guid mapId, CompendiumNode refenceNode, CompendiumNode originNode)
        {
            var xmlLink = new ReferenceLink(mapId, refenceNode, originNode, Doc, _xmlElementLinks);
            xmlLink.CreateElement();
        }
    }
}
