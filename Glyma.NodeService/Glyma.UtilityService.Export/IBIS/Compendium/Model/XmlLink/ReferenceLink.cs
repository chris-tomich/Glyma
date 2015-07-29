using System;
using System.Xml;
using Glyma.UtilityService.Export.IBIS.Compendium.Extensions;

namespace Glyma.UtilityService.Export.IBIS.Compendium.Model.XmlLink
{
    public class ReferenceLink : LinkBase
    {
        private readonly Guid _mapId;
        private readonly CompendiumNode _originNode;

        public ReferenceLink(Guid mapId, CompendiumNode referenceNode, CompendiumNode originNode, XmlDocument doc, XmlElement parent)
            : base(doc, null, parent)
        {
            //Console.WriteLine("New Reference Link Found: {0}", originNode.Id + "Link");
            _mapId = mapId;
            _originNode = originNode;
            AddAttributeByKeyValue("id", originNode.Id+"Link");
            AddAttributeByKeyValue("created", originNode.Created);
            AddAttributeByKeyValue("lastModified", originNode.LastModified);
            AddAttributeByKeyValue("author", originNode.Created);
            AddAttributeByKeyValue("from", referenceNode.Id.ToLongString());
            AddAttributeByKeyValue("to", originNode.Id.ToLongString());
        }

        public override void CreateElement()
        {
            var xmlLink = Doc.CreateElement(string.Empty, "link", string.Empty);
            foreach (var attributeKeyPair in Attributes)
            {
                xmlLink.Attributes.Append(attributeKeyPair.Value);
            }

            var xmlLinkViews = Doc.CreateElement(string.Empty, "linkviews", string.Empty);

            var linkView = new LinkView(_mapId, _originNode, Doc, xmlLinkViews, this.LinkType);
            linkView.CreateElement();

            xmlLink.AppendChild(xmlLinkViews);
            Parent.AppendChild(xmlLink);
            base.CreateElement();
        }
    }
}
