using System.Globalization;
using System.Xml;
using Glyma.UtilityService.Export.IBIS.Common.Model.Xml;
using Glyma.UtilityService.Export.IBIS.Compendium.Extensions;

namespace Glyma.UtilityService.Export.IBIS.Compendium.Model.XmlNode
{
    public class Page : XmlElementBase
    {
        private readonly string _content;
        private readonly XmlDocument _doc;

        public Page(string content, CompendiumNode node, XmlDocument doc, XmlElement parent, int pageNo = 1)
            : base(doc, parent)
        {
            _content = content;
            _doc = doc;
            AddAttributeByKeyValue("nodeid", node.Id.ToLongString());
            AddAttributeByKeyValue("author", node.Author);
            AddAttributeByKeyValue("created", node.Created);
            AddAttributeByKeyValue("lastModified", node.LastModified);
            AddAttributeByKeyValue("pageno", pageNo.ToString(CultureInfo.InvariantCulture));
        }

        public override void CreateElement()
        {
            var xmlPage = Doc.CreateElement(string.Empty, "page", string.Empty);
            foreach (var attributeKeyPair in Attributes)
            {
                xmlPage.Attributes.Append(attributeKeyPair.Value);
            }
            if (!string.IsNullOrEmpty(_content))
            {
                var cdata = _doc.CreateCDataSection(_content);
                xmlPage.AppendChild(cdata);
            }
            Parent.AppendChild(xmlPage);
            base.CreateElement();
        }
    }
}
