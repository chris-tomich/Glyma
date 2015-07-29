using System;
using System.Collections;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace VideoPlayerSharedLib
{
    /// <remarks/>
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Command
    {
        /// <remarks/>
        [XmlArray("Params")]
        [XmlArrayItem("Param")]
        public List<Param> Params
        {
            get;
            set;
        }

        /// <remarks/>
        [XmlElement("Name")]
        public string Name
        {
            get;
            set;
        }

        public bool ContainsParam(string paramName)
        {
            bool result = false;
            if (Params != null)
            {
                foreach (Param param in Params)
                {
                    if (param.Name == paramName)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        public string GetParamValue(string paramName)
        {
            string result = null;
            if (ContainsParam(paramName))
            {
                foreach (Param param in Params)
                {
                    if (param.Name == paramName)
                    {
                        result = param.Value;
                        break;
                    }
                }
            }
            return result;
        }
    }

    /// <remarks/>
    [XmlType(AnonymousType = true)]
    [XmlRoot("Param")]
    public partial class Param
    {
        /// <remarks/>
        [XmlAttribute("Name")]
        public string Name
        {
            get;
            set;
        }

        /// <remarks/>
        [XmlAttribute("Value")]
        public string Value
        {
            get;
            set;
        }
    }
}
