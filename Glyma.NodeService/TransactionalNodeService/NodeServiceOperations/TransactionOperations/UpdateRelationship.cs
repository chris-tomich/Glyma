using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransactionalNodeService.TransactionOperations;
using System.Data.SqlClient;
using TransactionalNodeService.Model;

namespace TransactionalNodeService.TransactionOperations
{
    public class UpdateRelationship : ITransactionOperation
    {
        public UpdateRelationship(MapParameters sessionMapParameters)
        {
            SessionMapParameters = sessionMapParameters;
        }

        public MapParameters SessionMapParameters
        {
            get;
            protected set;
        }

        public TransactionType OperationType
        {
            get
            {
                return TransactionType.UpdateRelationship;
            }
        }

        public void ExecuteTransaction(IDbConnectionAbstraction connectionAbstraction, MapTransactionWrapper transactionWrapper, ref MapResponse response)
        {
            IMapTransaction transaction = (IMapTransaction)transactionWrapper;

            DateTime currentTime = DateTime.Now;

            Relationship updatedRelationship = new Relationship();
            updatedRelationship.DomainUid = transactionWrapper.DomainParameter.Value;
            updatedRelationship.RelationshipUid = transactionWrapper.RelationshipParameter.Value;
            updatedRelationship.RelationshipTypeUid = transaction.RelationshipTypeUid.Value;
            updatedRelationship.Modified = currentTime;
            updatedRelationship.ModifiedBy = transaction.User;

            SqlCommand updateRelationshipCommand = UpdateRelationshipCommand(connectionAbstraction.Connection, updatedRelationship);

            connectionAbstraction.Open();
            updateRelationshipCommand.ExecuteNonQuery();
            connectionAbstraction.Close();

            response.Changes.Add(new MapChange(transaction.TransactionId, transactionWrapper.ResponseParameter.Id, updatedRelationship, TransactionType.UpdateRelationship));

            transactionWrapper.ResponseParameter.Value = updatedRelationship.RelationshipUid;
        }

        protected SqlCommand UpdateRelationshipCommand(SqlConnection connection, Relationship relationship)
        {
            SqlCommand updateRelationshipCommand = new SqlCommand();
            updateRelationshipCommand.CommandText = "UPDATE [Relationships] SET [RelationshipTypeUid] = @RelationshipTypeUid, [Modified] = @Modified, [ModifiedBy] = @ModifiedBy WHERE [DomainUid] = @DomainUid AND [RelationshipUid] = @RelationshipUid";
            updateRelationshipCommand.Connection = connection;

            updateRelationshipCommand.Parameters.AddWithValue("@RelationshipTypeUid", relationship.RelationshipTypeUid);
            updateRelationshipCommand.Parameters.AddWithValue("@DomainUid", relationship.DomainUid);
            updateRelationshipCommand.Parameters.AddWithValue("@RelationshipUid", relationship.RelationshipUid);

            if (relationship.Modified == null)
            {
                updateRelationshipCommand.Parameters.AddWithValue("@Modified", DBNull.Value);
            }
            else
            {
                updateRelationshipCommand.Parameters.AddWithValue("@Modified", relationship.Modified);
            }

            if (relationship.ModifiedBy == null)
            {
                updateRelationshipCommand.Parameters.AddWithValue("@ModifiedBy", DBNull.Value);
            }
            else
            {
                updateRelationshipCommand.Parameters.AddWithValue("@ModifiedBy", relationship.ModifiedBy);
            }

            return updateRelationshipCommand;
        }
    }
}