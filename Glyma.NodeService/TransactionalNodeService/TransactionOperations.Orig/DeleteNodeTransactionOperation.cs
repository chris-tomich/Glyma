using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace TransactionalNodeService
{
    public class DeleteNodeTransactionOperation : TransactionOperation
    {
        protected DeleteNodeTransactionOperation()
        {
        }

        public DeleteNodeTransactionOperation(SqlConnection connection, TransactionTokenFactory factory)
            : base(connection, factory)
        {
        }

        public TransactionToken DomainId
        {
            get;
            set;
        }

        public TransactionToken NodeId
        {
            get;
            set;
        }

        public override void ExecuteOperation()
        {
            DeleteNodeCommand().ExecuteNonQuery();

            //TODO: Need to consider deleting relationships and descriptors.
        }

        protected SqlCommand DeleteNodeCommand()
        {
            SqlCommand deleteNodeCommand = new SqlCommand();
            deleteNodeCommand.CommandText = "DELETE FROM dbo.[Nodes] WHERE [NodeUid] = @NodeUid AND [DomainUid] = @DomainUid";
            deleteNodeCommand.Connection = Connection;

            deleteNodeCommand.Parameters.AddWithValue("@DomainUid", DomainId.GetValue());
            deleteNodeCommand.Parameters.AddWithValue("@NodeUid", NodeId.GetValue());

            return deleteNodeCommand;
        }
    }
}