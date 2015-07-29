using System.Collections.Generic;
using System.Collections.ObjectModel;
using SilverlightMappingToolBasic.GlymaSecurityService;
using SilverlightMappingToolBasic.UI.Extensions.Security;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.ViewModel
{
    public class Project : ManagementConsoleObject
    {
        private ObservableCollection<RootMap> _rootMaps;

        public bool IsChildrenLoaded
        {
            get; set; 
        }

        public ObservableCollection<RootMap> RootMaps
        {
            get
            {
                if (_rootMaps == null)
                {
                    _rootMaps = new ObservableCollection<RootMap>();
                }
                return _rootMaps;
            }
            set { _rootMaps = value; }
        }

        private bool _isExpanded;
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

        private bool _isSelected;
        public override bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
                OnPropertyChanged("SelectedIndicatorVisiblity");
                if (_isSelected)
                {
                    foreach (var rootMap in RootMaps)
                    {
                        rootMap.IsSelected = false;
                    }
                }
            }
        }

        public override string DisplayName
        {
            get { return Name; }
        }

        public Project(INode node, Dictionary<PermissionLevel, ObservableCollection<GlymaSecurityGroup>> template)
            : base(node, template)
        {

        }

        public override void ReloadMetadata()
        {
            base.ReloadMetadata();
        }
    }
}
