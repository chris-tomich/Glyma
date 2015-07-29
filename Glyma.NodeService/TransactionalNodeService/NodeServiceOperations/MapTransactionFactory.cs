using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using TransactionalNodeService.TransactionOperations;

namespace TransactionalNodeService
{
    public sealed class MapTransactionFactory
    {
        private const string SelectSessionTransactionsSqlQuery = "SELECT [TransactionId], [TransactionTimestamp], [User], [SessionUid], [OperationId], [NodeUid], [DescriptorUid], [RelationshipUid], [MetadataUid], [NodeTypeUid], [DescriptorTypeUid], [RelationshipTypeUid], [MetadataTypeUid], [MetadataName], [MetadataValue] FROM [Transactions] WHERE [SessionUid] = @SessionId";

        private Guid _sessionId;
        private SqlConnection _connection;

        public MapTransactionFactory(SqlConnection connection, Guid sessionId)
        {
            _sessionId = sessionId;
            _connection = connection;
        }

        public ITransactionOperation CreateTransactionOperation(TransactionType transactionType, MapParameters parameters)
        {
            ITransactionOperation transactionOperation;

            switch (transactionType)
            {
                case TransactionType.CreateNode:
                    transactionOperation = new CreateNode(_connection, parameters);
                    break;
                case TransactionType.DeleteNode:
                    transactionOperation = new DeleteNode(_connection, parameters);
                    break;
                case TransactionType.CreateRelationship:
                    transactionOperation = new CreateRelationship(_connection, parameters);
                    break;
                case TransactionType.DeleteRelationship:
                    transactionOperation = new DeleteRelationship(_connection, parameters);
                    break;
                case TransactionType.CreateDescriptor:
                    transactionOperation = new CreateDescriptor(_connection, parameters);
                    break;
                case TransactionType.CreateMetadata:
                    transactionOperation = new CreateMetadata(_connection, parameters);
                    break;
                case TransactionType.UpdateMetadata:
                    transactionOperation = new UpdateMetadata(_connection, parameters);
                    break;
                case TransactionType.DeleteMetadata:
                    transactionOperation = new DeleteMetadata(_connection, parameters);
                    break;
                default:
                    throw new NotSupportedException("The requested transaction type doesn't exist.");
            }

            return transactionOperation;
        }

        public Queue<MapTransaction> GetSessionTransactions(Guid sessionId)
        {
            Queue<MapTransaction> transactions = new Queue<MapTransaction>();

            /// TODO: Get all the transactions persisted in SQL server.
            /// TODO: Need to really turn this method and the following code into a factory. Need to dispose the objects but this could also be a little more encapsulated.
            SqlConnection connection = new SqlConnection("Data Source=chris-ultrabook;Initial Catalog=MappingToolDatabase;Integrated Security=True");

            SqlParameter sessionIdParameter = new SqlParameter("@SessionId", sessionId);

            SqlCommand selectSessionTransactions = new SqlCommand(SelectSessionTransactionsSqlQuery, connection);
            selectSessionTransactions.Parameters.Add(sessionId);

            SqlDataReader sessions = selectSessionTransactions.ExecuteReader();

            TransactionType transactionType = (TransactionType)sessions["OperationId"];
            MapParameters mapParameters = new MapParameters(connection, sessionId);

            while (sessions.Read())
            {
                MapTransaction mapTransaction = new MapTransaction(CreateTransactionOperation(transactionType, mapParameters));// = CreateTransaction((TransactionType)sessions["OperationId"]);

                mapTransaction.LoadTransaction(sessions);

                transactions.Enqueue(mapTransaction);
            }

            return transactions;
        }
    }
}