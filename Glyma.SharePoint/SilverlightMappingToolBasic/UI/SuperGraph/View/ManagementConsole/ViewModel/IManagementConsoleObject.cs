using System;
using System.ComponentModel;
using SilverlightMappingToolBasic.GlymaSecurityService;
using SilverlightMappingToolBasic.UI.Extensions.Security;
using TransactionalNodeService.Proxy;

namespace SilverlightMappingToolBasic.UI.SuperGraph.View.ManagementConsole.ViewModel
{
    public interface IManagementConsoleObject : INotifyPropertyChanged
    {
        event EventHandler<MetadataChangedEventArgs> MetadataChanged; 
        event EventHandler<PermissionValueChangedEventArgs> PermissionChanged;
        event EventHandler LoadCompleted;

        bool IsLoaded { get; set; }

        bool IsInherited { get; }

        bool IsSelected { get; set; }

        string Name { get; set; }

        PermissionLevel CurrentUserPermissionLevel { get; set; }

        string DisplayName { get; }

        INode Node { get; }

        Guid Id { get; }

        Guid ParentId { get; }

        bool HasNoSecurityAssociations { get; }

        PermissionGroupCollection Gpm { get; }

        PermissionGroupCollection Gmm { get; }

        PermissionGroupCollection Gma { get; }

        PermissionGroupCollection Gmr { get; }

        void Load(SecurityAssociations data, bool isInherited);

        void LoadValue(GlymaSecurityGroup group, bool value);

        bool GetValue(GlymaSecurityGroup group);

        void ResetChange(GlymaSecurityGroup group);

        void ReloadMetadata();

        GlymaSecurityAssociation CommitChange(GlymaSecurityGroup group, bool value);

    }
}
