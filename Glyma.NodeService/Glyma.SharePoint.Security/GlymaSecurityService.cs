using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Data.Linq;

using Glyma.Security;

using Microsoft.SharePoint.Client.Services;

namespace Glyma.SharePoint.Security
{
    [BasicHttpBindingServiceMetadataExchangeEndpoint]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single, Namespace = "http://sevensigma.com.au/GlymaSecurityService")]
    public class GlymaSecurityService : IGlymaSecurityService
    {
        public GlymaSecurityService()
        {
            
        }

        #region IGlymaSharePointSecurityService implementaion

        public GetIsUserGlymaSecurityManagerResponse GetIsUserGlymaSecurityManager(string webUrl)
        {
            GetIsUserGlymaSecurityManagerResponse response = new GetIsUserGlymaSecurityManagerResponse() { HasError = false };
            bool result = false;
            try 
            {
                SecurityContextManager contextManager = new SecurityContextManager(webUrl);
                result = contextManager.CurrentUser.IsUserSecurityManager();
            }
            catch (Exception ex) 
            {
                response.HasError = true;
                response.ErrorMessage = "Failed to discover if the current user is a Glyma Security Manager. " + ex.Message;
            }
            if (!response.HasError)
            {
                response.Result = result;
            }

            return response;
        }

        public GetAllSecurityGroupsResponse GetAllSecurityGroups(string webUrl)
        {
            GetAllSecurityGroupsResponse result = new GetAllSecurityGroupsResponse() { HasError = false };
            SecurityContextManager context = new SecurityContextManager(webUrl);
            if (context.CurrentUser.IsUserSecurityManager())
            {
                result = context.GetAllGlymaSecurityGroups();
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage = "Access Denied. User does not have permissions to access this web service method.";
            }

            return result;
        }

        /// <summary>
        /// Gets the highest permission level that a user has
        /// </summary>
        /// <param name="webUrl"></param>
        /// <returns></returns>
        public GetPermissionNameResponse GetUsersPermissionLevelName(string webUrl)
        {
            GetPermissionNameResponse result = new GetPermissionNameResponse() { HasError = false };
            
            GetPermissionLevelResponse response = GetUsersPermissionLevel(webUrl);
            if (!response.HasError)
            {
                result.Result = GlymaPermissionLevelHelper.GetPermissionLevelName(response.Result);
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage = response.ErrorMessage;
            }
            return result;
        }

        /// <summary>
        /// Gets the highest permission level that a user has
        /// </summary>
        /// <param name="webUrl"></param>
        /// <returns></returns>
        public GetPermissionLevelResponse GetUsersPermissionLevel(string webUrl)
        {
            GetPermissionLevelResponse result = new GetPermissionLevelResponse() { HasError = false };
            GlymaPermissionLevel permissionLevel = GlymaPermissionLevel.None;
            
            GetIsUserGlymaSecurityManagerResponse isSecurityManager = GetIsUserGlymaSecurityManager(webUrl);
            if (!isSecurityManager.HasError)
            {
                if (isSecurityManager.Result)
                {
                    permissionLevel = GlymaPermissionLevel.GlymaSecurityManager;
                    result.Result = permissionLevel;
                }
                else
                {
                    try
                    {
                        SecurityContextManager context = new SecurityContextManager(webUrl);
                        IGlymaPermission permission = context.CurrentUser.GetHighestPermissionLevel();
                        permissionLevel = permission.PermissionLevel;
                        result.Result = permissionLevel;
                    }
                    catch (Exception e)
                    {
                        result.HasError = true;
                        result.ErrorMessage = e.Message;
                    }
                }
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage = isSecurityManager.ErrorMessage;
            }
            return result;
        }

        /// <summary>
        /// Gets the security context id from the URL of the current site
        /// </summary>
        /// <param name="webUrl">The URL for the SP site</param>
        /// <returns>The SecurityContextId or -1 if it wasn't found (wrapped in a response object to indicate if it completed without error)</returns>
        public GetSecurableContextIdResponse GetSecurableContextId(string webUrl)
        {
            GetSecurableContextIdResponse result = new GetSecurableContextIdResponse() { HasError = false };
            SecurityContextManager securityContext = new SecurityContextManager(webUrl);
            result = securityContext.GetSecurableContextId();
            return result;
        }

        /// <summary>
        /// Adds or Removes security associations for a batch of groups.
        /// </summary>
        /// <param name="webUrl">The URL for the SP site</param>
        /// <param name="securityAssociations">The details of the group and the securable object and whether it's an add or remove operation</param>
        /// <returns>A response object to indicate if it completd without error.</returns>
        public ResponseObject UpdateSecurityAssociations(string webUrl, IList<GlymaSecurityAssociation> securityAssociations)
        {
            ResponseObject result = new ResponseObject() { HasError = false };

            SecurityContextManager securityContext = new SecurityContextManager(webUrl);
            if (securityContext.CurrentUser.IsUserSecurityManager())
            {
                try
                {
                    foreach (GlymaSecurityAssociation securityAssociation in securityAssociations)
                    {
                        GlymaSecurityAssociationContext securityAssociationContext = new GlymaSecurityAssociationContext(securityContext, securityAssociation.GlymaSecurityGroup, securityAssociation.SecurableObject);
                        if (securityAssociation.Value)
                        {
                            securityAssociationContext.SetSecurityAssociation(securityAssociation.BreakInheritance);
                        }
                        else
                        {
                            securityAssociationContext.RemoveSecurityAssociation();
                        }
                    }
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.ErrorMessage = ex.Message;
                }
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage = "Access Denied. User does not have permissions to access this web service.";
            }
            return result;
        }


        /// <summary>
        /// Gets the current security associations for a list of groups against a particular securable object
        /// </summary>
        /// <param name="webUrl">The URL for the SP site</param>
        /// <param name="groups">A list of groups to get the security assocations for</param>
        /// <param name="securableObject">An object that contains the Parent and Object ID's 
        ///                                 SecurableParentUid: The ID of the securable parent (Guid.Empty for projects), 
        ///                                 SecurableObjectUid: The ID of the securable object (root map UID or project UID if securing a project)</param>
        /// <returns>A dictionary of security association, Key: the group, Value: True if the group has an assocation. (wrapped in a Response Object to indicate if any errors occured)</returns>
        public GetSecurityAssociationsResponse GetSecurityAssociations(string webUrl, IEnumerable<GlymaSecurityGroup> groups, GlymaSecurableObject securableObject)
        {
            GetSecurityAssociationsResponse result = new GetSecurityAssociationsResponse() { HasError = false };

            SecurityContextManager securityContext = new SecurityContextManager(webUrl);
            result = securityContext.GetSecurityAssociations(groups, securableObject);
            return result;
        }

        /// <summary>
        /// Gets the highest permission (role) name that the current user has if they have access to the object
        /// </summary>
        /// <param name="webUrl">The URL for the SP site</param>
        /// <param name="securableObject">An object that contains the Parent and Object ID's 
        ///                                 SecurableParentUid: The ID of the securable parent (Guid.Empty for projects), 
        ///                                 SecurableObjectUid: The ID of the securable object (root map UID or project UID if securing a project)</param>
        /// <returns>The Glyma permission level name if the user has access or null if the user doens't have access</returns>
        public GetPermissionNameResponse GetPermissionNameForObject(string webUrl, GlymaSecurableObject securableObject)
        {
            GetPermissionNameResponse result = new GetPermissionNameResponse() { HasError = false };

            GetPermissionLevelResponse response = GetPermissionLevelForObject(webUrl, securableObject);
            if (!response.HasError)
            {
                result.Result = GlymaPermissionLevelHelper.GetPermissionLevelName(response.Result);
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage = response.ErrorMessage;
            }

            return result;
        }

        /// <summary>
        /// Gets the highest permission (role) name that the current user has if they have access to the object
        /// </summary>
        /// <param name="webUrl">The URL for the SP site</param>
        /// <param name="securableObject">An object that contains the Parent and Object ID's 
        ///                                 SecurableParentUid: The ID of the securable parent (Guid.Empty for projects), 
        ///                                 SecurableObjectUid: The ID of the securable object (root map UID or project UID if securing a project)</param>
        /// <returns>The Glyma permission level name if the user has access or null if the user doens't have access</returns>
        public GetPermissionLevelResponse GetPermissionLevelForObject(string webUrl, GlymaSecurableObject securableObject)
        {
            GetPermissionLevelResponse result = new GetPermissionLevelResponse() { HasError = false };

            SecurityContextManager securityContext = new SecurityContextManager(webUrl);
            if (securityContext.CurrentUser.IsUserMapReader())
            {
                GlymaPermissionLevel highestPermissionLevel = GlymaPermissionLevel.None;
                try
                {
                    GetCurrentUserAccessToObjectResponse response = securityContext.CurrentUser.GetCurrentUserAccessToObject(securableObject, false);
                    if (!response.HasError)
                    {
                        if (response.HasAccess)
                        {
                            //if the user has access to the object (project or root map) get there highest permission level
                            highestPermissionLevel = response.HighestPermissionLevel;
                        }
                        else
                        {
                            //The user has no access
                            result.Result = GlymaPermissionLevel.None;
                        }
                    }
                    else
                    {
                        result.HasError = true;
                        result.ErrorMessage = response.ErrorMessage;
                    }
                }
                catch (Exception ex)
                {
                    result.HasError = true;
                    result.ErrorMessage = ex.Message;
                }
                if (!result.HasError)
                {
                    result.Result = highestPermissionLevel;
                }
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage = "Access Denied. User does not have permissions to access this web service method.";
            }
            return result;
        }

        /// <summary>
        /// This method is called by a Glyma Project Manager when they create a new project, it will associate any Glyma Project Manager group the user belongs to
        /// with the newly created project.
        /// </summary>
        /// <param name="webUrl">The URL for the SP site</param>
        /// <param name="securableObject">Describes the project that was just added</param>
        /// <returns>A response object indicating if the operation completed without error.</returns>
        public ResponseObject SetProjectManagerGroupAssociations(string webUrl, GlymaSecurableObject securableObject)
        {
            ResponseObject result = new ResponseObject() { HasError = false };

            try
            {
                SecurityContextManager context = new SecurityContextManager(webUrl);
                result = context.CurrentUser.SetProjectManagerGroupAssociations(securableObject);
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public ResponseObject BreakRootMapInheritance(string webUrl, GlymaSecurableObject securableObject)
        {
            ResponseObject result = new ResponseObject() { HasError = false };

            try
            {
                SecurityContextManager context = new SecurityContextManager(webUrl);
                result = context.BreakRootMapInheritance(securableObject);
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        public GetSecurityAssociationsResponse RestoreRootMapInheritance(string webUrl, GlymaSecurableObject securableObject)
        {
            GetSecurityAssociationsResponse result = new GetSecurityAssociationsResponse() { HasError = false };

            try
            {
                SecurityContextManager context = new SecurityContextManager(webUrl);
                result = context.RestoreRootMapInheritance(securableObject);
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        #endregion
    }
}
