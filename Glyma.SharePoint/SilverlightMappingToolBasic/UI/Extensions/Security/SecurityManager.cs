using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using SilverlightMappingToolBasic.GlymaSecurityService;

namespace SilverlightMappingToolBasic.UI.Extensions.Security
{
    public class SecurityManager
    {
        private static readonly object Lock = new object();

        private Dictionary<Guid, object> _objectDictionary; 

        private GlymaSecurityServiceClient _client;

        private Dictionary<Guid, EventHandler<GetPermissionNameForObjectCompletedEventArgs>> _getPermissionNameForObjectCompletedEventHandlers;

        private Dictionary<Guid, EventHandler<GetSecurityAssociationsCompletedEventArgs>> _getSecurityAssociationsCompletedEventHandlers;

        private Dictionary<Guid, EventHandler<GetAllSecurityGroupsCompletedEventArgs>> _getAllSecurityGroupsCompletedEventHandlers;

        private Dictionary<Guid, EventHandler<UpdateSecurityAssociationsCompletedEventArgs>> _updateSecurityAssociationsCompletedEventHandlers;

        private Dictionary<Guid, EventHandler<SetProjectManagerGroupAssociationsCompletedEventArgs>> _setProjectManagerGroupAssociationsCompletedEventHandlers;

        private Dictionary<Guid, EventHandler<BreakRootMapInheritanceCompletedEventArgs>> _breakRootMapInheritanceCompletedEventHandlers;

        private Dictionary<Guid, EventHandler<RestoreRootMapInheritanceCompletedEventArgs>> _restoreRootMapInheritanceCompletedEventHandlers;

        private Dictionary<Guid, EventHandler<GetUsersPermissionLevelNameCompletedEventArgs>> _getUsersPermissionLevelNameCompletedEventHandlers;

        private Dictionary<Guid, object> ObjectDictionary
        {
            get
            {
                if (_objectDictionary == null)
                {
                    _objectDictionary = new Dictionary<Guid, object>();
                }
                return _objectDictionary;
            }
        }

        private Dictionary<Guid, EventHandler<GetPermissionNameForObjectCompletedEventArgs>> GetPermissionNameForObjectCompletedEventHandlers
        {
            get
            {
                if (_getPermissionNameForObjectCompletedEventHandlers == null)
                {
                    _getPermissionNameForObjectCompletedEventHandlers = new Dictionary<Guid, EventHandler<GetPermissionNameForObjectCompletedEventArgs>>();
                }
                return _getPermissionNameForObjectCompletedEventHandlers;
            }
        }

        private Dictionary<Guid, EventHandler<GetSecurityAssociationsCompletedEventArgs>> GetSecurityAssociationsCompletedEventHandlers
        {
            get
            {
                if (_getSecurityAssociationsCompletedEventHandlers == null)
                {
                    _getSecurityAssociationsCompletedEventHandlers = new Dictionary<Guid, EventHandler<GetSecurityAssociationsCompletedEventArgs>>();
                }
                return _getSecurityAssociationsCompletedEventHandlers;
            }
        }

        private Dictionary<Guid, EventHandler<GetAllSecurityGroupsCompletedEventArgs>> GetAllSecurityGroupsCompletedEventHandlers
        {
            get
            {
                if (_getAllSecurityGroupsCompletedEventHandlers == null)
                {
                    _getAllSecurityGroupsCompletedEventHandlers = new Dictionary<Guid, EventHandler<GetAllSecurityGroupsCompletedEventArgs>>();
                }
                return _getAllSecurityGroupsCompletedEventHandlers;
            }
        }

        private Dictionary<Guid, EventHandler<UpdateSecurityAssociationsCompletedEventArgs>> UpdateSecurityAssociationsCompletedEventHandlers
        {
            get
            {
                if (_updateSecurityAssociationsCompletedEventHandlers == null)
                {
                    _updateSecurityAssociationsCompletedEventHandlers = new Dictionary<Guid, EventHandler<UpdateSecurityAssociationsCompletedEventArgs>>();
                }
                return _updateSecurityAssociationsCompletedEventHandlers;
            }
        }

        private Dictionary<Guid, EventHandler<SetProjectManagerGroupAssociationsCompletedEventArgs>> SetProjectManagerGroupAssociationsCompletedEventHandlers
        {
            get
            {
                if (_setProjectManagerGroupAssociationsCompletedEventHandlers == null)
                {
                    _setProjectManagerGroupAssociationsCompletedEventHandlers = new Dictionary<Guid, EventHandler<SetProjectManagerGroupAssociationsCompletedEventArgs>>();
                }
                return _setProjectManagerGroupAssociationsCompletedEventHandlers;
            }
        }

        private Dictionary<Guid, EventHandler<BreakRootMapInheritanceCompletedEventArgs>> BreakRootMapInheritanceCompletedEventHandlers
        {
            get
            {
                if (_breakRootMapInheritanceCompletedEventHandlers == null)
                {
                    _breakRootMapInheritanceCompletedEventHandlers = new Dictionary<Guid, EventHandler<BreakRootMapInheritanceCompletedEventArgs>>();
                }
                return _breakRootMapInheritanceCompletedEventHandlers;
            }
        }

        private Dictionary<Guid, EventHandler<RestoreRootMapInheritanceCompletedEventArgs>> RestoreRootMapInheritanceCompletedEventHandlers
        {
            get
            {
                if (_restoreRootMapInheritanceCompletedEventHandlers == null)
                {
                    _restoreRootMapInheritanceCompletedEventHandlers = new Dictionary<Guid, EventHandler<RestoreRootMapInheritanceCompletedEventArgs>>();
                }
                return _restoreRootMapInheritanceCompletedEventHandlers;
            }
        }

        private Dictionary<Guid, EventHandler<GetUsersPermissionLevelNameCompletedEventArgs>> GetUsersPermissionLevelNameCompletedEventHandlers
        {
            get
            {
                if (_getUsersPermissionLevelNameCompletedEventHandlers == null)
                {
                    _getUsersPermissionLevelNameCompletedEventHandlers = new Dictionary<Guid, EventHandler<GetUsersPermissionLevelNameCompletedEventArgs>>();
                }
                return _getUsersPermissionLevelNameCompletedEventHandlers;
            }
        }

        public GlymaSecurityServiceClient Client
        {
            get
            {
                lock (Lock)
                {
                    if (_client == null || _client.State == CommunicationState.Closed ||
                    _client.State == CommunicationState.Closing || _client.State == CommunicationState.Faulted)
                    {
                        var binding = new BasicHttpBinding();
                        binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                        binding.MaxReceivedMessageSize = 2147483647;
                        var address = new EndpointAddress(new Uri(App.Params.GlymaSecuritySvcUrl));
                        _client = new GlymaSecurityServiceClient(binding, address);
                        _client.GetPermissionNameForObjectCompleted += ClientOnGetPermissionNameForObjectCompleted;
                        _client.GetSecurityAssociationsCompleted += ClientOnGetSecurityAssociationsCompleted;
                        _client.GetAllSecurityGroupsCompleted += ClientOnGetAllSecurityGroupsCompleted;
                        _client.UpdateSecurityAssociationsCompleted += ClientOnUpdateSecurityAssociationsCompleted;
                        _client.SetProjectManagerGroupAssociationsCompleted += ClientOnSetProjectManagerGroupAssociationsCompleted;
                        _client.BreakRootMapInheritanceCompleted += ClientOnBreakRootMapInheritanceCompleted;
                        _client.RestoreRootMapInheritanceCompleted += ClientOnRestoreRootMapInheritanceCompleted;
                        _client.GetUsersPermissionLevelNameCompleted += ClientOnGetUsersPermissionLevelNameCompleted;
                    }
                    return _client;
                }

            }
        }

        private void ClientOnGetUsersPermissionLevelNameCompleted(object sender, GetUsersPermissionLevelNameCompletedEventArgs e)
        {
            if (e.UserState is Guid)
            {
                var guid = (Guid)e.UserState;
                if (ObjectDictionary.ContainsKey(guid) &&
                    GetUsersPermissionLevelNameCompletedEventHandlers.ContainsKey(guid))
                {
                    GetUsersPermissionLevelNameCompletedEventHandlers[guid](ObjectDictionary[guid], e);
                    GetUsersPermissionLevelNameCompletedEventHandlers.Remove(guid);
                    ObjectDictionary.Remove(guid);
                }
            }
        }
        
        private void ClientOnGetPermissionNameForObjectCompleted(object sender, GetPermissionNameForObjectCompletedEventArgs e)
        {
            if (e.UserState is Guid)
            {
                var guid = (Guid) e.UserState;
                if (ObjectDictionary.ContainsKey(guid) &&
                    GetPermissionNameForObjectCompletedEventHandlers.ContainsKey(guid))
                {
                    //back compatibility with old version
                    if (App.PermissionLevel == PermissionLevel.OldReader)
                    {
                        e.Result.Result = PermissionLevel.Reader.ToString();
                    }
                    else if (App.PermissionLevel == PermissionLevel.OldAuthor)
                    {
                        e.Result.Result = PermissionLevel.Author.ToString();
                    }
                    GetPermissionNameForObjectCompletedEventHandlers[guid](ObjectDictionary[guid], e);
                    GetPermissionNameForObjectCompletedEventHandlers.Remove(guid);
                    ObjectDictionary.Remove(guid);
                }
            }
            
            
        }

        private void ClientOnGetSecurityAssociationsCompleted(object sender, GetSecurityAssociationsCompletedEventArgs e)
        {
            if (e.UserState is Guid)
            {
                var guid = (Guid) e.UserState;
                if (ObjectDictionary.ContainsKey(guid) &&
                    GetSecurityAssociationsCompletedEventHandlers.ContainsKey(guid))
                {
                    GetSecurityAssociationsCompletedEventHandlers[guid](ObjectDictionary[guid], e);
                    GetSecurityAssociationsCompletedEventHandlers.Remove(guid);
                    ObjectDictionary.Remove(guid);
                }
            }
        }

        private void ClientOnGetAllSecurityGroupsCompleted(object sender, GetAllSecurityGroupsCompletedEventArgs e)
        {
            if (e.UserState is Guid)
            {
                var guid = (Guid) e.UserState;
                if (ObjectDictionary.ContainsKey(guid) && GetAllSecurityGroupsCompletedEventHandlers.ContainsKey(guid))
                {
                    GetAllSecurityGroupsCompletedEventHandlers[guid](ObjectDictionary[guid], e);
                    GetAllSecurityGroupsCompletedEventHandlers.Remove(guid);
                    ObjectDictionary.Remove(guid);
                }
            }
        }

        private void ClientOnUpdateSecurityAssociationsCompleted(object sender, UpdateSecurityAssociationsCompletedEventArgs e)
        {
            if (e.UserState is Guid)
            {
                var guid = (Guid) e.UserState;
                if (ObjectDictionary.ContainsKey(guid) &&
                    UpdateSecurityAssociationsCompletedEventHandlers.ContainsKey(guid))
                {
                    UpdateSecurityAssociationsCompletedEventHandlers[guid](ObjectDictionary[guid], e);
                    UpdateSecurityAssociationsCompletedEventHandlers.Remove(guid);
                    ObjectDictionary.Remove(guid);
                }
            }
        }

        private void ClientOnSetProjectManagerGroupAssociationsCompleted(object sender, SetProjectManagerGroupAssociationsCompletedEventArgs e)
        {
            if (e.UserState is Guid)
            {
                var guid = (Guid) e.UserState;
                if (ObjectDictionary.ContainsKey(guid) &&
                    SetProjectManagerGroupAssociationsCompletedEventHandlers.ContainsKey(guid))
                {
                    SetProjectManagerGroupAssociationsCompletedEventHandlers[guid](ObjectDictionary[guid], e);
                    SetProjectManagerGroupAssociationsCompletedEventHandlers.Remove(guid);
                    ObjectDictionary.Remove(guid);
                }
            }
        }

        private void ClientOnBreakRootMapInheritanceCompleted(object sender, BreakRootMapInheritanceCompletedEventArgs e)
        {
            if (e.UserState is Guid)
            {
                var guid = (Guid) e.UserState;
                if (ObjectDictionary.ContainsKey(guid) &&
                    BreakRootMapInheritanceCompletedEventHandlers.ContainsKey(guid))
                {
                    BreakRootMapInheritanceCompletedEventHandlers[guid](ObjectDictionary[guid], e);
                    BreakRootMapInheritanceCompletedEventHandlers.Remove(guid);
                    ObjectDictionary.Remove(guid);

                }
            }
        }


        private void ClientOnRestoreRootMapInheritanceCompleted(object sender, RestoreRootMapInheritanceCompletedEventArgs e)
        {
            if (e.UserState is Guid)
            {
                var guid = (Guid) e.UserState;
                if (ObjectDictionary.ContainsKey(guid) &&
                    RestoreRootMapInheritanceCompletedEventHandlers.ContainsKey(guid))
                {
                    RestoreRootMapInheritanceCompletedEventHandlers[guid](ObjectDictionary[guid], e);
                    RestoreRootMapInheritanceCompletedEventHandlers.Remove(guid);
                    ObjectDictionary.Remove(guid);
                }
            }
        }

        public void GetUsersPermissionLevelNameAsync(object context, EventHandler<GetUsersPermissionLevelNameCompletedEventArgs> action)
        {
            var guid = Guid.NewGuid();
            ObjectDictionary.Add(guid, context);
            GetUsersPermissionLevelNameCompletedEventHandlers.Add(guid, action);
            Client.GetUsersPermissionLevelNameAsync(App.Params.SiteUrl, guid);
        }

        public void GetPermissionNameForObjectAsync(object context, EventHandler<GetPermissionNameForObjectCompletedEventArgs> action, GlymaSecurableObject securableObject)
        {
            var guid = Guid.NewGuid();
            ObjectDictionary.Add(guid, context);
            GetPermissionNameForObjectCompletedEventHandlers.Add(guid, action);
            Client.GetPermissionNameForObjectAsync(App.Params.SiteUrl, securableObject, guid);
        }

        public void GetSecurityAssociationsAsync(object context, EventHandler<GetSecurityAssociationsCompletedEventArgs> action, ObservableCollection<GlymaSecurityGroup> groups, GlymaSecurableObject securableObject)
        {
            var guid = Guid.NewGuid();
            ObjectDictionary.Add(guid, context);
            GetSecurityAssociationsCompletedEventHandlers.Add(guid, action);
            Client.GetSecurityAssociationsAsync(App.Params.SiteUrl, groups, securableObject, guid);
        }

        public void GetAllSecurityGroupsAsync(object context, EventHandler<GetAllSecurityGroupsCompletedEventArgs> action)
        {
            var guid = Guid.NewGuid();
            ObjectDictionary.Add(guid, context);
            GetAllSecurityGroupsCompletedEventHandlers.Add(guid, action);
            Client.GetAllSecurityGroupsAsync(App.Params.SiteUrl, guid);
        }

        public void UpdateSecurityAssociationsAsync(object context, EventHandler<UpdateSecurityAssociationsCompletedEventArgs> action, ObservableCollection<GlymaSecurityAssociation> updateQueries)
        {
            var guid = Guid.NewGuid();
            ObjectDictionary.Add(guid, context);
            UpdateSecurityAssociationsCompletedEventHandlers.Add(guid, action);
            Client.UpdateSecurityAssociationsAsync(App.Params.SiteUrl, updateQueries, guid);
        }

        public void SetProjectManagerGroupAssociationsAsync(object context, EventHandler<SetProjectManagerGroupAssociationsCompletedEventArgs> action, GlymaSecurableObject securableObject)
        {
            var guid = Guid.NewGuid();
            ObjectDictionary.Add(guid, context);
            SetProjectManagerGroupAssociationsCompletedEventHandlers.Add(guid, action);
            Client.SetProjectManagerGroupAssociationsAsync(App.Params.SiteUrl, securableObject, guid);
        }


        public void BreakRootMapInheritanceAsync(object context, EventHandler<BreakRootMapInheritanceCompletedEventArgs> action, Guid parentGuid, Guid objectGuid)
        {
            var guid = Guid.NewGuid();
            ObjectDictionary.Add(guid, context);
            BreakRootMapInheritanceCompletedEventHandlers.Add(guid, action);
            Client.BreakRootMapInheritanceAsync(App.Params.SiteUrl, new GlymaSecurableObject { SecurableParentUid = parentGuid, SecurableObjectUid = objectGuid }, guid);
            
        }

        public void RestoreRootMapInheritanceAsync(object context, EventHandler<RestoreRootMapInheritanceCompletedEventArgs> action, Guid parentGuid, Guid objectGuid)
        {
            var guid = Guid.NewGuid();
            ObjectDictionary.Add(guid, context);
            RestoreRootMapInheritanceCompletedEventHandlers.Add(guid, action);
            Client.RestoreRootMapInheritanceAsync(App.Params.SiteUrl, new GlymaSecurableObject { SecurableParentUid = parentGuid, SecurableObjectUid = objectGuid }, guid);
        }
    }
}
