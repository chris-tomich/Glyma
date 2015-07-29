using System.Windows;
using SilverlightMappingToolBasic.UI.SuperGraph.Controller;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow
{
    public class ArrowCreationSet
    {
        public bool IsAdded
        {
            get;
            set;
        }

        public NodeControl NodeControl
        {
            private set;
            get;
        }

        public ArrowNodeSurrogate From
        {
            private set;
            get;
        }

        public ArrowNodeSurrogate To
        {
            private set;
            get;
        }

        public ArrowControl ArrowControl
        {
            private set;
            get;
        }

        public ArrowController Controller
        {
            private set;
            get;
        }

        public ArrowCreationSet(NodeControl nodeControl, Point location)
        {
            IsAdded = false;
            NodeControl = nodeControl;
            ArrowControl = new ArrowControl(nodeControl, null);
            From = new ArrowNodeSurrogate {Centre = location};
            To = new ArrowNodeSurrogate {Centre = location};
            Controller = new ArrowController(null, From, To);
            ArrowControl.DataContext = Controller.ViewModel;
        }
    }
}
