using System;
using TransactionalNodeService.Proxy.Universal;
using TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection;
using TransactionalNodeService.Proxy.Universal.Collections.RelationshipContainers;
using TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework;
using TransactionalNodeService.Proxy.Universal.Types;

namespace Glyma.UtilityService.Export.IBIS.Compendium.Model.Glyma
{
    public class ReferenceNode : INode
    {
        public Guid ClientId { get; set; }
        public Guid Id { get; set; }
        public Guid DomainId { get; set; }
        public Guid? RootMapId { get; set; }
        public string OriginalId { get; set; }
        public LoadState Status { get; set; }
        public NodeType NodeType { get; set; }
        public IMapManager MapManager { get; private set; }
        public RelationshipSet Relationships { get; private set; }
        public NodeMetadataCollection Metadata { get; private set; }


        public ReferenceNode(INode original)
        {
            MapManager = original.MapManager;
            NodeType = MapManager.NodeTypes["CompendiumReferenceNode"];
            Status = LoadState.Full;
            DomainId = original.DomainId;
            Id = Guid.NewGuid();
        }

        public void Update(NodeType nodeType, ref TransactionChain chain)
        {
            
        }

        public void Delete(ref TransactionChain chain)
        {
            
        }
    }
}
