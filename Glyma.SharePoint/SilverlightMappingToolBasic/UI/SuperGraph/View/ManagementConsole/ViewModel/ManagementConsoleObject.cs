using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using SilverlightMappingToolBasic.GlymaSecurityService;
using SilverlightMappingToolBasic.UI.Extensions.Security;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.ViewModel
{
    public abstract class ManagementConsoleObject : IManagementConsoleObject
    {
        private string _name;
        private string _uiName;
        private readonly INode _node;
        private bool _isLoaded;
        private bool _isInherited;
        private ObservableCollection<PermissionGroupCollection> _permissionGroups;
        private PermissionLevel _currentUserPermissionLevel = PermissionLevel.None;

        public event EventHandler<PermissionValueChangedEventArgs> PermissionChanged;
        public event EventHandler<MetadataChangedEventArgs> MetadataChanged; 
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler LoadCompleted;

        public PermissionLevel CurrentUserPermissionLevel
        {
            get
            {
                return _currentUserPermissionLevel;
            }
            set
            {
                if (_currentUserPermissionLevel != value)
                {
                    _currentUserPermissionLevel = value;
                    OnPropertyChanged("IsEditable");
                    OnPropertyChanged("YammerEnabled");
                }
            }
        }

        public bool IsEditable
        {
            get
            {
                if (this is Project)
                {
                    return CurrentUserPermissionLevel >= PermissionLevel.ProjectManager;
                }
                return CurrentUserPermissionLevel >= PermissionLevel.Author;
            }
        }

        public SolidColorBrush FontColor
        {
            get
            {
                return new SolidColorBrush(IsEditable? Colors.Black : Colors.DarkGray);
            }
        }

        public bool IsLoaded {
            get
            {
                return _isLoaded;
                
            }
            set
            {
                if (_isLoaded != value)
                {
                    _isLoaded = value;
                    OnPropertyChanged("LoaderVisibility");
                    OnPropertyChanged("TabVisibility");
                    OnPropertyChanged("RootMapOnlyControlVisibility");
                    OnPropertyChanged("ExportControlVisibility");
                    OnPropertyChanged("YammerOpenGraphControlVisibility");
                    OnPropertyChanged("YammerGroupControlVisibility");
                    OnPropertyChanged("IsEditable");
                    OnPropertyChanged("FontColor");
                }
            } 
        }

        public bool HasNoSecurityAssociations
        {
            get
            {
                bool hasNoSecurityAssociations = GetHasNoSecurityAssociations();
                return hasNoSecurityAssociations;
            }
        }

        public string UIName
        {
            get
            {
                if (_uiName == null)
                {
                    _uiName = Name;
                }
                return _uiName;
            }
            set
            {
                if (_uiName != value)
                {
                    _uiName = value;
                    OnPropertyChanged("UIName");
                }
            }
        }

        public bool IsInherited {
            get
            {
                return _isInherited;
                
            }
            set
            {
                if (_isInherited != value)
                {
                    _isInherited = value;
                    OnPropertyChanged("BreakInheritanceButtonVisibility");
                    OnPropertyChanged("RestoreInheritanceButtonVisibility");
                }
            } 
        }

        public virtual bool IsSelected { get; set; }

        public abstract string DisplayName { get; }

        public bool IsRootMap
        {
            get { return this is RootMap; }
        }

        public Visibility LoaderVisibility
        {
            get
            {
                return IsLoaded ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility TabVisibility
        {
            get
            {
                return IsLoaded ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility RootMapOnlyControlVisibility
        {
            get
            {
                return IsLoaded ? ((this is Project) ? Visibility.Collapsed : Visibility.Visible) : Visibility.Collapsed;
            }
        }

        public Visibility ExportControlVisibility
        {
            get { return App.IsExportEnabled ? RootMapOnlyControlVisibility : Visibility.Collapsed; }
        }


        public Visibility YammerOpenGraphControlVisibility
        {
            get
            {
                bool isProject = this is Project;
                if (isProject)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    RootMap rootMap = this as RootMap;
                    if (rootMap != null)
                    {
                        if (rootMap.YammerFeedType == "open-graph")
                        {
                            return Visibility.Visible;
                        }
                        else
                        {
                            return Visibility.Collapsed;
                        }
                    }
                    return Visibility.Collapsed;
                }
            }
        }

        public Visibility YammerGroupControlVisibility
        {
            get
            {
                bool isProject = this is Project;
                if (isProject)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    RootMap rootMap = this as RootMap;
                    if (rootMap != null)
                    {
                        if (rootMap.YammerFeedType == "group" || rootMap.YammerFeedType == "topic" || rootMap.YammerFeedType == "user")
                        {
                            return Visibility.Visible;
                        }
                        else
                        {
                            return Visibility.Collapsed;
                        }
                    }
                    return Visibility.Collapsed;
                }
            }
        }

        public Visibility ExpanderVisibility
        {
            get
            {
                return (this is Project) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility BreakInheritanceButtonVisibility
        {
            get
            {
                return IsInherited ? Visibility.Visible :Visibility.Collapsed;
            }
        }

        public Visibility RestoreInheritanceButtonVisibility
        {
            get
            {
                return IsInherited ? Visibility.Collapsed : Visibility.Visible;
                
            }
        }

        public Visibility SelectedIndicatorVisiblity
        {
            get { return IsSelected ? Visibility.Visible : Visibility.Collapsed; }
        }

        public ObservableCollection<PermissionGroupCollection> PermissionGroups
        {
            get
            {
                if (_permissionGroups == null)
                {
                    _permissionGroups = new ObservableCollection<PermissionGroupCollection>
                    {
                        Gpm,
                        Gmm,
                        Gma,
                        Gmr
                    };
                }
                return _permissionGroups;
            }
        } 

        public PermissionGroupCollection GetPermissionGroup(PermissionLevel permissionLevel)
        {
            switch (permissionLevel.ToString())
            {
                case PermissionLevel.AuthorRoleName:
                    return Gma;
                case PermissionLevel.ReaderRoleName:
                    return Gmr;
                case PermissionLevel.ProjectManagerRoleName:
                    return Gpm;
                case PermissionLevel.MapManagerRoleName:
                    return Gmm;
                default:
                    return null;
            }
        }

        protected ManagementConsoleObject(INode node, Dictionary<PermissionLevel, ObservableCollection<GlymaSecurityGroup>> template, bool isEnabled = true)
        {
            _node = node;
            _name = node.Metadata.FindMetadata("Name").Value;
            foreach (var valuePair in template)
            {
                var group = new PermissionGroupCollection(valuePair.Key);
                foreach (var item in valuePair.Value)
                {
                    group.Add(new PermissionGroup(item, false, isEnabled));
                }

                switch (valuePair.Key.ToString())
                {
                    case PermissionLevel.AuthorRoleName:
                        Gma = group;
                        break;
                    case PermissionLevel.ReaderRoleName:
                        Gmr = group;
                        break;
                    case PermissionLevel.ProjectManagerRoleName:
                        Gpm = group;
                        break;
                    case PermissionLevel.MapManagerRoleName:
                        Gmm = group;
                        break;
                }
            }
        }

        public void Load(SecurityAssociations data, bool isInherited)
        {
            IsInherited = isInherited;//!data.IsInherited;
            foreach (var valuePair in data.HasAssociations)
            {
                foreach (var permissionGroup in PermissionGroups)
                {
                    var found = permissionGroup.FirstOrDefault(q => q.Id == valuePair.Key.GroupId);
                    if (found != null)
                    {
                        found.ResetValue(valuePair.Value);
                        found.IsEnabled = !isInherited;//!data.IsInherited;
                    }
                }
            }
            IsLoaded = true;
            if (LoadCompleted != null)
            {
                LoadCompleted(this, null);
            }
        }

        public void LoadValue(GlymaSecurityGroup group, bool value)
        {
            foreach (var permissionGroup in PermissionGroups)
            {
                var found = permissionGroup.FirstOrDefault(q => q.Id == group.GroupId);
                if (found != null)
                {
                    found.SetValue(value);
                }
            }
        }

        public bool GetValue(GlymaSecurityGroup group)
        {
            foreach (var permissionGroup in PermissionGroups)
            {
                var found = permissionGroup.FirstOrDefault(q => q.Id == group.GroupId);
                if (found != null)
                {
                    return  found.IsSelected.HasValue && found.IsSelected.Value;
                }
            }
            return false;
        }

        public bool GetHasNoSecurityAssociations()
        {
            bool assocationFound = false;
            foreach (var permissionGroup in PermissionGroups)
            {
                foreach (var group in permissionGroup)
                {
                    if (group.IsSelected.HasValue && group.IsSelected.Value)
                    {
                        assocationFound = true;
                        break;
                    }
                }
                if (assocationFound)
                {
                    break;
                }
            }
            return !assocationFound;
        }

        public GlymaSecurityAssociation CommitChange(GlymaSecurityGroup group, bool value)
        {
            foreach (var permissionGroup in PermissionGroups)
            {
                var found = permissionGroup.FirstOrDefault(q => q.Id == group.GroupId);
                if (found != null)
                {
                    found.ResetValue(value);
                    return new GlymaSecurityAssociation
                    {
                        BreakInheritance = this is RootMap,
                        GlymaSecurityGroup = group,
                        SecurableObject = new GlymaSecurableObject
                        {
                            SecurableObjectUid = Id,
                            SecurableParentUid = this is RootMap ? ParentId : Guid.Empty,
                        },
                        
                        Value = value
                    };
                }
            }
            return null;
        }

        public virtual void ReloadMetadata()
        {
            _name = Node.Metadata.FindMetadata("Name").Value;
            _uiName = _name;
        }

        public void ResetChange(GlymaSecurityGroup group)
        {
            foreach (var permissionGroup in PermissionGroups)
            {
                var found = permissionGroup.FirstOrDefault(q => q.Id == group.GroupId);
                if (found != null)
                {
                    found.ResetValue();
                }
            }
        }

        public INode Node
        {
            get { return _node; }
        }

        public Guid Id
        {
            get
            {
                if (this is Project)
                {
                    return Node.DomainId; 
                }
                return Node.Id;
            }
        }

        public Guid ParentId
        {
            get { return Node.DomainId; }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    UIName = value;
                    OnPropertyChanged("Name");
                }
            }
        }


        private PermissionGroupCollection _gpm;
        private PermissionGroupCollection _gmm;
        private PermissionGroupCollection _gma;
        private PermissionGroupCollection _gmr; 

        public PermissionGroupCollection Gpm
        {
            get
            {
                if (_gpm == null)
                {
                    var group = new PermissionGroupCollection(PermissionLevel.ProjectManager);
                    _gpm = group;
                    _gpm.ItemPropertyChanged += GpmItemPropertyChanged;
                    
                }
                return _gpm;
            }
            set
            {
                _gpm = value;
                _gpm.ItemPropertyChanged += GpmItemPropertyChanged;
            }
        }

        public PermissionGroupCollection Gmm
        {
            get
            {
                if (_gmm == null)
                {
                    var group = new PermissionGroupCollection(PermissionLevel.MapManager);
                    _gmm = group;
                    _gmm.ItemPropertyChanged += GmmItemPropertyChanged;

                }
                return _gmm;
            }
            set
            {
                _gmm = value;
                _gmm.ItemPropertyChanged += GmmItemPropertyChanged;
            }
        }

        public PermissionGroupCollection Gma
        {
            get
            {
                if (_gma == null)
                {
                    var group = new PermissionGroupCollection(PermissionLevel.Author);
                    _gma = group;
                    _gma.ItemPropertyChanged += GmaItemPropertyChanged;

                }
                return _gma;
            }
            set
            {
                _gma = value;
                _gma.ItemPropertyChanged += GmaItemPropertyChanged;
            }
        }

        public PermissionGroupCollection Gmr
        {
            get
            {
                if (_gmr == null)
                {
                    var group = new PermissionGroupCollection(PermissionLevel.Reader);
                    _gmr = group;
                    _gmr.ItemPropertyChanged += GmrItemPropertyChanged;

                }
                return _gmr;
            }
            set
            {
                _gmr = value;
                _gmr.ItemPropertyChanged += GmrItemPropertyChanged;
            }
        }

        private void GpmItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "DefaultValue" && e.PropertyName != "IsEnabled")
            {
                OnPermissionChanged(sender as PermissionGroup, PermissionLevel.ProjectManager);
            }
            
        }

        private void GmmItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "DefaultValue" && e.PropertyName != "IsEnabled")
            {
                OnPermissionChanged(sender as PermissionGroup, PermissionLevel.MapManager);
            }
        }

        private void GmaItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "DefaultValue" && e.PropertyName != "IsEnabled")
            {
                OnPermissionChanged(sender as PermissionGroup, PermissionLevel.Author);
            }
        }

        private void GmrItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "DefaultValue" && e.PropertyName != "IsEnabled")
            {
                OnPermissionChanged(sender as PermissionGroup, PermissionLevel.Reader);
            }
        }

        private void OnPermissionChanged(PermissionGroup changedItem, PermissionLevel permissionLevel)
        {
            if (PermissionChanged != null)
            {
                var args = new PermissionValueChangedEventArgs { ChangedItem = changedItem, PermissionLevel = permissionLevel };
                PermissionChanged(this, args);
            }
        }

        protected void OnMetadataChanged(string key, string value)
        {
            if (MetadataChanged != null)
            {
                MetadataChanged(this, new MetadataChangedEventArgs
                {
                    Key = key,
                    Value = value
                });
            }
        }

        public void UpdateName(string value)
        {
            if (MetadataChanged != null)
            {
                MetadataChanged(this, new MetadataChangedEventArgs
                {
                    Key = "Name",
                    Value = value
                });
            }
            _uiName = value;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (propertyName == "UIName")
            {
                OnMetadataChanged("Name", UIName);
            }

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
