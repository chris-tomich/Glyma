using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace NodeService
{
    public class GraphQueryContext
    {
        public GraphQueryContext()
        {
        }

        [XmlAttribute]
        public Guid DomainId
        {
            get;
            set;
        }

        [XmlAttribute]
        public Guid NodeId
        {
            get;
            set;
        }
    }
}