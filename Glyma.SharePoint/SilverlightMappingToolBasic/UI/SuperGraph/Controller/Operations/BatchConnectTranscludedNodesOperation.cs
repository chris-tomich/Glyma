using System;
using System.Collections.Generic;
using TransactionalNodeService.Service;
using Proxy = TransactionalNodeService.Proxy;
using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public class BatchConnectTranscludedNodesOperation : CommonOperationBase, ISuperGraphOperation
    {
        private Queue<ConnectTranscludedNodesOperation> _operations;

        public BatchConnectTranscludedNodesOperation(Proxy.IMapManager mapManager)
            : base(mapManager)
        {
        }

        private Queue<ConnectTranscludedNodesOperation> Operations
        {
            get
            {
                if (_operations == null)
                {
                    _operations = new Queue<ConnectTranscludedNodesOperation>();
                }

                return _operations;
            }
        }

        public void AddConnectTranscludedNodesOperation(ConnectTranscludedNodesOperation operation)
        {
            Operations.Enqueue(operation);
        }

        public event EventHandler<NodeOperationCompletedArgs> OperationCompleted;

        public void ExecuteOperation()
        {
            Response = new TransactionFramework.InProcessTransactionResponse();

            TransactionFramework.TransactionChain chain = new TransactionFramework.TransactionChain();
            chain.TransactionCompleted += ChainOnTransactionCompleted;
            while (Operations.Count > 0)
            {
                ConnectTranscludedNodesOperation operation = Operations.Dequeue();

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

        private void ChainOnTransactionCompleted(object sender, CompleteTransactionCompletedEventArgs completeTransactionCompletedEventArgs)
        {
            if (OperationCompleted != null)
            {
                OperationCompleted(sender, new NodeOperationCompletedArgs{Response = Response});
            }
        }
    }
}
