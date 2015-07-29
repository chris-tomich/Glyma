using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransactionalNodeService.Model;
using System.Runtime.Serialization;

namespace TransactionalNodeService
{
    [DataContract]
    [KnownType(typeof(OrEdgeCondOp))]
    [KnownType(typeof(AndEdgeCondOp))]
    [KnownType(typeof(EqualEdgeCondOp))]
    [KnownType(typeof(NotEqualEdgeCondOp))]
    public class OrEdgeCondOp : IEdgeCondOp
    {
        private List<IEdgeCondOp> _conditionOperations = null;

        public OrEdgeCondOp()
        {
        }

        [DataMember]
        public List<IEdgeCondOp> ConditionOperations
        {
            get
            {
                if (_conditionOperations == null)
                {
                    _conditionOperations = new List<IEdgeCondOp>();
                }

                return _conditionOperations;
            }
            set
            {
                _conditionOperations = value;
            }
        }

        public EdgeResult EvaluateCondition(IMapElement mapElement)
        {
            bool isEdge = false;
            bool isIncluded = false;
            bool hasValidCondition = false;

            foreach (IEdgeCondOp conditionOperation in ConditionOperations)
            {
                EdgeResult evalResult = conditionOperation.EvaluateCondition(mapElement);

                /// If the operation we were given has a null Condition then we don't want to include this as it will dirty our result.
                if (evalResult.IsEdge.HasValue)
                {
                    hasValidCondition = true;
                    isEdge |= evalResult.IsEdge.Value;
                    isIncluded |= evalResult.IsIncluded;

                    /// This is just to squeeze a little more performance. If isEdge is false and isIncluded is true
                    /// then we can safely assume nothing will further change these values (consider the OR and AND
                    /// operators being used for each) and so we can end early.
                    if (isEdge && isIncluded)
                    {
                        break;
                    }
                }
            }

            if (hasValidCondition)
            {
                return new EdgeResult() { IsEdge = isEdge, IsIncluded = isIncluded };
            }
            else
            {
                return new EdgeResult() { IsEdge = null, IsIncluded = false };
            }
        }
    }
}