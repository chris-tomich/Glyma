using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace TransactionalNodeService
{
    [DataContract]
    [KnownType(typeof(OrEdgeCondOp))]
    [KnownType(typeof(AndEdgeCondOp))]
    [KnownType(typeof(EqualEdgeCondOp))]
    [KnownType(typeof(NotEqualEdgeCondOp))]
    public class EdgeConditions
    {
        public EdgeConditions()
        {
        }

        [DataMember]
        public IEdgeCondOp EdgeCondition
        {
            get;
            set;
        }
    }
}