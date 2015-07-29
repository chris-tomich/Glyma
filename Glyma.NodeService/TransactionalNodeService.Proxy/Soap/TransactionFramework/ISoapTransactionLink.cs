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
using Service = TransactionalNodeService.Service;

namespace TransactionalNodeService.Soap.TransactionFramework
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
