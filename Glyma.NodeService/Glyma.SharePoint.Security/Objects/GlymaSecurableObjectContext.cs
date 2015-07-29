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
    internal class GlymaSecurableObjectContext
    {
        private int SecurableContextId
        {
            get;
            set;
        }

        private GlymaSecurableObject SecurableObject
        {
            get;
            set;
        }

        private SecurityContextManager Context
        {
            get;
            set;
        }

        internal GlymaSecurableObjectContext(SecurityContextManager context, int securableContextId, GlymaSecurableObject securableObject)
        {
            SecurableContextId = securableContextId;
            SecurableObject = securableObject;
            Context = context;
        }

        internal bool GetIsInherited()
        {
            bool result = false;
            if (SecurableObject.SecurableParentUid != Guid.Empty)
            {
                SecurableObject securableObject = Context.GetSecurableObject(SecurableContextId, SecurableObject.SecurableObjectUid);
                if (securableObject != null)
                {
                    result = !securableObject.BreaksInheritance;
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }

        internal void SetSecurableObjectInheritance(bool breaksInheritance)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (IGlymaSession glymaSession = new WebAppSPGlymaSession(Context.WebUrl))
                {
                    using (IDbConnectionAbstraction connectionAbstraction = glymaSession.ConnectionFactory.CreateSecurityDbConnection())
                    {
                        using (SecurityServiceDataContext dataContext = new SecurityServiceDataContext(connectionAbstraction.Connection))
                        {
                            SecurableObject securableObject = (from so in dataContext.SecurableObjects
                                                               where so.SecurableObjectUid == SecurableObject.SecurableObjectUid &&
                                                               so.SecurableContextId == SecurableContextId
                                                               select so).First();
                            securableObject.BreaksInheritance = breaksInheritance;
                            dataContext.SubmitChanges();
                        }
                    }
                }
            });
        }

        internal SecurableObject CreateSecurableObject(bool breaksInheritance)
        {
            SecurableObject createdSecurableObject = null;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (IGlymaSession glymaSession = new WebAppSPGlymaSession(Context.WebUrl))
                {
                    using (IDbConnectionAbstraction connectionAbstraction = glymaSession.ConnectionFactory.CreateSecurityDbConnection())
                    {
                        using (SecurityServiceDataContext dataContext = new SecurityServiceDataContext(connectionAbstraction.Connection))
                        {
                            SecurableObject securableObject = new SecurableObject();
                            securableObject.SecurableObjectUid = SecurableObject.SecurableObjectUid;
                            securableObject.BreaksInheritance = breaksInheritance;
                            securableObject.SecurableContextId = SecurableContextId;
                            dataContext.SecurableObjects.InsertOnSubmit(securableObject);
                            dataContext.SubmitChanges();

                            // Return the group that was created
                            securableObject = (from so in dataContext.SecurableObjects
                                               where so.SecurableObjectUid == SecurableObject.SecurableObjectUid &&
                                               so.SecurableContextId == SecurableContextId
                                               select so).First();
                            createdSecurableObject = securableObject;
                        }
                    }
                }
            });

            return createdSecurableObject;
        }
    }
}
