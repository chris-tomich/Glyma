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
using Service = TransactionalNodeService.Service;
using TransactionalNodeService;
using System.Collections.Generic;

namespace TransactionalNodeService.Proxy
{
    public abstract class Node : IAsyncMapObject, IComparable<Node>, INode
    {
        protected RelationshipSet _relationshipCollection = null;
        protected NodeMetadataCollection _metadataCollection = null;

        protected NodeSetMetadataEventRegister _nodeSetMetadataEventRegister = null;

        protected Node()
        {
        }

        protected Node(IMapManager mapManager)
            : this()
        {
            MapManager = mapManager;
            ClientId = Guid.NewGuid();
        }

        public abstract void Update(NodeType nodeType, ref Soap.TransactionFramework.TransactionChain chain);
        public abstract void Delete(ref Soap.TransactionFramework.TransactionChain chain);

        public NodeSetMetadataEventRegister SetMetadataCompleted
        {
            get
            {
                if (_nodeSetMetadataEventRegister == null)
                {
                    _nodeSetMetadataEventRegister = new NodeSetMetadataEventRegister();
                }

                return _nodeSetMetadataEventRegister;
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

        public NodeType NodeType
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

        public RelationshipSet Relationships
        {
            get
            {
                if (Status == LoadState.None)
                {
                    return null;
                }

                if (_relationshipCollection == null)
                {
                    _relationshipCollection = new RelationshipSet(this);
                }

                return _relationshipCollection;
            }
            protected set
            {
                _relationshipCollection = value;
            }
        }

        public virtual NodeMetadataCollection Metadata
        {
            get
            {
                if (Status == LoadState.None)
                {
                    return null;
                }

                if (_metadataCollection == null)
                {
                    _metadataCollection = new NodeMetadataCollection(this, MapManager);
                }

                return _metadataCollection;
            }
            protected set
            {
                _metadataCollection = value;
            }
        }

        protected void OnLoadNeighboursCompleted()
        {
        }

        public void LoadNeighboursAsync()
        {
        }

        public override bool Equals(object obj)
        {
            Node comparisonObj = obj as Node;

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

        public int CompareTo(Node other)
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
