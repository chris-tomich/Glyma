using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SilverlightMappingToolBasic.GlymaSecurityService;
using SilverlightMappingToolBasic.UI.Extensions.Security;
using SilverlightMappingToolBasic.UI.SuperGraph.View.DomainAndMapSelection;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.ViewModel;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.DragDrop;
using TransactionalNodeService.Proxy;
using TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.UI
{
    public partial class ManagementConsoleDialog : ChildWindow
    {
        private ObservableCollection<Project> _projects;
        private RadTreeViewItem _selectedProjectItemToExpand;
        private readonly SecurityManager _securityManager;

        private Dictionary<PermissionLevel, ObservableCollection<GlymaSecurityGroup>> _securityGroups;
        private ObservableCollection<GlymaSecurityGroup> _allSecurityGroups;
        private Dictionary<IManagementConsoleObject, Dictionary<GlymaSecurityGroup, UpdateValue>> _updates;

        private Dictionary<IManagementConsoleObject, Dictionary<string, string>> _metadataUpdates;

        public event EventHandler LoadMapClicked;


        public DomainSelectionDialog Ref { get; set; }

        public Dictionary<IManagementConsoleObject, Dictionary<GlymaSecurityGroup, UpdateValue>> Updates
        {
            get
            {
                if (_updates == null)
                {
                    _updates = new Dictionary<IManagementConsoleObject, Dictionary<GlymaSecurityGroup, UpdateValue>>();
                }
                return _updates;
            }
        }

        public Dictionary<IManagementConsoleObject, Dictionary<string, string>> MetadataUpdates
        {
            get
            {
                if (_metadataUpdates == null)
                {
                    _metadataUpdates = new Dictionary<IManagementConsoleObject, Dictionary<string, string>>();
                }
                return _metadataUpdates;
            }
        }

        public Dictionary<PermissionLevel, ObservableCollection<GlymaSecurityGroup>> SecurityGroups
        {
            get
            {
                if (_securityGroups == null)
                {
                    _securityGroups = new Dictionary<PermissionLevel, ObservableCollection<GlymaSecurityGroup>>();
                }
                return _securityGroups;
            }
        }

        private ObservableCollection<GlymaSecurityGroup> AllSecurityGroups
        {
            get
            {
                if (_allSecurityGroups == null)
                {
                    _allSecurityGroups = new ObservableCollection<GlymaSecurityGroup>();
                }
                return _allSecurityGroups;
            }
        }

        private IMapManager MapManager { get; set; }

        public ManagementConsoleDialog(IMapManager mapManager, SecurityManager securityManager)
        {
            MapManager = mapManager;
            _securityManager = securityManager;
            InitializeComponent();
            ProjectListViewControlPanel.SecurityManager = _securityManager;
            if (App.PermissionLevel == PermissionLevel.SecurityManager)
            {
                _securityManager.GetAllSecurityGroupsAsync("ManagementConsoleDialog",
                    ClientOnGetAllSecurityGroupsCompleted);
            }
            else
            {
                MapManager.QueryDomainsCompleted.RegisterEvent(OnAcquireDomainsCompleted);
                MapManager.QueryDomainsAsync();
            }
            this.AddHandler(RadDragAndDropManager.DragInfoEvent, new EventHandler<DragDropEventArgs>(OnDragInfo), true);
        }


        private void OnDragInfo(object sender, DragDropEventArgs e)
        {
            if (e.Options.Status == DragStatus.DragInProgress)
            {
                var dragCue = new ContentControl {ContentTemplate = Resources["DragTemplate"] as DataTemplate};
                var list = e.Options.Payload as IList;
                if (list != null)
                {
                    var payload = list[0];
                    dragCue.Content = payload;
                }
                e.Options.DragCue = dragCue;
            }
        }

        private void UpdateCompleted(object sender, UpdateSecurityAssociationsCompletedEventArgs e)
        {
            if (!e.Result.HasError)
            {
                UpdateSuccessful();
            }
            else
            {
                SuperMessageBoxService.ShowError("Error Occurred",
                    "An error occurred while applying the Glyma permissions.");
            }
        }

        private void UpdateSuccessful()
        {
            MetadataUpdates.Clear();
            Updates.Clear();
            PermissionDetails.ApplyEnabled = false;
            ProcessIndicatorPanel.Visibility = Visibility.Collapsed;
        }

        private void ClientOnGetAllSecurityGroupsCompleted(object sender, GetAllSecurityGroupsCompletedEventArgs e)
        {
            if (!e.Result.HasError)
            {
                foreach (var pair in e.Result.Result)
                {
                    SecurityGroups.Add(PermissionLevel.Convert(pair.Key), pair.Value);
                }
                foreach (var valuePair in SecurityGroups)
                {
                    foreach (var item in valuePair.Value)
                    {
                        if (AllSecurityGroups.All(q => q.GroupId != item.GroupId))
                        {
                            AllSecurityGroups.Add(item);
                        }
                    }
                }
                MapManager.QueryDomainsCompleted.RegisterEvent(OnAcquireDomainsCompleted);
                MapManager.QueryDomainsAsync();
            }
            else
            {
                SuperMessageBoxService.ShowError("Error Occurred",
                    "An error occurred while retrieving the groups with Glyma permissions.", Close);
            }
        }

        private void OnAcquireDomainsCompleted(object sender, TransactionalNodeService.Proxy.NodesEventArgs e)
        {
            XTreeView.Items.Clear();
            _projects = new ObservableCollection<Project>();
            foreach (var node in e.Nodes.Values)
            {
                if (node != null)
                {
                    var project = new Project(node, SecurityGroups);
                    if (App.PermissionLevel == PermissionLevel.SecurityManager)
                    {
                        project.PermissionChanged += OnPermissionChanged;
                    }
                    project.MetadataChanged += OnMetadataChanged;
                    _projects.Add(project);
                }
            }
            ObservableCollection<Project> sortedCollection = new ObservableCollection<Project>();
            foreach (Project project in _projects.OrderBy(p => p.DisplayName))
            {
                sortedCollection.Add(project);
            }
            _projects = sortedCollection;
            XTreeView.ItemsSource = _projects;
        }

        private void OnMetadataChanged(object sender, MetadataChangedEventArgs e)
        {
            var securityObject = sender as IManagementConsoleObject;
            if (securityObject != null)
            {
                if (MetadataUpdates.ContainsKey(securityObject))
                {
                    if (MetadataUpdates[securityObject].ContainsKey(e.Key))
                    {
                        MetadataUpdates[securityObject][e.Key] = e.Value;
                    }
                    else
                    {
                        MetadataUpdates[securityObject].Add(e.Key, e.Value);
                    }
                }
                else
                {
                    MetadataUpdates.Add(securityObject, new Dictionary<string, string>());
                    MetadataUpdates[securityObject].Add(e.Key, e.Value);
                }
                PermissionDetails.ApplyEnabled = true;
            }
        }

        private void OnPermissionChanged(object sender, PermissionValueChangedEventArgs e)
        {
            var project = sender as Project;
            if (project != null)
            {
                PermissionDetails.ApplyEnabled = true;
                AddUpdate(project, e.ChangedItem.Group, e.ChangedItem.IsSelected);
                if (project.RootMaps.Count > 0)
                {
                    var newValue = e.ChangedItem.IsSelected.HasValue && e.ChangedItem.IsSelected.Value;
                    foreach (var rootMap in project.RootMaps)
                    {
                        if (rootMap.IsInherited)
                        {
                            rootMap.LoadValue(e.ChangedItem.Group, newValue);
                            AddUpdate(rootMap, e.ChangedItem.Group, newValue, !newValue);
                        }
                    }
                }
            }
            else
            {
                var rootMap = sender as RootMap;
                if (rootMap != null)
                {
                    PermissionDetails.ApplyEnabled = true;
                    AddUpdate(rootMap, e.ChangedItem.Group, e.ChangedItem.IsSelected);
                }
            }
        }

        private void AddUpdate(IManagementConsoleObject managementConsoleObject, GlymaSecurityGroup group, bool? value,
            bool isUpdatable = true)
        {
            if (Updates.ContainsKey(managementConsoleObject))
            {
                if (Updates[managementConsoleObject].ContainsKey(group) &&
                    Updates[managementConsoleObject][group].IsUpdatable == isUpdatable)
                {
                    Updates[managementConsoleObject][group].IsChecked = value.HasValue && value.Value;
                }
                else
                {
                    Updates[managementConsoleObject].Add(group,
                        new UpdateValue(value.HasValue && value.Value, isUpdatable));
                }
            }
            else
            {
                var newGroups = new Dictionary<GlymaSecurityGroup, UpdateValue>
                {
                    {group, new UpdateValue(value.HasValue && value.Value, isUpdatable)}
                };
                Updates.Add(managementConsoleObject, newGroups);
            }
        }


        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ProcessIndicatorPanel.Visibility = Visibility.Visible;
            ApplyChanges();
        }

        private void ApplyChanges()
        {

            foreach (var metadataUpdate in MetadataUpdates)
            {
                var chain = new TransactionChain();
                foreach (var updatePair in metadataUpdate.Value)
                {
                    metadataUpdate.Key.Node.Metadata.Add(null, null, updatePair.Key, updatePair.Value, ref chain);
                    if (updatePair.Key == "Name")
                    {
                        metadataUpdate.Key.Name = updatePair.Value;
                    }
                }
                metadataUpdate.Key.Node.MapManager.ExecuteTransaction(chain);
            }

            if (Updates.Count > 0)
            {
                var updateQueries = new ObservableCollection<GlymaSecurityAssociation>();
                foreach (var update in Updates)
                {
                    foreach (var updatePair in update.Value)
                    {
                        if (updatePair.Value.IsUpdatable)
                        {
                            updateQueries.Add(update.Key.CommitChange(updatePair.Key, updatePair.Value.IsChecked));
                        }
                        else
                        {
                            update.Key.CommitChange(updatePair.Key, updatePair.Value.IsChecked);
                        }
                    }
                }
                if (updateQueries.Count > 0)
                {
                    _securityManager.UpdateSecurityAssociationsAsync("UpdateSecurityAssociations", UpdateCompleted,
                        updateQueries);
                }
            }
            else
            {
                UpdateSuccessful();
            }
        }

        private void XTreeView_OnLoadOnDemand(object sender, RadRoutedEventArgs e)
        {
            var clickedItem = e.OriginalSource as RadTreeViewItem;
            if (clickedItem != null)
            {
                var securityObject = clickedItem.DataContext as IManagementConsoleObject;

                if (securityObject != null)
                {
                    if (_selectedProjectItemToExpand == null)
                    {
                        if (securityObject is Project)
                        {
                            _selectedProjectItemToExpand = clickedItem;
                            if (App.PermissionLevel == PermissionLevel.SecurityManager)
                            {
                                _securityManager.GetSecurityAssociationsAsync(securityObject,
                                    ClientOnGetSecurityAssociationsCompleted, AllSecurityGroups,
                                    new GlymaSecurableObject
                                    {
                                        SecurableParentUid = Guid.Empty,
                                        SecurableObjectUid = securityObject.Id
                                    });
                            }
                            MapManager.QueryMapByDomainCompleted.RegisterEvent(securityObject.Node.DomainId,
                                OnAcquireNodeByDomainCompleted);
                            MapManager.QueryMapByDomainAsync(securityObject.Node.DomainId);
                        }
                        else if (securityObject is RootMap)
                        {
                            clickedItem.IsLoadOnDemandEnabled = false;
                            clickedItem.IsLoadingOnDemand = false;
                        }
                    }
                    else
                    {
                        SuperMessageBoxService.ShowWarning("Busy",
                            "We are currently working on retreiving data from the server, please try again later", "OK",
                            null);
                        clickedItem.IsLoadOnDemandEnabled = true;
                        clickedItem.IsLoadingOnDemand = false; //allow it to be loaded later
                    }
                }
            }
        }

        private void ClientOnGetSecurityAssociationsCompleted(object sender, GetSecurityAssociationsCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                if (!e.Result.HasError)
                {
                    var securityObject = sender as ManagementConsoleObject;
                    if (securityObject != null)
                    {
                        securityObject.Load(e.Result.Result, e.Result.Result.IsInherited);

                        var selectedItem = XTreeView.SelectedItem;
                        if (selectedItem != null)
                        {
                            PermissionDetails.DataContext = null;
                            var binding = new Binding
                            {
                                Source = XTreeView,
                                Path = new PropertyPath("SelectedItem"),
                                Mode = BindingMode.TwoWay,
                            };
                            PermissionDetails.SetBinding(DataContextProperty, binding);
                            PermissionDetails.Visibility = Visibility.Visible;
                            XTreeView.SelectedItem = selectedItem;
                        }
                    }
                }
                else
                {
                    SuperMessageBoxService.ShowError("Error Occurred",
                        "An error occurred while retrieving the Glyma permissions.");
                }
            }
        }

        private void OnAcquireNodeByDomainCompleted(object sender, TransactionalNodeService.Proxy.NodesEventArgs e)
        {
            if (e.Context is Guid && _selectedProjectItemToExpand != null)
            {
                var project = _projects.FirstOrDefault(q => q.Node.DomainId == (Guid) e.Context);
                if (project != null)
                {
                    foreach (var node in e.Nodes.Values)
                    {
                        if (Equals(node.NodeType, MapManager.NodeTypes["CompendiumMapNode"]))
                        {
                            var map = new RootMap(node, SecurityGroups, project.Name);
                            if (App.PermissionLevel == PermissionLevel.SecurityManager)
                            {
                                map.PermissionChanged += OnPermissionChanged;
                            }
                            map.MetadataChanged += OnMetadataChanged;
                            project.RootMaps.Add(map);
                        }
                    }

                    if (project.RootMaps.Count > 0)
                    {
                        ObservableCollection<RootMap> sortedCollection = new ObservableCollection<RootMap>();
                        foreach (RootMap rootmap in project.RootMaps.OrderBy(rm => rm.DisplayName))
                        {
                            sortedCollection.Add(rootmap);
                        }
                        project.RootMaps = sortedCollection;
                        _selectedProjectItemToExpand.ItemsSource = project.RootMaps;
                    }
                }
                _selectedProjectItemToExpand.IsLoadOnDemandEnabled = false;
                _selectedProjectItemToExpand.IsLoadingOnDemand = false;
                _selectedProjectItemToExpand = null;
            }
        }

        private void XTreeView_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (sender == this)
            {
                return;
            }
            var isSkip = false;
            if (e.AddedItems.Count == 1)
            {
                var securityObject = e.AddedItems[0] as IManagementConsoleObject;
                if (securityObject != null)
                {
                    ProjectListViewControlPanel.DataContext = securityObject;
                    if (e.RemovedItems.Count == 1 && (Updates.Count > 0 || MetadataUpdates.Count > 0))
                    {
                        var oldItem = e.RemovedItems[0];
                        SuperMessageBoxService.ShowWarning("Confirmation",
                            "Do you want to apply your changes? \r\n\r\n" +
                            "Press YES to apply\r\n" +
                            "Press No to discard the changes you have made\r\n",
                            "Yes", "No", "Cancel",
                            ApplyChanges,
                            () =>
                            {
                                foreach (var update in Updates)
                                {
                                    foreach (var updatePair in update.Value)
                                    {
                                        update.Key.ResetChange(updatePair.Key);
                                    }
                                }
                                Updates.Clear();
                                MetadataUpdates.Clear();
                                PermissionDetails.ApplyEnabled = false;
                                var oldObject = oldItem as IManagementConsoleObject;
                                if (oldObject != null)
                                {
                                    oldObject.ReloadMetadata();
                                }
                                XTreeView.SelectedItem = e.AddedItems[0];
                            },
                            () =>
                            {
                                XTreeView.SelectedItem = oldItem;
                                isSkip = true;
                            });
                    }


                    if (!isSkip && App.PermissionLevel == PermissionLevel.SecurityManager)
                    {
                        if (securityObject is Project)
                        {
                            if (!securityObject.IsLoaded)
                            {
                                _securityManager.GetSecurityAssociationsAsync(securityObject,
                                    ClientOnGetSecurityAssociationsCompleted, AllSecurityGroups,
                                    new GlymaSecurableObject
                                    {
                                        SecurableParentUid = Guid.Empty,
                                        SecurableObjectUid = securityObject.Id
                                    });
                            }
                            foreach (var project in _projects)
                            {
                                if (project.Id != securityObject.Id)
                                {
                                    project.IsSelected = false;
                                }
                            }
                        }
                        else if (securityObject is RootMap)
                        {
                            if (!securityObject.IsLoaded)
                            {
                                _securityManager.GetSecurityAssociationsAsync(securityObject,
                                    ClientOnGetSecurityAssociationsCompleted, AllSecurityGroups,
                                    new GlymaSecurableObject
                                    {
                                        SecurableParentUid = securityObject.ParentId,
                                        SecurableObjectUid = securityObject.Id
                                    });
                            }
                            foreach (var project in _projects)
                            {
                                if (project.Id == securityObject.ParentId)
                                {
                                    foreach (var rootMap in project.RootMaps)
                                    {
                                        if (rootMap.Id != securityObject.Id)
                                        {
                                            rootMap.IsSelected = false;
                                        }
                                    }
                                }
                                else
                                {
                                    project.IsSelected = false;
                                }
                            }
                        }
                    }

                }
                Focus();
                XTreeView.Focus();
            }
        }

        private void PermissionDetails_OnBreakInheritanceButtonClicked(object sender, RoutedEventArgs e)
        {
            SuperMessageBoxService.ShowConfirmation("Break Inheritance Warning",
                "You are about to create unique permissions for this map. Changes made to the project permissions will no longer affect this map.",
                () =>
                {
                    var rootMap = sender as RootMap;
                    if (rootMap != null)
                    {
                        _securityManager.BreakRootMapInheritanceAsync(rootMap, BreakRootMapInheritanceCompleted,
                            rootMap.ParentId, rootMap.Id);
                    }
                });
        }



        private void PermissionDetails_OnRestoreInheritanceButtonClicked(object sender, RoutedEventArgs e)
        {
            SuperMessageBoxService.ShowConfirmation("Restore Inheritance Warning",
                "You are about to inherit permissions from the project. Any custom permissions will be lost.",
                () =>
                {

                    var rootMap = sender as RootMap;
                    if (rootMap != null)
                    {
                        _securityManager.RestoreRootMapInheritanceAsync(rootMap, RestoreRootMapInheritanceCompleted,
                            rootMap.ParentId, rootMap.Id);
                    }
                });
        }

        private void BreakRootMapInheritanceCompleted(object sender, BreakRootMapInheritanceCompletedEventArgs e)
        {
            var rootMap = sender as RootMap;
            if (rootMap != null)
            {
                rootMap.IsInherited = false;
                foreach (var groupPair in rootMap.PermissionGroups)
                {
                    foreach (var group in groupPair)
                    {
                        group.IsEnabled = true;
                    }
                }
                PermissionDetails.ApplyEnabled = false;
            }
        }

        private void RestoreRootMapInheritanceCompleted(object sender, RestoreRootMapInheritanceCompletedEventArgs e)
        {
            var rootMap = sender as RootMap;
            if (rootMap != null)
            {
                rootMap.IsInherited = true;
                var project = _projects.FirstOrDefault(q => q.Id == rootMap.ParentId);
                if (project != null)
                {
                    foreach (var groupPair in rootMap.PermissionGroups)
                    {
                        foreach (var group in groupPair)
                        {
                            group.IsEnabled = false;

                            var value = project.GetValue(group.Group);
                            group.IsSelected = value;
                            group.ResetValue(value);
                        }
                    }

                    var selectedItem = XTreeView.SelectedItem;
                    if (selectedItem != null)
                    {
                        PermissionDetails.DataContext = null;
                        var binding = new Binding
                        {
                            Source = XTreeView,
                            Path = new PropertyPath("SelectedItem"),
                            Mode = BindingMode.TwoWay,
                        };
                        PermissionDetails.SetBinding(DataContextProperty, binding);
                        PermissionDetails.Visibility = Visibility.Visible;
                        XTreeView.SelectedItem = selectedItem;
                    }

                    Updates.Clear();
                    PermissionDetails.ApplyEnabled = false;
                }
            }
        }

        private void ProjectListViewControlPanel_OnDeleteClicked(object sender, RoutedEventArgs e)
        {
            var item = sender as IManagementConsoleObject;
            if (item != null)
            {
                Delete(item);
            }
        }

        private void ProjectListViewControlPanel_OnNewMapClicked(object sender, RoutedEventArgs e)
        {
            var item = sender as IManagementConsoleObject;
            if (item != null)
            {
                NewMap(item);
            }
        }

        private void ProjectListViewControlPanel_OnNewProjectClicked(object sender, RoutedEventArgs e)
        {
            NewProject();
        }

        private void Delete(IManagementConsoleObject item)
        {
            var project = item as Project;
            if (project != null)
            {
                SuperMessageBoxService.ShowWarning("Delete Project",
                    "You will lose all maps within the project. Are you sure you wish to continue?", "Yes", "No",
                    () =>
                    {
                        Guid domainId = project.ParentId;
                        _projects.Remove(project);
                        ProjectListViewControlPanel.DataContext = null;

                        MapManager.DeleteDomainCompleted.RegisterEvent(domainId, OnDeletionCompleted);
                        ((MainPage) Application.Current.RootVisual).Loader.Visibility = Visibility.Visible;
                        ((MainPage)Application.Current.RootVisual).Progress.Visibility = Visibility.Collapsed;
                        ((MainPage)Application.Current.RootVisual).LoaderStatusText.Visibility = Visibility.Visible;
                        ((MainPage)Application.Current.RootVisual).StatusText.Text = "We're working on deleting your project...";
                        MapManager.DeleteDomain(domainId);
                    });
            }
            else
            {
                var map = item as RootMap;
                if (map != null)
                {
                    SuperMessageBoxService.ShowWarning("Delete Map",
                        "You will lose any content within the map. Are you sure you wish to continue?", "Yes", "No",
                        () =>
                        {
                            Guid domainId = Guid.Empty;
                            Guid rootMapId = Guid.Empty;

                            var parentProject = _projects.FirstOrDefault(q => q.Id == map.ParentId);

                            if (parentProject != null)
                            {
                                if (map.Node != null)
                                {
                                    domainId = map.Node.DomainId;
                                    rootMapId = map.Node.Id;
                                }

                                parentProject.RootMaps.Remove(map);
                            }

                            ProjectListViewControlPanel.DataContext = null;

                            if (domainId != Guid.Empty && rootMapId != Guid.Empty)
                            {
                                MapManager.DeleteRootMapCompleted.RegisterEvent(rootMapId, OnDeletionCompleted);
                                ((MainPage)Application.Current.RootVisual).Loader.Visibility = Visibility.Visible;
                                ((MainPage)Application.Current.RootVisual).Progress.Visibility = Visibility.Collapsed;
                                ((MainPage)Application.Current.RootVisual).LoaderStatusText.Visibility = Visibility.Visible;
                                ((MainPage)Application.Current.RootVisual).StatusText.Text = "We're working on deleting your map...";
                                MapManager.DeleteRootMap(domainId, rootMapId);
                            }
                        });
                }
            }
        }

        private void OnDeletionCompleted(object sender, EventRegisterEventArgs eventArgs)
        {
            ((MainPage)Application.Current.RootVisual).Loader.Visibility = Visibility.Collapsed;
        }

        private void NewMap(IManagementConsoleObject item)
        {
            var project = item as Project;
            if (project != null)
            {
                CreateNewMap(project);
            }
            else
            {
                var map = item as RootMap;
                if (map != null)
                {
                    var parentProject = _projects.FirstOrDefault(q => q.Id == map.ParentId);
                    if (parentProject != null)
                    {
                        CreateNewMap(parentProject);
                    }
                }
            }
        }

        private void CreateNewMap(IManagementConsoleObject project)
        {
            var mapNameDialog = new NewMapDialog();
            mapNameDialog.Closed += delegate
            {
                if (mapNameDialog.DialogResult != null && mapNameDialog.DialogResult.Value)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        if (!string.IsNullOrWhiteSpace(mapNameDialog.MapName)) //extra check that the map name isn't blank
                        {
                            MapManager.CreateRootMapCompleted.RegisterEvent(OnMapNodeCreated);
                            MapManager.CreateRootMap(project.Id, mapNameDialog.MapName,
                                MapManager.NodeTypes["CompendiumMapNode"], string.Empty);
                        }
                    });
                }
            };
            mapNameDialog.Show();
        }

        private void OnMapNodeCreated(object sender, TransactionalNodeService.Proxy.NodesEventArgs e)
        {
            if (e.Nodes != null && e.Nodes.Count > 0)
            {
                var map = e.Nodes.Values.FirstOrDefault();
                if (map != null && Equals(map.NodeType, MapManager.NodeTypes["CompendiumMapNode"]))
                {
                    var project = _projects.FirstOrDefault(q => q.Id == map.DomainId);
                    if (project != null)
                    {
                        var newRootMap = new RootMap(map, SecurityGroups, project.Name);
                        newRootMap.IsNewMap = true;
                        newRootMap.MetadataChanged += OnMetadataChanged;
                        newRootMap.PermissionChanged += OnPermissionChanged;
                        project.RootMaps = InsertSortedRootMap(newRootMap, project.RootMaps);
                        if (XTreeView.SelectedContainer != null && XTreeView.SelectedContainer.Item == project)
                        {
                            XTreeView.SelectedContainer.ItemsSource = null;
                            XTreeView.SelectedContainer.ItemsSource = project.RootMaps;
                        }
                    }
                }
            }
        }

        private ObservableCollection<RootMap> InsertSortedRootMap(RootMap item,
            ObservableCollection<RootMap> originalList)
        {
            ObservableCollection<RootMap> sortedCollection = new ObservableCollection<RootMap>();
            originalList.Add(item);
            foreach (RootMap rootMap in originalList.OrderBy(rm => rm.DisplayName))
            {
                sortedCollection.Add(rootMap);
            }
            return sortedCollection;
        }

        private ObservableCollection<Project> InsertSortedProject(Project item,
            ObservableCollection<Project> originalList)
        {
            ObservableCollection<Project> sortedCollection = new ObservableCollection<Project>();
            originalList.Add(item);
            foreach (Project project in originalList.OrderBy(p => p.DisplayName))
            {
                sortedCollection.Add(project);
            }
            return sortedCollection;
        }

        private void NewProject()
        {
            var domainNameDialog = new NewDomainDialog();
            domainNameDialog.Closed += delegate
            {
                if (domainNameDialog.DialogResult != null && domainNameDialog.DialogResult.Value)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        MapManager.CreateDomainCompleted.RegisterEvent(OnCreateNewDomainCompleted);
                        MapManager.CreateDomain(domainNameDialog.DomainName);
                    });
                }
            };
            domainNameDialog.Show();
        }

        private void OnCreateNewDomainCompleted(object sender, DomainEventArgs e)
        {
            _securityManager.SetProjectManagerGroupAssociationsAsync("New Domain", NewDomainPermissionAssisgned,
                new GlymaSecurableObject
                {
                    SecurableParentUid = Guid.Empty,
                    SecurableObjectUid = e.Domain
                });
        }

        private void NewDomainPermissionAssisgned(object sender, SetProjectManagerGroupAssociationsCompletedEventArgs e)
        {
            MapManager.QueryDomainsCompleted.RegisterEvent(OnAcquireNewDomainsCompleted);
            MapManager.QueryDomainsAsync();
        }

        private void OnNewDomainSecurityAssociationCreated(object sender,
            SetProjectManagerGroupAssociationsCompletedEventArgs e)
        {
            var node = sender as INode;
            if (node != null)
            {
                var project = new Project(node, SecurityGroups);
                project.PermissionChanged += OnPermissionChanged;
                project.MetadataChanged += OnMetadataChanged;
                _projects = InsertSortedProject(project, _projects);
                XTreeView.ItemsSource = null;
                XTreeView.ItemsSource = _projects;
            }
        }

        private void OnAcquireNewDomainsCompleted(object sender, TransactionalNodeService.Proxy.NodesEventArgs e)
        {
            foreach (var node in e.Nodes.Values)
            {
                if (node != null && _projects.All(q => q.Id != node.DomainId))
                {
                    _securityManager.SetProjectManagerGroupAssociationsAsync(node, OnNewDomainSecurityAssociationCreated,
                        new GlymaSecurableObject
                        {
                            SecurableParentUid = Guid.Empty,
                            SecurableObjectUid = node.DomainId
                        });


                }
            }
        }

        private void XTreeView_PreviewDragStarted(object sender, RadTreeViewDragEventArgs e)
        {
            //todo: remove following two lines if drag function is completed
            e.Handled = true;
            return;
            if (e.DraggedItems != null && e.DraggedItems.Count > 0)
            {
                if (e.DraggedItems[0] is Project)
                {
                    e.Handled = true;
                }
                else
                {
                    var map = e.DraggedItems[0] as RootMap;
                    if (map != null)
                    {
                        if (!map.IsSelectable)
                        {
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        private void XTreeView_PreviewDragEnded(object sender, RadTreeViewDragEndedEventArgs e)
        {
            if (e.DraggedItems != null && e.DraggedItems.Count > 0)
            {
                var rootMap = e.DraggedItems[0] as RootMap;
                if (rootMap != null)
                {
                    var oldParent = _projects.FirstOrDefault(q => q.Id == rootMap.ParentId);
                    Project parent = null;
                    RootMap target = null;
                    if (e.DropPosition == DropPosition.Inside)
                    {
                        parent = e.TargetDropItem.DataContext as Project;
                    }
                    else if (e.DropPosition == DropPosition.Before || e.DropPosition == DropPosition.After)
                    {
                        target = e.TargetDropItem.DataContext as RootMap;
                        if (target != null)
                        {
                            parent = _projects.FirstOrDefault(q => q.Id == target.ParentId);
                        }
                    }

                    if (parent != null && oldParent != null && parent != oldParent)
                    {
                        SuperMessageBoxService.Show("Confirmation",
                            "Which operation do you want to do?\r\n\r\nYou can move the map to the new project\r\nor copy the map to the new project",
                            "Move", "Copy", "Cancel", MessageBoxType.Confirmation,
                            () =>
                            {
                                oldParent.RootMaps.Remove(rootMap);
                                if (rootMap.IsInherited)
                                {
                                    foreach (var groups in parent.PermissionGroups)
                                    {
                                        foreach (var group in groups)
                                        {
                                            rootMap.LoadValue(group.Group,
                                                group.DefaultValue.HasValue && group.DefaultValue.Value);
                                        }
                                    }
                                }

                                rootMap.Node.DomainId = parent.Id;
                                if (e.DropPosition == DropPosition.Inside)
                                {
                                    parent.RootMaps.Add(rootMap);
                                }
                                else if (e.DropPosition == DropPosition.Before)
                                {
                                    var index = parent.RootMaps.IndexOf(target);
                                    parent.RootMaps.Insert(index, rootMap);
                                }
                                else if (e.DropPosition == DropPosition.After)
                                {
                                    var index = parent.RootMaps.IndexOf(target);
                                    parent.RootMaps.Insert(index + 1, rootMap);
                                }
                            },
                            () =>
                            {
                                MapManager.CreateRootMapCompleted.RegisterEvent(OnCopyRootMapCompleted);
                                MapManager.CreateRootMap(parent.Id, "Copy of " + rootMap.Name,
                                    MapManager.NodeTypes["CompendiumMapNode"], string.Empty);
                            });
                    }

                }
            }
            e.Handled = true;
        }

        private void OnCopyRootMapCompleted(object sender, TransactionalNodeService.Proxy.NodesEventArgs e)
        {
            if (e.Nodes != null && e.Nodes.Count > 0)
            {
                var map = e.Nodes.Values.FirstOrDefault();
                if (map != null && Equals(map.NodeType, MapManager.NodeTypes["CompendiumMapNode"]))
                {
                    var project = _projects.FirstOrDefault(q => q.Id == map.DomainId);
                    if (project != null)
                    {
                        project.RootMaps.Add(new RootMap(map, SecurityGroups, project.Name, false));
                    }
                }
            }
        }

        private void XTreeView_DragOver(object sender, DragEventArgs e)
        {

        }

        private void LoadMapButton_Click(object sender, RoutedEventArgs e)
        {
            if (MetadataUpdates.Count > 0)
            {
                SuperMessageBoxService.ShowWarning("Confirmation",
                    "Do you want to apply your changes? \r\n\r\n" +
                    "Press YES to apply\r\n" +
                    "Press No to discard the changes you have made\r\n",
                    "Yes", "No", "Cancel",
                    () =>
                    {
                        ApplyChanges();
                        if (LoadMapClicked != null)
                        {
                            LoadMapClicked(sender, e);
                        }
                    }, 
                    () => {
                        if (LoadMapClicked != null)
                        {
                            LoadMapClicked(sender, e);
                        }
                    });
            }
            else
            {
                if (LoadMapClicked != null)
                {
                    LoadMapClicked(sender, e);
                }
            }
        }
    }
}
