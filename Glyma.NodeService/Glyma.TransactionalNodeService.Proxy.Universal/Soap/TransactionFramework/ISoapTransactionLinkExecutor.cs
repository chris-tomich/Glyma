using System;
using TransactionalNodeService.Proxy.Universal.Service;


namespace TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework
{
    internal interface ISoapTransactionLinkExecutor
    {
        DependencyCollection Dependencies { get; }

        event EventHandler TransactionFailed;
        event EventHandler<CompleteTransactionCompletedEventArgs> TransactionCompleted;

        void Reset();
        DependencyCollection UpdateDependencies();
        void ExecuteTransaction(Guid sessionId, TransactionalMappingToolServiceClient serviceProxy);
        void ExecuteBulkTransactions(Guid sessionId, TransactionalMappingToolServiceClient serviceProxy, ref Service.OPS bulkOperations);
        void ConsumeBulkOperationResponse(Service.BOR bulkOperationResponse);
    }
}
