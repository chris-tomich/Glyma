using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.ComponentModel;

namespace NodeService
{
    public class GraphQuery
    {
        public GraphQuery()
        {
        }

        [XmlAttribute]
        public int Depth
        {
            get;
            set;
        }

        [XmlAttribute]
        public int Items
        {
            get;
            set;
        }

        public GraphQueryContext Context
        {
            get;
            set;
        }

        [XmlArray("Filters")]
        [XmlArrayItem("Filter", typeof(GraphQueryFilter))]
        public GraphQueryFilter[] Filters
        {
            get;
            set;
        }

        [XmlArray("Conditions")]
        [XmlArrayItem("Condition", typeof(GraphQueryCondition))]
        public GraphQueryCondition[] Conditions
        {
            get;
            set;
        }
    }
}