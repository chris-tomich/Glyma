using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Data;

namespace TransactionalNodeService.Model
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects", Name = "NT")]
    public class NodeType : MapTypeElement
    {
        public NodeType()
        {
        }

        [DataMember(Name = "I")]
        public override Guid Id
        {
            get;
            set;
        }

        [DataMember(Name = "N")]
        public override string Name
        {
            get;
            set;
        }

        public override void LoadSessionObject(IDataRecord record)
        {
            Id = (Guid)record["NodeTypeUid"];
            Name = (string)record["NodeTypeName"];
        }
    }
}