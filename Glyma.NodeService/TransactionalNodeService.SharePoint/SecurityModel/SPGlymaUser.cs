using Glyma.Security.Model;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using TransactionalNodeService.Common;
using TransactionalNodeService.SharePoint.SecurityModel.SupportClasses;

namespace TransactionalNodeService.SharePoint.SecurityModel
{
    public class SPGlymaUser : IUser
    {
        private SqlConnection _sqlConnection = null;

        private const string GlymaProjectManagerRoleName = "Glyma Project Manager";
        private const string GlymaMapManagerRoleName = "Glyma Root Map Manager";
        private const string GlymaMapAuthorRoleName = "Glyma Map Author";
        private const string GlymaMapReaderRoleName = "Glyma Map Reader";
        private const string OldGlymaMapAuthorRoleName = "Glyma Author";
        private const string OldGlymaMapReaderRoleName = "Glyma Reader";

        public SPGlymaUser(SPWeb web, IGlymaSession glymaSession)
        {
            Web = web;
            GlymaSession = glymaSession;

            SecurableContextId = GlymaSession.SecurableContextId;

            SecurityDBDataContext dataContext = null;
            IDbConnectionAbstraction securityDbConnection = GlymaSession.ConnectionFactory.CreateSecurityDbConnection();

            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    dataContext = new SecurityDBDataContext(securityDbConnection.Connection);

                    var securableContext = (from dcSecurableContext in dataContext.SecurableContexts
                                            where dcSecurableContext.SecurableContextId == SecurableContextId
                                            select dcSecurableContext).FirstOrDefault();

                    if (securableContext == null)
                    {
                        return;
                    }

                    SecurableContextName = securableContext.SecurableContextName;
                    SecurableContextUid = securableContext.SecurableContextUid;
                });
            }
            finally
            {
                if (dataContext != null)
                {
                    dataContext.Dispose();
                    dataContext = null;
                }

                if (securityDbConnection != null)
                {
                    securityDbConnection.Dispose();
                    securityDbConnection = null;
                }
            }
        }

        public SPWeb Web
        {
            get;
            private set;
        }

        public IGlymaSession GlymaSession
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

        private SPSite GetElevatedSite()
        {
            SPSite _elevatedSite = null;

            if (Web != null)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    _elevatedSite = new SPSite(Web.Url);
                });
            }

            return _elevatedSite;
        }

        private SPWeb GetElevatedWeb(SPSite elevatedSite)
        {
            SPWeb _elevatedWeb = null;

            if (elevatedSite != null)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    _elevatedWeb = elevatedSite.OpenWeb();
                });
            }

            return _elevatedWeb;
        }

        private IRole GetRole(string roleName)
        {
            IRole role;

            switch (roleName)
            {
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
                    role = null;
                    break;
            }

            return role;
        }

        public bool IsAuthorised(params IRight[] requiredRights)
        {
            /// This user is a Glyma Security Manager, they have access to everything.
            if (Web.UserIsSiteAdmin)
            {
                return true;
            }

            /// We need to filter out only the rights for which kind of query is going to be permitted. In this case only the following rights should be allowed.
            /// 1. All Glyma Security Manager rights as they have unrestricted access to the entire securable context.
            /// 2. Glyma Project Manager create right as they have unrestricted rights to create new domains.
            /// 3. The transaction right as this is a requires transaction to do anything other than reading.
            /// 4. The benign access right as this right doesn't actually have access to do anything and should be given to anyone that has access to the system, even unauthorised people.
            List<IRight> rightsPermittedForThisQuery = new List<IRight>();

            foreach (IRight requiredRight in requiredRights)
            {
                if (requiredRight == SPGlymaRightFactory.Instance.SecurableContextManageRight ||
                    requiredRight == SPGlymaRightFactory.Instance.RootMapCreateRight ||
                    requiredRight == SPGlymaRightFactory.Instance.TransactionRight ||
                    requiredRight == SPGlymaRightFactory.Instance.BenignAccessRight)
                {
                    rightsPermittedForThisQuery.Add(requiredRight);
                }
            }

            IRight[] permittedRights = rightsPermittedForThisQuery.ToArray();

            SPSite elevatedSite = null;
            SPWeb elevatedWeb = null;

            try
            {
                elevatedSite = GetElevatedSite();
                elevatedWeb = GetElevatedWeb(elevatedSite);

                foreach (SPRoleAssignment roleAssignment in elevatedWeb.RoleAssignments)
                {
                    SPGroup elevatedGroup = roleAssignment.Member as SPGroup;

                    if (elevatedGroup != null)
                    {
                        SPGroup group = Web.Groups.GetByID(elevatedGroup.ID);

                        /// Check that we're actually looking at a group, that this group contains the current user, and that this group is on the list of accessible groups.
                        if (group != null && group.ContainsCurrentUser)
                        {
                            /// Do a check that this group actually has the required right.
                            foreach (SPRoleDefinition roleDefinition in roleAssignment.RoleDefinitionBindings)
                            {
                                IRole role = GetRole(roleDefinition.Name);

                                if (role != null && role.HasRights(permittedRights))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (elevatedWeb != null)
                {
                    elevatedWeb.Dispose();
                    elevatedWeb = null;
                }

                if (elevatedSite != null)
                {
                    elevatedSite.Dispose();
                    elevatedSite = null;
                }
            }

            return false;
        }

        public bool IsAuthorised(Guid domainUid, params IRight[] requiredRights)
        {
            return IsAuthorised(null, domainUid, requiredRights);
        }

        public bool IsAuthorised(Guid? securableParentUid, Guid securableObjectUids, params IRight[] requiredRights)
        {
            HashSet<Guid> accessibleObjects;

            accessibleObjects = IsAuthorisedHashSet(securableParentUid, new Guid[] { securableObjectUids }, requiredRights);

            return accessibleObjects.Contains(securableObjectUids);
        }

        public IEnumerable<Guid> IsAuthorised(IEnumerable<Guid> domainUids, params IRight[] requiredRights)
        {
            return IsAuthorised(null, domainUids, requiredRights);
        }

        public IEnumerable<Guid> IsAuthorised(Guid? securableParentUid, IEnumerable<Guid> securableObjectUids, params IRight[] requiredRights)
        {
            return IsAuthorisedHashSet(securableParentUid, securableObjectUids, requiredRights);
        }

        private HashSet<Guid> IsAuthorisedHashSet(Guid? securableParentUid, IEnumerable<Guid> securableObjectUids, params IRight[] requiredRights)
        {
            /// This user is a Glyma Security Manager, they have access to everything.
            if (Web.UserIsSiteAdmin)
            {
                return new HashSet<Guid>(securableObjectUids);
            }

            IDbConnectionAbstraction securityDbConnection = GlymaSession.ConnectionFactory.CreateSecurityDbConnection();
            SecurityDBDataContext dataContext = null;
            SPSite elevatedSite = null;
            SPWeb elevatedWeb = null;

            try
            {
                GroupAssociationsOrderedByGroupSPId groupAssociations = new GroupAssociationsOrderedByGroupSPId();
                SecurableObjectGenealogyTracker geneaologyTracker = new SecurableObjectGenealogyTracker();

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    dataContext = new SecurityDBDataContext(securityDbConnection.Connection);

                    if (securableParentUid != null)
                    {
                        IQueryable<SecurableObject> securableObjectInheritanceInfo = from securableObject in dataContext.SecurableObjects
                                                                                     where securableObject.SecurableContextId == SecurableContextId
                                                                                     select securableObject;

                        MultipleOrExpressionFilter<SecurableObject, Guid> securableObjectInheritanceInfoFilter = new MultipleOrExpressionFilter<SecurableObject, Guid>(securableObjectInheritanceInfo, securableObjectUids, "SecurableObjectUid");

                        IQueryable<SecurableObject> filteredSecurableObjectInheritanceInfo = securableObjectInheritanceInfoFilter.FilterResultSet();

                        geneaologyTracker.AddRange(securableParentUid, filteredSecurableObjectInheritanceInfo);

                        geneaologyTracker.AddRangeIfNotPreExisting(securableParentUid, securableObjectUids);
                    }
                    else
                    {
                        geneaologyTracker.AddRange(null, securableObjectUids);

                        IQueryable<GroupAssociation> groupsAssocationsWithBrokenInheritance = from securableObjectGroupAssocation in dataContext.GroupAssociations
                                                                                              where securableObjectGroupAssocation.SecurableContextId == SecurableContextId && securableObjectGroupAssocation.SecurableObject.BreaksInheritance == true
                                                                                              select securableObjectGroupAssocation;

                        MultipleOrExpressionFilter<GroupAssociation, Guid> securableObjectInheritanceInfoFilter = new MultipleOrExpressionFilter<GroupAssociation, Guid>(groupsAssocationsWithBrokenInheritance, securableObjectUids, "SecurableParentUid");

                        IQueryable<GroupAssociation> filteredSecurableObjectInheritanceInfo = securableObjectInheritanceInfoFilter.FilterResultSet();

                        groupAssociations.AddRange(filteredSecurableObjectInheritanceInfo);

                        foreach (GroupAssociation securableObjectWithBrokenInheritance in filteredSecurableObjectInheritanceInfo)
                        {
                            geneaologyTracker.Add(securableObjectWithBrokenInheritance.SecurableParentUid, securableObjectWithBrokenInheritance.SecurableObjectUid);
                        }
                    }

                    /// Get the group associations for only the securable objects.
                    IQueryable<GroupAssociation> allAccessibleGroupAssociations;

                    /// The following needs to be done as Linq doesn't detect the null value in Nullable types and so doesn't generate the correct "IS NULL" SQL.
                    if (securableParentUid.HasValue && geneaologyTracker.HasIndependentObjects && geneaologyTracker.HasInheritingObjects)
                    {
                        allAccessibleGroupAssociations = from securableObject in dataContext.GroupAssociations
                                                         where (securableObject.SecurableContextId == SecurableContextId && securableObject.SecurableParentUid == securableParentUid) || (securableObject.SecurableContextId == SecurableContextId && securableObject.SecurableParentUid == null)
                                                         select securableObject;
                    }
                    else if (securableParentUid.HasValue && geneaologyTracker.HasIndependentObjects)
                    {
                        allAccessibleGroupAssociations = from securableObject in dataContext.GroupAssociations
                                                         where securableObject.SecurableContextId == SecurableContextId && securableObject.SecurableParentUid == securableParentUid
                                                         select securableObject;
                    }
                    else /// This means the tracker has only either inheriting objects or domains.
                    {
                        allAccessibleGroupAssociations = from securableObject in dataContext.GroupAssociations
                                                         where securableObject.SecurableContextId == SecurableContextId && securableObject.SecurableParentUid == null
                                                         select securableObject;
                    }

                    MultipleOrExpressionFilter<GroupAssociation, Guid> groupAssocationFilter = new MultipleOrExpressionFilter<GroupAssociation, Guid>(allAccessibleGroupAssociations, geneaologyTracker.GetPermissionSources(), "SecurableObjectUid");

                    IQueryable<GroupAssociation> filteredGroupAssociations = groupAssocationFilter.FilterResultSet();

                    groupAssociations.AddRange(filteredGroupAssociations);
                });

                AccessibleObjectsBuilder accessibleObjects = new AccessibleObjectsBuilder();

                elevatedSite = GetElevatedSite();
                elevatedWeb = GetElevatedWeb(elevatedSite);

                foreach (SPRoleAssignment roleAssignment in elevatedWeb.RoleAssignments)
                {
                    SPGroup elevatedGroup = roleAssignment.Member as SPGroup;

                    if (elevatedGroup != null)
                    {
                        SPGroup group = Web.Groups.GetByID(elevatedGroup.ID);

                        /// Check that we're actually looking at a group, and that this group is on the list of accessible groups, and that this group contains the current user.
                        if (group != null && group.ContainsCurrentUser)
                        {
                            /// Do a check that this group actually has the required right.
                            foreach (SPRoleDefinition roleDefinition in roleAssignment.RoleDefinitionBindings)
                            {
                                IRole role = GetRole(roleDefinition.Name);

                                IEnumerable<GroupAssociation> accessibleParentObjects;

                                if (groupAssociations.GetBySPId(group.ID, out accessibleParentObjects))
                                {
                                    if (role != null && role.HasRights(requiredRights))
                                    {
                                        IEnumerable<Guid> accessibleSecurableObjects = geneaologyTracker.GetSecurableObjectsByPermissionSources(accessibleParentObjects);
                                        accessibleObjects.AddRange(accessibleSecurableObjects);
                                    }
                                }
                            }
                        }
                    }
                }

                return accessibleObjects.GetAccessibleObjects();
            }
            finally
            {
                if (dataContext != null)
                {
                    dataContext.Dispose();
                    dataContext = null;
                }

                if (elevatedWeb != null)
                {
                    elevatedWeb.Dispose();
                    elevatedWeb = null;
                }

                if (elevatedSite != null)
                {
                    elevatedSite.Dispose();
                    elevatedSite = null;
                }

                if (securityDbConnection != null)
                {
                    securityDbConnection.Dispose();
                    securityDbConnection = null;
                }
            }
        }
    }
}