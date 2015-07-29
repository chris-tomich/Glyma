using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TransactionalNodeService.Common.Model;

namespace TransactionalNodeService.Common.TransactionOperations
{
    public class DeleteMetadata : ITransactionOperation
    {
        public DeleteMetadata(MapParameters sessionMapParameters)
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
                return TransactionType.DeleteMetadata;
            }
        }

        public void ExecuteTransaction(IDbConnectionAbstraction connectionAbstraction, MapTransactionWrapper transactionWrapper, ref MapResponse response)
        {
            IMapTransaction transaction = (IMapTransaction)transactionWrapper;

            SqlCommand command = SelectDeleteMetadataCommand(connectionAbstraction.Connection, transactionWrapper.MetadataParameter.Value);

            connectionAbstraction.Open();

            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Metadata deletedMetadata = new Metadata();
                deletedMetadata.LoadElement(reader);

                response.Changes.Add(new MapChange(transaction.TransactionId, transactionWrapper.ResponseParameter.Id, deletedMetadata, TransactionType.DeleteMetadata));

                transactionWrapper.ResponseParameter.Value = deletedMetadata.MetadataId;
            }
            connectionAbstraction.Close();
        }

        protected SqlCommand SelectDeleteMetadataCommand(SqlConnection connection, Guid metadataId)
        {
            SqlCommand selectDeleteMetadataCommand = new SqlCommand();
            selectDeleteMetadataCommand.CommandText = "SELECT * FROM [Metadata] WHERE [MetadataId] = @MetadataId; DELETE FROM [Metadata] WHERE [MetadataId] = @MetadataId;";
            selectDeleteMetadataCommand.Connection = connection;

            selectDeleteMetadataCommand.Parameters.AddWithValue("@MetadataId", metadataId);

            return selectDeleteMetadataCommand;
        }
    }
}
