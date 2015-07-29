using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ThemeService
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    [DataContract]
    public partial class MappingToolConfig
    {
        private ThemesType themesField;
        private ContextMenusType contextMenusField;

        /// <remarks/>
        [DataMember]
        public ThemesType Themes
        {
            get
            {
                return this.themesField;
            }
            set
            {
                this.themesField = value;
            }
        }

        [DataMember]
        public ContextMenusType ContextMenus
        {
            get
            {
                return this.contextMenusField;
            }
            set
            {
                this.contextMenusField = value;
            }
        }

        public ContextMenu GetContextMenuByName(string name)
        {
            foreach (ContextMenu cm in ContextMenus.ContextMenus)
            {
                if (cm.Name == name)
                {
                    return cm;
                }
            }
            return null;
        }

        public Theme GetThemeByName(string name)
        {
            Theme result = null;
            foreach (Theme theme in Themes.Themes)
            {
                if (theme.Name == name)
                {
                    result = theme;
                    break;
                }
            }
            return result;
        }

    }
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [DataContract]
    public partial class ThemesType
    {
        private Theme[] themeField;

        /// <remarks/>
        [DataMember]
        [System.Xml.Serialization.XmlElementAttribute("Theme")]
        public Theme[] Themes
        {
            get
            {
                return this.themeField;
            }
            set
            {
                this.themeField = value;
            }
        }
    }
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [DataContract]
    public partial class Theme
    {
        private ThemeSkin[] skinField;
        private string nameField;
        private string assemblyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Skin")]
        [DataMember]
        public ThemeSkin[] Skin
        {
            get
            {
                return this.skinField;
            }
            set
            {
                this.skinField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [DataMember]
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
        [DataMember]
        public string Assembly
        {
            get
            {
                return this.assemblyField;
            }
            set
            {
                this.assemblyField = value;
            }
        }
    }
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [DataContract]
    public partial class ThemeSkin
    {
        private ThemeSkinProperty[] propertyField;
        private string nameField;
        private string nodeTypeField;
        private string classField;
        private string assemblyField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Property")]
        [DataMember]
        public ThemeSkinProperty[] Property
        {
            get
            {
                return this.propertyField;
            }
            set
            {
                this.propertyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [DataMember]
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
        [DataMember]
        public string NodeType
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [DataMember]
        public string Class
        {
            get
            {
                return this.classField;
            }
            set
            {
                this.classField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [DataMember]
        public string Assembly
        {
            get
            {
                return this.assemblyField;
            }
            set
            {
                this.assemblyField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [DataContract]
    public partial class ThemeSkinProperty
    {
        private string nameField;
        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        [DataMember]
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
        [System.Xml.Serialization.XmlTextAttribute()]
        [DataMember]
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

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [DataContract]
    public partial class ContextMenusType
    {
        private ContextMenu[] contextMenuField;

        [System.Xml.Serialization.XmlElementAttribute("ContextMenu")]
        [DataMember]
        public ContextMenu[] ContextMenus
        {
            get
            {
                return this.contextMenuField;
            }
            set
            {
                this.contextMenuField = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.233")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [DataContract]
    public partial class ContextMenu
    {
        private string nameField;
        private string contextMenuXamlField;

        [System.Xml.Serialization.XmlAttributeAttribute()]
        [DataMember]
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

        [System.Xml.Serialization.XmlElementAttribute()]
        [DataMember]
        public string ContextMenuXaml
        {
            get
            {
                return this.contextMenuXamlField;
            }
            set
            {
                this.contextMenuXamlField = value;
            }
        }
    }
}