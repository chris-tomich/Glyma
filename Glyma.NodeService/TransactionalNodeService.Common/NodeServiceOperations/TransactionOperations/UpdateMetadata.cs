using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TransactionalNodeService.Common.Model;

namespace TransactionalNodeService.Common.TransactionOperations
{
    public class UpdateMetadata : ITransactionOperation
    {
        public UpdateMetadata(MapParameters sessionMapParameters)
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
                return TransactionType.UpdateMetadata;
            }
        }

        public void ExecuteTransaction(IDbConnectionAbstraction connectionAbstraction, MapTransactionWrapper transactionWrapper, ref MapResponse response)
        {
            IMapTransaction transaction = (IMapTransaction)transactionWrapper;

            DateTime currentTime = DateTime.Now.ToUniversalTime();

            Metadata updatedMetadata = new Metadata();
            updatedMetadata.MetadataId = transactionWrapper.MetadataParameter.Value;
            updatedMetadata.MetadataName = transaction.MetadataName;
            updatedMetadata.MetadataValue = transaction.MetadataValue;
            updatedMetadata.Modified = currentTime;
            updatedMetadata.ModifiedBy = transaction.User;

            SqlCommand command = UpdateMetadataCommand(connectionAbstraction.Connection, updatedMetadata);

            connectionAbstraction.Open();
            command.ExecuteNonQuery();
            connectionAbstraction.Close();

            response.Changes.Add(new MapChange(transaction.TransactionId, transactionWrapper.ResponseParameter.Id, updatedMetadata, TransactionType.UpdateMetadata));

            transactionWrapper.ResponseParameter.Value = updatedMetadata.MetadataId;
        }

        protected SqlCommand UpdateMetadataCommand(SqlConnection connection, Metadata metadata)
        {
            bool hasName = false;
            bool hasValue = false;
            string commandText = "UPDATE [Metadata] SET {0}{1}{2}, [Modified] = @Modified, [ModifiedBy] = @ModifiedBy WHERE [MetadataId] = @MetadataId";
            string metadataNameText = string.Empty;
            string metadataValueText = string.Empty;
            string separatorText = string.Empty;

            if (!string.IsNullOrEmpty(metadata.MetadataName))
            {
                metadataNameText = "[MetadataName] = @MetadataName";
                hasName = true;
            }

            if (metadata.MetadataValue != null)
            {
                metadataValueText = "[MetadataValue] = @MetadataValue";
                hasValue = true;
            }

            if (hasName && hasValue)
            {
                separatorText = ", ";
            }

            SqlCommand updateMetadataCommand = new SqlCommand();
            updateMetadataCommand.CommandText = string.Format(commandText, metadataNameText, separatorText, metadataValueText);
            updateMetadataCommand.Connection = connection;

            updateMetadataCommand.Parameters.AddWithValue("@MetadataId", metadata.MetadataId);

            if (hasName)
            {
                updateMetadataCommand.Parameters.AddWithValue("@MetadataName", metadata.MetadataName);
            }

            if (hasValue)
            {
                updateMetadataCommand.Parameters.AddWithValue("@MetadataValue", metadata.MetadataValue);
            }

            if (metadata.Modified == null)
            {
                updateMetadataCommand.Parameters.AddWithValue("@Modified", DBNull.Value);
            }
            else
            {
                updateMetadataCommand.Parameters.AddWithValue("@Modified", metadata.Modified);
            }

            if (metadata.ModifiedBy == null)
            {
                updateMetadataCommand.Parameters.AddWithValue("@ModifiedBy", DBNull.Value);
            }
            else
            {
                updateMetadataCommand.Parameters.AddWithValue("@ModifiedBy", metadata.ModifiedBy);
            }

            return updateMetadataCommand;
        }
    }
}
