using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;

namespace NodeService
{
    public class AddRelationshipTransactionOperation : TransactionOperation
    {
        protected AddRelationshipTransactionOperation()
        {
        }

        public AddRelationshipTransactionOperation(SqlConnection connection, TransactionTokenFactory factory)
            : base(connection, factory)
        {
            ResultTokens.Add(TokenFactory.CreateToken());
        }

        public TransactionToken DomainId
        {
            get;
            set;
        }

        public Dictionary<SoapDescriptorType, TransactionToken> Nodes
        {
            get;
            set;
        }

        public SoapRelationshipType RelationshipType
        {
            get;
            set;
        }

        public string OriginalId
        {
            get;
            set;
        }

        public override void ExecuteOperation()
        {
            Guid newRelationshipUid = Guid.Empty;

            CreateRelationship(out newRelationshipUid).ExecuteNonQuery();

            foreach (KeyValuePair<SoapDescriptorType, TransactionToken> node in Nodes)
            {
                Guid newDescriptorUid = Guid.Empty;

                CreateDescriptor(out newDescriptorUid, node.Key, node.Value, newRelationshipUid).ExecuteNonQuery();
            }

            ResultTokens.First().DelayedExecutionMapObjectId = newRelationshipUid;
        }

        protected SqlCommand CreateRelationship(out Guid newRelationshipUid)
        {
            SqlCommand createRelationshipCommand = new SqlCommand();
            createRelationshipCommand.CommandText = "INSERT INTO dbo.[Relationships] (RelationshipUid, RelationshipOriginalId, RelationshipTypeUid, DomainUid) VALUES (@RelationshipUid, @RelationshipOriginalId, @RelationshipTypeUid, @DomainUid)";
            createRelationshipCommand.Connection = Connection;

            newRelationshipUid = Guid.NewGuid();

            createRelationshipCommand.Parameters.AddWithValue("@RelationshipUid", newRelationshipUid);
            createRelationshipCommand.Parameters.AddWithValue("@RelationshipOriginalId", OriginalId);
            createRelationshipCommand.Parameters.AddWithValue("@RelationshipTypeUid", RelationshipType.Id);
            createRelationshipCommand.Parameters.AddWithValue("@DomainUid", DomainId.GetValue());

            return createRelationshipCommand;
        }

        protected SqlCommand CreateDescriptor(out Guid newDescriptorUid, SoapDescriptorType descriptorType, TransactionToken nodeUid, Guid relationshipUid)
        {
            SqlCommand createDescriptorCommand = new SqlCommand();
            createDescriptorCommand.CommandText = "INSERT INTO dbo.[Descriptors] (DescriptorUid, DescriptorTypeUid, NodeUid, RelationshipUid) VALUES (@DescriptorUid, @DescriptorTypeUid, @NodeUid, @RelationshipUid)";
            createDescriptorCommand.Connection = Connection;

            newDescriptorUid = Guid.NewGuid();

            createDescriptorCommand.Parameters.AddWithValue("@DescriptorUid", newDescriptorUid);
            createDescriptorCommand.Parameters.AddWithValue("@DescriptorTypeUid", descriptorType.Id);
            createDescriptorCommand.Parameters.AddWithValue("@NodeUid", nodeUid.GetValue());
            createDescriptorCommand.Parameters.AddWithValue("@RelationshipUid", relationshipUid);

            return createDescriptorCommand;
        }
    }
}