using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NodeService
{
    public interface ISearchCondition
    {
        ConditionResult Evaluate(Node initialSearchNode, Node node, Relationship relationship, Descriptor descriptor);
    }
}
