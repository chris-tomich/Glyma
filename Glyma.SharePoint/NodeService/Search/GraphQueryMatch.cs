using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.ComponentModel;

namespace NodeService
{
    public class GraphQueryMatch
    {
        public GraphQueryMatch()
        {
        }

        [XmlAttribute]
        [DefaultValue(true)]
        public bool Included
        {
            get;
            set;
        }

        [XmlArray("TypeSet")]
        [XmlArrayItem("Type", typeof(GraphQueryType))]
        public GraphQueryType[] TypeSet
        {
            get;
            set;
        }

        [XmlArray("SchemaSet")]
        [XmlArrayItem("Schema", typeof(GraphQuerySchema))]
        public GraphQuerySchema[] SchemaSet
        {
            get;
            set;
        }

        [XmlArray("MetadataSet")]
        [XmlArrayItem("Metadata", typeof(GraphQueryMetadata))]
        public GraphQueryMetadata[] MetadataSet
        {
            get;
            set;
        }
    }
}