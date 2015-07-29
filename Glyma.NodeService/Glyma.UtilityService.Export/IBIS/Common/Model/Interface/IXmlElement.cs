using System.Collections.Generic;
using System.Xml;

namespace Glyma.UtilityService.Export.IBIS.Common.Model.Interface
{
    public interface IXmlElement
    {
        XmlDocument Doc { get; }
        XmlElement Parent { get; }
        
        Dictionary<string, XmlAttribute> Attributes { get; }

        void CreateElement();
    }
}
