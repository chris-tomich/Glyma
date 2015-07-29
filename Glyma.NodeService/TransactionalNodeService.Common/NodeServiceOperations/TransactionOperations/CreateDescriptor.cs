using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TransactionalNodeService.Common.Model;

namespace TransactionalNodeService.Common.TransactionOperations
{
    public class CreateDescriptor : ITransactionOperation
    {
        internal CreateDescriptor(MapParameters sessionMapParameters)
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
                return TransactionType.CreateDescriptor;
            }
        }

        public void ExecuteTransaction(IDbConnectionAbstraction connectionAbstraction, MapTransactionWrapper transactionWrapper, ref MapResponse response)
        {
            IMapTransaction transaction = (IMapTransaction)transactionWrapper;

            Descriptor newDescriptor = new Descriptor();
            newDescriptor.DescriptorTypeUid = transaction.DescriptorTypeUid.Value;
            newDescriptor.NodeUid = transactionWrapper.NodeParameter.Value;
            newDescriptor.RelationshipUid = transactionWrapper.RelationshipParameter.Value;

            Guid newDescriptorId;

            SqlCommand command = CreateDescriptorCommand(connectionAbstraction.Connection, out newDescriptorId, newDescriptor);

            connectionAbstraction.Open();
            command.ExecuteNonQuery();
            connectionAbstraction.Close();

            newDescriptor.DescriptorUid = newDescriptorId;
            response.Changes.Add(new MapChange(transaction.TransactionId, newDescriptor, TransactionType.CreateDescriptor));

            transactionWrapper.ResponseParameter.Value = newDescriptorId;
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
