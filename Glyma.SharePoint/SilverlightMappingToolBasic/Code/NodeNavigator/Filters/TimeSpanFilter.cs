using System;

using SilverlightMappingToolBasic.MappingService;

namespace SilverlightMappingToolBasic
{
    public class TimeSpanFilter : MetadataFilter
    {
        /// <summary>
        /// Compares if the nodes metadata value is ==, !=, >, >=, <, <=, to the value passed as a parameter.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value">The string representaion of a TimeSpan</param>
        /// <returns></returns>
        public override bool HasMatch(INodeProxy node)
        {
            bool result = false;
            if (!ContainsKey(node))
            {
                return false;
            }

            TimeSpan metaValue = TimeSpan.MinValue;
            TimeSpan compareValue = TimeSpan.MinValue;
            if (TimeSpan.TryParse(node.GetNodeMetadata(Key).MetadataValue, out metaValue) && TimeSpan.TryParse(Value, out compareValue))
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
