using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Glyma.Security;

namespace Glyma.SharePoint.Security
{
    public static class GlymaPermissionLevelHelper
    {
        public static string GetPermissionLevelName(GlymaPermissionLevel permissionLevel) 
        {
            string result = null;
            switch (permissionLevel)
            {
                case GlymaPermissionLevel.GlymaSecurityManager:
                    result = Constants.GLYMA_SECURITY_MAGAGERS_PERMISSION_LEVEL;
                    break;
                case GlymaPermissionLevel.GlymaProjectManager:
                    result = Constants.GLYMA_PROJECT_MANAGERS_PERMISSION_LEVEL;
                    break;
                case GlymaPermissionLevel.GlymaMapManager:
                    result = Constants.GLYMA_MAP_MANAGERS_PERMISSION_LEVEL;
                    break;
                case GlymaPermissionLevel.GlymaMapAuthor:
                    result = Constants.GLYMA_MAP_AUTHORS_PERMISSION_LEVEL;
                    break;
                case GlymaPermissionLevel.GlymaMapAuthorOld:
                    result = Constants.GLYMA_MAP_AUTHORS_OLD_PERMISSION_LEVEL;
                    break;
                case GlymaPermissionLevel.GlymaMapReader:
                    result = Constants.GLYMA_MAP_READERS_PERMISSION_LEVEL;
                    break;
                case GlymaPermissionLevel.GlymaMapReaderOld:
                    result = Constants.GLYMA_MAP_READERS_OLD_PERMISSION_LEVEL;
                    break;
                case GlymaPermissionLevel.None:
                    result = Constants.GLYMA_NO_PERMISSION_LEVEL;
                    break;
            }
            return result;
        }

        public static GlymaPermissionLevel GetPermissionLevelByName(string permissionLevel)
        {
            GlymaPermissionLevel result = GlymaPermissionLevel.None;

            if (permissionLevel == Constants.GLYMA_SECURITY_MAGAGERS_PERMISSION_LEVEL)
            {
                result = GlymaPermissionLevel.GlymaSecurityManager;
            }
            else if (permissionLevel == Constants.GLYMA_PROJECT_MANAGERS_PERMISSION_LEVEL)
            {
                result = GlymaPermissionLevel.GlymaProjectManager;
            }
            else if (permissionLevel == Constants.GLYMA_MAP_MANAGERS_PERMISSION_LEVEL)
            {
                result = GlymaPermissionLevel.GlymaMapManager;
            }
            else if (permissionLevel == Constants.GLYMA_MAP_AUTHORS_PERMISSION_LEVEL)
            {
                result = GlymaPermissionLevel.GlymaMapAuthor;
            }
            else if (permissionLevel == Constants.GLYMA_MAP_READERS_PERMISSION_LEVEL)
            {
                result = GlymaPermissionLevel.GlymaMapReader;
            }
            else if (permissionLevel == Constants.GLYMA_MAP_READERS_OLD_PERMISSION_LEVEL)
            {
                result = GlymaPermissionLevel.GlymaMapReaderOld;
            }
            else if (permissionLevel == Constants.GLYMA_MAP_AUTHORS_OLD_PERMISSION_LEVEL)
            {
                result = GlymaPermissionLevel.GlymaMapAuthorOld;
            }
            return result;
        }

        public static bool IsPermissionGreaterOrEqual(GlymaPermissionLevel permissionLevel, GlymaPermissionLevel maxPermissionLevel)
        {
            bool result = false;
            switch (maxPermissionLevel)
            {
                case GlymaPermissionLevel.GlymaSecurityManager:
                    if (permissionLevel == GlymaPermissionLevel.GlymaSecurityManager)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    break;
                case GlymaPermissionLevel.GlymaProjectManager:
                    if (permissionLevel == GlymaPermissionLevel.GlymaSecurityManager ||
                        permissionLevel == GlymaPermissionLevel.GlymaProjectManager)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    break;
                case GlymaPermissionLevel.GlymaMapManager:
                    if (permissionLevel == GlymaPermissionLevel.GlymaSecurityManager ||
                        permissionLevel == GlymaPermissionLevel.GlymaProjectManager ||
                        permissionLevel == GlymaPermissionLevel.GlymaMapManager)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    break;
                case GlymaPermissionLevel.GlymaMapAuthor:
                    if (permissionLevel == GlymaPermissionLevel.GlymaSecurityManager ||
                        permissionLevel == GlymaPermissionLevel.GlymaProjectManager ||
                        permissionLevel == GlymaPermissionLevel.GlymaMapManager ||
                        permissionLevel == GlymaPermissionLevel.GlymaMapAuthor ||
                        permissionLevel == GlymaPermissionLevel.GlymaMapAuthorOld)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    break;
                case GlymaPermissionLevel.GlymaMapReader:
                    if (permissionLevel == GlymaPermissionLevel.GlymaSecurityManager ||
                        permissionLevel == GlymaPermissionLevel.GlymaProjectManager ||
                        permissionLevel == GlymaPermissionLevel.GlymaMapManager ||
                        permissionLevel == GlymaPermissionLevel.GlymaMapAuthor ||
                        permissionLevel == GlymaPermissionLevel.GlymaMapAuthorOld ||
                        permissionLevel == GlymaPermissionLevel.GlymaMapReader ||
                        permissionLevel == GlymaPermissionLevel.GlymaMapReaderOld)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }

        /// <summary>
        /// Helper method used for checking if one Glyma permission is greater than or less than another.
        /// </summary>
        /// <param name="permissionName">The name of the permission to check</param>
        /// <param name="maxPermission">The current maximum permision to check against</param>
        /// <returns></returns>
        public static bool IsPermissionGreaterOrEqual(string permissionName, string maxPermission)
        {
            GlymaPermissionLevel permssionLevel = GetPermissionLevelByName(permissionName);
            GlymaPermissionLevel maxPermissionLevel = GetPermissionLevelByName(maxPermission);
            return IsPermissionGreaterOrEqual(permssionLevel, maxPermissionLevel);
        }
    }
}
