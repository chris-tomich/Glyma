using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Security.Principal;

namespace TransactionalNodeService.TransactionOperations
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

        public SessionFinalisation(IMapObjects mapObjects, Guid sessionId)
        {
            MapObjects = mapObjects;
            SessionId = sessionId;
        }

        private IMapObjects MapObjects
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
            SqlCommand command = CreateQuerySessionCommand(MapObjects.SessionDbConnection.Connection);

            MapObjects.SessionDbConnection.Open();
            int numberOfCompleteRows = (int)command.ExecuteScalar();
            MapObjects.SessionDbConnection.Close();

            if (numberOfCompleteRows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ExecuteCompleteSession()
        {
            SqlCommand command = CreateCompleteSessionCommand(MapObjects.SessionDbConnection.Connection);

            MapObjects.SessionDbConnection.Open();
            command.ExecuteNonQuery();
            MapObjects.SessionDbConnection.Close();
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
            createCompleteSessionCommand.Parameters.AddWithValue("@TransactionTimestamp", DateTime.Now);
            createCompleteSessionCommand.Parameters.AddWithValue("@SessionUid", SessionId);

            return createCompleteSessionCommand;
        }
    }
}