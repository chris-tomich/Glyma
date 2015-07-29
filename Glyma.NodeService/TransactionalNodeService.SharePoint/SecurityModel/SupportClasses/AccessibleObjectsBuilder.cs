using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransactionalNodeService.SharePoint.SecurityModel.SupportClasses
{
    /// <summary>
    /// Helper class that builds out an IEnumerable of the SecurableObjects that can be accessed based off information provided by a SecurableObjectGenealogyTracker.
    /// </summary>
    internal class AccessibleObjectsBuilder
    {
        private HashSet<Guid> _accessibleObjects;

        public AccessibleObjectsBuilder()
        {
            _accessibleObjects = new HashSet<Guid>();
        }

        public void AddRange(IEnumerable<Guid> accessibleObjectUids)
        {
            foreach (Guid accessibleObjectUid in accessibleObjectUids)
            {
                if (!_accessibleObjects.Contains(accessibleObjectUid))
                {
                    _accessibleObjects.Add(accessibleObjectUid);
                }
            }
        }

        public HashSet<Guid> GetAccessibleObjects()
        {
            return _accessibleObjects;
        }
    }
}
