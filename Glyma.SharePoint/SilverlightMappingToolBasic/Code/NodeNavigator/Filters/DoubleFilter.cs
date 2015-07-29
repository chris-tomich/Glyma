using System;

using SilverlightMappingToolBasic.MappingService;

namespace SilverlightMappingToolBasic
{
    public class DoubleFilter : MetadataFilter
    {
        public override bool HasMatch(INodeProxy node)
        {
            bool result = false;
            if (!ContainsKey(node))
            {
                return false;
            }

            double metaValue = double.NaN;
            double compareValue = double.NaN;
            if (Double.TryParse(node.GetNodeMetadata(Key).MetadataValue, out metaValue) && Double.TryParse(Value, out compareValue))
            {
                switch (Operator)
                {
                    case MetadataFilterOperator.EQUAL:
                        if (metaValue.Equals(compareValue))
                        {
                            result = true;
                        }
                        break;
                    case MetadataFilterOperator.NOT_EQUAL:
                        if (!metaValue.Equals(compareValue))
                        {
                            result = true;
                        }
                        break;
                    case MetadataFilterOperator.GREATER_THAN:
                        if (metaValue > compareValue)
                        {
                            result = true;
                        }
                        break;
                    case MetadataFilterOperator.GREATER_THAN_OR_EQUAL:
                        if (metaValue >= compareValue)
                        {
                            result = true;
                        }
                        break;
                    case MetadataFilterOperator.LESS_THAN:
                        if (metaValue < compareValue)
                        {
                            result = true;
                        }
                        break;
                    case MetadataFilterOperator.LESS_THAN_OR_EQUAL:
                        if (metaValue <= compareValue)
                        {
                            result = true;
                        }
                        break;
                    default:
                        result = false;
                        break;
                }
            }
            return result;
        }
    }
}
