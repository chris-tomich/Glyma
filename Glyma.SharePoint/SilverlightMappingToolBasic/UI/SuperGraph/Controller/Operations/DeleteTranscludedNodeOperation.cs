using System;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using TransactionalNodeService.Service;
using Proxy = TransactionalNodeService.Proxy;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;
using System.Collections.Generic;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class DeleteTranscludedNodeOperation : CommonOperationBase, ISuperGraphOperation, IBatchOperation
    {
        public DeleteTranscludedNodeOperation(Proxy.IMapManager mapManager)
            : base(mapManager)
        {
        }

        internal IBatchOperation OperationBuilder
        {
            get
            {
                return this;
            }
        }

        public Node ViewModeNode
        {
            get; 
            set;
        }


        public Proxy.INode Context
        {
            get;
            set;
        }

        public Proxy.INode Node
        {
            get;
            set;
        }

        TransactionFramework.InProcessTransactionResponse IBatchOperation.BuildTransactionChain(ref TransactionFramework.TransactionChain chain)
        {
            /// TODO: The following really needs to be refactored.
            var response = new TransactionFramework.InProcessTransactionResponse();

            var mapRelationships = Node.Relationships.FindRelationships(FromConnectionType, MapContainerRelationshipType);

            var mapsToRemove = new List<Proxy.IRelationship>();

            foreach (Proxy.IRelationship relationship in mapRelationships)
            {
                var maps = relationship.Nodes.FindNodes(ToConnectionType, MapNodeType);

                if (maps != null)
                {
                    foreach (Proxy.INode mapNode in maps)
                    {
                        if (mapNode.Id == Context.Id)
                        {
                            mapsToRemove.Add(relationship);
                            break;
                        }
                    }
                }
            }

            foreach (Proxy.IRelationship relationship in mapsToRemove)
            {
                relationship.Delete(ref chain);
                /// TODO: Need to move the following operation back into the relationship class.
                Proxy.IRelationshipManager relationships = Node.Relationships;
                relationships.Remove(relationship);
                response.Relationships.Add(relationship);
            }

            IEnumerable<KeyValuePair<Proxy.ConnectionType, Proxy.IRelationship>> transclusionRelationships = Node.Relationships.FindRelationships(TransclusionRelationshipType);

            List<Proxy.IRelationship> transclusionRelationshipsToRemove = new List<Proxy.IRelationship>();

            foreach (KeyValuePair<Proxy.ConnectionType, Proxy.IRelationship> relationshipPair in transclusionRelationships)
            {
                IEnumerable<Proxy.INode> maps = relationshipPair.Value.Nodes.FindNodes(TransclusionMapConnectionType, MapNodeType);

                if (maps != null)
                {
                    foreach (Proxy.INode mapNode in maps)
                    {
                        if (mapNode.Id == Context.Id)
                        {
                            transclusionRelationshipsToRemove.Add(relationshipPair.Value);
                            break;
                        }
                    }
                }
            }

            foreach (Proxy.IRelationship relationship in transclusionRelationshipsToRemove)
            {
                relationship.Delete(ref chain);
                /// TODO: Need to move the following operation back into the relationship class.
                Proxy.IRelationshipManager relationships = Node.Relationships;
                relationships.Remove(relationship);
                response.Relationships.Add(relationship);
            }
            return response;
        }

        public event EventHandler<NodeOperationCompletedArgs> OperationCompleted;

        public void ExecuteOperation()
        {
            var chain = new TransactionFramework.TransactionChain();
            chain.TransactionCompleted += ChainOnTransactionCompleted;
            Response = OperationBuilder.BuildTransactionChain(ref chain);
            MapManager.ExecuteTransaction(chain);
        }

        private void ChainOnTransactionCompleted(object sender, CompleteTransactionCompletedEventArgs completeTransactionCompletedEventArgs)
        {
            if (OperationCompleted != null)
            {
                OperationCompleted(sender, new NodeOperationCompletedArgs
                {
                    Response = Response,
                    ViewModeNode = ViewModeNode
                });
            }
        }
    }
}
