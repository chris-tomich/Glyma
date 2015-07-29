using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightMappingToolBasic
{
    public class ThemeStyleAttribute : Attribute
    {
        public ThemeStyleAttribute(string styleKeyName)
        {
            StyleKeyName = styleKeyName;
        }

        public string StyleKeyName
        {
            get;
            set;
        }
    }
}
