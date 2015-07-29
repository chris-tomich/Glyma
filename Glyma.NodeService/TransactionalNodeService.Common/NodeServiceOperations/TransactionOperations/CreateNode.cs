using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using TransactionalNodeService.Common.Model;

namespace TransactionalNodeService.Common.TransactionOperations
{
    public class CreateNode : ITransactionOperation
    {
        public CreateNode(MapParameters sessionMapParameters)
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
                return TransactionType.CreateNode;
            }
        }

        public void ExecuteTransaction(IDbConnectionAbstraction connectionAbstraction, MapTransactionWrapper transactionWrapper, ref MapResponse response)
        {
            IMapTransaction transaction = (IMapTransaction)transactionWrapper;

            DateTime currentTime = DateTime.Now.ToUniversalTime();

            Node newNode = new Node();
            newNode.NodeTypeUid = transaction.NodeTypeUid.Value;
            newNode.DomainUid = transactionWrapper.DomainParameter.Value;

            if (transactionWrapper.RootMapParameter != null)
            {
                newNode.RootMapUid = transactionWrapper.RootMapParameter.Value;
            }
            else
            {
                newNode.RootMapUid = null;
            }

            newNode.Created = currentTime;
            newNode.Modified = currentTime;
            newNode.CreatedBy = transaction.User;
            newNode.ModifiedBy = transaction.User;

            Guid newNodeId;

            SqlCommand command = CreateNodeCommand(connectionAbstraction.Connection, out newNodeId, newNode);

            connectionAbstraction.Open();
            command.ExecuteNonQuery();
            connectionAbstraction.Close();

            newNode.NodeUid = newNodeId;
            response.Changes.Add(new MapChange(transaction.TransactionId, transactionWrapper.ResponseParameter.Id, newNode, TransactionType.CreateNode));

            transactionWrapper.ResponseParameter.Value = newNodeId;
        }

        protected SqlCommand CreateNodeCommand(SqlConnection connection, out Guid newNodeId, Node newNodeParameters)
        {
            SqlCommand createNodeCommand = new SqlCommand();
            //createNodeCommand.CommandText = "INSERT INTO [Nodes] ([NodeUid], [NodeOriginalId], [NodeTypeUid], [DomainUid]) VALUES (@NodeUid, @NodeOriginalId, @NodeTypeUid, @DomainUid)";
            createNodeCommand.CommandText = "INSERT INTO [Nodes] ([NodeUid], [NodeTypeUid], [DomainUid], [RootMapUid], [Created], [Modified], [CreatedBy], [ModifiedBy]) VALUES (@NodeUid, @NodeTypeUid, @DomainUid, @RootMapUid, @Created, @Modified, @CreatedBy, @ModifiedBy)";
            createNodeCommand.Connection = connection;

            newNodeId = Guid.NewGuid();

            createNodeCommand.Parameters.AddWithValue("@NodeUid", newNodeId);
            /// TODO: Need to persist the NodeOriginalId value.
            //createNodeCommand.Parameters.AddWithValue("@NodeOriginalId", GetNodeUid(mapParameters));
            createNodeCommand.Parameters.AddWithValue("@NodeTypeUid", newNodeParameters.NodeTypeUid);
            createNodeCommand.Parameters.AddWithValue("@DomainUid", newNodeParameters.DomainUid);

            if (newNodeParameters.RootMapUid == null)
            {
                createNodeCommand.Parameters.AddWithValue("@RootMapUid", DBNull.Value);
            }
            else
            {
                createNodeCommand.Parameters.AddWithValue("@RootMapUid", newNodeParameters.RootMapUid);
            }

            if (newNodeParameters.Created == null)
            {
                createNodeCommand.Parameters.AddWithValue("@Created", DBNull.Value);
            }
            else
            {
                createNodeCommand.Parameters.AddWithValue("@Created", newNodeParameters.Created);
            }

            if (newNodeParameters.Modified == null)
            {
                createNodeCommand.Parameters.AddWithValue("@Modified", DBNull.Value);
            }
            else
            {
                createNodeCommand.Parameters.AddWithValue("@Modified", newNodeParameters.Modified);
            }

            if (newNodeParameters.CreatedBy == null)
            {
                createNodeCommand.Parameters.AddWithValue("@CreatedBy", DBNull.Value);
            }
            else
            {
                createNodeCommand.Parameters.AddWithValue("@CreatedBy", newNodeParameters.CreatedBy);
            }

            if (newNodeParameters.ModifiedBy == null)
            {
                createNodeCommand.Parameters.AddWithValue("@ModifiedBy", DBNull.Value);
            }
            else
            {
                createNodeCommand.Parameters.AddWithValue("@ModifiedBy", newNodeParameters.ModifiedBy);
            }

            return createNodeCommand;
        }
    }
}
