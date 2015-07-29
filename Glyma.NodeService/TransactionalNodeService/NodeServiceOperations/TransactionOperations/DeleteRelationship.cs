using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TransactionalNodeService.Model;

namespace TransactionalNodeService.TransactionOperations
{
    public class DeleteRelationship : ITransactionOperation
    {
        public DeleteRelationship(MapParameters sessionMapParameters)
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
                return TransactionType.DeleteRelationship;
            }
        }

        public void ExecuteTransaction(IDbConnectionAbstraction connectionAbstraction, MapTransactionWrapper transactionWrapper, ref MapResponse response)
        {
            IMapTransaction transaction = (IMapTransaction)transactionWrapper;

            SqlCommand command = SelectDeleteRelationshipDescriptorsMetadataCommand(connectionAbstraction.Connection, transactionWrapper.RelationshipParameter.Value);

            connectionAbstraction.Open();

            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Relationship deletedRelationship = new Relationship();
                deletedRelationship.LoadElement(reader);

                response.Changes.Add(new MapChange(transaction.TransactionId, transactionWrapper.ResponseParameter.Id, deletedRelationship, TransactionType.DeleteRelationship));

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        Descriptor deletedDescriptor = new Descriptor();
                        deletedDescriptor.LoadElement(reader);

                        response.Changes.Add(new MapChange(transaction.TransactionId, transactionWrapper.ResponseParameter.Id, deletedDescriptor, TransactionType.DeleteRelationship));
                    }
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        Metadata deletedMetadata = new Metadata();
                        deletedMetadata.LoadElement(reader);

                        response.Changes.Add(new MapChange(transaction.TransactionId, transactionWrapper.ResponseParameter.Id, deletedMetadata, TransactionType.DeleteRelationship));
                    }
                }

                transactionWrapper.ResponseParameter.Value = deletedRelationship.RelationshipUid;
            }
            connectionAbstraction.Close();
        }

        /// <summary>
        /// We need to delete all descriptors and metadata associated with this relationship. There are a few key points to know about this operation -
        ///     1. We don't need ANY of the connected descriptors because otherwise they would be orphaned and would not link the associated nodes to anything.
        ///     2. We don't need ANY of the related metadata. In the scenario where metadata may have both a node and relationship ID it needs to be remembered
        ///     that the metadata doesn't relate to just the node but relates to the node from the perspective of this relationship. If the relationship doesn't
        ///     exist then the metadata is meaningless.
        /// </summary>
        /// <param name="relationshipUid"></param>
        /// <returns></returns>
        protected SqlCommand SelectDeleteRelationshipDescriptorsMetadataCommand(SqlConnection connection, Guid relationshipUid)
        {
            SqlCommand selectDeleteRelationshipDescriptorsMetadataCommand = new SqlCommand();
            selectDeleteRelationshipDescriptorsMetadataCommand.CommandText = @"SELECT * FROM [Relationships] WHERE [RelationshipUid] = @RelationshipUid;
                                                                               SELECT * FROM [Descriptors] WHERE [RelationshipUid] = @RelationshipUid;
                                                                               SELECT * FROM [Metadata] WHERE [RelationshipUid] = @RelationshipUid;
                                                                               DELETE FROM [Metadata] WHERE [RelationshipUid] = @RelationshipUid;
                                                                               DELETE FROM [Descriptors] WHERE [RelationshipUid] = @RelationshipUid;
                                                                               DELETE FROM [Relationships] WHERE [RelationshipUid] = @RelationshipUid;";
            selectDeleteRelationshipDescriptorsMetadataCommand.Connection = connection;

            selectDeleteRelationshipDescriptorsMetadataCommand.Parameters.AddWithValue("@RelationshipUid", relationshipUid);

            return selectDeleteRelationshipDescriptorsMetadataCommand;
        }
    }
}
