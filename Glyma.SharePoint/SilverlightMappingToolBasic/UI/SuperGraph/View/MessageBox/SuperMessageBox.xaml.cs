using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox
{
    public partial class SuperMessageBox : ChildWindow
    {
        private MessageBoxType _messageBoxType;

        private readonly Action _callbackOnYes;
        private readonly Action _callbackOnNo;
        private readonly Action _callbackOnCancel;

        private string Caption
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Title = string.Format("{0}", _messageBoxType);                  
                }
                else
                {
                    Title = value;
                }
            }
        }

        private MessageBoxType MessageBoxType
        {
            set
            {
                _messageBoxType = value;
                Content.MessageBoxType = _messageBoxType;
            }
        }

        private string Text
        {
            set
            {
                Content.Text = value;
            }
        }

        private string YesButtonText
        {
            set
            {
                Content.YesButtonText = value;
            }
        }

        private string NoButtonText
        {
            set
            {
                Content.NoButtonText = value;
            }
        }

        private string CancelButtonText
        {
            set
            {
                Content.CancelButtonText = value;
            }
        }

        public SuperMessageBox(string caption, string message, MessageBoxType messageBoxType, Action callbackOnYes = null, Action callbackOnNo = null, Action callbackOnCancel = null)
        {
            InitializeComponent();

            _callbackOnYes = callbackOnYes;
            _callbackOnNo = callbackOnNo;
            _callbackOnCancel = callbackOnCancel;

            MessageBoxType = messageBoxType;
            Caption = caption;
            Text = message;

            Content.YesClicked += YesButtonClick;
            Content.NoClicked += NoButtonClick;
            Content.CancelClicked += CancelButtonClick;
        }



        public SuperMessageBox(string caption, string message, string yesButtonText, string noButtonText, MessageBoxType messageBoxType, Action callbackOnOk = null, Action callbackOnNo = null)
            : this(caption, message, messageBoxType, callbackOnOk, callbackOnNo, null)
        {
            YesButtonText = yesButtonText;
            NoButtonText = noButtonText;
            CancelButtonText = null;
        }

        public SuperMessageBox(string caption, string message, string yesButtonText, MessageBoxType messageBoxType, Action callbackOnOk = null)
            : this(caption, message, messageBoxType, callbackOnOk)
        {
            YesButtonText = yesButtonText;
            NoButtonText = null;
        }


        public SuperMessageBox(string caption, string message, string yesButtonText, string noButtonText, string cancelButtonText, MessageBoxType messageBoxType, Action callbackOnOk = null, Action callbackOnNo = null, Action callbackOnCancel = null)
            : this(caption, message, messageBoxType, callbackOnOk, callbackOnNo, callbackOnCancel)
        {
            YesButtonText = yesButtonText;
            NoButtonText = noButtonText;
            CancelButtonText = cancelButtonText;
        }

        private void YesButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            if (_callbackOnYes != null)
            {
                _callbackOnYes();
            }
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            if (_callbackOnCancel != null)
            {
                _callbackOnCancel();
            }
        }

        private void NoButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            if (_callbackOnNo != null)
            {
                _callbackOnNo();
            }
        }

        private void ChildWindow_Closed(object sender, EventArgs e)
        {
            if (DialogResult == false || DialogResult == null)
            {
                if (_callbackOnCancel != null)
                {
                    _callbackOnCancel();
                }
            }
        }

        private void SuperMessageBox_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}

