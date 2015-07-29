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
    internal class GlymaSecurityGroupCollection
    {
        private IDictionary<GlymaPermissionLevel, IList<GlymaSecurityGroup>> _groups;

        private SecurityContextManager Context
        {
            get;
            set;
        }

        public GlymaSecurityGroupCollection(SecurityContextManager context, IDictionary<GlymaPermissionLevel, IList<GlymaSecurityGroup>> groups) 
        {
            _groups = groups;
            Context = context;
        }

        public IList<GlymaSecurityGroup> GetUsersGroups(SPWeb web, SPUser currentUser)
        {
            IList<GlymaSecurityGroup> pmGroups = _groups[GlymaPermissionLevel.GlymaProjectManager];
            IList<GlymaSecurityGroup> mmGroups = _groups[GlymaPermissionLevel.GlymaMapManager];
            IList<GlymaSecurityGroup> maGroups = _groups[GlymaPermissionLevel.GlymaMapAuthor];
            IList<GlymaSecurityGroup> mrGroups = _groups[GlymaPermissionLevel.GlymaMapReader];

            IList<GlymaSecurityGroup> usersGlymaGroups = new List<GlymaSecurityGroup>();
            foreach (GlymaSecurityGroup sGroup in pmGroups)
            {
                Group glGroup = Context.GetGroup(sGroup);
                if (glGroup != null)
                {
                    SPGroup spGroup = web.Groups.GetByID(glGroup.GroupSPID);
                    if (spGroup.ContainsCurrentUser)
                    {
                        usersGlymaGroups.Add(sGroup);
                    }
                }
            }

            foreach (GlymaSecurityGroup sGroup in mmGroups)
            {
                Group glGroup = Context.GetGroup(sGroup);
                if (glGroup != null)
                {
                    SPGroup spGroup = web.Groups.GetByID(glGroup.GroupSPID);
                    if (spGroup.ContainsCurrentUser)
                    {
                        usersGlymaGroups.Add(sGroup);
                    }
                }
            }

            foreach (GlymaSecurityGroup sGroup in maGroups)
            {
                Group glGroup = Context.GetGroup(sGroup);
                if (glGroup != null)
                {
                    SPGroup spGroup = web.Groups.GetByID(glGroup.GroupSPID);
                    if (spGroup.ContainsCurrentUser)
                    {
                        usersGlymaGroups.Add(sGroup);
                    }
                }
            }

            foreach (GlymaSecurityGroup sGroup in mrGroups)
            {
                Group glGroup = Context.GetGroup(sGroup);
                if (glGroup != null)
                {
                    SPGroup spGroup = web.Groups.GetByID(glGroup.GroupSPID);
                    if (spGroup.ContainsCurrentUser)
                    {
                        usersGlymaGroups.Add(sGroup);
                    }
                }
            }
            return usersGlymaGroups;
        }

        internal GlymaPermissionLevel GetGroupsPermissionLevel(GlymaSecurityGroup glGroup)
        {
            GlymaPermissionLevel result = GlymaPermissionLevel.None;

            IList<GlymaSecurityGroup> pmGroups = _groups[GlymaPermissionLevel.GlymaProjectManager];
            IList<GlymaSecurityGroup> mmGroups = _groups[GlymaPermissionLevel.GlymaMapManager];
            IList<GlymaSecurityGroup> maGroups = _groups[GlymaPermissionLevel.GlymaMapAuthor];
            IList<GlymaSecurityGroup> mrGroups = _groups[GlymaPermissionLevel.GlymaMapReader];

            foreach (GlymaSecurityGroup spGroup in pmGroups)
            {
                if (spGroup.GroupId == glGroup.GroupId)
                {
                    result = GlymaPermissionLevel.GlymaProjectManager;
                    return result; // group has project manager permissions
                }
            }
            foreach (GlymaSecurityGroup spGroup in mmGroups)
            {
                if (spGroup.GroupId == glGroup.GroupId)
                {
                    result = GlymaPermissionLevel.GlymaMapManager;
                    return result; // group has map manager permissions
                }
            }
            foreach (GlymaSecurityGroup spGroup in maGroups)
            {
                if (spGroup.GroupId == glGroup.GroupId)
                {
                    result = GlymaPermissionLevel.GlymaMapAuthor;
                    return result; // group has map author permissions
                }
            }
            foreach (GlymaSecurityGroup spGroup in mrGroups)
            {
                if (spGroup.GroupId == glGroup.GroupId)
                {
                    result = GlymaPermissionLevel.GlymaMapReader;
                    return result; // group has map reader permissions
                }
            }
            return result;
        }

        /// <summary>
        /// Filters the GlymaSPSecurityGroup dictionary so that groups only return for the highest permission level they've been granted.
        /// </summary>
        /// <returns>THe dictionary trimmed so that groups only appear once in their highest permission level</returns>
        internal IDictionary<GlymaPermissionLevel, IList<GlymaSecurityGroup>> FilterGroups()
        {
            IDictionary<GlymaPermissionLevel, IList<GlymaSecurityGroup>> result = new Dictionary<GlymaPermissionLevel, IList<GlymaSecurityGroup>>();

            IList<GlymaSecurityGroup> mapManagersGroups = _groups[GlymaPermissionLevel.GlymaMapManager];
            IList<GlymaSecurityGroup> mapAuthorGroups = _groups[GlymaPermissionLevel.GlymaMapAuthor];
            IList<GlymaSecurityGroup> mapReaderGroups = _groups[GlymaPermissionLevel.GlymaMapReader];
            IList<GlymaSecurityGroup> oldMapReaderGroups = _groups[GlymaPermissionLevel.GlymaMapReaderOld];
            IList<GlymaSecurityGroup> oldMapAuthorGroups = _groups[GlymaPermissionLevel.GlymaMapAuthorOld];

            //The Project Managers group doesn't get filtered
            result.Add(GlymaPermissionLevel.GlymaProjectManager, _groups[GlymaPermissionLevel.GlymaProjectManager]);

            //Filter the Old Map Readers
            IList<GlymaSecurityGroup> filteredOldMapReaderGroups = new List<GlymaSecurityGroup>();
            foreach (GlymaSecurityGroup oldMapReader in oldMapReaderGroups)
            {
                if (!ExistsInHigherPermissionLevel(oldMapReader, GlymaPermissionLevel.GlymaMapReader))
                {
                    filteredOldMapReaderGroups.Add(oldMapReader);
                }
            }
            result.Add(GlymaPermissionLevel.GlymaMapReader, filteredOldMapReaderGroups);

            //Filter the Map Readers
            IList<GlymaSecurityGroup> filteredMapReaderGroups = new List<GlymaSecurityGroup>();
            foreach (GlymaSecurityGroup mapReader in mapReaderGroups)
            {
                if (!ExistsInHigherPermissionLevel(mapReader, GlymaPermissionLevel.GlymaMapReader))
                {
                    filteredMapReaderGroups.Add(mapReader);
                }
            }
            foreach (GlymaSecurityGroup filteredMapReader in filteredMapReaderGroups)
            {
                if (!result[GlymaPermissionLevel.GlymaMapReader].Contains(filteredMapReader))
                {
                    result[GlymaPermissionLevel.GlymaMapReader].Add(filteredMapReader);
                }
            }

            //Filter the Old Map Authors
            IList<GlymaSecurityGroup> filteredOldMapAuthorGroups = new List<GlymaSecurityGroup>();
            foreach (GlymaSecurityGroup oldMapAuthor in oldMapAuthorGroups)
            {
                if (!ExistsInHigherPermissionLevel(oldMapAuthor, GlymaPermissionLevel.GlymaMapAuthor))
                {
                    filteredOldMapAuthorGroups.Add(oldMapAuthor);
                }
            }
            result.Add(GlymaPermissionLevel.GlymaMapAuthor, filteredOldMapAuthorGroups);

            //Filter the Map Authors
            IList<GlymaSecurityGroup> filteredMapAuthorGroups = new List<GlymaSecurityGroup>();
            foreach (GlymaSecurityGroup mapAuthor in mapAuthorGroups)
            {
                if (!ExistsInHigherPermissionLevel(mapAuthor, GlymaPermissionLevel.GlymaMapAuthor))
                {
                    filteredMapAuthorGroups.Add(mapAuthor);
                }
            }
            foreach (GlymaSecurityGroup filteredMapAuthor in filteredMapAuthorGroups)
            {
                if (!result[GlymaPermissionLevel.GlymaMapAuthor].Contains(filteredMapAuthor))
                {
                    result[GlymaPermissionLevel.GlymaMapAuthor].Add(filteredMapAuthor);
                }
            }

            //Filter the Map Managers
            IList<GlymaSecurityGroup> filteredMapManagersGroups = new List<GlymaSecurityGroup>();
            foreach (GlymaSecurityGroup mapManager in mapManagersGroups)
            {
                if (!ExistsInHigherPermissionLevel(mapManager, GlymaPermissionLevel.GlymaMapManager))
                {
                    filteredMapManagersGroups.Add(mapManager);
                }
            }
            result.Add(GlymaPermissionLevel.GlymaMapManager, filteredMapManagersGroups);

            return result;
        }

        /// <summary>
        /// Checks if the the group appears in a higher permission level
        /// </summary>
        /// <param name="group"></param>
        /// <param name="currentPermissionLevel"></param>
        /// <returns>True if the group is in a higher level</returns>
        private bool ExistsInHigherPermissionLevel(GlymaSecurityGroup group, GlymaPermissionLevel currentPermissionLevel)
        {
            bool result = false;

            IList<GlymaSecurityGroup> projectManagerGroups = _groups[GlymaPermissionLevel.GlymaProjectManager];
            IList<GlymaSecurityGroup> mapManagersGroups = _groups[GlymaPermissionLevel.GlymaMapManager];
            IList<GlymaSecurityGroup> mapAuthorGroups = _groups[GlymaPermissionLevel.GlymaMapAuthor];
            IList<GlymaSecurityGroup> oldMapAuthorsGroups = _groups[GlymaPermissionLevel.GlymaMapAuthorOld];

            switch (currentPermissionLevel)
            {
                case GlymaPermissionLevel.GlymaMapManager:
                    foreach (GlymaSecurityGroup securityGroup in projectManagerGroups)
                    {
                        if (group.GroupId == securityGroup.GroupId)
                        {
                            result = true;
                            break; //found match, break foreach loop early
                        }
                    }
                    break;
                case GlymaPermissionLevel.GlymaMapAuthor:
                    foreach (GlymaSecurityGroup securityGroup in mapManagersGroups)
                    {
                        if (group.GroupId == securityGroup.GroupId)
                        {
                            result = true;
                            break; //found match, break foreach loop early
                        }
                    }
                    foreach (GlymaSecurityGroup securityGroup in projectManagerGroups)
                    {
                        if (group.GroupId == securityGroup.GroupId)
                        {
                            result = true;
                            break; //found match, break foreach loop early
                        }
                    }
                    break;
                case GlymaPermissionLevel.GlymaMapReader:
                    foreach (GlymaSecurityGroup securityGroup in mapAuthorGroups)
                    {
                        if (group.GroupId == securityGroup.GroupId)
                        {
                            result = true;
                            break; //found match, break foreach loop early
                        }
                    }
                    foreach (GlymaSecurityGroup securityGroup in oldMapAuthorsGroups)
                    {
                        if (group.GroupId == securityGroup.GroupId)
                        {
                            result = true;
                            break; //found match, break foreach loop early
                        }
                    }
                    foreach (GlymaSecurityGroup securityGroup in mapManagersGroups)
                    {
                        if (group.GroupId == securityGroup.GroupId)
                        {
                            result = true;
                            break; //found match, break foreach loop early
                        }
                    }
                    foreach (GlymaSecurityGroup securityGroup in projectManagerGroups)
                    {
                        if (group.GroupId == securityGroup.GroupId)
                        {
                            result = true;
                            break; //found match, break foreach loop early
                        }
                    }
                    break;
            }
            return result;
        }
    }
}
