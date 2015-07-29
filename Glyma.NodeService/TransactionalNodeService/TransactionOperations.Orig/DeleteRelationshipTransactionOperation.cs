using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace TransactionalNodeService
{
    public class DeleteRelationshipTransactionOperation : TransactionOperation
    {
        protected DeleteRelationshipTransactionOperation()
        {
        }

        public DeleteRelationshipTransactionOperation(SqlConnection connection, TransactionTokenFactory factory)
            : base(connection, factory)
        {
        }

        public TransactionToken DomainId
        {
            get;
            set;
        }

        public TransactionToken RelationshipId
        {
            get;
            set;
        }

        public override void ExecuteOperation()
        {
            DeleteRelationshipCommand().ExecuteNonQuery();

            //TODO: Need to consider deleting descriptors.
        }

        protected SqlCommand DeleteRelationshipCommand()
        {
            SqlCommand deleteNodeCommand = new SqlCommand();
            deleteNodeCommand.CommandText = "DELETE FROM dbo.[Relationships] WHERE [RelationshipUid] = @RelationshipUid AND [DomainUid] = @DomainUid";
            deleteNodeCommand.Connection = Connection;

            deleteNodeCommand.Parameters.AddWithValue("@DomainUid", DomainId.GetValue());
            deleteNodeCommand.Parameters.AddWithValue("@RelationshipUid", RelationshipId.GetValue());

            return deleteNodeCommand;
        }
    }
}