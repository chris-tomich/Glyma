using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using SilverlightMappingToolBasic.UI.Extensions.CookieManagement;
using SilverlightMappingToolBasic.UI.SuperGraph.Controller;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Arrow;
using SilverlightMappingToolBasic.UI.SuperGraph.View.CollapseSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.ContextMenu;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MessageBox;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.SuperGraph.View.MouseCursor;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses.Interface;
using SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using SimpleIoC;
using Clipboard = System.Windows.Clipboard;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses
{
    public partial class NodeControl : UserControl, ISelectableNode, ICutableNode, IPolymorphicShape,IDisposable
    {
        private bool _isReferenceNode;

        private Point _location;
        private IIoCContainer _ioc;
        private FacadeNodeMotionHandler _motionHandler;
        private SuperMouseManager _nodeImageClickManager;
        private SuperMouseManager _nodeTextClickManager;
        private EllipseEquation _ellipse;
        private RectEquation _rect;

        private List<NodeControl> _childNodes;
        private List<NodeControl> _parentNodes;

        public event EventHandler CutClicked;
        public event EventHandler CopyClicked;
        public event EventHandler CloneClicked;
        //public event EventHandler Deleted;
        public event EventHandler<NodeSelectedEventArgs> NodeSelected;
        //public event EventHandler<NodeOperationEventArgs> DeleteClicked;
        public event EventHandler<ShapeMovedEventArgs> ShapeMoved;
        public event EventHandler<NodeClickedEventArgs> NodeClicked;
        public event EventHandler<ChangeNodeTypeEventArgs> ChangeNodeTypeClicked;


        public NodeControl()
        {
            InitializeComponent();
            ((ICutableNode)this).ReleaseCutState();
            CollapseControl.NodeControl = this;
            
        }

        #region MouseManager
        private SuperMouseManager NodeImageMouseManager
        {
            get
            {
                if (_nodeImageClickManager == null)
                {
                    _nodeImageClickManager = new SuperMouseManager(NodeImage);
                }

                return _nodeImageClickManager;
            }
        }

        private SuperMouseManager NodeTextMouseManager
        {
            get
            {
                if (_nodeTextClickManager == null)
                {
                    _nodeTextClickManager = new SuperMouseManager(NodeText);
                }

                return _nodeTextClickManager;
            }
        }

        private IMouseManager NodeImageExplorerMouseManager
        {
            get
            {
                return NodeImageMouseManager.ExplorerMouseManager;
            }
        }

        private IMouseManager NodeImagePointerMouseManager
        {
            get
            {
                return NodeImageMouseManager.PointerMouseManager;
            }
        }

        private IMouseManager NodeTextExplorerMouseManager
        {
            get
            {
                return NodeTextMouseManager.ExplorerMouseManager;
            }
        }

        private IMouseManager NodeTextPointerMouseManager
        {
            get
            {
                return NodeTextMouseManager.PointerMouseManager;
            }
        }

        #endregion

        private double NodeControlRadius
        {
            get
            {
                double radius;

                if (NodeText.ActualWidth >= GlymaParameters.NodeImageWidth + 5)
                {
                    if (NodeText.ActualWidth > GlymaParameters.NodeTextWidth)
                    {
                        NodeText.UpdateLayout();
                        radius = NodeText.ActualWidth > GlymaParameters.NodeTextWidth ? GlymaParameters.NodeTextWidth / 2 : NodeText.ActualWidth / 2;
                    }
                    else
                    {
                        radius = NodeText.ActualWidth / 2;
                    }
                    
                }
                else
                {
                    radius = GlymaParameters.NodeImageWidth + 5 / 2;
                }

                if ((GlymaParameters.NodeImageHeight + 5) > radius)
                {
                    radius = GlymaParameters.NodeImageHeight + 5;
                }

                return radius;
            }
        }

        public bool IsLoading { get; private set; }

        private EllipseEquation Ellipse
        {
            get
            {
                if (_ellipse == null)
                {
                    _ellipse = new EllipseEquation(Centre, NodeControlRadius, GlymaParameters.NodeImageHeight);
                }
                else
                {
                    _ellipse.Centre = Centre;
                    _ellipse.HorizontalRadius = NodeControlRadius;
                    _ellipse.VerticalRadius = GlymaParameters.NodeImageHeight;
                }

                return _ellipse;
            }
        }

        private RectEquation Rect
        {
            get
            {
                double width = NodeControlRadius * 2;
                double height = NodeText.ActualHeight * 2 + GlymaParameters.NodeImageHeight;

                if (_rect == null)
                {
                    _rect = new RectEquation(Centre, width, height);
                }
                else
                {
                    _rect.Centre = Centre;
                    _rect.Width = width;
                    _rect.Height = height;
                }

                return _rect;
            }
        }

        private int OriginalZIndex
        {
            get;
            set;
        }

        public Uri Link { get; set; }

        public List<NodeControl> ChildNodes
        {
            get
            {
                if (_childNodes == null)
                {
                    _childNodes = new List<NodeControl>();
                }
                return _childNodes;
            }
        }

        public List<NodeControl> ParentNodes
        {
            get
            {
                if (_parentNodes == null)
                {
                    _parentNodes = new List<NodeControl>();
                }
                return _parentNodes;
            }
        }

        #region UserStyle
        public UserStyle UserStyle
        {
            set
            {
                switch (value)
                {
                    case UserStyle.Reader:
                        NodeText.Cursor = IsReferenceNode ? Cursors.Hand : Cursors.Arrow;
                        NodeImage.Cursor = ViewModelNode.IsMapNode ? Cursors.Hand : Cursors.Arrow;
                        //VideoRemoteControl.Cursor = ViewModelNode.VideoInfo != null ? Cursors.Arrow : Cursors.Hand;
                        NodeTextTip.IsEnabled = false;
                        NodeTextTip.Visibility = Visibility.Collapsed;
                        ToolTip.Text = "Click to open the link in new tap.";
                        break;
                    case UserStyle.Author:
                        NodeText.Cursor = IsReferenceNode ? Cursors.Hand : Cursors.Arrow;
                        NodeImage.Cursor = ViewModelNode.IsMapNode ? Cursors.Hand : Cursors.Arrow;
                        //VideoRemoteControl.Cursor = ViewModelNode.VideoInfo != null ? Cursors.Hand : Cursors.Arrow;
                        NodeTextTip.IsEnabled = true;
                        NodeTextTip.Visibility = IsReferenceNode ? Visibility.Visible : Visibility.Collapsed;
                        ToolTip.Text = "Double click to open the link in new tap.";
                        break;
                }
            }
        }
        #endregion

        public bool IsReferenceNode
        {
            get
            {
                return _isReferenceNode;
            }
            set
            {
                _isReferenceNode = value;
                NodeText.TextDecorations = _isReferenceNode ? TextDecorations.Underline : null;
                NodeText.Foreground = new SolidColorBrush(_isReferenceNode? Colors.Blue : Colors.Black);
                NodeText.Cursor = _isReferenceNode ? Cursors.Hand : Cursors.Arrow;
                NodeTextTip.Visibility = _isReferenceNode ? Visibility.Visible : Visibility.Collapsed;
                NodeTextTip.IsEnabled = _isReferenceNode;
            }
        }

        public ContextMenuContainer ContextMenuContainer
        {
            get
            {
                return ParentSurface.ContextMenuContainer;
            }
        }

        public ViewerNodeControlContextMenu ViewerNodeControlContextMenu
        {
            get
            {
                return ((ISelectorControl)ParentSurface).ViewerNodeControlContextMenu;
            }
        }

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

        public bool IsCollapsed
        {
            get
            {
                return ViewModelNode.Visibility == Visibility.Collapsed;
            }
            set
            {
                if ((ViewModelNode.Visibility == Visibility.Collapsed) != value)
                {
                    ViewModelNode.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
                }
            }
        }

        public FacadeNodeMotionHandler MotionHandler
        {
            get
            {
                if (_motionHandler == null && ParentSurface != null)
                {
                    _motionHandler = new FacadeNodeMotionHandler(this);
                    NodeImagePointerMouseManager.LeftButtonDragStart += _motionHandler.OnLeftButtonDragStart;
                    NodeImagePointerMouseManager.LeftButtonDragReleased += _motionHandler.OnLeftButtonDragReleased;
                    NodeImagePointerMouseManager.LeftButtonDragMove += _motionHandler.OnLeftButtonDragMove;
                    NodeImagePointerMouseManager.LeftButtonSingleClick += NodeImage_LeftButtonSingleClick;
                    NodeImagePointerMouseManager.LeftButtonDoubleClick += NodeImage_LeftButtonDoubleClick;
                    NodeImagePointerMouseManager.RightButtonDragStart += NodeImage_RightButtonDragStart;
                    NodeImagePointerMouseManager.RightButtonDragReleased += NodeImage_RightButtonDragReleased;
                    NodeImagePointerMouseManager.RightButtonSingleClick += NodeImage_RightButtonSingleClick;

                    //NodeImageExplorerMouseManager.LeftButtonDragStart += _motionHandler.OnExplorerLeftButtonDragStart;
                    //NodeImageExplorerMouseManager.LeftButtonDragReleased += _motionHandler.OnExplorerLeftButtonDragReleased;
                    //NodeImageExplorerMouseManager.LeftButtonDragMove += _motionHandler.OnExplorerLeftButtonDragMove;
                    NodeImageExplorerMouseManager.LeftButtonDoubleClick += NodeImage_LeftButtonDoubleClick;
                    NodeImageExplorerMouseManager.LeftButtonSingleClick += NodeImage_ExplorerModeLeftButtonSingleClick;
                    NodeImageExplorerMouseManager.RightButtonSingleClick += NodeImageExplorer_RightButtonSingleClick;

                    //NodeTextExplorerMouseManager.LeftButtonDragStart += _motionHandler.OnExplorerTextLeftButtonDragStart;
                    //NodeTextExplorerMouseManager.LeftButtonDragReleased += _motionHandler.OnExplorerTextLeftButtonDragReleased;
                    //NodeTextExplorerMouseManager.LeftButtonDragMove += _motionHandler.OnExplorerLeftButtonDragMove;
                    NodeTextPointerMouseManager.LeftButtonSingleClick += NodeText_LeftButtonSingleClick;
                    NodeTextPointerMouseManager.LeftButtonDoubleClick += NodeTextMouseManagerOnLeftButtonDoubleClick;
                    NodeTextExplorerMouseManager.LeftButtonSingleClick += NodeTextMouseManagerOnLeftButtonDoubleClick;
                    NodeTextExplorerMouseManager.RightButtonDragReleased += NodeImage_RightButtonDragReleased;
                }

                return _motionHandler;
            }
        }

        public CollapseState CollapseState {
            get { return CollapseControl.State; }
        }

        public SuperGraphControl ParentSurface
        {
            get;
            set;
        }

        public Node ViewModelNode
        {
            get
            {
                return DataContext as Node;
            }
        }

        private void NodeControl_Loaded(object sender, RoutedEventArgs e)
        {
            MotionHandler.NodeInMotionEvent += MotionHandler_NodeInMotionEvent;
            Centre = ViewModelNode.Location;
            ViewModelNode.PropertyChanged += ViewModelNodeOnPropertyChanged;
            UserStyle = App.UserStyle;
            OriginalZIndex = Canvas.GetZIndex(this);
            VideoRemoteControl.VisibilityChanged += VideoRemoteControlOnVisibilityChanged;
            VideoRemoteControl.PlayClicked += VideoRemoteControlOnPlayClicked;
            RefreshCornerControl();
        }

        private void VideoRemoteControlOnPlayClicked(object sender, EventArgs eventArgs)
        {
            if (CollapseState == CollapseState.Collapsed || CollapseState == CollapseState.SemiCollapsed)
            {
                CollapseControl.ExpandNodes(new ChildrenControlCollection(this, false, true));
            }
            if (ViewModelNode.NodeActionOptions.ShowRelatedContentWithVideo)
            {
                NodeCornerControlOnNodeSelected(sender, new NodeSelectedEventArgs { Type = NodeSelectedEventArgs.NodeSelectedType.RelatedContent, Source = NodeSelectedEventArgs.ClickSource.CornerButton });
            }
        }

        private void ViewModelNodeOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                if (ShapeMoved != null)
                {
                    ShapeMoved(this, new ShapeMovedEventArgs { Location = Centre });
                }
            }
        }

        private void VideoRemoteControlOnVisibilityChanged(object sender, VisibilityChangedEventArgs visibilityChangedEventArgs)
        {
            RefreshCornerControl();
        }

        private void RefreshCornerControl()
        {
            if (ViewModelNode == null) return;

            var count = 0;
            if (ViewModelNode.VideoInfo != null && ViewModelNode.VideoInfo.HasVideo)
            {
                VideoRemoteControl.Visibility = Visibility.Visible;
                count++;
            }

            if (ViewModelNode.DescriptionType != NodeDescriptionType.None)
            {
                if (count == 0)
                {
                    CornerButton.Visibility = Visibility.Visible;
                    var buttonViewModel = new NodeCornerButtonViewModel { ButtonType = NodeCornerButtonType.Content };
                    NodeCornerButtonLoader.DressButton(buttonViewModel);
                    CornerButton.DataContext = buttonViewModel;
                }
                count++;
            }
            else
            {
                NodeCornerButtonViewModel buttonViewModel = CornerButton.DataContext as NodeCornerButtonViewModel;
                if (buttonViewModel != null && buttonViewModel.ButtonType == NodeCornerButtonType.Content && CornerButton.Visibility == Visibility.Visible)
                {
                    CornerButton.Visibility = Visibility.Collapsed;
                }
            }

            if (ViewModelNode.IsTranscluded)
            {
                if (count == 0)
                {
                    CornerButton.Visibility = Visibility.Visible;
                    var viewmodel = new NodeCornerButtonViewModel { ButtonType = NodeCornerButtonType.Map };
                    NodeCornerButtonLoader.DressButton(viewmodel);
                    CornerButton.DataContext = viewmodel;
                }
                count++;

            }

            if (count > 1)
            {
                MultiButtonIndicator.Visibility = Visibility.Visible;
            }
            else
            {
                MultiButtonIndicator.Visibility = Visibility.Collapsed;
            }
        }

        private void NodeCornerControlOnNodeSelected(object sender, NodeSelectedEventArgs e)
        {
            if (CollapseControl.State == CollapseState.Collapsed ||
                CollapseControl.State == CollapseState.SemiCollapsed)
            {
                CollapseControl.ExpandNodes(new ChildrenControlCollection(this, false, true));
            }
            SelectNodeDetail(e.Type, e.Source);
        }

        private void CornerButton_OnButtonClicked(object sender, NodeCornerButtonClickedEventArgs e)
        {
            ParentSurface.Selector.Clear();
            ParentSurface.Selector.AddNode(this, false);
            switch (e.Type)
            {
                 case NodeCornerButtonType.Content:
                    NodeCornerControlOnNodeSelected(sender, new NodeSelectedEventArgs{Type = NodeSelectedEventArgs.NodeSelectedType.RelatedContent, Source = NodeSelectedEventArgs.ClickSource.CornerButton });
                    break;
                 case NodeCornerButtonType.Map:
                    NodeCornerControlOnNodeSelected(sender, new NodeSelectedEventArgs { Type = NodeSelectedEventArgs.NodeSelectedType.RelatedMap, Source = NodeSelectedEventArgs.ClickSource.CornerButton });
                    break;
                 case NodeCornerButtonType.Feed:
                    NodeCornerControlOnNodeSelected(sender, new NodeSelectedEventArgs { Type = NodeSelectedEventArgs.NodeSelectedType.ActiveFeed, Source = NodeSelectedEventArgs.ClickSource.CornerButton });
                    break;
                 case NodeCornerButtonType.Play:
                    NodeCornerControlOnNodeSelected(sender, new NodeSelectedEventArgs { Type = NodeSelectedEventArgs.NodeSelectedType.Video, Source = NodeSelectedEventArgs.ClickSource.CornerButton });
                    break;
                 case NodeCornerButtonType.Pause:
                    NodeCornerControlOnNodeSelected(sender, new NodeSelectedEventArgs { Type = NodeSelectedEventArgs.NodeSelectedType.Video, Source = NodeSelectedEventArgs.ClickSource.CornerButton });
                    break;
            }
        }

        private void OnExtendButtonClicked(object sender, EventArgs e)
        {
            if (CollapseState == CollapseState.Collapsed ||
                CollapseState == CollapseState.SemiCollapsed)
            {
                CollapseControl.ExpandNodes(new ChildrenControlCollection(this, false, true));
            }
            ParentSurface.Selector.Clear();
            ParentSurface.Selector.AddNode(this);
        }

        #region Context Menu Events

        public void ChangeNodeType(ChangeNodeTypeEventArgs e)
        {
            if (ChangeNodeTypeClicked != null)
            {
                ChangeNodeTypeClicked(this, e);
            }
        }

        public void ShareWith(ShareWithType item)
        {
            var nodeId = ViewModelNode.Proxy.Id;
            var domainId = ViewModelNode.Proxy.DomainId;
            var mapId = ParentSurface.Context.Proxy.Id;
            var query = HtmlPage.Document.DocumentUri.Query;
            var absoluteUri = HtmlPage.Document.DocumentUri.AbsoluteUri;
            var baseUrl = string.IsNullOrWhiteSpace(query)
                ? absoluteUri
                : absoluteUri.Replace(query, "");
            var url = string.Format("{0}?NodeUid={1}&DomainUid={2}&MapUid={3}", baseUrl, nodeId, domainId, mapId);
            item.Share(url, ParentSurface.Ref.Breadcrumbs.CurrentBreadcrumbControl.BreadcrumbData.Name);
        }

        public void CopyUrl()
        {
            if (Link != null)
            {
                Clipboard.SetText(Link.ToString());
            }
        }

        public void MenuClosed(object sender, EventArgs e)
        {
            //if (!SingleNodeContextMenu.IsContextMenuOpened() && MultiNodesContextMenu.Visibility == Visibility.Collapsed)
            //    Canvas.SetZIndex(this, OriginalZIndex);
        }

        public void CutNode()
        {
            ((ICutableNode)this).SetCutState();
            if (CutClicked != null)
            {
                CutClicked(this, null);
            }
        }

        public void Copy()
        {
            ((ICutableNode)this).ReleaseCutState();
            if (CopyClicked != null)
            {
                CopyClicked(this, null);
            }
        }

        public void Clone()
        {
            ((ICutableNode)this).ReleaseCutState();
            ParentSurface.Selector.Clear();
            if ( CloneClicked != null)
            {
                CloneClicked(this, null);
            }
        }

        public void NodeProperties()
        {
            ((ICutableNode)this).ReleaseCutState();
            ParentSurface.NodePropertiesDialog.DataContext = ViewModelNode.NodeProperties;
            ParentSurface.NodePropertiesDialog.Closed += NodePropertiesDialogOnClosed;
            ParentSurface.NodePropertiesDialog.Show();
        }


        private void NodePropertiesDialogOnClosed(object sender, EventArgs eventArgs)
        {
            ParentSurface.NodePropertiesDialog.Closed -= NodePropertiesDialogOnClosed;
            RefreshNodeControl();
        }

        #endregion

        #region Node Image Mouse Events

        private void NodeImage_ExplorerModeLeftButtonSingleClick(object sender, MouseManagerEventArgs e)
        {
            ParentSurface.Selector.Clear();
            if (ParentSurface.Selector.HasNode(this))
            {
                ParentSurface.Selector.RemoveNode(this);
            }
            else
            {
                ParentSurface.Selector.AddNode(this);
            }
        }

        private void NodeImage_LeftButtonSingleClick(object sender, MouseManagerEventArgs e)
        {
            if (!ParentSurface.LeftDragStarted)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        if (ParentSurface.Selector.HasNode(this))
                        {
                            ParentSurface.Selector.DeSelectAllChildsOfNode(this);
                        }
                        else
                        {
                            ParentSurface.Selector.SelectAllChildsOfNode(this);
                        }
                    }
                    else
                    {
                        if (ParentSurface.Selector.HasNode(this))
                        {
                            ParentSurface.Selector.RemoveNode(this);
                        }
                        else
                        {
                            ParentSurface.Selector.AddNode(this);
                        }
                    }

                }
                else if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    ParentSurface.Selector.Clear();
                    ParentSurface.Selector.SelectAllChildsOfNode(this);
                }
                else
                {
                    ParentSurface.Selector.Clear();
                    ParentSurface.Selector.AddNode(this);
                    if (string.IsNullOrEmpty(ViewModelNode.Name) && !ParentSurface.Selector.IsMultiSelect)
                    {
                        SetEditFocus();
                    }
                    else
                    {
                        Focus();
                    }
                }
            }
        }

        private void NodeImage_LeftButtonDoubleClick(object sender, MouseManagerEventArgs e)
        {
            if (IoC.GetInstance<ArrowCreationManager>().InMotion)
            {
                IoC.GetInstance<ArrowCreationManager>().SetTo(this);
            }

            if (NodeClicked != null)
            {
                NodeClicked(this, new NodeClickedEventArgs());
            }
        }

        private void NodeImage_RightButtonDragStart(object sender, MouseManagerEventArgs e)
        {
            if(ParentSurface.LeftDragStarted || ParentSurface.Selector.IsDrag) return;
            if (ParentSurface.Selector.IsMultiSelect)
            {
                if (ParentSurface.Selector.HasNode(this))
                {
                    foreach (var nodeControl in ParentSurface.Selector.NodeControls)
                    {
                        IoC.GetInstance<ArrowCreationManager>().SetFrom(nodeControl as NodeControl);
                    }
                }
                else
                {
                    ParentSurface.Selector.Clear();
                    ParentSurface.Selector.AddNode(this);
                    IoC.GetInstance<ArrowCreationManager>().SetFrom(this); 
                }
                
            }
            else
            {
                IoC.GetInstance<ArrowCreationManager>().SetFrom(this); 
            }
        }

        private void NodeImage_RightButtonDragReleased(object sender, MouseManagerEventArgs e)
        {
            if (IoC.GetInstance<ArrowCreationManager>().InMotion)
            {
                IoC.GetInstance<ArrowCreationManager>().SetTo(this);
            }
        }

        private void NodeImage_RightButtonSingleClick(object sender, MouseManagerEventArgs e)
        {
            if (IoC.GetInstance<ArrowCreationManager>().InMotion)
            {
                IoC.GetInstance<ArrowCreationManager>().SetTo(this);
            }
            else
            {
                if (!ParentSurface.Selector.IsMultiSelect || !ViewModelNode.IsSelected)
                {
                    ParentSurface.Selector.Clear();
                    ParentSurface.Selector.AddNode(this);
                    Focus();
                }

                Point location;

                if (ParentSurface.Selector.IsMultiSelect)
                {
                    foreach (var control in ParentSurface.Selector.NodeControls)
                    {
                        if (control == this && !control.IsCollapsed)
                        {
                            location = e.End.PositionInformation.GetPosition(ParentSurface);
                            ContextMenuContainer.ShowContextMenu<MultiNodesContextMenu>(location);
                            return;
                        }
                    }
                }
                else
                {
                    location = e.End.PositionInformation.GetPosition(ParentSurface);
                    ContextMenuContainer.ShowContextMenu<AuthorNodeControlContextMenu>(location, this);
                }
            }
        }

        private void NodeImageExplorer_RightButtonSingleClick(object sender, MouseManagerEventArgs e)
        {
            ParentSurface.Selector.Clear();
            ParentSurface.Selector.AddNode(this, false);
            var location = e.End.PositionInformation.GetPosition(ParentSurface);
            ContextMenuContainer.ShowContextMenu<ViewerNodeControlContextMenu>(location, this);
            ViewerNodeControlContextMenu.Rating.DataContext = ViewModelNode.NodeProperties;
        }

        #endregion

        #region Node Text Mouse Events

        private void NodeTextMouseManagerOnLeftButtonDoubleClick(object sender, MouseManagerEventArgs mouseManagerEventArgs)
        {
            if (IsReferenceNode)
            {
                HtmlPage.Window.Navigate(Link, "_blank");
            }
        }

        private void NodeText_LeftButtonSingleClick(object sender, MouseManagerEventArgs e)
        {
            if (!ParentSurface.LeftDragStarted)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                {
                    ParentSurface.Selector.AddNode(this);
                }
                else
                {
                    ParentSurface.Selector.Clear();
                    ParentSurface.Selector.AddNode(this);
                    SetEditFocus();
                }
            }
        }

        #endregion

        #region Other Events

        private void MotionHandler_NodeInMotionEvent(object sender, NodeMotionHandlerEventArgs e)
        {
            if (ShapeMoved != null)
            {
                ShapeMoved(this, new ShapeMovedEventArgs { Location = e.Location });
            }
        }

        private void NodeControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var newPosition = new Point
            {
                X = Centre.X - Container.ActualWidth/2,
                Y = Centre.Y - GlymaParameters.NodeImageHeight
            };

            Canvas.SetLeft(this, newPosition.X);
            Canvas.SetTop(this, newPosition.Y);
        }

        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IoC.GetInstance<ArrowCreationManager>().InMotion)
            {
                IoC.GetInstance<ArrowCreationManager>().SetTo(this);
            }
        }


        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RefreshNodeControl();
            CollapseControl.DataContext = null;
            CollapseControl.DataContext = DataContext;
        }

        #endregion

        #region Key Events

        private void NodeControl_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            if (App.UserStyle == UserStyle.Reader) return;
            switch (e.Key)
            {
                case Key.Delete:
                    ParentSurface.Selector.DeleteNodes(this);
                    ParentSurface.Focus();
                    break;
                case Key.C:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        if (ParentSurface.Selector.IsMultiSelect)
                        {
                            ParentSurface.Selector.Copy();
                        }
                        else
                        {
                            Copy();
                        }
                    }
                    else if (!ParentSurface.Selector.IsMultiSelect && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        var location = Centre;
                        location.X += 200;
                        IoC.GetInstance<ISuperGraphNodeFactory>().AddLinkedNode("CompendiumConNode", location, ViewModelNode);
                    }
                    break;
                case Key.X:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        if (ParentSurface.Selector.IsMultiSelect)
                        {
                            ParentSurface.Selector.Cut();
                        }
                        else
                        {
                            CutNode();
                        }
                    }
                    break;
                case Key.P:
                    if (!ParentSurface.Selector.IsMultiSelect && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        var location = Centre;
                        location.X += GlymaParameters.NodeTextWidth + GlymaParameters.Margin * 2;
                        IoC.GetInstance<ISuperGraphNodeFactory>().AddLinkedNode("CompendiumProNode", location, ViewModelNode);
                    }
                    break;
                case Key.M:
                    if (!ParentSurface.Selector.IsMultiSelect && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        var location = Centre;
                        location.X += GlymaParameters.NodeTextWidth + GlymaParameters.Margin * 2;
                        IoC.GetInstance<ISuperGraphNodeFactory>().AddLinkedNode("CompendiumMapNode", location, ViewModelNode);
                    }
                    break;
                case Key.I:
                    if (!ParentSurface.Selector.IsMultiSelect && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        var location = Centre;
                        location.X += GlymaParameters.NodeTextWidth + GlymaParameters.Margin * 2;
                        IoC.GetInstance<ISuperGraphNodeFactory>().AddLinkedNode("CompendiumIdeaNode", location, ViewModelNode);
                    }
                    break;
                case Key.Q:
                    if (!ParentSurface.Selector.IsMultiSelect && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        var location = Centre;
                        location.X += GlymaParameters.NodeTextWidth + GlymaParameters.Margin * 2;
                        IoC.GetInstance<ISuperGraphNodeFactory>().AddLinkedNode("CompendiumQuestionNode", location, ViewModelNode);
                    }
                    break;
                case Key.N:
                    if (!ParentSurface.Selector.IsMultiSelect && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        var location = Centre;
                        location.X += GlymaParameters.NodeTextWidth + GlymaParameters.Margin * 2;
                        IoC.GetInstance<ISuperGraphNodeFactory>().AddLinkedNode("CompendiumNoteNode", location, ViewModelNode);
                    }
                    break;
                case Key.D:
                    if (!ParentSurface.Selector.IsMultiSelect && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        var location = Centre;
                        location.X += GlymaParameters.NodeTextWidth + GlymaParameters.Margin * 2;
                        IoC.GetInstance<ISuperGraphNodeFactory>().AddLinkedNode("CompendiumDecisionNode", location, ViewModelNode);
                    }
                    break;
            }
        }

        private void Container_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    e.Handled = true;
                    break;
                case Key.Right:
                    e.Handled = true;
                    break;
                case Key.Up:
                    e.Handled = true;
                    break;
                case Key.Down:
                    e.Handled = true;
                    break;
            }
        }


        #endregion

        public bool HasChildren()
        {
            return ChildNodes.Count > 0;
        }



        public void SetEditFocus()
        {
            if (!ParentSurface.NodeTextInput.IsInitialised)
            {
                ParentSurface.Ref.PreloadControls.Children.Remove(ParentSurface.NodeTextInput);
                ParentSurface.NodeTextInput.IsInitialised = true;
            }

            ParentSurface.Selector.Clear();
            ParentSurface.Selector.AddNode(this, false);

            if (ParentSurface.NodeTextInput.ParentControl == null)
            {
                TextArea.Children.Add(ParentSurface.NodeTextInput);
                ParentSurface.NodeTextInput.ParentControl = this;
            }
            else
            {
                ParentSurface.NodeTextInput.FinishEdit();
                TextArea.Children.Add(ParentSurface.NodeTextInput);
                ParentSurface.NodeTextInput.ParentControl = this;
            }
        }

        public void GiveEditorFocus()
        {
            ParentSurface.NodeTextInput.Editor.Focus();
        }

        //public void Delete(bool isContextMenuOperation)
        //{
        //    if (ViewModelNode.IsSelected || isContextMenuOperation)
        //    {
        //        var eventArgs = new NodeOperationEventArgs();

        //        if (ViewModelNode.IsTranscluded && ViewModelNode.NumTranclusions > 1)
        //        {
        //            eventArgs.Operation = NodeOperation.DeleteTransclusion;
        //        }
        //        else
        //        {
        //            eventArgs.Operation = NodeOperation.DeleteNode;
        //        }

        //        if (DeleteClicked != null)
        //        {
        //            DeleteClicked(this, eventArgs);
        //        }
        //    }
        //}

        /// <summary>
        /// Gets and sets the location of the node. The point given will be the centre of the node.
        /// </summary>
        public Point Centre
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
                var newPosition = new Point
                {
                    X = _location.X - Container.ActualWidth/2,
                    Y = _location.Y - GlymaParameters.NodeImageHeight
                };

                Canvas.SetLeft(this, newPosition.X);
                Canvas.SetTop(this, newPosition.Y);

                if (ShapeMoved != null)
                {
                    ShapeMoved(this, new ShapeMovedEventArgs { Location = _location });
                }
            }
        }

        public IShapeEquation GetShapeLeftMost()
        {
            return Rect;
        }

        public IShapeEquation GetShapeRightMost()
        {
            return Rect;
        }

        public IShapeEquation GetShapeLowest()
        {
            return Rect;
        }

        public IShapeEquation GetShapeHighest()
        {
            return Ellipse;
        }

        void ISelectableNode.DeSelect()
        {
            ViewModelNode.IsSelected = false;
            ((ICutableNode)this).ReleaseCutState();
            ((DropShadowEffect) NodeImage.Effect).BlurRadius = 0;
            ParentSurface.NodeTextInput.UnlinkNodeControl();
            ParentSurface.NodeDetailsControl.UnlinkParentControl();
            Canvas.SetZIndex(this, OriginalZIndex);
        }

        void ISelectableNode.Select(bool isShowDetail = true)
        {
            ViewModelNode.IsSelected = true;
            ((ICutableNode)this).ReleaseCutState();
            ((DropShadowEffect)NodeImage.Effect).BlurRadius = 100;
            Canvas.SetZIndex(this, 1000);
            Focus();
            if (!ParentSurface.NodeDetailsControl.IsInitialised)
            {
                ParentSurface.Ref.PreloadControls.Children.Remove(ParentSurface.NodeDetailsControl);
                ParentSurface.NodeDetailsControl.IsInitialised = true;
            }

            if (ParentSurface.NodeDetailsControl.ParentControl == null)
            {
                LayoutRoot.Children.Add(ParentSurface.NodeDetailsControl);
                ParentSurface.NodeDetailsControl.ParentControl = this;
            }
            else
            {
                ParentSurface.NodeDetailsControl.UnlinkParentControl();
                LayoutRoot.Children.Add(ParentSurface.NodeDetailsControl);
                ParentSurface.NodeDetailsControl.ParentControl = this;
            }

            //Select the context aware panels - only the Related Maps panel
            // This will clear the related maps panel from view if no related maps or update the related maps panel
            if (isShowDetail)
            {
                SelectNodeDetail(NodeSelectedEventArgs.NodeSelectedType.RelatedMap, NodeSelectedEventArgs.ClickSource.Node);
            }

            //if (NodeSelected != null && isShowDetail)
            //{
            //    NodeSelected(this, new NodeSelectedEventArgs { SelectedNode = ViewModelNode, Type = NodeSelectedEventArgs.NodeSelectedType.All} );
            //}
        }

        public void SelectNodeDetail(NodeSelectedEventArgs.NodeSelectedType type, NodeSelectedEventArgs.ClickSource source)
        {
            if (NodeSelected != null)
            {
                NodeSelected(this, new NodeSelectedEventArgs { SelectedNode = ViewModelNode, Type = type, Source = source });
            }
        }

        void ICutableNode.SetCutState()
        {
            NodeImage.Opacity = 0.6;
            NodeText.Opacity = 0.6;

        }

        void ICutableNode.ReleaseCutState()
        {
            NodeImage.Opacity = 1;
            NodeText.Opacity = 1;
        }

        public void RefreshNodeControl()
        {
            var node = DataContext as Node;
            if (node != null)
            {
                if (node.Metadata.ContainsKey("Link") && !string.IsNullOrEmpty(node.Metadata["Link"].Value))
                {
                    Uri uri;
                    if (Uri.TryCreate(node.Metadata["Link"].Value, UriKind.RelativeOrAbsolute, out uri))
                    {
                        IsReferenceNode = true;
                        Link = uri;
                    }
                }
                else
                {
                    IsReferenceNode = false;
                }

                RefreshCornerControl();
                if (ParentSurface.NodeDetailsControl.IsInitialised && ParentSurface.NodeDetailsControl.ParentControl == this && ParentSurface.NodeDetailsControl.Visibility == Visibility.Visible)
                {
                    ParentSurface.NodeDetailsControl.UnlinkParentControl();
                    LayoutRoot.Children.Add(ParentSurface.NodeDetailsControl);
                    ParentSurface.NodeDetailsControl.ParentControl = this;
                }
            }
            
        }

        

        public void Dispose()
        {
            if (MotionHandler != null)
            {
                MotionHandler.Dispose();
            }
        }

        #region child and parent supporting methods

        private void AddChild(NodeControl node)
        {
            if (!ChildNodes.Contains(node))
            {
                ChildNodes.Add(node);
            }
        }

        private void RemoveChild(NodeControl node)
        {
            if (ChildNodes.Contains(node))
            {
                ChildNodes.Remove(node);
            }
        }

        private void AddParent(NodeControl node)
        {
            if (!ParentNodes.Contains(node))
            {
                ParentNodes.Add(node);
            }
        }

        private void RemoveParent(NodeControl node)
        {
            if (ParentNodes.Contains(node))
            {
                ParentNodes.Remove(node);
            }
        }

        public IEnumerable<NodeControl> GetAllNodeControls()
        {
            var output = new List<NodeControl> {this};
            output.AddRange(GetAllChildNodeControls());
            return output;
        }

        void ISelectableNode.Focus()
        {
            Focus();
        }

        public IEnumerable<NodeControl> GetAllChildNodeControls()
        {
            var output = new List<NodeControl>();
            var nodeToLoop = ChildNodes;
            var list = new List<NodeControl>();
            while (nodeToLoop.Count > 0)
            {
                list.Clear();
                foreach (var nodeControl in nodeToLoop)
                {
                    if (!output.Contains(nodeControl))
                    {
                        output.Add(nodeControl);
                        if (nodeControl.ChildNodes.Count > 0)
                        {
                            foreach (var child in nodeControl.ChildNodes)
                            {
                                if (!output.Contains(child))
                                {
                                    list.Add(child);
                                }
                            }
                        }
                    }
                }
                nodeToLoop = list.ToList();
            }
            
            if (output.Contains(this))
            {
                output.Remove(this);
            }
            return output.Distinct();
        }

        public void LinkFromNode(NodeControl node)
        {
            AddChild(node);
            node.AddParent(this);
        }

        public void UnLinkFromNode(NodeControl node)
        {
            RemoveChild(node);
            node.RemoveParent(this);
            if (ChildNodes.Count == 0)
            {
                CollapseControl.State = CollapseState.None;
            }
        }

        

        #endregion

        private void Container_OnMouseEnter(object sender, MouseEventArgs e)
        {
            
        }

        private void Container_OnMouseLeave(object sender, MouseEventArgs e)
        {
            
        }
    }
}
