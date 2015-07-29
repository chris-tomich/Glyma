using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TransactionalNodeService.Model;

namespace TransactionalNodeService.TransactionOperations
{
    public class DeleteNode : ITransactionOperation
    {
        public DeleteNode(MapParameters sessionMapParameters)
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
                return TransactionType.DeleteNode;
            }
        }

        public void ExecuteTransaction(IDbConnectionAbstraction connectionAbstraction, MapTransactionWrapper transactionWrapper, ref MapResponse response)
        {
            IMapTransaction transaction = (IMapTransaction)transactionWrapper;

            SqlCommand command = SelectDeleteNodeRelationshipsDescriptorsMetadataCommand(connectionAbstraction.Connection, transactionWrapper.NodeParameter.Value);

            connectionAbstraction.Open();

            SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                Node deletedNode = new Node();
                deletedNode.LoadElement(reader);

                response.Changes.Add(new MapChange(transaction.TransactionId, transactionWrapper.ResponseParameter.Id, deletedNode, TransactionType.DeleteNode));

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        Metadata deletedMetadata = new Metadata();
                        deletedMetadata.LoadElement(reader);

                        response.Changes.Add(new MapChange(transaction.TransactionId, transactionWrapper.ResponseParameter.Id, deletedMetadata, TransactionType.DeleteMetadata));
                    }
                }

                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        Relationship deletedRelationship = new Relationship();
                        deletedRelationship.LoadElement(reader);

                        response.Changes.Add(new MapChange(transaction.TransactionId, transactionWrapper.ResponseParameter.Id, deletedRelationship, TransactionType.DeleteRelationship));
                    }
                }

                transactionWrapper.ResponseParameter.Value = deletedNode.NodeUid;
            }
            connectionAbstraction.Close();
        }

        protected SqlCommand SelectDeleteNodeRelationshipsDescriptorsMetadataCommand(SqlConnection connection, Guid nodeUid)
        {
            SqlCommand selectDeleteNodeRelationshipsDescriptorsMetadataCommand = new SqlCommand();
            selectDeleteNodeRelationshipsDescriptorsMetadataCommand.CommandText = @"DECLARE @FirstLevelDescriptors TABLE(
                                                                                        DescriptorUid uniqueidentifier NOT NULL,
                                                                                        DescriptorTypeUid uniqueidentifier,
                                                                                        NodeUid uniqueidentifier,
                                                                                        RelationshipUid uniqueidentifier);
                                                                                    DECLARE @Relationships TABLE(
                                                                                        RelationshipUid uniqueidentifier NOT NULL);
                                                                                    SELECT [Nodes].* FROM [Nodes] WHERE [Nodes].[NodeUid] = @NodeUid;
                                                                                    SELECT [Metadata].* FROM [Metadata] WHERE [Metadata].[NodeUid] = @NodeUid
                                                                                    UNION
                                                                                    SELECT [Metadata].* FROM [Metadata] INNER JOIN @Relationships ON [Metadata].[RelationshipUid] = [@Relationships].[RelationshipUid];
                                                                                    DELETE FROM [Descriptors] OUTPUT DELETED.* INTO @FirstLevelDescriptors WHERE [Descriptors].[NodeUid] = @NodeUid;
                                                                                    INSERT @Relationships SELECT [Relationships].[RelationshipUid] FROM [Relationships] INNER JOIN @FirstLevelDescriptors ON [Relationships].[RelationshipUid] = [@FirstLevelDescriptors].[RelationshipUid];
                                                                                    SELECT [Relationships].* FROM [Relationships] INNER JOIN @Relationships ON [Relationships].[RelationshipUid] = [@Relationships].[RelationshipUid];
                                                                                    DELETE FROM [Metadata] WHERE [Metadata].[NodeUid] = @NodeUid;
                                                                                    DELETE FROM [Metadata] FROM @Relationships WHERE [Metadata].[RelationshipUid] = [@Relationships].[RelationshipUid];
                                                                                    DELETE FROM [Descriptors] FROM @Relationships WHERE [Descriptors].[RelationshipUid] = [@Relationships].[RelationshipUid];
                                                                                    DELETE FROM [Relationships] FROM @FirstLevelDescriptors WHERE [@FirstLevelDescriptors].[RelationshipUid] = [Relationships].[RelationshipUid] AND [@FirstLevelDescriptors].[NodeUid] = @NodeUid;
                                                                                    DELETE FROM [Nodes] WHERE [Nodes].[NodeUid] = @NodeUid;";
            selectDeleteNodeRelationshipsDescriptorsMetadataCommand.Connection = connection;

            selectDeleteNodeRelationshipsDescriptorsMetadataCommand.Parameters.AddWithValue("@NodeUid", nodeUid);

            return selectDeleteNodeRelationshipsDescriptorsMetadataCommand;
        }
    }
}
