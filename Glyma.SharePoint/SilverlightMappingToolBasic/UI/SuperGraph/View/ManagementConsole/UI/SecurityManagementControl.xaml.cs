using SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.UI
{
    public partial class SecurityManagementControl : UserControl
    {
        private const string ROOTMAP_WARNING = "Warning this map has not had any groups associated with it yet, it will only be visible to users who are Glyma Security Managers.";
        private const string PROJECT_WARNING = "Warning this project has not had any groups associated with it yet, it will only be visible to users who are Glyma Security Managers.";

        public event RoutedEventHandler RestoreInheritanceButtonClicked;

        public event RoutedEventHandler BreakInheritanceButtonClicked;

        public event RoutedEventHandler ApplyButtonClicked;

        public SecurityManagementControl()
        {
            InitializeComponent();
        }

        private void SecurityBlockSelection_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                NoAssociationWarning.Visibility = Visibility.Collapsed;
                SecurityManagementBox.Visibility = Visibility.Visible;
                SecurityManagementBox.ItemsSource = (PermissionGroupCollection) e.AddedItems[0];
            }
        }

        private void SecurityBlockSelection_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            SecurityManagementBox.ItemsSource = null;

            SecurityManagementBox.Visibility = Visibility.Collapsed;
            IManagementConsoleObject securityObject = e.NewValue as IManagementConsoleObject;
            if (securityObject != null)
            {
                if (securityObject is RootMap)
                {
                    NoAssociationsWarningTextblock.Text = ROOTMAP_WARNING;
                }
                else if (securityObject is Project)
                {
                    NoAssociationsWarningTextblock.Text = PROJECT_WARNING;
                }

                if (securityObject.IsLoaded)
                {
                    if (securityObject.HasNoSecurityAssociations)
                    {
                        NoAssociationWarning.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        NoAssociationWarning.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    securityObject.LoadCompleted += securityObject_LoadCompleted;
                }
            }
        }

        private void securityObject_LoadCompleted(object sender, System.EventArgs e)
        {
            IManagementConsoleObject securityObject = DataContext as IManagementConsoleObject;
            if (securityObject != null)
            {
                if (securityObject.HasNoSecurityAssociations)
                {
                    NoAssociationWarning.Visibility = Visibility.Visible;
                }
                else
                {
                    NoAssociationWarning.Visibility = Visibility.Collapsed;
                }
            }
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
    }
}
