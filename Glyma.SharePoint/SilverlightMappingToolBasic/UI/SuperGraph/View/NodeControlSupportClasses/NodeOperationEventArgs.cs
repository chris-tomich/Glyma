using System;
using SilverlightMappingToolBasic.UI.SuperGraph.Controller;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public class NodeOperationEventArgs : EventArgs
    {
        public NodeOperation Operation
        {
            get;
            set;
        }
    }
}
