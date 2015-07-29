using System;
using System.Windows;
using System.Windows.Controls;
using SilverlightMappingToolBasic.GlymaSecurityService;
using SilverlightMappingToolBasic.UI.Extensions.Security;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.ViewModel;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.UI
{
    public partial class ProjectListViewControlPanel : UserControl
    {
        public event RoutedEventHandler NewMapClicked;

        public event RoutedEventHandler NewProjectClicked;

        public event RoutedEventHandler DeleteClicked;

        public SecurityManager SecurityManager { get; set; }

        public ProjectListViewControlPanel()
        {
            InitializeComponent();
            if (!App.IsDesignTime)
            {
                NewProjectButton.IsEnabled = App.PermissionLevel >= PermissionLevel.ProjectManager;
            }
        }

        private void OnNewMapClicked(object sender, RoutedEventArgs e)
        {
            if (NewMapClicked != null)
            {
                NewMapClicked(DataContext, e);
            }
        }

        private void OnNewProjectClicked(object sender, RoutedEventArgs e)
        {
            if (NewProjectClicked != null)
            {
                NewProjectClicked(DataContext, e);
            }
        }

        private void OnDeleteClicked(object sender, RoutedEventArgs e)
        {
            if (DeleteClicked != null)
            {
                DeleteClicked(DataContext, e);
            }
        }

        private void ProjectListViewControlPanelDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            NewMapButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            var item = e.NewValue as IManagementConsoleObject;
            if (item != null)
            {
                if (!PermissionLevel.IsOldPermission(App.PermissionLevel))
                {
                    SecurityManager.GetPermissionNameForObjectAsync(item.Id, 
                        ClientOnGetHighestPermissionNameCompleted, 
                        new GlymaSecurableObject
                        {
                            SecurableParentUid = item.ParentId,
                            SecurableObjectUid = item.Id
                        });
                }
            }
        }

        private void ClientOnGetHighestPermissionNameCompleted(object sender, GetPermissionNameForObjectCompletedEventArgs e)
        {
            var item = DataContext as IManagementConsoleObject;
            if (!e.Result.HasError && item != null)
            {
                var id = sender as Guid?;
                if (id != null && id == item.Id)
                {
                    item.CurrentUserPermissionLevel = PermissionLevel.Convert(e.Result.Result);
                    //todo: if the user is security manager, needs to be returned as well
                    if (item.CurrentUserPermissionLevel >= PermissionLevel.Author || App.PermissionLevel == PermissionLevel.SecurityManager)
                    {
                        NewMapButton.IsEnabled = true;
                        if (item.CurrentUserPermissionLevel >= PermissionLevel.MapManager || App.PermissionLevel == PermissionLevel.SecurityManager)
                        {
                            DeleteButton.IsEnabled = true;
                        }
                    }

                    if (App.PermissionLevel != PermissionLevel.SecurityManager)
                    {
                        item.IsLoaded = true;
                    }
                }
            }
            else
            {
                SuperMessageBoxService.ShowError("Error Occurred", "There was a problem reading Glyma permissions, please try again later");
            }
        }
    }
}
