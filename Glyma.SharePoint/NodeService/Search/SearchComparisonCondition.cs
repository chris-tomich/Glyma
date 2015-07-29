using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NodeService
{  

    public class SearchComparisonCondition //: ISearchCondition
    {
        public SearchComparisonCondition()
        {
        }

        public Guid SearchValue
        {
            get;
            set;
        }

        public ConditionContext Context
        {
            get;
            set;
        }

        public ComparisonOperators Operator
        {
            get;
            set;
        }

        #region ISearchCondition

        public bool Evaluate(Node node, Relationship relationship, Descriptor descriptor)
        {
            Guid searchId = Guid.Empty;
            bool? evaluation = new bool?();

            switch (Context)
            {
                case ConditionContext.Node:
                    searchId = node.NodeUid;
                    break;
                case ConditionContext.Relationship:
                    searchId = relationship.RelationshipUid;
                    break;
                case ConditionContext.Descriptor:
                    searchId = descriptor.DescriptorUid;
                    break;
                case ConditionContext.NodeType:
                    searchId = node.NodeTypeUid.Value;
                    break;
                case ConditionContext.RelationshipType:
                    searchId = relationship.RelationshipTypeUid.Value;
                    break;
                case ConditionContext.DescriptorType:
                    searchId = descriptor.DescriptorTypeUid.Value;
                    break;
                default:
                    break;
            }

            switch (Operator)
            {
                case ComparisonOperators.Equal:
                    evaluation = SearchValue == searchId;
                    break;
                case ComparisonOperators.NotEqual:
                    evaluation = SearchValue != searchId;
                    break;
                default:
                    evaluation = false;
                    break;
            }

            return evaluation.Value;
        }

        #endregion
    }
}