using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Glyma.Security;

namespace Glyma.SharePoint.Security
{
    public static class ConversionUtility
    {
        public static IList<GlymaSecurityGroup> ConvertDictToList(IDictionary<GlymaPermissionLevel, IList<GlymaSecurityGroup>> groupsDict)
        {
            List<GlymaSecurityGroup> combinedList = new List<GlymaSecurityGroup>();
            foreach (GlymaPermissionLevel level in groupsDict.Keys)
            {
                IList<GlymaSecurityGroup> levelPermissions = groupsDict[level];
                combinedList.AddRange(levelPermissions);
            }
            return combinedList;
        }
    }
}
