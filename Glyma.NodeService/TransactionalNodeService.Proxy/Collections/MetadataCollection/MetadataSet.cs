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
    public abstract class MetadataSet : IMetadataSet, IMetadataSetManager, IUpdatableMetadataSet
    {
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

        public string Name
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public INode Node
        {
            get;
            set;
        }

        public IRelationship Relationship
        {
            get;
            set;
        }

        public ConnectionType ConnectionType
        {
            get;
            set;
        }

        public Proxy.IMapManager MapManager
        {
            get;
            protected set;
        }

        MetadataCollection IMetadataSetManager.Container
        {
            get;
            set;
        }

        public override bool Equals(object obj)
        {
            IMetadataSet comparisonObj = obj as IMetadataSet;

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

        public int CompareTo(object obj)
        {
            return CompareTo(obj as IMetadataSet);
        }

        public int CompareTo(IMetadataSet other)
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

        public virtual void Update(string name, string value, Proxy.INode node, Proxy.IRelationship relationship, Proxy.ConnectionType connectionType, ref TransactionFramework.TransactionChain chain)
        {
            if (name != null)
            {
                Name = name;
            }

            if (value != null)
            {
                Value = value;
            }

            if (node != null)
            {
                Node = node;
            }

            if (relationship != null)
            {
                Relationship = relationship;
            }

            if (connectionType != null)
            {
                ConnectionType = connectionType;
            }
        }

        public virtual void Delete(ref TransactionFramework.TransactionChain chain)
        {
            IMetadataSetManager metadataSetManager = this;
            metadataSetManager.Container.Remove(this);

            /// TODO: Need to consider whether the following should be done here.
            //MetadataSetFactory.GetInstance(MapManager).Remove(this);
        }

        void IUpdatableMetadataSet.UpdateMetadataSet(ServerObjects.Metadata metadataSet)
        {
            INode newNode = null;
            IRelationship newRelationship = null;
            ConnectionType newConnectionType = null;

            if (metadataSet.NodeUid.HasValue)
            {
                newNode = MapManager.NodeFactory.FindNode(metadataSet.NodeUid.Value);
            }

            if (metadataSet.RelationshipUid.HasValue)
            {
                newRelationship = MapManager.RelationshipFactory.FindRelationship(metadataSet.RelationshipUid.Value);
            }

            if (metadataSet.DescriptorTypeUid.HasValue)
            {
                newConnectionType = MapManager.ConnectionTypes[metadataSet.DescriptorTypeUid.Value];
            }

            if (Node == newNode || Node.Id != newNode.Id)
            {
                Node = newNode;
            }

            if (Relationship == newRelationship || Relationship.Id != newRelationship.Id)
            {
                Relationship = newRelationship;
            }

            if (ConnectionType != newConnectionType)
            {
                ConnectionType = newConnectionType;
            }

            if (Name != metadataSet.MetadataName)
            {
                Name = metadataSet.MetadataName;
            }

            if (Value != metadataSet.MetadataValue)
            {
                Value = metadataSet.MetadataValue;
            }
        }
    }
}
