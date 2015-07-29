using System.Windows;
using System.Windows.Controls;
using SilverlightMappingToolBasic.UI.SuperGraph.View.RichTextSupportClasses;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.UI
{
    public partial class NewMapDialog : ChildWindow
    {
        public NewMapDialog()
        {
            InitializeComponent();
        }

        public string MapName
        {
            get;
            protected set;
        }

        private bool IsTextBoxEmpty
        {
            get
            {
                if (string.IsNullOrWhiteSpace(MapNameTextBox.UIText))
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
            MapName = MapNameTextBox.Text;
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

