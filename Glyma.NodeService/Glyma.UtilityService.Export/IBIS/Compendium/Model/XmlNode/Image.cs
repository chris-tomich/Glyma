using System.Xml;
using Glyma.UtilityService.Export.IBIS.Common.Model.Xml;

namespace Glyma.UtilityService.Export.IBIS.Compendium.Model.XmlNode
{
    public class Image: XmlElementBase
    {
        public Image(XmlDocument doc, XmlElement parent) : base(doc, parent)
        {
            AddAttributeByKeyValue("width","0");
            AddAttributeByKeyValue("height", "0");
        }

        public override void CreateElement()
        {
            var xmlImage = Doc.CreateElement(string.Empty, "image", string.Empty);
            foreach (var attributeKeyPair in Attributes)
            {
                xmlImage.Attributes.Append(attributeKeyPair.Value);
            }
            Parent.AppendChild(xmlImage);
            base.CreateElement();
        }
    }
}
