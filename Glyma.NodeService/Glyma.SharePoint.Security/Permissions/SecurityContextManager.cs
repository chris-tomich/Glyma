using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TransactionalNodeService.Common;
using TransactionalNodeService.SharePoint;

using Glyma.Security;

namespace Glyma.SharePoint.Security
{
    public class SecurityContextManager
    {
        private IList<IGlymaPermission> _permissionLevels;
        private GlymaPermissionFactory _permissionFactory = null;
        private GlymaUserContext _currentUserContext = null;

        internal string WebUrl
        {
            get;
            private set;
        }

        private GlymaPermissionFactory GlymaPermissionFactory
        {
            get
            {
                if (_permissionFactory == null)
                {
                    _permissionFactory = new GlymaPermissionFactory(this);
                }
                return _permissionFactory;
            }
        }

        internal GlymaUserContext CurrentUser
        {
            get
            {
                if (_currentUserContext == null)
                {
                    _currentUserContext = new GlymaUserContext(this);
                }
                return _currentUserContext;
            }
        }

        internal IList<IGlymaPermission> SortedPermissionLevels
        {
            get
            {
                if (_permissionLevels == null)
                {
                    InitialisePermissionLevels();
                }
                return _permissionLevels;
            }
        }

        internal SecurityContextManager(string webUrl)
        {
            WebUrl = webUrl;
        }

        internal GetSecurableContextIdResponse GetSecurableContextId()
        {
            GetSecurableContextIdResponse result = new GetSecurableContextIdResponse() { HasError = false };
            if (this.CurrentUser.IsUserSecurityManager())
            {
                int securableContextId = -1;
                try
                {
                    using (SPSite site = new SPSite(WebUrl))
                    {
                        SecurableContext securableContext = this.GetSecurableContext();
                        if (securableContext != null)
                        {
                            securableContextId = securableContext.SecurableContextId;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //If an error occurs getting the security context id
                    result.HasError = true;
                    result.ErrorMessage = ex.Message;
                }

                if (!result.HasError)
                {
                    result.Result = securableContextId;
                }
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage = "Access Denied. User does not have permissions to access this web service method.";
            }

            return result;
        }

        internal SecurableObject GetSecurableObject(int securableContextId, Guid securableObjectUid)
        {
            SecurableObject result = null;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (IGlymaSession glymaSession = new WebAppSPGlymaSession(this.WebUrl))
                {
                    using (IDbConnectionAbstraction connectionAbstraction = glymaSession.ConnectionFactory.CreateSecurityDbConnection())
                    {
                        using (SecurityServiceDataContext dataContext = new SecurityServiceDataContext(connectionAbstraction.Connection))
                        {
                            var securableObjects = from so in dataContext.SecurableObjects
                                                   where so.SecurableObjectUid == securableObjectUid &&
                                                   so.SecurableContextId == securableContextId
                                                   select so;

                            SecurableObject securableObject = null; //default value if it doesn't exist
                            if (securableObjects.Any())
                            {
                                securableObject = securableObjects.First();
                            }
                            result = securableObject;
                        }
                    }
                }
            });
            return result;
        }
       
        /// <summary>
        /// Gets the current security associations for a list of groups against a particular securable object
        /// </summary>
        /// <param name="webUrl">The URL for the SP site</param>
        /// <param name="glGroups">A list of groups to get the security assocations for</param>
        /// <param name="securableObject">An object that contains the Parent and Object ID's 
        ///                                 SecurableParentUid: The ID of the securable parent (Guid.Empty for projects), 
        ///                                 SecurableObjectUid: The ID of the securable object (root map UID or project UID if securing a project)</param>
        /// <returns>A dictionary of security association, Key: the group, Value: True if the group has an assocation. (wrapped in a Response Object to indicate if any errors occured)</returns>
        internal GetSecurityAssociationsResponse GetSecurityAssociations(IEnumerable<GlymaSecurityGroup> glGroups, GlymaSecurableObject securableObject)
        {
            GetSecurityAssociationsResponse result = new GetSecurityAssociationsResponse() { HasError = false };

            if (this.CurrentUser.IsUserSecurityManager())
            {
                SecurityAssociations securityAssociations = new SecurityAssociations();
                Dictionary<GlymaSecurityGroup, bool> results = new Dictionary<GlymaSecurityGroup, bool>();
                SecurableContext securableContext = GetSecurableContext();
                int securableContextId = securableContext.SecurableContextId;
                GlymaSecurableObjectContext securableObjectContext = new GlymaSecurableObjectContext(this, securableContextId, securableObject);
                bool isInherited = securableObjectContext.GetIsInherited();

                foreach (GlymaSecurityGroup glymaSecurityGroup in glGroups)
                {
                    try
                    {
                        GlymaSecurityAssociationContext securityAssociationContext = new GlymaSecurityAssociationContext(this, glymaSecurityGroup, securableObject);
                        bool response = securityAssociationContext.HasAssociation();
                        results.Add(glymaSecurityGroup, response);
                    }
                    catch (Exception ex)
                    {
                        result.HasError = true;
                        result.ErrorMessage = ex.Message;
                    }
                }
                if (!result.HasError)
                {
                    securityAssociations.HasAssociations = results;
                    securityAssociations.IsInherited = isInherited;
                    result.Result = securityAssociations;
                }
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage = "Access Denied. User does not have permissions to access this web service method.";
            }

            return result;
        }

        internal GetAllSecurityGroupsResponse GetAllGlymaSecurityGroups()
        {
            GetAllSecurityGroupsResponse result = new GetAllSecurityGroupsResponse() { HasError = false };

            IList<string> permissionLevelNames = new List<string>();
            permissionLevelNames.Add(GlymaPermissionLevelHelper.GetPermissionLevelName(GlymaPermissionLevel.GlymaProjectManager));
            permissionLevelNames.Add(GlymaPermissionLevelHelper.GetPermissionLevelName(GlymaPermissionLevel.GlymaMapManager));
            permissionLevelNames.Add(GlymaPermissionLevelHelper.GetPermissionLevelName(GlymaPermissionLevel.GlymaMapAuthor));
            permissionLevelNames.Add(GlymaPermissionLevelHelper.GetPermissionLevelName(GlymaPermissionLevel.GlymaMapReader));
            permissionLevelNames.Add(GlymaPermissionLevelHelper.GetPermissionLevelName(GlymaPermissionLevel.GlymaMapReaderOld));
            permissionLevelNames.Add(GlymaPermissionLevelHelper.GetPermissionLevelName(GlymaPermissionLevel.GlymaMapAuthorOld));

            Dictionary<GlymaPermissionLevel, IList<GlymaSecurityGroup>> results = new Dictionary<GlymaPermissionLevel, IList<GlymaSecurityGroup>>();
            foreach (string permissionLevelName in permissionLevelNames)
            {
                GlymaPermissionLevel permissionLevel = GlymaPermissionLevelHelper.GetPermissionLevelByName(permissionLevelName);
                GetSecurityGroupsResponse response = GetSecurityGroups(permissionLevel);
                if (!response.HasError)
                {
                    IList<GlymaSecurityGroup> groups = response.Result;
                    if (results.ContainsKey(permissionLevel))
                    {
                        foreach (GlymaSecurityGroup group in groups)
                        {
                            if (!results[permissionLevel].Contains(group))
                            {
                                results[permissionLevel].Add(group);
                            }
                        }
                    }
                    else
                    {
                        results.Add(permissionLevel, groups);
                    }
                }
                else
                {
                    result.HasError = true;
                    result.ErrorMessage = response.ErrorMessage;
                    break; //an error occurred so stop at this point
                }
            }
            if (!result.HasError)
            {
                GlymaSecurityGroupCollection groups = new GlymaSecurityGroupCollection(this, results);
                IDictionary<GlymaPermissionLevel, IList<GlymaSecurityGroup>> filteredGroups = groups.FilterGroups();
                result.Result = filteredGroups;
            }

            return result;
        }

        

        /// <summary>
        /// Returns a list presenting the SharePoint Security Groups for the current web that have a specified permission associated with them
        /// </summary>
        /// <param name="webUrl">The URL for the SP site</param>
        /// <param name="permissionLevel">The permission level the groups must have</param>
        /// <returns>A list of groups (wrapped by a ResponseObject)</returns>
        internal GetSecurityGroupsResponse GetSecurityGroups(GlymaPermissionLevel permissionLevel)
        {
            GetSecurityGroupsResponse result = new GetSecurityGroupsResponse() { HasError = false };
            IList<GlymaSecurityGroup> results = new List<GlymaSecurityGroup>();

            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(WebUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPRoleDefinition roleDefinition = null;
                            try
                            {
                                // Check if the role exists, if it does a definition will exist
                                roleDefinition = web.RoleDefinitions[GlymaPermissionLevelHelper.GetPermissionLevelName(permissionLevel)];
                            }
                            catch (Exception)
                            {
                                //if unable to find the role definition it will throw an exception
                            }

                            if (roleDefinition != null)
                            {
                                SPRoleAssignmentCollection roleAssignments = web.RoleAssignments;
                                foreach (SPRoleAssignment roleAssignment in roleAssignments)
                                {
                                    bool hasRoleDefinition = false;
                                    foreach (
                                        SPRoleDefinition definition in roleAssignment.RoleDefinitionBindings)
                                    {
                                        if (definition.Id == roleDefinition.Id)
                                        {
                                            //The role exists for this role assignment
                                            hasRoleDefinition = true;
                                            break;
                                        }
                                    }

                                    if (hasRoleDefinition)
                                    {
                                        SPGroup group = roleAssignment.Member as SPGroup;
                                        //we only want to look at groups
                                        if (group != null)
                                        {
                                            GlymaSecurityGroup glymaGroup = new GlymaSecurityGroup();
                                            glymaGroup.DisplayName = group.Name;

                                            SecurableContext securableContext = this.GetSecurableContext();
                                            glymaGroup.SecurableContextId = securableContext.SecurableContextId;

                                            GlymaSecurityGroupContext sgc = new GlymaSecurityGroupContext(this, securableContext.SecurableContextId, group.ID, web.ID);
                                            Group glGroup = sgc.GetGroup(group.Name);
                                            if (glGroup == null)
                                            {
                                                glGroup = sgc.CreateGroup(group.Name);
                                            }
                                            if (glGroup != null)
                                            {
                                                glymaGroup.GroupId = glGroup.GroupId;
                                                results.Add(glymaGroup);
                                            }
                                            else
                                            {
                                                result.HasError = true;
                                                result.ErrorMessage = "Failed to create the Group in the Glyma Security Database.";
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                results = new List<GlymaSecurityGroup>(); //there was no role by this name, it has no groups
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                //If an error occurs getting the group listing return no groups
                result.HasError = true;
                result.ErrorMessage = ex.Message;

            }
            if (!result.HasError)
            {
                result.Result = results;
            }
            return result;
        }

        /// <summary>
        /// Gets the securable context identifier for the siteID
        /// </summary>
        /// <returns>The Securable Context ID associated with the SP Site ID or -1 if it doesn't exist</returns>
        internal SecurableContext GetSecurableContext()
        {
            SecurableContext result = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (IGlymaSession glymaSession = new WebAppSPGlymaSession(this.WebUrl))
                {
                    using (IDbConnectionAbstraction connectionAbstraction = glymaSession.ConnectionFactory.CreateSecurityDbConnection())
                    {
                        using (SecurityServiceDataContext dataContext = new SecurityServiceDataContext(connectionAbstraction.Connection))
                        {
                            var securableContext = from sc in dataContext.SecurableContexts
                                                   where sc.SecurableContextId == glymaSession.SecurableContextId
                                                   select sc;
                            if (securableContext.Any())
                            {
                                result = securableContext.First();
                            }
                        }
                    }
                }
            });
            return result;
        }

        private void InitialisePermissionLevels() 
        {
            _permissionLevels = new List<IGlymaPermission>();
            _permissionLevels.Add(GlymaPermissionFactory.CreateGlymaPermission(GlymaPermissionLevel.GlymaSecurityManager));
            _permissionLevels.Add(GlymaPermissionFactory.CreateGlymaPermission(GlymaPermissionLevel.GlymaProjectManager));
            _permissionLevels.Add(GlymaPermissionFactory.CreateGlymaPermission(GlymaPermissionLevel.GlymaMapManager));
            _permissionLevels.Add(GlymaPermissionFactory.CreateGlymaPermission(GlymaPermissionLevel.GlymaMapAuthor));
            _permissionLevels.Add(GlymaPermissionFactory.CreateGlymaPermission(GlymaPermissionLevel.GlymaMapReader));
        }

        internal ResponseObject BreakRootMapInheritance(GlymaSecurableObject securableObject)
        {
            ResponseObject response = new ResponseObject() { HasError = false };
            try
            {
                GetSecurableContextIdResponse securableContextIdResponse = GetSecurableContextId();
                if (!securableContextIdResponse.HasError)
                {
                    int securableContextId = securableContextIdResponse.Result;
                    SecurableObject obj = GetSecurableObject(securableContextId, securableObject.SecurableObjectUid);
                    GlymaSecurableObjectContext securableObjectContext = new GlymaSecurableObjectContext(this, securableContextId, securableObject);
                    if (obj == null)
                    {
                        obj = securableObjectContext.CreateSecurableObject(true);
                    }
                    if (!obj.BreaksInheritance)
                    {
                        securableObjectContext.SetSecurableObjectInheritance(true);
                    }
                    CopyGroupAssociationsToRootMap(securableObject);
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }

            return response;
        }

        private void CopyGroupAssociationsToRootMap(GlymaSecurableObject rootMapSecurableObject)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (IGlymaSession glymaSession = new WebAppSPGlymaSession(this.WebUrl))
                {
                    using (IDbConnectionAbstraction connectionAbstraction = glymaSession.ConnectionFactory.CreateSecurityDbConnection())
                    {
                        using (SecurityServiceDataContext dataContext = new SecurityServiceDataContext(connectionAbstraction.Connection))
                        {
                            var groupAssociations = from ga in dataContext.GroupAssociations
                                                    where ga.SecurableObjectUid == rootMapSecurableObject.SecurableParentUid
                                                    select ga;

                            if (groupAssociations.Any())
                            {
                                foreach (GroupAssociation groupAssociation in groupAssociations)
                                {
                                    GlymaSecurableObject securableObject = new GlymaSecurableObject();
                                    securableObject.SecurableParentUid = groupAssociation.SecurableObjectUid; //the parent is now the project uid
                                    securableObject.SecurableObjectUid = rootMapSecurableObject.SecurableObjectUid; //the object is now the root map being copied to
                                    GlymaSecurityAssociationContext securityAssocationContext = new GlymaSecurityAssociationContext(this, rootMapSecurableObject);
                                    securityAssocationContext.CreateGroupAssociation(groupAssociation.GroupId);
                                }
                            }
                        }
                    }
                }
            });
        }

        private void RemoveRootMapGroupAssociations(GlymaSecurableObject rootMapSecurableObject)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (IGlymaSession glymaSession = new WebAppSPGlymaSession(this.WebUrl))
                {
                    using (IDbConnectionAbstraction connectionAbstraction = glymaSession.ConnectionFactory.CreateSecurityDbConnection())
                    {
                        using (SecurityServiceDataContext dataContext = new SecurityServiceDataContext(connectionAbstraction.Connection))
                        {
                            var groupAssociations = from ga in dataContext.GroupAssociations
                                                    where ga.SecurableObjectUid == rootMapSecurableObject.SecurableObjectUid &&
                                                    ga.SecurableParentUid == rootMapSecurableObject.SecurableParentUid
                                                    select ga;

                            if (groupAssociations.Any())
                            {
                                foreach (GroupAssociation groupAssociation in groupAssociations)
                                {
                                    dataContext.GroupAssociations.DeleteOnSubmit(groupAssociation);
                                }
                                dataContext.SubmitChanges();
                            }
                        }
                    }
                }
            });
        }

        internal GetSecurityAssociationsResponse RestoreRootMapInheritance(GlymaSecurableObject securableObject)
        {
            GetSecurityAssociationsResponse response = new GetSecurityAssociationsResponse() { HasError = false };
            try
            {
                GetSecurableContextIdResponse securableContextIdResponse = GetSecurableContextId();
                if (!securableContextIdResponse.HasError)
                {
                    int securableContextId = securableContextIdResponse.Result;
                    SecurableObject obj = GetSecurableObject(securableContextId, securableObject.SecurableObjectUid);
                    GlymaSecurableObjectContext securableObjectContext = new GlymaSecurableObjectContext(this, securableContextId, securableObject);
                    if (obj == null)
                    {
                        obj = securableObjectContext.CreateSecurableObject(false);
                    }
                    if (obj.BreaksInheritance)
                    {
                        securableObjectContext.SetSecurableObjectInheritance(false);
                    }
                    RemoveRootMapGroupAssociations(securableObject);

                    GlymaSecurableObject parentObject = new GlymaSecurableObject();
                    parentObject.SecurableParentUid = Guid.Empty;
                    parentObject.SecurableObjectUid = securableObject.SecurableParentUid;
                    GetAllSecurityGroupsResponse res = GetAllGlymaSecurityGroups();
                    if (!res.HasError)
                    {
                        IList<GlymaSecurityGroup> groups = ConversionUtility.ConvertDictToList(res.Result);
                        response = GetSecurityAssociations(groups, parentObject);
                    }
                    else
                    {
                        response.HasError = true;
                        response.ErrorMessage = "Failed returning the Glyma security groups. " + res.ErrorMessage;
                    }
                }
                else
                {
                    response.HasError = true;
                    response.ErrorMessage = "Failed to restore root map inheritance. " + securableContextIdResponse.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
            }
            return response;
        }

        internal Group GetGroup(GlymaSecurityGroup securityGroup)
        {
            Group result = null;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (IGlymaSession glymaSession = new WebAppSPGlymaSession(this.WebUrl))
                {
                    using (IDbConnectionAbstraction connectionAbstraction = glymaSession.ConnectionFactory.CreateSecurityDbConnection())
                    {
                        using (SecurityServiceDataContext dataContext = new SecurityServiceDataContext(connectionAbstraction.Connection))
                        {
                            var groups = from g in dataContext.Groups
                                         where g.GroupId == securityGroup.GroupId
                                         && g.SecurableContextId == securityGroup.SecurableContextId
                                         select g;

                            Group group = null; //default value if it doesn't exist
                            if (groups.Any())
                            {
                                group = groups.First();
                            }
                            result = group;
                        }
                    }
                }
            });
            return result;
        }
    }
}
