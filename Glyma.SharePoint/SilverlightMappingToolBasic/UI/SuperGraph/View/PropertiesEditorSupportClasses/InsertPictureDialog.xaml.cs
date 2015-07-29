using System;
using System.Windows;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Model;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses
{
    public partial class InsertPictureDialog : RadWindow
    {
        public ImageInline ImageInline
        {
            get
            {
                return DataContext as ImageInline;
            }
        }

        public event EventHandler<InsertImageEventArgs> InsertClicked;
        public InsertPictureDialog()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Uri uri;
            if (Uri.TryCreate(UrlTextBox.Text, UriKind.Absolute, out uri))
            {
                if (HeightTextBox.Value != null && WidthTextBox.Value != null && InsertClicked != null)
                {
                    var args = new InsertImageEventArgs
                    {
                        Url = ImageInline.UriSource,
                        Height = (int)ImageInline.Height,
                        Width = (int)ImageInline.Width
                    };
                    InsertClicked(sender, args);
                }
                else
                {
                    SuperMessageBoxService.ShowError("Error", "Please specify width and height for the image.");
                }
            }
            else
            {
                SuperMessageBoxService.ShowError("Error","Invalid URL entered");
            }
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RadWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            DataContext = null;
        }

        private void RadWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //var imageInline = e.NewValue as ImageInline;
            //if (imageInline != null)
            //{
            //    ImageInline = imageInline;
            //}
        }
    }
}
