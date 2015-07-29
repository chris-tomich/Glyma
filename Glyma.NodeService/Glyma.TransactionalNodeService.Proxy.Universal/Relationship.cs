using System;
using TransactionalNodeService.Proxy.Universal.Collections;
using TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection;
using TransactionalNodeService.Proxy.Universal.Collections.NodeContainers;
using TransactionalNodeService.Proxy.Universal.EventRegisters;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal
{
    public abstract class Relationship : IAsyncMapObject, IComparable<Relationship>, IRelationship
    {
        protected NodeSet _nodePairs = null;
        protected RelationshipMetadataCollection _metadataCollection = null;

        protected RelationshipSetMetadataEventRegister _relationshipSetMetadataEventRegister = null;

        protected Relationship()
        {
            ClientId = Guid.NewGuid();
        }

        protected Relationship(IMapManager mapManager)
            : this()
        {
            MapManager = mapManager;
        }

        public virtual void ConnectNode(ConnectionType connectionType, INode node, ref Soap.TransactionFramework.TransactionChain chain)
        {
            InProcess.InProcessRelationship inProcessRelationship = this as InProcess.InProcessRelationship;
            ConnectionSet connectionSet;

            if (inProcessRelationship != null)
            {
                connectionSet = ConnectionSetFactory.Instance.GetConnection(node, inProcessRelationship.Facade, connectionType);
            }
            else
            {
                connectionSet = ConnectionSetFactory.Instance.GetConnection(node, this, connectionType);
            }

            INodeManager nodes = Nodes;
            nodes.Load(connectionSet);

            IRelationshipManager relationships = node.Relationships;
            relationships.Load(connectionSet);
        }

        public abstract void Update(RelationshipType relationshipType, ref Soap.TransactionFramework.TransactionChain chain);
        public abstract void Delete(ref Soap.TransactionFramework.TransactionChain chain);

        public RelationshipSetMetadataEventRegister SetMetadataCompleted
        {
            get
            {
                if (_relationshipSetMetadataEventRegister == null)
                {
                    _relationshipSetMetadataEventRegister = new RelationshipSetMetadataEventRegister();
                }

                return _relationshipSetMetadataEventRegister;
            }
        }

        public Guid ClientId
        {
            get;
            set;
        }

        public Guid Id
        {
            get;
            set;
        }

        public Guid DomainId
        {
            get;
            set;
        }

        public Guid? RootMapId
        {
            get;
            set;
        }

        public string OriginalId
        {
            get;
            set;
        }

        public RelationshipType RelationshipType
        {
            get;
            set;
        }

        public LoadState Status
        {
            get;
            set;
        }

        public IMapManager MapManager
        {
            get;
            protected set;
        }

        public NodeSet Nodes
        {
            get
            {
                if (Status == LoadState.None)
                {
                    return null;
                }

                if (_nodePairs == null)
                {
                    _nodePairs = new NodeSet(this);
                }

                return _nodePairs;
            }
            protected set
            {
                _nodePairs = value;
            }
        }

        public virtual RelationshipMetadataCollection Metadata
        {
            get
            {
                if (Status == LoadState.None)
                {
                    return null;
                }

                if (_metadataCollection == null)
                {
                    _metadataCollection = new RelationshipMetadataCollection(this);
                }

                return _metadataCollection;
            }
            protected set
            {
                _metadataCollection = value;
            }
        }

        public override bool Equals(object obj)
        {
            Relationship comparisonObj = obj as Relationship;

            if (comparisonObj != null)
            {
                return comparisonObj.Id.Equals(Id);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public int CompareTo(Relationship other)
        {
            if (other == null)
            {
                return 1;
            }

            int idHashCode = Id.GetHashCode();
            int otherHashCode = other.GetHashCode();

            if (idHashCode > otherHashCode)
            {
                return 1;
            }
            else if (idHashCode == otherHashCode)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
