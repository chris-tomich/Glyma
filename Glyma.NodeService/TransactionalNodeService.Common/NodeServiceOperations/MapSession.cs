using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using TransactionalNodeService.Common.TransactionOperations;
using System.Security.Principal;
using System.Data;
using Microsoft.IdentityModel.Claims;
using System.Threading;

namespace TransactionalNodeService.Common
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

        private const string IdentityClaimType = @"http://schemas.microsoft.com/sharepoint/2009/08/claims/userid";

        protected string _user;
        protected HashSet<MapTransactionWrapper> _newTransactions = null;

        protected MapSession()
            : base()
        {
        }

        public MapSession(IGlymaSession glymaSession)
            : this()
        {
            Id = Guid.NewGuid();
            IsNew = true;
            GlymaSession = glymaSession;
        }

        public MapSession(IGlymaSession glymaSession, Guid sessionId)
            : this(glymaSession)
        {
            Id = sessionId;
            IsNew = false;
        }

        protected IGlymaSession GlymaSession
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

                    string identityUser = GetIdentity();
                    if (!string.IsNullOrEmpty(identityUser))
                    {
                        _user = identityUser;
                    }
                }

                return _user;
            }
        }

        private string GetIdentity()
        {
            string identityName = String.Empty;
            IClaimsIdentity claimsIdentity = Thread.CurrentPrincipal.Identity as IClaimsIdentity;
            if (claimsIdentity != null)
            {
                // claims identity processing
                foreach (Claim claim in claimsIdentity.Claims)
                {
                    if (string.Equals(IdentityClaimType, claim.ClaimType, StringComparison.OrdinalIgnoreCase))
                    {
                        identityName = claim.Value;
                        break;
                    }
                }
            }
            else
            {
                // non claims identity processing
                identityName = Thread.CurrentPrincipal.Identity.Name;
            }

            return identityName;
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
                using (IDbConnectionAbstraction sessionDbConnection = GlymaSession.ConnectionFactory.CreateSessionDbConnection())
                {
                    transaction.PersistSessionObject(sessionDbConnection);
                }
            }
        }

        protected void CreateNewSession()
        {
            SqlParameter user = new SqlParameter("@User", User);
            SqlParameter transactionTimestamp = new SqlParameter("@TransactionTimestamp", DateTime.Now.ToUniversalTime());
            SqlParameter sessionId = new SqlParameter("@SessionUid", Id);
            SqlParameter operationId = new SqlParameter("@OperationId", TransactionType.BeginSession);

            using (IDbConnectionAbstraction sessionDbConnection = GlymaSession.ConnectionFactory.CreateSessionDbConnection())
            {
                SqlCommand insertSessionTransaction = new SqlCommand(InsertSessionTransactionSqlQuery, sessionDbConnection.Connection);
                insertSessionTransaction.Parameters.Add(user);
                insertSessionTransaction.Parameters.Add(transactionTimestamp);
                insertSessionTransaction.Parameters.Add(sessionId);
                insertSessionTransaction.Parameters.Add(operationId);

                sessionDbConnection.Open();
                insertSessionTransaction.ExecuteNonQuery();
                sessionDbConnection.Close();
            }
        }

        public void LoadTransactions()
        {
            SqlParameter sessionIdParameter = new SqlParameter("@SessionId", Id);

            using (IDbConnectionAbstraction sessionDbConnection = GlymaSession.ConnectionFactory.CreateSessionDbConnection())
            {
                SqlCommand selectSessionTransactions = new SqlCommand(SelectSessionTransactionsSqlQuery, sessionDbConnection.Connection);
                selectSessionTransactions.Parameters.Add(sessionIdParameter);

                sessionDbConnection.Open();

                SqlDataReader sessions = selectSessionTransactions.ExecuteReader();

                while (sessions.Read())
                {
                    TransactionType transactionType = (TransactionType)sessions["OperationId"];

                    if (transactionType != TransactionType.BeginSession && transactionType != TransactionType.CompleteSession)
                    {
                        MapTransactionWrapper mapTransaction = new MapTransactionWrapper(GlymaSession);

                        mapTransaction.LoadSessionObject(sessions);

                        Enqueue(mapTransaction);
                    }
                }

                sessionDbConnection.Close();
            }
        }
    }
}