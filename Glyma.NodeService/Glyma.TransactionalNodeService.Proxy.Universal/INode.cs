using System;
using TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection;
using TransactionalNodeService.Proxy.Universal.Collections.RelationshipContainers;
using TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal
{
    public interface INode
    {
        Guid ClientId { get; set; }
        Guid Id { get; set; }
        Guid DomainId { get; set; }
        Guid? RootMapId { get; set; }
        string OriginalId { get; set; }
        LoadState Status { get; set; }
        NodeType NodeType { get; set; }

        IMapManager MapManager { get; }
        RelationshipSet Relationships { get; }
        NodeMetadataCollection Metadata { get; }

        void Update(NodeType nodeType, ref TransactionChain chain);
        void Delete(ref TransactionChain chain);
    }
}
