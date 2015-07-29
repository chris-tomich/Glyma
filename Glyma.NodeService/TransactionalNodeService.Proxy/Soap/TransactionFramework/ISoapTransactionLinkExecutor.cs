using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TransactionalNodeService.Soap.TransactionFramework
{
    internal interface ISoapTransactionLinkExecutor
    {
        DependencyCollection Dependencies { get; }

        event EventHandler TransactionFailed;
        event EventHandler<Service.CompleteTransactionCompletedEventArgs> TransactionCompleted;

        void Reset();
        DependencyCollection UpdateDependencies();
        void ExecuteTransaction(Guid sessionId, Service.TransactionalMappingToolServiceClient serviceProxy);
        void ExecuteBulkTransactions(Guid sessionId, Service.TransactionalMappingToolServiceClient serviceProxy, ref Service.OPS bulkOperations);
        void ConsumeBulkOperationResponse(Service.BOR bulkOperationResponse);
    }
}
