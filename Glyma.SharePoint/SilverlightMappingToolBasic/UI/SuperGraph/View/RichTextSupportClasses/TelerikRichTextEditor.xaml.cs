using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;
using SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses;
using SilverlightMappingToolBasic.UI.ViewModel;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents;
using Telerik.Windows.Documents.FormatProviders.Html;
using Telerik.Windows.Documents.FormatProviders.Html.Import;
using Telerik.Windows.Documents.Layout;
using Telerik.Windows.Documents.Model;
using SelectionChangedEventArgs = Telerik.Windows.Controls.SelectionChangedEventArgs;
using Telerik.Windows.Documents.UI.Extensibility;
using System.ComponentModel.Composition.Hosting;

using Telerik.Windows.Controls.RichTextBoxUI.Dialogs;
using Telerik.Windows.Controls.RichTextBoxUI;
using Telerik.Windows.Documents.Proofing;
using EditCustomDictionaryDialog = Telerik.Windows.Controls.RichTextBoxUI.Dialogs.EditCustomDictionaryDialog;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.RichTextSupportClasses
{
    public partial class TelerikRichTextEditor : UserControl
    {
        private HtmlFormatProvider _htmlFormatProvider;
        private int _descriptionWidth;
        private int _descriptionHeight;
        private InsertPictureDialog _insertPictureDialog;

        public InsertPictureDialog InsertPictureDialog
        {
            get
            {
                if (_insertPictureDialog == null)
                {
                    _insertPictureDialog = new InsertPictureDialog();
                }
                return _insertPictureDialog;
            }
        }

        public HtmlFormatProvider HtmlFormatProvider
        {
            get
            {
                if (_htmlFormatProvider == null)
                {
                    _htmlFormatProvider = new HtmlFormatProvider();
                    _htmlFormatProvider.ExportSettings.StylesExportMode = StylesExportMode.Inline;
                    _htmlFormatProvider.ExportSettings.DocumentExportLevel = DocumentExportLevel.Fragment;
                    _htmlFormatProvider.ExportSettings.ExportBoldAsStrong = true;
                    _htmlFormatProvider.ExportSettings.ExportEmptyDocumentAsEmptyString = true;
                    _htmlFormatProvider.ExportSettings.ExportFontStylesAsTags = true;
                    _htmlFormatProvider.ExportSettings.ExportHeadingsAsTags = true;
                    _htmlFormatProvider.ExportSettings.ExportItalicAsEm = true;
                    _htmlFormatProvider.ExportSettings.ExportStyleMetadata = false;
                    _htmlFormatProvider.ExportSettings.ExportLocalOrStyleValueSource = false;
                    _htmlFormatProvider.ExportSettings.ImageExportMode = ImageExportMode.ImageExportingEvent;
                    _htmlFormatProvider.ExportSettings.ImageExporting += ExportSettingsOnImageExporting;

                    _htmlFormatProvider.ExportSettings.StyleRepositoryExportMode = StyleRepositoryExportMode.DontExportStyles;
                    _htmlFormatProvider.ImportSettings.UseDefaultStylesheetForFontProperties = false;
                    _htmlFormatProvider.ImportSettings.UseHtmlHeadingStyles = false;
                    _htmlFormatProvider.ImportSettings.LoadImageFromUrl += ImportSettingsOnLoadImageFromUrl;
                }
                return _htmlFormatProvider;
            }
        }

        private void ImportSettingsOnLoadImageFromUrl(object sender, LoadImageEventArgs e)
        {
            try
            {
                if (e.Url != null)
                {
                    var extension = System.IO.Path.GetExtension(e.Url);
                    var uri = new Uri(e.Url, UriKind.RelativeOrAbsolute);
                    var client = new WebClient();
                    client.OpenReadCompleted += (s, a) =>
                    {
                        if (a.Error == null)
                        {
                            try
                            {
                                e.ImageElement.Init(a.Result, extension.Remove(0,1));
                            }
                            catch
                            {
                                //Handle errors
                            }
                        }
                        else
                        {
                            //External URL
                        }
                    };
                    client.OpenReadAsync(uri);
                }
            }
            catch
            {
                //Handle errors
            }
        }


        private void ExportSettingsOnImageExporting(object sender, ImageExportingEventArgs e)
        {
            e.Src = e.Image.UriSource.ToString();
            if (e.Image.Height.Equals(10) && e.Image.Width.Equals(10))
            {
                
            }

        }

        public INodeProperties NodeProperties
        {
            get
            {
                var context = DataContext as INodeProperties;
                if (context != null)
                {
                    return context;
                }
                return null;
            }
        }

        public TelerikRichTextEditor()
        {
            InitializeComponent();
            InsertPictureDialog.InsertClicked += InsertPictureDialogOnInsertClicked;
            RadRichTextBox.IsImageMiniToolBarEnabled = false;

            RadCompositionInitializer.Catalog = new TypeCatalog(
                // format providers - we only use the HtmlFormatProvider
                //typeof(XamlFormatProvider),
                //typeof(RtfFormatProvider),
                //typeof(DocxFormatProvider),
                //typeof(PdfFormatProvider),
                typeof(HtmlFormatProvider),
                //typeof(TxtFormatProvider),

                // mini toolbars
                typeof(SelectionMiniToolBar),
                typeof(ImageMiniToolBar),

                // context menu
                typeof(Telerik.Windows.Controls.RichTextBoxUI.ContextMenu),

                // the default English spellchecking dictionary
                typeof(RadEn_USDictionary),

                // dialogs
                typeof(AddNewBibliographicSourceDialog),
                typeof(ChangeEditingPermissionsDialog),
                typeof(EditCustomDictionaryDialog),
                typeof(FindReplaceDialog),
                typeof(FloatingBlockPropertiesDialog),
                typeof(FontPropertiesDialog),
                //typeof(ImageEditorDialog), //we don't use the image editor dialog
                typeof(CodeFormattingDialog),
                typeof(InsertCaptionDialog),
                typeof(InsertCrossReferenceWindow),
                typeof(InsertDateTimeDialog),
                typeof(InsertTableDialog),
                typeof(InsertTableOfContentsDialog),
                typeof(ManageBibliographicSourcesDialog),
                typeof(ManageBookmarksDialog),
                typeof(ManageStylesDialog),
                typeof(NotesDialog),
                typeof(ProtectDocumentDialog),
                typeof(RadInsertHyperlinkDialog),
                typeof(RadInsertSymbolDialog),
                typeof(RadParagraphPropertiesDialog),
                typeof(SetNumberingValueDialog),
                typeof(SpellCheckingDialog),
                typeof(StyleFormattingPropertiesDialog),
                typeof(TableBordersDialog),
                typeof(TablePropertiesDialog),
                typeof(TabStopsPropertiesDialog),
                typeof(UnprotectDocumentDialog),
                typeof(WatermarkSettingsDialog)
            );
        }

        private void InsertPictureDialogOnInsertClicked(object sender, InsertImageEventArgs e)
        {
            var image = new ImageInline();
            image.UriSource = e.Url;
            image.Extension = Path.GetExtension(e.Url.ToString());
            image.Width = e.Width;
            image.Height = e.Height;
            // Insert the image at current caret position.
            RadRichTextBox.InsertInline(image);
        }

        public RadDocument RadDocument
        {
            get
            {
                return RadRichTextBox.Document;
            }
        }

        private void SetupNewDocument(RadDocument document)
        {
            document.LayoutMode = DocumentLayoutMode.Flow;
            //document.ParagraphDefaultSpacingAfter = 10;
            document.SectionDefaultPageMargin = new Padding(95);
        }

        private bool IsSupportedImageFormat(string extension)
        {
            if (extension != null)
            {
                extension = extension.ToLower();
            }

            return extension == ".jpg" ||
                extension == ".jpeg" ||
                extension == ".png" ||
                extension == ".bmp";
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (NodeProperties != null)
            {
                LoadHtml(NodeProperties.Description);
            }
            else
            {
                RadRichTextBox.Document = new RadDocument();
            }
        }

        public void LoadHtml(string description)
        {
            var html = new HtmlAgilityPack.HtmlDocument();
            html.LoadHtml(description);

            var doc = HtmlFormatProvider.Import(description);

            SetupNewDocument(doc);
            RadRichTextBox.Document = doc;
        }

        public string GetRawText()
        {
            var description = HtmlFormatProvider.Export(RadRichTextBox.Document);
            
            //if (_descriptionWidth > 0)
            //{
            //    //custom sizing provided
            //    return string.Format(@"<div id=""GlymaNodeDescriptionDiv"" width=""{0}"" height=""{1}"" style=""width:{0}px;height:{1}px;"">{2}</div>", _descriptionWidth, _descriptionHeight, description);
            //}
            if (string.IsNullOrWhiteSpace(description))
            {
                //no content, return empty
                return string.Empty;
            }
            return description;
            ////no custom sizing but want to mark it up as coming from the RichTextEdit rather than the advanced
            //return string.Format(@"<div id=""GlymaNodeDescriptionDiv"">{0}</div>", description);
        }

        /// <summary>
        /// This method is provided for when populating the HtmlEditor in Advanced (Raw) mode, it doesn't insert the GlymaNodeDescription DIV around the content
        /// </summary>
        /// <returns></returns>
        public string GetRawTextForAdvancedMode()
        {
            var description = HtmlFormatProvider.Export(RadRichTextBox.Document);
            if (description.Length > 8000)
            {
                SuperMessageBoxService.ShowError("Save Description Failed", "Sorry, your description is too long.");
                description = NodeProperties.Description;
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                //no content, return empty
                return string.Empty;
            }
            else
            {
                return description;
            }
        }

        //private void WidthSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var item = WidthSelector.SelectedItem as RadRibbonComboBoxItem;
        //    if (item != null)
        //    {
        //        int.TryParse(item.Tag.ToString(), out _descriptionWidth);
        //    }
        //    else
        //    {
        //        _descriptionWidth = 0;
        //    }
        //}

        //private void WidthSelector_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    if (WidthSelector.Text != "Default")
        //    {
        //        int temp;
        //        if (int.TryParse(WidthSelector.Text, out temp))
        //        {
        //            _descriptionWidth = temp;
        //        }
        //        else if (!string.IsNullOrEmpty(WidthSelector.Text))
        //        {
        //            WidthSelector.Text = _descriptionWidth.ToString();
        //        }
        //    }
        //}

        //private void HeightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var item = HeightSelector.SelectedItem as RadRibbonComboBoxItem;
        //    if (item != null)
        //    {
        //        int.TryParse(item.Tag.ToString(), out _descriptionHeight);
        //    }
        //    else
        //    {
        //        _descriptionHeight = 0;
        //    }
        //}

        //private void HeightSelector_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    if (HeightSelector.Text != "Default")
        //    {
        //        int temp;
        //        if (int.TryParse(HeightSelector.Text, out temp))
        //        {
        //            _descriptionHeight = temp;
        //        }
        //        else if (!string.IsNullOrEmpty(HeightSelector.Text))
        //        {
        //            HeightSelector.Text = _descriptionHeight.ToString();
        //        }
        //    }
        //}

        private void RadRibbonButton_Click(object sender, RoutedEventArgs e)
        {
            if (!RadRichTextBox.Document.Selection.IsEmpty)
            {
                var selected = RadRichTextBox.Document.Selection.GetSelectedSingleInline() as ImageInline;
                if (selected != null)
                {
                    InsertPictureDialog.DataContext = selected;
                }
            }
            else
            {
                InsertPictureDialog.DataContext = new ImageInline();
            }
            InsertPictureDialog.ShowDialog();
        }
    }
}
