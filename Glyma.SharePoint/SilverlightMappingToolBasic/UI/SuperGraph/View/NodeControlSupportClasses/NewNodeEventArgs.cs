using System;
using System.Windows;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public class NewNodeEventArgs : EventArgs
    {
        public string Name
        {
            get;
            set;
        }

        public Point Location
        {
            get;
            set;
        }
    }
}
