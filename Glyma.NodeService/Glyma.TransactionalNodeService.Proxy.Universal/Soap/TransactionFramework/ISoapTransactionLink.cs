namespace TransactionalNodeService.Proxy.Universal.Soap.TransactionFramework
{
    public interface ISoapTransactionLink
    {
        int TransactionLinkId { get; set; }
        Service.MP ResponseParameter { get; }
        Service.MapParameterType ResponseParameterType { get; }
        ServerStatus TransactionStatus { get; }
        TransactionFramework.TransactionChain OriginChain { get; set; }

        void AddNextLink(ISoapTransactionLink nextLink);
    }
}
