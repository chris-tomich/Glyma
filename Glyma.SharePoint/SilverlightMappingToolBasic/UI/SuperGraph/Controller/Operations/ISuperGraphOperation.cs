using System;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    public interface ISuperGraphOperation
    {
        event EventHandler<NodeOperationCompletedArgs> OperationCompleted;
        void ExecuteOperation();

        TransactionalNodeService.Soap.TransactionFramework.InProcessTransactionResponse Response { get; }
    }
}
