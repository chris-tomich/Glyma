using System;

namespace SilverlightMappingToolBasic
{
    public interface INodeNote : IStorageElement
    {
        string[] Pages { get; set; }

        void AddPage(string pageData);
        void InsertPage(string pageData, int page);
        void RemovePage(int page);
        void RemovePage(string pageData);
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class NodeNotes
    {

        private NodeNote[] nodeNoteField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("NodeNote")]
        public NodeNote[] NodeNote
        {
            get
            {
                return this.nodeNoteField;
            }
            set
            {
                this.nodeNoteField = value;
            }
        }
    }
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class NodeNote
    {

        private Guid idField;

        private string nameField;

        private string createdByField;

        private string lastModifiedByField;

        private DateTime createdField;

        private DateTime lastModifiedField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public Guid Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

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
        public string CreatedBy
        {
            get
            {
                return this.createdByField;
            }
            set
            {
                this.createdByField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string LastModifiedBy
        {
            get
            {
                return this.lastModifiedByField;
            }
            set
            {
                this.lastModifiedByField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public DateTime Created
        {
            get
            {
                return this.createdField;
            }
            set
            {
                this.createdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public DateTime LastModified
        {
            get
            {
                return this.lastModifiedField;
            }
            set
            {
                this.lastModifiedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
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
