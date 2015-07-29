using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Glyma.Security;

namespace Glyma.SharePoint.Security
{
    public abstract class GlymaPermission : IGlymaPermission
    {
        protected SecurityContextManager SecurityContextManager
        {
            get;
            set;
        }

        public virtual string Name
        {
            get
            {
                return Constants.GLYMA_NO_PERMISSION_LEVEL;
            }
        }

        public virtual string OldName
        {
            get
            {
                return null;
            }
        }

        public virtual GlymaPermissionLevel PermissionLevel
        {
            get
            {
                return GlymaPermissionLevel.None;
            }
        }

        public virtual bool CurrentUserHasPermission()
        {
            bool result = false;
            string currentUserLoginName = SecurityContextManager.CurrentUser.CurrentSPUser.LoginName;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(SecurityContextManager.WebUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPPermissionInfo info = web.GetUserEffectivePermissionInfo(currentUserLoginName);
                        SPRoleDefinition roleDefinition = null;
                        try
                        {
                            roleDefinition = web.RoleDefinitions[Name];
                        }
                        catch (Exception)
                        {
                            //if unable to find the role definition it will throw an exception
                        }
                        SPRoleDefinition secondaryRoleDefinition = null;
                        if (OldName != null)
                        {
                            try
                            {
                                secondaryRoleDefinition = web.RoleDefinitions[OldName];
                            }
                            catch (Exception)
                            {
                                //if unable to find the role definition it will throw an exception
                            }
                        }

                        if (roleDefinition != null && info != null)
                        {
                            foreach (SPRoleAssignment roleAssignment in info.RoleAssignments)
                            {
                                if (roleAssignment.RoleDefinitionBindings.Contains(roleDefinition))
                                {
                                    result = true;
                                    break;
                                }
                                if (secondaryRoleDefinition != null)
                                {
                                    if (roleAssignment.RoleDefinitionBindings.Contains(secondaryRoleDefinition))
                                    {
                                        result = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            });

            return result;
        }
    }
}
