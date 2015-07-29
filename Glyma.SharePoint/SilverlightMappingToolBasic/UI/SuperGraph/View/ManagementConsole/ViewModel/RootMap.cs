using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Glyma.UtilityService.Proxy;
using Glyma.UtilityService.Proxy.Service;
using SilverlightMappingToolBasic.GlymaSecurityService;
using SilverlightMappingToolBasic.UI.Extensions.Security;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.ViewModel
{
    public sealed class RootMap : ManagementConsoleObject
    {
        private bool _isOperationPanelVisible;

        private readonly string _parentName;
        private bool _disableAutoRealign;

        private bool _yammerEnabled;
        private string _yammerNetwork;
        private string _yammerFeedType;
        private string _yammerObjectUrl;
        private string _yammerPromptText;
        private string _yammerFeedId;

        private ObservableCollection<string> _yammerFeedTypes;

        private ObservableCollection<IExportJob> _exportJobs; 

        private bool _isExpanded;
        private bool _disableInsightsFeed;
        private bool _disableActivityFeed;

        public bool IsNewMap { get; set; }


        public ObservableCollection<IExportJob> ExportJobs
        {
            get
            {
                if (_exportJobs == null)
                {
                    _exportJobs = new ObservableCollection<IExportJob>();
                }
                return _exportJobs;
            }
            set
            {
                _exportJobs = value;
                OnPropertyChanged("ExportJobs");
            }
        } 

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }
            }
        }

        public bool IsSelectable { get; private set; }

        private bool _isSelected;
        public override bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (IsSelectable)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                    OnPropertyChanged("SelectedIndicatorVisiblity");
                }
                else
                {
                    _isSelected = false;
                    OnPropertyChanged("IsSelected");
                    OnPropertyChanged("SelectedIndicatorVisiblity");
                }
            }
        }

        public override string DisplayName
        {
            get
            {
                return _parentName + " - " + Name;
            }
        }

        public bool IsOperationPanelVisible
        {
            get { return _isOperationPanelVisible; }
            set
            {
                if (_isOperationPanelVisible != value)
                {
                    _isOperationPanelVisible = value;
                    OnPropertyChanged("IsOperationPanelVisible");
                    OnPropertyChanged("OperationPanelVisibility");
                }
            }
        }

        public Visibility OperationPanelVisibility
        {
            get { return _isOperationPanelVisible ? Visibility.Visible : Visibility.Collapsed; }
        }

        public RootMap(INode node, Dictionary<PermissionLevel, ObservableCollection<GlymaSecurityGroup>> template, string parentName, bool isSelectable = true)
            : base(node, template, false)
        {
            _parentName = parentName;
            IsSelectable = isSelectable;
            ReloadMetadata();
        }

        


        public bool DisableAutoRealign
        {
            get
            {
                return _disableAutoRealign;
            }
            set
            {
                _disableAutoRealign = value;
                OnPropertyChanged("AllowAutoRealign");
                OnMetadataChanged("AllowAutoRealign", (!value).ToString());
            }
        }

        public bool YammerEnabled
        {
            get
            {
                return _yammerEnabled && IsEditable;
            }
            set
            {
                if (_yammerEnabled != value)
                {
                    _yammerEnabled = value;
                    OnPropertyChanged("YammerEnabled");
                    if (value)
                    {
                        OnMetadataChanged("Yammer.Network", YammerNetwork);
                        OnMetadataChanged("Yammer.FeedType", YammerFeedType);
                        if (YammerPromptText != null)
                        {
                            OnMetadataChanged("Yammer.PromptText", YammerPromptText);
                        }
                        if (YammerFeedID != null)
                        {
                            OnMetadataChanged("Yammer.FeedId", YammerFeedID);
                        }
                        if (YammerObjectUrl != null)
                        {
                            OnMetadataChanged("Yammer.ObjectUrl", YammerObjectUrl);
                        }
                    }
                    else
                    {
                        //clear all the Yammer metadata values
                        OnMetadataChanged("Yammer.Network", string.Empty);
                        OnMetadataChanged("Yammer.FeedType", string.Empty);
                        OnMetadataChanged("Yammer.PromptText", string.Empty);
                        OnMetadataChanged("Yammer.FeedId", string.Empty);
                        OnMetadataChanged("Yammer.ObjectUrl", string.Empty);
                    }
                }
            }
        }

        public ObservableCollection<string> YammerFeedTypes
        {
            get
            {
                if (_yammerFeedTypes == null)
                {
                    _yammerFeedTypes = new ObservableCollection<string>();
                    _yammerFeedTypes.Add("open-graph");
                    _yammerFeedTypes.Add("group");
                    _yammerFeedTypes.Add("topic");
                    _yammerFeedTypes.Add("user");
                }
                return _yammerFeedTypes;
            }
        }

        public string YammerNetwork
        {
            get
            {
                return _yammerNetwork;
            }
            set
            {
                if (_yammerNetwork != value)
                {
                    _yammerNetwork = value;
                    OnPropertyChanged("YammerNetwork");
                    OnMetadataChanged("Yammer.Network", _yammerNetwork);
                }
            }
        }

        public string YammerFeedType
        {
            get
            {
                return _yammerFeedType;
            }
            set
            {
                if (_yammerFeedType != value)
                {
                    _yammerFeedType = value;
                    if (_yammerFeedType == "open-graph")
                    {
                        YammerFeedID = string.Empty;
                    }
                    else if (_yammerFeedType == "group" || _yammerFeedType == "topic" || _yammerFeedType == "user")
                    {
                        YammerObjectUrl = string.Empty;
                    }
                    OnPropertyChanged("YammerFeedType");
                    OnPropertyChanged("YammerOpenGraphControlVisibility");
                    OnPropertyChanged("YammerGroupControlVisibility");
                    OnMetadataChanged("Yammer.FeedType", _yammerFeedType);
                }
            }
        }

        public string YammerPromptText
        {
            get
            {
                return _yammerPromptText;
            }
            set
            {
                if (_yammerPromptText != value)
                {
                    _yammerPromptText = value;
                    OnPropertyChanged("YammerPromptText");
                    OnMetadataChanged("Yammer.PromptText", _yammerPromptText);
                }
            }
        }

        public string YammerObjectUrl
        {
            get
            {
                return _yammerObjectUrl;
            }
            set
            {
                if (_yammerObjectUrl != value)
                {
                    _yammerObjectUrl = value;
                    OnPropertyChanged("YammerObjectUrl");
                    OnMetadataChanged("Yammer.ObjectUrl", _yammerObjectUrl);
                }
            }
        }

        public string YammerFeedID
        {
            get
            {
                return _yammerFeedId;
            }
            set
            {
                if (_yammerFeedId != value)
                {
                    _yammerFeedId = value;
                    OnPropertyChanged("YammerFeedID");
                    OnMetadataChanged("Yammer.FeedId", _yammerFeedId);
                }
            }
        }

        public bool DisableActivityFeed
        {
            get
            {
                return _disableActivityFeed;
            }
            set
            {
                if (_disableActivityFeed != value)
                {
                    _disableActivityFeed = value;
                    OnPropertyChanged("DisableActivityFeed");
                    if (value)
                    {
                        OnMetadataChanged("ShowActivityFeed", "False");
                    }
                    else
                    {
                        OnMetadataChanged("ShowActivityFeed", "True");
                    }
                }
            }
        }

        public bool DisableInsightsFeed
        {
            get
            {
                return _disableInsightsFeed;
            }
            set
            {
                if (_disableInsightsFeed != value)
                {
                    _disableInsightsFeed = value;
                    OnPropertyChanged("DisableInsightsFeed");
                    if (value)
                    {
                        OnMetadataChanged("ShowInsightsFeed", "False");
                    }
                    else
                    {
                        OnMetadataChanged("ShowInsightsFeed", "True");
                    }
                }
            }
        }

        private void ReloadYammerMetadata()
        {
            if (Node.Metadata.FindMetadata("Yammer.Network") != null)
            {
                var yammerNetworkValue = Node.Metadata.FindMetadata("Yammer.Network").Value;
                if (yammerNetworkValue.Length > 0)
                {
                    _yammerNetwork = yammerNetworkValue;
                    _yammerEnabled = true;
                }
                else
                {
                    _yammerNetwork = string.Empty;
                }
                ReloadYammerFeedTypeMetadata();
            }
            else
            {
                _yammerNetwork = string.Empty;
                ReloadYammerFeedTypeMetadata();
            }

            if (_yammerEnabled && Node.Metadata.FindMetadata("Yammer.PromptText") != null)
            {
                var value = Node.Metadata.FindMetadata("Yammer.PromptText").Value;
                if (value.Length > 0)
                {
                    _yammerPromptText = value;
                }
                else
                {
                    _yammerPromptText = string.Empty;
                }
            }
        }

        private void ReloadYammerFeedTypeMetadata()
        {
            if (Node.Metadata.FindMetadata("Yammer.FeedType") != null)
            {
                var feedTypeValue = Node.Metadata.FindMetadata("Yammer.FeedType").Value;
                string feedType = feedTypeValue.ToLower();
                if (feedType == "group" || feedType == "topic" || feedType == "user" || feedType == "open-graph")
                {
                    _yammerEnabled = true;
                    _yammerFeedType = feedType;

                    if (feedType == "group" || feedType == "topic" || feedType == "user")
                    {
                        if (Node.Metadata.FindMetadata("Yammer.FeedId") != null)
                        {
                            var feedIdVal = Node.Metadata.FindMetadata("Yammer.FeedId").Value;
                            _yammerFeedId = feedIdVal;
                        }
                        else
                        {
                            _yammerFeedId = string.Empty;
                        }
                    }
                    else if (feedType == "open-graph")
                    {
                        if (Node.Metadata.FindMetadata("Yammer.ObjectUrl") != null)
                        {
                            var objectUrlVal = Node.Metadata.FindMetadata("Yammer.ObjectUrl").Value;
                            _yammerObjectUrl = objectUrlVal;
                        }
                        else
                        {
                            _yammerObjectUrl = string.Empty;
                        }
                    }
                }
                else
                {
                    _yammerEnabled = false;
                    _yammerFeedType = string.Empty;
                }
            }
        }

        public override void ReloadMetadata()
        {
            base.ReloadMetadata();
            ReloadYammerMetadata();

            if (Node.Metadata.FindMetadata("AllowAutoRealign") != null)
            {
                var value = Node.Metadata.FindMetadata("AllowAutoRealign").Value;
                if (value == "False" || value == "false" || value == "0")
                {
                    _disableAutoRealign = true;
                }
                else
                {
                    _disableAutoRealign = false;
                }
            }
            else
            {
                _disableAutoRealign = false;
            }

            if (Node.Metadata.FindMetadata("ShowInsightsFeed") != null)
            {
                var value = Node.Metadata.FindMetadata("ShowInsightsFeed").Value;
                if (value == "true" || value == "True")
                {
                    _disableInsightsFeed = false;
                }
                else
                {
                    _disableInsightsFeed = true;
                }
            }
            else
            {
                _disableInsightsFeed = false;
            }

            if (Node.Metadata.FindMetadata("ShowActivityFeed") != null)
            {
                var value = Node.Metadata.FindMetadata("ShowActivityFeed").Value;
                if (value == "true" || value == "True")
                {
                    _disableActivityFeed = false;
                }
                else
                {
                    _disableActivityFeed = true;
                }
            }
            else
            {
                _disableActivityFeed = false;
            }

            OnPropertyChanged("AllowAutoRealign");
            OnPropertyChanged("YammerEnabled");
            OnPropertyChanged("YammerNetwork");
            OnPropertyChanged("YammerFeedType");
            OnPropertyChanged("YammerObjectUrl");
            OnPropertyChanged("YammerFeedID");
            OnPropertyChanged("YammerPromptText");
            OnPropertyChanged("DisableInsightsFeed");
            OnPropertyChanged("DisableActivityFeed");
        }
    }
}
