using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;
using System.Windows.Markup;
using System.Windows.Messaging;

using System.Threading;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;

using SilverlightMappingToolBasic.MappingService;
using SilverlightMappingToolBasic.Controls;
using SilverlightMappingToolBasic;
using SilverlightMappingToolBasic.UI.Extensions.Security;
using SilverlightMappingToolBasic.UI.SuperGraph.View.DomainAndMapSelection;
using VideoPlayerSharedLib;
using System.Runtime.Serialization;
using Proxy = TransactionalNodeService.Proxy;
using Soap = TransactionalNodeService.Soap;
using Service = TransactionalNodeService.Service;
using TransactionalNodeService.Proxy;
using TransactionalNodeService.Soap;


namespace SilverlightMappingToolBasic.MapDepth
{
    public partial class CompendiumMapDepthMap : UserControl, IMapControl
    {
        //public TextBox _focusBox;
        public event EventHandler NavigatorInitialised;

        private TypeManager _typeManager;
        private MapDepthNavigator _navigator;
        private DatabaseMappingService _nodeService;
        private IMapManager _mapManager;

        private Point _currentMousePosition;
        private Point _startLinePosition;
        private bool _isRightMouseButtonDown;
        private bool _mouseMovedWhileRMBDown;
        private bool _isLeftMouseButtonDown;

        private Point _selectionStartPoint;
        private Line _tempRelationshipLine;
        private Rectangle _selectionRectangle;

        private Point _originalPanPosition;

        private Dictionary<Guid, INodeRenderer> _renderers = new Dictionary<Guid, INodeRenderer>();
        private Dictionary<Guid, IRelationshipRenderer> _relationshipRenderers = new Dictionary<Guid, IRelationshipRenderer>();

        // TODO: These should be removed and replaced with Guid versions.
        public string DomainUid
        {
            get;
            set;
        }

        // TODO: These should be removed and replaced with Guid versions.
        public string NodeUid
        {
            get;
            set;
        }

        public string VideoSource
        {
            get;
            set;
        }

        public string TransactionalMappingToolSvcUrl
        {
            get;
            set;
        }

        public string MappingToolSvcUrl
        {
            get;
            set;
        }

        public string ThemeSvcUrl
        {
            get;
            set;
        }

        public LocalMessageSender MessageSender
        {
            get;
            set;
        }

        public EventHandler<MessageReceivedEventArgs> MessageReceivedHandler
        {
            get;
            set;
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public INodeNavigator Navigator
        {
            get
            {
                return _navigator;
            }
        }

        public MapDepthViewManager ViewManager
        {
            get;
            protected set;
        }

        public Canvas MapSurface
        {
            get
            {
                return FindName("uxMapSurface") as Canvas;
            }
        }

        public INodeProxy[] SelectedNodes
        {
            get
            {
                List<INodeProxy> selectedNodes = new List<INodeProxy>();
                if (ViewManager != null)
                {
                    selectedNodes.AddRange(ViewManager.Nodes.Where(nodeRenderer => nodeRenderer.IsSelected == true).Select(node => node.Node));
                }
                return selectedNodes.ToArray();
            }
        }

        public IRelationshipProxy[] SelectedRelationships
        {
            get
            {
                List<IRelationshipProxy> selectedRelationships = new List<IRelationshipProxy>();
                if (ViewManager != null)
                {
                    selectedRelationships.AddRange(ViewManager.Relationships.Where(relaRenderer => relaRenderer.IsSelected == true).Select(relationship => relationship.Relationship));
                }
                return selectedRelationships.ToArray();
            }
        }

        public CompendiumMapDepthMap()
        {
            InitializeComponent();
            
            NodeRelationshipHelper nrh = new NodeRelationshipHelper();
            IoC.IoCContainer.GetInjectionInstance().RegisterComponent<NodeRelationshipHelper>(nrh);
            nrh.NodesConnected += new EventHandler(OnNodesConnected);

            this.MouseMove += new MouseEventHandler(CompendiumMapDepthMap_MouseMove);
            this.KeyDown += new KeyEventHandler(CompendiumMapDepthMap_KeyDown);
            this.MouseRightButtonDown += new MouseButtonEventHandler(CompendiumMapDepthMap_MouseRightButtonDown);
            this.MouseRightButtonUp += new MouseButtonEventHandler(CompendiumMapDepthMap_MouseRightButtonUp);
            this.MouseWheel += new MouseWheelEventHandler(CompendiumMapDepthMap_MouseWheel);
            this.MouseLeftButtonDown += new MouseButtonEventHandler(CompendiumMapDepthMap_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(CompendiumMapDepthMap_MouseLeftButtonUp);
            
            this.Loaded += new RoutedEventHandler(CompendiumMapDepthMap_Loaded);

            if (!IsInDesignMode)
            {
                // TODO: The following code has been superceded;
                #region Obsolete
                if (string.IsNullOrEmpty(MappingToolSvcUrl))
                {
                    _nodeService = new DatabaseMappingService();
                }
                else
                {
                    _nodeService = new DatabaseMappingService(MappingToolSvcUrl);
                }
                IoC.IoCContainer.GetInjectionInstance().RegisterComponent<INodeService>(_nodeService);

                _typeManager = new TypeManager(_nodeService);
                IoC.IoCContainer.GetInjectionInstance().RegisterComponent<TypeManager>(_typeManager);

                _typeManager.InitialiseNodeTypeManagerCompleted += new EventHandler(InitialiseNodeTypeManagerCompleted);
                _typeManager.InitialiseNodeTypeManager();
                #endregion

                // TODO: Need to change this to pull from the params in the HTML declaration itself.
                TransactionalMappingToolSvcUrl = "http://glyma-dev/_vti_bin/sevensigma/transactionalmappingtoolservice.svc";

                System.ServiceModel.BasicHttpBinding binding = new System.ServiceModel.BasicHttpBinding();
                binding.Security.Mode = System.ServiceModel.BasicHttpSecurityMode.TransportCredentialOnly;
                binding.MaxReceivedMessageSize = 2147483647;

                System.ServiceModel.EndpointAddress address = new System.ServiceModel.EndpointAddress(new Uri(TransactionalMappingToolSvcUrl));

                Service.TransactionalMappingToolServiceClient client = new Service.TransactionalMappingToolServiceClient(binding, address);

                //_mapManager = new SoapMapManager(client);

                IoC.IoCContainer.GetInjectionInstance().RegisterComponent<IMapManager>(_mapManager);

                _mapManager.InitialiseMapManagerCompleted += new EventHandler<InitialiseMapManagerEventArgs>(OnInitialiseMapManagerCompleted);
                _mapManager.InitialiseMapManagerAsync();
            }
        }

        private bool _nodeUidChecked = false;
        private bool _domainUidChecked = false;
        private Guid? _nodeId = null;
        private Guid? _domainId = null;

        private Guid? GetDomainId()
        {
            if (!_domainUidChecked)
            {
                if (string.IsNullOrEmpty(DomainUid))
                {
                    _domainId = null;
                }
                else
                {
                    Guid domainId;

                    if (Guid.TryParse(DomainUid, out domainId))
                    {
                        _domainId = domainId;
                    }
                    else
                    {
                        _domainId = null;
                    }
                }
            }

            return _domainId;
        }

        private Guid? GetNodeId()
        {
            if (!_nodeUidChecked)
            {
                if (string.IsNullOrEmpty(NodeUid))
                {
                    _nodeId = null;
                }
                else
                {
                    Guid nodeId;

                    if (Guid.TryParse(NodeUid, out nodeId))
                    {
                        _nodeId = nodeId;
                    }
                    else
                    {
                        _nodeId = null;
                    }
                }
            }

            return _nodeId;
        }

        private void SetNodeId(Guid id)
        {
            _nodeId = id;
        }

        private void OnInitialiseMapManagerCompleted(object sender, InitialiseMapManagerEventArgs e)
        {
            if (GetDomainId().HasValue)
            {
                //if the starting NodeUid is provided use that
                if (GetNodeId().HasValue)
                {
                    InitializeNavigator(GetDomainId().Value, GetNodeId().Value);
                }
                //if no starting NodeUid is provided find the domain node and use that
                else
                {
                    _mapManager.QueryMapByDomainCompleted.RegisterEvent(GetDomainId().Value, new EventHandler<Proxy.NodesEventArgs>(OnAcquireNodesByDomainCompleted));
                    _mapManager.QueryMapByDomainAsync(GetDomainId().Value);
                }
            }
            else
            {
                // If there is no domain ID supplied then provide a dialog box to select one.
                DomainSelectionDialog selectionDialog = new DomainSelectionDialog(new SecurityManager());
                // TODO: The following lines need to be modded so that the MapManager is passed through rather than the NodeService.
                selectionDialog.DataContext = _nodeService;
                selectionDialog.MapManager = _mapManager;
                selectionDialog.HasCloseButton = false;

                selectionDialog.Show();
            }
        }

        private void OnAcquireNodesByDomainCompleted(object sender, Proxy.NodesEventArgs e)
        {
            foreach (Proxy.Node node in e.Nodes.Values)
            {
                if (node.NodeType == _mapManager.NodeTypes["DomainNode"])
                {
                    // TODO: The following line doesn't actually need to be here.
                    NodeUid = node.Id.ToString();
                    SetNodeId(node.Id);
                    InitializeNavigator(GetDomainId().Value, GetNodeId().Value);
                }
            }
        }

        private void CompendiumMapDepthMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isLeftMouseButtonDown = true;
            _selectionStartPoint = translateTransform.Inverse.Transform(e.GetPosition(this));
            _selectionStartPoint = scaleTransform.Inverse.Transform(_selectionStartPoint);
            if (_selectionRectangle != null)
            {
                this.uxMapSurface.Children.Remove(_selectionRectangle);
                _selectionRectangle = null;
            }
            if (e.ClickCount != 2)
            {
                _selectionRectangle = new Rectangle();
                _selectionRectangle.Stroke = new SolidColorBrush(Colors.Black);
                _selectionRectangle.StrokeThickness = 1.0;
                _selectionRectangle.Opacity = 0.7;
                DoubleCollection dashArray = new DoubleCollection();
                dashArray.Add(2);
                dashArray.Add(4);
                _selectionRectangle.StrokeDashArray = dashArray;
                _selectionRectangle.SetValue(Canvas.LeftProperty, _selectionStartPoint.X);
                _selectionRectangle.SetValue(Canvas.TopProperty, _selectionStartPoint.Y);
                this.uxMapSurface.Children.Add(_selectionRectangle);
            }

            ViewManager.CommitNodeName();
        }

        private void CompendiumMapDepthMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isLeftMouseButtonDown = false;
            INodeRenderer nr = ViewManager.GetNodeRenderer(_currentMousePosition);
            NodeRelationshipHelper nrh = IoC.IoCContainer.GetInjectionInstance().GetInstance<NodeRelationshipHelper>();

            if (nrh != null)
            {
                if (nrh.IsEditting)
                {
                    if (nr != null)
                    {
                        IRelationshipProxy originalRelationship = nrh.Relationship.DataContext as IRelationshipProxy;
                        _nodeService.DeleteRelationship(Navigator.DomainId, originalRelationship.Id);

                        IDescriptorTypeProxy toDescriptorTypeProxy = _typeManager.GetDescriptorType("To");
                        IDescriptorTypeProxy fromDescriptorTypeProxy = _typeManager.GetDescriptorType("From");
                        IDescriptorTypeProxy transMapDescriptorTypeProxy = _typeManager.GetDescriptorType("TransclusionMap");

                        Dictionary<IDescriptorTypeProxy, Guid> nodes = new Dictionary<IDescriptorTypeProxy, Guid>();
                        INodeProxy fromNode = null, toNode = null;
                        
                        switch (nrh.EdittingSide)
                        {
                            case RelationshipSide.From:
                                fromNode = nr.Node;
                                toNode = originalRelationship.Descriptors.GetByDescriptorTypeName("To").First().Node;

                                break;
                            case RelationshipSide.To:
                                fromNode = originalRelationship.Descriptors.GetByDescriptorTypeName("From").First().Node;
                                toNode = nr.Node;

                                break;
                            default:
                                break;
                        }

                        nodes.Add(toDescriptorTypeProxy, toNode.Id);
                        nodes.Add(fromDescriptorTypeProxy, fromNode.Id);
                        IRelationshipTypeProxy relationshipTypeProxy = null;

                        if (fromNode.ParentMapNodeUid != this.Navigator.FocalNodeId || toNode.ParentMapNodeUid != this.Navigator.FocalNodeId)
                        {
                            nodes.Add(transMapDescriptorTypeProxy, this.Navigator.FocalNodeId);
                            relationshipTypeProxy = _typeManager.GetRelationshipType("TransclusionRelationship");
                        }
                        else
                        {
                            relationshipTypeProxy = _typeManager.GetRelationshipType("FromToRelationship");
                        }

                        _navigator.ConnectNodesAsync(nodes, relationshipTypeProxy, originalRelationship.Id.ToString());
                        _navigator.GetCurrentNodesAsync();
                    }
                    else
                    {
                        nrh.Relationship.UpdateArrow();
                    }
                }
                nrh.IsEditting = false;
            }

            if (_selectionRectangle != null)
            {
                this.uxMapSurface.Children.Remove(_selectionRectangle);
                double topLeftX = (double)_selectionRectangle.GetValue(Canvas.LeftProperty);
                double topLeftY = (double)_selectionRectangle.GetValue(Canvas.TopProperty);
                double bottomRightX = _selectionRectangle.Width + topLeftX;
                double bottomRightY = _selectionRectangle.Height + topLeftY;
                _selectionRectangle = null;
                if (!(Double.IsNaN(bottomRightX) && Double.IsNaN(bottomRightY)))
                {
                    ViewManager.SelectAllWithinBounds(new Point(topLeftX, topLeftY), new Point(bottomRightX, bottomRightY));
                }
                else
                {
                    ViewManager.UnselectAllNodes();
                    ViewManager.UnselectAllRelationships();
                }
            }
        }

        private void CompendiumMapDepthMap_Loaded(object sender, RoutedEventArgs e)
        {
            MessageReceivedHandler = new EventHandler<MessageReceivedEventArgs>(MessageReceiver_MessageReceived);
        }

        private void MessageReceiver_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (MessageReceived != null)
            {
                MessageReceived.Invoke(this, e);
            }

            Event receivedEvent = Utilities.Deserialize<Event>(e.Message);
            if (receivedEvent != null)
            {
                switch (receivedEvent.Name)
                {
                    case "GetSourceAndPositionCallback":
                        HandleGetSourceAndPositionCallback(receivedEvent);
                        break;
                    case "CurrentStateChanged":
                        HandledCurrentStateChanged(receivedEvent);
                        break;
                    case "GetPlayingStateCallback":
                        HandleGetPlayingStateCallback(receivedEvent);
                        break;
                    case "Seek":
                        HandleSeekEvent(receivedEvent);
                        break;
                    case "ReplayingLastNode":
                        HandleReplayingLastNodeEvent(receivedEvent);
                        break;
                    default:
                        //Debug.WriteLine("Unhandled message received");
                        break;
                }
            }
        }

        private void HandleGetSourceAndPositionCallback(Event callback)
        {
            if (callback.EventArgs != null && callback.ContainsEventArg("CallbackId")
                && callback.ContainsEventArg("Position") && callback.ContainsEventArg("Source")
                && callback.ContainsEventArg("NodeId"))
            {
                string callbackId = callback.GetEventArgValue("CallbackId");
                string source = callback.GetEventArgValue("Source");
                string nodeId = callback.GetEventArgValue("NodeId");
                switch (callbackId)
                {
                    case "StartPosition":
                        TimeSpan startPosition = TimeSpan.Parse(callback.GetEventArgValue("Position"));
                        SaveMediaInformation(nodeId, source, callbackId, startPosition);
                        break;
                    case "EndPosition":
                        TimeSpan endPosition = TimeSpan.Parse(callback.GetEventArgValue("Position"));
                        SaveMediaInformation(nodeId, source, callbackId, endPosition);
                        break;
                    default:
                        break;
                }
            }
        }

        private void HandleGetPlayingStateCallback(Event callback) 
        {
            if (callback.EventArgs != null && callback.ContainsEventArg("CallbackId")
                && callback.ContainsEventArg("State") && callback.ContainsEventArg("NodeId"))
            {
                string callbackId = callback.GetEventArgValue("CallbackId");
                string state = callback.GetEventArgValue("State");
                string currentlyPlayingNodeId = callback.GetEventArgValue("NodeId");

                if (ViewManager != null && ViewManager.CurrentView != null)
                {
                    if (callbackId == Navigator.FocalNodeId.ToString()) //the request was made for this map refresh
                    {
                        if (state == "Playing" || state == "Buffering")
                        {
                            foreach (NodeRenderer nodeRenderer in ViewManager.CurrentView.NodeRenderers.Values)
                            {
                                if (nodeRenderer.Node.Id.ToString() == currentlyPlayingNodeId)
                                {
                                    nodeRenderer.SetIsPlaying(true);
                                    nodeRenderer.ResetMediaIcon(true);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void HandledCurrentStateChanged(Event receivedEvent)
        {
            if (receivedEvent.EventArgs != null && receivedEvent.ContainsEventArg("State"))
            {
                string state = receivedEvent.GetEventArgValue("State");
                Guid nodeId = Guid.Empty;
                if (receivedEvent.ContainsEventArg("NodeId"))
                {
                    Guid.TryParse(receivedEvent.GetEventArgValue("NodeId"), out nodeId);
                }
                if (!string.IsNullOrEmpty(state) && state.ToLower() == "idle")
                {
                    ResetNodeRendererMediaIcon(nodeId, false);
                }
                else if (!string.IsNullOrEmpty(state) && state.ToLower() == "playing")
                {
                    ResetNodeRendererMediaIcon(nodeId, true);
                }
                else if (!string.IsNullOrEmpty(state) && state.ToLower() == "buffering")
                {
                    ResetNodeRendererMediaIcon(nodeId, true);
                }
            }
        }

        private void HandleSeekEvent(Event receivedEvent)
        {
            if (receivedEvent.EventArgs != null && receivedEvent.ContainsEventArg("NewPosition"))
            {
                if (ViewManager != null && ViewManager.CurrentView != null && ViewManager.CurrentView.NodeRenderers != null)
                {
                    TimeSpan newPosition = TimeSpan.MinValue;
                    if (TimeSpan.TryParse(receivedEvent.GetEventArgValue("NewPosition"), out newPosition))
                    {
                        foreach (NodeRenderer nodeRenderer in ViewManager.CurrentView.NodeRenderers.Values)
                        {
                            if (nodeRenderer.ResetMediaIcon(newPosition))
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void ResetNodeRendererMediaIcon(Guid nodeId, bool playing)
        {
            if (ViewManager != null && ViewManager.CurrentView != null && ViewManager.CurrentView.NodeRenderers != null && nodeId != Guid.Empty)
            {
                foreach (NodeRenderer nodeRenderer in ViewManager.CurrentView.NodeRenderers.Values)
                {
                    if (nodeRenderer.Node.Id == nodeId)
                    {
                        nodeRenderer.ResetMediaIcon(playing);
                    }
                    else
                    {
                        //all other nodes are not playing
                        nodeRenderer.ResetMediaIcon(false);
                    }
                }
            }
        }

        private void HandleReplayingLastNodeEvent(Event receivedEvent)
        {
            if (ViewManager != null && ViewManager.CurrentView != null && ViewManager.CurrentView.NodeRenderers != null
                && receivedEvent.EventArgs != null && receivedEvent.ContainsEventArg("NodeId"))
            {
                Guid nodeId = Guid.Empty;
                if (Guid.TryParse(receivedEvent.GetEventArgValue("NodeId"), out nodeId)) 
                {
                    NodeRenderer playingNode = ViewManager.CurrentView.NodeRenderers[nodeId];
                    {
                        playingNode.SetIsPlaying(true);
                        playingNode.ResetMediaIcon(true);
                    }
                }
            }
        }

        private void SaveMediaInformation(string nodeId, string source, string markerName, TimeSpan position)
        {
            var nodeRenderers = ViewManager.Nodes.Where(n => n.Node.Id.ToString() == nodeId);
            INodeRenderer nodeRenderer = nodeRenderers.First() as INodeRenderer;
            IMetadataTypeProxy timeSpanType = _typeManager.GetMetadataType("timespan");
            IMetadataTypeProxy stringType = _typeManager.GetMetadataType("string");
            Navigator.UpdateNodeMetadataAsync(nodeRenderer.Node, Guid.Empty, null, "Video." + markerName, position.ToString(), timeSpanType);
            Navigator.UpdateNodeMetadataAsync(nodeRenderer.Node, Guid.Empty, null, "Video.Source", source, stringType);
        }

        private void CompendiumMapDepthMap_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Zoom(e.GetPosition(this), e.Delta/600d);
        }

        // TODO: Need to remove the following.
        private void InitialiseNodeTypeManagerCompleted(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(DomainUid))
            //{
            //    Guid domainId = Guid.Empty;
            //    if (Guid.TryParse(DomainUid, out domainId))
            //    {
            //        //if the starting NodeUid is provided use that
            //        if (!string.IsNullOrEmpty(NodeUid))
            //        {
            //            Guid nodeId = Guid.Empty;
            //            if (Guid.TryParse(NodeUid, out nodeId))
            //            {
            //                InitializeNavigator(domainId, nodeId);
            //            }
            //        }

            //        //if no starting NodeUid is provided find the domain node and use that
            //        else
            //        {
            //            _nodeService.GetDomainNodeIdCompleted += new EventHandler<ReturnedNodeIdEventArgs>(OnGetDomainNodeIdCompleted);
            //            _nodeService.GetDomainNodeIdAsync(domainId);
            //        }
            //    }
            //}
            //else
            //{
            //    // If there is no domain ID supplied then provide a dialog box to select one.
            //    DomainSelectionDialog selectionDialog = new DomainSelectionDialog();
            //    selectionDialog.DataContext = _nodeService;
            //    selectionDialog.HasCloseButton = false;

            //    selectionDialog.Closed += delegate(object dialog, EventArgs dialogArgs)
            //    {
            //        this.Dispatcher.BeginInvoke(delegate() { InitializeNavigator(selectionDialog.DomainId, selectionDialog.NodeId); });
            //    };

            //    selectionDialog.Show();
            //}
        }

        private void OnGetDomainNodeIdCompleted(object sender, ReturnedNodeIdEventArgs e)
        {
            if (!string.IsNullOrEmpty(DomainUid) && e.NodeId != null)
            {
                Guid domainId = Guid.Empty;
                if (Guid.TryParse(DomainUid, out domainId))
                {
                    InitializeNavigator(domainId, e.NodeId);
                }
            }
        }

        private void InitializeNavigator(Guid domainId, Guid nodeId)
        {
            ThemeManager themeManagementObject = IoC.IoCContainer.GetInjectionInstance().GetInstance<ThemeManager>();
            themeManagementObject.ThemeLoaded += new EventHandler(themeManagementObject_ThemeLoaded);
            _navigator = new MapDepthNavigator(_nodeService, themeManagementObject, domainId);
            _nodeService.GetContainerMapNodeIdCompleted += new EventHandler<ReturnedNodeIdEventArgs>(nodeService_GetContainerMapNodeIdCompleted);
            _nodeService.GetContainerMapNodeId(domainId, nodeId);
        }

        private void nodeService_GetContainerMapNodeIdCompleted(object sender, ReturnedNodeIdEventArgs e)
        {
            ThemeManager themeManagementObject = IoC.IoCContainer.GetInjectionInstance().GetInstance<ThemeManager>();
            _navigator.SetCurrentNode(e.NodeId);

            ViewManager = new MapDepthViewManager(_navigator, themeManagementObject);
            ViewManager.NodesUpdated += new EventHandler(ViewManager_NodesUpdated);

            IoC.IoCContainer.GetInjectionInstance().RegisterComponent<MapDepthNavigator>(_navigator);
            if (NavigatorInitialised != null)
            {
                NavigatorInitialised.Invoke(this, new EventArgs());
            }

            themeManagementObject.LoadTheme(ThemeSvcUrl, "Generic");
        }

        /// <summary>
        /// The theme management object must have loaded the theme from the ThemeService before it can be used.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void themeManagementObject_ThemeLoaded(object sender, EventArgs e)
        {
            _navigator.GetCurrentNodesAsync();
        }

        private void ViewManager_NodesUpdated(object sender, EventArgs e)
        {
            MapDepthViewManager viewManager = sender as MapDepthViewManager;

            if (viewManager == null)
            {
                return;
            }

            RenderingContextInfo contextInfo = new RenderingContextInfo();

            contextInfo.SurfaceHeight = this.Height;
            contextInfo.SurfaceWidth = this.Width;
            contextInfo.SurfaceTopLeftX = 0;
            contextInfo.SurfaceTopLeftY = 0;

            viewManager.UpdateSurface(this, contextInfo);
            
            Command getPlayingStateCommand = new Command();
            getPlayingStateCommand.Name = "GetPlayingState";
            getPlayingStateCommand.Params = new List<Param>();
            getPlayingStateCommand.Params.Add(new Param() { Name = "CallbackId", Value = Navigator.FocalNodeId.ToString() });
            //Utilities.SendMessage(MessageSender, getPlayingStateCommand);
        }

        private void CompendiumMapDepthMap_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _currentMousePosition = e.GetPosition(this);
            _isRightMouseButtonDown = true;
            _mouseMovedWhileRMBDown = false;
            NodeRelationshipHelper nrh = IoC.IoCContainer.GetInjectionInstance().GetInstance<NodeRelationshipHelper>();
            if (nrh.FromNode != null)
            {
                _startLinePosition = nrh.GetCenterOfFromNode();
                _tempRelationshipLine = new Line();
                _tempRelationshipLine.StrokeThickness = 1.25;
                _tempRelationshipLine.Stroke = new SolidColorBrush(Colors.Black);
                _tempRelationshipLine.Opacity = 0.40;
                _tempRelationshipLine.X1 = _startLinePosition.X;
                _tempRelationshipLine.Y1 = _startLinePosition.Y;
                _tempRelationshipLine.X2 = _startLinePosition.X;
                _tempRelationshipLine.Y2 = _startLinePosition.Y;
                _tempRelationshipLine.SetValue(Canvas.ZIndexProperty, -1);
                this.uxMapSurface.Children.Add(_tempRelationshipLine);

                Thread rightClickDelayThread = new Thread(this.ShowNodeContextMenu);
                rightClickDelayThread.Start(nrh.FromNode.DataContext as INodeProxy);
            }
            else if (nrh.Relationship != null)
            {
                Thread rightCLickDelayThread = new Thread(this.ShowRelationshipContextMenu);
                rightCLickDelayThread.Start(nrh.Relationship.DataContext as IRelationshipProxy);
            }
            else
            {
                _originalPanPosition = _currentMousePosition;
                Thread rightClickDelayThread = new Thread(this.ShowCanvasContextMenu);
                rightClickDelayThread.Start();
            }
            e.Handled = true;
        }

        private void ShowNodeContextMenu(object nodeProxy)
        {
            Thread.Sleep(300);

            // put it on the UI thread to execute.
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (!_mouseMovedWhileRMBDown)
                {
                    NodeRelationshipHelper nrh = IoC.IoCContainer.GetInjectionInstance().GetInstance<NodeRelationshipHelper>();
                    if (nrh.FromNode != null)
                    {
                        nrh.FromNode = null;
                    }
                    if (this.uxMapSurface.Children.Contains(_tempRelationshipLine))
                    {
                        this.uxMapSurface.Children.Remove(_tempRelationshipLine);
                    }
                    NodeContextMenu contextMenu = new NodeContextMenu(nodeProxy as INodeProxy, this, _nodeService, _currentMousePosition);
                    ContextMenuService.SetContextMenu(this, contextMenu);

                    contextMenu.IsOpen = true;
                    contextMenu.HorizontalOffset = _currentMousePosition.X;
                    contextMenu.VerticalOffset = _currentMousePosition.Y + 30;
                }
            });
        }

        private void ShowRelationshipContextMenu(object relationshipProxy)
        {
            Thread.Sleep(300);

            // put it on the UI thread to execute.
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (!_mouseMovedWhileRMBDown)
                {
                    RelationshipContextMenu contextMenu = new RelationshipContextMenu(relationshipProxy as IRelationshipProxy, this, _nodeService);
                    ContextMenuService.SetContextMenu(this, contextMenu);

                    contextMenu.IsOpen = true;
                    contextMenu.HorizontalOffset = _currentMousePosition.X;
                    contextMenu.VerticalOffset = _currentMousePosition.Y + 30;
                }
            });
        }

        private void ShowCanvasContextMenu(object state)
        {
            Thread.Sleep(300);

            // put it on the UI thread to execute.
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (!_mouseMovedWhileRMBDown)
                {
                    CanvasContextMenu contextMenu = new CanvasContextMenu(_typeManager, MessageSender, Navigator, _currentMousePosition);
                    ContextMenuService.SetContextMenu(this, contextMenu);

                    contextMenu.IsOpen = true;
                    contextMenu.HorizontalOffset = _currentMousePosition.X;
                    contextMenu.VerticalOffset = _currentMousePosition.Y + 30;
                }
            });
        }

        private void CompendiumMapDepthMap_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isRightMouseButtonDown = false;
            _mouseMovedWhileRMBDown = false;
            NodeRelationshipHelper nrh = IoC.IoCContainer.GetInjectionInstance().GetInstance<NodeRelationshipHelper>();
            if (nrh.ToNode == null)
            {
                nrh.FromNode = null;
            }
            if (this.uxMapSurface.Children.Contains(_tempRelationshipLine))
            {
                this.uxMapSurface.Children.Remove(_tempRelationshipLine);
            }
        }

        internal void CompendiumMapDepthMap_KeyDown(object sender, KeyEventArgs e)
        {
            // TODO: This needs to be done better.
            if (e.OriginalSource is TextBox)
            {
                return;
            }

            INodeTypeProxy nodeType = null;

            if (e.Key == Key.I || e.Key == Key.A || e.Key == Key.P || e.Key == Key.D1 || e.Key == Key.NumPad1)
            {
                // I, A, P, ! or 1
                nodeType = _typeManager.GetNodeType("CompendiumIdeaNode");
            }
            else if (e.PlatformKeyCode == 187 || e.Key == Key.Add && Keyboard.Modifiers != ModifierKeys.Control)
            {
                // + or =
                nodeType = _typeManager.GetNodeType("CompendiumProNode");
            }
            else if (e.PlatformKeyCode == 189 || e.Key == Key.Subtract && Keyboard.Modifiers != ModifierKeys.Control)
            {
                // -
                nodeType = _typeManager.GetNodeType("CompendiumConNode");
            }
            else if (e.Key == Key.Q || e.Key == Key.Divide || e.PlatformKeyCode == 191)
            {
                // Q, ? or /
                nodeType = _typeManager.GetNodeType("CompendiumQuestionNode");
            }
            else if (e.Key == Key.M)
            {
                // M
                nodeType = _typeManager.GetNodeType("CompendiumMapNode");
            }
            else if (e.Key == Key.D)
            {
                // D
                nodeType = _typeManager.GetNodeType("CompendiumDecisionNode");
            }
            else if (e.Key == Key.Delete)
            {
                Navigator.DeleteNodes(SelectedNodes);
                Navigator.DeleteRelationships(SelectedRelationships);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
            {
                if (SelectedNodes.Length == 1)
                {
                    ShowNodePropertiesDialog(SelectedNodes[0]);
                }
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift && e.Key == Key.R)
            {
                //Ctrl+R
                SortLeftToRight();
            }
            else if (e.Key == Key.Down)
            {
                translateTransform.Y -= 10;
            }
            else if (e.Key == Key.Up)
            {
                translateTransform.Y += 10;
            }
            else if (e.Key == Key.Left)
            {
                translateTransform.X += 10;
            }
            else if (e.Key == Key.Right)
            {
                translateTransform.X -= 10;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Add)
            {
                Zoom(new Point(this.ActualWidth / 2, this.ActualHeight / 2), 0.2);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Subtract)
            {
                Zoom(new Point(this.ActualWidth / 2, this.ActualHeight / 2), -0.2);
            }
            else if (e.Key == Key.Back)
            {
                Navigator.GoBack();
            }

            if (nodeType != null)
            {
                Navigator.AddNode(nodeType, "", _currentMousePosition);
            }
        }

        private void ShowNodePropertiesDialog(INodeProxy node)
        {
            NodePropertiesDialog npd = new NodePropertiesDialog();
            npd.DataContext = node;
            MetadataContext noteKey = new MetadataContext()
            {
                NodeUid = node.Id,
                MetadataName = "Note"
            };
            if (node.HasMetadata(noteKey))
            {
                npd.Note = node.GetNodeMetadata(noteKey).MetadataValue;
            }
            npd.Closed += new EventHandler(NodePropertiesDialog_Close);
            npd.Show();
        }

        private void NodePropertiesDialog_Close(object sender, EventArgs e)
        {
            NodePropertiesDialog dialog = sender as NodePropertiesDialog;
            if (dialog != null)
            {
                if (dialog.DialogResult.Value)
                {
                    TypeManager typeManager = IoC.IoCContainer.GetInjectionInstance().GetInstance<TypeManager>();
                    IMetadataTypeProxy stringMetadataTypeProxy = typeManager.GetMetadataType("string");
                    Navigator.UpdateNodeMetadataAsync(dialog.NodeProxy, Guid.Empty, null, "Note", dialog.Note, stringMetadataTypeProxy);
                }
                else
                {
                    //cancel was selected
                }
            }
        }

        private void Zoom(Point point, double zoomFactor)
        {
            double realCenterX = (point.X - translateTransform.X) / scaleTransform.ScaleX;
            double realCenterY = (point.Y - translateTransform.Y) / scaleTransform.ScaleY;

            scaleTransform.ScaleX += zoomFactor;
            scaleTransform.ScaleY += zoomFactor;

            translateTransform.X = point.X - realCenterX * scaleTransform.ScaleX;
            translateTransform.Y = point.Y - realCenterY * scaleTransform.ScaleY;
        }

        private void CompendiumMapDepthMap_MouseMove(object sender, MouseEventArgs e)
        {
            _currentMousePosition = scaleTransform.Inverse.Transform(translateTransform.Inverse.Transform(e.GetPosition(this)));
            object fe = FocusManager.GetFocusedElement();
            if (!(fe is TextBox))
            {
                this.Focus();
            }
            if (_isRightMouseButtonDown && _tempRelationshipLine != null)
            {
                _tempRelationshipLine.X2 = _currentMousePosition.X;
                _tempRelationshipLine.Y2 = _currentMousePosition.Y;
            }
            if (_isRightMouseButtonDown)
            {
                _mouseMovedWhileRMBDown = true;
                //double xMovement = _originalPanPosition.X - _currentMousePosition.X;
                //double yMovement = _originalPanPosition.Y - _currentMousePosition.Y;
                //translateTransform.Y += yMovement;
                //translateTransform.X += xMovement;
                //_originalPanPosition = _currentMousePosition;
            }
            if (_isLeftMouseButtonDown && _selectionRectangle != null)
            {
                _selectionRectangle.Width = Math.Abs(_selectionStartPoint.X - _currentMousePosition.X);
                _selectionRectangle.Height = Math.Abs(_selectionStartPoint.Y - _currentMousePosition.Y);
                if (_selectionStartPoint.X > _currentMousePosition.X)
                {
                    _selectionRectangle.SetValue(Canvas.LeftProperty, _currentMousePosition.X);
                }
                if (_selectionStartPoint.Y > _currentMousePosition.Y)
                {
                    _selectionRectangle.SetValue(Canvas.TopProperty, _currentMousePosition.Y);
                }
            }
            if (ViewManager != null && ViewManager.Relationships != null)
            {
                foreach (IRelationshipRenderer relationshipRenderer in ViewManager.Relationships)
                {
                    if (relationshipRenderer.IsEditting)
                    {
                        relationshipRenderer.UpdateRelationship(_currentMousePosition);
                    }
                }
            }
        }

        /// <summary>
        /// When a relationship is drawn by right clicking on one node and dragging the line to the another node a FromToRelationship
        /// will be constructed, that is the event that triggers this method.
        /// </summary>
        /// <param name="sender">The NodeRelationshipHelper that has detected the connection has been drawn</param>
        /// <param name="e"></param>
        private void OnNodesConnected(object sender, EventArgs e)
        {
            NodeRelationshipHelper nrh = sender as NodeRelationshipHelper;
            if (nrh != null)
            {
                IDescriptorTypeProxy toDescriptorTypeProxy = _typeManager.GetDescriptorType("To");
                IDescriptorTypeProxy fromDescriptorTypeProxy = _typeManager.GetDescriptorType("From");
                IDescriptorTypeProxy transMapDescriptorTypeProxy = _typeManager.GetDescriptorType("TransclusionMap");

                INodeProxy fromNode = nrh.FromNode.DataContext as INodeProxy;
                INodeProxy toNode = nrh.ToNode.DataContext as INodeProxy;

                Dictionary<IDescriptorTypeProxy, Guid> nodes = new Dictionary<IDescriptorTypeProxy, Guid>();
                nodes.Add(toDescriptorTypeProxy, toNode.Id);
                nodes.Add(fromDescriptorTypeProxy, fromNode.Id);

                IRelationshipTypeProxy relationshipTypeProxy = null;
                if (fromNode.ParentMapNodeUid != this.Navigator.FocalNodeId || toNode.ParentMapNodeUid != this.Navigator.FocalNodeId)
                {
                    nodes.Add(transMapDescriptorTypeProxy, this.Navigator.FocalNodeId);
                    relationshipTypeProxy = _typeManager.GetRelationshipType("TransclusionRelationship");
                }
                else
                {
                    relationshipTypeProxy = _typeManager.GetRelationshipType("FromToRelationship");
                }

                _navigator.ConnectNodesAsync(nodes, relationshipTypeProxy, string.Empty);
                _navigator.GetCurrentNodesAsync();
            }
        }

        private IDictionary<int, List<NodeRenderer>> _columns = new Dictionary<int, List<NodeRenderer>>();
        private List<TreeNodeGroup> _trees = new List<TreeNodeGroup>();

        private void SortLeftToRight()
        {
            lock (_columns)
            {
                _columns.Clear();
                _trees.Clear();

                List<NodeRenderer> leftMostNodes = GetLeftMostNodes();
                leftMostNodes.Sort(new Comparison<NodeRenderer>(NodePositionComparison));
                _columns[1] = leftMostNodes;

                PopulateColumns(2, leftMostNodes, true);

                //position the X coord of each column
                SetNodeXLocations();

                foreach (NodeRenderer leftMostNode in leftMostNodes)
                {
                    TreeNodeGroup treeStructure = BuildTreeNodeGroup(leftMostNode);
                    _trees.Add(treeStructure);
                }

                double marginFromTop = 20; //the margin between the breadcrumb and top of the map
                foreach (TreeNodeGroup treeStructure in _trees)
                {
                    TreeRenderer renderer = new TreeRenderer(marginFromTop);
                    renderer.PositionNodes(treeStructure, 1);
                    marginFromTop = renderer.GetMaximumYPosition();
                }

                //Store the new locations
                foreach (NodeRenderer nodeRenderer in ViewManager.CurrentView.NodeRenderers.Values)
                {
                    nodeRenderer.StoreLocation();
                }
            }
        }

        private TreeNodeGroup BuildTreeNodeGroup(NodeRenderer leftMostNode)
        {
            TreeNodeGroup nodeGroup = new TreeNodeGroup();
            nodeGroup.ParentNode = leftMostNode;
            nodeGroup.ChildNodes = GetChildNodeRenderers(leftMostNode);
            nodeGroup.ChildNodes.Sort(new Comparison<NodeRenderer>(NodePositionComparison));
            nodeGroup.ParentChildGroups = new List<TreeNodeGroup>();

            foreach (NodeRenderer nodeRenderer in nodeGroup.ChildNodes)
            {
                TreeNodeGroup childGroup = BuildTreeNodeGroup(nodeRenderer);
                nodeGroup.ParentChildGroups.Add(childGroup);
            }
            return nodeGroup;
        }

        private void PopulateColumns(int columnNumber, List<NodeRenderer> parentNodes, bool useLocationOrder)
        {
            foreach (NodeRenderer nodeRenderer in parentNodes)
            {
                List<NodeRenderer> childNodes = GetChildNodeRenderers(nodeRenderer);
                if (useLocationOrder)
                {
                    childNodes.Sort(new Comparison<NodeRenderer>(NodePositionComparison));
                }

                if (!_columns.ContainsKey(columnNumber))
                {
                    _columns[columnNumber] = new List<NodeRenderer>();
                }
                _columns[columnNumber].AddRange(childNodes);
            }
            if (_columns[columnNumber].Count() != 0)
            {
                int nextColumn = columnNumber + 1;
                PopulateColumns(nextColumn, _columns[columnNumber], true);
            }
            else
            {
                _columns.Remove(columnNumber); //trim the empty column off
            }
        }

        /// <summary>
        /// Used as the comparison for sorting the nodes based on their initial position.
        /// </summary>
        /// <param name="node1"></param>
        /// <param name="node2"></param>
        /// <returns></returns>
        private int NodePositionComparison(NodeRenderer node1, NodeRenderer node2)
        {
            double node1YPosition = (double)node1.GetValue(Canvas.TopProperty);
            double node2YPosition = (double)node2.GetValue(Canvas.TopProperty);
            if (node1YPosition > node2YPosition)
            {
                return 1;
            }
            else if (node1YPosition < node2YPosition)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private List<NodeRenderer> GetLeftMostNodes()
        {
            List<NodeRenderer> result = new List<NodeRenderer>();
            foreach (NodeRenderer nodeRenderer in this.ViewManager.CurrentView.NodeRenderers.Values)
            {
                bool hasFromDescriptor = false;
                foreach (IDescriptorProxy descriptor in nodeRenderer.Node.Descriptors.Where(
                    x => x.Relationship.RelationshipType.Name == "FromToRelationship" || x.Relationship.RelationshipType.Name == "TransclusionRelationship")) 
                {
                    if (descriptor.DescriptorType.Name == "From")
                    {
                        hasFromDescriptor = true;
                        break;
                    }
                }
                if (!hasFromDescriptor)
                {
                    result.Add(nodeRenderer);
                }
            }
            return result;
        }

        private List<NodeRenderer> GetChildNodeRenderers(NodeRenderer parentNode)
        {
            List<NodeRenderer> result = new List<NodeRenderer>();
            foreach (IDescriptorProxy descriptor in parentNode.Node.Descriptors.Where(d => d.DescriptorType.Name == "To"
                && d.Relationship.RelationshipType.Name != "MapContainerRelationship"))
            {
                if (descriptor.Relationship.RelationshipType.Name == "TransclusionRelationship")
                {
                    IDescriptorProxy transMapDescriptor = descriptor.Relationship.Descriptors.Where(d => d.DescriptorType.Name == "TransclusionMap").First();
                    if (transMapDescriptor.NodeId != this.Navigator.FocalNodeId ||
                        descriptor.Relationship.Descriptors.Where(d => d.DescriptorType.Name == "From").Count() == 0)
                    {
                        continue;
                    }
                }
                IDescriptorProxy fromDescriptor = descriptor.Relationship.Descriptors.Where(d => d.DescriptorType.Name == "From").First();
                if (this.ViewManager.CurrentView.NodeRenderers.ContainsKey(fromDescriptor.NodeId))
                {
                    NodeRenderer childNodeRenderer = this.ViewManager.CurrentView.NodeRenderers[fromDescriptor.NodeId];
                    result.Add(childNodeRenderer);
                }
            }
            return result;
        }

        private double CalculateColumnWidth(List<NodeRenderer> nodeRenderers)
        {
            double result = 0;
            foreach (NodeRenderer nodeRenderer in nodeRenderers)
            {
                if (nodeRenderer.NodeControl.ActualWidth > result)
                {
                    result = nodeRenderer.NodeControl.ActualWidth;
                }
            }
            return result;
        }

        private double GetColumnXPosition(int columnNumber)
        {
            double result = 0;
            for (int c = 1; c < columnNumber; c++)
            {
                result += CalculateColumnWidth(_columns[c]);
                result += 75; //nominal spacing between columns
            }
            result += 50; //left margin
            return result;
        }

        private void SetNodeXLocations()
        {
            for (int columnNumber = 1; columnNumber <= _columns.Count; columnNumber++)
            {
                foreach (NodeRenderer nodeRenderer in _columns[columnNumber])
                {
                    double position = GetColumnXPosition(columnNumber);
                    nodeRenderer.MoveNode(new Point(position, nodeRenderer.Location.Y), true);
                }
            }
        }

        #region Design Mode Helpers
        private static bool? _isInDesignMode;

        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running in Blend
        /// or Visual Studio).
        /// </summary>
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
#if SILVERLIGHT
                    _isInDesignMode = DesignerProperties.IsInDesignTool;
#else
            var prop = DesignerProperties.IsInDesignModeProperty;
            _isInDesignMode
                = (bool)DependencyPropertyDescriptor
                .FromProperty(prop, typeof(FrameworkElement))
                .Metadata.DefaultValue;
#endif
                }

                return _isInDesignMode.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running under Blend
        /// or Visual Studio).
        /// </summary>
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "Non static member needed for data binding")]
        public bool IsInDesignMode
        {
            get
            {
                return IsInDesignModeStatic;
            }
        }

        #endregion
    }
}
