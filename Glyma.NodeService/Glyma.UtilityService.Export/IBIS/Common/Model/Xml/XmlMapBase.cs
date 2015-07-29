using System.Collections.Generic;
using System.Xml;
using Glyma.UtilityService.Export.IBIS.Common.Model.Interface;

namespace Glyma.UtilityService.Export.IBIS.Common.Model.Xml
{
    public class XmlMapBase : IXmlMap
    {
        private List<IGlymaNode> _nodes;
        private List<IGlymaRelationship> _relationships;
        private XmlDocument _doc;

        public List<IGlymaNode> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new List<IGlymaNode>();
                }
                return _nodes;
            }
        }

        public List<IGlymaRelationship> Relationships
        {
            get
            {
                if (_relationships == null)
                {
                    _relationships = new List<IGlymaRelationship>();
                }
                return _relationships;
            }
        }

        public XmlDocument Doc
        {
            get
            {
                if (_doc == null)
                {
                    _doc = new XmlDocument();
                }
                return _doc;
            }
        }

        public XmlMapBase()
        {
            var xmlDeclaration = Doc.CreateXmlDeclaration("1.0", "UTF-8", null);

            var root = Doc.DocumentElement;
            Doc.InsertBefore(xmlDeclaration, root);
            Doc.XmlResolver = null;

            
        }


        public void Create(string outputFileLocation)
        {
            Doc.Save(outputFileLocation);
        }
    }
}
