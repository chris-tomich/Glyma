using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Glyma.UtilityService.Proxy;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.UI
{
    public partial class ExportOptionDialog : ChildWindow
    {
        public event EventHandler<ExportOptionSelectedEventArgs> ExportOptionSelected;

        private ExportOption ExportOption
        {
            get { return DataContext as ExportOption; }
        }

        public ExportOptionDialog(ExportType exporyType, MapType mapType)
        {
            InitializeComponent();
            DataContext = new ExportOption(exporyType, mapType);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            if (ExportOptionSelected != null)
            {
                ExportOptionSelected(this, new ExportOptionSelectedEventArgs{ ExportOption =  ExportOption});
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

