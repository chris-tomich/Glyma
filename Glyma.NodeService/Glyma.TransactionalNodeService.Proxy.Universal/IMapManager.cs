using System;
using TransactionalNodeService.Proxy.Universal.EventArgs;
using TransactionalNodeService.Proxy.Universal.EventRegisters;
using TransactionalNodeService.Proxy.Universal.Soap;
using TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework;
using TransactionalNodeService.Proxy.Universal.TypeManagers;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal
{
    public interface IMapManager
    {
        ITypeManager<Service.NT, NodeType> NodeTypes { get; }
        ITypeManager<Service.RT, RelationshipType> RelationshipTypes { get; }
        ITypeManager<Service.DT, ConnectionType> ConnectionTypes { get; }
        ITypeManager<Service.MetadataType, MetadataType> MetadataTypes { get; }

        INodeFactory NodeFactory { get; }
        IRelationshipFactory RelationshipFactory { get; }
        /// <summary>
        /// TODO: Need to refactor this so that it is referencing a generic interface rather than the specific SOAP version.
        /// </summary>
        SoapToServerObjectConverter ServerObjectConverter { get; }

        event EventHandler<InitialiseMapManagerEventArgs> InitialiseMapManagerCompleted;
        event EventHandler<MapManagerActivityEventArgs> MapManagerActivityStatusUpdated;

        void InitialiseMapManagerAsync();

        CreateDomainEventRegister CreateDomainCompleted { get; }
        CreateRootMapEventRegister CreateRootMapCompleted { get; }

        DeleteEventRegister DeleteDomainCompleted { get; }
        DeleteEventRegister DeleteRootMapCompleted { get; }

        void CreateDomain(string domainName);
        void CreateRootMap(Guid domainId, string mapName, NodeType nodeType, string originalId);

        void DeleteDomain(Guid domainId);
        void DeleteRootMap(Guid domainId, Guid rootMapId);

        QueryDomainsEventRegister QueryDomainsCompleted { get; }
        QueryMapByNodeEventRegister QueryMapByNodeCompleted { get; }
        QueryMapByIdEventRegister QueryMapByIdCompleted { get; }
        QueryMapByDomainEventRegister QueryMapByDomainCompleted { get; }

        void QueryDomainsAsync();
        void QueryMapByDomainAsync(Guid domainId);
        void QueryMapByIdAsync(Guid domainId, Guid nodeId);
        void QueryMapByIdAsync(Guid domainId, Guid nodeId, int depth);
        void QueryMapByNodeAsync(INode node);
        void QueryMapByNodeAsync(INode node, int depth);

        void ForceTransactionReExecution();
        void ExecuteTransaction(TransactionChain transactionChain);
        //void ExecuteTransactions(Queue<TransactionFramework.TransactionChain> chains);

        event EventHandler<MetadataChangedEventArgs> AssignMetadataCompleted;

        INode CreateNode(Guid domainId, Guid rootMapId, NodeType nodeType, string originalId, ref TransactionChain chain);
        IRelationship CreateRelationship(Guid domainId, Guid rootMapId, RelationshipType relationshipType, string originalId, ref TransactionChain chain);
    }
}
