using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace TransactionalNodeService
{
    public class AddNodeTransactionOperation : TransactionOperation
    {
        protected AddNodeTransactionOperation()
        {
        }

        public AddNodeTransactionOperation(SqlConnection connection, TransactionTokenFactory factory)
            : base(connection, factory)
        {
            NewNodeToken = TokenFactory.CreateToken();
            ResultTokens.Add(NewNodeToken);
        }

        public TransactionToken DomainId
        {
            get;
            set;
        }

        public SoapNodeType NodeType
        {
            get;
            set;
        }

        public string OriginalId
        {
            get;
            set;
        }

        public TransactionToken NewNodeToken
        {
            get;
            set;
        }

        public override void ExecuteOperation()
        {
            if (!ExecuteDomainExistsQuery())
            {
                CreateDomainCommand().ExecuteNonQuery();
            }

            Guid newNodeId = Guid.Empty;

            CreateNodeCommand(out newNodeId).ExecuteNonQuery();

            TransactionToken token = NewNodeToken;

            token.DelayedExecutionMapObjectId = newNodeId;

            //ResultTokens.Add(token);
        }

        protected bool ExecuteDomainExistsQuery()
        {
            SqlCommand domainExistsQuery = new SqlCommand();
            domainExistsQuery.CommandText = "SELECT COUNT(*) FROM dbo.[Domains] WHERE dbo.[Domains].[DomainUid] = @DomainUid";
            domainExistsQuery.Connection = Connection;

            domainExistsQuery.Parameters.AddWithValue("@DomainUid", DomainId.GetValue());

            object numberOfRows = domainExistsQuery.ExecuteScalar();

            if (numberOfRows != DBNull.Value)
            {
                if (((int)numberOfRows) > 0)
                {
                    return true;
                }
            }

            return false;
        }

        protected SqlCommand CreateDomainCommand()
        {
            SqlCommand createDomainCommand = new SqlCommand();
            createDomainCommand.CommandText = "INSERT INTO dbo.[Domains] ([DomainUid], [DomainOriginalId]) VALUES (@DomainUid, @DomainOriginalId)";
            createDomainCommand.Connection = Connection;

            return createDomainCommand;
        }

        protected SqlCommand CreateNodeCommand(out Guid newNodeId)
        {
            SqlCommand createNodeCommand = new SqlCommand();
            createNodeCommand.CommandText = "INSERT INTO dbo.[Nodes] ([NodeUid], [NodeOriginalId], [NodeTypeUid], [DomainUid]) VALUES (@NodeUid, @NodeOriginalId, @NodeTypeUid, @DomainUid)";
            createNodeCommand.Connection = Connection;

            newNodeId = Guid.NewGuid();

            createNodeCommand.Parameters.AddWithValue("@NodeUid", newNodeId);
            createNodeCommand.Parameters.AddWithValue("@NodeOriginalId", OriginalId);
            createNodeCommand.Parameters.AddWithValue("@NodeTypeUid", NodeType.Id);
            createNodeCommand.Parameters.AddWithValue("@DomainUid", DomainId.GetValue());

            return createNodeCommand;
        }
    }
}