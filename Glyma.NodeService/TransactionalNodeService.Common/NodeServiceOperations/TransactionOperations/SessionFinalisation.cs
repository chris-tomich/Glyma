using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Security.Principal;

namespace TransactionalNodeService.Common.TransactionOperations
{
    public class SessionFinalisation
    {
        private const string InsertSessionTransactionSqlQuery = @"INSERT INTO [Transactions] ([User],
                                                                                                [TransactionTimestamp],
                                                                                                [SessionUid],
                                                                                                [OperationId]) VALUES (@User,
                                                                                                @TransactionTimestamp,
                                                                                                @SessionUid,
                                                                                                '2')";

        private const string SelectSessionTransactionSqlQuery = @"SELECT COUNT(*) FROM [Transactions] WHERE [SessionUid] = @SessionUid AND [OperationId] = '2'";

        protected string _user;

        public SessionFinalisation(IGlymaSession glymaSession, Guid sessionId)
        {
            GlymaSession = glymaSession;
            SessionId = sessionId;
        }

        private IGlymaSession GlymaSession
        {
            get;
            set;
        }

        private Guid SessionId
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

        public bool ExecuteIsCompletedQuery()
        {
            using (IDbConnectionAbstraction sessionDbConnection = GlymaSession.ConnectionFactory.CreateSessionDbConnection())
            {
                SqlCommand command = CreateQuerySessionCommand(sessionDbConnection.Connection);

                sessionDbConnection.Open();
                int numberOfCompleteRows = (int)command.ExecuteScalar();
                sessionDbConnection.Close();

                if (numberOfCompleteRows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void ExecuteCompleteSession()
        {
            using (IDbConnectionAbstraction sessionDbConnection = GlymaSession.ConnectionFactory.CreateSessionDbConnection())
            {
                SqlCommand command = CreateCompleteSessionCommand(sessionDbConnection.Connection);

                sessionDbConnection.Open();
                command.ExecuteNonQuery();
                sessionDbConnection.Close();
            }
        }

        protected SqlCommand CreateQuerySessionCommand(SqlConnection connection)
        {
            SqlCommand createQuerySessionCommand = new SqlCommand();
            createQuerySessionCommand.CommandText = SelectSessionTransactionSqlQuery;
            createQuerySessionCommand.Connection = connection;

            createQuerySessionCommand.Parameters.AddWithValue("@SessionUid", SessionId);

            return createQuerySessionCommand;
        }

        protected SqlCommand CreateCompleteSessionCommand(SqlConnection connection)
        {
            SqlCommand createCompleteSessionCommand = new SqlCommand();
            createCompleteSessionCommand.CommandText = InsertSessionTransactionSqlQuery;
            createCompleteSessionCommand.Connection = connection;

            createCompleteSessionCommand.Parameters.AddWithValue("@User", User);
            createCompleteSessionCommand.Parameters.AddWithValue("@TransactionTimestamp", DateTime.Now.ToUniversalTime());
            createCompleteSessionCommand.Parameters.AddWithValue("@SessionUid", SessionId);

            return createCompleteSessionCommand;
        }
    }
}