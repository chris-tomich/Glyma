using System;
using System.Collections.Generic;
using ServerObjects = TransactionalNodeService.Proxy.ServerObjects;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace TransactionalNodeService.Proxy
{
    public interface IRelationshipFactory
    {
        void UpgradeFacade(TransactionFramework.ISoapTransactionLink transactionLink, ServerObjects.Relationship serviceRelationship);
        Proxy.IRelationship CreateRelationship(TransactionFramework.ISoapTransactionLink transactionLink, Guid domainId, Guid rootMapId, Dictionary<Proxy.ConnectionType, Proxy.INode> nodes, Proxy.RelationshipType relationshipType, string originalId);
        Proxy.IRelationship FindRelationship(Guid relationshipId);
        void DeleteRelationship(ServerObjects.Relationship serviceRelationship);
        void DeleteRelationship(Proxy.IRelationship proxyRelationship);
    }
}
