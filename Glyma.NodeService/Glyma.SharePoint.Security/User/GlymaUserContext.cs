using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Glyma.Security;

namespace Glyma.SharePoint.Security
{
    public class GlymaUserContext
    {
        private SPUser _currentUser = null;
        private SecurityContextManager _contextManager = null;

        internal SPUser CurrentSPUser
        {
            get
            {
                if (_currentUser == null)
                {
                    try
                    {
                        using (SPSite site = new SPSite(_contextManager.WebUrl))
                        {
                            using (SPWeb currentWeb = site.OpenWeb())
                            {
                                _currentUser = currentWeb.CurrentUser;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Failed to get the current user.", ex);
                    }
                }
                return _currentUser;
            }
        }

        public bool IsSiteAdmin
        {
            get 
            {
                bool result = false;
                try
                {
                    using (SPSite site = new SPSite(_contextManager.WebUrl))
                    {
                        using (SPWeb currentWeb = site.OpenWeb())
                        {
                            result = currentWeb.CurrentUser.IsSiteAdmin;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to get the current user a site admin.", ex);
                }
                return result;
            }
        }

        private SecurityContextManager Context
        {
            get
            {
                return _contextManager;
            }
        }

        public GlymaUserContext(SecurityContextManager context)
        {
            _contextManager = context;
        }

        internal IGlymaPermission GetHighestPermissionLevel()
        {
            foreach (IGlymaPermission permissionLevel in Context.SortedPermissionLevels)
            {
                if (permissionLevel.CurrentUserHasPermission())
                {
                    return permissionLevel;
                }
            }

            return null;
        }

        internal bool IsUserSecurityManager()
        {
            bool hasAccess = IsUserAuthorized(GlymaPermissionLevel.GlymaSecurityManager);
            return hasAccess;
        }

        internal bool IsUserMapReader()
        {
            bool hasAccess = IsUserAuthorized(GlymaPermissionLevel.GlymaMapReader);
            return hasAccess;
        }

        internal bool IsUserMapAuthor()
        {
            bool hasAccess = IsUserAuthorized(GlymaPermissionLevel.GlymaMapAuthor);
            return hasAccess;
        }

        internal bool IsUserMapManager()
        {
            bool hasAccess = IsUserAuthorized(GlymaPermissionLevel.GlymaMapManager);
            return hasAccess;
        }

        internal bool IsUserProjectManager()
        {
            bool hasAccess = IsUserAuthorized(GlymaPermissionLevel.GlymaProjectManager);
            return hasAccess;
        }

        private bool IsUserAuthorized(GlymaPermissionLevel permissionLevel)
        {
            bool hasAccess = false;
            try
            {
                IGlymaPermission highestPermissionLevel = this.GetHighestPermissionLevel();

                if (GlymaPermissionLevelHelper.IsPermissionGreaterOrEqual(highestPermissionLevel.PermissionLevel, permissionLevel))
                {
                    hasAccess = true;
                }
            }
            catch (Exception)
            {
                hasAccess = false;
            }
            return hasAccess;
        }

        /// </summary>
        /// <param name="webUrl">The URL for the SP site</param>
        /// <param name="securableObject">An object that contains the Parent and Object ID's 
        ///                                 SecurableParentUid: The ID of the securable parent (Guid.Empty for projects), 
        ///                                 SecurableObjectUid: The ID of the securable object (root map UID or project UID if securing a project)</param>
        /// <param name="checkProjectsChildren">If this is true when checking the access to a Project if there are any root maps under that project the user
        /// has access to it returns true for the project as well (only true for when working out the filtered lists)</param>
        /// <returns>True if the user belongs to a group that has access to the securable object</returns>
        internal GetCurrentUserAccessToObjectResponse GetCurrentUserAccessToObject(GlymaSecurableObject securableObject, bool checkProjectsChildren = false)
        {
            GetCurrentUserAccessToObjectResponse result = new GetCurrentUserAccessToObjectResponse() { 
                HasError = false, 
                HasAccess = false, 
                HighestPermissionLevel = GlymaPermissionLevel.None 
            };

            try
            {
                using (SPSite site = new SPSite(Context.WebUrl))
                {
                    using (SPWeb currentWeb = site.OpenWeb())
                    {
                        IGlymaPermission highestPermissionLevel = this.GetHighestPermissionLevel();
                        if (highestPermissionLevel.PermissionLevel == GlymaPermissionLevel.None)
                        {
                            result.HasAccess = false;
                            result.HighestPermissionLevel = GlymaPermissionLevel.None;
                            return result; //an error occured so assume there is no access to the object
                        }
                        else
                        {
                            if (highestPermissionLevel.PermissionLevel == GlymaPermissionLevel.GlymaSecurityManager)
                            {
                                //The Glyma Security Manager permission exists for this user, they can access anything
                                result.HasAccess = true;
                                result.HighestPermissionLevel = GlymaPermissionLevel.GlymaSecurityManager;
                                return result;
                            }
                        }

                        GetAllSecurityGroupsResponse allSPSecurityGroups = Context.GetAllGlymaSecurityGroups();
                        if (!allSPSecurityGroups.HasError)
                        {
                            //GlymaGroupCollection groups = new GlymaGroupCollection(allSPSecurityGroups.Result);
                            GlymaSecurityGroupCollection groups = new GlymaSecurityGroupCollection(Context, allSPSecurityGroups.Result);

                            //gets a sorted list of groups highest to lowest permission level
                            IList<GlymaSecurityGroup> usersGlymaGroups = groups.GetUsersGroups(currentWeb, CurrentSPUser);

                            SecurableContext securableContext = Context.GetSecurableContext();

                            //check each glyma group the person has associated with them for access to the maps
                            foreach (GlymaSecurityGroup glymaGroup in usersGlymaGroups)
                            {
                                GlymaSecurityAssociationContext securityAssociation = new GlymaSecurityAssociationContext(Context, glymaGroup, securableObject);
                                bool response = securityAssociation.HasAssociation(checkProjectsChildren);

                                if (response)
                                {
                                    result.HasAccess = response;
                                    result.HighestPermissionLevel = groups.GetGroupsPermissionLevel(glymaGroup);
                                    return result;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                result.HasError = true;
                result.ErrorMessage = "Failed to read the users current access to the object. " + e.Message;
            }
            return result; //if it gets all the way to here it's the default no access response
        }

        /// <summary>
        /// This method is called by a Glyma Project Manager when they create a new project, it will associate any Glyma Project Manager group the user belongs to
        /// with the newly created project.
        /// </summary>
        /// <param name="securableObject">Describes the project that was just added</param>
        /// <returns>A response object indicating if the operation completed without error.</returns>
        internal ResponseObject SetProjectManagerGroupAssociations(GlymaSecurableObject securableObject)
        {
            ResponseObject result = new ResponseObject() { HasError = false };

            try
            {
                if (this.IsUserProjectManager()) //ensure they are a project manager
                {
                    using (SPSite site = new SPSite(Context.WebUrl))
                    {
                        using (SPWeb currentWeb = site.OpenWeb())
                        {
                            GetSecurityGroupsResponse response = Context.GetSecurityGroups(GlymaPermissionLevel.GlymaProjectManager);
                            if (!response.HasError)
                            {
                                IList<GlymaSecurityGroup> pmGroupsToAssociate = new List<GlymaSecurityGroup>();
                                IList<GlymaSecurityGroup> pmGroups = response.Result;

                                //for any group that is a Glyma Project Manager group
                                foreach (SPGroup group in CurrentSPUser.Groups)
                                {
                                    foreach (GlymaSecurityGroup projectManagerGroup in pmGroups)
                                    {
                                        Group glGroup = Context.GetGroup(projectManagerGroup);
                                        if (group.ID == glGroup.GroupSPID)
                                        {
                                            pmGroupsToAssociate.Add(projectManagerGroup);
                                        }
                                    }
                                }

                                //Add the security association for every Glyma Project Manager group the current user belongs to.
                                foreach (GlymaSecurityGroup glGroup in pmGroupsToAssociate)
                                {
                                    GlymaSecurityAssociationContext securityAssociationContext = new GlymaSecurityAssociationContext(Context, glGroup, securableObject);
                                    ResponseObject addResponse = securityAssociationContext.SetSecurityAssociation(false);
                                    if (addResponse.HasError)
                                    {
                                        //if an error occurs adding the security association for any of the groups stop and return the error
                                        result.HasError = true;
                                        result.ErrorMessage = addResponse.ErrorMessage;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                //there was an error get the groups that have been assigned the permission level of Glyma Project Manager
                                result.HasError = true;
                                result.ErrorMessage = response.ErrorMessage;
                            }
                        }
                    }
                }
                else
                {
                    //Only a Glyma Project Manager can call this method
                    result.HasError = true;
                    result.ErrorMessage = "Access Denied. User does not have permissions to access this web service method.";
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
    }
}
