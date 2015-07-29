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
using SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller
{
    public class ArrowControlFactorySet
    {
        public ArrowControlFactorySet()
        {
        }

        public ViewModel.Relationship Relationship
        {
            get;
            set;
        }

        public ArrowController Controller
        {
            get;
            set;
        }

        public ArrowControl Control
        {
            get;
            set;
        }
    }
}
