using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace NodeService
{
    public class GraphQueryMetadata
    {
        public GraphQueryMetadata()
        {
        }

        [XmlAttribute]
        public string BaseType
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Operator
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Name
        {
            get;
            set;
        }

        [XmlAttribute]
        public string Value
        {
            get;
            set;
        }
    }
}