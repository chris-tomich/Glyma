using System;
using System.Globalization;
using System.Xml;
using Glyma.UtilityService.Export.IBIS.Common.Model.Xml;
using Glyma.UtilityService.Export.IBIS.Compendium.Extensions;

namespace Glyma.UtilityService.Export.IBIS.Compendium.Model.XmlView
{
    public class View : XmlElementBase
    {
        private readonly string _viewRef;

        private readonly string _noderef;

        private string _xPosition;

        private string _yPosition;

        private string _created;

        private string _lastModified;

        private bool _showTags = true;

        private bool _showText = true;

        private bool _showTrans = true;

        private bool _showWeight = true;

        private bool _smallIcon = false;

        private bool _hideIcon = false;

        private int _labelWrapWidth = 25;

        private int _fontsize = 12;

        private string _fontface = "Dialog";

        private int _fontstyle = 0;

        private string _foreground = "-16777216";

        private string _background = "-1";

        public View(Guid mapId, CompendiumNode node, XmlDocument doc, XmlElement parent)
            : base(doc, parent)
        {
            AddAttributeByKeyValue("viewref", mapId.ToLongString());
            AddAttributeByKeyValue("noderef", node.Id.ToLongString());
            AddAttributeByKeyValue("XPosition", ((int)node.XPosition).ToString(CultureInfo.InvariantCulture));
            AddAttributeByKeyValue("YPosition", ((int)node.YPosition).ToString(CultureInfo.InvariantCulture));
            AddAttributeByKeyValue("created", node.Created);
            AddAttributeByKeyValue("lastModified", node.LastModified);
            AddAttributeByKeyValue("showTags", "true");
            AddAttributeByKeyValue("showText", "true");
            AddAttributeByKeyValue("showTrans", "true");
            AddAttributeByKeyValue("showWeight", "true");
            AddAttributeByKeyValue("smallIcon", "false");
            AddAttributeByKeyValue("hideIcon", "false");
            AddAttributeByKeyValue("labelWrapWidth", "25");
            AddAttributeByKeyValue("fontsize", "12");
            AddAttributeByKeyValue("fontface", "Dialog");
            AddAttributeByKeyValue("fontstyle", "0");
            AddAttributeByKeyValue("foreground", "-16777216");
            AddAttributeByKeyValue("background", "-1");
        }

        public override void CreateElement()
        {
            var xmlView = Doc.CreateElement(string.Empty, "view", string.Empty);
            foreach (var attributeKeyPair in Attributes)
            {
                xmlView.Attributes.Append(attributeKeyPair.Value);
            }
            Parent.AppendChild(xmlView);
            base.CreateElement();
        }
    }
}
