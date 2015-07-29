using System;
using System.Collections.Generic;
using TransactionalNodeService.Service;
using Proxy = TransactionalNodeService.Proxy;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class BatchTranscludeNodesOperation : CommonOperationBase, ISuperGraphOperation
    {
        private Queue<TranscludeNodesOperation> _operations;
        private List<KeyValuePair<ViewModel.Node, ModelOperationType>> _nodes;

        public event EventHandler<NodeOperationCompletedArgs> OperationCompleted;

        public BatchTranscludeNodesOperation(Proxy.IMapManager mapManager)
            : base(mapManager)
        {
        }

        private Queue<TranscludeNodesOperation> Operations
        {
            get
            {
                if (_operations == null)
                {
                    _operations = new Queue<TranscludeNodesOperation>();
                }

                return _operations;
            }
        }

        public List<KeyValuePair<ViewModel.Node, ModelOperationType>> Nodes
        {
            get
            {
                if(_nodes == null)
                    _nodes = new List<KeyValuePair<ViewModel.Node, ModelOperationType>>();
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

        public void AddTranscludeNodeOperation(TranscludeNodesOperation operation)
        {
            Operations.Enqueue(operation);
        }

        public void ExecuteOperation()
        {
            Response = new TransactionFramework.InProcessTransactionResponse();

            TransactionFramework.TransactionChain chain = new TransactionFramework.TransactionChain();
            chain.TransactionCompleted += ChainOnTransactionCompleted;
            while (Operations.Count > 0)
            {
                TranscludeNodesOperation operation = Operations.Dequeue();

                if (chain.NumOfTransactions > 50)
                {
                    MapManager.ExecuteTransaction(chain);
                    chain = new TransactionFramework.TransactionChain();
                }
                TransactionFramework.InProcessTransactionResponse newResponse = operation.OperationBuilder.BuildTransactionChain(ref chain);

                Response.Add(newResponse);
            }

            MapManager.ExecuteTransaction(chain);
        }
    }
}
