using TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework;

namespace TransactionalNodeService.Proxy.Universal.InProcess
{
    public interface IInProcess
    {
        ISoapTransactionLink OriginLink { get; }
    }
}
