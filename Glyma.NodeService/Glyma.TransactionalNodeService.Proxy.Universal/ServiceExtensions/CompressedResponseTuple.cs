using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TransactionalNodeService.Proxy.Universal.Service
{
    [DataContract(Name = "CompressedResponseTuple", Namespace = "http://schemas.datacontract.org/2004/07/TransactionalNodeService.Common.Model")]
    public class CompressedResponseTuple
    {
        [DataMember]
        public Dictionary<Guid, NO> Nodes;

        [DataMember]
        public Dictionary<Guid, RE> Relationships;
    }
}
