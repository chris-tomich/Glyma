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
    internal class GlymaSecurityAssociationContext
    {
        internal GlymaSecurityGroup Group
        {
            get;
            set;
        }

        internal GlymaSecurableObject SecurableObject
        {
            get;
            set;
        }

        internal SecurityContextManager Context
        {
            get;
            set;
        }

        internal GlymaSecurityAssociationContext(SecurityContextManager context, GlymaSecurityGroup group, GlymaSecurableObject securableObject)
        {
            Group = group;
            SecurableObject = securableObject;
            Context = context;
        }

        internal GlymaSecurityAssociationContext(SecurityContextManager context, GlymaSecurableObject securableObject)
        {
            Context = context;
            SecurableObject = securableObject;
        }

        /// <summary>
        /// Checks if the association exists
        /// </summary>
        /// <param name="checkProjectsChildren">If this is true when checking the access to a Project if there are any root maps under that project the user
        /// has access to it returns true for the project as well (only true for when working out the filtered lists)</param>
        /// <returns>True if the GroupAssociation exists on this particular object for the group</returns>
        internal bool HasAssociation(bool checkProjectsChildren = false)
        {
            bool result = false;
            if (Group != null)
            {
                using (IGlymaSession glymaSession = new WebAppSPGlymaSession(Context.WebUrl))
                {
                    using (IDbConnectionAbstraction connectionAbstraction = glymaSession.ConnectionFactory.CreateSecurityDbConnection())
                    {
                        using (SecurityServiceDataContext dataContext = new SecurityServiceDataContext(connectionAbstraction.Connection))
                        {
                            SPSecurity.RunWithElevatedPrivileges(delegate()
                            {
                                Group glGroup = Context.GetGroup(Group);
                                if (glGroup != null)
                                {
                                    IEnumerable<GroupAssociation> groupAssociations = null;
                                    if (SecurableObject.SecurableParentUid == Guid.Empty)
                                    {
                                        //searching for a Project association
                                        groupAssociations = from ga in dataContext.GroupAssociations
                                                            where ga.SecurableObjectUid == SecurableObject.SecurableObjectUid && ga.SecurableParentUid.HasValue == false
                                                                && ga.SecurableContextId == Group.SecurableContextId && ga.GroupId == glGroup.GroupId
                                                            select ga;

                                        if (!groupAssociations.Any() && checkProjectsChildren)
                                        {
                                            //check if the user has access to anything under the project which inherintly gives them access to that project
                                            //cross check the project id (sent as the object id) against the parent uid and we don't care about the object uid as any root map
                                            //gives them access to that particular project
                                            groupAssociations = from ga in dataContext.GroupAssociations
                                                                where ga.SecurableParentUid == SecurableObject.SecurableObjectUid
                                                                    && ga.SecurableContextId == Group.SecurableContextId && ga.GroupId == glGroup.GroupId
                                                                select ga;
                                        }
                                    }
                                    else
                                    {
                                        //searching for a RootMap association
                                        GlymaSecurableObjectContext objectContext = new GlymaSecurableObjectContext(Context, Group.SecurableContextId, SecurableObject);
                                        bool isInherited = objectContext.GetIsInherited();

                                        if (!isInherited)
                                        {
                                            //if not inherited it will have it's own group associations
                                            groupAssociations = from ga in dataContext.GroupAssociations
                                                                where ga.SecurableObjectUid == SecurableObject.SecurableObjectUid && ga.SecurableParentUid == SecurableObject.SecurableParentUid
                                                                    && ga.SecurableContextId == Group.SecurableContextId && ga.GroupId == glGroup.GroupId
                                                                select ga;
                                        }
                                        else
                                        {

                                            //if it is inherited look fro the parents group associations
                                            groupAssociations = from ga in dataContext.GroupAssociations
                                                                where ga.SecurableObjectUid == SecurableObject.SecurableParentUid && ga.SecurableParentUid.HasValue == false
                                                                && ga.SecurableContextId == Group.SecurableContextId && ga.GroupId == glGroup.GroupId
                                                                select ga;
                                        }
                                    }

                                    if (groupAssociations.Any())
                                    {
                                        result = true;
                                    }
                                }
                            });
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Adds a security association for a SharePoint group to the security DB
        /// </summary>
        /// <param name="breakInheritance">Whether it should have inheritance broken or not</param>
        /// <returns>A response object indicating if completed without error</returns>
        internal ResponseObject SetSecurityAssociation(bool breakInheritance)
        {
            ResponseObject result = new ResponseObject() { HasError = false };

            try
            {
                if (Group != null)
                {
                    Group group = Context.GetGroup(Group);

                    //if (group == null)
                    //{
                    //    //Create the Group since it doesn't exist
                    //    group = this.CreateGroup();

                    //}
                    bool response = this.HasAssociation();
                    if (!response)
                    {
                        SecurableObject so = Context.GetSecurableObject(Group.SecurableContextId, SecurableObject.SecurableObjectUid);
                        GlymaSecurableObjectContext securableObjectContext = new GlymaSecurableObjectContext(Context, Group.SecurableContextId, SecurableObject);
                        if (so == null)
                        {
                            so = securableObjectContext.CreateSecurableObject(breakInheritance);
                        }

                        //Create the group association since it doesn't exist
                        this.CreateGroupAssociation(group.GroupId);
                    }
                }
                else
                {
                    result.HasError = true;
                    result.ErrorMessage = "The Glyma security group was not known.";
                }
            }
            catch (Exception ex)
            {
                result.HasError = true;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        /// <summary>
        /// Removes the group association if it exists
        /// </summary>
        /// <returns>A response object indicating if completed without error</returns>
        internal ResponseObject RemoveSecurityAssociation()
        {
            ResponseObject result = new ResponseObject() { HasError = false };
            if (Group != null)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    try
                    {
                        using (IGlymaSession glymaSession = new WebAppSPGlymaSession(Context.WebUrl))
                        {
                            using (IDbConnectionAbstraction connectionAbstraction = glymaSession.ConnectionFactory.CreateSecurityDbConnection())
                            {
                                using (SecurityServiceDataContext dataContext = new SecurityServiceDataContext(connectionAbstraction.Connection))
                                {
                                    Group sgroup = Context.GetGroup(Group);

                                    if (sgroup != null)
                                    {
                                        IEnumerable<GroupAssociation> groupAssociations = null;
                                        if (SecurableObject.SecurableParentUid != Guid.Empty)
                                        {
                                            //removing a root map group association
                                            groupAssociations = from ga in dataContext.GroupAssociations
                                                                where
                                                                    ga.SecurableObjectUid == SecurableObject.SecurableObjectUid &&
                                                                    ga.SecurableParentUid == SecurableObject.SecurableParentUid
                                                                    && ga.SecurableContextId == Group.SecurableContextId && ga.GroupId == sgroup.GroupId
                                                                select ga;
                                        }
                                        else
                                        {
                                            //removing a project group association
                                            groupAssociations = from ga in dataContext.GroupAssociations
                                                                where
                                                                    ga.SecurableObjectUid == SecurableObject.SecurableObjectUid &&
                                                                    ga.SecurableParentUid.HasValue == false
                                                                    && ga.SecurableContextId == Group.SecurableContextId && ga.GroupId == sgroup.GroupId
                                                                select ga;
                                        }
                                        if (groupAssociations.Any())
                                        {
                                            dataContext.GroupAssociations.DeleteAllOnSubmit(groupAssociations.ToList());
                                            dataContext.SubmitChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.HasError = true;
                        result.ErrorMessage = ex.Message;
                    }
                });
            }
            else
            {
                result.HasError = true;
                result.ErrorMessage = "The Glyma security group was not known.";
            }
            return result;
        }

        /// <summary>
        /// Creates the GroupAssociation
        /// </summary>
        /// <param name="groupId">The ID of the Group object</param>
        internal void CreateGroupAssociation(int groupId)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (IGlymaSession glymaSession = new WebAppSPGlymaSession(Context.WebUrl))
                {
                    using (IDbConnectionAbstraction connectionAbstraction = glymaSession.ConnectionFactory.CreateSecurityDbConnection())
                    {
                        using (SecurityServiceDataContext dataContext = new SecurityServiceDataContext(connectionAbstraction.Connection))
                        {
                            SecurableContext securableContext = Context.GetSecurableContext();
                            int securableContextId = securableContext.SecurableContextId;

                            GroupAssociation groupAssociation = new GroupAssociation();
                            groupAssociation.GroupId = groupId;
                            groupAssociation.SecurableContextId = securableContextId;
                            if (SecurableObject.SecurableParentUid != Guid.Empty)
                            {
                                //group association is for a root map (not a project)
                                groupAssociation.SecurableParentUid = SecurableObject.SecurableParentUid;
                            }
                            groupAssociation.SecurableObjectUid = SecurableObject.SecurableObjectUid;
                            dataContext.GroupAssociations.InsertOnSubmit(groupAssociation);
                            dataContext.SubmitChanges();
                        }
                    }
                }
            });
        }
    }
}
