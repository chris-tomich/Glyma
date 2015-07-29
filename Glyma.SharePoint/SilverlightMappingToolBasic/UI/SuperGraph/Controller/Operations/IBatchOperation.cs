using TransactionFramework = TransactionalNodeService.Soap.TransactionFramework;

namespace SilverlightMappingToolBasic.UI.SuperGraph.Controller.Operations
{
    internal interface IBatchOperation
    {
        TransactionFramework.InProcessTransactionResponse BuildTransactionChain(ref TransactionFramework.TransactionChain chain);
    }
}
