using System;
using System.Collections.Generic;
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
    public interface IRelationshipQueryable
    {
        IEnumerable<KeyValuePair<ConnectionType, IRelationship>> FindRelationships();
        IEnumerable<IRelationship> FindRelationships(ConnectionType connectionType);
        IEnumerable<KeyValuePair<ConnectionType, IRelationship>> FindRelationships(RelationshipType relationshipType);
        IEnumerable<IRelationship> FindRelationships(ConnectionType connectionType, RelationshipType relationshipType);
    }
}
