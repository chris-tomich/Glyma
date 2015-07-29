using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Messaging;

using VideoPlayerSharedLib;

using SilverlightMappingToolBasic.Controls;
using SilverlightMappingToolBasic.MappingService;
using System.Windows.Browser;
using SilverlightMappingToolBasic.UI.Extensions.VideoWebPart;

namespace SilverlightMappingToolBasic
{
    public abstract class NodeRenderer : Canvas, INodeRenderer
    {
        protected static int CurrentTopZOrder = 1;

        private bool _isLeftMouseDown = false;
        private Point _nodeOffset;
        protected NodeControl _nodeControl = null;
        private Point _lastPosition;
        private bool _nodeDirty;
        private bool _doubleClicked = false;

        public event RoutedEventHandler Selected;
        public event RoutedEventHandler Unselected;

        protected NodeRenderer()
        {
            SkinName = "Default";
            SkinProperties = new Dictionary<string, object>();
            _nodeDirty = false;
            InEditState = false;
        }

        public NodeRenderer(NavigatorView parentNavigatorView, INodeProxy nodeProxy, ThemeManager themeManager, string skinName)
            : this()
        {
            Node = nodeProxy;
            ParentNavigatorView = parentNavigatorView;
            ThemeManagementObject = themeManager;

            SkinName = skinName;
            Skin = ThemeManagementObject.GetSkin(Node.NodeType, SkinName);

            this.Loaded += new RoutedEventHandler(OnLoaded);

            this.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);
            this.MouseMove += new MouseEventHandler(OnMouseMove);
        }

        public bool IsSelected
        {
            get
            {
                bool result = false;
                if (_nodeControl != null)
                {
                    result = _nodeControl.IsSelected;
                }
                return result;
            }
            set
            {
                if (_nodeControl != null)
                {
                    _nodeControl.IsSelected = value;
                }
            }
        }

        public bool InEditState
        {
            get;
            set;
        }

        private void nodeRenderer_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Point newLocation = e.GetPosition(null);
            if (newLocation == _lastPosition)
            {
                if (NodeDoubleClicked != null)
                {
                    NodeDoubleClicked.Invoke(this, new NodeClickedArgs() { Node = Node });
                }
            }
        }

        protected void OnLoaded(object sender, RoutedEventArgs e)
        {
            int zIndex = (int)this.GetValue(Canvas.ZIndexProperty);

            this.SetValue(Canvas.ZIndexProperty, 0);

            if (Context != null)
            {
                _nodeControl = RenderChildren(Context);

                if (!this.Children.Contains(_nodeControl))
                {
                    this.Children.Add(_nodeControl);
                    _nodeControl.PlayMediaIconClicked += new RoutedEventHandler(_nodeControl_PlayMediaIconClicked);
                    _nodeControl.PauseMediaIconClicked += new RoutedEventHandler(_nodeControl_PauseMediaIconClicked);
                    _nodeControl.Selected += new RoutedEventHandler(_nodeControl_Selected);
                    _nodeControl.Unselected += new RoutedEventHandler(_nodeControl_Unselected);
                    _nodeControl.NodeTextBlockClicked += new RoutedEventHandler(_nodeControl_NodeTextBlockClicked);

                    if (InEditState)
                    {
                        _nodeControl.InEditState = true;
                    }
                }

                this.SetValue(Canvas.LeftProperty, Location.X);
                this.SetValue(Canvas.TopProperty, Location.Y);
            }
        }

        void _nodeControl_NodeTextBlockClicked(object sender, RoutedEventArgs e)
        {
            foreach (INodeRenderer renderer in ParentNavigatorView.NodeRenderers.Values)
            {
                if (renderer.Node.Id != this.Node.Id)
                {
                    renderer.CommitNodeName();
                }
            }
        }

        private void _nodeControl_Unselected(object sender, RoutedEventArgs e)
        {
            if (Unselected != null)
            {
                Unselected.Invoke(sender, e);
            }
        }

        private void _nodeControl_Selected(object sender, RoutedEventArgs e)
        {
            if (Selected != null)
            {
                Selected.Invoke(sender, e);
            }
        }

        private void _nodeControl_PlayMediaIconClicked(object sender, RoutedEventArgs e)
        {
            MetadataContext videoSourceKey = new MetadataContext() { MetadataName = "Video.Source", NodeUid = Node.Id };
            string source = Node.GetNodeMetadata(videoSourceKey).MetadataValue;

            VideoSizeHelper sizeHelper = new VideoSizeHelper(Node);
            RelatedContentPanelUtil.Instance.LoadRelatedVideoContent(source, sizeHelper.Size);

            string startPosition = null;
            string endPosition = null;
            Command playCommand = new Command();
            playCommand.Name = "Play";
            playCommand.Params = new List<Param>();
            playCommand.Params.Add(new Param() { Name = "Source", Value = source });
            playCommand.Params.Add(new Param() { Name="NodeId", Value=Node.Id.ToString()});
            playCommand.Params.Add(new Param() { Name="AutoPlay", Value="true"});
            MetadataContext videoStartPosKey = new MetadataContext() { MetadataName = "Video.StartPosition", NodeUid = Node.Id };
            if (Node.HasMetadata(videoStartPosKey))
            {
                startPosition = Node.GetNodeMetadata(videoStartPosKey).MetadataValue;
                playCommand.Params.Add(new Param() { Name="StartTimeCode", Value=startPosition});
            }
            MetadataContext videoEndPosKey = new MetadataContext() { MetadataName = "Video.EndPosition", NodeUid = Node.Id };
            if (Node.HasMetadata(videoEndPosKey))
            {
                endPosition = Node.GetNodeMetadata(videoEndPosKey).MetadataValue;
                playCommand.Params.Add(new Param() { Name="EndTimeCode", Value=endPosition});
            }
            LocalMessageSender msgSender = IoC.IoCContainer.GetInjectionInstance().GetInstance<LocalMessageSender>();
            //Utilities.SendMessage<Command>(msgSender, playCommand);
        }

        private void _nodeControl_PauseMediaIconClicked(object sender, RoutedEventArgs e)
        {
            Command pauseCommand = new Command();
            pauseCommand.Name = "Pause";
            LocalMessageSender msgSender = IoC.IoCContainer.GetInjectionInstance().GetInstance<LocalMessageSender>();
            //Utilities.SendMessage<Command>(msgSender, pauseCommand);
        }

        public void Refresh()
        {
            _nodeControl.RefreshMetadata(Node);
        }

        public void CommitNodeName()
        {
            _nodeControl.CommitNodeName();
        }

        public bool ResetMediaIcon(bool playing)
        {
            if (_nodeControl.IsPlayingMedia)
            {
                if (playing)
                {
                    _nodeControl.SetPlayingMediaIcon();
                }
                else
                {
                    _nodeControl.ResetMediaIcon();
                }
                return true;
            }
            return false;
        }

        public void SetIsPlaying(bool playing)
        {
            if (_nodeControl != null)
            {
                _nodeControl.IsPlayingMedia = playing;
            }
        }

        public bool ResetMediaIcon(TimeSpan newPosition)
        {
            if (_nodeControl.IsPlayingMedia)
            {
                string endPosition;
                MetadataContext videoEndPosKey = new MetadataContext() { MetadataName = "Video.EndPosition", NodeUid = Node.Id };
                if (Node.HasMetadata(videoEndPosKey))
                {
                    TimeSpan endPositionTs = TimeSpan.MinValue;
                    endPosition = Node.GetNodeMetadata(videoEndPosKey).MetadataValue;
                    if (TimeSpan.TryParse(endPosition, out endPositionTs))
                    {
                        if (newPosition > endPositionTs)
                        {
                            _nodeControl.ResetMediaIcon();
                            _nodeControl.IsPlayingMedia = false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        protected virtual void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (INodeRenderer renderer in ParentNavigatorView.NodeRenderers.Values)
            {
                if (renderer.Node.Id != this.Node.Id)
                {
                    renderer.CommitNodeName();
                }
            }

            switch (e.ClickCount)
            {
                case 1:
                    Point currentPosition = e.GetPosition(null);
                    _lastPosition = currentPosition;

                    this.SetValue(Canvas.ZIndexProperty, CurrentTopZOrder++);

                    _isLeftMouseDown = this.CaptureMouse();

                    double nodeXOffset = Canvas.GetLeft(this) - currentPosition.X;
                    double nodeYOffset = Canvas.GetTop(this) - currentPosition.Y;

                    ScaleTransform scaleTransform = FindName("scaleTransform") as ScaleTransform;
                    if (scaleTransform != null)
                    {
                        nodeXOffset = Canvas.GetLeft(this) - currentPosition.X / scaleTransform.ScaleX;
                        nodeYOffset = Canvas.GetTop(this) - currentPosition.Y / scaleTransform.ScaleY;
                    }

                    _nodeOffset = new Point(nodeXOffset, nodeYOffset);

                    ThreadStart selectNodeDelayThread = new ThreadStart(SelectNodeControl);
                    Thread thread = new Thread(selectNodeDelayThread);
                    thread.Start();
                    _doubleClicked = false;
                    e.Handled = true;
                    break;
                case 2:
                    _doubleClicked = true;
                    nodeRenderer_DoubleClick(this, e);
                    break;
            }            
        }

        protected void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ReleaseMouseCapture();
            _isLeftMouseDown = false;

            StoreLocation();
        }

        protected void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isLeftMouseDown)
            {
                Point newNodePosition = e.GetPosition(null);

                UIElement uiElement = this;

                double newXCoord = newNodePosition.X + _nodeOffset.X;
                double newYCoord = newNodePosition.Y + _nodeOffset.Y;
                ScaleTransform scaleTransform = FindName("scaleTransform") as ScaleTransform;
                if (scaleTransform != null)
                {
                    newXCoord = newNodePosition.X / scaleTransform.ScaleX + _nodeOffset.X;
                    newYCoord = newNodePosition.Y / scaleTransform.ScaleY + _nodeOffset.Y;
                }

                if (!IsSelected)
                {
                    foreach (INodeRenderer renderer in ParentNavigatorView.NodeRenderers.Values)
                    {
                        if (renderer.IsSelected && renderer.Node.Id != this.Node.Id)
                        {
                            renderer.IsSelected = false;
                        }
                    }
                    foreach (IRelationshipRenderer relRenderer in ParentNavigatorView.RelationshipRenderers.Values)
                    {
                        if (relRenderer.IsSelected)
                        {
                            relRenderer.IsSelected = false;
                        }
                    }
                }

                MoveNode(new Point(newXCoord, newYCoord), false);

                if (NodePositionUpdating != null && _nodeDirty)
                {
                    NodePositionUpdating.Invoke(this, null);
                }
            }
        }

        private Point _originalLocation;

        public void MoveNode(Point newLocation, bool forceRelationshipUpdate)
        {
            this.SetValue(Canvas.LeftProperty, newLocation.X);
            this.SetValue(Canvas.TopProperty, newLocation.Y);

            if (!_nodeDirty)
            {
                _originalLocation = Location;
            }

            if (newLocation != Location)
            {
                _nodeDirty = true;
                Location = newLocation;
            }

            if (forceRelationshipUpdate)
            {
                if (NodePositionUpdating != null && _nodeDirty)
                {
                    NodePositionUpdating.Invoke(this, null);
                }
            }
        }

        public void StoreLocation()
        {
            if (NodePositionUpdated != null && _nodeDirty)
            {
                _nodeDirty = false;
                if (HasMoved())
                {
                    NodePositionUpdated.Invoke(this, new EventArgs());
                }
            }
        }


        /// <summary>
        /// Due to rounding the values can appear dirty even though it's such a small fraction of a difference
        /// This determines if there was a change by checking with rounding if it's really an unsaved value.
        /// This is used to greatly reduce the hits on the NodeService when moving nodes via the auto arrange.
        /// </summary>
        /// <returns></returns>
        private bool HasMoved()
        {
            bool result = false;
            double xVal = Math.Round((Location.X / _originalLocation.X), 7);
            double yVal = Math.Round((Location.Y / _originalLocation.Y), 7);
            if (xVal != 1)
            {
                result = true;
            }
            if (yVal != 1)
            {
                result = true;
            }
            return result;
        }


        /// <summary>
        /// Ensures the selection only occurs on nodes that aren't being moved
        /// </summary>
        private void SelectNodeControl()
        {
            Thread.Sleep(300); //slight delay if the node hasn't been moved when this is up it will select

            // put it on the UI thread to execute.
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (_nodeControl != null && !_nodeDirty && !_doubleClicked)
                {
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        if (!_nodeControl.IsSelected)
                        {
                            _nodeControl.IsSelected = true;
                            SelectChildNodes(Node);
                        }
                        else
                        {
                            _nodeControl.IsSelected = false;
                            UnselectChildNodes(Node);
                        }
                    }
                    else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
                    {
                        if (!_nodeControl.IsSelected)
                        {
                            _nodeControl.IsSelected = true;
                            SelectParentNodes(Node);
                        }
                        else
                        {
                            _nodeControl.IsSelected = false;
                            UnselectParentNodes(Node);
                        }
                    }
                    else
                    {
                        if (Keyboard.Modifiers != ModifierKeys.Shift)
                        {
                            foreach (INodeRenderer renderer in ParentNavigatorView.NodeRenderers.Values)
                            {
                                if (renderer.IsSelected && renderer.Node.Id != this.Node.Id)
                                {
                                    renderer.IsSelected = false;
                                }
                            }
                            foreach (IRelationshipRenderer relRenderer in ParentNavigatorView.RelationshipRenderers.Values)
                            {
                                if (relRenderer.IsSelected)
                                {
                                    relRenderer.IsSelected = false;
                                }
                            }
                        }
                        IsSelected = !IsSelected;
                    }
                }
            });
        }

        public abstract void SelectChildNodes(INodeProxy nodeProxy);
        public abstract void UnselectChildNodes(INodeProxy nodeProxy);
        public abstract void SelectParentNodes(INodeProxy nodeProxy);
        public abstract void UnselectParentNodes(INodeProxy nodeProxy);
       
        public abstract NodeControl RenderChildren(RenderingContextInfo context);

        #region INodeRenderer Members

        public event EventHandler NodePositionUpdated;
        public event EventHandler NodePositionUpdating;
        public event EventHandler<NodeClickedArgs> NodeDoubleClicked;

        public Point Location
        {
            get;
            protected set;
        }

        public INodeProxy Node
        {
            get;
            set;
        }

        public NodeControl NodeControl
        {
            get
            {
                return _nodeControl;
            }
        }

        public string SkinName
        {
            get;
            protected set;
        }

        public INodeSkin Skin
        {
            get;
            protected set;
        }

        public Dictionary<string, object> SkinProperties
        {
            get;
            protected set;
        }

        public NavigatorView ParentNavigatorView
        {
            get;
            protected set;
        }

        public ThemeManager ThemeManagementObject
        {
            get;
            protected set;
        }

        public RenderingContextInfo Context
        {
            get;
            set;
        }

        public void SetSkinName(string skinName)
        {
            SkinName = skinName;
            Skin = ThemeManagementObject.GetSkin(Node.NodeType, SkinName);
        }

        #endregion
    }
}
