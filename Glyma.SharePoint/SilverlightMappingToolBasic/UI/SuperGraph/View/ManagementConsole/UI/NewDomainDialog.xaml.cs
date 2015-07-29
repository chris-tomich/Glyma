using System.Windows;
using System.Windows.Controls;
using SilverlightMappingToolBasic.UI.SuperGraph.View.RichTextSupportClasses;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.UI
{
    public partial class NewDomainDialog : ChildWindow
    {
        public NewDomainDialog()
        {
            InitializeComponent();
        }

        public string DomainName
        {
            get;
            protected set;
        }

        private bool IsTextBoxEmpty
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DomainNameTextBox.UIText))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DomainName = DomainNameTextBox.Text;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OnTextChanged(object sender, System.EventArgs e)
        {
            OKButton.IsEnabled = !IsTextBoxEmpty;
        }
    }
}

