using System;
using TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal
{
    public interface INodeFactory
    {
        void UpgradeFacade(ISoapTransactionLink transactionLink, ServerObjects.Node serviceNode);
        INode CreateNode(ISoapTransactionLink transactionLink, Guid domainId, Guid rootMapId, NodeType nodeType, string originalId);
        INode FindNode(Guid nodeId);
        void DeleteNode(ServerObjects.Node serviceNode);
        void DeleteNode(INode serviceNode);
    }
}
