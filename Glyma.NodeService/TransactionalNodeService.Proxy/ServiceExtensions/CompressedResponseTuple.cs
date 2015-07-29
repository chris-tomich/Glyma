using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TransactionalNodeService.Service
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
