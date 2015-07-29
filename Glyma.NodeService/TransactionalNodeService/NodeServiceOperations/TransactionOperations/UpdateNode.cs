using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransactionalNodeService.TransactionOperations;
using System.Data.SqlClient;
using TransactionalNodeService.Model;

namespace TransactionalNodeService.TransactionOperations
{
    public class UpdateNode : ITransactionOperation
    {
        public UpdateNode(MapParameters sessionMapParameters)
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
                return TransactionType.UpdateNode;
            }
        }

        public void ExecuteTransaction(IDbConnectionAbstraction connectionAbstraction, MapTransactionWrapper transactionWrapper, ref MapResponse response)
        {
            IMapTransaction transaction = (IMapTransaction)transactionWrapper;

            DateTime currentTime = DateTime.Now;

            Node updatedNode = new Node();
            updatedNode.DomainUid = transactionWrapper.DomainParameter.Value;
            updatedNode.NodeUid = transactionWrapper.NodeParameter.Value;
            updatedNode.NodeTypeUid = transaction.NodeTypeUid.Value;
            updatedNode.Modified = currentTime;
            updatedNode.ModifiedBy = transaction.User;

            SqlCommand updateNodeCommand = UpdateNodeCommand(connectionAbstraction.Connection, updatedNode);

            connectionAbstraction.Open();
            updateNodeCommand.ExecuteNonQuery();
            connectionAbstraction.Close();

            response.Changes.Add(new MapChange(transaction.TransactionId, transactionWrapper.ResponseParameter.Id, updatedNode, TransactionType.UpdateNode));

            transactionWrapper.ResponseParameter.Value = updatedNode.NodeUid;
        }

        protected SqlCommand UpdateNodeCommand(SqlConnection connection, Node node)
        {
            SqlCommand updateNodeCommand = new SqlCommand();
            updateNodeCommand.CommandText = "UPDATE [Nodes] SET [NodeTypeUid] = @NodeTypeUid, [Modified] = @Modified, [ModifiedBy] = @ModifiedBy WHERE [DomainUid] = @DomainUid AND [NodeUid] = @NodeUid";
            updateNodeCommand.Connection = connection;

            updateNodeCommand.Parameters.AddWithValue("@DomainUid", node.DomainUid);
            updateNodeCommand.Parameters.AddWithValue("@NodeUid", node.NodeUid);
            updateNodeCommand.Parameters.AddWithValue("@NodeTypeUid", node.NodeTypeUid);

            if (node.Modified == null)
            {
                updateNodeCommand.Parameters.AddWithValue("@Modified", DBNull.Value);
            }
            else
            {
                updateNodeCommand.Parameters.AddWithValue("@Modified", node.Modified);
            }

            if (node.ModifiedBy == null)
            {
                updateNodeCommand.Parameters.AddWithValue("@ModifiedBy", DBNull.Value);
            }
            else
            {
                updateNodeCommand.Parameters.AddWithValue("@ModifiedBy", node.ModifiedBy);
            }

            return updateNodeCommand;
        }
    }
}