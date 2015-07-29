#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Glyma.Debug;
using Glyma.UtilityService.Proxy;
using SilverlightMappingToolBasic.CompendiumMapProcessor;
using SilverlightMappingToolBasic.UI.Extensions.CookieManagement;
using SilverlightMappingToolBasic.UI.Extensions.ScreenCapture;
using SilverlightMappingToolBasic.UI.Extensions.Security;
using SilverlightMappingToolBasic.UI.SuperGraph.Controller;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow;
using SilverlightMappingToolBasic.UI.SuperGraph.View.CollapseSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MouseCursor;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Realign;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Selector;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using SilverlightMappingToolBasic.UI.ViewModel;
using SimpleIoC;
using System.Windows.Browser;

#endregion

namespace SilverlightMappingToolBasic.UI.SuperGraph.View
{
    public partial class SuperGraphControl : UserControl, ISidebarOperationHandler, IBreadcrumbOperationHandler,
        IRealignControl, ISelectorControl, IArrowContainerControl, IRelatedContentOperationsHandler, IContextMenuParent
    {
        private const double ZoomChangeRatio = 0.9;
        private ArrowControlFactory _arrowFactory;
        private SuperMouseManager _clickManager;
        private Point _exploreStartLocation;
        private bool _explorerOnly;
        private IIoCContainer _ioc;
        private Point _lastLocation;
        private MapInformation _mapInformation;
        private Dictionary<Guid, MapInformation> _mapInformationCollection;
        private TranslateTransform _moveGraphTransform;
        private NodeDetailsControl _nodeDetailsControl;
        private NodeControlFactory _nodeFactory;

        private NodePropertiesDialog _nodePropertiesDialog;
        private NodeTextInput _nodeTextInput;
        private double _zoom = 1;
        private Dictionary<Guid, Point> _oldLocations; 

        public SuperGraphControl()
        {
            InitializeComponent();

            if (!App.IsDesignTime)
            {
                ContextMenuContainer.CommitCollapseStatesClicked += ContextMenuOnCommitCollapseStatesClicked;
                ContextMenuContainer.CreateNewNodeClicked += ContextMenuContainerOnCreateNewNodeClicked;
                ContextMenuContainer.ImportClicked += ContextMenuContainerOnImportClicked;
                ContextMenuContainer.ExportClicked += ContextMenuContainerOnExportClicked;
                ContextMenuContainer.ContextMenuParent = this;
                var arrowCreationManager = new ArrowCreationManager(this);
                arrowCreationManager.RenderTransform = MoveGraphTransform;
                IoC.RegisterComponent(arrowCreationManager);

                PointerMouseManager.LeftButtonDragStart += Pointer_OnLeftButtonDragStart;
                PointerMouseManager.LeftButtonDragMove += Pointer_OnLeftButtonDragMove;
                PointerMouseManager.LeftButtonDragReleased += Pointer_OnLeftButtonDragReleased;
                PointerMouseManager.LeftButtonSingleClick += Pointer_OnLeftButtonSingleClick;
                PointerMouseManager.RightButtonDragStart += Pointer_OnRightButtonDragStart;
                PointerMouseManager.RightButtonDragMove += Pointer_OnRightButtonDragMove;
                PointerMouseManager.RightButtonDragReleased += Pointer_OnRightButtonDragReleased;
                PointerMouseManager.RightButtonSingleClick += Pointer_RightButtonSingleClick;
                PointerMouseManager.RightButtonDragReleased += Pointer_RightButtonDragReleased;
                PointerMouseManager.MouseMove += Pointer_MouseMove;

                ExplorerMouseManager.LeftButtonDragStart += Explorer_OnLeftButtonDragStart;
                ExplorerMouseManager.LeftButtonDragMove += Explorer_OnLeftButtonDragMove;
                ExplorerMouseManager.LeftButtonDragReleased += Explorer_OnLeftButtonDragReleased;
                ExplorerMouseManager.LeftButtonSingleClick += Explorer_OnLeftButtonSingleClick;

                Selector.Initialise(this);
            }
            
        }

        


        public Dictionary<Guid, MapInformation> MapInformationCollection
        {
            get
            {
                if (_mapInformationCollection == null)
                {
                    _mapInformationCollection = new Dictionary<Guid, MapInformation>();
                }
                return _mapInformationCollection;
            }
        }

        public Dictionary<Guid, Point> OldLocations
        {
            get
            {
                if (_oldLocations == null)
                {
                    _oldLocations = new Dictionary<Guid, Point>();
                }
                return _oldLocations;
            }
        } 

        private IIoCContainer IoC
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

        private IMouseManager PointerMouseManager
        {
            get { return MouseManager.PointerMouseManager; }
        }

        private IMouseManager ExplorerMouseManager
        {
            get { return MouseManager.ExplorerMouseManager; }
        }

        public bool LeftDragStarted { get; private set; }

        public bool RightDragStarted { get; private set; }

        public bool AllowAutoRealign
        {
            get
            {
                if (Context != null && Context.Metadata.ContainsKey("AllowAutoRealign"))
                {
                    var value = Context.Metadata["AllowAutoRealign"].Value;
                    if (value == "1" || value == "true" || value == "True")
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
        }

        public SuperMouseManager MouseManager
        {
            get { return _clickManager ?? (_clickManager = new SuperMouseManager(LayoutRoot)); }
        }


        public NodeControlFactory NodeFactory
        {
            get
            {
                if (_nodeFactory == null)
                {
                    _nodeFactory = new NodeControlFactory();
                }

                return _nodeFactory;
            }
        }

        public ArrowControlFactory ArrowFactory
        {
            get
            {
                if (_arrowFactory == null)
                {
                    _arrowFactory = new ArrowControlFactory(NodeFactory);
                }

                return _arrowFactory;
            }
        }

        public NodeDetailsControl NodeDetailsControl
        {
            get
            {
                if (_nodeDetailsControl == null)
                {
                    _nodeDetailsControl = Ref.NodeDetailsControl;
                }
                return _nodeDetailsControl;
            }
        }

        #region UserStyle

        public UserStyle UserStyle
        {
            set
            {
                if (App.UserStyle != value)
                {
                    if (Context != null)
                    {
                        MapInformation.NodeLocations = GetVisibleNodeLocations();
                    }
                    App.UserStyle = value;
                    if (Context != null)
                    {
                        MapInformation.Clear();
                    }
                    foreach (MapInformation info in MapInformationCollection.Values)
                    {
                        info.Clear();
                    }
                    switch (App.UserStyle)
                    {
                        case UserStyle.Author:
                            Ref.Sidebar.Pointer.Visibility = Visibility.Collapsed;
                            Ref.Sidebar.Explorer.Visibility = Visibility.Visible;
                            SuperCursor.SetCursorTemplate(this, null);
                            Cursor = Cursors.Arrow;
                            Ref.Refresh(Ref.Breadcrumbs.CurrentBreadcrumbControl);
                            foreach (NodeControl nodeControl in GetNodeControls())
                            {
                                nodeControl.UserStyle = UserStyle.Author;
                            }
                            break;
                        case UserStyle.Reader:
                            Ref.Sidebar.Pointer.Visibility = Visibility.Visible;
                            Ref.Sidebar.Explorer.Visibility = Visibility.Collapsed;
                            Cursor = Cursors.Arrow;
                            Ref.Refresh(Ref.Breadcrumbs.CurrentBreadcrumbControl);
                            foreach (NodeControl nodeControl in GetNodeControls())
                            {
                                nodeControl.UserStyle = UserStyle.Reader;
                            }
                            if (AllowAutoRealign && Context != null)
                            {
                                Deployment.Current.Dispatcher.BeginInvoke(() => AutoRealign());
                            }
                            break;
                    }
                    Ref.Breadcrumbs.ChangeMouseStyle();
                }
            }
        }

        private Dictionary<Guid, Point> GetVisibleNodeLocations()
        {
            return GetVisibleNodeControls().ToDictionary(nodeControl => nodeControl.ViewModelNode.Id, nodeControl => nodeControl.Centre);
        }

        public bool ExplorerOnly
        {
            get { return _explorerOnly; }
            set
            {
                _explorerOnly = value;
                if (_explorerOnly)
                {
                    UserStyle = UserStyle.Reader;
                    Ref.Sidebar.Pointer.Visibility = Visibility.Collapsed;
                    Ref.Sidebar.Explorer.Visibility = Visibility.Collapsed;
                    Ref.Sidebar.Pointer.IsHitTestVisible = false;
                    Ref.Sidebar.Explorer.IsHitTestVisible = false;
                    Ref.Sidebar.RealignSubMenu.Margin = new Thickness(-10, 145, 0, 0);
                    Ref.Sidebar.ZoomSubMenu.Margin = new Thickness(-10, 215, 0, 0);
                }
                else
                {
                    if (App.UserStyle == UserStyle.Author)
                    {
                        Ref.Sidebar.Pointer.Visibility = Visibility.Collapsed;
                        Ref.Sidebar.Explorer.Visibility = Visibility.Visible;
                    }
                    else if (App.UserStyle == UserStyle.Reader)
                    {
                        Ref.Sidebar.Pointer.Visibility = Visibility.Visible;
                        Ref.Sidebar.Explorer.Visibility = Visibility.Collapsed;
                    }
                    Ref.Sidebar.Pointer.IsHitTestVisible = true;
                    Ref.Sidebar.Explorer.IsHitTestVisible = true;
                    Ref.Sidebar.RealignSubMenu.Margin = new Thickness(-10, 215, 0, 0);
                    Ref.Sidebar.ZoomSubMenu.Margin = new Thickness(-10, 285, 0, 0);
                }
            }
        }

        public void SetMouse()
        {
            switch (App.UserStyle)
            {
                case UserStyle.Author:
                    SuperCursor.SetCursorTemplate(this, null);
                    Cursor = Cursors.Arrow;
                    break;
                case UserStyle.Reader:
                    SuperCursor.SetCursorTemplate(this, null);
                    Cursor = Cursors.Arrow;
                    break;
            }
        }

        #endregion

        public void Add(ArrowControl arrowControl)
        {
            NodeContainer.Children.Add(arrowControl);
        }

        public void Remove(ArrowControl arrowControl)
        {
            NodeContainer.Children.Remove(arrowControl);
        }

        public bool IsArrowExist(NodeControl from, NodeControl to)
        {
            var exist =
                GetRelationships()
                    .FirstOrDefault(q => q != null && q.From == from.ViewModelNode.Id && q.To == to.ViewModelNode.Id);
            return exist != null;
        }

        public MainPage Ref { get; set; }

        void IBreadcrumbOperationHandler.Home()
        {
            Ref.HomeScreen(true);
        }

        public NodePropertiesDialog NodePropertiesDialog
        {
            get
            {
                if (_nodePropertiesDialog == null)
                {
                    _nodePropertiesDialog = new NodePropertiesDialog();
                }
                return _nodePropertiesDialog;
            }
        }


        public NodeTextInput NodeTextInput
        {
            get
            {
                if (_nodeTextInput == null)
                {
                    _nodeTextInput = Ref.NodeTextEditor;
                }
                return _nodeTextInput;
            }
        }

        public double ContainerHeight
        {
            get { return ActualHeight; }
        }

        public double ContainerWidth
        {
            get { return ActualWidth; }
        }

        public MapInformation MapInformation
        {
            get
            {
                if (_mapInformation == null || _mapInformation.MapId != Context.Id)
                {
                    if (Context != null && MapInformationCollection.ContainsKey(Context.Id))
                    {
                        _mapInformation = MapInformationCollection[Context.Id];
                    }
                    else
                    {
                        _mapInformation = CookieManager.GetMapInfo(Context.Id);
                        MapInformationCollection.Add(Context.Id, _mapInformation);
                    }
                }
                return _mapInformation;
            }
        }

        public IEnumerable<NodeControl> GetNodeControls()
        {
            return NodeContainer.Children.OfType<NodeControl>();
        }

        public IEnumerable<NodeControl> GetVisibleNodeControls()
        {
            return GetNodeControls().Where(nodeControl => !nodeControl.IsCollapsed);
        }

        public bool HasVisibleNodes()
        {
            return GetVisibleNodeControls().Any();
        }

        public IEnumerable<ArrowControl> GetArrowControls()
        {
            return NodeContainer.Children.OfType<ArrowControl>();
        }

        public IEnumerable<Relationship> GetRelationships()
        {
            var arrowViewModels = GetArrowControls().Select(q => (ArrowViewModel) q.DataContext);
            return arrowViewModels.Select(q => q.ViewModelRelationship);
        }

        public void ShowDescription(Node node)
        {
            if (node.DescriptionType != NodeDescriptionType.None)
            {
                switch (node.DescriptionType)
                {
                    case NodeDescriptionType.Gps:
                        break;
                    case NodeDescriptionType.Iframe:
                        RelatedContentPanelUtil.Instance.LoadRelatedIframeContent(node.NodeProperties.DescriptionUrl, node.NodeProperties.Width, node.NodeProperties.Height);
                        break;
                    default:
                        RelatedContentPanelUtil.Instance.LoadRelatedNodeHtmlContent(node.Description, node.NodeProperties.Width, node.NodeProperties.Height);
                        break;
                }
            }
        }

        public void ShowRelatedMaps(Node node)
        {
            var controller = IoC.GetInstance<ISuperGraphNodeOperations>();
            controller.TransclusionsLoadCompleted += TransclusionsLoadCompleted;
            controller.GetNodeTransclusions(node);
        }

        public void ShowVideoControl(Node node)
        {
            if (node.VideoInfo.HasVideo)
            {
                RelatedContentPanelUtil.Instance.LoadRelatedVideoContent(node.VideoInfo.VideoSource, node.VideoInfo.Size);
            }
        }

        public event EventHandler<MoveTransformEventArgs> MapMoved;
        public INode Context { get; private set; }

        public Point LastLocation
        {
            get
            {
                var transformedLocation = MoveGraphTransform.Inverse.Transform(_lastLocation);

                return transformedLocation;
            }
        }

        public void RecheckIncorrectVisibility()
        {
            var needCheck = true;
            while (needCheck)
            {
                needCheck = false;

                foreach (NodeControl nodeControl in GetNodeControls().ToList())
                {
                    if (!nodeControl.IsCollapsed)
                    {
                        if (nodeControl.ParentNodes.Count > 0 && nodeControl.ParentNodes.All(q => q.IsCollapsed))
                        {
                            nodeControl.IsCollapsed = true;
                            needCheck = true;
                            break;
                        }
                    }
                    else
                    {
                        if (nodeControl.ParentNodes.Count == 0)
                        {
                            nodeControl.IsCollapsed = false;
                            needCheck = true;
                            break;
                        }
                    }
                }
            }

            foreach (ArrowControl arrowControl in GetArrowControls())
            {
                if (!arrowControl.To.IsCollapsed && !arrowControl.From.IsCollapsed)
                {
                    arrowControl.IsCollapsed = false;
                }
                else
                {
                    arrowControl.IsCollapsed = true;
                }
            }
        }

        void ISelectorControl.SelectNodeByGuid(Guid id)
        {
            var node = GetNodeControls().FirstOrDefault(q => q.ViewModelNode.Proxy.Id == id);

            if (node != null)
            {
                RecheckIncorrectVisibility();
                if (node.IsCollapsed)
                {
                    node.IsCollapsed = false;
                    var listToLoop = new List<NodeControl> {node};
                    var needRecheck = true;
                    while (needRecheck)
                    {
                        needRecheck = false;
                        var list = new List<NodeControl>();
                        foreach (NodeControl nodeControl in listToLoop)
                        {
                            foreach (NodeControl parent in nodeControl.ParentNodes)
                            {
                                if (parent.IsCollapsed)
                                {
                                    parent.IsCollapsed = false;
                                    list.Add(parent);
                                    needRecheck = true;
                                }
                            }
                        }
                        listToLoop = list.ToList();
                    }
                }
                RecheckIncorrectVisibility();
                ReScanForCollapseStates();

                Selector.AddNode(node);
                CentraliseNode(node);
            }
            else
            {
                if (App.UserStyle == UserStyle.Reader && AllowAutoRealign)
                {
                    //Force UI thread to realign
                    Deployment.Current.Dispatcher.BeginInvoke(() => AutoRealign());
                }
            }
        }

        void ISelectorControl.CentraliseMostImportantParent()
        {
            var parentNodes = GetVisibleNodeControls().Where(q => q.ParentNodes.Count == 0);
            NodeControl mostImportantParent = null;
            foreach (NodeControl parent in parentNodes)
            {
                if (mostImportantParent == null)
                {
                    mostImportantParent = parent;
                }
                else if (parent.GetAllChildNodeControls().Count() > mostImportantParent.GetAllChildNodeControls().Count())
                {
                    mostImportantParent = parent;
                }
            }

            if (mostImportantParent == null)
            {
                var ordered =
                    GetVisibleNodeControls()
                        .OrderBy(q => q.ViewModelNode.Location.Y)
                        .ThenBy(q => q.ViewModelNode.Location.X);
                mostImportantParent = ordered.FirstOrDefault();
            }


            if (mostImportantParent != null)
            {
                CentraliseNodeVertically(mostImportantParent);
            }
        }

        public void ReScanForCollapseStates()
        {
            foreach (NodeControl nodeControl in GetNodeControls())
            {
                if (nodeControl.ChildNodes.Count == 0)
                {
                    nodeControl.CollapseControl.State = CollapseState.None;
                }
                else if (nodeControl.ChildNodes.All(q => q.IsCollapsed))
                {
                    nodeControl.CollapseControl.State = CollapseState.Collapsed;
                }
                else if (nodeControl.ChildNodes.All(q => !q.IsCollapsed))
                {
                    nodeControl.CollapseControl.State = CollapseState.Expanded;
                }
                else
                {
                    nodeControl.CollapseControl.State = CollapseState.SemiCollapsed;
                }
            }
        }

        public event EventHandler<RoutedPropertyChangedEventArgs<double>> ZoomChanged;

        public double Zoom
        {
            get { return _zoom; }
            private set
            {
                if (!_zoom.Equals(value))
                {
                    var old = _zoom;
                    _zoom = value;
                    if (_zoom < 0.1)
                    {
                        _zoom = 0.1;
                    }
                    else if (_zoom > 10)
                    {
                        _zoom = 10;
                    }

                    if (!old.Equals(_zoom))
                    {
                        ScaleTransform.ScaleX = _zoom;
                        ScaleTransform.ScaleY = _zoom;
                    }
                    MapInformation.Zoom = _zoom;

                    if (ZoomChanged != null)
                    {
                        ZoomChanged(this, new RoutedPropertyChangedEventArgs<double>(old, _zoom));
                    }
                }
            }
        }

        public TranslateTransform MoveGraphTransform
        {
            get
            {
                if (_moveGraphTransform == null)
                {
                    _moveGraphTransform = new TranslateTransform();
                }

                return _moveGraphTransform;
            }
        }

        public void MoveMap(double x, double y)
        {
            MoveGraphTransform.Y += y;
            MoveGraphTransform.X += x;
            if (MapMoved != null)
            {
                MapMoved(this, new MoveTransformEventArgs(x, y));
            }
            MapInformation.MapLocation = new Point(MoveGraphTransform.X, MoveGraphTransform.Y);
        }

        public void VerticalRealign(bool isPartialRealign = false)
        {
            var redrawFactory = new VerticalRealignFactory(this, isPartialRealign);
            var nodesToBeCommitted = redrawFactory.Draw();
            if (App.UserStyle != UserStyle.Reader && nodesToBeCommitted.Count > 0)
            {
                IoC.GetInstance<ISuperGraphNodeBatchOperations>().CommitLocations(nodesToBeCommitted);
            }
        }

        void IOperationHandler.Refresh()
        {
            CookieManager.Delete(Context.Id);
            _mapInformation = null;
            if (MapInformationCollection.ContainsKey(Context.Id))
            {
                MapInformationCollection.Remove(Context.Id);
            }
            var currentBreadcrumb = Ref.Breadcrumbs.CurrentBreadcrumbControl;
            Ref.Refresh(currentBreadcrumb);
        }

        public void ReaderMode()
        {
            UserStyle = UserStyle.Reader;
            if (Selector.NodeControls.Count > 0)
            {
                Selector.Clear();
            }
            RelatedContentPanelUtil.Instance.SetReaderMode();
        }

        public void AuthorMode()
        {
            UserStyle = UserStyle.Author;
            if (Selector.NodeControls.Count > 0)
            {
                Selector.Clear();
            }
            RelatedContentPanelUtil.Instance.SetAuthorMode();
        }

        public void HorizontalRealign(bool isPartialRealign = false)
        {
            var redrawFactory = new HorizontalRealignFactory(this, isPartialRealign);
            var nodesToBeCommitted = redrawFactory.Draw();
            if (App.UserStyle != UserStyle.Reader && nodesToBeCommitted.Count > 0)
            {
                IoC.GetInstance<ISuperGraphNodeBatchOperations>().CommitLocations(nodesToBeCommitted);
            }
        }

        public void ZoomIn()
        {
            Zoom /= ZoomChangeRatio;
            _lastLocation.Y = _lastLocation.Y*ZoomChangeRatio;
            _lastLocation.X = _lastLocation.X*ZoomChangeRatio;
        }

        public void ZoomOut()
        {
            Zoom *= ZoomChangeRatio;
            _lastLocation.Y = _lastLocation.Y/ZoomChangeRatio;
            _lastLocation.X = _lastLocation.X/ZoomChangeRatio;
        }

        public void DefaultZoom()
        {
            var previousZoom = Zoom;
            Zoom = 1;
            _lastLocation.Y = _lastLocation.Y*previousZoom;
            _lastLocation.X = _lastLocation.X*previousZoom;
        }

        public void ResetViewport()
        {
            ((ISelectorControl) this).CentraliseMostImportantParent();
        }

        #region Public Accessable Events

        public void ModelChanged(object sender, ModelChangedEventArgs<Node, Relationship> e)
        {
            NodeControl focusedNode = null;
            var controller = sender as SuperGraphController;

            if (controller != null)
            {
                Context = controller.Context;
            }
            /// TODO: Need to refactor the following to be self-contained.
            foreach (KeyValuePair<Node, ModelOperationType> viewModelNodeOperation in e.Nodes)
            {
                if (viewModelNodeOperation.Value == ModelOperationType.New ||
                    viewModelNodeOperation.Value == ModelOperationType.Clone)
                {
                    var viewModelNode = viewModelNodeOperation.Key;
                    var nodeControl = NodeFactory.CreateNode(viewModelNode, this);
                    if (nodeControl != null && !NodeContainer.Children.Contains(nodeControl))
                    {
                        nodeControl.RenderTransform = MoveGraphTransform;
                        nodeControl.CutClicked += OnCutClicked;
                        nodeControl.CopyClicked += OnCopyClicked;
                        nodeControl.CloneClicked += OnCloneClicked;
                        nodeControl.NodeClicked += OnNodeClicked;
                        nodeControl.ChangeNodeTypeClicked += OnChangeNodeTypeClicked;
                        nodeControl.NodeSelected += OnNodeSelected;
                        if (viewModelNode.IsFocused)
                        {
                            focusedNode = nodeControl;
                        }

                        //nodeControl.NodeCornerControl.DataContext = viewModelNode;
                        NodeContainer.Children.Add(nodeControl);
                    }
                }
                else if (viewModelNodeOperation.Value == ModelOperationType.Deleted)
                {
                    var viewModelNode = viewModelNodeOperation.Key;
                    if (viewModelNode == null)
                    {
                        continue;
                    }

                    var removedNodeSet = NodeFactory.RemoveNode(viewModelNode);
                    if (removedNodeSet != null)
                    {
                        NodeContainer.Children.Remove(removedNodeSet.Control);
                        if (removedNodeSet.Control != null)
                        {
                            removedNodeSet.Control.Dispose();
                        }
                    }

                    var irrelevantArrowSets = ArrowFactory.RemoveArrows(viewModelNode);

                    foreach (ArrowControlFactorySet irrelevantArrowSet in irrelevantArrowSets)
                    {
                        NodeContainer.Children.Remove(irrelevantArrowSet.Control);
                    }
                }
            }

            var arrows = ArrowFactory.ReassessPendingRelationships();
            foreach (ArrowControl arrow in arrows)
            {
                NodeContainer.Children.Add(arrow);
            }

            /// TODO: Need to refactor the following to be self-contained.

            foreach (KeyValuePair<Relationship, ModelOperationType> viewModelRelationshipOperation in e.Relationships)
            {
                if (viewModelRelationshipOperation.Value == ModelOperationType.New)
                {
                    var viewModelRelationship = viewModelRelationshipOperation.Key;

                    var arrow = ArrowFactory.CreateArrow(viewModelRelationship);

                    if (arrow != null)
                    {
                        arrow.RenderTransform = MoveGraphTransform;
                        arrow.Ref = this;
                        if (!NodeContainer.Children.Contains(arrow))
                        {
                            NodeContainer.Children.Add(arrow);
                        }
                    }
                }
                else if (viewModelRelationshipOperation.Value == ModelOperationType.Deleted)
                {
                    var viewModelRelationship = viewModelRelationshipOperation.Key;

                    var arrowSet = ArrowFactory.RemoveArrow(viewModelRelationship);

                    if (arrowSet != null)
                    {
                        NodeContainer.Children.Remove(arrowSet.Control);
                        arrowSet.Control.UnlinkRelatedNodeControls();
                    }
                }
            }

            GC.Collect();
            NodeContainer.InvalidateArrange();
            if (focusedNode != null && focusedNode.Name.Trim().Length == 0)
            {
                focusedNode.SetEditFocus();
                focusedNode.GiveEditorFocus();
            }
        }

        #endregion

        #region implementation of ISelectorControl

        INodesSelector ISelectorControl.NodesSelector
        {
            get { return Selector; }
        }

        MultiNodesContextMenu ISelectorControl.MultiNodesContextMenu
        {
            get { return ContextMenuContainer.MultiNodesContextMenu; }
        }

        AuthorNodeControlContextMenu ISelectorControl.AuthorNodeControlContextMenu
        {
            get { return ContextMenuContainer.AuthorContextMenu; }
        }

        ViewerNodeControlContextMenu ISelectorControl.ViewerNodeControlContextMenu
        {
            get { return ContextMenuContainer.ViewerContextMenu; }
        }

        ArrowContextMenu ISelectorControl.ArrowContextMenu
        {
            get { return ContextMenuContainer.ArrowContextMenu; }
        }

        Canvas ISelectorControl.NodeContainer
        {
            get { return NodeContainer; }
        }

        NodeOperationDetails ISelectorControl.NodeOperationDetails
        {
            set { ContextMenuContainer.ContextMenu.OperationDetails = value; }
        }

        Border ISelectorControl.DragSelectionBorder
        {
            get { return DragSelectionBorder; }
        }

        #endregion

        public event EventHandler<NodeClickedEventArgs> NodeClicked;
        public event EventHandler<FilesDroppedEventArgs> FilesDropped;

        public void LoadCookie()
        {
            Zoom = MapInformation.Zoom;
            MoveMap(MapInformation.MapLocation.X, MapInformation.MapLocation.Y);
            RealignFactory.RealignStyle = MapInformation.AutoRealignStyle;
            foreach (NodeControl node in GetNodeControls())
            {
                if (MapInformation.Visibilitys.ContainsKey(node.ViewModelNode.Proxy.Id))
                {
                    node.IsCollapsed = MapInformation.Visibilitys[node.ViewModelNode.Proxy.Id] == Visibility.Collapsed;
                }

                if (MapInformation.States.ContainsKey(node.ViewModelNode.Proxy.Id))
                {
                    node.ViewModelNode.State = MapInformation.States[node.ViewModelNode.Proxy.Id];
                }
            }

            foreach (ArrowControl arrow in GetArrowControls())
            {
                if (arrow.From.IsCollapsed || arrow.To.IsCollapsed)
                {
                    arrow.IsCollapsed = true;
                }
                else
                {
                    arrow.IsCollapsed = false;
                }
            }


            ReScanForCollapseStates();
        }

        public void RecheckLocations()
        {
            var nodeControls = GetVisibleNodeControls().ToList();
            foreach (var valuePair in MapInformation.NodeLocations)
            {
                var node = nodeControls.FirstOrDefault(q => q.ViewModelNode.Id == valuePair.Key);
                Point dif;
                if (node != null && PointExtension.IsMovedALot(valuePair.Value, node.Centre, out dif))
                {
                    MoveMap(dif.X, dif.Y);
                    break;
                }
            }
        }

        private void ContextMenuContainerOnImportClicked(object sender, EventArgs eventArgs)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Map Files (.xml .zip)|*.xml;*.zip|Xml Files (.xml)|*.xml|Zip Files (*.zip)|*.zip|All Files (*.*)|*.*";
            openFileDialog.Multiselect = true;

            var showDialog = openFileDialog.ShowDialog();
            if (showDialog != null && showDialog.Value && openFileDialog.Files.Any())
            {
                Ref.ImportFiles(openFileDialog.Files.ToArray());
            }
        }

        private void ContextMenuContainerOnExportClicked(object sender, ExportClickedEventArgs e)
        {
            //todo: using selected nodes in.
            IoC.GetInstance<IExportServiceManager>().CreateExportJobCompleted.RegisterEvent(Context.Proxy.Id, ExportJobCreated);
            IoC.GetInstance<IExportServiceManager>().CreateExportJobAsync(Context.Proxy.DomainId, Context.Proxy.Id, new Dictionary<string, string>(), MapType.IBIS,  e.Type);
        }

        private void ExportJobCreated(object sender, ResultEventArgs<IExportJob> e)
        {
            //todo: complete this
        }

        private void ContextMenuOnCommitCollapseStatesClicked(object sender, CommitCollapseStatesEventArgs e)
        {
            var pendingVisibilityNodes = new List<Node>();
            var pendingCollapseStateNodes = new List<Node>();
            foreach (NodeControl nodeControl in GetNodeControls())
            {
                if (e.Group == PermissionLevel.Author)
                {
                    if (nodeControl.ViewModelNode.IsAuthorCollapseStateDirty)
                    {
                        pendingCollapseStateNodes.Add(nodeControl.ViewModelNode);
                        nodeControl.ViewModelNode.IsAuthorCollapseStateDirty = false;
                    }

                    if (nodeControl.ViewModelNode.IsAuthorVisibilityDirty)
                    {
                        pendingVisibilityNodes.Add(nodeControl.ViewModelNode);
                        nodeControl.ViewModelNode.IsAuthorVisibilityDirty = false;
                    }
                }
                else
                {
                    if (nodeControl.ViewModelNode.IsViewerCollapseStateDirty ||
                            nodeControl.ViewModelNode.State != nodeControl.ViewModelNode.ViewerCollapseState)
                    {
                        pendingCollapseStateNodes.Add(nodeControl.ViewModelNode);
                        nodeControl.ViewModelNode.IsViewerCollapseStateDirty = false;
                    }

                    if (nodeControl.ViewModelNode.IsViewerVisibilityDirty ||
                        nodeControl.ViewModelNode.Visibility != nodeControl.ViewModelNode.ViewerVisibility)
                    {
                        pendingVisibilityNodes.Add(nodeControl.ViewModelNode);
                        nodeControl.ViewModelNode.IsViewerVisibilityDirty = false;
                    }
                }
            }
            IoC.GetInstance<ISuperGraphNodeBatchOperations>()
                .CommitCollapseStates(pendingVisibilityNodes, pendingCollapseStateNodes, e.Group);
        }

        private void OnNodeSelected(object sender, NodeSelectedEventArgs e)
        {
            switch (e.Type)
            {
                case NodeSelectedEventArgs.NodeSelectedType.All:
                    RelatedContentPanelUtil.Instance.ResetAndHidePanels();
                    ShowDescription(e.SelectedNode);
                    ShowRelatedMaps(e.SelectedNode);
                    ShowVideoControl(e.SelectedNode);
                    break;
                case NodeSelectedEventArgs.NodeSelectedType.RelatedContent:
                    ShowDescription(e.SelectedNode);
                    break;
                case NodeSelectedEventArgs.NodeSelectedType.RelatedMap:
                    if (e.Source == NodeSelectedEventArgs.ClickSource.Node) 
                    {
                        if (e.SelectedNode.NodeClickOptions.ShowRelatedMaps)
                        {
                            ShowRelatedMaps(e.SelectedNode);
                        }
                        else 
                        {
                            RelatedContentPanelUtil.Instance.ClearRelatedMaps(); //hide the related maps if already showing
                        }
                    }
                    else if (e.Source == NodeSelectedEventArgs.ClickSource.CornerButton)
                    {
                        ShowRelatedMaps(e.SelectedNode);
                    }
                    break;
                case NodeSelectedEventArgs.NodeSelectedType.Video:
                    ShowVideoControl(e.SelectedNode);
                    if (e.Source == NodeSelectedEventArgs.ClickSource.CornerButton && e.SelectedNode.NodeActionOptions.ShowRelatedContentWithVideo)
                    {
                        ShowDescription(e.SelectedNode);
                    }
                    break;
            }
        }

        public void ClearMapObjects(bool isSaveStates = true)
        {
            Selector.Clear();
            if (isSaveStates)
            {
                foreach (NodeControl node in GetNodeControls())
                {
                    MapInformation.SetCollapseState(node.ViewModelNode.Proxy.Id, node.CollapseState);
                    MapInformation.SetVisibility(node.ViewModelNode.Proxy.Id,
                        node.IsCollapsed ? Visibility.Collapsed : Visibility.Visible);
                }
                if (Context != null)
                {
                    MapInformation.NodeLocations = GetVisibleNodeLocations();
                }
            }

            var removedArrows = ArrowFactory.Clear();

            foreach (ArrowControlFactorySet removedArrowSet in removedArrows)
            {
                NodeContainer.Children.Remove(removedArrowSet.Control);
            }

            var removedNodes = NodeFactory.Clear();

            foreach (NodeControlFactorySet removedNodeSet in removedNodes)
            {
                NodeContainer.Children.Remove(removedNodeSet.Control);
                if (removedNodeSet.Control != null)
                {
                    removedNodeSet.Control.Dispose();
                }
            }
            _moveGraphTransform = new TranslateTransform();
            IoC.GetInstance<ArrowCreationManager>().RenderTransform = MoveGraphTransform;
        }

        /// <summary>
        ///     This method is for automatical realignment, it will realigns the map (horizontally or vertically)
        ///     based on last realign operation. If there is no realign action occurred, it will realign the map horizontally.
        /// </summary>
        /// <param name="focusedControl">
        ///     If focusedControl has been set, it will automatically moves the map to make sure the focused control not being
        ///     moved.
        /// </param>
        public void AutoRealign(NodeControl focusedControl = null)
        {
            var lastPosition = new Point();
            if (focusedControl != null)
            {
                lastPosition = focusedControl.ViewModelNode.Location;
            }
            if (RealignFactory.RealignStyle == RealignStyle.Horizontal)
            {
                HorizontalRealign();
            }
            else
            {
                VerticalRealign();
            }

            if (focusedControl != null)
            {
                var currentPosintion = focusedControl.ViewModelNode.Location;
                var difference = currentPosintion.Subtract(lastPosition);
                MoveMap(-difference.X, -difference.Y);
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IoC.GetInstance<ArrowCreationManager>().InMotion)
            {
                IoC.GetInstance<ArrowCreationManager>().SetTo(null);
            }
            else if (RightDragStarted)
            {
                Pointer_OnRightButtonDragReleased(this, new MouseManagerEventArgs());
            }
        }

        private void CentraliseNodeVertically(NodeControl nodeControl)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                var moveY = ActualHeight/Zoom/2 - MoveGraphTransform.Y - nodeControl.Centre.Y;
                var moveX = - MoveGraphTransform.X - nodeControl.Centre.X +
                            (GlymaParameters.NodeTextWidth/2 + GlymaParameters.Margin);
                MoveMap(moveX, moveY);
            });
        }

        public void GetMapBounds(out double left, out double right, out double top, out double bottom)
        {
            var visibleNodes = GetVisibleNodeControls();

            var locationList = visibleNodes.Select(q => q.Centre).ToList();

            left = locationList.Min(q => q.X);
            right = locationList.Max(q => q.X);
            top = locationList.Min(q => q.Y);
            bottom = locationList.Max(q => q.Y);
        }

        private void CentraliseNode(NodeControl nodeControl)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (App.UserStyle == UserStyle.Reader && AllowAutoRealign)
                {
                    AutoRealign();
                }


                var oldLocation = nodeControl.Centre;

                var visibleNodes = GetVisibleNodeControls();

                var locationList = visibleNodes.Select(q => q.Centre).ToList();

                var leftMost = locationList.Min(q => q.X);
                var rightMost = locationList.Max(q => q.X);
                var topMost = locationList.Min(q => q.Y);
                var bottomMost = locationList.Max(q => q.Y);

                var newLocation = oldLocation;

                var widthOffset = GlymaParameters.NodeTextWidth/2 + GlymaParameters.Margin;
                var heightOffset = (nodeControl.TextArea.ActualHeight + 40)/2 + GlymaParameters.Margin;


                if (oldLocation.X <= ActualWidth/Zoom/2 + leftMost - widthOffset)
                {
                    newLocation.X = ActualWidth/Zoom/2 + leftMost - widthOffset;
                }
                else if (oldLocation.X >= rightMost - ActualWidth/Zoom/2 + widthOffset)
                {
                    newLocation.X = rightMost - ActualWidth/Zoom/2 + widthOffset;
                }

                //This make sure if the map is smaller than the display area, it will realign the map to the center
                if (bottomMost - topMost + 80 >= ActualHeight/Zoom)
                {
                    if (oldLocation.Y < ActualHeight/Zoom/2 + topMost - heightOffset &&
                        !oldLocation.Y.Equals(bottomMost))
                    {
                        newLocation.Y = ActualHeight/Zoom/2 + topMost - heightOffset;
                    }
                    else if (oldLocation.Y > bottomMost - ActualHeight/Zoom/2 + heightOffset &&
                             !oldLocation.Y.Equals(topMost))
                    {
                        newLocation.Y = bottomMost - ActualHeight/Zoom/2 + heightOffset;
                    }
                }


                //var leftMostMove = leftMost

                var moveX = ActualWidth/Zoom/2 - MoveGraphTransform.X - newLocation.X;
                var moveY = ActualHeight/Zoom/2 - MoveGraphTransform.Y - newLocation.Y;


                MoveMap(moveX, moveY);
            });
        }

        private void TransclusionsLoadCompleted(object sender, TransclusionLoadEventArgs eventArgs)
        {
            var controller = IoC.GetInstance<ISuperGraphNodeOperations>();
            controller.TransclusionsLoadCompleted -= TransclusionsLoadCompleted;
            var node = eventArgs.Node;

            //If there is only one Map Container Node then the node isn't transcluded
            if (eventArgs.MapContainerNodes != null && eventArgs.MapContainerNodes.Count > 1)
            {
                //Filter out the current map context to have the transclusions only
                var filteredContainers = new Dictionary<Guid, string>();
                foreach (KeyValuePair<Guid, string> mapContainerNode in eventArgs.MapContainerNodes)
                {
                    if (Context != null && Context.Proxy != null)
                    {
                        if (Context.Proxy.Id != mapContainerNode.Key)
                        {
                            filteredContainers.Add(mapContainerNode.Key, mapContainerNode.Value);
                        }
                    }
                    else
                    {
                        DebugLogger.Instance.LogMsg(
                            "Context or Context.Proxy was null when a transcluded node was selected.");
                    }
                }
                if (filteredContainers.Count > 0)
                {
                    var json = SerializeToJsonDictionary(filteredContainers, node.DomainId, node.Proxy.Id);
                    RelatedContentPanelUtil.Instance.ShowRelatedMaps(json);
                }
                else
                {
                    //if there are no maps that it is within clear the related maps panel from view
                    RelatedContentPanelUtil.Instance.ClearRelatedMaps();
                }
            }
            else
            {
                //if it's not transcluded clear the related maps side panel
                RelatedContentPanelUtil.Instance.ClearRelatedMaps();
            }
        }

        /// <summary>
        ///     Lightweight helper method to serialise a Dictionary into a JSON object to pass to a JavaScript function
        /// </summary>
        private string SerializeToJsonDictionary(IEnumerable<KeyValuePair<Guid, string>> dictionary, Guid domainId,
            Guid selectedNodeId)
        {
            var json = new StringBuilder("[");
            var count = 0;
            foreach (KeyValuePair<Guid, string> mapNode in dictionary)
            {
                count++;
                json.Append("{");
                json.Append(
                    string.Format("\"MapNodeId\":\"{0}\",\"Name\":\"{1}\",\"DomainId\":\"{2}\",\"NodeId\":\"{3}\"",
                        mapNode.Key, Uri.EscapeDataString(mapNode.Value), domainId, selectedNodeId));
                json.Append("}");
                if (count < dictionary.Count())
                {
                    json.Append(",");
                }
            }
            json.Append("]");
            return json.ToString();
        }

        private void SuperGraphControl_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Selector.NodeControls.Count > 0 && e.NewSize.Width < e.PreviousSize.Width && Context != null)
            {
                var rightMost = Selector.NodeControls.Max(q => q.Centre.X);
                if (rightMost > e.NewSize.Width - MoveGraphTransform.X)
                {
                    MoveMap(e.NewSize.Width - e.PreviousSize.Width, 0);
                }
            }
        }

        public List<NodeControl> SearchNodeControlsByName(string searchTerm)
        {
            var output = new List<NodeControl>();
            foreach (NodeControl node in GetNodeControls())
            {
                var originText = node.ViewModelNode.Name;
                if (originText.IndexOf(searchTerm, StringComparison.InvariantCultureIgnoreCase) > 0)
                {
                    output.Add(node);
                    var highlightRegex = new Regex("<Run Foreground='Yellow'>(.*?)</Run>");
                    TextBlockProperties.SetStyledText(node.NodeText, highlightRegex.Replace(originText, "$1"));
                    var searchRegex = new Regex(string.Format("({0})", searchTerm), RegexOptions.IgnoreCase);
                    TextBlockProperties.SetStyledText(node.NodeText,
                        searchRegex.Replace(originText, "<Run Foreground='Yellow'>$1</Run>"));
                }
            }
            return output;
        }

        private void ContextMenuContainerOnCreateNewNodeClicked(object sender, CreateNewNodeClickedEventArgs e)
        {
            Focus();
            switch (e.NodeType)
            {
                case "Map":
                    IoC.GetInstance<ISuperGraphNodeFactory>().AddMap(e.Location);
                    break;
                case "Pro":
                    IoC.GetInstance<ISuperGraphNodeFactory>().AddPro(e.Location);
                    break;
                case "Question":
                    IoC.GetInstance<ISuperGraphNodeFactory>().AddQuestion(e.Location);
                    break;
                case "Con":
                    IoC.GetInstance<ISuperGraphNodeFactory>().AddCon(e.Location);
                    break;
                case "Note":
                    IoC.GetInstance<ISuperGraphNodeFactory>().AddNote(e.Location);
                    break;
                case "Decision":
                    IoC.GetInstance<ISuperGraphNodeFactory>().AddDecision(e.Location);
                    break;
                case "Idea":
                    IoC.GetInstance<ISuperGraphNodeFactory>().AddIdea(e.Location);
                    break;
            }
        }

        #region AuthorMode Mouse Drag

        private void Pointer_OnLeftButtonDragReleased(object sender, MouseManagerEventArgs e)
        {
            LeftDragStarted = false;
            NodeContainer.ReleaseMouseCapture();
            Selector.MouseManagerOnLeftButtonDragReleased(sender, e);
        }


        private void Pointer_OnLeftButtonDragMove(object sender, MouseEventArgs e)
        {
            Selector.MouseManagerOnLeftButtonDragMove(sender, e);
        }

        private void Pointer_OnLeftButtonDragStart(object sender, MouseManagerEventArgs e)
        {
            LeftDragStarted = true;
            NodeContainer.CaptureMouse();
            Selector.MouseManagerOnLeftButtonDragStart(sender, e);
        }

        private void Pointer_OnRightButtonDragReleased(object sender, MouseManagerEventArgs e)
        {
            RightDragStarted = false;
            SuperCursor.SetCursorTemplate(this, null);
        }

        private void Pointer_OnRightButtonDragMove(object sender, MouseEventArgs e)
        {
            if (RightDragStarted)
            {
                var endPoint = e.GetPosition(NodeContainer);
                var xDistance = endPoint.X - _exploreStartLocation.X;
                var yDistance = endPoint.Y - _exploreStartLocation.Y;
                _exploreStartLocation = endPoint;
                MoveMap(xDistance, yDistance);
            }
        }

        private void Pointer_OnRightButtonDragStart(object sender, MouseManagerEventArgs e)
        {
            Focus();
            SuperCursor.SetCursorTemplate(this, ((DataTemplate) Application.Current.Resources["HoldHand"]));
            RightDragStarted = true;
            _exploreStartLocation = e.Start.PositionInformation.GetPosition(NodeContainer);
        }

        #endregion

        #region ReaderMode Events

        private void Explorer_OnLeftButtonSingleClick(object sender, MouseManagerEventArgs mouseManagerEventArgs)
        {
            Selector.Clear();
            Focus();
        }

        #endregion

        #region AuthorMode Events

        private void Pointer_OnLeftButtonSingleClick(object sender, MouseManagerEventArgs mouseManagerEventArgs)
        {
            Focus();
            if (LeftDragStarted)
            {
                Pointer_OnLeftButtonDragReleased(this, new MouseManagerEventArgs());
            }
            else
            {
                Selector.Clear();
            }
        }

        private void Pointer_RightButtonSingleClick(object sender, MouseManagerEventArgs e)
        {
            if (IoC.GetInstance<ArrowCreationManager>().InMotion || RightDragStarted)
            {
                return;
            }

            var location = e.End.PositionInformation.GetPosition(this);
            ContextMenuContainer.ShowContextMenu<SuperGraphControlContextMenu>(location);
            ContextMenuContainer.ContextMenu.Location = LastLocation;
        }

        private void Pointer_RightButtonDragReleased(object sender, MouseManagerEventArgs e)
        {
            if (IoC.GetInstance<ArrowCreationManager>().InMotion)
            {
                IoC.GetInstance<ArrowCreationManager>().SetTo(null);
            }
            else if (RightDragStarted)
            {
                Pointer_OnRightButtonDragReleased(sender, new MouseManagerEventArgs());
            }
        }

        private void Pointer_MouseMove(object sender, MouseEventArgs e)
        {
            _lastLocation = e.GetPosition(this);
            if (!Zoom.Equals(1))
            {
                _lastLocation.X = _lastLocation.X/Zoom;
                _lastLocation.Y = _lastLocation.Y/Zoom;
            }
        }

        #endregion

        #region Node Context Menu Events

        private void OnChangeNodeTypeClicked(object sender, ChangeNodeTypeEventArgs e)
        {
            var nodeControl = sender as NodeControl;

            if (nodeControl != null && nodeControl.ViewModelNode != null)
            {
                IoC.GetInstance<ISuperGraphNodeFactory>().UpdateNode(nodeControl.ViewModelNode, e.ChangedTo);
            }
        }

        private void OnCutClicked(object sender, EventArgs e)
        {
            var nodeControl = sender as NodeControl;

            if (nodeControl == null)
            {
                return;
            }
            IoC.GetInstance<ISuperGraphRelationshipFactory>().IsSameDomain = true;
            ContextMenuContainer.ContextMenu.OperationDetails = new NodeOperationDetails(NodeOperation.Cut, Context,
                nodeControl.ViewModelNode);
        }

        private void OnCopyClicked(object sender, EventArgs e)
        {
            var nodeControl = sender as NodeControl;

            if (nodeControl == null)
            {
                return;
            }
            IoC.GetInstance<ISuperGraphRelationshipFactory>().IsSameDomain = true;
            ContextMenuContainer.ContextMenu.OperationDetails = new NodeOperationDetails(NodeOperation.Copy, Context,
                nodeControl.ViewModelNode);
        }

        private void OnCloneClicked(object sender, EventArgs e)
        {
            var nodeControl = sender as NodeControl;

            if (nodeControl != null)
            {
                IoC.GetInstance<ISuperGraphNodeFactory>().CloneNode(nodeControl.ViewModelNode);
            }
        }

        #endregion

        #region Key Shortcuts

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Left || e.Key == Key.Right)
            {
                double x = 0, y = 0;
                switch (e.Key)
                {
                    case Key.Down:
                        y = -10/Zoom;
                        break;
                    case Key.Up:
                        y = 10/Zoom;
                        break;
                    case Key.Left:
                        x = 10/Zoom;
                        break;
                    case Key.Right:
                        x = -10/Zoom;
                        break;
                }
                MoveMap(x, y);
            }
            else
            {
                if (App.UserStyle == UserStyle.Reader) return;
                switch (e.Key)
                {
                    case Key.Ctrl:
                        e.Handled = true;
                        break;
                    case Key.Shift:
                        e.Handled = true;
                        break;
                }
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (App.UserStyle == UserStyle.Reader && e.Key != Key.Ctrl && e.Key != Key.Shift && e.Key != Key.E && e.Key != Key.S)
            {
                //If the user is in Reader mode (Explorer) then only toggle between Explorer and Author (Ctrl+Shift+E) and
                //the screen capture (Ctrl+Alt+S) will work in this mode as well as Author mode.
                return;
            }
            switch (e.Key)
            {
                case Key.A:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        foreach (NodeControl nodeControl in GetVisibleNodeControls())
                        {
                            Selector.AddNode(nodeControl, false);
                            nodeControl.Focus();
                        }
                    }
                    break;
                case Key.C:
                    IoC.GetInstance<ISuperGraphNodeFactory>().AddCon(LastLocation);
                    break;
                case Key.I:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        var dlg = new OpenFileDialog();

                        dlg.Multiselect = true;
                        dlg.Filter = "Xml Files (*.xml)|*.xml|Zip Files (*.zip)|*.zip|All Files (*.*)|*.*";

                        var result = dlg.ShowDialog();

                        // Process open file dialog box results 
                        if (result == true && FilesDropped != null)
                        {
                            FilesDropped(this, new FilesDroppedEventArgs {DroppedFiles = dlg.Files.ToArray()});
                        }
                    }
                    else
                    {
                        IoC.GetInstance<ISuperGraphNodeFactory>().AddIdea(LastLocation);
                    }
                    break;
                case Key.M:
                    IoC.GetInstance<ISuperGraphNodeFactory>().AddMap(LastLocation);
                    break;
                case Key.P:
                    IoC.GetInstance<ISuperGraphNodeFactory>().AddPro(LastLocation);
                    break;
                case Key.Q:
                    IoC.GetInstance<ISuperGraphNodeFactory>().AddQuestion(LastLocation);
                    break;
                case Key.Subtract:
                    IoC.GetInstance<ISuperGraphNodeFactory>().AddCon(LastLocation);
                    break;
                case Key.N:
                    IoC.GetInstance<ISuperGraphNodeFactory>().AddNote(LastLocation);
                    break;
                case Key.D:
                    IoC.GetInstance<ISuperGraphNodeFactory>().AddDecision(LastLocation);
                    break;
                case Key.R:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                        (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        HorizontalRealign();
                    }
                    break;
                case Key.V:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        {
                            VerticalRealign();
                        }
                        else if (ContextMenuContainer.ContextMenu.OperationDetails != null &&
                                 ContextMenuContainer.ContextMenu.OperationDetails.Nodes != null)
                        {
                            ContextMenuContainer.ContextMenu.Paste(_lastLocation);
                        }
                    }
                    break;
                case Key.T:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                        (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        HorizontalRealign();
                    }
                    break;
                case Key.B:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                        (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        VerticalRealign();
                    }
                    break;
                case Key.E:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                        (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        if (!ExplorerOnly)
                        {
                            if (App.UserStyle == UserStyle.Reader)
                            {
                                UserStyle = UserStyle.Author;
                            }
                            else if (App.UserStyle == UserStyle.Author)
                            {
                                UserStyle = UserStyle.Reader;
                            }
                        }
                    }
                    break;
                case Key.X:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                        (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
                    }
                    break;
                case Key.S:
                    e.Handled = true;
                    if (((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) &&
                        ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt))
                    {
                        ScreenCaptureUtility.Instance.SaveToImage(this);
                    }
                    break;
                case Key.Ctrl:
                    e.Handled = true;
                    break;
                case Key.Shift:
                    e.Handled = true;
                    break;
            }
        }

        #endregion

        #region Mouse Wheel

        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true;
            double x = 0, y = 0;
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                var previousLocation = _lastLocation;
                var old = Zoom;
                if (e.Delta > 0 && Zoom < 10)
                {
                    ZoomIn();
                }
                else if (e.Delta < 0 && Zoom > 0.1)
                {
                    ZoomOut();
                }

                if (!Zoom.Equals(old))
                {
                    y = _lastLocation.Y - previousLocation.Y;
                    x = _lastLocation.X - previousLocation.X;


                    if (ContextMenuContainer.ContextMenuVisibility == Visibility.Visible)
                    {
                        ContextMenuContainer.ContextMenuVisibility = Visibility.Collapsed;
                    }
                    MoveMap(x, y);
                }
            }
            else
            {
                if (e.Delta > 0)
                {
                    y = 30/Zoom;
                }
                else if (e.Delta < 0)
                {
                    y = -30/Zoom;
                }
                MoveMap(x, y);
            }
        }

        #endregion

        #region Other Events

        private void OnNodeClicked(object sender, NodeClickedEventArgs e)
        {
            if (NodeClicked != null)
            {
                NodeClicked(sender, e);
            }
        }

        private void SuperGraphControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void OnFileDragAndDrop(object sender, DragEventArgs e)
        {
            if (App.UserStyle == UserStyle.Reader) return;
            var fileData = e.Data.GetData(DataFormats.FileDrop);
            var files = fileData as FileInfo[];

            if (FilesDropped != null)
            {
                FilesDropped(this, new FilesDroppedEventArgs {DroppedFiles = files});
            }
        }

        #endregion

        #region ReaderMode Mouse Drag

        public void Explorer_OnLeftButtonDragReleased(object sender, MouseManagerEventArgs e)
        {
            SuperCursor.SetCursorTemplate(this, null);
            DragRelease();
        }

        public void Explorer_OnLeftButtonDragMove(object sender, MouseEventArgs e)
        {
            var endPoint = e.GetPosition(NodeContainer);
            var xDistance = endPoint.X - _exploreStartLocation.X;
            var yDistance = endPoint.Y - _exploreStartLocation.Y;
            _exploreStartLocation = endPoint;
            MoveMap(xDistance, yDistance);
        }

        public void Explorer_OnLeftButtonDragStart(object sender, MouseManagerEventArgs e)
        {
            SuperCursor.SetCursorTemplate(this, ((DataTemplate) Application.Current.Resources["HoldHand"]));
            DragStart(e);
        }


        public void DragStart(MouseManagerEventArgs e)
        {
            NodeContainer.CaptureMouse();
            _exploreStartLocation = e.Start.PositionInformation.GetPosition(NodeContainer);
        }

        public void DragRelease()
        {
            NodeContainer.ReleaseMouseCapture();
        }

        #endregion
    }
}