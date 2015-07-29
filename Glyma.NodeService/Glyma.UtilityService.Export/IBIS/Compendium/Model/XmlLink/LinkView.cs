using System;
using System.Xml;
using Glyma.UtilityService.Export.IBIS.Common.Model.Interface;
using Glyma.UtilityService.Export.IBIS.Common.Model.Xml;
using Glyma.UtilityService.Export.IBIS.Compendium.Extensions;

namespace Glyma.UtilityService.Export.IBIS.Compendium.Model.XmlLink
{
    public class LinkView: XmlElementBase
    {
        public LinkView(Guid mapUid, IGlymaRelationship relationship, XmlDocument doc, XmlElement parent, LinkType linkType)
            : this(doc, parent, linkType)
        {
            AddAttributeByKeyValue("id", mapUid.ToLongString());
            AddAttributeByKeyValue("created", relationship.Created);
            AddAttributeByKeyValue("lastModified", relationship.LastModified);
            
        }

        public LinkView(Guid mapUid, CompendiumNode originNode, XmlDocument doc, XmlElement parent, LinkType linkType)
            : this(doc, parent, linkType)
        {
            AddAttributeByKeyValue("id", mapUid.ToLongString());
            AddAttributeByKeyValue("created", originNode.Created);
            AddAttributeByKeyValue("lastModified", originNode.LastModified);
        }

        public LinkView(XmlDocument doc, XmlElement parent, LinkType linkType): base(doc, parent)
        {
            AddAttributeByKeyValue("arrowtype", "1");
            AddAttributeByKeyValue("linkstyle", "1");
            AddAttributeByKeyValue("linkdashed", "0");
            AddAttributeByKeyValue("linkweight", "1");
            switch (linkType)
            {
                case LinkType.About:
                    AddAttributeByKeyValue("linkcolour", "-16711681");
                    break;
                case LinkType.Challenges:
                    AddAttributeByKeyValue("linkcolour", "-20561");
                    break;
                case LinkType.ExpandsOn:
                    AddAttributeByKeyValue("linkcolour", "-14336");
                    break;
                case LinkType.ObjectsTo:
                    AddAttributeByKeyValue("linkcolour", "-65536");
                    break;
                case LinkType.RelatedTo:
                    AddAttributeByKeyValue("linkcolour", "-16777216");
                    break;
                case LinkType.Resolves:
                    AddAttributeByKeyValue("linkcolour", "-8355712");
                    break;
                case LinkType.RespondsTo:
                    AddAttributeByKeyValue("linkcolour", "-13434727");
                    break;
                case LinkType.Specializes:
                    AddAttributeByKeyValue("linkcolour", "-16776961");
                    break;
                case LinkType.Supports:
                    AddAttributeByKeyValue("linkcolour", "-16711936");
                    break;
                default:
                    //Defaults to the same as a RespondsTo
                    AddAttributeByKeyValue("linkcolour", "-13434727");
                    break;
            }
            //AddAttributeByKeyValue("linkcolour", "-16711936");
            AddAttributeByKeyValue("labelWrapWidth", "25");
            AddAttributeByKeyValue("fontsize", "12");
            AddAttributeByKeyValue("fontface", "Dialog");
            AddAttributeByKeyValue("fontstyle", "0");
            AddAttributeByKeyValue("foreground", "-16777216");
            AddAttributeByKeyValue("background", "-1");
        }

        public override void CreateElement()
        {
            var xmlLinkView = Doc.CreateElement(string.Empty, "linkview", string.Empty);
            foreach (var attributeKeyPair in Attributes)
            {
                xmlLinkView.Attributes.Append(attributeKeyPair.Value);
            }
            Parent.AppendChild(xmlLinkView);
            base.CreateElement();
        }
    }
}
