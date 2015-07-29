using System;
using System.Collections.Generic;
using ServerObjects = TransactionalNodeService.Proxy.ServerObjects;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace TransactionalNodeService.Proxy
{
    public interface INodeFactory
    {
        void UpgradeFacade(TransactionFramework.ISoapTransactionLink transactionLink, ServerObjects.Node serviceNode);
        Proxy.INode CreateNode(TransactionFramework.ISoapTransactionLink transactionLink, Guid domainId, Guid rootMapId, Proxy.NodeType nodeType, string originalId);
        Proxy.INode FindNode(Guid nodeId);
        void DeleteNode(ServerObjects.Node serviceNode);
        void DeleteNode(Proxy.INode serviceNode);
    }
}
