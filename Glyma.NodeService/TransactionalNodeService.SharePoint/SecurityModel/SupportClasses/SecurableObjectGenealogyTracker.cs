using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransactionalNodeService.SharePoint.SecurityModel.SupportClasses
{
    /// <summary>
    /// Helper class that tracks the permission inheritance and broken inheritance for SecurableObjects.
    /// </summary>
    internal class SecurableObjectGenealogyTracker
    {
        /// <summary>
        /// The first GUID is the ID of the securable object whose permissions will be used.
        /// If the object has broken inheritance and uses it's own permissions,
        /// then this ID will be that of the object itself and the List will only contain one element.
        /// </summary>
        private Dictionary<Guid, HashSet<Guid>> _securableObjectPermissionSource;

        /// <summary>
        /// Tracks all the securable objects that have been added.
        /// </summary>
        private HashSet<Guid> _addedSecurableObjectUids;

        public SecurableObjectGenealogyTracker()
        {
            HasInheritingObjects = false;
            HasIndependentObjects = false;
            _securableObjectPermissionSource = new Dictionary<Guid, HashSet<Guid>>();
            _addedSecurableObjectUids = new HashSet<Guid>();
        }

        /// <summary>
        /// Indicates whether the tracker contains objects that inherit their inheritance.
        /// This will be true if there are any domain objects listed.
        /// </summary>
        public bool HasInheritingObjects
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates whether the tracker contains objects that have broken inheritance.
        /// </summary>
        public bool HasIndependentObjects
        {
            get;
            private set;
        }

        public void Add(Guid? securableParentUid, Guid securableObjectUid)
        {
            if (securableObjectUid == Guid.Empty)
            {
                return;
            }

            Guid permissionSourceObject;

            if (securableParentUid == null || securableParentUid == securableObjectUid)
            {
                /// This means it's a domain object.
                permissionSourceObject = securableObjectUid;
                HasIndependentObjects = true;
            }
            else
            {
                /// Assume inheritance as we were given a securable parent GUID.
                permissionSourceObject = securableParentUid.Value;
                HasInheritingObjects = true;
            }

            HashSet<Guid> inheritingObjects;

            if (_securableObjectPermissionSource.ContainsKey(permissionSourceObject))
            {
                inheritingObjects = _securableObjectPermissionSource[permissionSourceObject];
            }
            else
            {
                inheritingObjects = new HashSet<Guid>();
                _securableObjectPermissionSource[permissionSourceObject] = inheritingObjects;
            }

            if (!inheritingObjects.Contains(securableObjectUid))
            {
                inheritingObjects.Add(securableObjectUid);
                _addedSecurableObjectUids.Add(securableObjectUid);
            }
        }

        public void Add(Guid? securableParentUid, SecurableObject securableObject)
        {
            Guid permissionSourceObject;

            if (securableParentUid == null || securableObject.BreaksInheritance)
            {
                /// This means it's a domain object or has broken inheritance.
                permissionSourceObject = securableObject.SecurableObjectUid;
            }
            else
            {
                /// Assume inheritance as we were only given a GUID.
                permissionSourceObject = securableParentUid.Value;
            }

            /// We can now call the other overloaded version. As we have filled in the securableParentUid field,
            /// this will be what is used to store our SecurableObjectUid against.
            Add(permissionSourceObject, securableObject.SecurableObjectUid);
        }

        public void AddIfNotPreExisting(Guid? securableParentUid, Guid securableObjectUid)
        {
            if (!_addedSecurableObjectUids.Contains(securableObjectUid))
            {
                Add(securableObjectUid, securableObjectUid);
            }
        }

        public void AddRange(Guid? securableParentUid, IEnumerable<Guid> securableObjectUids)
        {
            foreach (Guid securableObjectUid in securableObjectUids)
            {
                Add(securableParentUid, securableObjectUid);
            }
        }

        public void AddRange(Guid? securableParentUid, IEnumerable<SecurableObject> securableObjects)
        {
            foreach (SecurableObject securableObject in securableObjects)
            {
                Add(securableParentUid, securableObject);
            }
        }

        public void AddRangeIfNotPreExisting(Guid? securableParentUid, IEnumerable<Guid> securableObjectUids)
        {
            foreach (Guid securableObjectUid in securableObjectUids)
            {
                if (!_addedSecurableObjectUids.Contains(securableObjectUid))
                {
                    Add(securableParentUid, securableObjectUid);
                }
            }
        }

        public IEnumerable<Guid> GetPermissionSources()
        {
            return _securableObjectPermissionSource.Keys;
        }

        public IEnumerable<Guid> GetSecurableObjectsByPermissionSources(IEnumerable<GroupAssociation> permissionSources)
        {
            HashSet<Guid> securableObjectUids = new HashSet<Guid>();

            foreach (GroupAssociation groupAssociation in permissionSources)
            {
                if (_securableObjectPermissionSource.ContainsKey(groupAssociation.SecurableObjectUid))
                {
                    foreach (Guid securableObjectUid in _securableObjectPermissionSource[groupAssociation.SecurableObjectUid])
                    {
                        if (!securableObjectUids.Contains(securableObjectUid))
                        {
                            securableObjectUids.Add(securableObjectUid);
                        }
                    }
                }
                else if (groupAssociation.SecurableParentUid != null && _securableObjectPermissionSource.ContainsKey(groupAssociation.SecurableParentUid.Value))
                {
                    foreach (Guid securableObjectUid in _securableObjectPermissionSource[groupAssociation.SecurableParentUid.Value])
                    {
                        if (!securableObjectUids.Contains(securableObjectUid))
                        {
                            securableObjectUids.Add(securableObjectUid);
                        }
                    }
                }
            }

            return securableObjectUids;
        }
    }
}
