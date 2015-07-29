using System.Collections.Generic;
using System.Linq;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Collections.RelationshipContainers
{
    public sealed class RelationshipSet : IRelationshipQueryable, IRelationshipManager
    {
        private HashSet<ConnectionSet> _connectionSets = null;

        public RelationshipSet(INode nodeContext)
        {
            NodeContext = nodeContext;
        }

        public INode NodeContext
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
                                                       where relationshipPair.ConnectionType.Equals(connectionType)
                                                       select relationshipPair.Relationship;

            /// TODO: This need to be replaced with a proper and more efficient fix. There are other places this needs to be fix, search for 6218535.
            return relationships.Distinct(new EqualityComparer2());
        }

        public IEnumerable<KeyValuePair<ConnectionType, IRelationship>> FindRelationships(RelationshipType relationshipType)
        {
            IEnumerable<KeyValuePair<ConnectionType, IRelationship>> relationships = from relationshipPair in Connections
                                                                                     where relationshipPair.Relationship.RelationshipType.Equals(relationshipType)
                                                                                     select (new KeyValuePair<ConnectionType, IRelationship>(relationshipPair.ConnectionType, relationshipPair.Relationship));

            /// TODO: This need to be replaced with a proper and more efficient fix. There are other places this needs to be fix, search for 6218535.
            return relationships.Distinct(new EqualityComparer1());
        }

        public IEnumerable<IRelationship> FindRelationships(ConnectionType connectionType, RelationshipType relationshipType)
        {
            var output = new List<IRelationship>();
            foreach (var connection in Connections)
            {
                if (connection.ConnectionType.Equals(connectionType))
                {
                    if (connection.Relationship.RelationshipType.Equals(relationshipType))
                    {
                        if (output.All(q => q.Id != connection.Relationship.Id))
                        {
                            output.Add(connection.Relationship);
                        }
                    }
                } 
            }
            return output;

            //IEnumerable<IRelationship> relationships = from relationshipPair in Connections
            //                                           where relationshipPair.ConnectionType == connectionType && relationshipPair.Relationship.RelationshipType == relationshipType
            //                                           select relationshipPair.Relationship;

            ///// TODO: This need to be replaced with a proper and more efficient fix. There are other places this needs to be fix, search for 6218535.
            //return relationships.Distinct(new EqualityComparer2());
        }
    }
}
