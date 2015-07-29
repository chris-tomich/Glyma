namespace TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework
{
    public enum ServerStatus
    {
        ProcessingClient,
        SendingTransaction,
        TransactionReceived,
        TransactionExecuting,
        TransactionCompleted,
        TransactionFailed
    }
}
