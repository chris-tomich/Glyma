using System;
using System.Windows;
using System.Windows.Controls;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;
using SilverlightMappingToolBasic.UI.ViewModel;
using TransactionalNodeService.Proxy;
using SelectionChangedEventArgs = Telerik.Windows.Controls.SelectionChangedEventArgs;
using System.Windows.Media;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses
{
    public partial class NodePropertiesDialog : ChildWindow
    {
        private int? _previousSelection;

        public NodeDescriptionType DescriptionType
        {
            get;
            set;
        }

        public string Description
        {
            get
            {
                return NodeProperties.Description;
            }
            set
            {
                NodeProperties.Description = value;
            }
        }

        public INodeProperties NodeProperties
        {
            get
            {
                return DataContext as INodeProperties;
            }
        }

        public NodePropertiesDialog()
        {
            InitializeComponent();
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PropertyEditorGrid.DataContext = null;
            PropertyEditorGrid.DataContext = DataContext;

            RichTextEditor.DataContext = null;
            RichTextEditor.DataContext = DataContext;

            if (NodeProperties != null)
            {
                DescriptionType = NodeProperties.DescriptionType;
                HtmlEditor.Text = NodeProperties.Description;
                switch (DescriptionType)
                {
                    case NodeDescriptionType.Html:
                        if (string.IsNullOrEmpty(Description))
                        {
                            DescriptionTypeSelector.SelectedIndex = -1;
                            if (NodeProperties is MultipleNodesProperties)
                            {
                                MultipleNodeTextWarning.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                MultipleNodeTextWarning.Visibility = Visibility.Collapsed;
                            }
                        }
                        else
                        {
                            CbRichTextAdvanceMode.IsChecked = false;
                            DescriptionTypeSelector.SelectedIndex = 0;
                            IframePanel.Visibility = Visibility.Collapsed;
                            RawPanel.Visibility = Visibility.Collapsed;
                            DesignerPanel.Visibility = Visibility.Visible;
                            DescriptionType = NodeDescriptionType.Html;
                            MultipleNodeTextWarning.Visibility = Visibility.Collapsed;
                        }
                        break;
                    case NodeDescriptionType.Iframe:
                        DescriptionTypeSelector.SelectedIndex = 1;
                        IframePanel.Visibility = Visibility.Visible;
                        MultipleNodeTextWarning.Visibility = Visibility.Collapsed;
                        break;
                    case NodeDescriptionType.RawInput:
                        if (string.IsNullOrEmpty(Description))
                        {
                            DescriptionTypeSelector.SelectedIndex = -1;
                            if (NodeProperties is MultipleNodesProperties)
                            {
                                MultipleNodeTextWarning.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                MultipleNodeTextWarning.Visibility = Visibility.Collapsed;
                            }
                        }
                        else
                        {
                            CbRichTextAdvanceMode.IsChecked = true;
                            DescriptionTypeSelector.SelectedIndex = 0;
                            IframePanel.Visibility = Visibility.Collapsed;
                            RawPanel.Visibility = Visibility.Visible;
                            DesignerPanel.Visibility = Visibility.Collapsed;
                            DescriptionType = NodeDescriptionType.RawInput;
                            MultipleNodeTextWarning.Visibility = Visibility.Collapsed;
                        }
                        break;
                    default:
                        DescriptionTypeSelector.SelectedIndex = -1;
                        IframePanel.Visibility = Visibility.Collapsed;
                        RawPanel.Visibility = Visibility.Collapsed;
                        DesignerPanel.Visibility = Visibility.Collapsed;
                        MultipleNodeTextWarning.Visibility = Visibility.Collapsed;
                        break;
                }
                NodeProperties.BeginEdit();
                Initialise();
            }
        }

        private void Initialise()
        {
            CbRichTextAdvanceMode.Unchecked += CbRichTextAdvanceMode_Unchecked;
            CbRichTextAdvanceMode.Checked += CbRichTextAdvanceMode_Checked;
        }

        private void ChildWindow_Closed(object sender, EventArgs eventArgs)
        {
            DataContext = null;
            CbRichTextAdvanceMode.Unchecked -= CbRichTextAdvanceMode_Unchecked;
            CbRichTextAdvanceMode.Checked -= CbRichTextAdvanceMode_Checked;
            var main = Application.Current.RootVisual as MainPage;
            if (main != null)
            {
                main.SuperGraph.ContextMenuContainer.AuthorContextMenu.Properties.Disabled = false;
            }
        }


        private void RadComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (DescriptionTypeSelector.SelectedIndex)
            {
                case 0:
                    //Selected Text (Rich Text or Raw HTML)

                    //check if the previous selection was Embed Web Page and if the URL had been specified
                    if (_previousSelection == 1 && !string.IsNullOrWhiteSpace(NodeProperties.DescriptionUrl))
                    {
                        SuperMessageBoxService.ShowWarning("Change Description Type", 
                            "The embedded web page will be removed and replaced with text.\r\n" +
                            "Are you sure you want to continue?",
                            "Yes", "No",
                            () =>
                            {
                                DescriptionNotice.Visibility = Visibility.Collapsed;
                                CbRichTextAdvanceMode.Visibility = Visibility.Visible;
                                IframePanel.Visibility = Visibility.Collapsed;
                                WidthHeightPanels.Visibility = Visibility.Visible;

                                //Clear the IFrame Details.
                                NodeProperties.DescriptionUrl = "";

                                if (CbRichTextAdvanceMode.IsChecked == true)
                                {
                                    RawPanel.Visibility = Visibility.Visible;
                                    DesignerPanel.Visibility = Visibility.Collapsed;
                                    DescriptionType = NodeDescriptionType.RawInput;
                                }
                                else
                                {
                                    RawPanel.Visibility = Visibility.Collapsed;
                                    DesignerPanel.Visibility = Visibility.Visible;
                                    DescriptionType = NodeDescriptionType.Html;
                                }
                            },
                            () =>
                            {
                                _previousSelection = -1; //this will prevent a double warning message
                                DescriptionTypeSelector.SelectedIndex = 1;
                            });
                    }
                    else
                    {
                        DescriptionNotice.Visibility = Visibility.Collapsed;
                        CbRichTextAdvanceMode.Visibility = Visibility.Visible;
                        IframePanel.Visibility = Visibility.Collapsed;
                        WidthHeightPanels.Visibility = Visibility.Visible;
                        if (CbRichTextAdvanceMode.IsChecked == true)
                        {
                            RawPanel.Visibility = Visibility.Visible;
                            DesignerPanel.Visibility = Visibility.Collapsed;
                            DescriptionType = NodeDescriptionType.RawInput;
                        }
                        else
                        {
                            RawPanel.Visibility = Visibility.Collapsed;
                            DesignerPanel.Visibility = Visibility.Visible;
                            DescriptionType = NodeDescriptionType.Html;
                        }
                    }
                    break;
                case 1:
                    //Selected Embed Web Page

                    //check if previous selection was Text and if there was any content entered
                    if (_previousSelection == 0 && ((DescriptionType == NodeDescriptionType.Html && !string.IsNullOrWhiteSpace(RichTextEditor.GetRawText())) ||
                        (DescriptionType == NodeDescriptionType.RawInput && !string.IsNullOrWhiteSpace(HtmlEditor.Text))))
                    {
                        SuperMessageBoxService.ShowWarning("Change Description Type",
                            "The text will be removed and replaced with embedded web page.\r\n" +
                            "Are you sure you want to continue?",
                            "Yes", "No",
                            () =>
                            {
                                DescriptionNotice.Visibility = Visibility.Collapsed;
                                IframePanel.Visibility = Visibility.Visible;
                                RawPanel.Visibility = Visibility.Collapsed;
                                DesignerPanel.Visibility = Visibility.Collapsed;
                                DescriptionType = NodeDescriptionType.Iframe;
                                CbRichTextAdvanceMode.Visibility = Visibility.Collapsed;
                                WidthHeightPanels.Visibility = Visibility.Visible;

                                //Clear the HTML editors
                                RichTextEditor.LoadHtml("");
                                HtmlEditor.Text = "";
                            },
                            () =>
                            {
                                _previousSelection = -1; //this will prevent a double warning message
                                DescriptionTypeSelector.SelectedIndex = 0;
                            });
                    }
                    else
                    {
                        DescriptionNotice.Visibility = Visibility.Collapsed;
                        IframePanel.Visibility = Visibility.Visible;
                        RawPanel.Visibility = Visibility.Collapsed;
                        DesignerPanel.Visibility = Visibility.Collapsed;
                        DescriptionType = NodeDescriptionType.Iframe;
                        CbRichTextAdvanceMode.Visibility = Visibility.Collapsed;
                        WidthHeightPanels.Visibility = Visibility.Visible;
                    }
                    break;
                case -1:
                    DescriptionNotice.Visibility = Visibility.Visible;
                    IframePanel.Visibility = Visibility.Collapsed;
                    WidthHeightPanels.Visibility = Visibility.Collapsed;
                    RawPanel.Visibility = Visibility.Collapsed;
                    DesignerPanel.Visibility = Visibility.Collapsed;
                    DescriptionType = NodeDescriptionType.None;
                    CbRichTextAdvanceMode.Visibility = Visibility.Collapsed;
                    break;
            }
            _previousSelection = DescriptionTypeSelector.SelectedIndex;
        }

        private void OnSaveClicked(object sender, RoutedEventArgs e)
        {
            if (NodeProperties != null)
            {
                var description = string.Empty;
                var iframeUrl = string.Empty;
                switch (DescriptionType)
                {
                    case NodeDescriptionType.Iframe:
                        iframeUrl = UrlTextBox.Text;
                        NodeProperties.RemoveMetadata("Description.Content");
                        break;
                    case NodeDescriptionType.Html:
                        description = RichTextEditor.GetRawText();
                        NodeProperties.RemoveMetadata("Description.Url");
                        break;
                    case NodeDescriptionType.RawInput:
                        description = HtmlEditor.Text;
                        NodeProperties.RemoveMetadata("Description.Url");
                        break;
                    case NodeDescriptionType.None:
                        description = string.Empty;
                        NodeProperties.RemoveMetadata("Description.Url");
                        NodeProperties.RemoveMetadata("Description.Width");
                        NodeProperties.RemoveMetadata("Description.Height");
                        NodeProperties.RemoveMetadata("Description.Content");
                        NodeProperties.RemoveMetadata("Description.Type");
                        
                        break;
                }

                if (DescriptionType != NodeDescriptionType.None)
                {
                    //If no content was entered in the HTML or RawInput assume the Description.Type is none and all should be removed
                    if (DescriptionType == NodeDescriptionType.Html || DescriptionType == NodeDescriptionType.RawInput)
                    {
                        if (description.Length > 8000)
                        {
                            SuperMessageBoxService.ShowError("Save Description Failed", "Sorry, your description is too long.", () => { RadTabControl.SelectedIndex = 1; });
                        }
                        else
                        {
                            if (NodeProperties is MultipleNodesProperties)
                            {
                                NodeProperties.DescriptionType = DescriptionType;

                                if (!string.IsNullOrEmpty(description))
                                {
                                    Description = description;
                                }
                            }
                            else
                            {
                                Description = description; //if setting an empty string it will remove the Description.Content metadata

                                if (!string.IsNullOrEmpty(description))
                                {
                                    NodeProperties.DescriptionType = DescriptionType;
                                }
                                else
                                {
                                    NodeProperties.RemoveMetadata("Description.Type"); //if setting an empty string it will remove the Description.Type when it was HTML or RawInput
                                    NodeProperties.DescriptionType = NodeDescriptionType.None; //trigger the PropertyChanged event - though not stored
                                }
                            }
                        }
                    }
                    else if (DescriptionType == NodeDescriptionType.Iframe)
                    {
                        if (NodeProperties is MultipleNodesProperties)
                        {
                            // If the DataContext is the MultipleNodesProperties and the URLs didn't all match the field will be an empty string.
                            NodeProperties.DescriptionType = DescriptionType;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(iframeUrl))
                            {
                                NodeProperties.DescriptionType = DescriptionType;
                            }
                            else
                            {
                                NodeProperties.RemoveMetadata("Description.Type"); //if no url is entered then clear the Description.Type metadata
                                NodeProperties.DescriptionType = NodeDescriptionType.None; //trigger the PropertyChanged event - though not stored
                            }
                        }
                    }
                    else
                    {
                        NodeProperties.DescriptionType = DescriptionType; //for future description types like GPS, though they may have their own validation
                    }
                }

                NodeProperties.EndEdit();
                Close();
            }
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e)
        {
            if (NodeProperties != null)
            {
                NodeProperties.CancelEdit();
            }
            Close();
        }

        private void CbRichTextAdvanceMode_Checked(object sender, RoutedEventArgs e)
        {
            SuperMessageBoxService.ShowWarning("Advanced Text", 
                "Some content might be automatically removed.\r\n" +
                "Are you sure you want to continue?", "Yes", "No",
                    () =>
                    {
                        IframePanel.Visibility = Visibility.Collapsed;
                        RawPanel.Visibility = Visibility.Visible;
                        DesignerPanel.Visibility = Visibility.Collapsed;
                        DescriptionType = NodeDescriptionType.RawInput;
                        HtmlEditor.Text = RichTextEditor.GetRawTextForAdvancedMode();
                    },
                    () =>
                    {
                        CbRichTextAdvanceMode.Unchecked -= CbRichTextAdvanceMode_Unchecked;
                        CbRichTextAdvanceMode.IsChecked = false;
                        CbRichTextAdvanceMode.Unchecked += CbRichTextAdvanceMode_Unchecked;
                    });
        }

        private void CbRichTextAdvanceMode_Unchecked(object sender, RoutedEventArgs e)
        {
            SuperMessageBoxService.ShowWarning("Advanced Text", "Some content might be automatically removed.\r\n" +
                                                          "Are you sure you want to continue?", "Yes", "No",
                    () =>
                    {
                        IframePanel.Visibility = Visibility.Collapsed;
                        RawPanel.Visibility = Visibility.Collapsed;
                        DesignerPanel.Visibility = Visibility.Visible;
                        DescriptionType = NodeDescriptionType.Html;
                        RichTextEditor.LoadHtml(HtmlEditor.Text);
                    },
                    () =>
                    {
                        CbRichTextAdvanceMode.Checked -= CbRichTextAdvanceMode_Checked;
                        CbRichTextAdvanceMode.IsChecked = true;
                        CbRichTextAdvanceMode.Checked += CbRichTextAdvanceMode_Checked;
                    });
        }
    }
}

