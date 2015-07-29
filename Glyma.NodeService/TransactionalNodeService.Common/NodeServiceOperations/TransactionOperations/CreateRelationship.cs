using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TransactionalNodeService.Common.Model;

namespace TransactionalNodeService.Common.TransactionOperations
{
    public class CreateRelationship : ITransactionOperation
    {
        public CreateRelationship(MapParameters sessionMapParameters)
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
                return TransactionType.CreateRelationship;
            }
        }

        public void ExecuteTransaction(IDbConnectionAbstraction connectionAbstraction, MapTransactionWrapper transactionWrapper, ref MapResponse response)
        {
            IMapTransaction transaction = (IMapTransaction)transactionWrapper;

            DateTime currentTime = DateTime.Now.ToUniversalTime();

            Relationship newRelationship = new Relationship();
            newRelationship.RelationshipTypeUid = transaction.RelationshipTypeUid.Value;
            newRelationship.DomainUid = transactionWrapper.DomainParameter.Value;

            if (transactionWrapper.RootMapParameter != null)
            {
                newRelationship.RootMapUid = transactionWrapper.RootMapParameter.Value;
            }
            else
            {
                newRelationship.RootMapUid = null;
            }

            newRelationship.Created = currentTime;
            newRelationship.Modified = currentTime;
            newRelationship.CreatedBy = transaction.User;
            newRelationship.ModifiedBy = transaction.User;

            Guid newRelationshipId;

            SqlCommand command = CreateRelationshipCommand(connectionAbstraction.Connection, out newRelationshipId, newRelationship);

            connectionAbstraction.Open();

            command.ExecuteNonQuery();
            connectionAbstraction.Close();
            
            newRelationship.RelationshipUid = newRelationshipId;
            response.Changes.Add(new MapChange(transaction.TransactionId, transactionWrapper.ResponseParameter.Id, newRelationship, TransactionType.CreateRelationship));

            transactionWrapper.ResponseParameter.Value = newRelationshipId;
        }

        protected SqlCommand CreateRelationshipCommand(SqlConnection connection, out Guid newRelationshipUid, Relationship newRelationshipParameters)
        {
            SqlCommand createRelationshipCommand = new SqlCommand();
            //createRelationshipCommand.CommandText = "INSERT INTO [Relationships] (RelationshipUid, RelationshipOriginalId, RelationshipTypeUid, DomainUid) VALUES (@RelationshipUid, @RelationshipOriginalId, @RelationshipTypeUid, @DomainUid)";
            createRelationshipCommand.CommandText = "INSERT INTO [Relationships] ([RelationshipUid], [RelationshipTypeUid], [DomainUid], [RootMapUid], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (@RelationshipUid, @RelationshipTypeUid, @DomainUid, @RootMapUid, @Created, @Modified, @CreatedBy, @ModifiedBy)";
            createRelationshipCommand.Connection = connection;

            newRelationshipUid = Guid.NewGuid();

            createRelationshipCommand.Parameters.AddWithValue("@RelationshipUid", newRelationshipUid);
            /// TODO: Need to persist the RelationshipOriginalId value.
            //createRelationshipCommand.Parameters.AddWithValue("@RelationshipOriginalId", OriginalId);
            createRelationshipCommand.Parameters.AddWithValue("@RelationshipTypeUid", newRelationshipParameters.RelationshipTypeUid);
            createRelationshipCommand.Parameters.AddWithValue("@DomainUid", newRelationshipParameters.DomainUid);

            if (newRelationshipParameters.RootMapUid == null)
            {
                createRelationshipCommand.Parameters.AddWithValue("@RootMapUid", DBNull.Value);
            }
            else
            {
                createRelationshipCommand.Parameters.AddWithValue("@RootMapUid", newRelationshipParameters.RootMapUid);
            }

            if (newRelationshipParameters.Created == null)
            {
                createRelationshipCommand.Parameters.AddWithValue("@Created", DBNull.Value);
            }
            else
            {
                createRelationshipCommand.Parameters.AddWithValue("@Created", newRelationshipParameters.Created);
            }

            if (newRelationshipParameters.Modified == null)
            {
                createRelationshipCommand.Parameters.AddWithValue("@Modified", DBNull.Value);
            }
            else
            {
                createRelationshipCommand.Parameters.AddWithValue("@Modified", newRelationshipParameters.Modified);
            }

            if (newRelationshipParameters.CreatedBy == null)
            {
                createRelationshipCommand.Parameters.AddWithValue("@CreatedBy", DBNull.Value);
            }
            else
            {
                createRelationshipCommand.Parameters.AddWithValue("@CreatedBy", newRelationshipParameters.CreatedBy);
            }

            if (newRelationshipParameters.ModifiedBy == null)
            {
                createRelationshipCommand.Parameters.AddWithValue("@ModifiedBy", DBNull.Value);
            }
            else
            {
                createRelationshipCommand.Parameters.AddWithValue("@ModifiedBy", newRelationshipParameters.ModifiedBy);
            }

            return createRelationshipCommand;
        }
    }
}
