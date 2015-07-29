using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TransactionalNodeService.Model;

namespace TransactionalNodeService.TransactionOperations
{
    public class CreateMetadata : ITransactionOperation
    {
        public CreateMetadata(MapParameters sessionMapParameters)
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
                return TransactionType.CreateMetadata;
            }
        }

        public void ExecuteTransaction(IDbConnectionAbstraction connectionAbstraction, MapTransactionWrapper transactionWrapper, ref MapResponse response)
        {
            IMapTransaction transaction = (IMapTransaction)transactionWrapper;

            DateTime currentTime = DateTime.Now;

            Metadata newMetadata = new Metadata();
            newMetadata.MetadataTypeUid = transaction.MetadataTypeUid.Value;
            newMetadata.DomainUid = transactionWrapper.DomainParameter.Value;

            if (transactionWrapper.RootMapParameter != null)
            {
                newMetadata.RootMapUid = transactionWrapper.RootMapParameter.Value;
            }
            else
            {
                newMetadata.RootMapUid = null;
            }

            newMetadata.Created = currentTime;
            newMetadata.Modified = currentTime;
            newMetadata.CreatedBy = transaction.User;
            newMetadata.ModifiedBy = transaction.User;

            if (transactionWrapper.NodeParameter == null)
            {
                newMetadata.NodeUid = null;
            }
            else
            {
                newMetadata.NodeUid = transactionWrapper.NodeParameter.Value;
            }

            if (transactionWrapper.RelationshipParameter == null)
            {
                newMetadata.RelationshipUid = null;
            }
            else
            {
                newMetadata.RelationshipUid = transactionWrapper.RelationshipParameter.Value;
            }

            if (transaction.DescriptorTypeUid == null)
            {
                newMetadata.DescriptorTypeUid = null;
            }
            else
            {
                newMetadata.DescriptorTypeUid = transaction.DescriptorTypeUid.Value;
            }

            newMetadata.MetadataName = transaction.MetadataName;
            newMetadata.MetadataValue = transaction.MetadataValue;

            Guid newMetadataId;

            SqlCommand command = CreateMetadataCommand(connectionAbstraction.Connection, out newMetadataId, newMetadata);

            connectionAbstraction.Open();
            command.ExecuteNonQuery();
            connectionAbstraction.Close();

            newMetadata.MetadataId = newMetadataId;
            response.Changes.Add(new MapChange(transaction.TransactionId, transactionWrapper.ResponseParameter.Id, newMetadata, TransactionType.CreateMetadata));

            transactionWrapper.ResponseParameter.Value = newMetadataId;
        }

        protected SqlCommand CreateMetadataCommand(SqlConnection connection, out Guid newMetadataId, Metadata newMetadataParameters)
        {
            SqlCommand createMetadataCommand = new SqlCommand();
            createMetadataCommand.CommandText = "INSERT INTO [Metadata] ([MetadataId], [MetadataTypeUid], [NodeUid], [RelationshipUid], [DescriptorTypeUid], [MetadataName], [MetadataValue], [DomainUid], [RootMapUid], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (@MetadataId, @MetadataTypeUid, @NodeUid, @RelationshipUid, @DescriptorTypeUid, @MetadataName, @MetadataValue, @DomainUid, @RootMapUid, @Created, @Modified, @CreatedBy, @ModifiedBy)";
            createMetadataCommand.Connection = connection;

            newMetadataId = Guid.NewGuid();

            createMetadataCommand.Parameters.AddWithValue("@MetadataId", newMetadataId);
            createMetadataCommand.Parameters.AddWithValue("@MetadataTypeUid", newMetadataParameters.MetadataTypeUid);

            if (newMetadataParameters.NodeUid == null)
            {
                createMetadataCommand.Parameters.AddWithValue("@NodeUid", DBNull.Value);
            }
            else
            {
                createMetadataCommand.Parameters.AddWithValue("@NodeUid", newMetadataParameters.NodeUid);
            }

            if (newMetadataParameters.RelationshipUid == null)
            {
                createMetadataCommand.Parameters.AddWithValue("@RelationshipUid", DBNull.Value);
            }
            else
            {
                createMetadataCommand.Parameters.AddWithValue("@RelationshipUid", newMetadataParameters.RelationshipUid);
            }

            if (newMetadataParameters.DescriptorTypeUid == null)
            {
                createMetadataCommand.Parameters.AddWithValue("@DescriptorTypeUid", DBNull.Value);
            }
            else
            {
                createMetadataCommand.Parameters.AddWithValue("@DescriptorTypeUid", newMetadataParameters.DescriptorTypeUid);
            }

            createMetadataCommand.Parameters.AddWithValue("@MetadataName", newMetadataParameters.MetadataName);
            createMetadataCommand.Parameters.AddWithValue("@MetadataValue", newMetadataParameters.MetadataValue);
            createMetadataCommand.Parameters.AddWithValue("@DomainUid", newMetadataParameters.DomainUid);

            if (newMetadataParameters.RootMapUid == null)
            {
                createMetadataCommand.Parameters.AddWithValue("@RootMapUid", DBNull.Value);
            }
            else
            {
                createMetadataCommand.Parameters.AddWithValue("@RootMapUid", newMetadataParameters.RootMapUid);
            }

            if (newMetadataParameters.Created == null)
            {
                createMetadataCommand.Parameters.AddWithValue("@Created", DBNull.Value);
            }
            else
            {
                createMetadataCommand.Parameters.AddWithValue("@Created", newMetadataParameters.Created);
            }

            if (newMetadataParameters.Modified == null)
            {
                createMetadataCommand.Parameters.AddWithValue("@Modified", DBNull.Value);
            }
            else
            {
                createMetadataCommand.Parameters.AddWithValue("@Modified", newMetadataParameters.Modified);
            }

            if (newMetadataParameters.CreatedBy == null)
            {
                createMetadataCommand.Parameters.AddWithValue("@CreatedBy", DBNull.Value);
            }
            else
            {
                createMetadataCommand.Parameters.AddWithValue("@CreatedBy", newMetadataParameters.CreatedBy);
            }

            if (newMetadataParameters.ModifiedBy == null)
            {
                createMetadataCommand.Parameters.AddWithValue("@ModifiedBy", DBNull.Value);
            }
            else
            {
                createMetadataCommand.Parameters.AddWithValue("@ModifiedBy", newMetadataParameters.ModifiedBy);
            }

            return createMetadataCommand;
        }
    }
}
