using System;
using System.Windows;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu
{
    public class CreateNewNodeClickedEventArgs:EventArgs
    {
        public string NodeType { get; set; }

        public Point Location { get; set; }
    }
}
