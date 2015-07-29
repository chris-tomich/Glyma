using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Data;

namespace TransactionalNodeService.Common.Model
{
    [DataContract(Namespace = "http://sevensigma.com.au/TransactionalNodeService/OpObjects")]
    public class MetadataType : MapTypeElement
    {
        public MetadataType()
        {
        }

        [DataMember]
        public override Guid Id
        {
            get;
            set;
        }

        [DataMember]
        public override string Name
        {
            get;
            set;
        }

        public override void LoadSessionObject(IDataRecord record)
        {
            Id = (Guid)record["MetadataTypeUid"];
            Name = (string)record["MetadataTypeName"];
        }
    }
}