using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TransactionalNodeService.Common.Model
{
    [DataContract(Name = "CompressedResponseTuple", Namespace = "http://schemas.datacontract.org/2004/07/TransactionalNodeService.Common.Model")]
    public class CompressedResponseTuple
    {
        [DataMember]
        public Dictionary<Guid, Node> Nodes;

        [DataMember]
        public Dictionary<Guid, Relationship> Relationships;
    }
}
