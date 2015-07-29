using System.Windows;
using System.Windows.Controls;
using Glyma.UtilityService.Proxy;
using SilverlightMappingToolBasic.UI.Extensions.Security;
using SilverlightMappingToolBasic.UI.SuperGraph.View.DomainAndMapSelection;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;
using SimpleIoC;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.UI
{
    public partial class ManagementConsoleTabControl : UserControl
    {
        public event RoutedEventHandler RestoreInheritanceButtonClicked;

        public event RoutedEventHandler BreakInheritanceButtonClicked;

        public event RoutedEventHandler ApplyButtonClicked;

        public event RoutedEventHandler LoadMapButtonClicked;

        public IExportServiceManager ExportServiceManager
        {
            get
            {
                return IoCContainer.GetInjectionInstance().GetInstance<IExportServiceManager>();
            }
        }

        public bool ApplyEnabled
        {
            get
            {
                return ApplyButton.IsEnabled;
            }
            set
            {
                ApplyButton.IsEnabled = value;
            }
        }

        public ManagementConsoleTabControl()
        {
            InitializeComponent();
            if (!App.IsDesignTime)
            {
                PermissionsTab.IsEnabled = App.PermissionLevel == PermissionLevel.SecurityManager;
            }
        }

        private void NodeManagementTabControl_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            GeneralSettingControl.DataContext = e.NewValue;
            SecurityManagementControl.DataContext = e.NewValue;
            ExportTab.DataContext = e.NewValue;
            Visibility = e.NewValue != null ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BreakInheritanceButton_Click(object sender, RoutedEventArgs e)
        {
            if (BreakInheritanceButtonClicked != null)
            {
                BreakInheritanceButtonClicked(DataContext, e);
            }
        }

        private void RestoreInheritanceButton_Click(object sender, RoutedEventArgs e)
        {
            if (RestoreInheritanceButtonClicked != null)
            {
                RestoreInheritanceButtonClicked(DataContext, e);
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (ApplyButtonClicked != null)
            {
                ApplyButtonClicked(DataContext, e);
            }
        }

        private void RadTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RadTabControl != null)
            {
                if (RadTabControl.SelectedIndex == 1)
                {
                    PermissionManagementButtons.Visibility = Visibility.Visible;
                }
                else
                {
                    PermissionManagementButtons.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void LoadMap_Click(object sender, RoutedEventArgs e)
        {
            if (LoadMapButtonClicked != null)
            {
                LoadMapButtonClicked(DataContext, e);
            }
        }
    }
}
