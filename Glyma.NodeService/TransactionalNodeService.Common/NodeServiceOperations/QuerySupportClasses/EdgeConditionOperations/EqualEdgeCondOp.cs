using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TransactionalNodeService.Common.Model;
using System.Runtime.Serialization;

namespace TransactionalNodeService.Common
{
    [DataContract]
    [KnownType(typeof(OrEdgeCondOp))]
    [KnownType(typeof(AndEdgeCondOp))]
    [KnownType(typeof(EqualEdgeCondOp))]
    [KnownType(typeof(NotEqualEdgeCondOp))]
    public class EqualEdgeCondOp : IEdgeCondOp
    {
        public EqualEdgeCondOp()
        {
        }

        [DataMember]
        public EdgeConditionTest Condition
        {
            get;
            set;
        }

        public EdgeResult EvaluateCondition(IMapElement mapElement)
        {
            if (Condition == null)
            {
                // As we weren't given a Condition we want this to be ignored so other results aren't dirtied.
                return new EdgeResult() { IsEdge = null, IsIncluded = false };
            }

            if (mapElement is Node && Condition.EdgeElement == EdgeElementType.Relationship)
            {
                // If the mapElement type doesn't match the condition type then ignore this condition.
                return new EdgeResult() { IsEdge = null, IsIncluded = false };
            }
            else if (mapElement is Relationship && Condition.EdgeElement == EdgeElementType.Node)
            {
                // If the mapElement type doesn't match the condition type then ignore this condition.
                return new EdgeResult() { IsEdge = null, IsIncluded = false };
            }
            else if (!(mapElement is Node) && !(mapElement is Relationship))
            {
                // If the mapElement isn't a known type then ignore this condition.
                return new EdgeResult() { IsEdge = null, IsIncluded = false };
            }

            bool isEdge = false;

            switch (Condition.EdgeComponent)
            {
                case EdgeComponentType.Id:
                    isEdge = mapElement.Id.Equals(Condition.ComponentValue);
                    break;

                case EdgeComponentType.OriginalId:
                    isEdge = mapElement.OriginalId.Equals(Condition.ComponentValue);
                    break;

                case EdgeComponentType.Type:
                    isEdge = mapElement.TypeId.Equals(Condition.ComponentValue);
                    break;

                case EdgeComponentType.Metadata:
                    MetadataEdgeConditionTest metadataCondition = Condition as MetadataEdgeConditionTest;

                    if (metadataCondition != null)
                    {
                        IEnumerable<KeyValuePair<MetadataContext, Metadata>> matchingMetadata = mapElement.Metadata.Where(item => item.Value.MetadataValue.Equals(Condition.ComponentValue));
                        isEdge = (matchingMetadata.Count() > 0);
                    }
                    else
                    {
                        isEdge = false;
                    }
                    break;

                default:
                    isEdge = false;
                    break;
            }

            return new EdgeResult() { IsEdge = isEdge, IsIncluded = Condition.IsIncluded };
        }
    }
}