using System;
using TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection;
using TransactionalNodeService.Proxy.Universal.Collections.NodeContainers;
using TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal
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

        void ConnectNode(ConnectionType connectionType, INode node, ref TransactionChain chain);

        void Update(RelationshipType relationshipType, ref TransactionChain chain);
        void Delete(ref TransactionChain chain);
    }
}
