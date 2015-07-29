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
using SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole
{
    public class ExportOptionSelectedEventArgs : EventArgs
    {
        public ExportOption ExportOption { get; set; }
    }
}
