using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace TransactionalNodeService.Common.TransactionOperations
{
    public class DeleteRootMap
    {
//        private const string DeleteRootMapMetadataSql = @"DELETE FROM [Metadata] WHERE [Metadata].[DomainUid] = @DomainUid AND [Metadata].[RootMapUid] = @RootMapUid";

//        private const string DeleteRootMapDescriptorsSql = @"DELETE [Descriptors] FROM [Descriptors]
//                                                            INNER JOIN (SELECT * FROM [Relationships] WHERE [Relationships].[DomainUid] = @DomainUid AND [Relationships].[RootMapUid] = @RootMapUid) AS FilteredRelationships ON [Descriptors].[RelationshipUid] = [FilteredRelationships].[RelationshipUid]";

//        private const string DeleteRootMapNodesSql = @"DELETE FROM [Nodes] WHERE [Nodes].[DomainUid] = @DomainUid AND [Nodes].[RootMapUid] = @RootMapUid";

//        private const string DeleteRootMapRelationshipsSql = @"DELETE FROM [Relationships] WHERE [Relationships].[DomainUid] = @DomainUid AND [Relationships].[RootMapUid] = @RootMapUid";

        private const string DeleteRootMapSql = @"DELETE FROM [Metadata] WHERE [Metadata].[DomainUid] = @DomainUid AND [Metadata].[RootMapUid] = @RootMapUid;
                                                  DELETE [Descriptors] FROM [Descriptors]
                                                    INNER JOIN (SELECT * FROM [Relationships] WHERE [Relationships].[DomainUid] = @DomainUid AND [Relationships].[RootMapUid] = @RootMapUid) AS FilteredRelationships ON [Descriptors].[RelationshipUid] = [FilteredRelationships].[RelationshipUid];
                                                  DELETE FROM [Relationships] WHERE [Relationships].[DomainUid] = @DomainUid AND [Relationships].[RootMapUid] = @RootMapUid;
                                                  DELETE FROM [Nodes] WHERE [Nodes].[DomainUid] = @DomainUid AND [Nodes].[RootMapUid] = @RootMapUid;";

        protected string _user = null;

        public DeleteRootMap(IDbConnectionAbstraction mapDbConnection, IDbConnectionAbstraction parametersDbConnection, IDbConnectionAbstraction sessionDbConnection)
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

        public int ExecuteDeletion(Guid domainId, Guid rootMapId)
        {
            //SqlCommand deleteMetadataByRootMap = new SqlCommand(DeleteRootMapMetadataSql, MapDbConnection.Connection);
            //deleteMetadataByRootMap.Parameters.Add(new SqlParameter("@DomainUid", domainId));
            //deleteMetadataByRootMap.Parameters.Add(new SqlParameter("@RootMapUid", rootMapId));

            //deleteMetadataByRootMap.ExecuteNonQuery();

            //SqlCommand deleteDescriptorsByRootMap = new SqlCommand(DeleteRootMapDescriptorsSql, MapDbConnection.Connection);
            //deleteDescriptorsByRootMap.Parameters.Add(new SqlParameter("@DomainUid", domainId));
            //deleteDescriptorsByRootMap.Parameters.Add(new SqlParameter("@RootMapUid", rootMapId));

            //deleteDescriptorsByRootMap.ExecuteNonQuery();

            //SqlCommand deleteRelationshipsByRootMap = new SqlCommand(DeleteRootMapRelationshipsSql, MapDbConnection.Connection);
            //deleteRelationshipsByRootMap.Parameters.Add(new SqlParameter("@DomainUid", domainId));
            //deleteRelationshipsByRootMap.Parameters.Add(new SqlParameter("@RootMapUid", rootMapId));

            //deleteRelationshipsByRootMap.ExecuteNonQuery();

            //SqlCommand deleteNodesByRootMap = new SqlCommand(DeleteRootMapNodesSql, MapDbConnection.Connection);
            //deleteNodesByRootMap.Parameters.Add(new SqlParameter("@DomainUid", domainId));
            //deleteNodesByRootMap.Parameters.Add(new SqlParameter("@RootMapUid", rootMapId));

            //deleteNodesByRootMap.ExecuteNonQuery();

            SqlCommand deleteByRootMap = new SqlCommand(DeleteRootMapSql, MapDbConnection.Connection);
            deleteByRootMap.Parameters.Add(new SqlParameter("@DomainUid", domainId));
            deleteByRootMap.Parameters.Add(new SqlParameter("@RootMapUid", rootMapId));

            int rowsDeleted = deleteByRootMap.ExecuteNonQuery();

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

            MapParameter rootMapParameter = new MapParameter();
            rootMapParameter.SessionId = sessionId;
            rootMapParameter.IsDelayed = false;
            rootMapParameter.ParameterType = MapParameterType.RootMap;
            rootMapParameter.Value = rootMapId;
            rootMapParameter.PersistSessionObject(ParametersDbConnection);

            MapTransaction createRootMapTransaction = new MapTransaction();
            createRootMapTransaction.SessionUid = sessionId;
            createRootMapTransaction.TransactionTimestamp = DateTime.Now.ToUniversalTime();
            createRootMapTransaction.User = User;
            createRootMapTransaction.OperationId = TransactionType.DeleteRootMap;
            createRootMapTransaction.DomainParameterUid = domainParameter.Id;
            createRootMapTransaction.RootMapParameterUid = rootMapParameter.Id;
            createRootMapTransaction.NodeParameterUid = rootMapParameter.Id;
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
