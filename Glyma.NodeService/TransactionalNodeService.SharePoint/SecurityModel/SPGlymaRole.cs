using Glyma.Security.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionalNodeService.SharePoint.SecurityModel
{
    public class SPGlymaRole : IRole
    {
        private HashSet<IRight> _rights = null;

        public SPGlymaRole(int securableContextId, int roleId, string displayName, params IRight[] rights)
        {
            SecurableContextId = securableContextId;
            RoleId = roleId;
            DisplayName = displayName;

            foreach (IRight right in rights)
            {
                Rights.Add(right);
            }
        }

        private HashSet<IRight> Rights
        {
            get
            {
                if (_rights == null)
                {
                    _rights = new HashSet<IRight>();
                }

                return _rights;
            }
        }

        public int? SecurableContextId
        {
            get;
            private set;
        }

        public int RoleId
        {
            get;
            private set;
        }

        public string DisplayName
        {
            get;
            private set;
        }

        public bool HasRights(params IRight[] requiredRights)
        {
            foreach (IRight requiredRight in requiredRights)
            {
                if (!Rights.Contains(requiredRight))
                {
                    return false;
                }
            }

            return true;
        }
    }
}