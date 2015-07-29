using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransactionalNodeService.SharePoint.SecurityModel.SupportClasses
{
    /// <summary>
    /// Helper class that orders GroupAssociation objects by their SharePoint Group ID.
    /// </summary>
    internal class GroupAssociationsOrderedByGroupSPId
    {
        private Dictionary<int, HashSet<GroupAssociation>> _groupAssociationsBySPId = null;

        public GroupAssociationsOrderedByGroupSPId()
        {
            _groupAssociationsBySPId = new Dictionary<int, HashSet<GroupAssociation>>();
        }

        public void Add(GroupAssociation groupAssociation)
        {
            HashSet<GroupAssociation> groupAssociations;

            if (_groupAssociationsBySPId.ContainsKey(groupAssociation.Group.GroupSPID))
            {
                groupAssociations = _groupAssociationsBySPId[groupAssociation.Group.GroupSPID];
            }
            else
            {
                groupAssociations = new HashSet<GroupAssociation>();
                _groupAssociationsBySPId[groupAssociation.Group.GroupSPID] = groupAssociations;
            }

            if (!groupAssociations.Contains(groupAssociation))
            {
                groupAssociations.Add(groupAssociation);
            }
        }

        public void AddRange(IEnumerable<GroupAssociation> groupAssociations)
        {
            foreach (GroupAssociation groupAssociation in groupAssociations)
            {
                Add(groupAssociation);
            }
        }

        public bool GetBySPId(int spId, out IEnumerable<GroupAssociation> groupAssociationsBySPId)
        {
            if (_groupAssociationsBySPId.ContainsKey(spId))
            {
                groupAssociationsBySPId = _groupAssociationsBySPId[spId];
                return true;
            }
            else
            {
                groupAssociationsBySPId = new GroupAssociation[0];
                return false;
            }
        }
    }
}
