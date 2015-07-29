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

namespace SilverlightMappingToolBasic.Compendium
{
    public class CompendiumNodeSettings
    {
        public CompendiumNodeSettings()
        {
        }

        public INode NodeDetails
        {
            get;
            set;
        }

        public int NodeIndex
        {
            get;
            set;
        }

        public bool IsFocused
        {
            get;
            set;
        }

        public int Radius
        {
            get;
            set;
        }

        public int NodeCount
        {
            get;
            set;
        }

        public double CentreX
        {
            get;
            set;
        }

        public double CentreY
        {
            get;
            set;
        }
    }
}
