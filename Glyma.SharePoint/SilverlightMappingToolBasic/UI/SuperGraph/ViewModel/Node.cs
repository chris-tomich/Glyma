using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.Extensions;
using SilverlightMappingToolBasic.UI.SuperGraph.View.CollapseSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.Mouse;
using SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses;
using SilverlightMappingToolBasic.UI.SuperGraph.View.PropertiesEditorSupportClasses.Edit;
using SilverlightMappingToolBasic.UI.ViewModel;
using TransactionalNodeService.Service;
using TransactionalNodeService.Soap.TransactionFramework;
using TNSProxy = TransactionalNodeService.Proxy;
using SilverlightMappingToolBasic.UI.Extensions.VideoWebPart;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using SilverlightMappingToolBasic.UI.SuperGraph.View.NodeControlSupportClasses;

namespace SilverlightMappingToolBasic.UI.SuperGraph.ViewModel
{
    public class Node : ViewModelBase, INode, ISkinnableNode, IViewModelMetadataFactory
    {
        #region private properties

        private int _numTransclusions;
        private bool _isSelected;
        private bool _isMetadataChangeRegistered;
        private string _skinName;
        private ImageSource _nodeImage;
        private Point _location;
        private NodeProperties _nodeProperties;

        private CollapseState _authorCollapseState;
        private Visibility _authorVisibility;

        private CollapseState _viewerCollapseState;
        private Visibility _viewerVisibility;

        #endregion

        public event EventHandler<LocationChangedEventArgs> LocationChanged;

        public Node(TNSProxy.IMapManager mapManager)
        {
            IsFocused = false;
            IsTranscluded = false;
            IsLocationDirty = false;
            MapManager = mapManager;
        }

        public Node(TNSProxy.INode node)
        {
            Proxy = node;
            MapManager = node.MapManager;
        }

        public Node(Node nodeToCopy)
        {
            MapManager = nodeToCopy.MapManager;
            Proxy = nodeToCopy.Proxy;
            IsTranscluded = nodeToCopy.IsTranscluded;
            _numTransclusions = nodeToCopy._numTransclusions;
            NodeImage = nodeToCopy.NodeImage;
            VideoInfo = nodeToCopy.VideoInfo;
            _skinName = nodeToCopy._skinName;
            _location = nodeToCopy._location;
            Metadata = nodeToCopy.Metadata;
            foreach (var metadata in nodeToCopy.NodeProperties.UIMetadata)
            {
                NodeProperties.UIMetadata.Add(metadata.Key, metadata.Value);
            }
            DescriptionType = nodeToCopy.DescriptionType;
            _viewerCollapseState = CollapseState.None;
            _authorCollapseState = CollapseState.None;
        }

        public NodeProperties NodeProperties
        {
            get
            {
                if (_nodeProperties == null)
                {
                    _nodeProperties= new NodeProperties(this);
                    _nodeProperties.PropertyChanged+= NodePropertiesOnPropertyChanged;
                }
                return _nodeProperties;
            }
        }

        private void NodePropertiesOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                OnNotifyPropertyChanged("Name");
            }
            else if (e.PropertyName == "Video.Source" || e.PropertyName == "Video.StartPosition" || e.PropertyName == "Video.EndPosition")
            {
                var nodeProperties = sender as NodeProperties;
                if (nodeProperties != null)
                {
                    if (e.PropertyName == "Video.Source")
                    {
                        VideoInfo.VideoSource = nodeProperties.UIMetadata["Video.Source"];
                    }
                    else if (e.PropertyName == "Video.StartPosition")
                    {
                        TimeSpan time;
                        if (TimeSpan.TryParse(nodeProperties.UIMetadata["Video.StartPosition"], out time))
                        {
                            VideoInfo.StartPosition = time;
                        }
                    }
                    else if (e.PropertyName == "Video.EndPosition")
                    {
                        TimeSpan time;
                        if (TimeSpan.TryParse(nodeProperties.UIMetadata["Video.EndPosition"], out time))
                        {
                            VideoInfo.StopPosition = time;
                        }
                    }
                    ((MainPage)Application.Current.RootVisual).SuperGraphController.VideoController.SendPlayCommand(VideoInfo, false);
                }
            }
        }

        public Visibility ExpandVisibility
        {
            get
            {
                return State == CollapseState.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool IsRootMap
        {
            get;
            set;
        }

        public Visibility CollapseVisibility
        {
            get
            {
                return State == CollapseState.Expanded ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility SemiCollapseVisibility
        {
            get
            {
                return State == CollapseState.SemiCollapsed ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public CollapseState State
        {
            get
            {
                return App.UserStyle == UserStyle.Reader ? _viewerCollapseState : _authorCollapseState;
            }
            set
            {
                if (ViewerCollapseState != value || AuthorCollapseState != value)
                {
                    ViewerCollapseState = value;
                    AuthorCollapseState = value;
                }
            }
        }

        public CollapseState AuthorCollapseState
        {
            get { return _authorCollapseState; }
            set
            {
                if (_authorCollapseState != value)
                {
                    _authorCollapseState = value;
                    IsAuthorCollapseStateDirty = true;
                    NotifiyStateChange();
                }
            }
        }

        public CollapseState ViewerCollapseState
        {
            get { return _viewerCollapseState; }
            set
            {
                if (_viewerCollapseState != value)
                {
                    _viewerCollapseState = value;
                    IsViewerCollapseStateDirty = true;
                    NotifiyStateChange();
                }
            }
        }

        public Visibility ViewerVisibility
        {
            get
            {
                return _viewerVisibility;
            }
        }

        public bool IsLocationDirty
        {
            get;
            set;
        }

        public bool IsViewerVisibilityDirty
        {
            get;
            set;
        }

        public bool IsViewerCollapseStateDirty
        {
            get;
            set;
        }

        public bool IsAuthorVisibilityDirty
        {
            get;
            set;
        }

        public bool IsAuthorCollapseStateDirty
        {
            get;
            set;
        }

        public TNSProxy.IMapManager MapManager
        {
            get;
            private set;
        }

        public TNSProxy.INode Proxy
        {
            get;
            private set;
        }

        public Guid Id
        {
            get
            {
                if (Proxy != null)
                {
                    return Proxy.ClientId;
                }
                return Guid.Empty;
            }
        }

        public bool IsMapNode
        {
            get
            {
                return MapObjectType.Equals(MapManager.NodeTypes["CompendiumMapNode"].Id);
            }
        }

        public string NodeType
        {
            get { return MapManager.NodeTypes[MapObjectType].Name; }
        }

        public Guid DomainId
        {
            get
            {
                if (Proxy != null)
                {
                    return Proxy.DomainId;
                }
                return Guid.Empty;
            }
        }

        public string OriginalId
        {
            get
            {
                if (Proxy != null)
                {
                    return Proxy.OriginalId;
                }
                return string.Empty;
            }
        }

        public Guid MapObjectType
        {
            get
            {
                if (Proxy != null)
                {
                    return Proxy.NodeType.Id;
                }
                return Guid.Empty;
            }
        }

        public bool IsTranscluded
        {
            get;
            set;
        }

        public int NumTranclusions
        {
            get
            {
                return _numTransclusions;
            }
            set
            {
                if (_numTransclusions != value)
                {
                    _numTransclusions = value;
                    OnNotifyPropertyChanged("NumTransclusions");
                }
            }
        }

        public bool IsFocused
        {
            get;
            set;
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnNotifyPropertyChanged("IsSelected");
                }
            }
        }

        public string Name
        {
            get
            {
                return NodeProperties.Name;
            }
            set
            {
                NodeProperties.Name = value;
                OnNotifyPropertyChanged("Name");
            }
        }

        public IViewModelMetadataFactory ViewModelMetadataFactory
        {
            get
            {
                return this;
            }
        }

        public string SpokenBy
        {
            get
            {
                return NodeProperties.SpokenBy;
            }
            set
            {
                NodeProperties.SpokenBy = value;
            }
        }

        public string Description
        {
            get
            {
                return NodeProperties.Description;
            }
            set
            {
                NodeProperties.Description = value;
            }
        }

        public NodeDescriptionType DescriptionType
        {
            get
            {
                return NodeProperties.DescriptionType;
            }
            set
            {
                NodeProperties.DescriptionType = value;
            }
        }

        public string Note
        {
            get
            {
                return NodeProperties.Note;
            }
            set
            {
                NodeProperties.Note = value;
            }
        }

        public Visibility Visibility
        {
            get
            {
                return App.UserStyle == UserStyle.Reader? _viewerVisibility : _authorVisibility;
            }
            set
            {
                if (_viewerVisibility != value || _authorVisibility != value)
                {
                    if (_viewerVisibility != value)
                    {
                        _viewerVisibility = value;
                        IsViewerVisibilityDirty = true;
                    }
                    if (_authorVisibility != value)
                    {
                        _authorVisibility = value;
                        IsAuthorVisibilityDirty = true;
                    }
                    OnNotifyPropertyChanged("Visibility");
                }
            }
        }

        public ImageSource NodeImage
        {
            get
            {
                return _nodeImage;
            }
            set
            {
                if (_nodeImage != value)
                {
                    _nodeImage = value;
                    OnNotifyPropertyChanged("NodeImage");
                }
            }
        }

        public VideoInfo VideoInfo
        {
            get;
            set;
        }

        public NodeClickOptions NodeClickOptions
        {
            get
            {
                NodeClickOptions options = new NodeClickOptions() { ShowRelatedMaps = true };
                IMetadata nodeActionOptions = null;
                if (this.Metadata.TryGetValue("NodeClickOptions", out nodeActionOptions))
                {
                    if (!string.IsNullOrWhiteSpace(nodeActionOptions.Value))
                    {
                        string[] separators = { ",", ";", " " };
                        string[] optionsStrings = nodeActionOptions.Value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string option in optionsStrings)
                        {
                            string normalizedOption = option.Trim().ToLower();
                            switch (normalizedOption)
                            {
                                case "relatedmaps.hide":
                                    options.ShowRelatedMaps = false;
                                    break;
                            }
                        }
                    }
                }
                return options;
            }
        }

        public NodeActionOptions NodeActionOptions
        {
            get
            {
                NodeActionOptions options = new NodeActionOptions() { ShowRelatedContentWithVideo = false };
                IMetadata nodeActionOptions = null;
                if (this.Metadata.TryGetValue("NodeActionOptions", out nodeActionOptions))
                {
                    if (!string.IsNullOrWhiteSpace(nodeActionOptions.Value))
                    {
                        string[] separators = { ",", ";", " " };
                        string[] optionsStrings = nodeActionOptions.Value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string option in optionsStrings)
                        {
                            string normalizedOption = option.Trim().ToLower();
                            switch (normalizedOption)
                            {
                                case "video.showrelatedcontent":
                                    options.ShowRelatedContentWithVideo = true;
                                    break;
                            }
                        }
                    }
                }
                return options;
            }
        }

        public string SkinName
        {
            get
            {
                return _skinName;
            }
            set
            {
                if (_skinName != value)
                {
                    _skinName = value;
                    OnNotifyPropertyChanged("SkinName");
                }
            }
        }

        public Point Location
        {
            get
            {
                return _location;
            }
            set
            {
                if (_location != value)
                {
                    IsLocationDirty = true;
                    _location = value;
                    OnNotifyPropertyChanged("Location");
                }
            }
        }

        public MetadataCollection Metadata
        {
            get
            {
                return NodeProperties.Metadata;
            }
            set
            {
                NodeProperties.Metadata = value;
            }
        }

        #region private methods

        private void OnMetadataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                var metadata = sender as Metadata;
                if (metadata != null)
                {
                    switch (metadata.Name)
                    {
                        case "Name":
                            Name = metadata.Value;
                            break;
                        case "Video.StartPosition":
                            TimeSpan startPosition;
                            if (TimeSpan.TryParse(metadata.Value, out startPosition))
                            {
                                VideoInfo.StartPosition = startPosition;
                            }
                            break;
                        case "Video.EndPosition":
                            TimeSpan stopPosition;
                            if (TimeSpan.TryParse(metadata.Value, out stopPosition))
                            {
                                VideoInfo.StopPosition = stopPosition;
                            }
                            else
                            {
                                VideoInfo.StopPosition = null;
                            }
                            break;
                    }
                }
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var viewModelMetadata = sender as IMetadata;
            if (viewModelMetadata != null && e.Action == NotifyCollectionChangedAction.Add)
            {
                viewModelMetadata.PropertyChanged += OnMetadataPropertyChanged;
            }
        }

        private void NotifiyStateChange()
        {
            OnNotifyPropertyChanged("State");
            OnNotifyPropertyChanged("ExpandVisibility");
            OnNotifyPropertyChanged("CollapseVisibility");
            OnNotifyPropertyChanged("SemiCollapseVisibility");
        }

        #endregion

        public void CommitLocation()
        {
            if (LocationChanged != null && IsLocationDirty)
            {
                IsLocationDirty = false;

                var eventArgs = new LocationChangedEventArgs {Location = Location};

                LocationChanged(this, eventArgs);
            }
        }

        public void LoadNode(TNSProxy.IRelationship relationship, TNSProxy.INode node)
        {
            Proxy = node;
            if (node.Status == TNSProxy.LoadState.Full)
            {
                /// Check that there isn't more than one MapContainerRelationship. If there is then this node is transcluded.
                IEnumerable<TNSProxy.IRelationship> mapContainerRelationships = node.Relationships.FindRelationships(MapManager.ConnectionTypes["From"], MapManager.RelationshipTypes["MapContainerRelationship"]);
                NumTranclusions = mapContainerRelationships.Count(rel => rel.MapManager.RelationshipFactory.FindRelationship(rel.Id) != null);
                IsTranscluded = (NumTranclusions > 1); //BUG: value changes based on the current view, possibly incorrect as the node is either transcluded or not transcluded regardless of the view

                TNSProxy.NodeMetadataCollection metadata = node.Metadata;

                string xPositionAsString = "0";
                string yPositionAsString = "0";

                if (metadata.FindMetadata(relationship, "XPosition") != null)
                {
                    xPositionAsString = metadata.FindMetadata(relationship, "XPosition").Value;
                }

                if (metadata.FindMetadata(relationship, "YPosition") != null)
                {
                    yPositionAsString = metadata.FindMetadata(relationship, "YPosition").Value;
                }

                if (metadata.FindMetadata(relationship, "CollapseState") != null)
                {
                    _viewerCollapseState = (CollapseState)Enum.Parse(typeof(CollapseState), metadata.FindMetadata(relationship, "CollapseState").Value, true);
                }
                else
                {
                    _viewerCollapseState = CollapseState.None;
                }

                if (metadata.FindMetadata(relationship, "Visibility") != null)
                {
                    _viewerVisibility = metadata.FindMetadata(relationship, "Visibility").Value.Equals("visible", StringComparison.OrdinalIgnoreCase)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
                else
                {
                    _viewerVisibility = Visibility.Visible;
                }

                if (metadata.FindMetadata(relationship, "AuthorCollapseState") != null)
                {
                    _authorCollapseState = (CollapseState)Enum.Parse(typeof(CollapseState), metadata.FindMetadata(relationship, "AuthorCollapseState").Value, true);
                }
                else
                {
                    _authorCollapseState = CollapseState.None;
                }

                if (metadata.FindMetadata(relationship, "AuthorVisibility") != null)
                {
                    _authorVisibility = metadata.FindMetadata(relationship, "AuthorVisibility").Value.Equals("visible", StringComparison.OrdinalIgnoreCase)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
                else
                {
                    _authorVisibility = Visibility.Visible;
                }

                if (!string.IsNullOrEmpty(xPositionAsString) && !string.IsNullOrEmpty(yPositionAsString))
                {
                    double xPosition;
                    double yPosition;

                    if (double.TryParse(xPositionAsString, out xPosition) && double.TryParse(yPositionAsString, out yPosition))
                    {
                        _location = new Point(xPosition, yPosition);
                    }
                }

                IDictionary<string, TNSProxy.IMetadataSet> nodeMetadata = metadata.FindMetadata(null, (TNSProxy.ConnectionType)null);
                foreach (TNSProxy.IMetadataSet metadataSet in nodeMetadata.Values)
                {
                    IMetadata viewModelMetadata = ViewModelMetadataFactory.CreateMetadata(metadataSet);
                    viewModelMetadata.PropertyChanged += OnMetadataPropertyChanged;
                    Metadata.Add(viewModelMetadata.Name, viewModelMetadata);
                    NodeProperties.UIMetadata.Add(viewModelMetadata.Name, viewModelMetadata.Value);
                }

                if (!_isMetadataChangeRegistered)
                {
                    Metadata.CollectionChanged += OnCollectionChanged;
                    _isMetadataChangeRegistered = true;
                }
            }
        }

        IMetadata IViewModelMetadataFactory.CreateMetadata(TNSProxy.IMetadataSet newMetadataSet)
        {
            return new Metadata(newMetadataSet, MapManager);
        }
    }
}
