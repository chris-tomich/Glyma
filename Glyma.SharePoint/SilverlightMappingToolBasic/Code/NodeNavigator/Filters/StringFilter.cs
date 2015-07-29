using System;

using SilverlightMappingToolBasic.MappingService;

namespace SilverlightMappingToolBasic
{
    public class StringFilter : MetadataFilter
    {
        public override bool HasMatch(INodeProxy node)
        {
            bool result = false;
            string metaValue;
            if (!ContainsKey(node))
            {
                if (string.IsNullOrEmpty(Value))
                {
                    //if the metadata doesn't exist and the compare value is null or string.Empty
                    //set the values to both be string.Empty and allow the operator to work it out
                    metaValue = string.Empty;
                    Value = string.Empty;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                metaValue = node.GetNodeMetadata(Key).MetadataValue;
            }

            switch (Operator)
            {
                case MetadataFilterOperator.EQUAL:
                    if (metaValue == Value)
                    {
                        result = true;
                    }
                    break;
                case MetadataFilterOperator.NOT_EQUAL:
                    if (metaValue != Value)
                    {
                        result = true;
                    }
                    break;
                case MetadataFilterOperator.GREATER_THAN:
                    if (metaValue.CompareTo(Value) > 0)
                    {
                        result = true;
                    }
                    break;
                case MetadataFilterOperator.GREATER_THAN_OR_EQUAL:
                    if (metaValue.CompareTo(Value) == 0)
                    {
                        result = true;
                    }
                    break;
                case MetadataFilterOperator.LESS_THAN:
                    if (metaValue.CompareTo(Value) < 0)
                    {
                        result = true;
                    }
                    break;
                case MetadataFilterOperator.LESS_THAN_OR_EQUAL:
                    if (metaValue.CompareTo(Value) == 0)
                    {
                        result = true;
                    }
                    break;
                case MetadataFilterOperator.CONTAINS:
                    if (metaValue.IndexOf(Value, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        result = true;
                    }
                    break;
                default:
                    result = false;
                    break;
            }
            return result;
        }
    }
}
