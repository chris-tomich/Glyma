using System.Collections.Generic;
using System.Xml;

namespace Glyma.UtilityService.Export.IBIS.Common.Model.Interface
{
    public interface IXmlMap
    {
        List<IGlymaNode> Nodes { get; }
        List<IGlymaRelationship> Relationships { get; }

        XmlDocument Doc { get; }

        void Create(string outputFileLocation);
    }
}
