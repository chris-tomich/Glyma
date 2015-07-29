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
using Service = TransactionalNodeService.Service;
using Proxy = TransactionalNodeService.Proxy;
using System.Collections.Generic;

namespace TransactionalNodeService.Soap
{
    internal sealed class SoapRelationship : Proxy.Relationship
    {
        public SoapRelationship(Proxy.IMapManager mapManager)
            : base(mapManager)
        {
            Status = Proxy.LoadState.None;
        }

        public SoapRelationship(InProcess.InProcessRelationship inProcessRelationship, ServerObjects.Relationship serviceRelationship)
        {
            ServiceRelationship = serviceRelationship;
            InProcessRelationship = inProcessRelationship;

            Id = serviceRelationship.RelationshipUid;
            DomainId = inProcessRelationship.DomainId;
            RootMapId = inProcessRelationship.RootMapId;
            OriginalId = inProcessRelationship.OriginalId;
            RelationshipType = inProcessRelationship.RelationshipType;
            Status = Proxy.LoadState.Full;

            MapManager = inProcessRelationship.MapManager;
            Nodes = inProcessRelationship.Nodes;
            Metadata = inProcessRelationship.Metadata;
        }

        private ServerObjects.Relationship ServiceRelationship
        {
            get;
            set;
        }

        private InProcess.InProcessRelationship InProcessRelationship
        {
            get;
            set;
        }

        public void UpdateRelationship(ServerObjects.Relationship serviceRelationship)
        {
            ServiceRelationship = serviceRelationship;

            Id = ServiceRelationship.RelationshipUid;
            DomainId = ServiceRelationship.DomainUid;
            OriginalId = ServiceRelationship.RelationshipOriginalId;
            RelationshipType = MapManager.RelationshipTypes[ServiceRelationship.RelationshipTypeUid];

            if (Status == Proxy.LoadState.None)
            {
                Status = Proxy.LoadState.Partial;
            }
        }

        public bool LoadRelationship(SoapNodeManager nodeManager)
        {
            Metadata.Clear();
            Proxy.INodeManager nodeSetManager = Nodes;
            nodeSetManager.Clear();

            foreach (KeyValuePair<ServerObjects.MetadataContext, ServerObjects.Metadata> metadataPair in ServiceRelationship.Metadata)
            {
                if (metadataPair.Value.NodeUid.HasValue)
                {
                    Proxy.INode node = nodeManager.FindNode(metadataPair.Value.NodeUid.Value);
                    Metadata.Load(metadataPair.Value, node);
                }
                else if (metadataPair.Value.RelationshipUid.HasValue)
                {
                    Metadata.Load(metadataPair.Value, null);
                }
            }

            foreach (KeyValuePair<Guid, Guid> relationship in ServiceRelationship.Nodes)
            {
                Guid descriptorTypeId = relationship.Key;
                Guid nodeId = relationship.Value;

                // TODO: There needs to be a proper check for the ConnectionType that will perform an appropriate operation if none exists.
                if (MapManager.ConnectionTypes.ContainsKey(descriptorTypeId))
                {
                    Proxy.INode node;
                    Proxy.ConnectionType connectionType = MapManager.ConnectionTypes[descriptorTypeId];

                    node = nodeManager.FindNode(nodeId);

                    Proxy.ConnectionSet connectionSet = Proxy.ConnectionSetFactory.Instance.GetConnection(node, this, connectionType);

                    if (node.Status != Proxy.LoadState.None)
                    {
                        Proxy.IRelationshipManager relationshipSetManager = node.Relationships;

                        relationshipSetManager.Load(connectionSet);
                    }

                    nodeSetManager.Load(connectionSet);
                }
            }

            Status = Proxy.LoadState.Full;

            ProcessDelayedActions();

            return true;
        }

        /// <summary>
        /// TODO: I don't like how this is public.
        /// </summary>
        public void ProcessDelayedActions()
        {
            if (InProcessRelationship != null)
            {
                TransactionFramework.TransactionChain chain = new TransactionFramework.TransactionChain();

                InProcessRelationship.DelayedActions.CreateTransactions(ref chain);

                MapManager.ExecuteTransaction(chain);
            }
        }

        private TransactionFramework.UpdateRelationshipTransactionLink LastUpdateRelationship
        {
            get;
            set;
        }

        private TransactionFramework.UpdateRelationshipTransactionLink CreateNewUpdateRelationship()
        {
            TransactionFramework.UpdateRelationshipTransactionLink updateRelationship = new TransactionFramework.UpdateRelationshipTransactionLink();
            updateRelationship.DomainId = DomainId;
            updateRelationship.MapManager = MapManager;
            updateRelationship.Relationship = this;
            updateRelationship.RelationshipType = null;

            return updateRelationship;
        }

        private void OnTransactionExecuting(object sender, EventArgs e)
        {
            LastUpdateRelationship = null;
        }

        public override void ConnectNode(Proxy.ConnectionType connectionType, Proxy.INode node, ref TransactionFramework.TransactionChain chain)
        {
            base.ConnectNode(connectionType, node, ref chain);

            if (LastUpdateRelationship != null && LastUpdateRelationship.TransactionStatus == TransactionFramework.ServerStatus.ProcessingClient)
            {
                LastUpdateRelationship.AddNode(connectionType, node);
            }
            else
            {
                LastUpdateRelationship = CreateNewUpdateRelationship();
                LastUpdateRelationship.AddNode(connectionType, node);
                chain.AddTransaction(LastUpdateRelationship);
            }

            chain.TransactionExecuting += OnTransactionExecuting;
        }

        public override void Update(Proxy.RelationshipType relationshipType, ref TransactionFramework.TransactionChain chain)
        {
            RelationshipType = relationshipType;

            if (LastUpdateRelationship != null && LastUpdateRelationship.TransactionStatus == TransactionFramework.ServerStatus.ProcessingClient)
            {
                LastUpdateRelationship.RelationshipType = relationshipType;
            }
            else
            {
                LastUpdateRelationship = CreateNewUpdateRelationship();
                LastUpdateRelationship.RelationshipType = relationshipType;
                chain.AddTransaction(LastUpdateRelationship);
            }

            chain.TransactionExecuting += OnTransactionExecuting;
        }

        public override void Delete(ref TransactionFramework.TransactionChain chain)
        {
            MapManager.RelationshipFactory.DeleteRelationship(this);

            TransactionFramework.DeleteRelationshipTransactionLink deleteRelationship = new TransactionFramework.DeleteRelationshipTransactionLink();
            deleteRelationship.DomainId = DomainId;
            deleteRelationship.MapManager = MapManager;
            deleteRelationship.Relationship = this;

            chain.AddTransaction(deleteRelationship);
        }
    }
}
