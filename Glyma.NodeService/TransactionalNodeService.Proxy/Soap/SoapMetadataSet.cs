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
using ServerObjects = TransactionalNodeService.Proxy.ServerObjects;

namespace TransactionalNodeService.Soap
{
    internal class SoapMetadataSet : Proxy.MetadataSet
    {
        public SoapMetadataSet(Proxy.IMapManager mapManager)
        {
            MapManager = mapManager;
        }

        public SoapMetadataSet(Proxy.IMapManager mapManager, ServerObjects.Metadata serviceMetadata, Proxy.INode node, Proxy.IRelationship relationship)
        {
            MapManager = mapManager;
            ServiceMetadata = serviceMetadata;

            Id = serviceMetadata.MetadataId;
            DomainId = serviceMetadata.DomainUid;
            RootMapId = serviceMetadata.RootMapUid;
            Name = serviceMetadata.MetadataName;
            Value = serviceMetadata.MetadataValue;

            if (serviceMetadata.NodeUid.HasValue && serviceMetadata.NodeUid == node.Id)
            {
                Node = node;
            }
            else if (serviceMetadata.NodeUid.HasValue && serviceMetadata.NodeUid != node.Id)
            {
                throw new NotSupportedException("The node ID in the service metadata object differs to the provided node.");
            }
            else
            {
                Node = null;
            }

            if (serviceMetadata.RelationshipUid.HasValue && serviceMetadata.RelationshipUid == relationship.Id)
            {
                Relationship = relationship;
            }
            else if (serviceMetadata.RelationshipUid.HasValue && serviceMetadata.RelationshipUid != relationship.Id)
            {
                throw new NotSupportedException("The relationship ID in the service metadata object differs to the provided relationship.");
            }
            else
            {
                Relationship = null;
            }

            ConnectionType = null;

            if (serviceMetadata.DescriptorTypeUid.HasValue && serviceMetadata.DescriptorTypeUid != Guid.Empty)
            {
                if (MapManager.ConnectionTypes.ContainsKey(serviceMetadata.DescriptorTypeUid.Value))
                {
                    ConnectionType = MapManager.ConnectionTypes[serviceMetadata.DescriptorTypeUid.Value];
                }
            }
        }

        public SoapMetadataSet(InProcess.InProcessMetadata inProcessMetadata, ServerObjects.Metadata serviceMetadata)
        {
            MapManager = inProcessMetadata.MapManager;
            ServiceMetadata = serviceMetadata;

            Id = serviceMetadata.MetadataId;
            Name = inProcessMetadata.Name;
            Value = inProcessMetadata.Value;
            Node = inProcessMetadata.Node;
            Relationship = inProcessMetadata.Relationship;
            ConnectionType = inProcessMetadata.ConnectionType;

            InProcessMetadata = inProcessMetadata;
        }

        private ServerObjects.Metadata ServiceMetadata
        {
            get;
            set;
        }

        private InProcess.InProcessMetadata InProcessMetadata
        {
            get;
            set;
        }

        public void ProcessDelayedActions(ref TransactionFramework.TransactionChain chain)
        {
            if (InProcessMetadata != null)
            {
                InProcessMetadata.DelayedActions.CreateTransactions(ref chain);
            }
        }

        public override void Update(string name, string value, Proxy.INode node, Proxy.IRelationship relationship, Proxy.ConnectionType connectionType, ref TransactionFramework.TransactionChain chain)
        {
            base.Update(name, value, node, relationship, connectionType, ref chain);

            TransactionFramework.UpdateMetadataTransactionLink updateMetadataTransaction = new TransactionFramework.UpdateMetadataTransactionLink();

            if (Node != null)
            {
                updateMetadataTransaction.DomainId = Node.DomainId;
            }
            else if (Relationship != null)
            {
                updateMetadataTransaction.DomainId = Relationship.DomainId;
            }

            updateMetadataTransaction.MapManager = MapManager;
            updateMetadataTransaction.Metadata = this;
            updateMetadataTransaction.Name = name;
            updateMetadataTransaction.Value = value;

            chain.AddTransaction(updateMetadataTransaction);
        }

        public override void Delete(ref TransactionFramework.TransactionChain chain)
        {
            base.Delete(ref chain);

            TransactionFramework.DeleteMetadataTransactionLink deleteMetadataTransction = new TransactionFramework.DeleteMetadataTransactionLink();

            if (Node != null)
            {
                deleteMetadataTransction.DomainId = Node.DomainId;
            }
            else if (Relationship != null)
            {
                deleteMetadataTransction.DomainId = Relationship.DomainId;
            }

            deleteMetadataTransction.MapManager = MapManager;
            deleteMetadataTransction.Metadata = this;

            chain.AddTransaction(deleteMetadataTransction);

            /// TODO: Need to consider whether the following should be done here. It was originally done in the base but this really shouldn't happen for an InProcessMetadata as when add transaction
            /// returns (for the InProcessMetadata transaction link) the delete transaction still needs to occur.
            Proxy.MetadataSetFactory.GetInstance(MapManager).Remove(this);
        }
    }
}
