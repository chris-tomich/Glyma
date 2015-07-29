using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransactionalNodeService.Common.Model;
using System.Data.SqlClient;

namespace TransactionalNodeService.Common
{
    public class NodeService
    {
        private readonly Guid DomainNodeTypeId = new Guid("263754C2-2F31-4D21-B9C4-6509E00A5E94");

        public NodeService()
        {
            Connection = new SqlConnection("Data Source=chris-ultrabook;Initial Catalog=MappingToolDatabase;Integrated Security=True");
        }

        public Domain DomainCreate()
        {
            Domain newDomain = new Domain();

            Guid newDomainId;
            Guid newDomainNodeId;

            SqlCommand createDomainCommand = CreateDomainCommand(out newDomainId, out newDomainNodeId);
            createDomainCommand.ExecuteNonQuery();

            newDomain.DomainUid = newDomainId;
            newDomain.DomainOriginalId = newDomainId.ToString();

            return newDomain;
        }

        public Node DomainGetRootNode(Domain domain)
        {
            Node domainNode = new Node();

            SqlCommand selectDomainNodeCommand = SelectDomainNodeCommand(domain.DomainUid);
            SqlDataReader reader = selectDomainNodeCommand.ExecuteReader();

            if (reader.Read())
            {
                domainNode.NodeUid = (Guid)reader["NodeUid"];
                domainNode.NodeOriginalId = (string)reader["NodeOriginalId"];
                domainNode.NodeTypeUid = (Guid)reader["NodeTypeUid"];
                domainNode.DomainUid = (Guid)reader["DomainUid"];
            }

            return domainNode;
        }

        public Node NodeCreate(Domain newDomain, Guid nodeTypeId)
        {
            Guid newNodeId;
            Node newNode = new Node();

            SqlCommand createNodeCommand = CreateNodeCommand(out newNodeId, nodeTypeUid: nodeTypeId, domainUid: newDomain.DomainUid);
            createNodeCommand.Parameters.AddWithValue("@NodeOriginalId", newNodeId.ToString());

            createNodeCommand.ExecuteNonQuery();

            newNode.NodeUid = newNodeId;
            newNode.NodeOriginalId = newNodeId.ToString();
            newNode.NodeTypeUid = nodeTypeId;
            newNode.DomainUid = newDomain.DomainUid;

            return newNode;
        }

        public Relationship RelationshipCreate(Domain newDomain, Guid relationshipTypeId, IDictionary<Guid, Guid> nodes)
        {
            Guid newRelationshipId;
            Relationship newRelationship = new Relationship();

            SqlCommand createRelationshipCommand = CreateRelationshipCommand(out newRelationshipId, newDomain.DomainUid, relationshipTypeId, nodes);

            createRelationshipCommand.ExecuteNonQuery();

            newRelationship.RelationshipUid = Guid.Empty;
            newRelationship.RelationshipOriginalId = string.Empty;
            newRelationship.RelationshipTypeUid = relationshipTypeId;
            newRelationship.DomainUid = newDomain.DomainUid;

            return newRelationship;
        }

        protected SqlCommand CreateRelationshipCommand(out Guid newRelationshipId, Guid domainId, Guid relationshipTypeId, IDictionary<Guid, Guid> nodes, string relationshipOriginalId = default(string))
        {
            string createRelationship = "INSERT INTO [Relationships] ([RelationshipUid], [RelationshipOriginalId], [RelationshipTypeUid], [DomainUid]) VALUES (@RelationshipUid, @RelationshipOriginalId, @RelationshipTypeUid, @DomainUid);";
            string createDescriptor = "INSERT INTO [Descriptors] ([DescriptorUid], [DescriptorTypeUid], [NodeUid], [RelationshipUid]) VALUES (@DescriptorUid_{0}, @DescriptorTypeUid_{0}, @NodeUid_{0}, @RelationshipUid);";

            SqlCommand createRelationshipCommand = new SqlCommand();
            createRelationshipCommand.Connection = Connection;

            newRelationshipId = Guid.NewGuid();

            int count = -1;
            string createDescriptors = string.Empty;

            foreach (KeyValuePair<Guid, Guid> nodeConnection in nodes)
            {
                count++;

                createDescriptors += string.Format(createDescriptor, count);

                Guid newDescriptorId = Guid.NewGuid();

                createRelationshipCommand.Parameters.AddWithValue("@DescriptorUid_" + count, newDescriptorId);
                createRelationshipCommand.Parameters.AddWithValue("@DescriptorTypeUid_" + count, nodeConnection.Key);
                createRelationshipCommand.Parameters.AddWithValue("@NodeUid_" + count, nodeConnection.Value);
            }

            createRelationshipCommand.CommandText = createRelationship + createDescriptors;

            createRelationshipCommand.Parameters.AddWithValue("@RelationshipUid", newRelationshipId);

            if (relationshipOriginalId != default(string))
            {
                createRelationshipCommand.Parameters.AddWithValue("@RelationshipOriginalId", relationshipOriginalId);
            }
            else
            {
                createRelationshipCommand.Parameters.AddWithValue("@RelationshipOriginalId", string.Empty);
            }

            createRelationshipCommand.Parameters.AddWithValue("@RelationshipTypeUid", relationshipTypeId);
            createRelationshipCommand.Parameters.AddWithValue("@DomainUid", domainId);

            return createRelationshipCommand;
        }

        public SqlConnection Connection
        {
            get;
            set;
        }

        protected SqlCommand CreateDomainCommand(out Guid newDomainId, out Guid newDomainNodeId, string domainOriginalId = default(string))
        {
            SqlCommand createDomainCommand = new SqlCommand();
            createDomainCommand.CommandText = "INSERT INTO [Domains] ([DomainUid], [DomainOriginalId]) VALUES (@DomainUid, @DomainOriginalId); INSERT INTO [Nodes] ([NodeUid], [NodeOriginalId], [NodeTypeUid], [DomainUid]) VALUES (@NodeUid, @DomainOriginalId, @DomainNodeTypeUid, @DomainUid)";
            createDomainCommand.Connection = Connection;

            newDomainId = Guid.NewGuid();
            newDomainNodeId = Guid.NewGuid();

            createDomainCommand.Parameters.AddWithValue("@DomainUid", newDomainId);
            createDomainCommand.Parameters.AddWithValue("@NodeUid", newDomainNodeId);
            createDomainCommand.Parameters.AddWithValue("@DomainNodeTypeUid", DomainNodeTypeId);

            if (domainOriginalId == default(string))
            {
                createDomainCommand.Parameters.AddWithValue("@DomainOriginalId", newDomainId.ToString());
            }
            else
            {
                createDomainCommand.Parameters.AddWithValue("@DomainOriginalId", domainOriginalId);
            }

            return createDomainCommand;
        }

        protected SqlCommand CreateNodeCommand(out Guid newNodeId, string nodeOriginalId = default(string), Guid nodeTypeUid = default(Guid), Guid domainUid = default(Guid))
        {
            SqlCommand createNodeCommand = new SqlCommand();
            createNodeCommand.CommandText = "INSERT INTO [Nodes] ([NodeUid], [NodeOriginalId], [NodeTypeUid], [DomainUid]) VALUES (@NodeUid, @NodeOriginalId, @NodeTypeUid, @DomainUid)";
            createNodeCommand.Connection = Connection;

            newNodeId = Guid.NewGuid();

            createNodeCommand.Parameters.AddWithValue("@NodeUid", newNodeId);

            if (nodeOriginalId != default(string))
            {
                createNodeCommand.Parameters.AddWithValue("@NodeOriginalId", nodeOriginalId);
            }

            if (nodeTypeUid != default(Guid))
            {
                createNodeCommand.Parameters.AddWithValue("@NodeTypeUid", nodeTypeUid);
            }

            if (domainUid != default(Guid))
            {
                createNodeCommand.Parameters.AddWithValue("@DomainUid", domainUid);
            }

            return createNodeCommand;
        }

        protected SqlCommand SelectDomainNodeCommand(Guid domainId)
        {
            SqlCommand selectDomainNodeCommand = new SqlCommand();
            selectDomainNodeCommand.CommandText = "SELECT * FROM [Nodes] WHERE [NodeTypeUid] = @DomainNodeTypeUid AND [DomainUid] = @DomainUid";
            selectDomainNodeCommand.Connection = Connection;

            selectDomainNodeCommand.Parameters.AddWithValue("@DomainNodeTypeUid", DomainNodeTypeId);
            selectDomainNodeCommand.Parameters.AddWithValue("@DomainUid", domainId);

            return selectDomainNodeCommand;
        }
    }
}