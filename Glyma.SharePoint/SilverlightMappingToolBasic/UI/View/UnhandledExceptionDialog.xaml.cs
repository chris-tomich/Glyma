using System;
using System.IO;
using System.Text;
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
using Glyma.Debug;

namespace SilverlightMappingToolBasic.UI.View
{
    public partial class UnhandledExceptionDialog : ChildWindow
    {
        private const String FileFilter = "CSV files (*.csv)|*.csv";

        public UnhandledExceptionDialog()
        {
            InitializeComponent();
        }

        public Exception UnhandledException
        {
            get;
            set;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void SaveErrorDetails_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = FileFilter;

            var showDialog = saveFileDialog.ShowDialog();
            if (showDialog != null && showDialog.Value)
            {
                Stream stream = saveFileDialog.OpenFile();

                StringBuilder debugContent = new StringBuilder();

                if (UnhandledException != null)
                {
                    debugContent.AppendLine("Exception Details");
                    debugContent.AppendLine("-------------------------------------");
                    debugContent.AppendLine("Exception Type: '" + UnhandledException.GetType().Name + "'");
                    debugContent.AppendLine("Message: '" + UnhandledException.Message + "'");
                    debugContent.AppendLine("Stack:");
                    debugContent.AppendLine(UnhandledException.StackTrace);

                    if (UnhandledException.InnerException != null)
                    {
                        debugContent.AppendLine("=====================================");
                        debugContent.AppendLine("Inner Exception Details");
                        debugContent.AppendLine("Exception Type: '" + UnhandledException.InnerException.GetType().Name + "'");
                        debugContent.AppendLine("Message: '" + UnhandledException.InnerException.Message + "'");
                        debugContent.AppendLine("Stack:");
                        debugContent.AppendLine(UnhandledException.InnerException.StackTrace);
                        debugContent.AppendLine("=====================================");
                    }

                    debugContent.AppendLine("-------------------------------------");
                    debugContent.AppendLine();
                }

                debugContent.AppendLine(DebugLogger.Instance.Log);

                byte[] textBytes = UTF8Encoding.UTF8.GetBytes(debugContent.ToString());

                stream.Write(textBytes, 0, textBytes.Length);
                stream.Close();
            }
        }
    }
}

