using System;
using System.Windows;
using System.Windows.Controls;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox
{
    [TemplateVisualState(Name = "Confirmation", GroupName = "MessageBoxStyle")]
    [TemplateVisualState(Name = "Error", GroupName = "MessageBoxStyle")]
    [TemplateVisualState(Name = "Information", GroupName = "MessageBoxStyle")]
    [TemplateVisualState(Name = "ErrorWithNoInput", GroupName = "MessageBoxStyle")]
    [TemplateVisualState(Name = "Warning", GroupName = "MessageBoxStyle")]
    public partial class MessageBoxContentControl : UserControl
    {
        private MessageBoxType _messageBoxType;
        private string _text;
        private string _yesButtonText;
        private string _noButtonText;
        private string _cancelButtonText;

        public event EventHandler<RoutedEventArgs> YesClicked;
        public event EventHandler<RoutedEventArgs> NoClicked;
        public event EventHandler<RoutedEventArgs> CancelClicked;

        public MessageBoxType MessageBoxType
        {
            get
            {
                return _messageBoxType;
            }
            set
            {
                _messageBoxType = value;
                VisualStateManager.GoToState(this, value.ToString(), true);
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                TbMessage.Text = _text;
            }
        }

        public string YesButtonText
        {
            get
            {
                return _yesButtonText;
            }
            set
            {
                _yesButtonText = value;
                YesButton.Content = _yesButtonText;
                YesButton.Visibility = !string.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public string CancelButtonText
        {
            get
            {
                return _cancelButtonText;
            }
            set
            {
                _cancelButtonText = value;
                CancelButton.Content = _cancelButtonText;
                CancelButton.Visibility = !string.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed;
                
            }
        }

        public string NoButtonText
        {
            get
            {
                return _noButtonText;
            }
            set
            {
                _noButtonText = value;
                NoButton.Content = _noButtonText;
                NoButton.Visibility = !string.IsNullOrEmpty(value) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public MessageBoxContentControl()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (CancelClicked != null)
            {
                CancelClicked(sender, e);
            }
            
        }

        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            if (YesClicked != null)
            {
                YesClicked(sender, e);
            }
        }


        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            if (NoClicked != null)
            {
                NoClicked(sender, e);
            }
        }
    }
}
