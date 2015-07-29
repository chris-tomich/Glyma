using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using Proxy = TransactionalNodeService.Proxy;
using Service = TransactionalNodeService.Service;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace TransactionalNodeService.Proxy
{
    public interface IMapManager
    {
        ITypeManager<Service.NT, Proxy.NodeType> NodeTypes { get; }
        ITypeManager<Service.RT, Proxy.RelationshipType> RelationshipTypes { get; }
        ITypeManager<Service.DT, Proxy.ConnectionType> ConnectionTypes { get; }
        ITypeManager<Service.MetadataType, Proxy.MetadataType> MetadataTypes { get; }

        INodeFactory NodeFactory { get; }
        IRelationshipFactory RelationshipFactory { get; }
        /// <summary>
        /// TODO: Need to refactor this so that it is referencing a generic interface rather than the specific SOAP version.
        /// </summary>
        TransactionalNodeService.Soap.SoapToServerObjectConverter ServerObjectConverter { get; }

        event EventHandler<Proxy.InitialiseMapManagerEventArgs> InitialiseMapManagerCompleted;
        event EventHandler<Proxy.MapManagerActivityEventArgs> MapManagerActivityStatusUpdated;

        void InitialiseMapManagerAsync();

        Proxy.CreateDomainEventRegister CreateDomainCompleted { get; }
        Proxy.CreateRootMapEventRegister CreateRootMapCompleted { get; }

        Proxy.DeleteEventRegister DeleteDomainCompleted { get; }
        Proxy.DeleteEventRegister DeleteRootMapCompleted { get; }

        void CreateDomain(string domainName);
        void CreateRootMap(Guid domainId, string mapName, Proxy.NodeType nodeType, string originalId);

        void DeleteDomain(Guid domainId);
        void DeleteRootMap(Guid domainId, Guid rootMapId);

        Proxy.QueryDomainsEventRegister QueryDomainsCompleted { get; }
        Proxy.QueryMapByNodeEventRegister QueryMapByNodeCompleted { get; }
        Proxy.QueryMapByIdEventRegister QueryMapByIdCompleted { get; }
        Proxy.QueryMapByDomainEventRegister QueryMapByDomainCompleted { get; }

        void QueryDomainsAsync();
        void QueryMapByDomainAsync(Guid domainId);
        void QueryMapByIdAsync(Guid domainId, Guid nodeId);
        void QueryMapByIdAsync(Guid domainId, Guid nodeId, int depth);
        void QueryMapByNodeAsync(Proxy.INode node);
        void QueryMapByNodeAsync(Proxy.INode node, int depth);

        void ForceTransactionReExecution();
        void ExecuteTransaction(TransactionFramework.TransactionChain transactionChain);
        //void ExecuteTransactions(Queue<TransactionFramework.TransactionChain> chains);

        event EventHandler<Proxy.MetadataChangedEventArgs> AssignMetadataCompleted;

        INode CreateNode(Guid domainId, Guid rootMapId, NodeType nodeType, string originalId, ref TransactionFramework.TransactionChain chain);
        IRelationship CreateRelationship(Guid domainId, Guid rootMapId, RelationshipType relationshipType, string originalId, ref TransactionFramework.TransactionChain chain);
    }
}
