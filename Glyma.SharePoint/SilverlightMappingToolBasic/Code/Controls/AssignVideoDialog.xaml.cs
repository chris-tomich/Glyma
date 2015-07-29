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

namespace SilverlightMappingToolBasic.Controls
{
    public partial class AssignVideoDialog : ChildWindow
    {
        public AssignVideoDialog()
        {
            InitializeComponent();
        }

        public string Source
        {
            get;
            set;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Source = SourceTextBlock.Text;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Source = null;
            this.DialogResult = false;
        }
    }
}

