using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace Glyma.Powershell.SharePoint
{
    [Cmdlet(VerbsCommon.Add, "SPGLPermissionLevels")]
    public class Add_SPGLPermissionLevels : PSCmdlet
    {
        private const string GlymaProjectManagerRoleName = "Glyma Project Manager";
        private const string GlymaMapManagerRoleName = "Glyma Map Manager";
        private const string GlymaAuthorRoleName = "Glyma Map Author";
        private const string GlymaReaderRoleName = "Glyma Map Reader";

        [Parameter(Position = 0, Mandatory = true)]
        public object Identity
        {
            get;
            set;
        }

        private SPWeb GetRootWebFromIdentity(object identity)
        {
            object identityAsObject = Identity;

            if (Identity is PSObject)
            {
                PSObject identityAsPSObject = Identity as PSObject;

                identityAsObject = identityAsPSObject.BaseObject;
            }

            if (identityAsObject is SPSite)
            {
                SPSite site = identityAsObject as SPSite;

                return site.RootWeb;
            }
            else if (identityAsObject is SPWeb)
            {
                SPWeb web = identityAsObject as SPWeb;

                return web.Site.RootWeb;
            }
            else if (identityAsObject is string)
            {
                string siteUrl = identityAsObject as string;

                SPSite site = new SPSite(siteUrl);
                SPWeb web = site.OpenWeb();

                return web.Site.RootWeb;
            }
            else if (identityAsObject is Guid)
            {
                Guid siteId = (Guid)identityAsObject;

                SPSite site = new SPSite(siteId);
                SPWeb web = site.RootWeb;

                return web;
            }
            else
            {
                return null;
            }
        }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            SPWeb webToConfigure = GetRootWebFromIdentity(Identity);

            if (webToConfigure == null)
            {
                WriteWarning("No valid 'Identity' specified. Identity must be a SPSite, SPWeb, site/web URL, or site colleciton ID.");
            }

            bool hasProjectManagerRole = false;
            bool hasMapManagerRole = false;
            bool hasAuthorRole = false;
            bool hasReaderRole = false;

            foreach (SPRoleDefinition definition in webToConfigure.RoleDefinitions)
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

                webToConfigure.RoleDefinitions.Add(glymaProjectManagerRole);
            }

            if (!hasMapManagerRole)
            {
                SPRoleDefinition glymaMapManagerRole = new SPRoleDefinition();
                glymaMapManagerRole.Name = GlymaMapManagerRoleName;
                glymaMapManagerRole.Description = "Users have access to create and modify root maps in a Glyma Project.";
                glymaMapManagerRole.BasePermissions = SPBasePermissions.EmptyMask;

                webToConfigure.RoleDefinitions.Add(glymaMapManagerRole);
            }

            if (!hasAuthorRole)
            {
                SPRoleDefinition glymaAuthorRole = new SPRoleDefinition();
                glymaAuthorRole.Name = GlymaAuthorRoleName;
                glymaAuthorRole.Description = "Users have access to create and modify Glyma maps.";
                glymaAuthorRole.BasePermissions = SPBasePermissions.EmptyMask;

                webToConfigure.RoleDefinitions.Add(glymaAuthorRole);
            }

            if (!hasReaderRole)
            {
                SPRoleDefinition glymaReaderRole = new SPRoleDefinition();
                glymaReaderRole.Name = GlymaReaderRoleName;
                glymaReaderRole.Description = "Users have access to read Glyma maps.";
                glymaReaderRole.BasePermissions = SPBasePermissions.EmptyMask;

                webToConfigure.RoleDefinitions.Add(glymaReaderRole);
            }

            SPPropertyBag propertyBag = webToConfigure.Properties;

            propertyBag["Glyma.RolesDefined"] = "true";

            propertyBag.Update();

            if (!hasReaderRole || !hasAuthorRole)
            {
                webToConfigure.Update();
            }
        }
    }
}
