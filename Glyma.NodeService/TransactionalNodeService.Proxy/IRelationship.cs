using System;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace TransactionalNodeService.Proxy
{
    public interface IRelationship
    {
        Guid ClientId { get; set; }
        Guid Id { get; set; }
        Guid DomainId { get; set; }
        Guid? RootMapId { get; set; }
        string OriginalId { get; set; }
        LoadState Status { get; set; }
        RelationshipType RelationshipType { get; set; }

        IMapManager MapManager { get; }
        NodeSet Nodes { get; }
        RelationshipMetadataCollection Metadata { get; }

        void ConnectNode(ConnectionType connectionType, INode node, ref TransactionFramework.TransactionChain chain);

        void Update(RelationshipType relationshipType, ref TransactionFramework.TransactionChain chain);
        void Delete(ref TransactionFramework.TransactionChain chain);
    }
}
