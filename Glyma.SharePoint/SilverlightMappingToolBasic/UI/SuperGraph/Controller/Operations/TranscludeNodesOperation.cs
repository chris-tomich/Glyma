using System;
using System.Windows;
using TransactionalNodeService.Service;
using Proxy = TransactionalNodeService.Proxy;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;
using System.Collections.Generic;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class TranscludeNodesOperation : CommonOperationBase, ISuperGraphOperation, IBatchOperation
    {
        public TranscludeNodesOperation(Proxy.IMapManager mapManager)
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

        public Proxy.INode NewMap
        {
            get;
            set;
        }

        public Proxy.INode OriginalMap
        {
            get;
            set;
        }

        public ViewModel.Node ViewModelNode
        {
            get;
            set;
        }


        public Proxy.INode OriginalNode
        {
            get
            {
                return ViewModelNode.Proxy;
            }
        }

        public Point Location
        {
            get;
            set;
        }

        TransactionFramework.InProcessTransactionResponse IBatchOperation.BuildTransactionChain(ref TransactionFramework.TransactionChain chain)
        {
            var response = new TransactionFramework.InProcessTransactionResponse();

            if (NewMap.Id != OriginalMap.Id)
            {
                Proxy.IRelationship newTransclusionRelationship = MapManager.CreateRelationship(NewMap.DomainId, NewMap.RootMapId.Value, MapContainerRelationshipType, string.Empty, ref chain);

                newTransclusionRelationship.ConnectNode(FromConnectionType, OriginalNode, ref chain);
                newTransclusionRelationship.ConnectNode(ToConnectionType, NewMap, ref chain);

                OriginalNode.Metadata.Add(newTransclusionRelationship, FromConnectionType, "XPosition", Location.X.ToString(), ref chain);
                OriginalNode.Metadata.Add(newTransclusionRelationship, FromConnectionType, "YPosition", Location.Y.ToString(), ref chain);

                IEnumerable<KeyValuePair<Proxy.ConnectionType, Proxy.IRelationship>> relationships = OriginalNode.Relationships.FindRelationships(MapManager.RelationshipTypes["FromToRelationship"]);

                foreach (KeyValuePair<Proxy.ConnectionType, Proxy.IRelationship> relationshipPair in relationships)
                {
                    Proxy.IRelationship relationshipToUpdate = relationshipPair.Value;
                    relationshipToUpdate.Update(MapManager.RelationshipTypes["TransclusionFromToRelationship"], ref chain);
                    relationshipToUpdate.ConnectNode(MapManager.ConnectionTypes["TransclusionMap"], OriginalMap, ref chain);
                }


                response.Relationships.Add(newTransclusionRelationship);
            }
            return response;
        }

        public event EventHandler<NodeOperationCompletedArgs> OperationCompleted;

        public void ExecuteOperation()
        {
            TransactionFramework.TransactionChain chain = new TransactionFramework.TransactionChain();
            chain.TransactionCompleted += ChainOnTransactionCompleted;
            Response = OperationBuilder.BuildTransactionChain(ref chain);
            if (chain.NumOfTransactions > 0)
            {
                MapManager.ExecuteTransaction(chain);
            }
        }

        private void ChainOnTransactionCompleted(object sender, CompleteTransactionCompletedEventArgs completeTransactionCompletedEventArgs)
        {
            if (OperationCompleted != null)
            {
                OperationCompleted(sender, new NodeOperationCompletedArgs{Response = Response, ViewModeNode = ViewModelNode});
            }
        }
    }
}
