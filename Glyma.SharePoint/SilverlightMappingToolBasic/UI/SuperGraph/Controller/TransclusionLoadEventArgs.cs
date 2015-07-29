using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller
{
    public class TransclusionLoadEventArgs : EventArgs
    {
        public ViewModel.Node Node
        {
            get;
            set;
        }

        public Dictionary<Guid, string> MapContainerNodes
        {
            get;
            set;
        }
    }
}
