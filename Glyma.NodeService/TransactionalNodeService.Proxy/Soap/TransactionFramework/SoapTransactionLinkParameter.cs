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
    internal class SoapTransactionLinkParameter
    {
        private enum ParameterType
        {
            Null,
            Explicit,
            MapParameter
        }

        private ParameterType _parameterValueType;
        private Service.MapParameterType _parameterType;

        public SoapTransactionLinkParameter(Service.MapParameterType parameterType)
        {
            _parameterType = parameterType;
        }

        private Guid ExplicitParameterValue
        {
            get;
            set;
        }

        private ISoapTransactionLink ChainedTransactionLink
        {
            get;
            set;
        }

        private Service.MP BuildExplicitParameter(Guid sessionId)
        {
            Service.MP mapParameter = new Service.MP();
            mapParameter.I = Guid.NewGuid();
            mapParameter.D = false;
            mapParameter.T = _parameterType;
            mapParameter.S = sessionId;
            mapParameter.V = ExplicitParameterValue;

            return mapParameter;
        }

        public void SetParameterNull()
        {
            _parameterValueType = ParameterType.Null;
        }

        public void SetParameterValue(Guid parameterValue)
        {
            ExplicitParameterValue = parameterValue;
            _parameterValueType = ParameterType.Explicit;
        }

        public void SetParameterValue(ISoapTransactionLink transactionLink)
        {
            ChainedTransactionLink = transactionLink;
            _parameterValueType = ParameterType.MapParameter;
        }

        public Service.MP GetParameterValue(Guid sessionId)
        {
            switch (_parameterValueType)
            {
                case ParameterType.Null:
                    return null;
                case ParameterType.Explicit:
                    return BuildExplicitParameter(sessionId);
                case ParameterType.MapParameter:
                    return ChainedTransactionLink.ResponseParameter;
                default:
                    return new Service.MP();
            }
        }
    }
}
