using System;
using System.Windows;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public class NodeMotionHandlerEventArgs : EventArgs
    {
        public Point Location
        {
            get;
            set;
        }
    }
}
