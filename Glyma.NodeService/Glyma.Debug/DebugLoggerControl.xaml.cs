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
using System.Text;
using System.IO;

namespace Glyma.Debug
{
    public partial class DebugLoggerControl : UserControl
    {
        private const String FileFilter = "CSV files (*.csv)|*.csv";

        public DebugLoggerControl()
        {
            InitializeComponent();
        }

        private void SaveDebugOutput_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = FileFilter;

            if (saveFileDialog.ShowDialog().Value)
            {
                Stream stream = saveFileDialog.OpenFile();

                byte[] textBytes = UTF8Encoding.UTF8.GetBytes(DebugLogger.Instance.Log);

                stream.Write(textBytes, 0, textBytes.Length);
                stream.Close();
            }
        }

        private void AddLineToOutput_Click(object sender, RoutedEventArgs e)
        {
            DebugLogger.Instance.LogMsg("Custom Message -" + "'" + CustomMessage.Text + "'");

            CustomMessage.Text = "";
        }
    }
}
