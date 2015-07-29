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
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller
{
    public class NodeControlFactorySet
    {
        public NodeControlFactorySet()
        {
        }

        public ViewModel.Node Node
        {
            get;
            set;
        }

        public NodeControl Control
        {
            get;
            set;
        }
    }
}
