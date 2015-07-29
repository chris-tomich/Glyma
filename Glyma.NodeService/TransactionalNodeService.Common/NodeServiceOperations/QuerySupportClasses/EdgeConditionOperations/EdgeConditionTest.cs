using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransactionalNodeService;
using System.Runtime.Serialization;

namespace TransactionalNodeService.Common
{
    [DataContract]
    public class EdgeConditionTest
    {
        public EdgeConditionTest()
        {
        }

        [DataMember]
        public bool IsIncluded
        {
            get;
            set;
        }

        [DataMember]
        public EdgeElementType EdgeElement
        {
            get;
            set;
        }

        [DataMember]
        public EdgeComponentType EdgeComponent
        {
            get;
            set;
        }

        [DataMember]
        public object ComponentValue
        {
            get;
            set;
        }
    }
}