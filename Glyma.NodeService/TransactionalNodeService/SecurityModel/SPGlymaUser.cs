using Glyma.Security.Model;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using TransactionalNodeService.Common;
using TransactionalNodeService.SecurityModel;

namespace TransactionalNodeService.SPSecurityModel
{
    public class SPGlymaUser : IUser
    {
        private SqlConnection _sqlConnection = null;

        private const string GlymaSecurityManagerRoleName = "Glyma Security Manager";
        private const string GlymaProjectManagerRoleName = "Glyma Project Manager";
        private const string GlymaMapManagerRoleName = "Glyma Root Map Manager";
        private const string GlymaMapAuthorRoleName = "Glyma Map Author";
        private const string GlymaMapReaderRoleName = "Glyma Map Reader";
        private const string OldGlymaMapAuthorRoleName = "Glyma Author";
        private const string OldGlymaMapReaderRoleName = "Glyma Reader";

        public SPGlymaUser(SPWeb web, IMapObjects mapObjectBootstrapper)
        {
            Web = web;
            MapObjectBootstrapper = mapObjectBootstrapper;

            if (web.Properties.ContainsKey("Glyma.SecurityDatabaseServer") && web.Properties.ContainsKey("Glyma.SecurityDatabaseName") && web.Properties.ContainsKey("Glyma.SecurableContextId"))
            {
                SecurityDatabaseServer = web.Properties["Glyma.SecurityDatabaseServer"];
                SecurityDatabaseName = web.Properties["Glyma.SecurityDatabaseName"];
                SecurableContextId = int.Parse(web.Properties["Glyma.SecurableContextId"]);
            }

            using (SecurityDBDataContext dataContext = new SecurityDBDataContext(Connection))
            {
                var securableContext = (from dcSecurableContext in dataContext.SecurableContexts
                                       where dcSecurableContext.SecurableContextId == SecurableContextId
                                       select dcSecurableContext).FirstOrDefault();

                if (securableContext == null)
                {
                    return;
                }

                SecurableContextName = securableContext.SecurableContextName;
                SecurableContextUid = securableContext.SecurableContextUid;
            }
        }

        private string SecurityDatabaseServer
        {
            get;
            set;
        }

        private string SecurityDatabaseName
        {
            get;
            set;
        }

        private SqlConnection Connection
        {
            get
            {
                if (_sqlConnection == null)
                {
                    string connectionString = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=SSPI;", SecurityDatabaseServer, SecurityDatabaseName);

                    _sqlConnection = new SqlConnection(connectionString);
                }

                return _sqlConnection;
            }
        }

        public SPWeb Web
        {
            get;
            private set;
        }

        public IMapObjects MapObjectBootstrapper
        {
            get;
            private set;
        }

        public int? SecurableContextId
        {
            get;
            set;
        }

        public string SecurableContextName
        {
            get;
            private set;
        }

        public Guid? SecurableContextUid
        {
            get;
            private set;
        }

        public int UserId
        {
            get;
            set;
        }

        public string DisplayName
        {
            get;
            set;
        }

        public string Claim
        {
            get;
            set;
        }

        public bool IsAuthorised(Guid domainUid, params IRight[] requiredRights)
        {
            return IsAuthorised(null, domainUid, requiredRights);
        }

        public bool IsAuthorised(Guid? domainUid, Guid rootMapUid, params IRight[] requiredRights)
        {
            using (SecurityDBDataContext dataContext = new SecurityDBDataContext(Connection))
            {
                var accessibleGroups = from securableObject in dataContext.GroupAssociations
                                       where securableObject.SecurableContextId == SecurableContextId && securableObject.SecurableParentUid == domainUid && securableObject.SecurableObjectUid == rootMapUid && securableObject.Group.GroupSPID != null
                                       select securableObject.Group;

                Dictionary<int, Group> sortedGroups = accessibleGroups.ToDictionary(x => x.GroupSPID.Value);

                foreach (SPRoleAssignment roleAssignment in Web.RoleAssignments)
                {
                    SPGroup group = roleAssignment.Member as SPGroup;

                    /// Check that we're actually looking at a group, that this group contains the current user, and that this group is on the list of accessible groups.
                    if (group != null && group.ContainsCurrentUser && sortedGroups.ContainsKey(group.ID))
                    {
                        /// Do a check that this group actually has the required right.
                        foreach (SPRoleDefinition roleDefinition in roleAssignment.RoleDefinitionBindings)
                        {
                            IRole role = null;

                            switch (roleDefinition.Name)
                            {
                                case GlymaSecurityManagerRoleName:
                                    role = SPGlymaRoleFactory.GetInstance(SecurableContextId.Value).GlymaSecurityManagerRole;
                                    break;

                                case GlymaProjectManagerRoleName:
                                    role = SPGlymaRoleFactory.GetInstance(SecurableContextId.Value).GlymaProjectManagerRole;
                                    break;

                                case GlymaMapManagerRoleName:
                                    role = SPGlymaRoleFactory.GetInstance(SecurableContextId.Value).GlymaMapManagerRole;
                                    break;

                                case GlymaMapAuthorRoleName:
                                    role = SPGlymaRoleFactory.GetInstance(SecurableContextId.Value).GlymaMapAuthorRole;
                                    break;

                                case GlymaMapReaderRoleName:
                                    role = SPGlymaRoleFactory.GetInstance(SecurableContextId.Value).GlymaMapReaderRole;
                                    break;

                                case OldGlymaMapAuthorRoleName:
                                    role = SPGlymaRoleFactory.GetInstance(SecurableContextId.Value).GlymaMapAuthorRole;
                                    break;

                                case OldGlymaMapReaderRoleName:
                                    role = SPGlymaRoleFactory.GetInstance(SecurableContextId.Value).GlymaMapReaderRole;
                                    break;

                                default:
                                    role = SPGlymaRoleFactory.GetInstance(SecurableContextId.Value).GlymaMapReaderRole;
                                    break;
                            }

                            if (role.HasRights(requiredRights))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }

        public bool IsAuthorised(Guid[] domainUids, params IRight[] requiredRights)
        {
            return IsAuthorised(null, domainUids, requiredRights);
        }

        public bool IsAuthorised(Guid? domainUid, Guid[] rootMapUids, params IRight[] requiredRights)
        {
            using (SecurityDBDataContext dataContext = new SecurityDBDataContext(Connection))
            {
                /// Only get the securable objects for the securable context ID we have access and the domain ID we were provided.
                IQueryable<GroupAssociation> accessibleGroupAssociations = from securableObject in dataContext.GroupAssociations
                                                                           where securableObject.SecurableContextId == SecurableContextId && securableObject.SecurableParentUid == domainUid && securableObject.Group.GroupSPID != null
                                                                           select securableObject;

                /// Only get the securable objects for the root map IDs we were supplied with.
                foreach (Guid rootMapUid in rootMapUids)
                {
                    accessibleGroupAssociations = from securableObject in accessibleGroupAssociations
                                                  where securableObject.SecurableObjectUid == rootMapUid
                                                  select securableObject;
                }

                var accessibleGroups = from securableObject in accessibleGroupAssociations
                                       select securableObject.Group;

                Dictionary<int, Group> sortedGroups = accessibleGroups.ToDictionary(x => x.GroupSPID.Value);

                foreach (SPRoleAssignment roleAssignment in Web.RoleAssignments)
                {
                    SPGroup group = roleAssignment.Member as SPGroup;

                    /// Check that we're actually looking at a group, that this group contains the current user, and that this group is on the list of accessible groups.
                    if (group != null && group.ContainsCurrentUser && sortedGroups.ContainsKey(group.ID))
                    {
                        /// Do a check that this group actually has the required right.
                        foreach (SPRoleDefinition roleDefinition in roleAssignment.RoleDefinitionBindings)
                        {
                            IRole role = null;

                            switch (roleDefinition.Name)
                            {
                                case GlymaSecurityManagerRoleName:
                                    role = SPGlymaRoleFactory.GetInstance(SecurableContextId.Value).GlymaSecurityManagerRole;
                                    break;

                                case GlymaProjectManagerRoleName:
                                    role = SPGlymaRoleFactory.GetInstance(SecurableContextId.Value).GlymaProjectManagerRole;
                                    break;

                                case GlymaMapManagerRoleName:
                                    role = SPGlymaRoleFactory.GetInstance(SecurableContextId.Value).GlymaMapManagerRole;
                                    break;

                                case GlymaMapAuthorRoleName:
                                    role = SPGlymaRoleFactory.GetInstance(SecurableContextId.Value).GlymaMapAuthorRole;
                                    break;

                                case GlymaMapReaderRoleName:
                                    role = SPGlymaRoleFactory.GetInstance(SecurableContextId.Value).GlymaMapReaderRole;
                                    break;

                                case OldGlymaMapAuthorRoleName:
                                    role = SPGlymaRoleFactory.GetInstance(SecurableContextId.Value).GlymaMapAuthorRole;
                                    break;

                                case OldGlymaMapReaderRoleName:
                                    role = SPGlymaRoleFactory.GetInstance(SecurableContextId.Value).GlymaMapReaderRole;
                                    break;

                                default:
                                    role = SPGlymaRoleFactory.GetInstance(SecurableContextId.Value).GlymaMapReaderRole;
                                    break;
                            }

                            if (role.HasRights(requiredRights))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
        }
    }
}