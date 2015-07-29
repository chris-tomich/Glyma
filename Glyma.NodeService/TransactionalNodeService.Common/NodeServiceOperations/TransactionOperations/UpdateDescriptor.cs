using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransactionalNodeService.Common.TransactionOperations;
using System.Data.SqlClient;
using TransactionalNodeService.Common.Model;

namespace TransactionalNodeService.Common.TransactionOperations
{
    public class UpdateDescriptor : ITransactionOperation
    {
        public UpdateDescriptor(MapParameters sessionMapParameters)
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
                return TransactionType.UpdateDescriptor;
            }
        }

        public void ExecuteTransaction(IDbConnectionAbstraction connectionAbstraction, MapTransactionWrapper transactionWrapper, ref MapResponse response)
        {
            IMapTransaction transaction = (IMapTransaction)transactionWrapper;

            Descriptor updatedDescriptor = new Descriptor();
            updatedDescriptor.NodeUid = transactionWrapper.NodeParameter.Value;
            updatedDescriptor.DescriptorTypeUid = transaction.DescriptorTypeUid.Value;
            updatedDescriptor.RelationshipUid = transactionWrapper.RelationshipParameter.Value;

            SqlCommand command = UpdateDescriptorCommand(connectionAbstraction.Connection, updatedDescriptor);

            connectionAbstraction.Open();
            object descriptorUidAsObject = command.ExecuteScalar();
            connectionAbstraction.Close();

            if (descriptorUidAsObject != null && descriptorUidAsObject != DBNull.Value)
            {
                updatedDescriptor.DescriptorUid = (Guid)descriptorUidAsObject;
            }
            else
            {
                // This case occurs if the relationship type has been changed and previously this descriptor didn't exist for this relationship type.
                Guid newDescriptorUid;

                SqlCommand createDescriptorCommand = CreateDescriptorCommand(connectionAbstraction.Connection, out newDescriptorUid, updatedDescriptor);
                updatedDescriptor.DescriptorUid = newDescriptorUid;

                connectionAbstraction.Open();
                createDescriptorCommand.ExecuteNonQuery();
                connectionAbstraction.Close();
            }

            response.Changes.Add(new MapChange(transaction.TransactionId, transactionWrapper.ResponseParameter.Id, updatedDescriptor, TransactionType.UpdateDescriptor));

            transactionWrapper.ResponseParameter.Value = updatedDescriptor.DescriptorUid;
        }

        protected SqlCommand UpdateDescriptorCommand(SqlConnection connection, Descriptor descriptor)
        {
            SqlCommand updateDescriptorCommand = new SqlCommand();
            updateDescriptorCommand.CommandText = @"SELECT TOP 1 [DescriptorUid] FROM [Descriptors] WHERE [DescriptorTypeUid] = @DescriptorTypeUid AND [RelationshipUid] = @RelationshipUid;
                                                    UPDATE [Descriptors] SET [NodeUid] = @NodeUid WHERE [DescriptorTypeUid] = @DescriptorTypeUid AND [RelationshipUid] = @RelationshipUid;";
            updateDescriptorCommand.Connection = connection;

            updateDescriptorCommand.Parameters.AddWithValue("@NodeUid", descriptor.NodeUid);
            updateDescriptorCommand.Parameters.AddWithValue("@DescriptorTypeUid", descriptor.DescriptorTypeUid);
            updateDescriptorCommand.Parameters.AddWithValue("@RelationshipUid", descriptor.RelationshipUid);

            return updateDescriptorCommand;
        }

        protected SqlCommand CreateDescriptorCommand(SqlConnection connection, out Guid newDescriptorUid, Descriptor newDescriptorParameters)
        {
            SqlCommand createDescriptorCommand = new SqlCommand();
            createDescriptorCommand.CommandText = "INSERT INTO [Descriptors] ([DescriptorUid], [DescriptorTypeUid], [NodeUid], [RelationshipUid]) VALUES (@DescriptorUid, @DescriptorTypeUid, @NodeUid, @RelationshipUid)";
            createDescriptorCommand.Connection = connection;

            newDescriptorUid = Guid.NewGuid();

            createDescriptorCommand.Parameters.AddWithValue("@DescriptorUid", newDescriptorUid);
            createDescriptorCommand.Parameters.AddWithValue("@DescriptorTypeUid", newDescriptorParameters.DescriptorTypeUid);
            createDescriptorCommand.Parameters.AddWithValue("@NodeUid", newDescriptorParameters.NodeUid);
            createDescriptorCommand.Parameters.AddWithValue("@RelationshipUid", newDescriptorParameters.RelationshipUid);

            return createDescriptorCommand;
        }
    }
}