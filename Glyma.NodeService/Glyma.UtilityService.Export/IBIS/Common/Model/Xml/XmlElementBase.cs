using System.Collections.Generic;
using System.Xml;
using Glyma.UtilityService.Export.IBIS.Common.Model.Interface;

namespace Glyma.UtilityService.Export.IBIS.Common.Model.Xml
{
    public class XmlElementBase : IXmlElement
    {
        private Dictionary<string, XmlAttribute> _attributes;

        public XmlElementBase(XmlDocument doc, XmlElement parent)
        {
            Doc = doc;
            Parent = parent;
        }

        public XmlDocument Doc
        {
            get; 
            private set;
        }

        public XmlElement Parent
        {
            get; 
            private set;
        }

        public Dictionary<string, XmlAttribute> Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new Dictionary<string, XmlAttribute>();
                }
                return _attributes;
            }
            set
            {
                _attributes = value;
            }
        }

        public void AddAttributeByKeyValue(string key, string value)
        {
            var xmlAttribute = Doc.CreateAttribute(key);
            xmlAttribute.Value = value;
            if (!Attributes.ContainsKey(key))
            {
                Attributes.Add(key,xmlAttribute);
            }
            else
            {
                Attributes[key] = xmlAttribute;
            }
        }

        public virtual void CreateElement()
        {
            
        }
    }
}
