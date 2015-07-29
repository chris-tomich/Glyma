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
using System.Windows.Data;
using SilverlightMappingToolBasic.MappingService;

namespace SilverlightMappingToolBasic.Controls
{
    public partial class NodePropertiesDialog : ChildWindow
    {
        private string _originalName;
        private string _originalNote;

        public NodePropertiesDialog()
        {
            InitializeComponent();

            this.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(NodePropertiesDialog_Closing);
        }

        private void NodePropertiesDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.DialogResult == true && PropertyEditGrid.HasChanges)
            {
                MessageBoxResult result = MessageBox.Show("There are unsaved changes in the properties editor that will be lost.\r\n" +
                    "Click cancel and save them if you want to keep the changes.",
                    "Confirm Close", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        public INodeProxy NodeProxy
        {
            get;
            set;
        }

        public string Note
        {
            get;
            set;
        }

        private void NodePropertiesDialog_Loaded(object sender, RoutedEventArgs e)
        {
            NodeProxy = DataContext as INodeProxy;
            if (NodeProxy != null) 
            {
                _originalName = NodeProxy.Name;
                _originalNote = Note; //make a copy to restore closing
                if (Note != null)
                {
                    NodeNotesTextBlock.Text = Note;
                }
                NodeNotesTextBlock.Focus();
                NodeNotesTextBlock.SelectionStart = NodeNotesTextBlock.Text.Length;

                if (NodeProxy.Created != DateTime.MinValue)
                {
                    CreatedTimeTextBlock.Text = NodeProxy.Created.ToString("f");
                }
                if (NodeProxy.LastModified != DateTime.MinValue)
                {
                    ModifiedTimeTextBlock.Text = NodeProxy.LastModified.ToString("f");
                }

                
                if (NodeProxy.NodeType.Name == "CompendiumReferenceNode")
                {
                    MetadataContext context = new MetadataContext()
                    {
                        NodeUid = NodeProxy.Id,
                        MetadataName = "LinkedFile.Source"
                    };
                    if (NodeProxy.HasMetadata(context))
                    {
                        Height = 480;
                        ReferenceLocationRow.Height = new GridLength(30);
                        ReferenceNodeInfoPanel.Visibility = System.Windows.Visibility.Visible;

                        SoapMetadata linkedFileMetadata = NodeProxy.GetNodeMetadata(context);
                        ReferenceFileLocationTextBox.Text = linkedFileMetadata.MetadataValue;
                    }
                }
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Note = NodeNotesTextBlock.Text;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (NodeProxy != null)
            {
                NodeProxy.Name = _originalName;
                Note = _originalNote;
            }
            this.DialogResult = false;
        }

        private void NoteCmMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            switch (menuItem.Header.ToString())
            {
                case "Cut":
                    Clipboard.SetText(NodeNotesTextBlock.SelectedText);
                    NodeNotesTextBlock.SelectedText = "";
                    NodeNotesTextBlock.Focus();
                    break;
                case "Copy":
                    Clipboard.SetText(NodeNotesTextBlock.SelectedText);
                    NodeNotesTextBlock.Focus();
                    break;
                case "Paste":
                    NodeNotesTextBlock.SelectedText = Clipboard.GetText();
                    break;
                default:
                    break;
            }
            cm.IsOpen = false;
        }

        private void SpokenByCmMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            switch (menuItem.Header.ToString())
            {
                case "Cut":
                    Clipboard.SetText(CreatedByTextBox.SelectedText);
                    CreatedByTextBox.SelectedText = "";
                    CreatedByTextBox.Focus();
                    break;
                case "Copy":
                    Clipboard.SetText(CreatedByTextBox.SelectedText);
                    CreatedByTextBox.Focus();
                    break;
                case "Paste":
                    CreatedByTextBox.SelectedText = Clipboard.GetText();
                    break;
                default:
                    break;
            }
            spokenByCm.IsOpen = false;
        }

        private void NodeNameCmMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            switch (menuItem.Header.ToString())
            {
                case "Cut":
                    Clipboard.SetText(NodeNameTextBox.SelectedText);
                    NodeNameTextBox.SelectedText = "";
                    NodeNameTextBox.Focus();
                    break;
                case "Copy":
                    Clipboard.SetText(NodeNameTextBox.SelectedText);
                    NodeNameTextBox.Focus();
                    break;
                case "Paste":
                    NodeNameTextBox.SelectedText = Clipboard.GetText();
                    break;
                default:
                    break;
            }
            NodeNameCm.IsOpen = false;
        }
    }
}

