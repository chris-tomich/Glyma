using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
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
using System.Diagnostics;
using System.CodeDom.Compiler;

namespace VideoPlayerSharedLib
{
    /// <remarks/>
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class Event
    {
        /// <remarks/>
        [XmlArray("Params")]
        [XmlArrayItem("Param")]
        public List<EventArg> EventArgs
        {
            get;
            set;
        }

        [XmlElement("Name")]
        public string Name
        {
            get;
            set;
        }

        public bool ContainsEventArg(string eventArgName)
        {
            bool result = false;
            if (EventArgs != null)
            {
                foreach (EventArg eventArg in EventArgs)
                {
                    if (eventArg.Name == eventArgName)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        public string GetEventArgValue(string eventArgName)
        {
            string result = null;
            if (ContainsEventArg(eventArgName))
            {
                foreach (EventArg eventArg in EventArgs)
                {
                    if (eventArg.Name == eventArgName)
                    {
                        result = eventArg.Value;
                        break;
                    }
                }
            }
            return result;
        }
    }

    /// <remarks/>
    [XmlType(AnonymousType = true)]
    [XmlRoot("EventArg")]
    public partial class EventArg
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
