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
using Service = TransactionalNodeService.Service;
using System.Collections.Generic;
using System.Linq;

namespace TransactionalNodeService.Proxy
{
    public sealed class RelationshipSet : IRelationshipQueryable, IRelationshipManager
    {
        private HashSet<ConnectionSet> _connectionSets = null;

        public RelationshipSet(Proxy.INode nodeContext)
        {
            NodeContext = nodeContext;
        }

        public Proxy.INode NodeContext
        {
            get;
            set;
        }

        private HashSet<ConnectionSet> Connections
        {
            get
            {
                if (_connectionSets == null)
                {
                    _connectionSets = new HashSet<ConnectionSet>();
                }

                return _connectionSets;
            }
        }

        void IRelationshipManager.Load(ConnectionSet connectionSet)
        {
            Connections.Add(connectionSet);
        }

        void IRelationshipManager.Clear()
        {
            Connections.Clear();
        }

        void IRelationshipManager.UnionWith(RelationshipSet relationships)
        {
            if (this != relationships)
            {
                Connections.UnionWith(relationships.Connections);
            }
        }

        void IRelationshipManager.Remove(IRelationship relationship)
        {
            IEnumerable<ConnectionSet> relationshipTuples = from relationshipPair in Connections
                                                            where relationshipPair.Relationship == relationship
                                                            select relationshipPair;

            List<ConnectionSet> tuplesToDelete = new List<ConnectionSet>();

            foreach (ConnectionSet relationshipTuple in relationshipTuples)
            {
                tuplesToDelete.Add(relationshipTuple);
            }

            foreach (ConnectionSet relationshipTuple in tuplesToDelete)
            {
                Connections.Remove(relationshipTuple);
            }
        }

        public IEnumerable<KeyValuePair<ConnectionType, IRelationship>> FindRelationships()
        {
            IEnumerable<KeyValuePair<ConnectionType, IRelationship>> relationships = from relationshipPair in Connections
                                                                                     select (new KeyValuePair<ConnectionType, IRelationship>(relationshipPair.ConnectionType, relationshipPair.Relationship));

            /// TODO: This need to be replaced with a proper and more efficient fix. There are other places this needs to be fix, search for 6218535.
            return relationships.Distinct(new EqualityComparer1());
        }

        public IEnumerable<IRelationship> FindRelationships(ConnectionType connectionType)
        {
            IEnumerable<IRelationship> relationships = from relationshipPair in Connections
                                                       where relationshipPair.ConnectionType == connectionType
                                                       select relationshipPair.Relationship;

            /// TODO: This need to be replaced with a proper and more efficient fix. There are other places this needs to be fix, search for 6218535.
            return relationships.Distinct(new EqualityComparer2());
        }

        public IEnumerable<KeyValuePair<ConnectionType, IRelationship>> FindRelationships(RelationshipType relationshipType)
        {
            IEnumerable<KeyValuePair<ConnectionType, IRelationship>> relationships = from relationshipPair in Connections
                                                                                     where relationshipPair.Relationship.RelationshipType == relationshipType
                                                                                     select (new KeyValuePair<ConnectionType, IRelationship>(relationshipPair.ConnectionType, relationshipPair.Relationship));

            /// TODO: This need to be replaced with a proper and more efficient fix. There are other places this needs to be fix, search for 6218535.
            return relationships.Distinct(new EqualityComparer1());
        }

        public IEnumerable<IRelationship> FindRelationships(ConnectionType connectionType, RelationshipType relationshipType)
        {
            IEnumerable<IRelationship> relationships = from relationshipPair in Connections
                                                       where relationshipPair.ConnectionType == connectionType && relationshipPair.Relationship.RelationshipType == relationshipType
                                                       select relationshipPair.Relationship;

            /// TODO: This need to be replaced with a proper and more efficient fix. There are other places this needs to be fix, search for 6218535.
            return relationships.Distinct(new EqualityComparer2());
        }
    }
}
