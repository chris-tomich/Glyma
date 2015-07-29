using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TransactionalNodeService.Common
{
    [DataContract]
    public class MetadataEdgeConditionTest : EdgeConditionTest
    {
        [DataMember]
        public string MetadataName
        {
            get;
            set;
        }
    }
}