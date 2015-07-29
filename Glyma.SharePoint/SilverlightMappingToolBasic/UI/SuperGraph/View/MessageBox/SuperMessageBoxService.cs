using System;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox
{
    public class SuperMessageBoxService
    {
        public static void ShowInformation(string caption, string message, Action callback = null)
        {
            var messageBox = new SuperMessageBox(caption, message, MessageBoxType.Information, callback);
            messageBox.Show();
        }

        public static void ShowError(string caption, string message, Action callback = null)
        {
            var messageBox = new SuperMessageBox(caption, message, MessageBoxType.Error, callback);
            messageBox.Show();
        }

        public static void ShowConfirmation(string caption, string message, Action okayAction, Action cancelAction = null)
        {
            var messageBox = new SuperMessageBox(caption, message, "OK", "Cancel", MessageBoxType.Confirmation, okayAction, cancelAction);
            messageBox.Show();
        }

        public static void ShowWarning(string caption, string message, Action okayAction, Action cancelAction = null) 
        {
            var messageBox = new SuperMessageBox(caption, message, MessageBoxType.Warning, okayAction, cancelAction);
            messageBox.Show();
        }

        public static void ShowWarning(string caption, string message, string yesButtonText, Action yesAction)
        {
            var messageBox = new SuperMessageBox(caption, message, yesButtonText, MessageBoxType.Warning, yesAction);
            messageBox.Show();
        }

        public static void ShowWarning(string caption, string message, string yesButtonText, string noButtonText, Action yesAction, Action noAction = null)
        {
            var messageBox = new SuperMessageBox(caption, message, yesButtonText, noButtonText, MessageBoxType.Warning, yesAction, noAction);
            messageBox.Show();
        }

        public static void ShowWarning(string caption, string message, string yesButtonText, string noButtonText, string cancelButtonText, Action yesAction, Action noAction = null, Action cancelAction = null)
        {
            var messageBox = new SuperMessageBox(caption, message, yesButtonText, noButtonText, cancelButtonText, MessageBoxType.Warning, yesAction, noAction, cancelAction);
            messageBox.Show();
        }

        public static void Show(string caption, string message, MessageBoxType messageBoxType, Action callback = null)
        {
            var messageBox = new SuperMessageBox(caption, message, messageBoxType, callback);
            messageBox.Show();
        }

        public static void Show(string caption, string message, string yesButtonText, string noButtonText, MessageBoxType messageBoxType, Action yesAction = null, Action noAction = null)
        {
            var messageBox = new SuperMessageBox(caption, message, yesButtonText, noButtonText, messageBoxType, yesAction, noAction);
            messageBox.Show();
        }

        public static void Show(string caption, string message, string yesButtonText, string noButtonText, string cancelButtonText, MessageBoxType messageBoxType, Action yesAction = null, Action noAction = null, Action cancelAction = null)
        {
            var messageBox = new SuperMessageBox(caption, message, yesButtonText, noButtonText, cancelButtonText, messageBoxType, yesAction, noAction, cancelAction);
            messageBox.Show();
        }
    }
}
