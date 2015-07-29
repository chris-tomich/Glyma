using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using SilverlightMappingToolBasic.MappingService;

namespace SilverlightMappingToolBasic
{
    public class NodeSerializer
    {
        public static string SerializeNode(INodeProxy[] nodeProxies) 
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Nodes));
            Encoding utf8 = new UTF8Encoding(false);
            MemoryStream ms = new MemoryStream();
            XmlWriter xmlWriter = XmlWriter.Create(ms, new XmlWriterSettings() { Indent = false, NewLineHandling = NewLineHandling.None, CloseOutput = true, Encoding = utf8 });
            Nodes nodes = new Nodes();
            List<NodesNode> nodesList = new List<NodesNode>();
            foreach (INodeProxy nodeProxy in nodeProxies)
            {
                NodesNode node = new NodesNode()
                {
                    NodeUid = nodeProxy.Id,
                    NodeType = new NodesNodeNodeType()
                    {
                        NodeTypeName = nodeProxy.NodeType.Name,
                        NodeTypeUid = nodeProxy.NodeType.Id
                    },
                    DomainUid = nodeProxy.Domain
                };
                List<NodesNodeMetadata> metadatam = new List<NodesNodeMetadata>();
                foreach (MetadataContext key in nodeProxy.Metadata.Keys)
                {
                    NodesNodeMetadata metaData = new NodesNodeMetadata()
                    {
                        Name = key.MetadataName.Replace(".", "_"),
                        Value = nodeProxy.Metadata[key].MetadataValue
                    };
                    metadatam.Add(metaData);
                }
                node.Metadatum = metadatam.ToArray();
                nodesList.Add(node);
            }
            nodes.Node = nodesList.ToArray();
            serializer.Serialize(xmlWriter, nodes);

            byte[] bytes = ms.ToArray();
            ms.Close();
            string xmlString = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            return xmlString;
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Nodes
    {
        private NodesNode[] nodesField;

        [System.Xml.Serialization.XmlElementAttribute("Node", IsNullable = false)]
        public NodesNode[] Node
        {
            get
            {
                return this.nodesField;
            }
            set
            {
                this.nodesField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class NodesNode
    {
        private Guid nodeUidField;

        private Guid domainUidField;

        private NodesNodeNodeType nodeTypeField;

        private NodesNodeMetadata[] metadatamField;

        /// <remarks/>
        public Guid NodeUid
        {
            get
            {
                return this.nodeUidField;
            }
            set
            {
                this.nodeUidField = value;
            }
        }

        /// <remarks/>
        public Guid DomainUid
        {
            get
            {
                return this.domainUidField;
            }
            set
            {
                this.domainUidField = value;
            }
        }

        /// <remarks/>
        public NodesNodeNodeType NodeType
        {
            get
            {
                return this.nodeTypeField;
            }
            set
            {
                this.nodeTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Metadata", IsNullable = false)]
        public NodesNodeMetadata[] Metadatum
        {
            get
            {
                return this.metadatamField;
            }
            set
            {
                this.metadatamField = value;
            }
        }
    }
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class NodesNodeNodeType
    {

        private Guid nodeTypeUidField;

        private string nodeTypeNameField;

        /// <remarks/>
        public Guid NodeTypeUid
        {
            get
            {
                return this.nodeTypeUidField;
            }
            set
            {
                this.nodeTypeUidField = value;
            }
        }

        /// <remarks/>
        public string NodeTypeName
        {
            get
            {
                return this.nodeTypeNameField;
            }
            set
            {
                this.nodeTypeNameField = value;
            }
        }
    }
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.450")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class NodesNodeMetadata
    {

        private string nameField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }
}