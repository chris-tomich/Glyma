using System;
using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal.Collections;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Soap
{
    internal sealed class SoapRelationship : Relationship
    {
        public SoapRelationship(IMapManager mapManager)
            : base(mapManager)
        {
            Status = LoadState.None;
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
            Status = LoadState.Full;

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

            if (Status == LoadState.None)
            {
                Status = LoadState.Partial;
            }
        }

        public bool LoadRelationship(SoapNodeManager nodeManager)
        {
            Metadata.Clear();
            INodeManager nodeSetManager = Nodes;
            nodeSetManager.Clear();

            foreach (KeyValuePair<ServerObjects.MetadataContext, ServerObjects.Metadata> metadataPair in ServiceRelationship.Metadata)
            {
                if (metadataPair.Value.NodeUid.HasValue)
                {
                    INode node = nodeManager.FindNode(metadataPair.Value.NodeUid.Value);
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
                    INode node;
                    ConnectionType connectionType = MapManager.ConnectionTypes[descriptorTypeId];

                    node = nodeManager.FindNode(nodeId);

                    ConnectionSet connectionSet = ConnectionSetFactory.Instance.GetConnection(node, this, connectionType);

                    if (node.Status != LoadState.None)
                    {
                        IRelationshipManager relationshipSetManager = node.Relationships;

                        relationshipSetManager.Load(connectionSet);
                    }

                    nodeSetManager.Load(connectionSet);
                }
            }

            Status = LoadState.Full;

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

        private void OnTransactionExecuting(object sender, System.EventArgs e)
        {
            LastUpdateRelationship = null;
        }

        public override void ConnectNode(ConnectionType connectionType, INode node, ref TransactionFramework.TransactionChain chain)
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

        public override void Update(RelationshipType relationshipType, ref TransactionFramework.TransactionChain chain)
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
