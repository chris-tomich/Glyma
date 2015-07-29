using System;
using System.Xml;
using Glyma.UtilityService.Export.IBIS.Common.Model.Xml;
using Glyma.UtilityService.Export.IBIS.Compendium.Extensions;

namespace Glyma.UtilityService.Export.IBIS.Compendium.Model.XmlNode
{
    public class XmlNode : XmlElementBase
    {
        private readonly CompendiumNode _node;

        public XmlNode(CompendiumNode node, XmlDocument doc, XmlElement parent)
            : base(doc, parent)
        {
            //Console.WriteLine("New Node Found: {0}", node.Id);
            _node = node;
            AddAttributeByKeyValue("id", node.Id.ToLongString());
            AddAttributeByKeyValue("type", node.NodeType.ToId());
            AddAttributeByKeyValue("extendedtype", "");
            AddAttributeByKeyValue("originalid", "");
            AddAttributeByKeyValue("author", node.Author);
            AddAttributeByKeyValue("created", node.Created);
            AddAttributeByKeyValue("lastModified", node.LastModified);
            AddAttributeByKeyValue("label", node.Name);
            AddAttributeByKeyValue("state", "1");
            AddAttributeByKeyValue("lastModificationAuthor", node.LastModificationAuthor);
        }

        public override void CreateElement()
        {
            var xmlNode = Doc.CreateElement(string.Empty, "node", string.Empty);
            foreach (var attributeKeyPair in Attributes)
            {
                xmlNode.Attributes.Append(attributeKeyPair.Value);
            }
            var details = Doc.CreateElement(string.Empty, "details", string.Empty);


            var pageNum = 1;

            if (!string.IsNullOrEmpty(_node.Description))
            {
                var page = new Page(_node.Description, _node, Doc, details, pageNum);
                page.CreateElement();
                pageNum ++;
            }

            if (!string.IsNullOrEmpty(_node.ExtraMetadata))
            {
                var page2 = new Page(_node.ExtraMetadata, _node, Doc, details, pageNum);
                page2.CreateElement();
                pageNum ++;
            }

            if (pageNum == 1)
            {
                var page = new Page(_node.Description, _node, Doc, details, pageNum);
                page.CreateElement();
            }


            xmlNode.AppendChild(details);
            

            var source = Doc.CreateElement(string.Empty, "source", string.Empty);
            if (!string.IsNullOrEmpty(_node.Source))
            {
                var sourceCdata = Doc.CreateCDataSection(_node.Source);
                source.AppendChild(sourceCdata);
            }
            xmlNode.AppendChild(source);

            var image = new Image(Doc, xmlNode);
            image.CreateElement();

            var background = Doc.CreateElement(string.Empty, "background", string.Empty);
            xmlNode.AppendChild(background);

            var coderefs = Doc.CreateElement(string.Empty, "coderefs", string.Empty);
            xmlNode.AppendChild(coderefs);

            var shortcutrefs = Doc.CreateElement(string.Empty, "shortcutrefs", string.Empty);
            xmlNode.AppendChild(shortcutrefs);

            var movies = Doc.CreateElement(string.Empty, "movies", string.Empty);
            xmlNode.AppendChild(movies);

            Parent.AppendChild(xmlNode);
            base.CreateElement();
        }
    }
}
