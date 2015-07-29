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

using SilverlightMappingToolBasic.MappingService;
using System.Threading;
using System.Globalization;

namespace SilverlightMappingToolBasic.Controls
{
    public partial class EditMetadataDialog : ChildWindow
    {
        public EditMetadataDialog()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(EditMetadataDialog_Loaded);
            this.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(EditMetadataDialog_Closing);
        }

        public string MetadataName
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public Guid MetadataTypeUid
        {
            get;
            set;
        }

        private void EditMetadataDialog_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Validate())
            {
                e.Cancel = false;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void EditMetadataDialog_Loaded(object sender, RoutedEventArgs e)
        {
            NumericUpDown.Maximum = (double)decimal.MaxValue;
            NumericUpDown.Minimum = (double)decimal.MinValue;

            TypeComboBox.Items.Clear();
            TypeManager typeManager = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>();
            IMetadataTypeProxy[] metaDataTypes = typeManager.GetAllMetadataTypes();

            foreach (IMetadataTypeProxy metadataProxy in metaDataTypes)
            {
                TypeComboBox.Items.Add(metadataProxy);
            }

            if (Value != null && MetadataTypeUid != Guid.Empty)
            {
                foreach (IMetadataTypeProxy type in TypeComboBox.Items)
                {
                    if (type.Id == MetadataTypeUid)
                    {
                        TypeComboBox.SelectedItem = type;
                        break;
                    }
                }
            }

            if (!string.IsNullOrEmpty(MetadataName))
            {
                NameTextBox.IsReadOnly = true;
                NameTextBox.Text = MetadataName;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private bool Validate()
        {
            if (DialogResult.HasValue && DialogResult.Value == false)
            {
                return true;
            }

            if (!this.NameTextBox.IsReadOnly)
            {
                MetadataName = this.NameTextBox.Text; //it's an add dialog
            }

            bool validated = false;
            IMetadataTypeProxy proxyType = TypeComboBox.SelectedItem as IMetadataTypeProxy;
            if (proxyType != null)
            {
                switch (proxyType.Name)
                {
                    case "datetime":
                        if (DatePicker.SelectedDate.HasValue)
                        {
                            validated = true; //it can't be wrong from this point
                            DateTime datetime = DatePicker.SelectedDate.Value;
                            //if (TimeUpDown.IsEnabled)
                            //{
                            //    datetime = datetime.Add(TimeUpDown.Value.Value.TimeOfDay);
                            //    Value = datetime.ToString();
                            //}
                            //else
                            //{
                            //    Value = datetime.ToShortDateString();
                            //}
                        }
                        else
                        {
                            validated = false;
                            DatePicker.Background = new SolidColorBrush(Colors.Yellow);
                            ValidationErrorTextBlock.Text = "No date has been seleccted";
                        }
                        break;
                    case "string":
                        validated = true; //it can't get it wrong
                        Value = StringTextBox.Text;
                        break;
                    case "double":
                        validated = true; //it can't get it wrong
                        Value = NumericUpDown.Value.ToString();
                        break;
                    case "timespan":
                        TimeSpan test;
                        validated = TimeSpan.TryParse(StringTextBox.Text, out test);
                        if (validated)
                        {
                            Value = test.ToString();
                        }
                        else
                        {
                            StringTextBox.Background = new SolidColorBrush(Colors.Yellow);
                            ValidationErrorTextBlock.Text = "Enter a valid TimeSpan value";
                        }
                        break;
                    default:
                        TypeComboBox.Background = new SolidColorBrush(Colors.Yellow);
                        ValidationErrorTextBlock.Text = "Choose the metadata content type";
                        break;
                }
            }
            return validated;
        }

        private void TypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IMetadataTypeProxy proxyType = TypeComboBox.SelectedItem as IMetadataTypeProxy;
            if (proxyType != null)
            {
                this.MetadataTypeUid = proxyType.Id;
                switch (proxyType.Name)
                {
                    case "datetime":
                        StringTextBox.Visibility = Visibility.Collapsed;
                        NumericUpDown.Visibility = Visibility.Collapsed;
                        DateTimePicker.Visibility = Visibility.Visible;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            DatePicker.DisplayDate = DateTime.Parse(Value);
                            DatePicker.Text = DateTime.Parse(Value).ToShortDateString();
                            //TimeUpDown.Value = DateTime.Parse(Value);
                            //if (TimeUpDown.Value.Value != DateTime.Parse("12:00AM"))
                            //{
                            //    //there is no way to have a DateTime without a time but this check
                            //    //makes sure data isn't lost if someone just ok's the dialog and a value 
                            //    //other than the default existed.
                            //    IncludeTimeCheckBox.IsChecked = true;
                            //}
                        }
                        break;
                    case "string":
                        StringTextBox.Visibility = Visibility.Visible;
                        NumericUpDown.Visibility = Visibility.Collapsed;
                        DateTimePicker.Visibility = Visibility.Collapsed;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            StringTextBox.Text = Value;
                        }
                        break;
                    case "double":
                        StringTextBox.Visibility = Visibility.Collapsed;
                        NumericUpDown.Visibility = Visibility.Visible;
                        DateTimePicker.Visibility = Visibility.Collapsed;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            NumericUpDown.Value = Double.Parse(Value);
                        }
                        break;
                    case "timespan":
                        StringTextBox.Visibility = Visibility.Visible;
                        NumericUpDown.Visibility = Visibility.Collapsed;
                        DateTimePicker.Visibility = Visibility.Collapsed;
                        if (!string.IsNullOrEmpty(Value))
                        {
                            StringTextBox.Text = Value;
                        }
                        break;
                    default:
                        StringTextBox.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void IncludeTimeCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //TimeUpDown.IsEnabled = true;
            //if (!TimeUpDown.Value.HasValue)
            //{
            //    TimeUpDown.Value = new DateTime(1, 1, 1, 12, 0, 0);
            //}
        }

        private void IncludeTimeCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //TimeUpDown.IsEnabled = false;
        }
    }
}

