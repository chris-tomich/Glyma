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
    internal class GlymaSecurityGroupContext
    {
        internal SecurityContextManager Context
        {
            get;
            set;
        }

        internal int SPGroupId
        {
            get;
            set;
        }

        internal Guid WebId
        {
            get;
            set;
        }

        internal int SecurableContextId
        {
            get;
            set;
        }

        public GlymaSecurityGroupContext(SecurityContextManager context, int securableContextId, int spGroupId, Guid webId)
        {
            Context = context;
            SPGroupId = spGroupId;
            WebId = webId;
            SecurableContextId = securableContextId;
        }

        /// <summary>
        /// Creates the Group in the database
        /// </summary>
        /// <param name="glGroup">The details of the group</param>
        /// <returns>The group object that was created</returns>
        internal Group CreateGroup(string displayName)
        {
            Group createdGroup = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (IGlymaSession glymaSession = new WebAppSPGlymaSession(Context.WebUrl))
                {
                    using (IDbConnectionAbstraction connectionAbstraction = glymaSession.ConnectionFactory.CreateSecurityDbConnection())
                    {
                        using (SecurityServiceDataContext dataContext = new SecurityServiceDataContext(connectionAbstraction.Connection))
                        {
                            // Create the group
                            Group group = new Group();
                            group.SecurableContextId = SecurableContextId;
                            group.GroupSPID = SPGroupId;
                            group.WebSPID = WebId;
                            group.DisplayName = displayName;
                            dataContext.Groups.InsertOnSubmit(group);
                            dataContext.SubmitChanges();

                            // Return the group that was created
                            group = (from g in dataContext.Groups
                                     where g.DisplayName == displayName && g.GroupSPID == SPGroupId
                                     && g.SecurableContextId == SecurableContextId && g.WebSPID == WebId
                                     select g).First();
                            createdGroup = group;
                        }
                    }
                }
            });
            return createdGroup;
        }

        /// <summary>
        /// Gets the group if it exists.
        /// </summary>
        /// <returns>The group object if it exists or null</returns>
        internal Group GetGroup(string displayName)
        {
            Group result = null;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (IGlymaSession glymaSession = new WebAppSPGlymaSession(Context.WebUrl))
                {
                    using (IDbConnectionAbstraction connectionAbstraction = glymaSession.ConnectionFactory.CreateSecurityDbConnection())
                    {
                        using (SecurityServiceDataContext dataContext = new SecurityServiceDataContext(connectionAbstraction.Connection))
                        {
                            var groups = from g in dataContext.Groups
                                         where g.GroupSPID == SPGroupId && g.WebSPID == WebId
                                         && g.SecurableContextId == SecurableContextId
                                         select g;

                            Group group = null; //default value if it doesn't exist
                            if (groups.Any())
                            {
                                group = groups.First();
                                if (group != null)
                                {
                                    //if the display name has changed update it
                                    if (group.DisplayName != displayName)
                                    {
                                        group.DisplayName = displayName;
                                        dataContext.SubmitChanges();
                                    }
                                }
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
