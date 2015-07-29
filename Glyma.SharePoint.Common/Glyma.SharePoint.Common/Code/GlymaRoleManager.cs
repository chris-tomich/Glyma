using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glyma.SharePoint.Common
{
    public class GlymaRoleManager
    {
        private const string GlymaProjectManagerRoleName = "Glyma Project Manager";
        private const string GlymaMapManagerRoleName = "Glyma Map Manager";
        private const string GlymaAuthorRoleName = "Glyma Map Author";
        private const string GlymaReaderRoleName = "Glyma Map Reader";

        private SPWeb Site
        {
            get;
            set;
        }

        public GlymaRoleManager(SPWeb site)
        {
            Site = site;
        }

        public void AddRoleDefinitions()
        {
            bool hasProjectManagerRole = false;
            bool hasMapManagerRole = false;
            bool hasAuthorRole = false;
            bool hasReaderRole = false;

            foreach (SPRoleDefinition definition in Site.RoleDefinitions)
            {
                if (definition.Name == GlymaProjectManagerRoleName)
                {
                    hasProjectManagerRole = true;
                }

                if (definition.Name == GlymaMapManagerRoleName)
                {
                    hasMapManagerRole = true;
                }

                if (definition.Name == GlymaAuthorRoleName)
                {
                    hasAuthorRole = true;
                }

                if (definition.Name == GlymaReaderRoleName)
                {
                    hasReaderRole = true;
                }

                if (hasProjectManagerRole && hasMapManagerRole && hasReaderRole && hasAuthorRole)
                {
                    break;
                }
            }

            if (!hasProjectManagerRole)
            {
                SPRoleDefinition glymaProjectManagerRole = new SPRoleDefinition();
                glymaProjectManagerRole.Name = GlymaProjectManagerRoleName;
                glymaProjectManagerRole.Description = "Users have access to create and modify projects in a Glyma map database.";
                glymaProjectManagerRole.BasePermissions = SPBasePermissions.EmptyMask;

                Site.RoleDefinitions.Add(glymaProjectManagerRole);
            }

            if (!hasMapManagerRole)
            {
                SPRoleDefinition glymaMapManagerRole = new SPRoleDefinition();
                glymaMapManagerRole.Name = GlymaMapManagerRoleName;
                glymaMapManagerRole.Description = "Users have access to create and modify root maps in a Glyma Project.";
                glymaMapManagerRole.BasePermissions = SPBasePermissions.EmptyMask;

                Site.RoleDefinitions.Add(glymaMapManagerRole);
            }

            if (!hasAuthorRole)
            {
                SPRoleDefinition glymaAuthorRole = new SPRoleDefinition();
                glymaAuthorRole.Name = GlymaAuthorRoleName;
                glymaAuthorRole.Description = "Users have access to create and modify Glyma maps.";
                glymaAuthorRole.BasePermissions = SPBasePermissions.EmptyMask;

                Site.RoleDefinitions.Add(glymaAuthorRole);
            }

            if (!hasReaderRole)
            {
                SPRoleDefinition glymaReaderRole = new SPRoleDefinition();
                glymaReaderRole.Name = GlymaReaderRoleName;
                glymaReaderRole.Description = "Users have access to read Glyma maps.";
                glymaReaderRole.BasePermissions = SPBasePermissions.EmptyMask;

                Site.RoleDefinitions.Add(glymaReaderRole);
            }

            SPPropertyBag propertyBag = Site.Properties;

            propertyBag["Glyma.RolesDefined"] = "true";

            propertyBag.Update();

            if (!hasReaderRole || !hasAuthorRole)
            {
                Site.Update();
            }
        }
    }
}
