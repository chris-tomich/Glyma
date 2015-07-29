using System;
using Glyma.UtilityService.Export.IBIS.Common.Model.Xml;
using Glyma.UtilityService.Export.IBIS.Compendium.Extensions;

namespace Glyma.UtilityService.Export.IBIS.GlymaXml.Model
{
    public class GlymaXmlMap : XmlMapBase
    {
        public GlymaXmlMap(Guid mapId)
        {
            var doctype = Doc.CreateDocumentType("model", null, "Compendium.dtd", null);
            Doc.AppendChild(doctype);

            var model = Doc.CreateElement(string.Empty, "model", string.Empty);
            var attr = Doc.CreateAttribute("rootview");
            attr.Value = mapId.ToLongString();
            model.Attributes.Append(attr);
            Doc.AppendChild(model);


            //_xmlElementViews = Doc.CreateElement(string.Empty, "views", string.Empty);
            //model.AppendChild(_xmlElementViews);

            //_xmlElementNodes = Doc.CreateElement(string.Empty, "nodes", string.Empty);
            //model.AppendChild(_xmlElementNodes);

            //_xmlElementLinks = Doc.CreateElement(string.Empty, "links", string.Empty);
            //model.AppendChild(_xmlElementLinks);

        
        }
    }
}
