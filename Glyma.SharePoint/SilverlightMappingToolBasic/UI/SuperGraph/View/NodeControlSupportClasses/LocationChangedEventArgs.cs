using System;
using System.Windows;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public class LocationChangedEventArgs : EventArgs
    {
        public Point Location
        {
            get;
            set;
        }
    }
}
