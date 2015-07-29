using System;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace TransactionalNodeService.Proxy
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

        void Update(NodeType nodeType, ref TransactionFramework.TransactionChain chain);
        void Delete(ref TransactionFramework.TransactionChain chain);
    }
}
