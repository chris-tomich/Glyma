using System;
using TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection
{
    public interface IMetadataSet : IComparable, IComparable<IMetadataSet>
    {
        Guid Id { get; }
        Guid DomainId { get; }
        Guid? RootMapId { get; }
        string Name { get; }
        string Value { get; }
        INode Node { get; }
        IRelationship Relationship { get; }
        ConnectionType ConnectionType { get; }

        void Update(string name, string value, INode node, IRelationship relationship, ConnectionType connectionType, ref TransactionChain chain);
        void Delete(ref TransactionChain chain);
    }
}
