using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

using Microsoft.SharePoint.Administration;

namespace SevenSigma.Logging
{
    [ServiceContract(Namespace = "http://sevensigma.com.au/Logging")]
    [ServiceKnownType(typeof(Category))]
    public interface ILoggingWCFService
    {
        [OperationContract]
        void WriteTrace(Category category, TraceSeverity traceSeverity, string message);
    }

    [DataContract]
    public enum Category
    {
        [EnumMember]
        ApplicationPage,
        [EnumMember]
        WCFService,
        [EnumMember]
        SilverlightApplication
    }
}
