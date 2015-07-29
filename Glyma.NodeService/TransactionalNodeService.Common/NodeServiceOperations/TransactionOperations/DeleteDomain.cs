using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace TransactionalNodeService.Common.TransactionOperations
{
    public class DeleteDomain
    {
//        private const string DeleteDomainMetadataSql = @"DELETE FROM [Metadata] WHERE [Metadata].[DomainUid] = @DomainUid";

//        private const string DeleteDomainDescriptorsSql = @"DELETE [Descriptors] FROM [Descriptors]
//                                                            INNER JOIN (SELECT * FROM [Relationships] WHERE [Relationships].[DomainUid] = @DomainUid) AS FilteredRelationships ON [Descriptors].[RelationshipUid] = [FilteredRelationships].[RelationshipUid]";

//        private const string DeleteDomainNodesSql = @"DELETE FROM [Nodes] WHERE [Nodes].[DomainUid] = @DomainUid";

//        private const string DeleteDomainRelationshipsSql = @"DELETE FROM [Relationships] WHERE [Relationships].[DomainUid] = @DomainUid";

//        private const string DeleteDomainSql = @"DELETE FROM [Domains] WHERE [Domains].[DomainUid] = @DomainUid";

        private const string DeleteDomainSql = @"DELETE FROM [Metadata] WHERE [Metadata].[DomainUid] = @DomainUid;
                                                 DELETE [Descriptors] FROM [Descriptors]
                                                    INNER JOIN (SELECT * FROM [Relationships] WHERE [Relationships].[DomainUid] = @DomainUid) AS FilteredRelationships ON [Descriptors].[RelationshipUid] = [FilteredRelationships].[RelationshipUid];
                                                 DELETE FROM [Relationships] WHERE [Relationships].[DomainUid] = @DomainUid;
                                                 DELETE FROM [Nodes] WHERE [Nodes].[DomainUid] = @DomainUid;
                                                 DELETE FROM [Domains] WHERE [Domains].[DomainUid] = @DomainUid";

        protected string _user = null;

        public DeleteDomain(IDbConnectionAbstraction mapDbConnection, IDbConnectionAbstraction parametersDbConnection, IDbConnectionAbstraction sessionDbConnection)
        {
            MapDbConnection = mapDbConnection;
            ParametersDbConnection = parametersDbConnection;
            SessionDbConnection = sessionDbConnection;
        }

        private IDbConnectionAbstraction MapDbConnection
        {
            get;
            set;
        }

        private IDbConnectionAbstraction ParametersDbConnection
        {
            get;
            set;
        }

        private IDbConnectionAbstraction SessionDbConnection
        {
            get;
            set;
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

        public int ExecuteDeletion(Guid domainId)
        {
            //SqlCommand deleteMetadataByDomain = new SqlCommand(DeleteDomainMetadataSql, MapDbConnection.Connection);
            //deleteMetadataByDomain.Parameters.Add(new SqlParameter("@DomainUid", domainId));

            //deleteMetadataByDomain.ExecuteNonQuery();

            //SqlCommand deleteDescriptorsByDomain = new SqlCommand(DeleteDomainDescriptorsSql, MapDbConnection.Connection);
            //deleteDescriptorsByDomain.Parameters.Add(new SqlParameter("@DomainUid", domainId));

            //deleteDescriptorsByDomain.ExecuteNonQuery();

            //SqlCommand deleteRelationshipsByDomain = new SqlCommand(DeleteDomainRelationshipsSql, MapDbConnection.Connection);
            //deleteRelationshipsByDomain.Parameters.Add(new SqlParameter("@DomainUid", domainId));

            //deleteRelationshipsByDomain.ExecuteNonQuery();

            //SqlCommand deleteNodesByDomain = new SqlCommand(DeleteDomainNodesSql, MapDbConnection.Connection);
            //deleteNodesByDomain.Parameters.Add(new SqlParameter("@DomainUid", domainId));

            //deleteNodesByDomain.ExecuteNonQuery();

            //SqlCommand deleteDomain = new SqlCommand(DeleteDomainSql, MapDbConnection.Connection);
            //deleteDomain.Parameters.Add(new SqlParameter("@DomainUid", domainId));

            //deleteDomain.ExecuteNonQuery();

            SqlCommand deleteByDomain = new SqlCommand(DeleteDomainSql, MapDbConnection.Connection);
            deleteByDomain.Parameters.Add(new SqlParameter("@DomainUid", domainId));

            int rowsDeleted = deleteByDomain.ExecuteNonQuery();

            Guid sessionId = Guid.NewGuid();

            MapTransaction beginTransaction = new MapTransaction();
            beginTransaction.SessionUid = sessionId;
            beginTransaction.TransactionTimestamp = DateTime.Now.ToUniversalTime();
            beginTransaction.User = User;
            beginTransaction.OperationId = TransactionType.BeginSession;
            beginTransaction.PersistSessionObject(SessionDbConnection);

            MapParameter domainParameter = new MapParameter();
            domainParameter.SessionId = sessionId;
            domainParameter.IsDelayed = false;
            domainParameter.ParameterType = MapParameterType.Domain;
            domainParameter.Value = domainId;
            domainParameter.PersistSessionObject(ParametersDbConnection);

            MapTransaction createRootMapTransaction = new MapTransaction();
            createRootMapTransaction.SessionUid = sessionId;
            createRootMapTransaction.TransactionTimestamp = DateTime.Now.ToUniversalTime();
            createRootMapTransaction.User = User;
            createRootMapTransaction.OperationId = TransactionType.DeleteDomain;
            createRootMapTransaction.DomainParameterUid = domainParameter.Id;
            createRootMapTransaction.PersistSessionObject(SessionDbConnection);

            MapTransaction endTransaction = new MapTransaction();
            endTransaction.SessionUid = sessionId;
            endTransaction.TransactionTimestamp = DateTime.Now.ToUniversalTime();
            endTransaction.User = User;
            endTransaction.OperationId = TransactionType.CompleteSession;
            endTransaction.PersistSessionObject(SessionDbConnection);

            if (rowsDeleted > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
