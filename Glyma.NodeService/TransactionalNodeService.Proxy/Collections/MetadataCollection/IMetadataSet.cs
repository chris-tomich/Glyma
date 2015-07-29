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
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace TransactionalNodeService.Proxy
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

        void Update(string name, string value, Proxy.INode node, Proxy.IRelationship relationship, Proxy.ConnectionType connectionType, ref TransactionFramework.TransactionChain chain);
        void Delete(ref TransactionFramework.TransactionChain chain);
    }
}
