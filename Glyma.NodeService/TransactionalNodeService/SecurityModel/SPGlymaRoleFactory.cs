﻿using Glyma.Security.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionalNodeService.SecurityModel
{
    public class SPGlymaRoleFactory
    {
        static readonly object padlock = new object();
        static readonly Dictionary<int, SPGlymaRoleFactory> spGlymaRoleFactories = new Dictionary<int, SPGlymaRoleFactory>();

        private IRole _glymaSecurityManagerRole;
        private IRole _glymaProjectManagerRole;
        private IRole _glymaMapManagerRole;
        private IRole _glymaMapAuthorRole;
        private IRole _glymaMapReaderRole;

        public static SPGlymaRoleFactory GetInstance(int securableContextId)
        {
            lock (padlock)
            {
                if (!spGlymaRoleFactories.ContainsKey(securableContextId))
                {
                    spGlymaRoleFactories[securableContextId] = new SPGlymaRoleFactory(securableContextId);
                }

                return spGlymaRoleFactories[securableContextId];
            }
        }

        private SPGlymaRoleFactory(int securableContextId)
        {
            SecurableContextId = securableContextId;
        }

        public int SecurableContextId
        {
            get;
            private set;
        }

        public IRole GlymaSecurityManagerRole
        {
            get
            {
                if (_glymaSecurityManagerRole == null)
                {
                    _glymaSecurityManagerRole = new SPGlymaRole(SecurableContextId, 1, "", SPGlymaRightFactory.Instance.SecurableContextManageRight,
                                                                                           SPGlymaRightFactory.Instance.ProjectManageRight,
                                                                                           SPGlymaRightFactory.Instance.ProjectCreateRight,
                                                                                           SPGlymaRightFactory.Instance.ProjectReadRight,
                                                                                           SPGlymaRightFactory.Instance.ProjectUpdateRight,
                                                                                           SPGlymaRightFactory.Instance.ProjectDeleteRight,
                                                                                           SPGlymaRightFactory.Instance.RootMapManageRight,
                                                                                           SPGlymaRightFactory.Instance.RootMapCreateRight,
                                                                                           SPGlymaRightFactory.Instance.RootMapReadRight,
                                                                                           SPGlymaRightFactory.Instance.RootMapUpdateRight,
                                                                                           SPGlymaRightFactory.Instance.RootMapDeleteRight,
                                                                                           SPGlymaRightFactory.Instance.MapCreateRight,
                                                                                           SPGlymaRightFactory.Instance.MapReadRight,
                                                                                           SPGlymaRightFactory.Instance.MapUpdateRight,
                                                                                           SPGlymaRightFactory.Instance.MapDeleteRight);
                }

                return _glymaSecurityManagerRole;
            }
        }

        public IRole GlymaProjectManagerRole
        {
            get
            {
                if (_glymaProjectManagerRole == null)
                {
                    _glymaProjectManagerRole = new SPGlymaRole(SecurableContextId, 1, "", SPGlymaRightFactory.Instance.ProjectManageRight,
                                                                                          SPGlymaRightFactory.Instance.ProjectCreateRight,
                                                                                          SPGlymaRightFactory.Instance.ProjectReadRight,
                                                                                          SPGlymaRightFactory.Instance.ProjectUpdateRight,
                                                                                          SPGlymaRightFactory.Instance.ProjectDeleteRight,
                                                                                          SPGlymaRightFactory.Instance.RootMapManageRight,
                                                                                          SPGlymaRightFactory.Instance.RootMapCreateRight,
                                                                                          SPGlymaRightFactory.Instance.RootMapReadRight,
                                                                                          SPGlymaRightFactory.Instance.RootMapUpdateRight,
                                                                                          SPGlymaRightFactory.Instance.RootMapDeleteRight,
                                                                                          SPGlymaRightFactory.Instance.MapCreateRight,
                                                                                          SPGlymaRightFactory.Instance.MapReadRight,
                                                                                          SPGlymaRightFactory.Instance.MapUpdateRight,
                                                                                          SPGlymaRightFactory.Instance.MapDeleteRight);
                }

                return _glymaProjectManagerRole;
            }
        }

        public IRole GlymaMapManagerRole
        {
            get
            {
                if (_glymaMapManagerRole == null)
                {
                    _glymaMapManagerRole = new SPGlymaRole(SecurableContextId, 1, "", SPGlymaRightFactory.Instance.RootMapManageRight,
                                                                                      SPGlymaRightFactory.Instance.RootMapCreateRight,
                                                                                      SPGlymaRightFactory.Instance.RootMapReadRight,
                                                                                      SPGlymaRightFactory.Instance.RootMapUpdateRight,
                                                                                      SPGlymaRightFactory.Instance.RootMapDeleteRight,
                                                                                      SPGlymaRightFactory.Instance.MapCreateRight,
                                                                                      SPGlymaRightFactory.Instance.MapReadRight,
                                                                                      SPGlymaRightFactory.Instance.MapUpdateRight,
                                                                                      SPGlymaRightFactory.Instance.MapDeleteRight);
                }

                return _glymaMapManagerRole;
            }
        }

        public IRole GlymaMapAuthorRole
        {
            get
            {
                if (_glymaMapAuthorRole == null)
                {
                    _glymaMapAuthorRole = new SPGlymaRole(SecurableContextId, 1, "", SPGlymaRightFactory.Instance.RootMapCreateRight,
                                                                                     SPGlymaRightFactory.Instance.RootMapUpdateRight,
                                                                                     SPGlymaRightFactory.Instance.MapCreateRight,
                                                                                     SPGlymaRightFactory.Instance.MapReadRight,
                                                                                     SPGlymaRightFactory.Instance.MapUpdateRight,
                                                                                     SPGlymaRightFactory.Instance.MapDeleteRight);
        }

                return _glymaMapAuthorRole;
            }
        }

        public IRole GlymaMapReaderRole
        {
            get
            {
                if (_glymaMapReaderRole == null)
                {
                    _glymaMapReaderRole = new SPGlymaRole(SecurableContextId, 1, "", SPGlymaRightFactory.Instance.MapReadRight);
                }

                return _glymaMapReaderRole;
            }
        }
    }
}