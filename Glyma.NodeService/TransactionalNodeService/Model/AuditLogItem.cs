using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Security.Principal;

namespace TransactionalNodeService.Model
{
    public class AuditLogItem
    {
        private const string InsertAuditLogItem = @"INSERT INTO [AuditLogs] ([OperationName], [CallingUrl], [DomainUid], [NodeUid], [RootMapUid], [MaxDepth], [ObjectIndex], [EdgeConditions], [FilterConditions], [SearchConditions], [PageNumber], [PageSize], [Timestamp], [User]) VALUES (@OperationName, @CallingUrl, @DomainUid, @NodeUid, @RootMapUid, @MaxDepth, @ObjectIndex, @EdgeConditions, @FilterConditions, @SearchConditions, @PageNumber, @PageSize, @Timestamp, @User)";

        private string _user;

        public AuditLogItem(SqlConnection connection)
        {
            Connection = connection;
        }

        public SqlConnection Connection
        {
            get;
            private set;
        }

        private string User
        {
            get
            {
                if (_user == null)
                {
                    _user = "anonymous";

                    WindowsIdentity currentUserIdentity = WindowsIdentity.GetCurrent();

                    if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && !string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
                    {
                        /// This will capture for claims authentication.
                        _user = HttpContext.Current.User.Identity.Name;
                    }
                    else if (currentUserIdentity != null && currentUserIdentity.User != null && !string.IsNullOrEmpty(currentUserIdentity.User.Value))
                    {
                        _user = currentUserIdentity.User.Value + ";" + currentUserIdentity.Name;
                    }
                }

                return _user;
            }
        }

        public string OperationName
        {
            get;
            set;
        }

        public string CallingUrl
        {
            get;
            set;
        }

        public Guid? DomainUid
        {
            get;
            set;
        }

        public Guid? NodeUid
        {
            get;
            set;
        }

        public Guid? RootMapUid
        {
            get;
            set;
        }

        public int? MaxDepth
        {
            get;
            set;
        }

        public int? ObjectIndex
        {
            get;
            set;
        }

        public string EdgeConditions
        {
            get;
            set;
        }

        public string FilterConditions
        {
            get;
            set;
        }

        public string SearchConditions
        {
            get;
            set;
        }

        public int? PageNumber
        {
            get;
            set;
        }

        public int? PageSize
        {
            get;
            set;
        }

        public void Commit()
        {
            try
            {
                if (Connection == null)
                {
                    return;
                }

                using (SqlCommand insertAuditLog = new SqlCommand(InsertAuditLogItem, Connection))
                {
                    #region Add Parameters

                    if (OperationName == null)
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@OperationName", DBNull.Value));
                    }
                    else
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@OperationName", OperationName));
                    }

                    if (CallingUrl == null)
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@CallingUrl", DBNull.Value));
                    }
                    else
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@CallingUrl", CallingUrl));
                    }

                    if (DomainUid == null)
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@DomainUid", DBNull.Value));
                    }
                    else
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@DomainUid", DomainUid));
                    }

                    if (NodeUid == null)
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@NodeUid", DBNull.Value));
                    }
                    else
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@NodeUid", NodeUid));
                    }

                    if (RootMapUid == null)
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@RootMapUid", DBNull.Value));
                    }
                    else
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@RootMapUid", RootMapUid));
                    }

                    if (MaxDepth == null)
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@MaxDepth", DBNull.Value));
                    }
                    else
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@MaxDepth", MaxDepth));
                    }

                    if (ObjectIndex == null)
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@ObjectIndex", DBNull.Value));
                    }
                    else
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@ObjectIndex", ObjectIndex));
                    }

                    if (EdgeConditions == null)
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@EdgeConditions", DBNull.Value));
                    }
                    else
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@EdgeConditions", EdgeConditions));
                    }

                    if (FilterConditions == null)
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@FilterConditions", DBNull.Value));
                    }
                    else
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@FilterConditions", FilterConditions));
                    }

                    if (SearchConditions == null)
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@SearchConditions", DBNull.Value));
                    }
                    else
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@SearchConditions", SearchConditions));
                    }

                    if (PageNumber == null)
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@PageNumber", DBNull.Value));
                    }
                    else
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@PageNumber", PageNumber));
                    }

                    if (PageSize == null)
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@PageSize", DBNull.Value));
                    }
                    else
                    {
                        insertAuditLog.Parameters.Add(new SqlParameter("@PageSize", PageSize));
                    }

                    insertAuditLog.Parameters.Add(new SqlParameter("@Timestamp", DateTime.Now));
                    insertAuditLog.Parameters.Add(new SqlParameter("@User", User));
                    #endregion

                    insertAuditLog.ExecuteNonQuery();
                }
            }
            catch
            {
                /// Don't do anything. This is here because audit logging is a very low importance task and we don't want it potentially killing the more important tasks at hand.
            }
        }
    }
}