using System;
using System.Collections.Generic;
using TransactionalNodeService.Proxy.Universal.Collections.MetadataCollection;
using TransactionalNodeService.Proxy.Universal.Types;

namespace TransactionalNodeService.Proxy.Universal.Soap
{
    internal sealed class SoapNode : Node
    {
        public SoapNode(IMapManager mapManager)
            : base(mapManager)
        {
            Status = LoadState.None;
        }

        public SoapNode(InProcess.InProcessNode inProcessNode, ServerObjects.Node serviceNode)
        {
            ServiceNode = serviceNode;
            InProcessNode = inProcessNode;

            Id = serviceNode.NodeUid;
            DomainId = inProcessNode.DomainId;
            RootMapId = inProcessNode.RootMapId;
            OriginalId = inProcessNode.OriginalId;
            NodeType = inProcessNode.NodeType;
            Status = LoadState.Full;

            MapManager = inProcessNode.MapManager;
            Relationships = inProcessNode.Relationships;
            Metadata = inProcessNode.Metadata;
        }

        private ServerObjects.Node ServiceNode
        {
            get;
            set;
        }

        private InProcess.InProcessNode InProcessNode
        {
            get;
            set;
        }

        public void PreInitialiseNode(Guid nodeId)
        {
            ServiceNode = null;

            Id = nodeId;
        }

        public void UpdateNode(ServerObjects.Node serviceNode)
        {
            ServiceNode = serviceNode;

            Id = ServiceNode.NodeUid;
            DomainId = ServiceNode.DomainUid;
            RootMapId = ServiceNode.RootMapUid;
            OriginalId = ServiceNode.NodeOriginalId;
            NodeType = MapManager.NodeTypes[ServiceNode.NodeTypeUid];

            if (Status == LoadState.None)
            {
                Status = LoadState.Partial;
            }
        }

        public bool LoadNode(SoapRelationshipManager relationshipManager)
        {
            switch (Status)
            {
                case LoadState.None:
                    return false;

                case LoadState.Partial:
                    {
                        IRelationshipManager relationships = Relationships;
                        relationships.Clear();

                        break;
                    }

                case LoadState.Full:
                    break;

                default:
                    return false;
            }

            Metadata.Clear();

            foreach (KeyValuePair<ServerObjects.MetadataContext, ServerObjects.Metadata> metadataPair in ServiceNode.Metadata)
            {
                if (metadataPair.Value.RelationshipUid.HasValue && metadataPair.Value.RelationshipUid != Guid.Empty)
                {
                    if (relationshipManager.ProxyRelationships.ContainsKey(metadataPair.Value.RelationshipUid.Value))
                    {
                        IRelationship relationship = relationshipManager.ProxyRelationships[metadataPair.Value.RelationshipUid.Value];
                        Metadata.Load(metadataPair.Value, relationship);
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (metadataPair.Value.NodeUid.HasValue && metadataPair.Value.NodeUid != Guid.Empty)
                {
                    Metadata.Load(metadataPair.Value, null);
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
            if (InProcessNode != null)
            {
                TransactionFramework.TransactionChain chain = new TransactionFramework.TransactionChain();

                InProcessNode.DelayedActions.CreateTransactions(ref chain);

                foreach (IMetadataSet metadataSet in Metadata)
                {
                    FacadeMetadataSet facadeMetadataSet = metadataSet as FacadeMetadataSet;

                    if (facadeMetadataSet != null)
                    {
                        SoapMetadataSet soapMetadataSet = facadeMetadataSet.BaseMetadata as SoapMetadataSet;

                        if (soapMetadataSet != null)
                        {
                            soapMetadataSet.ProcessDelayedActions(ref chain);
                        }
                    }
                }

                MapManager.ExecuteTransaction(chain);
            }
        }

        private TransactionFramework.UpdateNodeTransactionLink LastUpdateNode
        {
            get;
            set;
        }

        private TransactionFramework.UpdateNodeTransactionLink CreateNewUpdateNode(NodeType nodeType)
        {
            TransactionFramework.UpdateNodeTransactionLink updateNode = new TransactionFramework.UpdateNodeTransactionLink();
            updateNode.DomainId = DomainId;
            updateNode.MapManager = MapManager;
            updateNode.Node = this;
            updateNode.NodeType = nodeType;

            return updateNode;
        }

        private void OnTransactionExecuting(object sender, System.EventArgs e)
        {
            LastUpdateNode = null;
        }

        public override void Update(NodeType nodeType, ref TransactionFramework.TransactionChain chain)
        {
            NodeType = nodeType;

            if (LastUpdateNode != null && LastUpdateNode.TransactionStatus == TransactionFramework.ServerStatus.ProcessingClient)
            {
                LastUpdateNode.NodeType = nodeType;
            }
            else
            {
                LastUpdateNode = CreateNewUpdateNode(nodeType);
                chain.AddTransaction(LastUpdateNode);
            }

            chain.TransactionExecuting += OnTransactionExecuting;
        }

        public override void Delete(ref TransactionFramework.TransactionChain chain)
        {
            MapManager.NodeFactory.DeleteNode(this);

            TransactionFramework.DeleteNodeTransactionLink deleteNode = new TransactionFramework.DeleteNodeTransactionLink();
            deleteNode.DomainId = DomainId;
            deleteNode.MapManager = MapManager;
            deleteNode.Node = this;

            chain.AddTransaction(deleteNode);
        }
    }
}
