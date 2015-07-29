using Glyma.Security.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransactionalNodeService.SharePoint.SecurityModel
{
    public class SPGlymaRightFactory
    {
        static SPGlymaRightFactory instance = null;
        static readonly object padlock = new object();

        private SPGlymaRight _securableContextManagePermission = null;
        private SPGlymaRight _projectManageRight = null;
        private SPGlymaRight _projectCreateRight = null;
        private SPGlymaRight _projectReadRight = null;
        private SPGlymaRight _projectUpdateRight = null;
        private SPGlymaRight _projectDeleteRight = null;
        private SPGlymaRight _rootMapManageRight = null;
        private SPGlymaRight _rootMapCreateRight = null;
        private SPGlymaRight _rootMapReadRight = null;
        private SPGlymaRight _rootMapUpdateRight = null;
        private SPGlymaRight _rootMapDeleteRight = null;
        private SPGlymaRight _mapCreateRight = null;
        private SPGlymaRight _mapReadRight = null;
        private SPGlymaRight _mapUpdateRight = null;
        private SPGlymaRight _mapDeleteRight = null;
        private SPGlymaRight _transactionRight = null;
        private SPGlymaRight _benignAccessRight = null;

        public static SPGlymaRightFactory Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SPGlymaRightFactory();
                    }
                    return instance;
                }
            }
        }

        private SPGlymaRightFactory()
        {
        }

        public IRight SecurableContextManageRight
        {
            get
            {
                if (_securableContextManagePermission == null)
                {
                    _securableContextManagePermission = new SPGlymaRight(1, "SecurableContextManageRight");
                }

                return _securableContextManagePermission;
            }
        }

        public IRight ProjectManageRight
        {
            get
            {
                if (_projectManageRight == null)
                {
                    _projectManageRight = new SPGlymaRight(2, "ProjectManageRight");
                }

                return _projectManageRight;
            }
        }

        public IRight ProjectCreateRight
        {
            get
            {
                if (_projectCreateRight == null)
                {
                    _projectCreateRight = new SPGlymaRight(3, "ProjectCreateRight");
                }

                return _projectCreateRight;
            }
        }

        public IRight ProjectReadRight
        {
            get
            {
                if (_projectReadRight == null)
                {
                    _projectReadRight = new SPGlymaRight(4, "ProjectReadRight");
                }

                return _projectReadRight;
            }
        }

        public IRight ProjectUpdateRight
        {
            get
            {
                if (_projectUpdateRight == null)
                {
                    _projectUpdateRight = new SPGlymaRight(5, "ProjectUpdateRight");
                }

                return _projectUpdateRight;
            }
        }

        public IRight ProjectDeleteRight
        {
            get
            {
                if (_projectDeleteRight == null)
                {
                    _projectDeleteRight = new SPGlymaRight(6, "ProjectDeleteRight");
                }

                return _projectDeleteRight;
            }
        }

        public IRight RootMapManageRight
        {
            get
            {
                if (_rootMapManageRight == null)
                {
                    _rootMapManageRight = new SPGlymaRight(7, "RootMapManageRight");
                }

                return _rootMapManageRight;
            }
        }

        public IRight RootMapCreateRight
        {
            get
            {
                if (_rootMapCreateRight == null)
                {
                    _rootMapCreateRight = new SPGlymaRight(8, "RootMapCreateRight");
                }

                return _rootMapCreateRight;
            }
        }

        public IRight RootMapReadRight
        {
            get
            {
                if (_rootMapReadRight == null)
                {
                    _rootMapReadRight = new SPGlymaRight(9, "RootMapReadRight");
                }

                return _rootMapReadRight;
            }
        }

        public IRight RootMapUpdateRight
        {
            get
            {
                if (_rootMapUpdateRight == null)
                {
                    _rootMapUpdateRight = new SPGlymaRight(10, "RootMapUpdateRight");
                }

                return _rootMapUpdateRight;
            }
        }

        public IRight RootMapDeleteRight
        {
            get
            {
                if (_rootMapDeleteRight == null)
                {
                    _rootMapDeleteRight = new SPGlymaRight(11, "RootMapDeleteRight");
                }

                return _rootMapDeleteRight;
            }
        }

        public IRight MapCreateRight
        {
            get
            {
                if (_mapCreateRight == null)
                {
                    _mapCreateRight = new SPGlymaRight(12, "MapCreateRight");
                }

                return _mapCreateRight;
            }
        }

        public IRight MapReadRight
        {
            get
            {
                if (_mapReadRight == null)
                {
                    _mapReadRight = new SPGlymaRight(13, "MapReadRight");
                }

                return _mapReadRight;
            }
        }

        public IRight MapUpdateRight
        {
            get
            {
                if (_mapUpdateRight == null)
                {
                    _mapUpdateRight = new SPGlymaRight(14, "MapUpdateRight");
                }

                return _mapUpdateRight;
            }
        }

        public IRight MapDeleteRight
        {
            get
            {
                if (_mapDeleteRight == null)
                {
                    _mapDeleteRight = new SPGlymaRight(15, "MapDeleteRight");
                }

                return _mapDeleteRight;
            }
        }

        public IRight TransactionRight
        {
            get
            {
                if (_transactionRight == null)
                {
                    _transactionRight = new SPGlymaRight(16, "TransactionRight");
                }

                return _transactionRight;
            }
        }

        public IRight BenignAccessRight
        {
            get
            {
                if (_benignAccessRight == null)
                {
                    _benignAccessRight = new SPGlymaRight(17, "BenignAccessRight");
                }

                return _benignAccessRight;
            }
        }
    }
}