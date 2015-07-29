using System;
using System.Xml;
using Glyma.UtilityService.Export.IBIS.Common.Model.Interface;
using Glyma.UtilityService.Export.IBIS.Compendium.Extensions;

namespace Glyma.UtilityService.Export.IBIS.Compendium.Model.XmlLink
{
    public class Link : LinkBase
    {
        private readonly Guid _mapId;
        private readonly IGlymaRelationship _relationship;

        public Link(Guid mapId, IGlymaRelationship relationship, XmlDocument doc, XmlElement parent)
            : base(doc, relationship, parent)
        {
            _mapId = mapId;
            _relationship = relationship;
            AddAttributeByKeyValue("id", _relationship.Id.ToLongString());
            AddAttributeByKeyValue("created", _relationship.Created);
            AddAttributeByKeyValue("lastModified", _relationship.LastModified);
            AddAttributeByKeyValue("author", "Glyma");
            AddAttributeByKeyValue("from", _relationship.From.ToLongString());
            AddAttributeByKeyValue("to", _relationship.To.ToLongString());
            
        }


        public override void CreateElement()
        {
            var xmlLink = Doc.CreateElement(string.Empty, "link", string.Empty);
            foreach (var attributeKeyPair in Attributes)
            {
                xmlLink.Attributes.Append(attributeKeyPair.Value);
            }

            var xmlLinkViews = Doc.CreateElement(string.Empty, "linkviews", string.Empty);

            var linkView = new LinkView(_mapId, _relationship, Doc, xmlLinkViews, this.LinkType);
            linkView.CreateElement();

            xmlLink.AppendChild(xmlLinkViews);
            Parent.AppendChild(xmlLink);
            base.CreateElement();
        }
    }
}
