using System;
using System.Collections.Generic;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;
using TransactionalNodeService.Service;
using Proxy = TransactionalNodeService.Proxy;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class BatchDeleteNodesOperation : CommonOperationBase, ISuperGraphOperation
    {
        private Queue<IBatchOperation> _operations;
        private List<KeyValuePair<Node, ModelOperationType>> _nodes;

        public event EventHandler<NodeOperationCompletedArgs> OperationCompleted;

        public BatchDeleteNodesOperation(Proxy.IMapManager mapManager)
            : base(mapManager)
        {
        }

        private Queue<IBatchOperation> Operations
        {
            get
            {
                if (_operations == null)
                {
                    _operations = new Queue<IBatchOperation>();
                }

                return _operations;
            }
        }

        public List<KeyValuePair<Node, ModelOperationType>> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new List<KeyValuePair<Node, ModelOperationType>>();
                }
                return _nodes;
            }
            set
            {
                _nodes = value;
            }
        }

        private void ChainOnTransactionCompleted(object sender, CompleteTransactionCompletedEventArgs completeTransactionCompletedEventArgs)
        {
            if (OperationCompleted != null)
            {
                OperationCompleted(sender, new NodeOperationCompletedArgs { Response = Response, Nodes = Nodes });
            }
        }

        public void AddDeleteNodeOperation(ISuperGraphOperation operation)
        {
            if (operation is IBatchOperation && (operation is DeleteTranscludedNodeOperation || operation is DeleteNodeOperation))
            {
                Operations.Enqueue((IBatchOperation)operation);
            }
            else
            {
                throw new NotSupportedException("This type of operation is not supported by the batch operation manager. Only delete operations are supported.");
            }
        }

        public void ExecuteOperation()
        {
            Response = new TransactionFramework.InProcessTransactionResponse();

            var chain = new TransactionFramework.TransactionChain();
            chain.TransactionCompleted += ChainOnTransactionCompleted;
            while (Operations.Count > 0)
            {
                IBatchOperation operation = Operations.Dequeue();

                TransactionFramework.InProcessTransactionResponse newResponse = operation.BuildTransactionChain(ref chain);

                Response.Add(newResponse);
            }
            MapManager.ExecuteTransaction(chain);
        }
    }
}
