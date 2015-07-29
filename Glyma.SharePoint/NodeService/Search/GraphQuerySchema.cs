using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace NodeService
{
    public class GraphQuerySchema
    {
        public GraphQuerySchema()
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
        public Guid Id
        {
            get;
            set;
        }
    }
}