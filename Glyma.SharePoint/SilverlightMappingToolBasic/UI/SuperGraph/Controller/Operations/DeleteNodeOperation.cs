using System;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using TransactionalNodeService.Service;
using Proxy = TransactionalNodeService.Proxy;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class DeleteNodeOperation : CommonOperationBase, ISuperGraphOperation, IBatchOperation
    {
        private readonly Node _viewModeNode;

        public DeleteNodeOperation(Node viewModeNode, Proxy.IMapManager mapManager)
            : base(mapManager)
        {
            _viewModeNode = viewModeNode;
        }

        internal IBatchOperation OperationBuilder
        {
            get
            {
                return this;
            }
        }

        public Proxy.INode Node
        {
            get
            {
                return _viewModeNode.Proxy;
            }
        }

        TransactionFramework.InProcessTransactionResponse IBatchOperation.BuildTransactionChain(ref TransactionFramework.TransactionChain chain)
        {
            Node.Delete(ref chain);

            var response = new TransactionFramework.InProcessTransactionResponse();

            response.Nodes.Add(Node);
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
                OperationCompleted(sender, new NodeOperationCompletedArgs { Response = Response, ViewModeNode = _viewModeNode });
            }
        }
    }
}
