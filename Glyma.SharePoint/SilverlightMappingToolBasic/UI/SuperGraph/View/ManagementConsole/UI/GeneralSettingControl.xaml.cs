using System;
using System.Windows.Controls;
using System.Windows;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.ViewModel;
using SilverlightMappingToolBasic.UI.SuperGraph.View.RichTextSupportClasses;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.UI
{
    public partial class GeneralSettingControl : UserControl
    {
        public GeneralSettingControl()
        {
            InitializeComponent();

            if (DataContext is RootMap)
            {
                YammerOptions.Visibility = RelatedContentPanelUtil.Instance.IsYammerAvailable() ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                YammerOptions.Visibility = Visibility.Collapsed;
            }
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            var managementObject = DataContext as ManagementConsoleObject;
            if (managementObject != null)
            {
                managementObject.UpdateName(ObjectNameBox.UIText);
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is RootMap)
            {
                YammerOptions.Visibility = RelatedContentPanelUtil.Instance.IsYammerAvailable() ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                YammerOptions.Visibility = Visibility.Collapsed;
            }
        }
    }
}
