using System;
using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal
{
    public interface IRelationshipFactory
    {
        void UpgradeFacade(ISoapTransactionLink transactionLink, ServerObjects.Relationship serviceRelationship);
        IRelationship CreateRelationship(ISoapTransactionLink transactionLink, Guid domainId, Guid rootMapId, Dictionary<ConnectionType, INode> nodes, RelationshipType relationshipType, string originalId);
        IRelationship FindRelationship(Guid relationshipId);
        void DeleteRelationship(ServerObjects.Relationship serviceRelationship);
        void DeleteRelationship(IRelationship proxyRelationship);
    }
}
