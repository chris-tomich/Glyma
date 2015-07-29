using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.RichTextBoxUI.Menus;
using Telerik.Windows.Documents.FormatProviders.Txt;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Controls.RichTextBoxUI.Dialogs;
using Telerik.Windows.Controls.RichTextBoxUI;
using Telerik.Windows.Documents.Proofing;
using EditCustomDictionaryDialog = Telerik.Windows.Controls.RichTextBoxUI.Dialogs.EditCustomDictionaryDialog;
using System.Globalization;
using Telerik.Windows.Documents.UI.Extensibility;
using System.ComponentModel.Composition.Hosting;
using Telerik.Windows.Documents.FormatProviders.Html;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.RichTextSupportClasses
{
    public class SpellCheckTextBox : RadRichTextBox
    {
        private static TxtFormatProvider _txtFormatProvider;
        private static string _originText;
        private bool _isEscape;

        public event EventHandler TextUpdated;
        public event EventHandler ClosedWithoutUpdate;
        public event EventHandler<TextFilledEventArgs> TextFilled;

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(SpellCheckTextBox), new PropertyMetadata("", OnTextPropertyChanged));

        //Property handles data binding
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        //Text of the UI level
        public string UIText
        {
            get
            {
                if (Document != null)
                {
                    return _txtFormatProvider.Export(Document);
                }
                return string.Empty;
            }
            set
            {
                _originText = value;
                Document = _txtFormatProvider.Import(value);
            }
        }

        public SpellCheckTextBox()
        {
            #region default values of simple rich textbox
            LayoutMode = DocumentLayoutMode.Flow;
            DocumentInheritsDefaultStyleSettings = true;
            AcceptsReturn = false;
            AcceptsTab = false;
            AutoInsertHyperlinks = false;
            FontFamily = new FontFamily("Trebuchet MS");
            IsImageMiniToolBarEnabled = false;
            IsContextMenuEnabled = true;
            IsSelectionMiniToolBarEnabled = false;
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            #endregion

            _txtFormatProvider = new TxtFormatProvider();
            LostFocus += UserControl_LostFocus;
            if (!App.IsDesignTime)
            {
                RadCompositionInitializer.Catalog = new TypeCatalog(
                    // format providers - we only use the TxtFormatProvider
                    typeof(TxtFormatProvider),

                    // the default English spellchecking dictionary
                    typeof(RadEn_USDictionary),

                    // dialogs
                    typeof(EditCustomDictionaryDialog),
                    typeof(SpellCheckingDialog)
                );

                // Next method call are required only to work around limitations for using MEF
                this.ContextMenu = new Telerik.Windows.Controls.RichTextBoxUI.ContextMenu();

                var contextMenu = (Telerik.Windows.Controls.RichTextBoxUI.ContextMenu)ContextMenu;
                contextMenu.ContentBuilder = new NodeTextInputContextMenuBuilder(this);
                contextMenu.Showing += ContextMenuOnShowing;
                contextMenu.Closed += ContextMenuOnClosed;
            }
        }

        private static void OnTextPropertyChanged(DependencyObject re, DependencyPropertyChangedEventArgs e)
        {
            var richEdit = (SpellCheckTextBox)re;

            if ((string)e.NewValue != null)
            {
                _originText = e.NewValue.ToString();
                richEdit.Document = _txtFormatProvider.Import(_originText);
            }
        }

        private void ContextMenuOnClosed(object sender, EventArgs e)
        {
            LostFocus += UserControl_LostFocus;
            var focusedObject = FocusManager.GetFocusedElement();
            if (focusedObject == null)
            {
                Focus();
            }
            else if (focusedObject != this && _originText != UIText && TextUpdated != null)
            {
                TextUpdated(sender, e);
            }
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        {
            if ((_isEscape || _originText == UIText) && ClosedWithoutUpdate != null)
            {
                ClosedWithoutUpdate(sender, e);
            }
            else if (_originText != UIText || (Text != UIText && string.IsNullOrEmpty(_originText)))
            {
                var test = FocusManager.GetFocusedElement();
                Text = UIText;
                if (TextUpdated != null)
                {
                    TextUpdated(sender, e);
                }
            }
            _isEscape = false;
        }

        private void ContextMenuOnShowing(object sender, ContextMenuEventArgs contextMenuEventArgs)
        {
            LostFocus -= UserControl_LostFocus;
        }

        protected override void OnHyperlinkClicked(object sender, HyperlinkClickedEventArgs eventArgs)
        {
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_originText != UIText && TextUpdated != null)
                {
                    TextUpdated(this, null);
                }
                else if (_originText == UIText && ClosedWithoutUpdate != null)
                {
                    ClosedWithoutUpdate(this, null);
                }
            }
            else if (e.Key == Key.Escape && ClosedWithoutUpdate != null)
            {
                _isEscape = true;
                ClosedWithoutUpdate(this, null);
                UIText = _originText;
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (TextFilled != null)
            {
                TextFilled(this, new TextFilledEventArgs { IsEmpty = string.IsNullOrWhiteSpace(UIText) });
            }
            base.OnKeyUp(e);
            e.Handled = true;
        }

        protected override void OnCurrentEditingStyleChanged()
        {
        }

        public override void ShowFindReplaceDialog()
        {
        }

        
    }
}
