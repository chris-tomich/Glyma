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

namespace SilverlightMappingToolBasic.SingleDepth
{
    public class DetailsStyle
    {
        public DetailsStyle()
        {
        }

        [ThemeProperty("SomeDetails1")]
        public string SomeDetails1
        {
            get;
            set;
        }
    }

    public class ExtraDetailsStyle
    {
        public ExtraDetailsStyle()
        {
        }

        [ThemeProperty("SomeExtraDetails1")]
        public string ExtraDetails1
        {
            get;
            set;
        }

        [ThemeProperty("SomeExtraDetails2")]
        public string ExtraDetails2
        {
            get;
            set;
        }
    }

    public class SingleDepthTheme : ThemeManager<SingleDepthTheme>
    {
        public SingleDepthTheme()
            : base("Default", "SilverlightMappingToolBasic.SingleDepth")
        {
        }

        public SingleDepthTheme(string themeName)
            : base(themeName, "SilverlightMappingToolBasic.SingleDepth")
        {
        }

        [ThemeStyle("Details")]
        public DetailsStyle SomeDetails
        {
            get;
            set;
        }

        [ThemeStyle("ExtraDetails")]
        public ExtraDetailsStyle SomeExtraDetails
        {
            get;
            set;
        }
    }
}
