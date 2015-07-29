using System;
using System.Linq;
using TransactionalNodeService.Service;
using Proxy = TransactionalNodeService.Proxy;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class ConnectTranscludedNodesOperation : CommonOperationBase, ISuperGraphOperation, IBatchOperation
    {
        public ConnectTranscludedNodesOperation(Proxy.IMapManager mapManager)
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

        public Proxy.INode Context
        {
            get;
            set;
        }

        public Proxy.INode From
        {
            get;
            set;
        }

        public Proxy.INode To
        {
            get;
            set;
        }

        TransactionFramework.InProcessTransactionResponse IBatchOperation.BuildTransactionChain(ref TransactionFramework.TransactionChain chain)
        {
            var response = new TransactionFramework.InProcessTransactionResponse();
            Proxy.IRelationship newTransclusionRelationship = MapManager.CreateRelationship(From.DomainId, From.RootMapId.Value, TransclusionRelationshipType, string.Empty, ref chain);
            var relationships = Context.Relationships.FindRelationships(TransclusionRelationshipType);

            foreach (var keyValuePair in relationships)
            {
                var from = keyValuePair.Value.Nodes.FindNodes(FromConnectionType);
                var to = keyValuePair.Value.Nodes.FindNodes(ToConnectionType);

                if (from == null || to == null || From == null || To == null)
                {
                    continue;
                }

                ///TODO Chris: following lines causing Bug 71
                if (from.Contains(From) && to.Contains(To))
                {
                    return response;
                }
            }

            newTransclusionRelationship.ConnectNode(TransclusionMapConnectionType, Context, ref chain);
            newTransclusionRelationship.ConnectNode(FromConnectionType, From, ref chain);
            newTransclusionRelationship.ConnectNode(ToConnectionType, To, ref chain);

            

            response.Relationships.Add(newTransclusionRelationship);
            return response;
        }

        public event EventHandler<NodeOperationCompletedArgs> OperationCompleted;

        public void ExecuteOperation()
        {
            TransactionFramework.TransactionChain chain = new TransactionFramework.TransactionChain();
            Response = OperationBuilder.BuildTransactionChain(ref chain);
            chain.TransactionCompleted += ChainOnTransactionCompleted;
            MapManager.ExecuteTransaction(chain);
        }

        private void ChainOnTransactionCompleted(object sender, CompleteTransactionCompletedEventArgs completeTransactionCompletedEventArgs)
        {
            if (OperationCompleted != null)
            {
                OperationCompleted(sender, new NodeOperationCompletedArgs{Response = Response});
            }
        }
    }
}
