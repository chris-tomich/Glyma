using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NodeService
{
    /*
    public enum LogicalOperators
    {
        Not,
        Or,
        And
    }

    public class SearchLogicalCondition : ISearchCondition
    {
        public SearchLogicalCondition()
        {
        }

        public LogicalOperators Operator
        {
            get;
            set;
        }

        public List<ISearchCondition> Conditions
        {
            get;
            set;
        }

        #region ISearchCondition

        public bool Evaluate(Node node, Relationship relationship, Descriptor descriptor)
        {
            bool? evaluation = new bool?();

            if (Operator == LogicalOperators.Not)
            {
                Conditions.First().Evaluate(node, relationship, descriptor);
            }

            foreach (ISearchCondition condition in Conditions)
            {
                if (evaluation.HasValue)
                {
                    switch (Operator)
                    {
                        case LogicalOperators.And:
                            evaluation = evaluation.Value && condition.Evaluate(node, relationship, descriptor);
                            break;
                        case LogicalOperators.Or:
                            evaluation = evaluation.Value || condition.Evaluate(node, relationship, descriptor);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    evaluation = condition.Evaluate(node, relationship, descriptor);
                }
            }

            return evaluation.Value;
        }

        #endregion
     
    } */
}