using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using Glyma.Debug;
using SilverlightMappingToolBasic.CompendiumMapProcessor;
using SilverlightMappingToolBasic.GlymaSecurityService;
using SilverlightMappingToolBasic.UI;
using SilverlightMappingToolBasic.UI.Extensions.CookieManagement;
using SilverlightMappingToolBasic.UI.Extensions.Security;
using SilverlightMappingToolBasic.UI.SuperGraph.Controller;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.DomainAndMapSelection;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.UI;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using Proxy = TransactionalNodeService.Proxy;
using SoapProxy = TransactionalNodeService.Soap;
using Service = TransactionalNodeService.Service;
using UtilityProxy = Glyma.UtilityService.Proxy;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using SimpleIoC;
using SilverlightMappingToolBasic.UI.SuperGraph.View.BreadcrumbSupportClasses;
using System.Windows.Browser;
using System.Net.NetworkInformation;
using System.Security;
using TransactionalNodeService.Soap;

namespace SilverlightMappingToolBasic
{
    public partial class MainPage : UserControl, ISuperGraphControllerContainer
    {
        private IIoCContainer _ioc;
        private SuperGraphController _superGraphController;
        private readonly Proxy.IMapManager _mapManager;
        private readonly ThemeManager _themeManager;
        private UI.View.MapPreloader _preLoader;
        private SecurityManager _securityManager;
        private bool _isOnline = false;

        public bool IsInitialised { get; private set; }
        

        public SuperGraphController SuperGraphController
        {
            get
            {
                return _superGraphController;
            }
        }

        public SecurityManager SecurityManager
        {
            get
            {
                if (_securityManager == null)
                {
                    _securityManager = new SecurityManager();
                }
                return _securityManager;
            }
        }

        public bool IsLoadMapByGuid { get; set; }

        public Guid NodeId { get; set; }
        public Guid MapId { get; set; }
        public Guid DomainId { get; set; }

        public IIoCContainer IoC
        {
            get
            {
                if (_ioc == null)
                {
                    _ioc = IoCContainer.GetInjectionInstance();
                }

                return _ioc;
            }
        }

        private string VideoSource
        {
            get;
            set;
        }

        private UI.View.IMapPreloader PreLoader
        {
            get
            {
                if (_preLoader == null)
                {
                    _preLoader = new UI.View.MapPreloader(_mapManager, this, Breadcrumbs);
                    _preLoader.LoadingStarted += PreLoaderOnLoadingStarted;
                    _preLoader.MapLoadFailed += PreLoaderOnMapLoadFailed;
                }

                return _preLoader;
            }
        }

        


        public MainPage()
        {
            DebugLogger.Instance.LogMsg("Initialising Glyma...   Url: " + HtmlPage.Document.DocumentUri);
            InitializeComponent();
            SetupNetworkChange(); //logs if the network is going up/down
            Sidebar.Handler = SuperGraph;
            ZommingControl.Handler = SuperGraph;
            Breadcrumbs.Handler = SuperGraph;
            SuperGraph.Ref = this;
            if (!App.IsDesignTime)
            {
                Loader.Visibility = Visibility.Visible;
                Breadcrumbs.BreadcrumbChanged += OnBreadcrumbChanged;
                Breadcrumbs.BreadcrumbClicked += OnBreadcrumbClicked;
                SuperGraph.DataContext = new SuperGraphProperties
                {
                    NodeTextWidth = GlymaParameters.NodeTextWidth,
                    NodeImageWidth = GlymaParameters.NodeImageWidth,
                    NodeImageHeight = GlymaParameters.NodeImageHeight
                };

                SoapEndPointFactory soapEndPointFactory = new SoapEndPointFactory(new Uri(App.Params.TransactionalMappingToolSvcUrl));
                var soapMapManager = new SoapProxy.SoapMapManager(soapEndPointFactory);

                _mapManager = soapMapManager;
                _mapManager.InitialiseMapManagerCompleted += OnInitialiseMapManagerCompleted;
                
                _mapManager.MapManagerActivityStatusUpdated += OnMapManagerActivityStatusUpdated;
                IoCContainer.GetInjectionInstance().RegisterComponent<Proxy.IMapManager, SoapProxy.SoapMapManager>(soapMapManager);

                var binding = new System.ServiceModel.BasicHttpBinding();
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;
                binding.MaxReceivedMessageSize = 2147483647;

                var utilityServiceAddress = new System.ServiceModel.EndpointAddress(new Uri(App.Params.GlymaUtilitySvcUrl));
                var utilityManagerServiceClient = new UtilityProxy.Service.UtilityServiceManagerClient(binding, utilityServiceAddress);
                var exportServiceManager = new UtilityProxy.ExportServiceManager(utilityManagerServiceClient);
                IoCContainer.GetInjectionInstance().RegisterComponent<UtilityProxy.IExportServiceManager, UtilityProxy.ExportServiceManager>(exportServiceManager);
                exportServiceManager.IsExportingAvailableCompleted.RegisterEvent(IsExportingAvailableCompleted);

                _themeManager = new ThemeManager();

                SuperGraph.NodeClicked += NodeClicked;
                SuperGraph.FilesDropped += FilesDropped;

                var jsBridge = new JavaScriptBridge(SuperGraph);
                HtmlPage.RegisterScriptableObject("glymaMapCanvas", jsBridge);

                exportServiceManager.IsExportingAvailableAsync();
            }
        }

        private void SetupNetworkChange()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                _isOnline = true;
            }
            else
            {
                _isOnline = false;
            }

            //We can detect when the network interface goes online/offline/changes address.
            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(OnNetworkAddressChanged) ;
        }

        private void OnNetworkAddressChanged(object sender, EventArgs e)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (!_isOnline)
                {
                    _isOnline = true;
                    DebugLogger.Instance.LogMsg("The network interface detected that it came online.");
                }
            }
            else
            {
                if (_isOnline)
                {
                    _isOnline = false;
                    DebugLogger.Instance.LogMsg("The network interface detected that it went offline.");
                }
            }
        }

        private void IsExportingAvailableCompleted(object sender, UtilityProxy.ResultEventArgs<bool> e)
        {
            if (!e.HasError)
            {
                App.IsExportEnabled = e.Result;
            }
            else
            {
                SuperMessageBoxService.ShowError("Error Occurred", "Failed to access export service");
            }

            _mapManager.InitialiseMapManagerAsync();
        }


        private void PreLoaderOnLoadingStarted(object sender, EventArgs eventArgs)
        {
            ShowLoading();
        }

        private void PreLoaderOnMapLoadFailed(object sender, EventArgs eventArgs)
        {
            Loader.Visibility = Visibility.Collapsed;
            SuperMessageBoxService.ShowWarning("We cant find your map","Please contact your system administrator for assistance",
                () => HomeScreen());
        }

        private void OnMapManagerActivityStatusUpdated(object sender, Proxy.MapManagerActivityEventArgs e)
        {
            switch (e.Status)
            {
                case Proxy.ActivityStatusEnum.Busy:
                    StatusBar.InProgressActionCount = e.TransactionsLeft;
                    break;
                case Proxy.ActivityStatusEnum.Idle:
                    StatusBar.InProgressActionCount = 0;
                    break;
            }
        }

        private void FilesDropped(object sender, FilesDroppedEventArgs e)
        {
            FileInfo[] files = e.DroppedFiles;

            if (files != null)
            {
                ImportFiles(files);
            }
        }

        public void ImportFiles(FileInfo[] files)
        {
            ShowLoading(true);
            var location = new Point((-SuperGraph.MoveGraphTransform.X + SuperGraph.ActualWidth / 2),
                (-SuperGraph.MoveGraphTransform.Y + SuperGraph.ActualHeight / 4));
            var processor = new SuperFileProcessor(files, _superGraphController.MapManager, _superGraphController.Context.Proxy, location);
            processor.ProgressChanged += OnProgressChanged;
            processor.ProgressCompleted += OnProgressCompleted;
            processor.ProcessFiles();
        }

        private void ShowLoading(bool isProgressVisible = false)
        {
            LoaderStatusText.Visibility = Visibility.Visible;
            Progress.Visibility = isProgressVisible ? Visibility.Visible : Visibility.Collapsed;
            if (!isProgressVisible)
            {
                StatusText.Text = "We're working on loading your map...";
            }
            else
            {
                StatusText.Text = "We're working on importing your map...";
                Progress.Text = "0%";
            }
            Loader.Visibility = Visibility.Visible;
        }

        private void OnProgressCompleted(object sender, FileProcessorCompletedEventArgs e)
        {
            Loader.Visibility = Visibility.Collapsed;
            if (e.NeedRefresh)
            {
                Refresh(Breadcrumbs.CurrentBreadcrumbControl);
            }
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null && e.UserState.ToString().Length > 0)
            {
                StatusText.Text = e.UserState.ToString();
            }
            Progress.Text = string.Format("{0}%", e.ProgressPercentage);
        }

        private void OnBreadcrumbChanged(object sender, BreadcrumbChangedEventArgs e)
        {
            var breadcrumb = sender as BreadcrumbControl;
            if (breadcrumb != null)
            {
                SuperGraph.ClearMapObjects();
                ShowLoading();
                _superGraphController.ChangeContextAsync(e.NewItem.Node);
                PreLoader.LoadParentBreadcrumb(e.NewItem.Node.Proxy, e.ParentIndex);
            }
            
        }

        private void OnBreadcrumbClicked(object sender, BreadcrumbClickedEventArgs e)
        {
            var breadcrumbControl = sender as BreadcrumbControl;

            if (breadcrumbControl != null)
            {
                SuperGraph.ClearMapObjects();
                ShowLoading();
                _superGraphController.ChangeContextAsync(breadcrumbControl.BreadcrumbData.Node);
                IsLoadMapByGuid = false;
                if (e.IsParentRemoved)
                {
                    PreLoader.LoadParentBreadcrumb(breadcrumbControl.BreadcrumbData.Node.Proxy, e.ParentIndex);
                }
            }
        }

        public void Refresh(BreadcrumbControl control = null)
        {
            if (control == null)
            {
                control = Breadcrumbs.CurrentBreadcrumbControl;
            }
            if (control != null)
            {
                SuperGraph.ClearMapObjects(false);
                ShowLoading();
                _superGraphController.ChangeContextAsync(control.BreadcrumbData.Node);
            }
        }

        private void NodeClicked(object sender, NodeClickedEventArgs e)
        {
            var nodeControl = sender as NodeControl;

            if (_superGraphController != null && nodeControl != null)
            {
                var node = nodeControl.DataContext as Node;
                if (node != null)
                {
                    if (node.MapObjectType == _mapManager.NodeTypes["CompendiumMapNode"].Id)
                    {
                        var facadeNode = node.Proxy as FacadeNode;
                        if (facadeNode != null)
                        {
                            if (!facadeNode.IsConcrete)
                            {
                                return; //if the node isn't concrete we won't be able to navigate down into the submap (ie it was just added and the transactions are outstanding)
                            }
                        }
                        IoC.GetInstance<ISuperGraphRelationshipFactory>().IsSameDomain = true;
                        SuperGraph.ClearMapObjects();
                        ShowLoading();

                        var breadcrumb = new BreadcrumbItem(node);
                        Breadcrumbs.BreadcrumbTrail.Add(breadcrumb);

                        _superGraphController.ChangeContextAsync(node);

                        //The line below forces the related content panels to reset and hide with each map navigation
                        //it's commented out to experiment what it's like to leave the content in place
                        //RelatedContentPanelUtil.Instance.ResetAndHidePanels();
                    }
                }
            }
        }

        public void HomeScreen(bool isInitialised = false)
        {
            // If there is no domain ID supplied then provide a dialog box to select one.
            var selectionDialog = new DomainSelectionDialog(SecurityManager)
            {
                HasCloseButton = isInitialised,
                MapManager = _mapManager,
            };

            if (isInitialised)
            {
                selectionDialog.Closed += delegate
                {
                    Dispatcher.BeginInvoke(() =>
                        OnDomainAndMapSelected(selectionDialog.Map,
                            selectionDialog.DialogResult != null && selectionDialog.DialogResult.Value, selectionDialog.IsAuthorMode)
                        );
                };
            }
            else
            {
                selectionDialog.Closed += delegate
                {
                    Dispatcher.BeginInvoke(() => OnDomainAndMapSelected(selectionDialog.Map, true, selectionDialog.IsAuthorMode));
                };
            }

            RelatedContentPanelUtil.Instance.ResetAndHidePanels();

            selectionDialog.Show();
        }

        private void OnInitialiseMapManagerCompleted(object sender, Proxy.InitialiseMapManagerEventArgs e)
        {
            if (!e.IsInitialised)
            {
                SuperMessageBoxService.Show("Incorrect Glyma Version", e.ErrorMessage, MessageBoxType.ErrorWithNoInput);
            }
            else
            {
                if (RelatedContentPanelUtil.Instance.IsJavascriptLibraryLoaded())
                {
                    SecurityManager.GetUsersPermissionLevelNameAsync("Initialisation", GetUsersPermissionLevelNameCompleted);
                }
                else
                {
                    SuperMessageBoxService.Show("Missing Dependencies", "Please contact your administrator as dependency files are missing from the system", MessageBoxType.ErrorWithNoInput);
                }
            }
        }

        

        private void InitialiseControllers()
        {
            _superGraphController = new SuperGraphController(_mapManager, SecurityManager, _themeManager, VideoSource);
            _superGraphController.ModelChanged += SuperGraph.ModelChanged;
            _superGraphController.MapLoadCompleted += MapLoadCompleted;
            _superGraphController.PermissionLoaded += SuperGraphControllerOnPermissionLoaded;
            IoC.RegisterComponent<ISuperGraphNodeFactory>(_superGraphController);
            IoC.RegisterComponent<ISuperGraphRelationshipFactory>(_superGraphController);
            IoC.RegisterComponent<ISuperGraphNodeBatchOperations>(_superGraphController);
            IoC.RegisterComponent<ISuperGraphNodeOperations>(_superGraphController);
            IoC.RegisterComponent(Breadcrumbs);
            IsInitialised = true;
        }

        private void SuperGraphControllerOnPermissionLoaded(object sender, UserPermissionEventArgs e)
        {
            SuperGraph.ExplorerOnly = e.Permission < PermissionLevel.Author;
        }

        private void MapLoaded()
        {
            if (!IoC.GetInstance<ISuperGraphRelationshipFactory>().IsSameDomain)
            {
                //RelatedContentPanelUtil.Instance.ShowActivityFeeds();
                LoadRootMapProperties();
            }
        }

        private void LoadRootMapProperties()
        {
            RootMapProperties.Instance.DomainId = SuperGraph.Ref.SuperGraphController.Context.DomainId;
            if (SuperGraph.Ref.SuperGraphController.Context.Proxy.RootMapId != null)
            {
                RootMapProperties.Instance.Id = SuperGraph.Ref.SuperGraphController.Context.Proxy.RootMapId.Value;
                _mapManager.QueryMapByIdCompleted.RegisterEvent(RootMapProperties.Instance.Id, LoadRootMapPropertiesCompleted, false);
                _mapManager.QueryMapByIdAsync(RootMapProperties.Instance.DomainId, RootMapProperties.Instance.Id, 1);
            }
        }

        private void LoadRootMapPropertiesCompleted(object sender, Proxy.NodesEventArgs eventArgs)
        {
            var contextGuid = (Guid)eventArgs.Context;
            IDictionary<string, string> rootNodeMetadata = new Dictionary<string, string>();
            if (eventArgs.Nodes != null && contextGuid != null)
            {
                foreach (var node in eventArgs.Nodes.Values)
                {
                    if (node.Id == contextGuid)
                    {
                        var metadataSet = node.Metadata;
                        foreach (var metadata in metadataSet)
                        {
                            rootNodeMetadata[metadata.Name] = metadata.Value;
                        }
                    }
                }
            }
            RootMapProperties.Instance.Metadata = rootNodeMetadata;

            RelatedContentPanelUtil.Instance.MapLoadedCallback(false);
        }

        private void MapLoadCompleted(object sender, EventArgs eventArgs)
        {
            var selector = SuperGraph as ISelectorControl;
            if (IsLoadMapByGuid)
            {
                selector.SelectNodeByGuid(NodeId);
                SuperGraph.RecheckIncorrectVisibility();
                SuperGraph.ReScanForCollapseStates();
            }
            else
            {
                SuperGraph.RecheckIncorrectVisibility();
                SuperGraph.ReScanForCollapseStates();
                
                Deployment.Current.Dispatcher.BeginInvoke(() => SuperGraph.LoadCookie());

                if (App.UserStyle == UserStyle.Reader && SuperGraph.AllowAutoRealign)
                {
                    //Force UI thread to realign
                    Deployment.Current.Dispatcher.BeginInvoke(() => SuperGraph.AutoRealign());
                }

                Deployment.Current.Dispatcher.BeginInvoke(() => SuperGraph.RecheckLocations());
            }

            if (!SuperGraph.MapInformation.IsMapMoved && selector.NodesSelector.NodeControls.Count == 0)
            {
                selector.CentraliseMostImportantParent();
            }

            SuperGraph.SetMouse();
            SuperGraph.Focus();
            Loader.Visibility = Visibility.Collapsed;
            IsLoadMapByGuid = false; //after successfully loading by id reset the state
            
            MapLoaded();
        }

        private void OnDomainAndMapSelected(Proxy.INode node, bool dialoagResult = true, bool isAuthorMode = false)
        {
            if (!IsInitialised)
            {
                InitialiseControllers();
            }
            LoadMap(node, dialoagResult, isAuthorMode);        
        }

        private void LoadMap(Proxy.INode node, bool dialoagResult, bool isAuthorMode)
        {
            if (dialoagResult)
            {
                DebugLogger.Instance.LogMsg(string.Format("Loading Map[{0}] of Domain[{1}]", node.Id, node.DomainId));
                IsLoadMapByGuid = false;
                SuperGraph.ClearMapObjects();
                Breadcrumbs.BreadcrumbTrail.Clear();
                ShowLoading();
                _mapManager.QueryMapByIdCompleted.RegisterEvent(node.Id, QueryMapByDomain);
                _mapManager.QueryMapByIdAsync(node.DomainId, node.Id, 1);
                IoC.GetInstance<ISuperGraphRelationshipFactory>().IsSameDomain = false;
                if (isAuthorMode)
                {
                    SuperGraph.AuthorMode();
                }

            }
            
        }

        internal void LoadMapById(Guid domainId, Guid mapNodeId, Guid nodeId)
        {
            IsLoadMapByGuid = true;
            PreLoader.Load(domainId, nodeId, mapNodeId);
        }

        private void QueryMapByDomain(object sender, Proxy.NodesEventArgs eventArgs)
        {
            var context = (Guid)eventArgs.Context;

            foreach (var node in eventArgs.Nodes.Values)
            {
                // The following line appears to be inherently incorrect. It relies on the fact that the first node will be the context.
                //if (node.NodeType == _mapManager.NodeTypes["CompendiumMapNode"])
                // The following line replaces the above line.
                if (node.Id == context)
                {
                    var viewModelNode = new Node(_mapManager);
                    viewModelNode.LoadNode(null, node);
                    viewModelNode.IsRootMap = true;
                    var breadcrumb = new BreadcrumbItem(viewModelNode);
                    Breadcrumbs.BreadcrumbTrail.Add(breadcrumb);

                    _superGraphController.ChangeContextAsync(node);
                    break;
                }
            }


        }

        /// <summary>
        /// The main page will pass through the DomainUid and NodeUid for the initial map to the map control within it
        /// </summary>
        /// <param name="mapLoadParamsManager">either the values from the Silverlight initparams, query string or cookie</param>
        public MainPage(MapLoadParamsManager mapLoadParamsManager)
            : this()
        {
            DomainId = mapLoadParamsManager.DomainUid;
            MapId = mapLoadParamsManager.MapUid;
            NodeId = mapLoadParamsManager.NodeUid;
            VideoSource = mapLoadParamsManager.VideoSource;
            IsLoadMapByGuid = mapLoadParamsManager.IsLoadMapByGuid;
        }

        private void GetUsersPermissionLevelNameCompleted(object sender, GetUsersPermissionLevelNameCompletedEventArgs e)
        {
            if (!e.Result.HasError)
            {
                var permission = PermissionLevel.Convert(e.Result.Result);
                App.PermissionLevel = permission;
                if (permission != PermissionLevel.None)
                {
                    SuperGraph.ExplorerOnly = permission < PermissionLevel.Author;
                    DebugLogger.Instance.LogMsg("User Permission Loaded: " + permission);
                    if (IsLoadMapByGuid)
                    {
                        DebugLogger.Instance.LogMsg(string.Format("Loading map through URL: NodeId[{0}],MapId[{1}],DomainId[{2}]", NodeId, MapId, DomainId));
                        PreLoader.ReadyToInitialiseControllers += OnReadyToInitialiseControllers;
                        PreLoader.Load(DomainId, NodeId, MapId);
                    }
                    else
                    {
                        Loader.Visibility = Visibility.Collapsed;
                        HomeScreen();
                    }
                }
                else
                {
                    SuperMessageBoxService.Show("Access Denied", "It seems that you don't have permission to access Glyma, please contact your system administrator for assistance.", MessageBoxType.ErrorWithNoInput);
                }
            }
            else
            {
                SuperMessageBoxService.ShowError("Error Occurred", "There was a problem reading Glyma permissions, please try again later");
            }
        }

        void OnPermissionsQueryCompleted(object sender, UserPermissionEventArgs e)
        {
            
        }

        private void OnReadyToInitialiseControllers(object sender, EventArgs e)
        {
            InitialiseControllers();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void UserControl_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D:
                    e.Handled = true;
                    if (((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) && ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt))
                    {
                        if (DebugPanel.Visibility == Visibility.Collapsed)
                        {
                            DebugPanelRow.Height = new GridLength(33);
                            DebugPanel.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            DebugPanelRow.Height = new GridLength(0);
                            DebugPanel.Visibility = Visibility.Collapsed;
                        }
                    }
                    break;
            }
        }

        private void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchTerm = SearchBox.Text.Trim();
            if (searchTerm.Length > 0)
            {
                var nodes = SuperGraph.SearchNodeControlsByName(searchTerm);
                if (nodes.Any())
                {
                    SuperGraph.Selector.Clear();
                    ((ISelectorControl)SuperGraph).SelectNodeByGuid(nodes[0].ViewModelNode.Proxy.Id);
                    SearchBox.Focus();
                }
            }
        }

        private void StatusBar_ForcePushClicked(object sender, EventArgs e)
        {
            _mapManager.ForceTransactionReExecution();
        }
    }
}
