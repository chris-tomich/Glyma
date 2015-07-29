using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using TransactionalNodeService.TransactionOperations;
using System.Security.Principal;
using System.Data;

namespace TransactionalNodeService
{
    public class MapSession : Queue<MapTransactionWrapper>, IPersistableSessionContainer
    {
        private const string InsertSessionTransactionSqlQuery = @"INSERT INTO [Transactions] ([User],
                                                                                                [TransactionTimestamp],
                                                                                                [SessionUid],
                                                                                                [OperationId]) VALUES (@User,
                                                                                                @TransactionTimestamp,
                                                                                                @SessionUid,
                                                                                                @OperationId)";

        private const string SelectSessionTransactionsSqlQuery = @"SELECT [TransactionId],
                                                                            [TransactionTimestamp],
                                                                            [User],
                                                                            [SessionUid],
                                                                            [OperationId],
                                                                            [ResponseParameterUid],
                                                                            [DomainParameterUid],
                                                                            [NodeParameterUid],
                                                                            [DescriptorParameterUid],
                                                                            [RelationshipParameterUid],
                                                                            [MetadataParameterUid],
                                                                            [NodeTypeUid],
                                                                            [DescriptorTypeUid],
                                                                            [RelationshipTypeUid],
                                                                            [MetadataTypeUid],
                                                                            [MetadataName],
                                                                            [MetadataValue],
                                                                            [RootMapParameterUid] FROM [Transactions] WHERE [SessionUid] = @SessionId ORDER BY [TransactionTimestamp]";

        protected string _user;
        protected HashSet<MapTransactionWrapper> _newTransactions = null;

        protected MapSession()
            : base()
        {
        }

        public MapSession(IMapObjects mapObjects)
            : this()
        {
            Id = Guid.NewGuid();
            IsNew = true;
            MapObjects = mapObjects;
        }

        public MapSession(IMapObjects mapObjects, Guid sessionId)
            : this(mapObjects)
        {
            Id = sessionId;
            IsNew = false;
        }

        protected IMapObjects MapObjects
        {
            get;
            set;
        }

        protected HashSet<MapTransactionWrapper> NewTransactions
        {
            get
            {
                if (_newTransactions == null)
                {
                    _newTransactions = new HashSet<MapTransactionWrapper>();
                }

                return _newTransactions;
            }
        }

        public Guid Id
        {
            get;
            protected set;
        }

        public string User
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

        protected bool IsNew
        {
            get;
            set;
        }

        public new void Enqueue(MapTransactionWrapper transaction)
        {
            NewTransactions.Add(transaction);

            base.Enqueue(transaction);
        }

        public new MapTransactionWrapper Dequeue()
        {
            MapTransactionWrapper transaction = base.Dequeue();

            NewTransactions.Remove(transaction);

            return transaction;
        }

        public void PersistSessionObject()
        {
            if (IsNew)
            {
                CreateNewSession();
                IsNew = false;
            }

            foreach (MapTransactionWrapper transaction in NewTransactions)
            {
                transaction.PersistSessionObject(MapObjects.SessionDbConnection);
            }
        }

        protected void CreateNewSession()
        {
            SqlParameter user = new SqlParameter("@User", User);
            SqlParameter transactionTimestamp = new SqlParameter("@TransactionTimestamp", DateTime.Now);
            SqlParameter sessionId = new SqlParameter("@SessionUid", Id);
            SqlParameter operationId = new SqlParameter("@OperationId", TransactionType.BeginSession);

            SqlCommand insertSessionTransaction = new SqlCommand(InsertSessionTransactionSqlQuery, MapObjects.SessionDbConnection.Connection);
            insertSessionTransaction.Parameters.Add(user);
            insertSessionTransaction.Parameters.Add(transactionTimestamp);
            insertSessionTransaction.Parameters.Add(sessionId);
            insertSessionTransaction.Parameters.Add(operationId);

            MapObjects.SessionDbConnection.Open();
            insertSessionTransaction.ExecuteNonQuery();
            MapObjects.SessionDbConnection.Close();
        }

        public void LoadTransactions()
        {
            SqlParameter sessionIdParameter = new SqlParameter("@SessionId", Id);

            SqlCommand selectSessionTransactions = new SqlCommand(SelectSessionTransactionsSqlQuery, MapObjects.SessionDbConnection.Connection);
            selectSessionTransactions.Parameters.Add(sessionIdParameter);

            MapObjects.SessionDbConnection.Open();

            SqlDataReader sessions = selectSessionTransactions.ExecuteReader();

            while (sessions.Read())
            {
                TransactionType transactionType = (TransactionType)sessions["OperationId"];

                if (transactionType != TransactionType.BeginSession && transactionType != TransactionType.CompleteSession)
                {
                    MapTransactionWrapper mapTransaction = new MapTransactionWrapper(MapObjects);

                    mapTransaction.LoadSessionObject(sessions);

                    Enqueue(mapTransaction);
                }
            }

            MapObjects.SessionDbConnection.Close();
        }
    }
}